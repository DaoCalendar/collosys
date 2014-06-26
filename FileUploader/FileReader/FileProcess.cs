using System;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploaderService.RowCounter;
using NHibernate;
using NLog;

namespace ColloSys.FileUploaderService.FileReader
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

                    fileScheduler.ErrorRows = rowCounter.ErrorRecords;
                    fileScheduler.ValidRows = rowCounter.ValidRecords;
                    fileScheduler.TotalRows = rowCounter.TotalRecords;
                    fileScheduler.EndDateTime = DateTime.Now;
                    fileScheduler.UploadStatus = uploadStatus;
                    fileScheduler.FileStatuss = null;
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

        public void ResetFileStatus()
        {
            try
            {
                using (var session = SessionManager.GetNewSession())
                {
                    using (var tx = session.BeginTransaction())
                    {
                        var fileSchedulerList = session.QueryOver<FileScheduler>()
                                                       .Where(c => c.StartDateTime <= DateTime.Now)
                                                       .And(c => (c.UploadStatus != ColloSysEnums.UploadStatus.Done) &&
                                                           (c.UploadStatus != ColloSysEnums.UploadStatus.DoneWithError) &&
                                                           (c.UploadStatus != ColloSysEnums.UploadStatus.Error) &&
                                                           (c.UploadStatus != ColloSysEnums.UploadStatus.UploadRequest))
                                                       .List();

                        foreach (var fileScheduler in fileSchedulerList)
                        {
                            fileScheduler.UploadStatus = ColloSysEnums.UploadStatus.UploadRequest;
                            session.SaveOrUpdate(fileScheduler);
                        }

                        _log.Fatal(string.Format("DB Layer - ResetStatus: Resetting status of {0} files.",
                                                  fileSchedulerList.Count));
                        tx.Commit();
                    }
                }
            }
            catch (HibernateException e)
            {
                _log.Error("ChangeStatus Exception : " + e);
            }
        }
    }
}
