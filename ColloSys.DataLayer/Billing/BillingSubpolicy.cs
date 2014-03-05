﻿#region References
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
    public class BillingSubpolicy : Entity, IPolicyStatusComponent //, IDateRangeComponent, IProductComponent,
    {
        #region relations

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    if (!NHibernateUtil.IsInitialized(BillingRelations) || forceEmpty) BillingRelations = null;
        //    if (!NHibernateUtil.IsInitialized(BConditions) || forceEmpty) BConditions = null;
        //}
        //// list
        public virtual ISet<BillingRelation> BillingRelations { get; set; }
        public virtual ISet<BCondition> BConditions { get; set; }
        public virtual ISet<BillDetail> BillDetails { get; set; }

        #endregion

        #region property
        public virtual string Name { get; set; }

        public virtual bool IsBasic { get; set; }

        public virtual ScbEnums.Products Products { get; set; }

        public virtual ScbEnums.Category Category { get; set; }

        public virtual ColloSysEnums.PayoutSubpolicyType PayoutSubpolicyType { get; set; }

        public virtual ColloSysEnums.OutputType OutputType { get; set; }

        public virtual string GroupBy { get; set; }
        #endregion

        #region IStatusComponent
        public virtual bool IsActive { get; set; }
        public virtual bool IsInUse { get; set; }
        #endregion

    }
}