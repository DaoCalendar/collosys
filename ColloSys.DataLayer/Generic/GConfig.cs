#region References
using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class GConfig : Entity, IApproverComponent
    {
        #region Properties

        public virtual string ParamName { get; set; }

        public virtual string Value { get; set; }

        public virtual string ParamCategory { get; set; }

        public virtual string ParamSubcategory { get; set; }

        #endregion

        #region Approver Component
        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
        public virtual Guid OrigEntityId { get; set; }
        public virtual RowStatus RowStatus { get; set; }

        #endregion

        //#region Relationship None
        //public override void MakeEmpty(bool forceEmpty = false) { }
        //#endregion
    }
}