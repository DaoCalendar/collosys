#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using BillingService.CustBillView;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.BillingBuilder;

#endregion


namespace ColloSys.UserInterface.Areas.Billing.apiController
{
    public class ReadyForBillingApiController : ApiController
    {
        private static readonly BillStatusBuilder BillStatusBuilder=new BillStatusBuilder();

        [HttpGet]
        
        public HttpResponseMessage FetchPageData(ScbEnums.Products products, uint month)
        {
           // var data = ProcessCustBillView.GetBillingServiceData(products, month);
            return Request.CreateResponse(HttpStatusCode.OK, "");
        }

        [HttpGet]
        
        public HttpResponseMessage GetBillStatus(ScbEnums.Products products, uint month)
        {
            var data =BillStatusBuilder.OnProductMonth(products,month);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }


        public IEnumerable<string> GetProductList()
        {
            return Enum.GetNames(typeof(ScbEnums.Products)).Where(x => x != "UNKNOWN")
                .ToList();
        }

        [HttpGet]
        
        public IEnumerable<BillStatus> GetBillingStatus()
        {
            return BillStatusBuilder.GetAll();
        }

        [HttpPost]
        
        public void SaveBillingdata(BillStatus billStatus)
        {
            BillStatusBuilder.Save(billStatus);
        }
    }
}
