#region references

using System;
using System.Collections.Generic;
using System.Globalization;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.StakeholderBuilder;
using System.Linq;
using NHibernate.Transform;

#endregion


namespace BillingService.DBLayer
{
    public static class StakeholderDbLayer
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();

        // get list of stakeholder based on products and billing month
        public static IList<Stakeholders> GetStakeholdersForBilling(ScbEnums.Products products, uint billMonth)
        {
            var startDate = DateTime.ParseExact(string.Format("{0}01", billMonth), "yyyyMMdd",
                                                CultureInfo.InvariantCulture);
            Stakeholders stakeholders = null;
            StkhWorking working = null;
            StkhHierarchy hierarchy = null;
            var query = StakeQuery.DefaultQuery();
            query.JoinQueryOver(() => stakeholders)
                 .JoinQueryOver(() => stakeholders.StkhWorkings, () => working)
                 .JoinQueryOver(() => stakeholders.Hierarchy, () => hierarchy);

            query
                .Where(() => working.Products == products)
                .And(() => hierarchy.IsInAllocation)
                .And(() => hierarchy.IsInField)
                .And(() => (working.EndDate == null || working.EndDate >= startDate));

            var data = StakeQuery.ExecuteQuery(query).ToList();

            return data;
        }
    }
}
