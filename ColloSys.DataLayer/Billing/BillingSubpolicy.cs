#region References

using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using Iesi.Collections.Generic;

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
        public virtual ISet<BillTokens> BillTokens { get; set; } 

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

        public virtual Guid? ProcessingFee { get; set; }
        
        public virtual Guid? PayoutCapping { get; set; }

        #endregion

        #region IStatusComponent
        public virtual bool IsActive { get; set; }
        public virtual bool IsInUse { get; set; }
        #endregion

    }
}