#region references

using System;
using System.Configuration.Provider;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.StakeholderBuilder;
using NLog;

#endregion


namespace UserInterfaceAngular.app
{
    public class ProfileApiController : ColloSys.UserInterface.Shared.BaseApiController<Stakeholders>
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static readonly StakeQueryBuilder StakeQuery=new StakeQueryBuilder();

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

                    var data = StakeQuery.GetOnExpression(x => x.ExternalId == user.UserName).FirstOrDefault();

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