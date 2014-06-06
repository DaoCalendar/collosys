#region references

using System;
using ColloSys.DataLayer.Billing;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class BillingRelationBuilder : Repository<BillingRelation>
    {
        public override QueryOver<BillingRelation, BillingRelation> ApplyRelations()
        {
            return QueryOver.Of<BillingRelation>();
        }

        public BillingRelation OnSubpolicyId(Guid subpolicyId)
        {
            var list = FilterBy(x => x.BillingSubpolicy.Id == subpolicyId);
            return list.Count > 0 ? list[0] : null;
        }
    }
}