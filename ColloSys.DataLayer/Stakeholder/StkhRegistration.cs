#region References
using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class StkhRegistration : Entity,IApproverComponent
    {
        #region Properties

        public virtual Stakeholders Stakeholder { get; set; }

        public virtual bool HasCollector { get; set; }

        public virtual string RegistrationNo { get; set; }

        public virtual string PanNo { get; set; }

        public virtual string TanNo { get; set; }

        public virtual string ServiceTaxno { get; set; }

        #endregion

        #region Approver
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
