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

        #region info logic

        public void ManageInfoBeforePayout(DHFL_Liner liner)
        {
            if (!InfoList.ContainsKey(liner.ApplNo))
            {
                CreateInfo(liner);
            }
            else
            {
                var info = InfoList[liner.ApplNo];
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
                SanctionAmt = liner.SanAmt,
                UpdateMonth = liner.DisbMonth,
            };
            InfoList.Add(info.ApplNo, info);
        }

        public void ManageInfoAfterPayout(DHFL_Liner liner)
        {
            var info = InfoList[liner.ApplNo];
            info.TotalPayout = liner.Payout + liner.TotalPayout;
        }

        public void ManageInfoAfterCapping(DHFL_Liner liner, decimal actualPayout)
        {
            var info = InfoList[liner.ApplNo];
            //info.DeductCap
        }

        public void ManageInfoAfterProcFee(DHFL_Liner liner)
        {
            var info = InfoList[liner.ApplNo];
            //info.DeductPf
        }

        #endregion

        //#region queries

        //public void SaveData(IEnumerable<DHFL_Liner> linerList )
        //{
        //    using (var tx = _session.BeginTransaction())
        //    {
        //        foreach (KeyValuePair<string, DHFL_Info> entry in InfoList)
        //        {
        //            _session.SaveOrUpdate(entry.Value);
        //        }
        //        foreach (var liner in linerList)
        //        {
        //            _session.SaveOrUpdate(liner);
        //        }
        //        tx.Commit();
        //    }
        //}
        //#endregion
    }
}
