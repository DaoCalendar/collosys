using System;

namespace AngularUI.Stakeholder.addedit.Working
{
    public class SalaryDetails
    {
        public Guid AgencyId { get; set; } //report to id, input from client
        public decimal FixpayBasic { get; set; } //input from client
        public decimal FixpayGross { get; set; } //input from client
        public decimal EmployeePfPct { get; set; }
        public decimal EmployeePf { get; set; }
        public decimal EmployerPfPct { get; set; }
        public decimal EmployeeEsicPct { get; set; }
        public decimal EmployerEsicPct { get; set; }
        public decimal EmployeeEsic { get; set; }
        public decimal EmployerPf { get; set; }
        public decimal EmployerEsic { get; set; }
        public decimal ServiceChargePct { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal ServiceTaxPct { get; set; }
        public decimal ServiceTax { get; set; }
        public decimal FixpayTotal { get; set; }
        public uint ReporteeCount { get; set; }
    }
}