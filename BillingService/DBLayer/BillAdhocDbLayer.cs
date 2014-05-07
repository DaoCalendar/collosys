using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using NLog;

namespace BillingService.DBLayer
{
    internal static class BillAdhocDbLayer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // save list of bill detail
        public static IList<BillAdhoc> GetBillAdhocForStkholder(Stakeholders stakeholder, ScbEnums.Products products, uint month)
        {
            try
            {
                var session = SessionManager.GetCurrentSession();

                var billAdhocs = session.QueryOver<BillAdhoc>()
                                        .Where(x => x.Stakeholder.Id == stakeholder.Id
                                                    && x.Products == products
                                                    && x.StartMonth <= month
                                                    && x.EndMonth >= month)
                                        .List<BillAdhoc>();
                return billAdhocs;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("BillAdhocDbLayer.GetBillAdhocForMonth() : {0}", ex.Message));

                return new List<BillAdhoc>();
            }
        }

        public static bool SaveBillAdhoc(IList<BillAdhoc> billAdhocs)
        {
            var session = SessionManager.GetCurrentSession();

            using (var tx = session.BeginTransaction())
            {
                foreach (var entity in billAdhocs)
                {
                    session.SaveOrUpdate(entity);
                }

                tx.Commit();
            }

            return true;
        }
    }
}