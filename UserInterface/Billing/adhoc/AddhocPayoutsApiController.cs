#region references

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.BillingBuilder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;

#endregion

//stakeholders calls changed
namespace ColloSys.UserInterface.Areas.Billing.apiController
{
    public class AddhocPayoutsApiController : BaseApiController<BillAdhoc>
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();
        private static readonly ProductConfigBuilder ProductConfigBuilder = new ProductConfigBuilder();
        private static readonly BillAdhocBuilder BillAdhocBuilder = new BillAdhocBuilder();

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetProducts()
        {
            var data = ProductConfigBuilder.GetProducts();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetAdhocdata(ScbEnums.Products products)
        {
            var query = BillAdhocBuilder.ApplyRelations();

            var data = BillAdhocBuilder
                .Execute(query)
                .Where(x => x.Products == products)
                .ToList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetStakeHolders(ScbEnums.Products products)
        {
            var data = StakeQuery.OnProduct(products);
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }
    }
}
