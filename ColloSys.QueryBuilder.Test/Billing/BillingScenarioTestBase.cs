using System;
using BillingService2;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.SessionMgr;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.Billing
{
    public class BillingScenarioTestBase
    {
        protected void RunBillingForBillStatus(int billMonth, string agentId, int originMonth = 0)
        {
            if (originMonth == 0)
                originMonth = billMonth;

            var session = SessionManager.GetCurrentSession();

            var billStatus = session.QueryOver<BillStatus>()
                .Where(x => x.BillMonth == Convert.ToUInt32(billMonth)
                            && x.OriginMonth == Convert.ToUInt32(originMonth)
                            && x.ExternalId == agentId)
                .SingleOrDefault<BillStatus>();
            if (billStatus == null) Assert.Fail();

            var billingService = new BillingServices();
            billingService.BillingForBillStatus(billStatus);
        }
    }
}