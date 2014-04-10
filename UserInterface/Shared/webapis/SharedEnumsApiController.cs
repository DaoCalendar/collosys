using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.Model;

namespace AngularUI.Shared.webapis
{
    public class SharedEnumsApiController : ApiController
    {
        private readonly FetchEnums _fetchEnums = new FetchEnums();

        [HttpGet]
        public HttpResponseMessage FetchAllEnum()
        {
            var list = new EnumList();

            _fetchEnums.InitSystemEnums(list);
            _fetchEnums.AddCustomEnums(list);
            _fetchEnums.AddQueryEnums(list);

            return Request.CreateResponse(HttpStatusCode.OK, list.Enums);
        }

    }
}
