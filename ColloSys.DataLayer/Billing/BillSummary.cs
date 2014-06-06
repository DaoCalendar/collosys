using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;

namespace ColloSys.DataLayer.Billing
{
    public class BillSummary : Entity
    {
        public virtual Stakeholders Stakeholder { get; set; }
        public virtual ScbEnums.Products Products { get; set; }

        public virtual uint BillMonth { get; set; }
        public virtual uint OriginMonth { get; set; }
        public virtual uint Cycle { get; set; }

        public virtual decimal FixedAmount { get; set; }
        public virtual decimal VariableAmount { get; set; }
        public virtual decimal Deductions { get; set; }
        public virtual decimal TaxAmount { get; set; }
        public virtual decimal CappingDeduction { get; set; }
        public virtual decimal ProcFeeDeduction { get; set; }
        public virtual decimal TotalAmount { get; set; }

        public virtual ColloSysEnums.BillPaymentStatus PayStatus { get; set; }
        public virtual DateTime PayStatusDate { get; set; }
        public virtual string PayStatusHistory { get; set; }

        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
    }
}