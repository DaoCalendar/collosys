#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BillingService.CustBillView;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BillingBuilder;
using ColloSys.UserInterface.Shared.Attributes;

#endregion


namespace ColloSys.UserInterface.Areas.Billing.apiController
{
    public class ReadyForBillingApiController : ApiController
    {
        private static readonly BillStatusBuilder BillStatusBuilder=new BillStatusBuilder();

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage FetchPageData(ScbEnums.Products products, uint month)
        {
            var data = ProcessCustBillView.GetBillingServiceData(products, month);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
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
        [HttpTransaction]
        public IEnumerable<BillStatus> GetBillingStatus()
        {
            return BillStatusBuilder.GetAll();
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public void SaveBillingdata(BillStatus billStatus)
        {
            BillStatusBuilder.Save(billStatus);
        }
    }
}
