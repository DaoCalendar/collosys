#region references
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class BFormula : Entity, IApproverComponent
    {
        #region Properties
        public virtual string Name { get; set; }

        public virtual ScbEnums.Products Products { get; set; }

        public virtual ScbEnums.Category Category { get; set; }
        #endregion

        #region IApprove
        public virtual ColloSysEnums.ApproveStatus Status { get; set; }

        public virtual string Description { get; set; }

        public virtual string ApprovedBy { get; set; }

        public virtual DateTime? ApprovedOn { get; set; }
        #endregion

        #region relationships - none

        protected override void InitRelationships()
        {
            return;
        }

        public override void MakeEmpty(bool forceEmpty = false)
        {
            return;
        }

        #endregion
    }
}