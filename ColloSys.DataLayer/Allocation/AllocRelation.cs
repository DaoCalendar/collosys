#region references

using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Allocation
{
    public class AllocRelation : Entity, IApproverComponent
    {
        public virtual AllocPolicy AllocPolicy { get; set; }
        public virtual AllocSubpolicy AllocSubpolicy { get; set; }

        #region Properties

        public virtual uint Priority { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        #endregion

        #region IApprove
        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
        public virtual Guid OrigEntityId { get; set; }
        public virtual RowStatus RowStatus { get; set; }
        #endregion
        //#region Relationship None

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    return;
        //}

        //#endregion
    }
}