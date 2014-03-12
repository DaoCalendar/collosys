using System.Net;
using System.Net.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.FileUploader;
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
    }
}
