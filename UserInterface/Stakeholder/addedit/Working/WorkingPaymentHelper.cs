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

        public static SalaryDetails GetSalaryDetails(StkhPayment payment, Dictionary<string, decimal> fixpayData)
        {
            var salDetail = new SalaryDetails();

            salDetail.EmployeePF = payment.FixpayBasic * (fixpayData["EmployeePF"] / 100);
            if (payment.FixpayTotal < 15000)
            {
                salDetail.EmployeeESI = (double)payment.FixpayTotal * (double)(fixpayData["EmployeeESIC"] / 100);
            }
            salDetail.EmployerPF = (double)payment.FixpayBasic * (double)(fixpayData["EmployerPF"] / 100);
            if (payment.FixpayTotal < 15000)
            {
                salDetail.EmployerESI = (double)payment.FixpayTotal * (double)(fixpayData["EmployerESIC"] / 100);
            }

            return salDetail;
        }
    }

    public class SalaryDetails
    {
        public decimal EmployeePF { get; set; }
        public double EmployeeESI { get; set; }
        public double EmployerPF { get; set; }
        public double EmployerESI { get; set; }
    }
}