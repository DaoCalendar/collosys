#region references

using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;

#endregion 

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhPayment : Entity, IApproverComponent
    {
        public virtual Stakeholders Stakeholder { get; set; }

        public virtual ScbEnums.Products Products { get; set; }
        public virtual decimal MobileElig { get; set; }
        public virtual decimal TravelElig { get; set; }
        public virtual decimal FixpayBasic { get; set; }
        public virtual decimal FixpayHra { get; set; }
        public virtual decimal FixpayOther { get; set; }
        public virtual decimal FixpayTotal { get; set; }
        public virtual decimal ServiceCharge { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }

        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
        public virtual Guid OrigEntityId { get; set; }
        public virtual RowStatus RowStatus { get; set; }
    }
}
