#region references

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.SharedDomain
{
    public class Alloc : Entity, IApproverComponent
    {
        #region Relation
        public virtual AllocPolicy AllocPolicy { get; set; }
        public virtual AllocSubpolicy AllocSubpolicy { get; set; }
        public virtual Stakeholders Stakeholder { get; set; }
        public virtual Info Info { get; set; }
        #endregion

        #region Date Range Component
        public virtual DateTime StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }
        #endregion

        public virtual bool WithTelecalling { get; set; }

        #region Allocation Component
        public virtual bool IsAllocated { get; set; }

        public virtual Int32 Bucket { get; set; }

        public virtual decimal AmountDue { get; set; }

        public virtual string ChangeReason { get; set; }

        public virtual ColloSysEnums.AllocStatus AllocStatus { get; set; }

        public virtual ColloSysEnums.NoAllocResons? NoAllocResons { get; set; }

        #endregion

        #region Approver
        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
        public virtual Guid OrigEntityId { get; set; }
        public virtual RowStatus RowStatus { get; set; }

        #endregion

       
    }
}

//#region Relationship None
//public override void MakeEmpty(bool forceEmpty = false)
//{
//    return;
//}
//#endregion