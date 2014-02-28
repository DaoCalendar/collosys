using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using Iesi.Collections.Generic;
using NHibernate;

namespace ColloSys.DataLayer.Allocation
{
    public class AllocSubpolicy : Entity, IPolicyStatusComponent
    {
        public virtual Stakeholders Stakeholder { get; set; }

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
        //    if (!NHibernateUtil.IsInitialized(Conditions) || forceEmpty) Conditions = null;
        //}

        // ReSharper disable MemberCanBeProtected.Global
        public virtual ISet<AllocRelation> AllocRelations { get; set; }
        public virtual ISet<Alloc> Allocs { get; set; }
        //public virtual ISet<CInfo> CInfos { get; set; }
        //public virtual ISet<RInfo> RInfos { get; set; }
        //public virtual ISet<EInfo> EInfos { get; set; }
        public virtual ISet<AllocCondition> Conditions { get; set; }
        // ReSharper restore MemberCanBeProtected.Global


        #endregion

        #region properties

        public virtual string Name { get; set; }
        
        public virtual ColloSysEnums.AllocationType AllocateType { get; set; }

        public virtual string ReasonNotAllocate { get; set; }

        public virtual UInt32 NoAllocMonth { get; set; }

        public virtual ScbEnums.Products Products { get; set; }

        public virtual ScbEnums.Category Category { get; set; }



        #endregion

        #region IStatusComponent
        public virtual bool IsActive { get; set; }

        public virtual bool IsInUse { get; set; }
        #endregion

        
    }
}