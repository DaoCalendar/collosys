#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.Shared.ConfigSectionReader;
using ColloSys.Shared.SharedUtils;
using ColloSys.UserInterface.Areas.FileUploader.Models;
using ColloSys.UserInterface.Shared.Attributes;

#endregion

namespace AngularUI.FileUpload.filescheduler
{
    public class FileSchedulerApiController : BaseApiController<FileScheduler>
    {
        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetFileDetails()
        {
            var data = Session.QueryOver<FileDetail>().List();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetFileStatus(ScbEnums.ScbSystems isystem, ScbEnums.Category icategory, DateTime idate)
        {
            var viewModel = new FileUploadViewModel
                        {
                            SelectedSystem = isystem,
                            SelectedCategory = icategory,
                            ScheduleDate = idate
                        };

            viewModel = FileUploadHelper.InitFileInfo(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, viewModel);
        }

        [HttpPost]
        public HttpResponseMessage Upload(FileUploadViewModel scheduledFiles)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            //// init schedule info from db, just to be safe ;)
            //var duplicateName = GetDuplicateFileNames();

            //foreach (string requestFile in Request.Files)
            //{
            //    HttpPostedFileBase file;
            //    if (requestFile.IndexOf("_", StringComparison.Ordinal) < 1)
            //    {
            //        throw new InvalidProgramException("Invalid alias name. it must be prepended with index.");
            //    }

            //    var aliasName = requestFile.Substring(requestFile.IndexOf("_", StringComparison.Ordinal) + 1);
            //    if (IsInvalidFile(scheduledFiles, requestFile, aliasName, duplicateName, out file))
            //        continue;

            //    // schedule the file
            //    var path = ColloSysParam.WebParams.UploadPath;
            //    var directory = Path.IsPathRooted(path) ? path : Server.MapPath(path);
            //    FileUploadHelper.ScheduleFile(requestFile, aliasName, directory, file, scheduledFiles);
            //}

            return Request.CreateResponse(HttpStatusCode.OK, scheduledFiles);
        }



        //private bool IsInvalidFile(FileUploadViewModel scheduledFiles, string requestFile,
        //    string aliasName, ICollection<string> duplicateName,
        //    Dictionary<string, HttpPostedFileBase> requestedFiles, out HttpPostedFileBase file)
        //{
        //    // check that file is not already scheduled
        //    ColloSysEnums.FileAliasName name;
        //    if (!Enum.TryParse(aliasName, true, out name))
        //    {
        //        throw new InvalidDataException("Alias name must be from enum.");
        //    }

        //    // get the non-empty valid file
        //    file = requestedFiles[requestFile];
        //    if (file == null)
        //    {
        //        _log.Fatal("Scheduling files : Received null file, should never happen.");
        //        return true;
        //    }

        //    if (scheduledFiles.ScheduleInfo.Count(x => x.AliasName == requestFile && x.IsScheduled) > 0)
        //    {
        //        _log.Info(string.Format("'{0}' file is already scheduled.", requestFile));
        //        return true;
        //    }

        //    var sfile = scheduledFiles.ScheduleInfo.First(x => x.AliasName == requestFile);
        //    if (string.IsNullOrWhiteSpace(file.FileName))
        //    {
        //        sfile.HasError = true;
        //        sfile.ErrorMessage = string.Empty;
        //        return true;
        //    }

        //    if (duplicateName.Contains(file.FileName))
        //    {
        //        sfile.HasError = true;
        //        sfile.ErrorMessage = string.Format("File '{0}' is provided multiple times", file.FileName);
        //        return true;
        //    }

        //    if (file.ContentLength == 0)
        //    {
        //        sfile.HasError = true;
        //        sfile.ErrorMessage = "Please provide non-empty file.";
        //        return true;
        //    }

        //    if (string.IsNullOrWhiteSpace(Path.GetFileName(file.FileName)))
        //    {
        //        sfile.HasError = true;
        //        sfile.ErrorMessage = "Please provide file with valid name.";
        //        _log.Fatal("Scheduling files : Received empty filename, should never happen.");
        //        return true;
        //    }

        //    return false;
        //}

        //private IList<string> GetDuplicateFileNames(IReadOnlyDictionary<string, HttpPostedFileBase> fileList)
        //{

        //    IList<string> fileNames = (from string requestFile in fileList
        //                               select fileList[requestFile]
        //                                   into file
        //                                   where (file != null) && (!string.IsNullOrWhiteSpace(file.FileName))
        //                                   select file.FileName)
        //        .ToList();

        //    return StringUtils.GetDuplicates(fileNames);
        //}

    }
}
