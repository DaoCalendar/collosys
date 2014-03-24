#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BillingBuilder;

#endregion


namespace BillingService.DBLayer
{
    internal static class BillStatusDbLayer
    {
        private static readonly BillStatusBuilder BillStatusBuilder=new BillStatusBuilder();

        // get list of pending billStatus
        public static IList<BillStatus> GetPendingBillStatus()
        {
            var billStatus = BillStatusBuilder.FilterBy(x => x.Status == ColloSysEnums.BillingStatus.Pending);
            return billStatus;
        }

        // save list of bill detail
        public static bool SaveDoneBillStatus(BillStatus billStatus)
        {
            billStatus.Status = ColloSysEnums.BillingStatus.Done;
            BillStatusBuilder.Save(billStatus);
            return true;
        }
    }
}
