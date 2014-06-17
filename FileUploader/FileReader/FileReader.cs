#region references

using System;
using System.Collections.Generic;
using System.IO;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploader.RowCounter;
using ColloSys.FileUploader.Utilities;
using ColloSys.FileUploaderService.RecordManager;
using NLog;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploaderService.FileReader
{
    public abstract class FileReader<T> : IFileReader<T> where T : class, IFileUploadable, new()
    {
        #region ctor

        private readonly FileProcess _fileProcess;
       
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        //public IRecord<T> _objRecord;
        
        private readonly uint _batchSize;
        public ICounter Counter { get; set; }
        public IExcelReader ExcelReader { get; private set; }
        public IRecord<T> ObjRecord { get; private set; }
        public FileScheduler _fs { get; private set; }

        protected FileReader(FileScheduler fileScheduler, IRecord<T> recordCreator)
        {

            _fs = fileScheduler;
            Counter = new ExcelRecordCounter();
            ExcelReader = SharedUtility.GetInstance(new FileInfo(_fs.FileDirectory + @"\" + _fs.FileName));
            recordCreator.Init(_fs, Counter, ExcelReader);
            _fileProcess = new FileProcess();
            ObjRecord = recordCreator;
            _batchSize = 500;
        }
        #endregion

        #region batch processing

        

        public void ReadAndSaveBatch()
        {
            for (var i = ExcelReader.CurrentRow;
                i < ExcelReader.TotalRows && (!ExcelReader.EndOfFile());
                i = i + _batchSize)
            {
                var list = GetNextBatch();
                SaveNextBatch(list);

                _fileProcess.UpdateFileStatus(_fs, ColloSysEnums.UploadStatus.ActInserting, Counter);
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
            for (var j = 0; j < _batchSize && (!ExcelReader.EndOfFile()); j++)
            {
                T obj;
                var isRecordCreate = ObjRecord.CreateRecord(_fs.FileDetail.FileMappings,out obj);
                if (isRecordCreate)
                {
                    obj.FileScheduler = _fs;
                    obj.FileDate = _fs.FileDate;
                    obj.FileRowNo = ExcelReader.CurrentRow;
                    list.Add(obj);
                }
                ExcelReader.NextRow();
            }
            return list;
        }

        public void ProcessFile()
        {
            try
            {
                _log.Info("FileUpload: uploading file : " + _fs.FileName + ", for date" +
                            _fs.FileDate.ToShortDateString());
                _fileProcess.UpdateFileScheduler(_fs, Counter, ColloSysEnums.UploadStatus.UploadStarted);
                _fileProcess.UpdateFileStatus(_fs, ColloSysEnums.UploadStatus.UploadStarted, Counter);
                ReadAndSaveBatch();

                _log.Info(string.Format("BatchProcessing : PostProcessing Start"));
                _fileProcess.UpdateFileStatus(_fs, ColloSysEnums.UploadStatus.PostProcessing, Counter);
                _fileProcess.PostProcesing();

                _log.Info(string.Format("BatchProcessing : PostProcessing() Done"));
                _fileProcess.ComputeStatus(_fs, Counter);
                _fileProcess.UpdateFileScheduler(_fs, Counter, _fs.UploadStatus);
                _fileProcess.UpdateFileStatus(_fs, _fs.UploadStatus, Counter);
            }
            catch (Exception exception)
            {
                _fs.UploadStatus = ColloSysEnums.UploadStatus.Error;
                _fs.StatusDescription = exception.Message;
                _fileProcess.UpdateFileScheduler(_fs, Counter, ColloSysEnums.UploadStatus.Error);
                _log.Error(string.Format("FileUpload : Could not upload file {0}-error description-{1}", _fs.FileName, exception));
            }
        }


        #endregion
    }
}
