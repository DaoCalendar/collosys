#region References
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class Stakeholders : Entity, IApproverComponent
    {
        #region Relation

       // public override void MakeEmpty(bool forceEmpty = false)
       // {
       //     if (!NHibernateUtil.IsInitialized(BillAdhocs) || forceEmpty) BillAdhocs = null;
       //     if (!NHibernateUtil.IsInitialized(BillAmounts) || forceEmpty) BillAmounts = null;
       //     if (!NHibernateUtil.IsInitialized(CAllocs) || forceEmpty) CAllocs = null;
       //     if (!NHibernateUtil.IsInitialized(BillDetails) || forceEmpty) BillDetails = null;
       //     if (!NHibernateUtil.IsInitialized(RAllocs) || forceEmpty) RAllocs = null;
       //     if (!NHibernateUtil.IsInitialized(EAllocs) || forceEmpty) EAllocs = null;
       //     if (!NHibernateUtil.IsInitialized(StkhPayments) || forceEmpty) StkhPayments = null;
       //     if (!NHibernateUtil.IsInitialized(StkhRegistrations) || forceEmpty) StkhRegistrations = null;
       //     if (!NHibernateUtil.IsInitialized(StkhWorkings) || forceEmpty) StkhWorkings = null;
       //     if (!NHibernateUtil.IsInitialized(GAddress) || forceEmpty) GAddress = null;
       //     if (!NHibernateUtil.IsInitialized(AllocSubpolicies) || forceEmpty) AllocSubpolicies = null;
       // }

       //// public virtual StakeAddress GCommAddress { get; set; } //comment this mahendra

        public virtual Iesi.Collections.Generic.ISet<BillAdhoc> BillAdhocs { get; set; }
        public virtual Iesi.Collections.Generic.ISet<BillAmount> BillAmounts { get; set; }
        public virtual Iesi.Collections.Generic.ISet<BillDetail> BillDetails { get; set; }
        public virtual Iesi.Collections.Generic.ISet<Alloc> Allocs { get; set; }
        public virtual IList<StkhPayment> StkhPayments { get; set; }
        public virtual IList<StkhRegistration> StkhRegistrations { get; set; }
        public virtual IList<StkhWorking> StkhWorkings { get; set; }
        public virtual IList<StakeAddress> GAddress { get; set; }
        public virtual Iesi.Collections.Generic.ISet<AllocSubpolicy> AllocSubpolicies { get; set; }

        #endregion

        #region Property

        //public virtual string Hierarchy { get; set; }

        //public virtual string Designation { get; set; }

        public virtual string ExternalId { get; set; }

        public virtual string Name { get; set; }

        public virtual string MobileNo { get; set; }

        public virtual string EmailId { get; set; }

        public virtual string Password { get; set; }

        public virtual Guid ReportingManager { get; set; }

        //public virtual Guid HierarchyId { get; set; }

        //public virtual string LocationLevel { get; set; }

        public virtual DateTime JoiningDate { get; set; }

        public virtual DateTime? LeavingDate { get; set; }

        //public virtual ColloSysEnums.Gender Gender { get; set; }

        //public virtual DateTime? BirthDate { get; set; }

        public virtual StkhHierarchy Hierarchy { get; set; }


        #region track to edit

        //public virtual bool IsAddressChange { get; set; }

        //public virtual bool IsPaymentChange { get; set; }

        //public virtual bool IsWorkingChange { get; set; }

        #endregion

        #endregion

        #region IApprover
        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
        public virtual Guid OrigEntityId { get; set; }
        public virtual RowStatus RowStatus { get; set; }

        #endregion
    }
}
