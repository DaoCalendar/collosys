#region references

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.StakeholderBuilder;

#endregion

namespace AngularUI.Generic.profile
{
    public class ProfileApiController : BaseApiController<Stakeholders>
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();

        [HttpGet]
        public HttpResponseMessage GetUser(string username)
        {
            var data = StakeQuery.GetOnExpression(x => x.ExternalId == username).FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.Created, data);
        }
    }
}