#region references

using System;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class BillingRelationBuilder : QueryBuilder<BillingRelation>
    {
        public override QueryOver<BillingRelation, BillingRelation> WithRelation()
        {
            return QueryOver.Of<BillingRelation>();
        }

        [Transaction]
        public BillingRelation OnSubpolicyId(Guid subpolicyId)
        {
            return SessionManager.GetCurrentSession().QueryOver<BillingRelation>().Where(x => x.BillingSubpolicy.Id == subpolicyId).SingleOrDefault();
        }
    }
}