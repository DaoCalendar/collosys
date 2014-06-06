using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.BillingBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;

namespace AngularUI.Billing.holdingpolicy
{
    public class ActivateHoldingApiController : BaseApiController<StkhHoldingPolicy>
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();
        private static readonly HoldingPolicyBuilder HoldingPolicyBuilder = new HoldingPolicyBuilder();
        private static readonly ActivateHoldingPolicyBuilder ActivateHoldingPolicyBuilder=new ActivateHoldingPolicyBuilder();

        [HttpGet]
        public HttpResponseMessage GetStakeholders()
        {
            var data = Session.QueryOver<Stakeholders>().List();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetHoldingPolicies()
        {
            var data = Session.QueryOver<HoldingPolicy>().List();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetPageData(ScbEnums.Products products)
        {
            var data =
                new
                {
                    Stakeholders = StakeQuery.OnProduct(products),
                    HoldingPolicies = HoldingPolicyBuilder.OnProduct(products)
                };
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetActivatePolicies()
        {
            var data = ActivateHoldingPolicyBuilder.GetAllActivateHoldingPolicies();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }


        [HttpGet]
        public HttpResponseMessage GetData()
        {
            var data = Session.QueryOver<StkhHoldingPolicy>().Fetch(x => x.HoldingPolicy).Eager
                              .Fetch(x => x.Stakeholder).Eager
                              .SingleOrDefault();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

    }
}


//[HttpGet]
//public HttpResponseMessage GetStakeholders(ScbEnums.Products products)
//{
//    var data = StakeQuery.OnProduct(products);
//    return Request.CreateResponse(HttpStatusCode.OK, data);
//}

//[HttpGet]
//public HttpResponseMessage GetHoldingPolicies(ScbEnums.Products products)
//{
//    var data = HoldingPolicyBuilder.OnProduct(products);
//    return Request.CreateResponse(HttpStatusCode.OK, data);
//}