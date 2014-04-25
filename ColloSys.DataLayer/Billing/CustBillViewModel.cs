using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Billing
{
    public class CustBillViewModel : Entity, ICustBillViewModel
    {
        public virtual string AccountNo { get; set; }

        public virtual string GlobalCustId { get; set; }

        public virtual ColloSysEnums.DelqFlag Flag { get; set; }

        public virtual ScbEnums.Products Product { get; set; }

        public virtual bool IsInRecovery { get; set; }

        public virtual DateTime? ChargeofDate { get; set; }

        public virtual uint Cycle { get; set; }

        public virtual uint Bucket { get; set; }

        public virtual uint MobWriteoff { get; set; }

        public virtual uint Vintage { get; set; }

        public virtual ColloSysEnums.CityCategory CityCategory { get; set; }

        public virtual string City { get; set; }

        public virtual bool IsXHoldAccount { get; set; }

        public virtual DateTime AllocationStartDate { get; set; }

        public virtual DateTime? AllocationEndDate { get; set; }

        public virtual decimal TotalDueOnAllocation { get; set; }

        public virtual decimal TotalAmountRecovered { get; set; }

        public virtual decimal ResolutionPercentage { get; set; }

        public virtual GPincode GPincode { get; set; }

        public virtual Stakeholders Stakeholders { get; set; }

        public virtual string ConditionSatisfy { get; set; }

        public virtual BillDetail BillDetail { get; set; }
    }
}
