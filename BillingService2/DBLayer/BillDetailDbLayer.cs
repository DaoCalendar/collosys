using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.SessionMgr;

namespace BillingService2.DBLayer
{
    internal static class BillDetailDbLayer
    {
        // save list of bill detail
        public static bool SaveBillDetailsBillAmount(BillStatus billStatus, IList<BillDetail> billDetails, BillAmount billAmount,List<DHFL_Liner> dhflLiners)
        {
            var session = SessionManager.GetCurrentSession();

            using (var tx = session.BeginTransaction())
            {
                session.SaveOrUpdate(billStatus);

                foreach (var entity in billDetails)
                {
                    session.SaveOrUpdate(entity);
                }

                foreach (var entity in dhflLiners)
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