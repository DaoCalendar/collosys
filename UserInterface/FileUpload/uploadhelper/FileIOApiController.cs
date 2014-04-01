using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AngularUI.FileUpload.uploadhelper
{
    public class FileIoApiController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> SaveFile()
        {
            if (!Request.Content.IsMimeMultipartContent())
                Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);

            var provider = FileSaver.GetMultipartProvider();

            var result = await Request.Content.ReadAsMultipartAsync(provider);
            var fileInfo = FileSaver.MoveToTemp(result);
            return Request.CreateResponse(HttpStatusCode.OK, fileInfo);
        }

    }
}
