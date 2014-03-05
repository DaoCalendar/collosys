﻿#region references
using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;
using Iesi.Collections.Generic;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class StkhHierarchy : Entity
    {
        #region properties

        public virtual string Designation { get; set; }

        public virtual string Hierarchy { get; set; }

        public virtual string ApplicationName { get; set; }

        public virtual string LocationLevel { get; set; }

        public virtual Guid ReportsTo { get; set; }

        public virtual int PositionLevel { get; set; }

        public virtual bool IsIndividual { get; set; }

        public virtual bool IsUser { get; set; }

        public virtual bool HasWorking { get; set; }

        public virtual bool HasPayment { get; set; }

        public virtual bool HasBuckets { get; set; }

        public virtual bool HasBankDetails { get; set; }

        public virtual bool HasMobileTravel { get; set; }

        public virtual bool HasVarible { get; set; }

        public virtual bool HasFixed { get; set; }

        public virtual bool HasFixedIndividual { get; set; }

        public virtual bool HasAddress { get; set; }

        public virtual bool HasMultipleAddress { get; set; }

        public virtual bool HasRegistration { get; set; }

        public virtual bool HasServiceCharge { get; set; }

        public virtual bool ManageReportsTo { get; set; }

        public virtual bool IsInAllocation { get; set; }

        public virtual bool IsEmployee { get; set; }

        public virtual bool IsInField { get; set; }

        public virtual ColloSysEnums.ReportingLevel ReportingLevel { get; set; }

        public virtual Guid WorkingReportsTo { get; set; }

        public virtual ColloSysEnums.ReportingLevel WorkingReportsLevel { get; set; }
        #endregion

        #region Relationship
        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    if (!NHibernateUtil.IsInitialized(UsersInRole) || forceEmpty) UsersInRole = null;
        //    if (!NHibernateUtil.IsInitialized(GPermissions) || forceEmpty) GPermissions = null;
        //}

        public virtual ISet<GPermission> GPermissions { get; set; }
        public virtual ISet<Users> UsersInRole { get; set; }
        public virtual ISet<Stakeholders> Stakeholders { get; set; }

        #endregion
    }

    
}