#region references

using System;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class BillAmountBuilder : Repository<BillAmount>
    {
        public override QueryOver<BillAmount, BillAmount> WithRelation()
        {
            return QueryOver.Of<BillAmount>();
        }
        [Transaction]
        public BillAmount OnStakeProductMonth(ScbEnums.Products products, Guid stakeId, int month)
        {
            return SessionManager.GetCurrentSession().QueryOver<BillAmount>()
                                 .Where(x => x.Stakeholder.Id == stakeId)
                                 .And(x => x.Products == products)
                                 .And(x => x.Month == month)
                                 .SingleOrDefault();
        }
    }
}