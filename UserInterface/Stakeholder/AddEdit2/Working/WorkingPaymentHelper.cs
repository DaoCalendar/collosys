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


        //public static IEnumerable<Stakeholders> GetReportsOnreportingLevel(Guid id, ColloSysEnums.ReportingLevel level)
        //{

        //    List<Stakeholders> stakeholderList;

        //    switch (level)
        //    {
        //        case ColloSysEnums.ReportingLevel.OneLevelUp:
        //            stakeholderList = GetOneLevelUp(id).ToList();
        //            return stakeholderList;
        //        case ColloSysEnums.ReportingLevel.TwoLevelUp:
        //            stakeholderList = GetReportsToTwoLevel(id).ToList();
        //            return stakeholderList;
        //        case ColloSysEnums.ReportingLevel.ThreeLevelUp:
        //            stakeholderList = GetReportsToThreeLevel(id).ToList();
        //            return stakeholderList;
        //        default:
        //            return null;
        //    }
        //}


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
            return StakeQuery.OnHierarchyId(hierarchyList).ToList();
        }

        //private static IEnumerable<Stakeholders> GetReportsToTwoLevel(Guid hierarchyId)
        //{
        //    var twoLevelUp = new List<Stakeholders>();

        //    var oneLevelUp = GetOneLevelUp(hierarchyId);
        //    twoLevelUp.AddRange(oneLevelUp);
        //    var oneLevelUpHierId = GetOneLevelUpHierarchy(hierarchyId).Id;

        //    var secondLevel = GetOneLevelUp(oneLevelUpHierId);
        //    twoLevelUp.AddRange(secondLevel);

        //    return twoLevelUp;
        //}

        //private static IEnumerable<Stakeholders> GetReportsToThreeLevel(Guid hierarchyId)
        //{
        //    var threeLevelUp = new List<Stakeholders>();

        //    var oneLevelUp = GetOneLevelUp(hierarchyId);
        //    threeLevelUp.AddRange(oneLevelUp);
        //    var oneLevelUpHierId = GetOneLevelUpHierarchy(hierarchyId).Id;

        //    var secondLevel = GetOneLevelUp(oneLevelUpHierId);
        //    threeLevelUp.AddRange(secondLevel);
        //    var twoLevelUpHierId = GetOneLevelUpHierarchy(oneLevelUpHierId).Id;

        //    var thirdLevel = GetOneLevelUp(twoLevelUpHierId).ToList();
        //    threeLevelUp.AddRange(thirdLevel);

        //    return threeLevelUp;
        //}



        //private static StkhHierarchy GetOneLevelUpHierarchy(Guid hierarchyId)
        //{
        //    var currHier = HierarchyQuery.OnHierarchyId(hierarchyId);
        //    if (currHier.ReportsTo == Guid.Empty)
        //    {
        //        return new StkhHierarchy();
        //    }
        //    return HierarchyQuery.OnHierarchyId(currHier.ReportsTo);
        //}

        //private static IEnumerable<Stakeholders> GetOneLevelUp(Guid id)
        //{
        //    if (id == Guid.Empty) return new List<Stakeholders>();
        //    var hierId = GetOneLevelUpHierarchy(id).Id;
        //    if (hierId != Guid.Empty)
        //    {
        //        var onelevelupperlist = StakeQuery.OnHierarchyId(hierId).ToList();
        //        return onelevelupperlist;
        //    }
        //    return new List<Stakeholders>();
        //}
    }
}