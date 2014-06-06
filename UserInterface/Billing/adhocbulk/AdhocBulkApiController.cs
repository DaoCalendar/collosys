using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.BillingBuilder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;

namespace AngularUI.Billing.adhocbulk
{
    public class AdhocBulkApiController : ApiController
    {
        private static readonly ProductConfigBuilder ProductConfigBuilder = new ProductConfigBuilder();
        private static readonly StakeQueryBuilder StakeQueryBuilder=new StakeQueryBuilder(); 
        private static readonly BillStatusBuilder BillStatusBuilder=new BillStatusBuilder();
        private static readonly BillAdhocBuilder BillAdhocBuilder=new BillAdhocBuilder();

        [HttpGet]
        public HttpResponseMessage GetProducts()
        {
            var data = ProductConfigBuilder.GetProducts();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage StakeholderList(string name, ScbEnums.Products products)
        {
            var data = StakeQueryBuilder.ListForAdhoc(name, products);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]

        public HttpResponseMessage GetStatus(ScbEnums.Products product, uint startmonth)
        {
            var data = BillStatusBuilder.FilterBy(x => x.Products == product && x.BillMonth == startmonth
                && x.Status == ColloSysEnums.BillingStatus.Done).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, data.Count > 0);

        }

        [HttpPost]
        public HttpResponseMessage SaveList(IEnumerable<BillAdhoc> paymentList)
        {
            var savelist = paymentList.Where(x => x.Products != ScbEnums.Products.UNKNOWN && x.Stakeholder != null).ToList();
            BillAdhocBuilder.Save(savelist);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
