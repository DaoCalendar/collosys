#region References

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;

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
        public virtual IList<BillingRelation> BillingRelations { get; set; }
        //public virtual IList<BCondition> BConditions { get; set; }
        public virtual IList<BillDetail> BillDetails { get; set; }
        public virtual IList<BillTokens> BillTokens { get; set; } 

        #endregion

        #region property
        public virtual string Name { get; set; }

        public virtual bool IsBasic { get; set; }

        public virtual ScbEnums.Products Products { get; set; }

        public virtual ScbEnums.Category Category { get; set; }

        public virtual ColloSysEnums.PayoutSubpolicyType PayoutSubpolicyType { get; set; }

        public virtual ColloSysEnums.OutputType OutputType { get; set; }

        public virtual string GroupBy { get; set; }

        public virtual string Description { get; set; }

      
        public virtual ColloSysEnums.PolicyType PolicyType { get; set; }

        #endregion

        #region IStatusComponent
        public virtual bool IsActive { get; set; }
        public virtual bool IsInUse { get; set; }
        #endregion

    }
}