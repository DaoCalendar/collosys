#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;

#endregion

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

        public static SalaryDetails GetSalaryDetails(SalaryDetails payment, Dictionary<string, decimal> fixpayData)
        {
            //payment.EmployeePfPct = fixpayData["EmployeePF"];
            payment.EmployeePfPct = 12;
            payment.EmployeePf = payment.FixpayBasic * (payment.EmployeePfPct / 100);

            //payment.EmployerPfPct = fixpayData["EmployerPF"];
            payment.EmployerPfPct = (decimal)13.61;
            payment.EmployerPf = payment.FixpayBasic * (payment.EmployerPfPct / 100);

            //payment.EmployeeEsicPct = payment.FixpayTotal >= 15000 ? 0 : fixpayData["EmployeeESIC"];
            payment.EmployeeEsicPct = payment.FixpayTotal >= 15000 ? 0 : (decimal)1.75;
            payment.EmployeeEsic = payment.FixpayTotal * (payment.EmployeeEsicPct / 100);

            //payment.EmployerEsicPct = payment.FixpayTotal >= 15000 ? 0 : fixpayData["EmployerESIC"];
            payment.EmployerEsicPct = payment.FixpayTotal >= 15000 ? 0 : (decimal)4.75;
            payment.EmployerEsic = payment.FixpayTotal * (payment.EmployerEsicPct / 100);

            var midTotal = payment.FixpayTotal + payment.EmployeeEsic + payment.EmployeePf
                           + payment.EmployerEsic + payment.EmployerPf;

            //TODO : get count of employees from db
            payment.ServiceChargePct = 8;
            payment.ServiceCharge = midTotal * (payment.EmployerEsicPct / 100);

            //payment.ServiceTaxPct = fixpayData["ServiceTax"];
            payment.ServiceTaxPct = (decimal)12.36;
            payment.ServiceTax = midTotal * (payment.ServiceTaxPct / 100);

            payment.TotalPayout = midTotal + payment.ServiceTax + payment.ServiceCharge;

            return payment;
        }
    }
}