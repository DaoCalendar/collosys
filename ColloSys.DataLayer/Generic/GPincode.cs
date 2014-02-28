#region references
using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using Iesi.Collections.Generic;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class GPincode : Entity
    {

        #region relationships
        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    if (!NHibernateUtil.IsInitialized(CLiners) || forceEmpty) CLiners = null;
        //    if (!NHibernateUtil.IsInitialized(CWriteoffs) || forceEmpty) CWriteoffs = null;
        //    if (!NHibernateUtil.IsInitialized(RLiners) || forceEmpty) RLiners = null;
        //    if (!NHibernateUtil.IsInitialized(RWriteoffs) || forceEmpty) RWriteoffs = null;
        //    if (!NHibernateUtil.IsInitialized(ELiners) || forceEmpty) ELiners = null;
        //    if (!NHibernateUtil.IsInitialized(EWriteoffs) || forceEmpty) EWriteoffs = null;
        //}

        public virtual ISet<CLiner> CLiners { get; set; }
        public virtual ISet<CWriteoff> CWriteoffs { get; set; }
        public virtual ISet<Info> Infos { get; set; }
        public virtual ISet<RLiner> RLiners { get; set; }
        public virtual ISet<RWriteoff> RWriteoffs { get; set; }
        public virtual ISet<ELiner> ELiners { get; set; }
        public virtual ISet<EWriteoff> EWriteoffs { get; set; }

        #endregion

        #region Properties
        public virtual string Country { get; set; }

        public virtual uint Pincode { get; set; }

        public virtual string Area { get; set; }

        public virtual string City { get; set; }

        public virtual string District { get; set; }

        public virtual string Cluster { get; set; }

        public virtual string State { get; set; }

        public virtual string Region { get; set; }

        public virtual bool IsInUse { get; set; }

        public virtual ColloSysEnums.CityCategory CityCategory { get; set; }

      //  public virtual StakeAddress StakeAddress { get; set; }
        #endregion

       
    }
}
