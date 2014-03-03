using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BillingService.CustBillView;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared.Attributes;

namespace ColloSys.UserInterface.Areas.Billing.apiController
{
    public class ReadyForBillingApiController : ApiController
    {
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
            var session = SessionManager.GetCurrentSession();

            var data = session.QueryOver<BillStatus>().Where(x=>x.Products==products && x.BillMonth==month).SingleOrDefault();
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
            using (var session = SessionManager.GetCurrentSession())
            {
                var data = session.QueryOver<BillStatus>().List();
                return data;
            }

        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public void SaveBillingdata(BillStatus billStatus)
        {
            using (var session = SessionManager.GetCurrentSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(billStatus);
                    transaction.Commit();
                }

            }
        }

    }
}
