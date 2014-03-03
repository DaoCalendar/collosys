using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;

namespace BillingService.DBLayer
{
    internal static class BillDetailDbLayer
    {
        // save list of bill detail
        public static bool SaveBillDetailsBillAmount(IList<BillDetail> billDetails, BillAmount billAmount)
        {
            var session = SessionManager.GetCurrentSession();

            using (var tx = session.BeginTransaction())
            {
                foreach (var entity in billDetails)
                {
                    session.SaveOrUpdate(entity);
                }

                session.SaveOrUpdate(billAmount);

                tx.Commit();
            }

            return true;
        }

    }
}