using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using Iesi.Collections.Generic;
using NHibernate;

namespace ColloSys.DataLayer.Allocation
{
    public class AllocPolicy : Entity, IApproverComponent
    {
        #region relations

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    if (!NHibernateUtil.IsInitialized(AllocRelations) || forceEmpty) AllocRelations = null;
        //    //if (!NHibernateUtil.IsInitialized(CInfos) || forceEmpty) CInfos = null;
        //    if (!NHibernateUtil.IsInitialized(CAllocs) || forceEmpty) CAllocs = null;
        //    //if (!NHibernateUtil.IsInitialized(RInfos) || forceEmpty) RInfos = null;
        //    if (!NHibernateUtil.IsInitialized(RAllocs) || forceEmpty) RAllocs = null;
        //    //if (!NHibernateUtil.IsInitialized(EInfos) || forceEmpty) EInfos = null;
        //    if (!NHibernateUtil.IsInitialized(EAllocs) || forceEmpty) EAllocs = null;
        //}

        public virtual ISet<AllocRelation> AllocRelations { get; set; }
        public virtual ISet<CAlloc> CAllocs { get; set; }
        public virtual ISet<EAlloc> EAllocs { get; set; }
        public virtual ISet<RAlloc> RAllocs { get; set; }
        //public virtual ISet<CInfo> CInfos { get; set; }
        //public virtual ISet<RInfo> RInfos { get; set; }
        //public virtual ISet<EInfo> EInfos { get; set; }

        #endregion

        #region properties

        public virtual string Name { get; set; }

        public virtual ScbEnums.Products Products { get; set; }

        public virtual ScbEnums.Category Category { get; set; }

        #endregion

        #region Approve
        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
        public virtual Guid OrigEntityId { get; set; }
        public virtual RowStatus RowStatus { get; set; }

        #endregion
    }
}