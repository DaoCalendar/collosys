#region references

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.FileUploadBuilder;
using ColloSys.Shared.ConfigSectionReader;
using ColloSys.UserInterface.Shared;
using Newtonsoft.Json;
using NLog;

#endregion

namespace AngularUI.FileUpload.filescheduler
{
    public static class FileUploadHelper
    {
        #region init

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static readonly FileDetailBuilder FileDetailBuilder = new FileDetailBuilder();
        private static readonly FileSchedulerBuilder FileSchedulerBuilder = new FileSchedulerBuilder();

        public static FileUploadViewModel InitFileInfo(FileUploadViewModel viewModel)
        {
            // first populate unscheduled files
            IList<ScheduledFiles> fileList = new List<ScheduledFiles>();

            var allFiles = FileDetailBuilder.OnSystemCategory(viewModel.SelectedSystem, viewModel.SelectedCategory);


            foreach (var fileDetail in allFiles)
            {
                for (var i = 0; i < fileDetail.FileCount; i++)
                {
                    fileList.Add(new ScheduledFiles
                    {
                        AliasName = fileDetail.AliasName,
                        FileName = fileDetail.FileName,
                        FileSize = 0,
                        FileStatus = ColloSysEnums.UploadStatus.Error,
                        IsScheduled = false,
                        HasError = false,
                        ErrorMessage = string.Empty,
                        FileType = fileDetail.FileType
                    });
                }
            }

            // then get scheduled files
            var scheduleList = FileSchedulerBuilder.OnSystemCategoryFileDate(viewModel.SelectedSystem,
                                                                             viewModel.SelectedCategory,
                                                                             viewModel.ScheduleDate).ToList();

            foreach (var fileScheduler in scheduleList)
            {
                var listelement = fileList.First(x => x.AliasName == fileScheduler.FileDetail.AliasName &&
                                                      x.IsScheduled == false);
                listelement.FileName = fileScheduler.FileName;
                listelement.FileSize = fileScheduler.FileSize;
                listelement.FileStatus = fileScheduler.UploadStatus;
                listelement.ScheduleDate = fileScheduler.StartDateTime;
                listelement.IsScheduled = true;
                listelement.HasError = false;
                listelement.ErrorMessage = string.Empty;
                listelement.FileType = fileScheduler.FileDetail.FileType;
            }

            Log.Info(string.Format("FileUpload: Total {0} files are already scheduled.", scheduleList.Count));
            viewModel.ScheduleInfo = fileList;
            return viewModel;
        }

        #endregion

        #region schedule file

        public static bool ScheduleFile(FileUploadViewModel scheduledFiles, ScheduledFiles schedulerInfo, string directory)
        {
            // get file details
            var fileDetails = FileDetailBuilder.OnAliasName(schedulerInfo.AliasName);
            if (fileDetails == null)
            {
                Log.Fatal("Scheduling files : Received null file details, should never happen.");
                throw new InvalidDataException("Database Issue. File Details must have details for alias : " + schedulerInfo.AliasName);
            }

            // check duplicate by name & size
            var checkdate = (fileDetails.Frequency == ColloSysEnums.FileFrequency.Daily)
                                ? DateTime.Today.AddDays(-10)
                                : (fileDetails.Frequency == ColloSysEnums.FileFrequency.Weekly
                                       ? DateTime.Today.AddDays(-28)
                                       : DateTime.Today.AddDays(-120));
            //FileScheduler queryobj = null;
            var file = new FileInfo(schedulerInfo.UploadPath);
            var wasScheduled = FileSchedulerBuilder.Count(schedulerInfo.AliasName, (ulong)file.Length, checkdate,
                                                          file.Name);
            if (wasScheduled > 0)
            {
                var checkcount = (fileDetails.Frequency == ColloSysEnums.FileFrequency.Daily) ? 10 : 5;
                schedulerInfo.HasError = true;
                schedulerInfo.ErrorMessage =
                    string.Format("warning: File with same name {0} & size was found in last {1} uploads." +
                                  "\n if thats conincidence, please rename the file and reupload.",
                                  file.Name, checkcount);
                return false;
            }

            //save the file
            directory += schedulerInfo.AliasName.ToString().ToUpperInvariant();
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception)
            {
                schedulerInfo.HasError = true;
                schedulerInfo.ErrorMessage = "Not able to create directory '" + directory + "' for writing.";
                Log.Error("Scheduling files : not able to access the directory." + directory);
                return false;
            }

            var path = Path.Combine(directory, DateTime.Now.ToString("yyyyMMdd_HHmmss_") + file.Name);
            Log.Info("Scheduling files : saving file - " + schedulerInfo.AliasName + ", filesize : "
                + Utilities.ByteSize(file.Length));
            file.MoveTo(path);
            schedulerInfo.UploadPath = file.FullName;
            schedulerInfo.FileName = file.Name;

