#region references

using System;
using ColloSys.DataLayer.ClientData;
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
    }
}
