#region References
using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using Iesi.Collections.Generic;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class StakeAddress : Entity, IApproverComponent
    {
        #region Relationship
        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    //if (!NHibernateUtil.IsInitialized(Stakeholders) || forceEmpty) Stakeholders = null;
        //    if (!NHibernateUtil.IsInitialized(GPincodes) || forceEmpty) GPincodes = null;
        //}

        //public virtual ISet<Stakeholders> Stakeholders { get; set; }
       // public virtual ISet<GPincode> GPincodes { get; set; }
        #endregion

        #region Properties

        //public virtual string Source { get; set; }

        //public virtual Guid SourceId { get; set; }

        //public virtual string AddressType { get; set; }

        //public virtual bool IsOfficial { get; set; }

        public virtual string Line1 { get; set; }

        public virtual string Line2 { get; set; }

        public virtual string Line3 { get; set; }

        public virtual string LandlineNo { get; set; }

        public virtual int Pincode { get; set; }

        public virtual string Country { get; set; }

        public virtual string StateCity { get; set; }

        public virtual Stakeholders Stakeholder { get; set; }
        #endregion

        #region Approver Component
        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
        public virtual Guid OrigEntityId { get; set; }
        public virtual RowStatus RowStatus { get; set; }

        #endregion
    }
}