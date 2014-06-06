#region references

using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;

#endregion

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhWorking : Entity
    {
        public virtual Stakeholders Stakeholder { get; set; }
        public virtual GPincode GPincode { get; set; }

        public virtual Guid ReportsTo { get; set; }
        public virtual uint BucketStart { get; set; }
        public virtual uint? BucketEnd { get; set; }
        public virtual string Country { get; set; }
        public virtual string Region { get; set; }
        public virtual string State { get; set; }
        public virtual string Cluster { get; set; }
        public virtual string District { get; set; }
        public virtual string City { get; set; }
        public virtual string Area { get; set; }
        public virtual ScbEnums.Products Products { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual string LocationLevel { get; set; }

        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
    }
}
