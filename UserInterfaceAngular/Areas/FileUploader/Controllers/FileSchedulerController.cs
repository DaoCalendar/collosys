#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using ColloSys.Shared.ConfigSectionReader;
using ColloSys.Shared.SharedUtils;
using ColloSys.UserInterface.Areas.FileUploader.Models;
using ColloSys.UserInterface.Controllers;
using ColloSys.UserInterface.Shared.Attributes;
using NLog;
using UserInterfaceAngular.Filters;

#endregion

namespace ColloSys.UserInterface.Areas.FileUploader.Controllers
{
    public class FileSchedulerController : BaseController
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.FileUploader)]
        public ActionResult Index()
        {
            _log.Debug("Rendering FileUploader Index action, FileUploader view.");
            return View("FileScheduler");
        }

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.FileUploader)]
        [MvcTransaction]
        public JsonResult GetFileStatus(ScbEnums.ScbSystems isystem, ScbEnums.Category icategory, DateTime idate)
        {
            var viewModel = new FileUploadViewModel
                {
                    SelectedSystem = isystem,
                    SelectedCategory = icategory,
                    ScheduleDate = idate
                };

            viewModel = FileUploadHelper.InitFileInfo(viewModel);
            _log.Debug("Retrieved file stauses.");
            return Json(viewModel.ScheduleInfo, "text/plain", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [UserActivity(Activity = ColloSysEnums.Activities.FileUploader)]
        [MvcTransaction]
        public JsonResult Upload(FileUploadViewModel scheduledFiles)
        {
            // init schedule info from db, just to be safe ;)
            scheduledFiles = FileUploadHelper.InitFileInfo(scheduledFiles);
            var duplicateName = GetDuplicateFileNames();
            _log.Info("File Upload path is: " + ColloSysParam.WebParams.UploadPath);

            foreach (string requestFile in Request.Files)
            {
                HttpPostedFileBase file;
                if (requestFile.IndexOf("_", StringComparison.Ordinal) < 1)
                {
                    throw new InvalidProgramException("Invalid alias name. it must be prepended with index.");
                }

                var aliasName = requestFile.Substring(requestFile.IndexOf("_", StringComparison.Ordinal) + 1);
                if (IsInvalidFile(scheduledFiles, requestFile, aliasName, duplicateName, out file))
                    continue;

                // schedule the file
                var path = ColloSysParam.WebParams.UploadPath;
                var directory = Path.IsPathRooted(path) ? path : Server.MapPath(path);
                FileUploadHelper.ScheduleFile(requestFile, aliasName, directory, file, scheduledFiles);
            }

            return Json(scheduledFiles.ScheduleInfo, "text/plain");
        }

        #region supporting methods

        private bool IsInvalidFile(FileUploadViewModel scheduledFiles, string requestFile,
            string aliasName, ICollection<string> duplicateName, out HttpPostedFileBase file)
        {
            // check that file is not already scheduled
            ColloSysEnums.FileAliasName name;
            if (!Enum.TryParse(aliasName, true, out name))
            {
                throw new InvalidDataException("Alias name must be from enum.");
            }

            // get the non-empty valid file
            file = Request.Files[requestFile];
            if (file == null)
            {
                _log.Fatal("Scheduling files : Received null file, should never happen.");
                return true;
            }

            if (scheduledFiles.ScheduleInfo.Count(x => x.AliasName == requestFile && x.IsScheduled) > 0)
            {
                _log.Info(string.Format("'{0}' file is already scheduled.", requestFile));
                return true;
            }

            var sfile = scheduledFiles.ScheduleInfo.First(x => x.AliasName == requestFile);
            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                sfile.HasError = true;
                sfile.ErrorMessage = string.Empty;
                return true;
            }

            if (duplicateName.Contains(file.FileName))
            {
                sfile.HasError = true;
                sfile.ErrorMessage = string.Format("File '{0}' is provided multiple times", file.FileName);
                return true;
            }

            if (file.ContentLength == 0)
            {
                sfile.HasError = true;
                sfile.ErrorMessage = "Please provide non-empty file.";
                return true;
            }

            if (string.IsNullOrWhiteSpace(Path.GetFileName(file.FileName)))
            {
                sfile.HasError = true;
                sfile.ErrorMessage = "Please provide file with valid name.";
                _log.Fatal("Scheduling files : Received empty filename, should never happen.");
                return true;
            }

            return false;
        }

        private IList<string> GetDuplicateFileNames()
        {

            IList<string> fileNames = (from string requestFile in Request.Files
                                       select Request.Files[requestFile]
                                           into file
                                           where (file != null) && (!string.IsNullOrWhiteSpace(file.FileName))
                                           select file.FileName)
                .ToList();

            return StringUtils.GetDuplicates(fileNames);
        }

        #endregion
    }
}
