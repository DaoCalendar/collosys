#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploaderService.DataLayer;
using ColloSys.FileUploaderService.RecordManager;
using ColloSys.FileUploaderService.RowCounter;
using NLog;

#endregion

namespace ColloSys.FileUploaderService.FileReader
{
    public abstract class FileReader<T> : IFileReader<T> where T : Entity, IFileUploadable, IUniqueKey, new()
    {
        #region ctor
        private const uint BatchSize = 1000;
        private readonly FileProcess _fileProcess;
        protected readonly Logger Log = LogManager.GetCurrentClassLogger();
        public IRecordCreator<T> RecordCreatorObj { get; private set; }
        public ICounter Counter { get; private set; }

        protected readonly IDbLayer DbLayer;

        public FileScheduler FileScheduler { get; private set; }

        protected FileReader(FileScheduler fileScheduler, IRecordCreator<T> recordCreator)
        {
            FileScheduler = fileScheduler;
            Counter = new ExcelRecordCounter();
            recordCreator.Init(FileScheduler, Counter);
            recordCreator.InitPreviousDayLiner(FileScheduler);
            _fileProcess = new FileProcess();
            RecordCreatorObj = recordCreator;
            DbLayer = new DbLayer();
        }

        #endregion

        #region batch processing

        public void ReadAndSaveBatch()
        {
            do
            {
                SaveNextBatch(GetNextBatch());
                _fileProcess.UpdateFileStatus(FileScheduler, ColloSysEnums.UploadStatus.ActInserting, Counter);
            } while (!RecordCreatorObj.EndOfFile());
        }

        private void SaveNextBatch(IList<T> list)
        {
            if (!list.Any()) return;

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
            do
            {
                if (RecordCreatorObj.EndOfFile()) break;
                T obj;
                var isRecordCreate = RecordCreatorObj.CreateRecord(out obj);
                if (isRecordCreate)
                {
                    obj.FileScheduler = FileScheduler;
                    ((IFileUploadable)obj).FileDate = FileScheduler.FileDate;
                    obj.FileRowNo = Counter.CurrentRow;
                    list.Add(obj);
                    RecordCreatorObj.TodayRecordList.AddEntity(obj);
                }

            } while (list.Count != BatchSize);

            Log.Info("GetNextBatch: Batch Create ");
            Counter.CalculateTotalRecord();
            return list;

        }

        public abstract bool PostProcessing();

        public void ProcessFile()
        {
            try
            {
                Log.Info("FileUpload: uploading file : " + FileScheduler.FileName + ", for date" +
                          FileScheduler.FileDate.ToShortDateString());
                _fileProcess.UpdateFileScheduler(FileScheduler, Counter, ColloSysEnums.UploadStatus.UploadStarted);
                _fileProcess.UpdateFileStatus(FileScheduler, ColloSysEnums.UploadStatus.UploadStarted, Counter);
                ReadAndSaveBatch();

                Log.Info(string.Format("BatchProcessing : PostProcessing Start"));
                _fileProcess.UpdateFileStatus(FileScheduler, ColloSysEnums.UploadStatus.PostProcessing, Counter);
                PostProcessing();

                Log.Info(string.Format("BatchProcessing : PostProcessing() Done"));
                _fileProcess.ComputeStatus(FileScheduler, Counter);
                _fileProcess.UpdateFileScheduler(FileScheduler, Counter, FileScheduler.UploadStatus);
                _fileProcess.UpdateFileStatus(FileScheduler, FileScheduler.UploadStatus, Counter);
            }
            catch (Exception exception)
            {
                FileScheduler.UploadStatus = ColloSysEnums.UploadStatus.Error;
                FileScheduler.StatusDescription = exception.Message;
                _fileProcess.UpdateFileScheduler(FileScheduler, Counter, ColloSysEnums.UploadStatus.Error);
                Log.Error(string.Format("FileUpload : Could not upload file {0}-error description-{1}",
                    FileScheduler.FileName, exception));
            }
        }




        #endregion
    }
}
