using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using NHibernate.Linq;
using NHibernate.Mapping;
using NLog;

namespace AngularUI.Stakeholder.AddEdit2.Working
{
    public static class WorkingPaymentHelper
    {

        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();



        public static IEnumerable<Stakeholders> GetReportsOnreportingLevel(Guid id, ColloSysEnums.ReportingLevel level)
        {

            List<Stakeholders> stakeholderList;

            switch (level)
            {
                case ColloSysEnums.ReportingLevel.OneLevelUp:
                    stakeholderList = GetReportsToList(id).ToList();
                    return stakeholderList;
                case ColloSysEnums.ReportingLevel.TwoLevelUp:
                    stakeholderList = GetReportsToTwoLevel(id).ToList();
                    return stakeholderList;
                case ColloSysEnums.ReportingLevel.ThreeLevelUp:
                    stakeholderList = GetReportsToThreeLevel(id).ToList();
                    return stakeholderList;
                default:
                    return null;
            }
        }

        private static IEnumerable<Stakeholders> GetReportsToTwoLevel(Guid hierarchyId)
        {
            var secondLevel = GetReportsToList(hierarchyId).ToList();
            secondLevel.AddRange(GetOneLevelUp(secondLevel));
            return secondLevel;
        }

        private static IEnumerable<Stakeholders> GetReportsToThreeLevel(Guid hierarchyId)
        {
            var threeLevelUp = new List<Stakeholders>();

            var oneLevel = GetReportsToList(hierarchyId).ToList();
            threeLevelUp.AddRange(oneLevel);

            var twoLevel = GetOneLevelUp(oneLevel).ToList();
            threeLevelUp.AddRange(twoLevel);

            var thirdLevel = GetOneLevelUp(twoLevel).ToList();
            threeLevelUp.AddRange(thirdLevel);

            return threeLevelUp;
        }

        private static IEnumerable<Stakeholders> GetReportsToList(Guid hierarchyId)
        {
            var data = StakeQuery.OnHieararchyIdWithPayments(hierarchyId).ToList();
            return GetOneLevelUp(data);
        }


        private static IEnumerable<Stakeholders> GetOneLevelUp(IList<Stakeholders> list)
        {
            if (list.Any() && (list.First().Hierarchy.ReportsTo != Guid.Empty))
            {
                var reporttoId = list[0].Hierarchy.ReportsTo;
                if (reporttoId != Guid.Empty)
                {
                    var onelevelupperlist = StakeQuery.OnHierarchyId(reporttoId).ToList();
                    return onelevelupperlist;
                }
            }
            else return list;
            return null;
        }

    }



}