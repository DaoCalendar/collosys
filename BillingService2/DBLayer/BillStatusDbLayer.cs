﻿using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;

namespace BillingService2.DBLayer
{
    internal static class BillStatusDbLayer
    {
        // get list of pending billStatus
        public static IList<BillStatus> GetPendingBillStatus()
        {
            var session = SessionManager.GetCurrentSession();
            var billStatus = session.QueryOver<BillStatus>()
                 .Where(x => x.Status == ColloSysEnums.BillingStatus.Pending).List();

            return billStatus;
        }

        // save list of bill detail
        public static bool SaveDoneBillStatus(BillStatus billStatus)
        {
            billStatus.Status = ColloSysEnums.BillingStatus.Done;

            var session = SessionManager.GetCurrentSession();

            using (var tx = session.BeginTransaction())
            {
                session.SaveOrUpdate(billStatus);
                tx.Commit();
            }

            return true;
        }
    }
}
