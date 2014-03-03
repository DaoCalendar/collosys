using System;
using System.Configuration.Provider;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using ColloSys.DataLayer.Domain;
using ColloSys.UserInterface.Shared.Attributes;
using NLog;

namespace UserInterfaceAngular.app
{
    public class ProfileApiController : ColloSys.UserInterface.Shared.BaseApiController<Stakeholders>
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();
        // GET api/<controller>
        [HttpGet]
        public override HttpResponseMessage Get()
        {
            try
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    var user = Membership.Provider.GetUser(HttpContext.Current.User.Identity.Name, true);
                    if (user == null)
                    {
                        throw new NullReferenceException("User Not Exist");
                    }
                    
                    var data = Session.QueryOver<Stakeholders>()
                        .Where(x => x.ExternalId == user.UserName)
                        .SingleOrDefault<Stakeholders>();

                    return Request.CreateResponse(HttpStatusCode.Created, data);
                }
                return null;
            }
            catch (NullReferenceException e)
            {
                Log.Error("Get User Profile Of Login User:" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
            catch (ProviderException e)
            {
                Log.Error("Get User Profile Of Login User:" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }
    }
}