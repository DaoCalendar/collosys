using System;
using System.Collections.Generic;

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

#endregion

//stakeholders calls changed
namespace ColloSys.UserInterface.Areas.Billing.apiController
{
    public class AddhocPayoutsApiController : BaseApiController<BillAdhoc>
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();
        private static readonly ProductConfigBuilder ProductConfigBuilder = new ProductConfigBuilder();
        private static readonly BillAdhocBuilder BillAdhocBuilder = new BillAdhocBuilder();
        private static readonly BillStatusBuilder BillStatusBuilder = new BillStatusBuilder();

        [HttpGet]

        public HttpResponseMessage GetProducts()
        {
            var data = ProductConfigBuilder.GetProducts();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]

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

        public HttpResponseMessage GetStatus(ScbEnums.Products product, uint startmonth)
        {
            var data = BillStatusBuilder.FilterBy(x => x.Products == product && x.BillMonth == startmonth 
                && x.Status == ColloSysEnums.BillingStatus.Done).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, data.Count > 0);

        }

        [HttpGet]

        public IList<Stakeholders> GetStakeHolders(ScbEnums.Products products)
        {
            var data = StakeQuery.OnProduct(products);
            return data;
            //return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }
}

//public HttpResponseMessage GetStatus(ScbEnums.Products product)
//        {
//            var data = BillStatusBuilder.FilterBy(x => x.Products == product && x.Status == ColloSysEnums.BillingStatus.Done).ToList();

//            data = data.OrderByDescending(x => x.BillMonth).ToList();

//            var year = DateTime.Now.Year.ToString();
//            var month = DateTime.Now.Month.ToString();

//            string date;

//            if (int.Parse(month) < 10)
//                date = year + "0" + month;
//            else
//            {
//                date = year + month;
//            }

//            return Request.CreateResponse(HttpStatusCode.OK, (data[0].BillMonth < int.Parse(date)));
// {           var data = BillStatusBuilder.FilterBy(x => x.Products == product && x.Status == ColloSysEnums.BillingStatus.Done).ToList();


//        }
