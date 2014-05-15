using System;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploader.RowCounter;
using NLog;

namespace ColloSys.FileUploader.FileReader
{
    public class FileProcess
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        
        #region status updates
        public void UpdateFileStatus(FileScheduler fileScheduler, ColloSysEnums.UploadStatus uploadStatus, ICounter rowCounter)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    

                    //_log.Info(string.Format("ReadFile: updating fileschduler-status table. " +
                    //                        "rows uploaded {0}. status {1}",
                    //    fileScheduler.TotalRows,
                    //    fileScheduler.UploadStatus));

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

        public void ComputeStatus(FileScheduler fileScheduler,ICounter counter)
        {
            if (counter.ValidRecords <= 0)
            {
                fileScheduler.UploadStatus = ColloSysEnums.UploadStatus.Error;
                fileScheduler.StatusDescription = "0 Valid Rows";
            }
            else if (counter.ErrorRecords > 0)
            {
                fileScheduler.UploadStatus = ColloSysEnums.UploadStatus.DoneWithError;
            }
            else
            {
                fileScheduler.UploadStatus = ColloSysEnums.UploadStatus.Done;
            }
        }

        public void UpdateFileScheduler(FileScheduler fileScheduler, ICounter rowCounter, ColloSysEnums.UploadStatus uploadStatus)
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
                        ErrorRows = rowCounter.ErrorRecords,
                        IgnoredRows = rowCounter.IgnoreRecord,
                        UploadStatus = uploadStatus,
                    };
                    fileScheduler.FileStatuss.Add(status);

                    fileScheduler.ErrorRows = rowCounter.ErrorRecords;
                    fileScheduler.ValidRows = rowCounter.ValidRecords;
                    fileScheduler.TotalRows = rowCounter.TotalRecords;
                    fileScheduler.EndDateTime = DateTime.Now;
                    fileScheduler.UploadStatus = uploadStatus;
                    session.SaveOrUpdate(fileScheduler);
                    tx.Commit();
                }
            }
        }
        #endregion
        public virtual bool PostProcesing()
        {
            return true;
        }
    }
}
