#region references

using System;
using System.Linq;
using BillingService2.Calculation;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using NHibernate.Linq;
using NUnit.Framework;

#endregion

namespace ColloSys.QueryBuilder.Test.Billing
{
    [TestFixture]
    public class BillingDataTests
    {
        [Test]
        public void TestFileUploadDataPresent()
        {
            var session = SessionManager.GetCurrentSession();
            var list = session.QueryOver<DHFL_Liner>().RowCount();
            Assert.Greater(list, 0);
        }

        [Test]
        public void FileUploadDataCleanup()
        {
            var session = SessionManager.GetCurrentSession();

            using (var tx = session.BeginTransaction())
            {
                string[] valuesList = { "Rebill", "Rebill, Disb Cancelled" };
                var dhflLiners = session.QueryOver<DHFL_Liner>().AndRestrictionOn(x => x.ExcludeReason)
                    // ReSharper disable once CoVariantArrayConversion
                          .IsIn(valuesList).List<DHFL_Liner>();

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

        [TestCase(201401)]
        [TestCase(201402)]
        [TestCase(201403)]
        public void FileUpload2Ready4Billing(int month)
        {
            var dataManager = new BillingDataMassager(Convert.ToUInt32(month), ScbEnums.Products.HL);
            dataManager.ProcessCurrentMonth();
            dataManager.ProcessHistoricalData();
            dataManager.SaveData();
        }

        [Test]
        public void PreBillingDataCleanup()
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
                    dhflLiner.ProratedProcFee = 0;
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

        [TestCase(201401, "009057", 201401)]
        [TestCase(201402, "009057", 201401)]
        [TestCase(201402, "009057", 201402)]
        public void AgentMonthDataCleanup(int billingMonth, string agentId, int disbMonth)
        {
            if (disbMonth == 0) disbMonth = billingMonth;

            var session = SessionManager.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                var dhflLiners = session.QueryOver<DHFL_Liner>()
                    .Where(x => x.BillMonth == billingMonth && x.DisbMonth == disbMonth)
                    .And(x => x.AgentId == agentId)
                    .List<DHFL_Liner>();

                foreach (var dhflLiner in dhflLiners)
                {
                    dhflLiner.BillStatus = ColloSysEnums.BillStatus.Unbilled;
                    dhflLiner.BillDetail = null;
                    dhflLiner.Payout = 0;
                    dhflLiner.DeductCap = 0;
                    dhflLiner.ProratedProcFee = 0;
                    session.SaveOrUpdate(dhflLiner);
                }

                var billStatuses = session.QueryOver<BillStatus>()
                    .Where(x => x.BillMonth == billingMonth && x.OriginMonth == disbMonth)
                    .And(x => x.ExternalId == agentId)
                    .SingleOrDefault();
                if (billStatuses == null) return;
                billStatuses.Status = ColloSysEnums.BillingStatus.Pending;
                session.SaveOrUpdate(billStatuses);

                var billDetails = session.Query<BillDetail>()
                    .Fetch(x => x.Stakeholder)
                    .Where(x => x.BillMonth == billingMonth && x.Stakeholder.ExternalId == agentId && x.OriginMonth == disbMonth)
                    .ToList();
                foreach (var billDetail in billDetails)
                {
                    session.Delete(billDetail);
                }

                var billAmounts = session.Query<BillAmount>()
                    .Fetch(x => x.Stakeholder)
                    .Where(x => x.BillMonth == billingMonth && x.Stakeholder.ExternalId == agentId)
                    .ToList();
                foreach (var billAmount in billAmounts)
                {
                    session.Delete(billAmount);
                }

                var infos = session.Query<DHFL_Info>()
                    .Where(x => x.AgentId == agentId)
                    .ToList();
                foreach (var info in infos)
                {
                    session.Delete(info);
                }

                tx.Commit();
            }
        }

        [Test]
        public void UpdateIsProfessional()
        {
            var session = SessionManager.GetCurrentSession();
            var linerList = session.QueryOver<DHFL_Liner>().List();
            using (var trans=session.BeginTransaction())
            {
                foreach (var dhflLiner in linerList)
                {
                    if (dhflLiner.Occupcategory.ToUpper() == "SALARIED" 
                        || dhflLiner.Occupcategory.ToUpper() == "SELF EMPLOYED PROFESSIONAL")
                        dhflLiner.IsProfessional = "Y";
                    else
                        dhflLiner.IsProfessional = "N";
                    session.SaveOrUpdate(dhflLiner);
                }    
                trans.Commit();
            }
        }
    }

}
