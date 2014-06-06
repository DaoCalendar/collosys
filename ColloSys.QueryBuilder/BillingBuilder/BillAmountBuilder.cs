#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class BillAmountBuilder : Repository<BillAmount>
    {
        public override QueryOver<BillAmount, BillAmount> ApplyRelations()
        {
            return QueryOver.Of<BillAmount>();
        }
        [Transaction]
        public BillAmount OnStakeProductMonth(ScbEnums.Products products, Guid stakeId, int month)
        {
           var data= SessionManager.GetCurrentSession().QueryOver<BillAmount>()
                                 .Where(x => x.Stakeholder.Id == stakeId)
                                 .And(x => x.Products == products)
                                 .And(x => x.BillMonth == month)
                                 .SingleOrDefault();
            return data;
        }

        [Transaction]
        public IEnumerable<BillAmount> OnProductMonth(ScbEnums.Products products, uint month)
        {
            return SessionManager.GetCurrentSession().QueryOver<BillAmount>()
                                 .Fetch(x => x.Stakeholder).Eager
                                 .Where(x => x.Products == products)
                                 .And(x => x.BillMonth == month)
                                 .List();
        }
    }
}