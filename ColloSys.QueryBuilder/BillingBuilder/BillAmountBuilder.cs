#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class BillAmountBuilder : Repository<BillSummary>
    {
        public override QueryOver<BillSummary, BillSummary> ApplyRelations()
        {
            return QueryOver.Of<BillSummary>();
        }
        [Transaction]
        public BillSummary OnStakeProductMonth(ScbEnums.Products products, Guid stakeId, int month)
        {
           var data= SessionManager.GetCurrentSession().QueryOver<BillSummary>()
                                 .Where(x => x.Stakeholder.Id == stakeId)
                                 .And(x => x.Products == products)
                                 .And(x => x.BillMonth == month)
                                 .SingleOrDefault();
            return data;
        }

        [Transaction]
        public IEnumerable<BillSummary> OnProductMonth(ScbEnums.Products products, uint month)
        {
            return SessionManager.GetCurrentSession().QueryOver<BillSummary>()
                                 .Fetch(x => x.Stakeholder).Eager
                                 .Where(x => x.Products == products)
                                 .And(x => x.BillMonth == month)
                                 .List();
        }
    }
}