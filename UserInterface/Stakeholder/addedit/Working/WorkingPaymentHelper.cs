using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;

namespace AngularUI.Stakeholder.addedit.Working
{
    public static class WorkingPaymentHelper
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();
        private static readonly HierarchyQueryBuilder HierarchyQuery = new HierarchyQueryBuilder();

        public static IEnumerable<Stakeholders> GetReportsOnreportingLevel(Guid hierarchyId, ColloSysEnums.ReportingLevel level)
        {
            var hierarchyList = new List<Guid>();
            if ((int)level == 0)
            {
                return StakeQuery.GetAllStakeholders();
            }
            var allHierarchies = HierarchyQuery.GetAllHierarchies().ToList();
            for (var i = 1; i <= (int)level; i++)
            {
                var reportsToId = allHierarchies.Single(x => x.Id == hierarchyId).ReportsTo;
                if (reportsToId == Guid.Empty) break;
                hierarchyList.Add(reportsToId);
                hierarchyId = reportsToId;
            }

            //TODO : filter by product
            return StakeQuery.OnHierarchyId(hierarchyList).ToList();
        }
    }
}