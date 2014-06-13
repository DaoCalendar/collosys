﻿#region references

using System;
using System.Collections.Generic;
using System.IO;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploader.AliasRecordCreator;
using ColloSys.FileUploader.FileReader;
using ColloSys.FileUploader.RecordCreator;
using ColloSys.FileUploader.RowCounter;
using ColloSys.FileUploader.Utilities;
using NLog;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploaderService.FileReader
{
    public abstract class FileReader<T> : IFileReader<T> where T : class, IFileUploadable, new()
    {
        #region ctor

        private readonly FileProcess _fileProcess;
        private readonly FileScheduler _fs;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IRecord<T> _objRecord;
        private readonly IExcelReader _excelReader;
        private readonly uint _batchSize;
        private readonly ICounter _counter;

        protected FileReader(IAliasRecordCreator<T> recordCreator)
        {
            
            _fs = recordCreator.FileScheduler;
           
            _fileProcess = new FileProcess();
            _counter = new ExcelRecordCounter();
            _excelReader = SharedUtility.GetInstance(new FileInfo(_fs.FileDirectory + @"\" + _fs.FileName));
            _objRecord = new RecordCreator<T>(recordCreator, _excelReader, _counter);
            _batchSize = 500;
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

               _fileProcess.UpdateFileStatus(_fs, ColloSysEnums.UploadStatus.ActInserting, _counter);
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

        public void ProcessFile()
        {
            try
            {
                _log.Info("FileUpload: uploading file : " + _fs.FileName + ", for date" +
                            _fs.FileDate.ToShortDateString());
                _fileProcess.UpdateFileScheduler(_fs, _counter, ColloSysEnums.UploadStatus.UploadStarted);
                _fileProcess.UpdateFileStatus(_fs, ColloSysEnums.UploadStatus.UploadStarted, _counter);
                ReadAndSaveBatch();

                _log.Info(string.Format("BatchProcessing : PostProcessing Start"));
                _fileProcess.UpdateFileStatus(_fs, ColloSysEnums.UploadStatus.PostProcessing, _counter);
                _fileProcess.PostProcesing();

                _log.Info(string.Format("BatchProcessing : PostProcessing() Done"));
                _fileProcess.ComputeStatus(_fs,_counter);
                _fileProcess.UpdateFileScheduler(_fs, _counter, _fs.UploadStatus);
                _fileProcess.UpdateFileStatus(_fs, _fs.UploadStatus, _counter);
            }
            catch (Exception exception)
            {
                _fs.UploadStatus = ColloSysEnums.UploadStatus.Error;
                _fs.StatusDescription = exception.Message;
                _fileProcess.UpdateFileScheduler(_fs, _counter, ColloSysEnums.UploadStatus.Error);
                _log.Error(string.Format("FileUpload : Could not upload file {0}-error description-{1}", _fs.FileName, exception) );
            }
        }

     

      
    }
}
