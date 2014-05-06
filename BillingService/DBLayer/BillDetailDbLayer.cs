using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.SessionMgr;

namespace BillingService.DBLayer
{
    internal static class BillDetailDbLayer
    {
        // save list of bill detail
        public static bool SaveBillDetailsBillAmount(IList<BillDetail> billDetails, BillAmount billAmount,List<CustBillViewModel> custBillViewModels)
        {
            var session = SessionManager.GetCurrentSession();

            using (var tx = session.BeginTransaction())
            {
                foreach (var entity in billDetails)
                {
                    session.SaveOrUpdate(entity);
                }

                foreach (var entity in custBillViewModels)
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