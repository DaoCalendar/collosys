#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.SessionMgr;
using NHibernate;

#endregion

namespace BillingService2.Calculation
{
    public class BillingInfoManager
    {
        #region constructor
        public readonly Dictionary<string, DHFL_Info> InfoList;
        private readonly ISession _session = SessionManager.GetCurrentSession();

        public BillingInfoManager()
        {
            InfoList = new Dictionary<string, DHFL_Info>();
            GetInfo();
        }

        private void GetInfo()
        {
            var infoList = _session.QueryOver<DHFL_Info>().List();
            foreach (var info in infoList)
            {
                InfoList.Add(info.ApplNo, info);
            }
        }

        #endregion

        #region payout
        public void ManageInfoBeforePayout(DHFL_Liner liner)
        {
            if (!InfoList.ContainsKey(liner.ApplNo))
            {
                CreateInfo(liner);
            }
            else
            {
                var info = InfoList[liner.ApplNo];
                if (liner.DisbMonth != liner.BillMonth) return;
                liner.TotalPayout = info.TotalPayout;
                liner.TotalDisbAmt = info.TotalDisbAmt;
                liner.TotalProcFee = info.TotalProcFee;
            }
        }

        private void CreateInfo(DHFL_Liner liner)
        {
            var info = new DHFL_Info
            {
                ApplNo = liner.ApplNo,
                AgentId = liner.AgentId,
                SanctionAmt = liner.SanAmt,
                OriginMonth = liner.DisbMonth,
            };
            InfoList.Add(info.ApplNo, info);
        }

        public void ManageInfoAfterPayout(DHFL_Liner liner)
        {
            var info = InfoList[liner.ApplNo];
            info.TotalPayout = liner.Payout + liner.TotalPayout;
            info.TotalDisbAmt = liner.DisbursementAmt + liner.TotalDisbAmt;
        }
        #endregion

        #region capping
        public void ManageInfoBeforeCapping(DHFL_Liner liner)
        {
            var info = InfoList[liner.ApplNo];
            if (liner.DisbMonth != liner.BillMonth) return;
            liner.TotalDeductCap = info.TotalDeductCap;
        }

        public void ManageInfoAfterCapping(DHFL_Liner liner)
        {
            var info = InfoList[liner.ApplNo];
            liner.TotalPayout = liner.TotalPayout + liner.TotalDeductCap;
            info.TotalDeductCap = liner.TotalDeductCap + liner.DeductCap;
            info.TotalPayout = info.TotalPayout + liner.DeductCap;
        }
        #endregion

        #region proc fee
        public void ManageInfoAfterProcFee(DHFL_Liner liner)
        {
            var info = InfoList[liner.ApplNo];
            if (liner.DisbMonth != liner.BillMonth) return;
            liner.TotalProcFee = liner.FeeReceived;
        }

        public void ManageInfoBeforeProcFee(DHFL_Liner liner)
        {
            var info = InfoList[liner.ApplNo];
            info.TotalProcFee = liner.FeeReceived;
        }
        #endregion
    }
}
