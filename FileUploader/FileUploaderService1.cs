#region references

using System;
using ColloSys.DataLayer.NhSetup;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploaderService.DataLayer;
using ColloSys.FileUploaderService.FileReader;
using ColloSys.Shared.ConfigSectionReader;
using NLog;

#endregion


namespace ColloSys.FileUploaderService
{
    public static class FileUploaderService1
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region nh init
        static FileUploaderService1()
        {
            try
            {
                var connString = ColloSysParam.WebParams.ConnectionString;
                Logger.Info(string.Format("FileUpload: Connection String : {0}", connString.ConnectionString));
                SessionManager.InitNhibernate(new NhInitParams
                {
                    ConnectionString = connString,
                    DbType = ConfiguredDbTypes.MsSql,
                    IsWeb = false
                });

                Logger.Info(string.Format("FileUpload: NhProf profiling is enabled"));
                HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            }
            catch (Exception ex)
            {
                Logger.Error("FileUploader : " + ex);
            }
        }
        #endregion

        #region upload files

        public static void UploadFiles()
        {
            IDbLayer dbLayer = new DataLayer.DbLayer();
            var file = dbLayer.GetNextFileForSchedule();
            if (file == null) return;
            AllFileUploader.UploadFile(file);
        }

        #endregion

        #region Reset Files

        public static void ResetFiles()
        {
            var db = new FileProcess();
            db.ResetFileStatus();
        }

        #endregion
    }
}
