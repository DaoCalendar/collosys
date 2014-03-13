#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.FileUploadBuilder;
using ColloSys.Shared.ConfigSectionReader;
using ColloSys.UserInterface.Shared;
using NHibernate.Linq;
using NLog;

#endregion

namespace ColloSys.UserInterface.Areas.FileUploader.Models
{
    public static class FileUploadHelper
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static readonly FileDetailBuilder FileDetailBuilder=new FileDetailBuilder();
        private static readonly FileSchedulerBuilder FileSchedulerBuilder=new FileSchedulerBuilder();
        #region init

        public static FileUploadViewModel InitFileInfo(FileUploadViewModel viewModel)
        {
            var session = SessionManager.GetCurrentSession();

            // first populate unscheduled files
            IList<ScheduledFiles> fileList = new List<ScheduledFiles>();

            var allFiles = FileDetailBuilder.OnSystemCategory(viewModel.SelectedSystem, viewModel.SelectedCategory);


            foreach (var fileDetail in allFiles)
            {
                var aliasName = Enum.GetName(typeof(ColloSysEnums.FileAliasName), fileDetail.AliasName);
                for (var i = 0; i < fileDetail.FileCount; i++)
                {
                    fileList.Add(new ScheduledFiles
                    {
                        AliasName = i + "_" + aliasName,
                        FileName = fileDetail.FileName,
                        FileSize = 0,
                        FileStatus = ColloSysEnums.UploadStatus.Error,
                        IsScheduled = false,
                        HasError = false,
                        ErrorMessage = string.Empty

                    });
                }
            }

            // then get scheduled files
            var scheduleList = FileSchedulerBuilder.OnSystemCategoryFileDate(viewModel.SelectedSystem,
                                                                             viewModel.SelectedCategory,
                                                                             viewModel.ScheduleDate).ToList();

            foreach (var fileScheduler in scheduleList)
            {
                var aliasName = Enum.GetName(typeof(ColloSysEnums.FileAliasName), fileScheduler.FileDetail.AliasName);
                var listelement = fileList.First(x =>
                                                 x.AliasName.Substring(
                                                     x.AliasName.IndexOf("_", StringComparison.Ordinal) + 1) ==
                                                 aliasName &&
                                                 x.IsScheduled == false);
                listelement.FileName = fileScheduler.FileName;
                listelement.FileSize = fileScheduler.FileSize;
                listelement.FileStatus = fileScheduler.UploadStatus;
                listelement.ScheduleDate = fileScheduler.StartDateTime;
                listelement.IsScheduled = true;
                listelement.HasError = false;
                listelement.ErrorMessage = string.Empty;
            }

            Log.Info(string.Format("FileUpload: Total {0} files are already scheduled.", scheduleList.Count));
            viewModel.ScheduleInfo = fileList;
            return viewModel;
        }

        #endregion

        #region schedule file

