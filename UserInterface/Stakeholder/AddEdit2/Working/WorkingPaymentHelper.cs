using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;

namespace AngularUI.Stakeholder.AddEdit2.Working
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
                hierarchyList.Add(allHierarchies.Single(x => x.Id == hierarchyId).ReportsTo);
                hierarchyId = hierarchyList.Last();
            }
            //TODO : filter by product
            return StakeQuery.OnHierarchyId(hierarchyList).ToList();
        }
       
    }
}