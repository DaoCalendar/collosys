using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AngularUI.Generic.login
{
    public class AutheticationController : ApiController
    {
        public HttpResponseMessage AutheticateUser(string login, string password)
        {

            return Request.CreateResponse(HttpStatusCode.OK, true);
        }
    }
}
