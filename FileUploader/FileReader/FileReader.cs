#region references

using System;
using System.Collections.Generic;
using System.IO;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.RecordCreator;
using ColloSys.FileUploader.RowCounter;
using ColloSys.FileUploader.Utilities;
using NLog;
using ReflectionExtension.ExcelReader;
using LogManager = NLog.LogManager;

#endregion

namespace ColloSys.FileUploader.FileReader
{
    public abstract class FileReader<T> : IFileReader<T> where T : class, IFileUploadable, new()
    {
        #region ctor
        private readonly IRecord<T> _objRecord;
        private readonly IExcelReader _excelReader;
        private readonly uint _batchSize;
        private readonly FileScheduler _fs;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ICounter _counter;

        protected FileReader(IAliasRecordCreator<T> recordCreator)
        {
            _fs = recordCreator.FileScheduler;
            _counter = new ExcelRecordCounter();
            _excelReader = SharedUtility.GetInstance(new FileInfo(_fs.FileDirectory + @"\" + _fs.FileName));
            _objRecord = new RecordCreator<T>(recordCreator, _excelReader, _counter);
            _batchSize = 500;
        }
        #endregion

        #region batch processing
        public void ReadAndSaveBatch()
        {
            for (var I = _excelReader.CurrentRow; I < _excelReader.TotalRows && (!_excelReader.EndOfFile()); I = I + _batchSize)
            {
                var list = GetNextBatch();
                SaveNextBatch(list);

                UpdateFileStatus(_fs, ColloSysEnums.UploadStatus.ActInserting, _counter);
            }
        }

        private void SaveNextBatch(IEnumerable<T> list)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    foreach (var record in list)
                    {
                        session.Save(record);
                    }

                    transaction.Commit();
                }
            }
        }

        private IEnumerable<T> GetNextBatch()
        {
            var list = new List<T>();
            for (var j = 0; j < _batchSize && (!_excelReader.EndOfFile()); j++)
            {
                var obj = new T();
                var isRecordCreate = _objRecord.CreateRecord(obj, _fs.FileDetail.FileMappings, _counter);
                if (isRecordCreate)
                {
                    obj.FileScheduler = _fs;
                    obj.FileDate = _fs.FileDate;
                    obj.FileRowNo = _excelReader.CurrentRow;
                    list.Add(obj);
                }
                _excelReader.NextRow();
            }
            return list;
        }
        #endregion

        public void Save()
        {
            try
            {
                _log.Info("FileUpload: uploading file : " + _fs.FileName + ", for date" +
                            _fs.FileDate.ToShortDateString());
                UpdateFileScheduler(_fs, _counter, ColloSysEnums.UploadStatus.UploadStarted);

                ReadAndSaveBatch();

                _log.Info(string.Format("BatchProcessing : PostProcessing Start"));
                UpdateFileStatus(_fs, ColloSysEnums.UploadStatus.PostProcessing, _counter);
                PostProcesing();

                _log.Info(string.Format("BatchProcessing : PostProcessing() Done"));
                ComputeStatus(_fs);
                UpdateFileScheduler(_fs, _counter, _fs.UploadStatus);
            }
            catch (Exception exception)
            {
                _fs.UploadStatus = ColloSysEnums.UploadStatus.Error;
                _fs.StatusDescription = exception.Message;
                UpdateFileScheduler(_fs, _counter, ColloSysEnums.UploadStatus.Error);
                _log.Error(string.Format("FileUpload : Could not upload file {0}", _fs.FileName) + exception);
            }
        }

        #region status updates
        private void UpdateFileStatus(FileScheduler fileScheduler, ColloSysEnums.UploadStatus uploadStatus, ICounter rowCounter)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    _log.Info(string.Format("ReadFile: updating fileschduler-status table. " +
                                               "rows uploaded {0}. status {1}",
                                               fileScheduler.TotalRows,
                                               fileScheduler.UploadStatus));

                    var status = new FileStatus
                    {
                        EntryDateTime = DateTime.Now,
                        FileScheduler = fileScheduler,
                        TotalRows = rowCounter.TotalRecords,
                        ValidRows = rowCounter.ValidRecords,
                        UploadedRows = rowCounter.InsertRecord,
                        DuplicateRows = rowCounter.Duplicate,
                        IgnoredRows = rowCounter.IgnoreRecord,
                        ErrorRows = rowCounter.ErrorRecords,
                        UploadStatus = uploadStatus
                    };

                    session.SaveOrUpdate(status);
                    tx.Commit();
                }
            }
        }

        private void ComputeStatus(FileScheduler fileScheduler)
        {
            if (fileScheduler.ValidRows <= 0)
            {
                fileScheduler.UploadStatus = ColloSysEnums.UploadStatus.Error;
                fileScheduler.StatusDescription = "0 Valid Rows";
            }
            else if (fileScheduler.ErrorRows > 0)
            {
                fileScheduler.UploadStatus = ColloSysEnums.UploadStatus.DoneWithError;
            }
            else
            {
                fileScheduler.UploadStatus = ColloSysEnums.UploadStatus.Done;
            }
        }

        private void UpdateFileScheduler(FileScheduler fileScheduler, ICounter rowCounter, ColloSysEnums.UploadStatus uploadStatus)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.Refresh(fileScheduler);

                    fileScheduler.ErrorRows = rowCounter.ErrorRecords;
                    fileScheduler.ValidRows = rowCounter.ValidRecords;
                    fileScheduler.TotalRows = rowCounter.TotalRecords;
                    fileScheduler.EndDateTime = DateTime.Now;
                    fileScheduler.UploadStatus = uploadStatus;

                    _log.Info(string.Format("ReadFile: updating fileschduler-status table. " +
                                               "rows uploaded {0}. status {1}",
                                               fileScheduler.TotalRows,
                                               fileScheduler.UploadStatus));

                    var status = new FileStatus
                        {
                            EntryDateTime = DateTime.Now,
                            FileScheduler = fileScheduler,
                            TotalRows = rowCounter.TotalRecords,
                            ValidRows = rowCounter.ValidRecords,
                            UploadedRows = rowCounter.InsertRecord,
                            DuplicateRows = rowCounter.Duplicate,
                            ErrorRows = rowCounter.ErrorRecords,
                            IgnoredRows = rowCounter.IgnoreRecord,
                            UploadStatus = fileScheduler.UploadStatus,
                        };
                    fileScheduler.FileStatuss.Add(status);

                    session.SaveOrUpdate(fileScheduler);
                    tx.Commit();
                }
            }
        }
        #endregion

        protected virtual bool PostProcesing()
        {
            return true;
        }
    }
}
