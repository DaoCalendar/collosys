#region references

using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;

#endregion 

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhPayment : Entity, IApprovalStatus
    {
        public virtual Stakeholders Stakeholder { get; set; }

        public virtual decimal MobileElig { get; set; }
        public virtual decimal TravelElig { get; set; }
        public virtual decimal FixpayBasic { get; set; }
        public virtual decimal FixpayGross { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual decimal ServiceCharge { get; set; }

        public virtual ColloSysEnums.ApproveStatus ApprovalStatus { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
    }
}
