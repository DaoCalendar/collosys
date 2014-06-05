using System;
using System.Configuration;
using ColloSys.DataLayer.NhSetup;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploader.FileReader;
using ColloSys.FileUploaderService.DbLayer;
using ColloSys.Shared.ConfigSectionReader;
using NLog;


namespace ColloSys.FileUploaderService
{
    public static class FileUploaderService1
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region nh init

        public static readonly ConnectionStringSettings ConnString;

        //  AllFileUploader _allFileUploader=new AllFileUploader();

        static FileUploaderService1()
        {
            try
            {
                Logger.Info(string.Format("getting constring here "));
                ConnString = ColloSysParam.WebParams.ConnectionString;
                Logger.Info(string.Format("FileUpload: Connection String : {0}", ConnString.ConnectionString));
                SessionManager.InitNhibernate(new NhInitParams
                {
                    ConnectionString = ConnString,
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
            #region

            //CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            //CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            //// if some file is uploading, dont start upload for another file
            //if (_fileUploading)
            //{
            //    Logger.Info("FileUpload: Waiting for other uploads to finish.");
            //    return;
            //}

            // get next file for uploading

            #endregion

            IDbLayer dbLayer = new DbLayer.DbLayer();
            var file = dbLayer.GetNextFileForSchedule();
             

            if (file != null)
            {
                AllFileUploader.UploadFile(file);
            }


            #region cmt

            //if (file == null)
            //{
            //    Logger.Info("FileUpload: No files scheduled for upload or waiting for dependent files to finish uploading.");
            //    return;
            //}

            //// upload the file - in case of error just log the error and leave
            //try
            //{
            //    _fileUploading = true;
            //    // change the status of the file to uploading
            //    file.UploadStatus = ColloSysEnums.UploadStatus.UploadStarted;
            //    file.StatusDescription = string.Empty;
            //    dbLayer.ChangeStatus(file);
            //    Logger.Info("FileUpload: uploading file : " + file.FileName + ", for date" + file.FileDate.ToShortDateString());

            //}
            //catch (Exception exception)
            //{
            //    file.UploadStatus = ColloSysEnums.UploadStatus.Error;
            //    file.StatusDescription = exception.Message;
            //    dbLayer.ChangeStatus(file);
            //    Logger.Error(string.Format("FileUpload : Could not upload file {0}", file.FileName) + exception);
            //}

            //// done with uploading the file, ready for next file
            //_fileUploading = false;
            //Logger.Info(string.Format("FileUpload : {0} is Upload with status : {1}", file.FileName, file.UploadStatus));

            #endregion
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
