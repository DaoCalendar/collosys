#region references

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Shared.Attributes;

#endregion

namespace AngularUI.Generic.profile
{
    public class ProfileApiController : BaseApiController<Stakeholders>
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();

        [HttpSession]
        public HttpResponseMessage GetUser(string username)
        {
            var data = StakeQuery.FilterBy(x => x.ExternalId == username).FirstOrDefault();
            if (data == null) return Request.CreateResponse(HttpStatusCode.OK, "");
            var data2 = StakeQuery.OnIdWithAllReferences(data.Id);
            return Request.CreateResponse(HttpStatusCode.Created, data2);
        }
    }
}