using System;
using System.Collections.Generic;
using System.IO;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.DbLayer;
using ColloSys.FileUploader.RecordCreator;
using ColloSys.FileUploader.Utilities;
using ColloSys.FileUploadService.Logging;
using  ColloSys.Shared.ConfigSectionReader;
using NLog;
using ReflectionExtension.ExcelReader;
using LogManager = NLog.LogManager;

namespace ColloSys.FileUploader.FileReader
{
    public abstract class FileReader<T> : IFileReader<T> where T : class,new()
    {
        #region:: Members ::

        private readonly IRecord<T> _objRecord;
       
        public IList<T> List { get; private set; }
        private readonly IExcelReader _excelReader;
        private readonly uint _batchSize;
        private readonly FileScheduler _fs;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        //private idb _dbLayer;

        protected FileReader(IAliasRecordCreator<T> recordCreator)
        {
             _fs = recordCreator.FileScheduler;
            string path = _fs.FileDirectory  +@"\";
            
            NLogConfig.InitConFig(ColloSysParam.WebParams.LogPath, ColloSysParam.WebParams.LogLevel);

            _excelReader = SharedUtility.GetInstance(new FileInfo(path+_fs.FileName));
            _objRecord = new RecordCreator<T>(recordCreator, _excelReader);
            _batchSize = 500;
        }
        #endregion

        public void ReadAndSaveBatch()
        {
            for ( var I = _excelReader.CurrentRow; I < _excelReader.TotalRows && (!_excelReader.EndOfFile()); I = I + _batchSize)
            {
                List = new List<T>();
                for (var j = 0; j < _batchSize && (!_excelReader.EndOfFile()); j++)
                {
                    var obj = new T();
                    var isRecordCreate = _objRecord.CreateRecord(obj, _fs.FileDetail.FileMappings);
                    if (isRecordCreate)
                    {
                        List.Add(obj);
                    }
                    _excelReader.NextRow();
                }
                _log.Info("Batch object bind success");
                using (var session = SessionManager.GetNewSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        foreach (var record in List)
                        {
                            session.Save(record);
                            //_log.Error(string.Format("Recored could not inserted {0}", record));
                        }

                        transaction.Commit();
                        _log.Debug("In Save Batch to DB");
                    }
                }
                //_log.Info(string.Format("BatchProcessing : PostProcessing Start"));
                //_fs.UploadStatus = ColloSysEnums.UploadStatus.PostProcessing;
                //_dbLayer.ChangeStatus(UploadedFile);
                //ReaderNeeds.PostProcesing();
                //_log.Info(string.Format("BatchProcessing : PostProcessing() Done"));

                //_log.Info("ReadFile: Retry error record.");
                //ReaderNeeds.RetryErrorRows();

                //_log.Info("ReadFile: saving the error table.");
                //SaveDoneStatus();


            }
        }

        public void Save(ICounter counter)
        {
            ReadAndSaveBatch();

            //Add Save error table here
            _log.Info(string.Format("BatchProcessing : SaveErrorTable() Done."));

            _log.Info(string.Format("BatchProcessing : PostProcessing Start"));
            ChangeStatus(_fs,ColloSysEnums.UploadStatus.PostProcessing);
            //ReaderNeeds.PostProcesing();
            _log.Info(string.Format("BatchProcessing : PostProcessing() Done"));

            _log.Info("ReadFile: Retry error record.");
           // ReaderNeeds.RetryErrorRows();

            _log.Info("ReadFile: saving the error table.");
           
            ChangeStatus(_fs,ColloSysEnums.UploadStatus.Done); // make if condition for status done or donrwith erroe

        }

        private void ChangeStatus(FileScheduler fileScheduler ,ColloSysEnums.UploadStatus uploadStatus)
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
                        TotalRows = fileScheduler.TotalRows,
                        ValidRows = fileScheduler.ValidRows,
                        UploadedRows = 0,
                        DuplicateRows = 0,
                        IgnoredRows = 0,
                        ErrorRows = 0,
                        UploadStatus = fileScheduler.UploadStatus
                    };

                    session.SaveOrUpdate(status);
                    tx.Commit();
                }
            }
        }
    }
}
