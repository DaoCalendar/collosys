#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.Shared.ConfigSectionReader;
using ColloSys.Shared.SharedUtils;
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
        public async Task<HttpResponseMessage> SaveFileOnServer()
        {
            if (!Request.Content.IsMimeMultipartContent())
                Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);

            var provider = FileUploadHelper.GetMultipartProvider();

            var result = await Request.Content.ReadAsMultipartAsync(provider);
            var fileInfo = FileUploadHelper.MoveToTemp(result);

            var pageData = FileUploadHelper.GetFormData<ScheduledFiles>(result);
            pageData.UploadPath = fileInfo.FullName;
            //pageData.FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss_") + fileInfo.Name;
            pageData.FileSize = (ulong)fileInfo.Length;

            return Request.CreateResponse(HttpStatusCode.OK, pageData);
        }

        [HttpPost]
        [HttpTransaction]
        public HttpResponseMessage Upload(FileUploadViewModel scheduledFiles)
        {
            var filenameslist = new List<string>();
            filenameslist.AddRange(scheduledFiles.ScheduleInfo.
                Where(x => x.IsScheduled)
                .Select(x => x.FileName.Substring(16))
                .ToList());
            filenameslist.AddRange(scheduledFiles.ScheduleInfo
                .Where(x => !x.IsScheduled && string.IsNullOrWhiteSpace(x.FileName))
                .Select(x => new FileInfo(x.UploadPath).Name)
                .ToList());
            var duplicateName = StringUtils.GetDuplicates(filenameslist);

            foreach (var schedulerInfo in scheduledFiles.ScheduleInfo.Where(x => !x.IsScheduled
                && !string.IsNullOrWhiteSpace(x.UploadPath)))
            {
                if (IsInvalidFile(schedulerInfo, duplicateName))
                    continue;

                // schedule the file
                var path = ColloSysParam.WebParams.UploadPath;
                var directory = Path.IsPathRooted(path) ? path : HostingEnvironment.MapPath(path);
                FileUploadHelper.ScheduleFile(scheduledFiles, schedulerInfo, directory);
            }

            return Request.CreateResponse(HttpStatusCode.OK, scheduledFiles);
        }

        private bool IsInvalidFile(ScheduledFiles scheduledFiles, IList<string> duplicateName)
        {
            // get the non-empty valid file
            var file = new FileInfo(scheduledFiles.UploadPath);
            if (!file.Exists)
            {
                scheduledFiles.HasError = true;
                scheduledFiles.ErrorMessage = "File has been removed from the server!!!";
            }

            if (string.IsNullOrWhiteSpace(file.Name))
            {
                scheduledFiles.HasError = true;
                scheduledFiles.ErrorMessage = string.Empty;
                return true;
            }

            if (file.Length == 0)
            {
                scheduledFiles.HasError = true;
                scheduledFiles.ErrorMessage = "Please provide non-empty file.";
                return true;
            }

            if (duplicateName.Contains(file.Name))
            {
                scheduledFiles.HasError = true;
                scheduledFiles.ErrorMessage = string.Format("File '{0}' is provided multiple times", file.Name);
                return true;
            }

            // check extension
            ColloSysEnums.FileType fileType;
            if ((!Enum.TryParse(file.Extension.Replace(".", ""), true, out fileType)) || (scheduledFiles.FileType != fileType))
            {
                var expectedExtension = Enum.GetName(typeof(ColloSysEnums.FileType), scheduledFiles.FileType);
                scheduledFiles.HasError = true;
                scheduledFiles.ErrorMessage = "Please provide file with '" + expectedExtension + "' extensions only.";
                return true;
            }

            return false;
        }
    }
}
