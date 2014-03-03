using System.Web.Http;
using System.Web.Security;
using NLog;

namespace UserInterfaceAngular.app
{
    public class BaseApiController : ApiController
    {
        protected readonly Logger _log = LogManager.GetCurrentClassLogger();

        public void CloseSession()
        {
            FormsAuthentication.SignOut();

        }
    }
}