        public static bool ScheduleFile(string requestFile, string aliasName, string directory,
                    HttpPostedFileBase file, FileUploadViewModel scheduledFiles)
        {
            //get alias name
            if (string.IsNullOrWhiteSpace(aliasName))
            {
                return false;
            }
            var fileAliasName = (ColloSysEnums.FileAliasName)
                        Enum.Parse(typeof(ColloSysEnums.FileAliasName), aliasName, true);

            // get file details
            var fileDetails = FileDetailBuilder.OnAliasName(fileAliasName);
            if (fileDetails == null)
            {
                Log.Fatal("Scheduling files : Received null file details, should never happen.");
                throw new InvalidDataException("Database Issue. File Details must have details for alias : " + aliasName);
            }

            //get the file
            var sfile = scheduledFiles.ScheduleInfo.First(x => x.AliasName == requestFile);

            // check extension
            var fileName = Path.GetFileName(file.FileName);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            ColloSysEnums.FileType fileType;
            var currentExtension = Path.GetExtension(fileName);
            if (currentExtension != null) currentExtension = currentExtension.Replace(".", "");

            if ((!Enum.TryParse(currentExtension, true, out fileType)) || (fileDetails.FileType != fileType))
            {
                var expectedExtension = Enum.GetName(typeof(ColloSysEnums.FileType), fileDetails.FileType);
                sfile.HasError = true;
                sfile.ErrorMessage = "Please provide file with '" + expectedExtension + "' extensions only.";
                return false;
            }

            //check duplicate
            var isduplicate = scheduledFiles.ScheduleInfo.Any(f =>
                                                              (!string.IsNullOrWhiteSpace(f.FileName)) &&
                                                              (f.IsScheduled) &&
                                                              (f.FileName.Substring(16) == file.FileName));
            if (isduplicate)
            {
                sfile.HasError = true;
                sfile.ErrorMessage = string.Format("File with name {0} is already scheduled.", file.FileName);
                return false;
            }

            // check duplicate by name & size
            var checkdate = (fileDetails.Frequency == ColloSysEnums.FileFrequency.Daily)
                                ? DateTime.Today.AddDays(-10)
                                : (fileDetails.Frequency == ColloSysEnums.FileFrequency.Weekly
                                       ? DateTime.Today.AddDays(-28)
                                       : DateTime.Today.AddDays(-120));
            //FileScheduler queryobj = null;
            var wasScheduled = FileSchedulerBuilder.Count(fileAliasName, (ulong) file.ContentLength, checkdate,
                                                          file.FileName);
            if (wasScheduled > 0)
            {
                var checkcount = (fileDetails.Frequency == ColloSysEnums.FileFrequency.Daily) ? 10 : 5;
                sfile.HasError = true;
                sfile.ErrorMessage =
                    string.Format("warning: File with same name {0} & size was found in last {1} uploads." +
                                  "\n if thats conincidence, please rename the file and reupload.",
                                  file.FileName, checkcount);
                return false;
            }

            //save the file
            directory += aliasName.ToUpperInvariant();
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception)
            {
                sfile.HasError = true;
                sfile.ErrorMessage = "Not able to create directory '" + directory + "' for writing.";
                Log.Error("Scheduling files : not able to access the directory." + directory);
                return false;
            }

            fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss_") + fileName;
            var path = Path.Combine(directory, fileName);

            Log.Info("Scheduling files : saving file - " + aliasName + ", filesize : "
                + Utilities.ByteSize(file.ContentLength));
            file.SaveAs(path);

            var checkFileInfo = new FileInfo(path);
            if (!checkFileInfo.Exists || checkFileInfo.Length != file.ContentLength)
            {
                sfile.HasError = true;
                sfile.ErrorMessage = "Not able to save files to '" + directory + "'.";
                Log.Fatal("Scheduling files : file has been moved from its location : " + fileName);
                return false;
            }

            // immediate reason
            if (scheduledFiles.IsImmediate && scheduledFiles.ImmediateReason != null)
            {
                scheduledFiles.ImmediateReason = scheduledFiles.ImmediateReason.Length > 250
                        ? scheduledFiles.ImmediateReason.Substring(0, 250) : string.Empty;
            }

            // save file schedulers
            var fileschedule = CreateFileScheduler(directory, (ulong)file.ContentLength,
                                    scheduledFiles, fileDetails, fileName);
            try
            {
                SessionManager.GetCurrentSession().SaveOrUpdate(fileschedule);
                Log.Info("Scheduling files : file has been scheduled : " + fileName);
            }
            catch (Exception ex)
            {
                sfile.HasError = true;
                sfile.ErrorMessage = "Not able to schedule file: '" + fileName + "'.\n" + ex.Message;
                return false;
            }

            // add to file upload view model
            sfile.AliasName = aliasName;
            sfile.FileName = fileschedule.FileName;
            sfile.FileSize = (ulong)file.ContentLength;
            sfile.FileStatus = fileschedule.UploadStatus;
            sfile.ScheduleDate = fileschedule.StartDateTime;
            sfile.IsScheduled = true;
            sfile.HasError = false;
            sfile.ErrorMessage = string.Empty;

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
    }

    #region supporting structures

    public class FileUploadViewModel
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public ScbEnums.ScbSystems SelectedSystem { get; set; }
        public ScbEnums.Category SelectedCategory { get; set; }
        public DateTime ScheduleDate { get; set; }
        public bool IsImmediate { get; set; }
        public string ImmediateReason { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global

        public IList<ScheduledFiles> ScheduleInfo { get; set; }
    }

    public class ScheduledFiles
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string AliasName { get; set; }
        public bool IsScheduled { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string FileName { get; set; }
        public UInt64 FileSize { get; set; }
        public ColloSysEnums.UploadStatus FileStatus { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }

    #endregion
}