            // immediate reason
            if (scheduledFiles.IsImmediate && scheduledFiles.ImmediateReason != null)
            {
                scheduledFiles.ImmediateReason = scheduledFiles.ImmediateReason.Length > 250
                        ? scheduledFiles.ImmediateReason.Substring(0, 250) : string.Empty;
            }

            // save file schedulers
            var fileschedule = CreateFileScheduler(directory, (ulong)file.Length,
                                    scheduledFiles, fileDetails, schedulerInfo.FileName);
            try
            {
                using (var session = SessionManager.GetNewSession())
                {
                    using (var tx = session.BeginTransaction())
                    {
                        session.SaveOrUpdate(fileschedule);
                        tx.Commit();
                    }
                }
                Log.Info("Scheduling files : file has been scheduled : " + schedulerInfo.FileName);
            }
            catch (Exception ex)
            {
                schedulerInfo.HasError = true;
                schedulerInfo.ErrorMessage = "Not able to schedule file: '" + schedulerInfo.FileName + "'.\n" + ex.Message;
                return false;
            }

            // add to file upload view model
            schedulerInfo.FileStatus = fileschedule.UploadStatus;
            schedulerInfo.ScheduleDate = fileschedule.StartDateTime;
            schedulerInfo.IsScheduled = true;
            schedulerInfo.HasError = false;
            schedulerInfo.ErrorMessage = string.Empty;

            return true;
        }

        private static FileScheduler CreateFileScheduler(string directory, ulong fileSize,
                                                        FileUploadViewModel scheduledFiles, FileDetail fileDetails,
                                                        string fileName)
        {
            //schedule the file
            var fileschedule = new FileScheduler
                {
                    FileDate = scheduledFiles.ScheduleDate,
                    FileDetail = fileDetails,
                    FileDirectory = directory,
                    FileName = fileName,
                    FileServer = "localhost",
                    FileSize = fileSize,
                    ImmediateReason = scheduledFiles.ImmediateReason,
                    IsImmediate = scheduledFiles.IsImmediate,
                    StartDateTime = (scheduledFiles.IsImmediate || DateTime.Now > ColloSysParam.WebParams.UploadStartTime)
                        ? DateTime.Now : ColloSysParam.WebParams.UploadStartTime,
                    UploadStatus = ColloSysEnums.UploadStatus.UploadRequest,
                    TotalRows = 0,
                    ValidRows = 0,
                    StatusDescription = "scheduled",
                    ScbSystems = fileDetails.ScbSystems,
                    Category = fileDetails.Category
                };

            // create file status
            var filestatus = new FileStatus
                {
                    UploadStatus = ColloSysEnums.UploadStatus.UploadRequest,
                    EntryDateTime = DateTime.Now,
                    TotalRows = 0,
                    ValidRows = 0,
                    UploadedRows = 0,
                    FileScheduler = fileschedule
                };

            // add cross reference
            fileschedule.FileStatuss.Add(filestatus);
            return fileschedule;
        }

        #endregion

        #region file reader

        public static MultipartFormDataStreamProvider GetMultipartProvider()
        {
            var uploadFolder = Path.GetTempPath();
            return new MultipartFormDataStreamProvider(uploadFolder);
        }

        // Extracts Request FormatData as a strongly typed model
        public static T GetFormData<T>(MultipartFormDataStreamProvider result) where T:class 
        {
            if (!result.FormData.HasKeys()) return null;
            var values = result.FormData.GetValues(0);
            if (values == null) return null;
            var unescapedFormData = Uri.UnescapeDataString(values.FirstOrDefault() ?? String.Empty);
            if (String.IsNullOrEmpty(unescapedFormData)) return null;
            return JsonConvert.DeserializeObject<T>(unescapedFormData);
        }

        public static string GetDeserializedFileName(MultipartFileData fileData)
        {
            var fileName = GetFileName(fileData);
            return JsonConvert.DeserializeObject(fileName).ToString();
        }

        public static string GetFileName(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.FileName;
        }

        public static FileInfo MoveToTemp(MultipartFormDataStreamProvider result)
        {
            var originalFileName = GetDeserializedFileName(result.FileData.First());
            var uploadedFileInfo = new FileInfo(result.FileData.First().LocalFileName);
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
            var folder = Directory.CreateDirectory(Path.GetTempPath() + @"\" + timestamp);
            if(folder.Exists) folder.Create();
            var filename = folder.FullName + @"\" + originalFileName;
            uploadedFileInfo.MoveTo(filename);
            return uploadedFileInfo;
        }

        #endregion
    }
}