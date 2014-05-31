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

        #region info logic
        public void ManageInfo(DHFL_Liner liner)
        {
            if (InfoList.ContainsKey(liner.ApplNo))
            {
                var info = InfoList[liner.ApplNo];
                if (info.UpdateMonth > liner.DisbMonth)
                {
                    UpdateInfoFromLiner(info, liner);
                }
                else
                {
                    SyncInfoNLiner(info, liner);
                }
            }
            else
            {
                CreateInfo(liner);
            }
        }

        private void CreateInfo(DHFL_Liner liner)
        {
            var info = new DHFL_Info
            {
                ApplNo = liner.ApplNo,
                SanctionAmt = liner.SanAmt,
                TotalDisbAmt = liner.DisbursementAmt,
                UpdateMonth = liner.DisbMonth,
            };
            InfoList.Add(info.ApplNo, info);
        }

        private void UpdateInfoFromLiner(DHFL_Info info, DHFL_Liner liner)
        {
            info.UpdateMonth = liner.DisbMonth;
            info.TotalDisbAmt = liner.TotalDisbAmt;
            info.DeductCap = liner.DeductCap;
            info.DeductPf = liner.DeductPf;
            info.TotalPayout = liner.TotalPayout;
            info.TotalProcFee = liner.TotalProcFee;
        }

        private void SyncInfoNLiner(DHFL_Info info, DHFL_Liner liner)
        {
            info.UpdateMonth = liner.DisbMonth;
            info.TotalDisbAmt = liner.DisbursementAmt + info.TotalDisbAmt;
            info.DeductCap = liner.DeductCap + info.DeductCap;
            info.DeductPf = liner.DeductPf + info.DeductPf;
            info.TotalPayout = liner.Payout +  info.TotalPayout;
            info.TotalProcFee = liner.ProcFee;
        }
        #endregion

        #region queries
        private readonly ISession _session = SessionManager.GetCurrentSession();

        public void SaveData(IEnumerable<DHFL_Liner> linerList )
        {
            using (var tx = _session.BeginTransaction())
            {
                foreach (KeyValuePair<string, DHFL_Info> entry in InfoList)
                {
                    _session.SaveOrUpdate(entry.Value);
                }
                foreach (var liner in linerList)
                {
                    _session.SaveOrUpdate(liner);
                }
                tx.Commit();
            }
        }
        #endregion
    }
}
