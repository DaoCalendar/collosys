using System;
using System.Net;
using System.Net.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.UserInterface.Areas.FileUploader.Models;
using ColloSys.UserInterface.Shared.Attributes;

namespace AngularUI.FileUpload.filescheduler
{
    public class FileSchedulerApiController : BaseApiController<FileScheduler>
    {
        [HttpTransaction]
        public HttpResponseMessage GetFileDetails()
        {
            var data = Session.QueryOver<FileDetail>().List();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

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
    }
}
