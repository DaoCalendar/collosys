#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Linq;

#endregion

namespace ColloSys.QueryBuilder.GenericBuilder
{
    public class StkhLeaveRepository : Repository<StkhLeave>
    {
        [Transaction]
        public IEnumerable<StkhLeave> FetchLeaves(Guid stakeholderId)
        {
            var session = SessionManager.GetCurrentSession();
            var list = session.Query<StkhLeave>()
                .Fetch(x => x.Stakeholder)
                .Where(x => x.Stakeholder.Id == stakeholderId)
                .ToList();
            return list;
        }

        [Transaction]
        public IEnumerable<Stakeholders> DelegateToStakeholdersList(Guid stakeholderId)
        {
            var hierarchyQuery = new HierarchyQueryBuilder();
            var stakeQuery = new StakeQueryBuilder();
            var stkh = stakeQuery.GetById(stakeholderId);
            var hierarchyList = new List<Guid> { stkh.Hierarchy.Id };

            var allHierarchies = hierarchyQuery.GetAllHierarchies().ToList();

            if (stkh.Hierarchy.ReportsTo != Guid.Empty)
            {
                var reportsToHierarchy = allHierarchies.Single(x => x.Id == stkh.Hierarchy.ReportsTo);
                hierarchyList.Add(reportsToHierarchy.Id);
            }
            else
            {
                var reportingHierarchies = allHierarchies.Where(x => x.ReportsTo == stkh.Hierarchy.Id);
                reportingHierarchies.ForEach(x => hierarchyList.Add(x.Id));
            }

            var stkhList = stakeQuery.OnHierarchyId(hierarchyList).ToList();
            return stkhList;
        }
    }
}
