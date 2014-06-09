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
                    //stakeholderList = GetReportsToThree(id).ToList();
                    stakeholderList = GetReportsToTwoLevel(id).ToList();
                    return stakeholderList;
                default:
                    return null;
            }
        }

        private static IEnumerable<Stakeholders> GetReportsToTwoLevel(Guid hierarchyId)
        {
            var secondLevel = GetReportsToList(hierarchyId).ToList();
            if (secondLevel.Any() && (secondLevel.First().Hierarchy.Id != Guid.Empty))
            {
                var reporttoId = secondLevel[0].Hierarchy.Id;
                if (reporttoId != Guid.Empty)
                {
                    //var stakeholder = StakeQuery.OnIdWithAllReferences(reporttoId).Hierarchy.Id;

                    var onelevelupperlist = StakeQuery.OnHierarchyId(reporttoId).ToList();
                    secondLevel.AddRange(onelevelupperlist);
                }
            }
            return secondLevel;
        }

        private static IEnumerable<Stakeholders> GetReportsToList(Guid hierarchyId)
        {
            var data = StakeQuery.OnHieararchyIdWithPayments(hierarchyId).ToList();

            if (data.Any() && (data.First().Hierarchy.ReportsTo != Guid.Empty))
            {
                var reporttoId = data[0].Hierarchy.ReportsTo;
                if (reporttoId != Guid.Empty)
                {
                    //var stakeholder = StakeQuery.OnIdWithAllReferences(reporttoId).Hierarchy.Id;

                    var onelevelupperlist = StakeQuery.OnHierarchyId(reporttoId).ToList();
                    return onelevelupperlist;
                }
            }
            else return data;
            return null;
        }


    }



}