#region references

using System;
using BillingService2;
using BillingService2.Calculation;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using NUnit.Framework;

#endregion

namespace ColloSys.QueryBuilder.Test.Billing
{
    [TestFixture]
    public class BillingDataTests
    {
        [Test]
        public void TestDbData()
        {
            var session = SessionManager.GetCurrentSession();
            var list = session.QueryOver<DHFL_Liner>().RowCount();
            Assert.Greater(list, 0);
        }

        [TestCase(201401)]
        [TestCase(201402)]
        [TestCase(201403)]
        public void BillDataMassageJan(int month)
        {
            var dataManager = new BillingDataMassager(Convert.ToUInt32(month), ScbEnums.Products.HL);
            dataManager.ProcessCurrentMonth();
            dataManager.ProcessHistoricalData();
            dataManager.SaveData();
        }

        [Test]
        public void CleanUpData()
        {
            var session = SessionManager.GetCurrentSession();

            using (var tx = session.BeginTransaction())
            {
                var dhflLiners = session.QueryOver<DHFL_Liner>().AndRestrictionOn(x => x.ExcludeReason)
                          .IsIn(new[] { "Rebill", "Rebill, Disb Cancelled" }).List<DHFL_Liner>();

                foreach (var dhflLiner in dhflLiners)
                {
                    session.Delete(dhflLiner);
                }

                var billStatuses = session.QueryOver<BillStatus>().List<BillStatus>();
                foreach (var billStatus in billStatuses)
                {
                    session.Delete(billStatus);
                }

                tx.Commit();
            }
        }

        [Test]
        public void Reset()
        {
            var session = SessionManager.GetCurrentSession();

            using (var tx = session.BeginTransaction())
            {
                var dhflLiners = session.QueryOver<DHFL_Liner>().List<DHFL_Liner>();

                foreach (var dhflLiner in dhflLiners)
                {
                    dhflLiner.BillStatus = ColloSysEnums.BillStatus.Unbilled;
                    dhflLiner.Payout = 0;
                    dhflLiner.BillDetail = null;
                    dhflLiner.TotalDisbAmt = 0;
                    dhflLiner.DeductCap = 0;
                    dhflLiner.TotalDeductCap = 0;
                    dhflLiner.DeductPf = 0;
                    dhflLiner.TotalPayout = 0;
                    dhflLiner.TotalProcFee = 0;

                    session.SaveOrUpdate(dhflLiner);
                }

                var billStatuses = session.QueryOver<BillStatus>().List<BillStatus>();
                foreach (var billStatus in billStatuses)
                {
                    billStatus.Status = ColloSysEnums.BillingStatus.Pending;
                    session.SaveOrUpdate(billStatus);
                }

                var billDetails = session.QueryOver<BillDetail>().List<BillDetail>();
                foreach (var billDetail in billDetails)
                {
                    session.Delete(billDetail);
                }

                var billAmounts = session.QueryOver<BillAmount>().List<BillAmount>();
                foreach (var billAmount in billAmounts)
                {
                    session.Delete(billAmount);
                }

                var dhflInfos = session.QueryOver<DHFL_Info>().List<DHFL_Info>();
                foreach (var dhflInfo in dhflInfos)
                {
                    session.Delete(dhflInfo);
                }

                tx.Commit();
            }
        }

        [TestCase(201401, "010366", 0)]
        [TestCase(201401, "009275", 0)]
        [TestCase(201401, "008325", 0)]
        [TestCase(201401, "008012", 0)]
        [TestCase(201401, "009077", 0)]
        [TestCase(201401, "009057", 0)]
        public void RunBillingForBillStatus01Jan(int billMonth, string agentId, int originMonth)
        {
            RunBillingForBillStatus(billMonth, agentId, originMonth);
        }

        [TestCase(201402, "008012", 201401)]
        public void RunBillingForBillStatus02JanFeb(int billMonth, string agentId, int originMonth)
        {
            RunBillingForBillStatus(billMonth, agentId, originMonth);
        }

        [TestCase(201402, "008471", 0)]
        [TestCase(201402, "008844", 0)]
        [TestCase(201402, "011762", 0)]
        [TestCase(201402, "003419", 0)]
        [TestCase(201402, "011952", 0)]
        [TestCase(201402, "008012", 201402)]
        public void RunBillingForBillStatus03Feb(int billMonth, string agentId, int originMonth)
        {
            RunBillingForBillStatus(billMonth, agentId, originMonth);
        }
       


        private void RunBillingForBillStatus(int billMonth, string agentId, int originMonth)
        {
            if (originMonth == 0)
                originMonth = billMonth;

            var session = SessionManager.GetCurrentSession();

            var billStatus = session.QueryOver<BillStatus>().Where(x => x.BillMonth == Convert.ToUInt32(billMonth)
                                                                        && x.OriginMonth == Convert.ToUInt32(originMonth)
                                                                        && x.ExternalId == agentId)
                                                                        .SingleOrDefault<BillStatus>();
            if (billStatus == null)
                Assert.Fail();

            var billingService = new BillingServices();
            billingService.BillingForBillStatus(billStatus);
        }








    }

}
