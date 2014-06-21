#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploader.RowCounter;
using ColloSys.FileUploader.Utilities;
using ColloSys.FileUploaderService.DbLayer;
using ColloSys.FileUploaderService.RecordManager;
using ColloSys.FileUploaderService.Utilities;
using NLog;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploaderService.FileReader
{
    public abstract class FileReader<T> : IFileReader<T> where T : Entity, IFileUploadable, IUniqueKey, new()
    {
        #region ctor

        private readonly FileProcess _fileProcess;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        protected readonly IRecord<T> _objRecord;
        readonly IExcelReader _excelReader;
        private readonly uint _batchSize;
        protected readonly ICounter _counter;
        protected readonly MultiKeyEntityList<T> TodayRecordList;
        protected readonly IDbLayer _DbLayer;

        public ICounter Counter {
            get { return _counter; }
        }

        public IExcelReader ExcelReader
        {
            get { return _excelReader; } 
        }


        public IRecord<T> RecordCreatorObj
        {
            get { return _objRecord; }
        }

        public FileScheduler FileScheduler { get; private set; }

        protected FileReader(FileScheduler fileScheduler, IRecord<T> recordCreator)
        {
            FileScheduler = fileScheduler;
            _counter = new ExcelRecordCounter();
            _excelReader = SharedUtility.GetInstance(new FileInfo(FileScheduler.FileDirectory + @"\" + FileScheduler.FileName));
            recordCreator.Init(FileScheduler, _counter, _excelReader);
            recordCreator.InitPreviousDayLiner(FileScheduler);
            _fileProcess = new FileProcess();
            _objRecord = recordCreator;
            _batchSize = 500;
            TodayRecordList=new MultiKeyEntityList<T>();
            _DbLayer=new DbLayer.DbLayer();
        }
        #endregion

        #region batch processing

       

        public void ReadAndSaveBatch()
        {
            for (var i = _excelReader.CurrentRow;
                i < _excelReader.TotalRows && (!_excelReader.EndOfFile());
                i = i + _batchSize)
            {
                var list = GetNextBatch();
                SaveNextBatch(list);

                _fileProcess.UpdateFileStatus(FileScheduler, ColloSysEnums.UploadStatus.ActInserting, _counter);
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

        public IList<T> GetNextBatch()
        {
            var list = new List<T>();
            for (var j = 0; j < _batchSize && (!_excelReader.EndOfFile()); j++)
            {
                T obj;
                var isRecordCreate = _objRecord.CreateRecord(FileScheduler.FileDetail.FileMappings,out obj);
                if (isRecordCreate)
                {
                    obj.FileScheduler = FileScheduler;
                    ((IFileUploadable)obj).FileDate = FileScheduler.FileDate;
                    obj.FileRowNo = _excelReader.CurrentRow;
                    list.Add(obj);
                    TodayRecordList.AddEntity(obj);
                }
                _excelReader.NextRow();
            }
            return list;
        }

        public abstract bool PostProcessing();

        public void ProcessFile()
        {
            try
            {
                _log.Info("FileUpload: uploading file : " + FileScheduler.FileName + ", for date" +
                            FileScheduler.FileDate.ToShortDateString());
                _fileProcess.UpdateFileScheduler(FileScheduler, _counter, ColloSysEnums.UploadStatus.UploadStarted);
                _fileProcess.UpdateFileStatus(FileScheduler, ColloSysEnums.UploadStatus.UploadStarted, _counter);
                ReadAndSaveBatch();

                _log.Info(string.Format("BatchProcessing : PostProcessing Start"));
                _fileProcess.UpdateFileStatus(FileScheduler, ColloSysEnums.UploadStatus.PostProcessing, _counter);
                _fileProcess.PostProcesing();

                _log.Info(string.Format("BatchProcessing : PostProcessing() Done"));
                _fileProcess.ComputeStatus(FileScheduler, _counter);
                _fileProcess.UpdateFileScheduler(FileScheduler, _counter, FileScheduler.UploadStatus);
                _fileProcess.UpdateFileStatus(FileScheduler, FileScheduler.UploadStatus, _counter);
            }
            catch (Exception exception)
            {
                FileScheduler.UploadStatus = ColloSysEnums.UploadStatus.Error;
                FileScheduler.StatusDescription = exception.Message;
                _fileProcess.UpdateFileScheduler(FileScheduler, _counter, ColloSysEnums.UploadStatus.Error);
                _log.Error(string.Format("FileUpload : Could not upload file {0}-error description-{1}", FileScheduler.FileName, exception));
            }
        }


        #endregion
    }
}
