using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using NHibernate.Transform;

namespace BillingService.DBLayer
{
    public static class StakeholderDbLayer
    {
        // get list of stakeholder based on products and billing month
        public static IList<Stakeholders> GetStakeholdersForBilling(ScbEnums.Products products, uint billMonth)
        {
            var startDate = DateTime.ParseExact(string.Format("{0}01", billMonth), "yyyyMMdd",
                                                CultureInfo.InvariantCulture);
           
            Stakeholders stakeholders = null;
            StkhWorking working = null;
            StkhHierarchy hierarchy = null;
            var session = SessionManager.GetCurrentSession();
            var data = session.QueryOver<Stakeholders>(() => stakeholders)
                              .Fetch(x => x.StkhWorkings).Eager
                              .Fetch(x => x.StkhPayments).Eager
                              .JoinQueryOver(() => stakeholders.StkhWorkings, () => working)
                              .JoinQueryOver(() => stakeholders.Hierarchy, () => hierarchy)
                              .Where(() => working.Products == products)
                              .And(() => hierarchy.IsInAllocation)
                              .And(() => hierarchy.IsInField)
                              .And(() => (working.EndDate == null || working.EndDate >= startDate))
                              .TransformUsing(Transformers.DistinctRootEntity)
                              .List<Stakeholders>();

            return data;
        }
    }
}
