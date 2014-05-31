using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using NHibernate;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.Billing
{
    [TestFixture]
    public class BillingData
    {
        [Test]
        public void TestDbData()
        {
            var session = SessionManager.GetCurrentSession();
            var list = session.QueryOver<DHFL_Liner>().RowCount();
            Assert.Greater(list, 0);
        }

        [Test]
        public void BillDataMassage()
        {
            var dataManager = new BillingDataMassager(201401, ScbEnums.Products.HL);
            dataManager.ProcessCurrentMonth();
            dataManager.ProcessHistoricalData();
            dataManager.SaveData();
        }
    }

    public class BillingDataMassager
    {
        #region constructor
        private readonly uint _billMonth;
        private const uint Months2Check = 3;
        private readonly ScbEnums.Products _product;

        public BillingDataMassager(uint month, ScbEnums.Products product)
        {
            _billMonth = month;
            _product = product;
        }
        #endregion

        #region queries
        private readonly ISession _session = SessionManager.GetCurrentSession();
        private IList<DHFL_Liner> _currentData = new List<DHFL_Liner>();
        private IList<DHFL_Liner> _historyData = new List<DHFL_Liner>();

        private void GetCurrentMonthData()
        {
            _currentData = _session.QueryOver<DHFL_Liner>()
                .Where(x => x.BillMonth == _billMonth)
                .List();
        }

        private void GetHistoricalData()
        {
            var monthYearDate = DateTime.ParseExact(_billMonth.ToString(CultureInfo.InvariantCulture),
                "yyyyMM",
                CultureInfo.InvariantCulture);

            var maxMonthYearDate = monthYearDate.AddMonths((int)(-1 * Months2Check));
            var maxMonthYear = Convert.ToUInt32(maxMonthYearDate.ToString("yyyyyMM"));

            _historyData = _session.QueryOver<DHFL_Liner>()
                .Where(x => x.BillMonth < _billMonth)
                .And(x => x.BillMonth >= maxMonthYear)
                .OrderBy(x => x.BillMonth).Desc
                .List();
        }

        private Stakeholders GetStakeholder(string agentId)
        {
            return _session.QueryOver<Stakeholders>()
                .Where(x => x.ExternalId == agentId)
                .SingleOrDefault();
        }

        public void SaveData()
        {
            using (var tx = _session.BeginTransaction())
            {
                SaveLiner(_currentData);
                SaveLiner(_recomputeData);
                SaveLiner(GenerateBillStatus());
                tx.Commit();
            }
        }

        private void SaveLiner(IEnumerable<DHFL_Liner> rows)
        {
            foreach (var liner in rows)
            {
                _session.SaveOrUpdate(liner);
            }
        }

        private void SaveLiner(IEnumerable<BillStatus> rows)
        {
            foreach (var liner in rows)
            {
                _session.SaveOrUpdate(liner);
            }
        }

        private IEnumerable<BillStatus> GenerateBillStatus()
        {
            var statusList = new List<BillStatus>();
            foreach (var entry in _stkhMonth)
            {
                var param = entry.Split('@');
                var stkh = GetStakeholder(param[0]);
                //var month = Convert.ToUInt32(param[1]);

                var billStatus = new BillStatus
                {
                    BillCycle = 0,
                    BillMonth = _billMonth,
                    Products = _product,
                    Stakeholder = stkh,
                    Status = ColloSysEnums.BillingStatus.Pending,
                };
                statusList.Add(billStatus);
            }
            return statusList;
        }

        #endregion

        #region process current month
        private readonly HashSet<string> _stkhMonth = new HashSet<string>();

        public void ProcessCurrentMonth()
        {
            GetCurrentMonthData();
            if (_currentData.Count == 0) return;

            ExcludeCases();
            AddStakeholdersForCurrentMonth();
        }

        private void ExcludeCases()
        {
            foreach (var liner in _currentData.Where(liner => liner.DisbursementAmt < 0))
            {
                liner.IsExcluded = true;
                liner.ExcludeReason = "Reversal Case";
            }
        }

        private void AddStakeholdersForCurrentMonth()
        {
            foreach (var liner in _currentData)
            {
                MarkStakeholderForBilling(liner);
            }
        }

        private void MarkStakeholderForBilling(DHFL_Liner liner)
        {
            _stkhMonth.Add(string.Format("{0}@{1}", liner.AgentId, liner.BillMonth));
        }

        private bool IsStakeholderAdded(DHFL_Liner liner)
        {
            return _stkhMonth.Contains(string.Format("{0}@{1}", liner.AgentId, liner.BillMonth));
        }
        #endregion

        #region process historical data
        private readonly List<DHFL_Liner> _recomputeData = new List<DHFL_Liner>();

        public void ProcessHistoricalData()
        {
            if (_currentData.Count == 0) return;
            GetHistoricalData();
            if (_historyData.Count == 0) return;
            AddStakeholdersForReversalCases();
        }

        private void AddStakeholdersForReversalCases()
        {
            foreach (var liner in _currentData.Where(liner => liner.DisbursementAmt < 0))
            {
                var liner1 = liner;
                var oldLiner = _historyData.FirstOrDefault(x => x.AgentId == liner1.AgentId &&
                                                               x.ApplNo == liner1.ApplNo &&
                                                               x.DisbursementAmt == (-1 * liner1.DisbursementAmt));
                if (oldLiner == null)
                {
                    liner1.ExcludeReason += ", no history";
                    continue;
                }
                DupliateDataForBilling(oldLiner);
                ExcludeEntryFromBilling(oldLiner);
            }
        }

        private void ExcludeEntryFromBilling(DHFL_Liner oldLiner)
        {
            var row = _recomputeData.FirstOrDefault(x => x.AgentId == oldLiner.AgentId &&
                                                         x.ApplNo == oldLiner.ApplNo &&
                                                         x.DisbursementAmt == oldLiner.DisbursementAmt);
            if (row == null) return;
            row.IsExcluded = true;
            row.ExcludeReason = "disb cancelled";
        }

        private void DupliateDataForBilling(DHFL_Liner oldLiner)
        {
            if (IsStakeholderAdded(oldLiner)) return;
            MarkStakeholderForBilling(oldLiner);

            var data = _historyData.Where(x => x.AgentId == oldLiner.AgentId
                && x.BillMonth == oldLiner.BillMonth).ToList();
            if (data.Count != 0) return;

            foreach (var liner in data)
            {
                liner.ResetUniqueProperties();
                liner.BillMonth = _billMonth;
                liner.ExcludeReason = "rebill";
            }
            _recomputeData.AddRange(data);
        }
        #endregion
    }
}
