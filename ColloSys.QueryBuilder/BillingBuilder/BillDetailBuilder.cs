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
    public class BillDetailBuilder : Repository<BillDetail>
    {
        public override QueryOver<BillDetail, BillDetail> ApplyRelations()
        {
            return QueryOver.Of<BillDetail>();
        }

        [Transaction]
        public IEnumerable<BillDetail> OnStakeProductMonth(ScbEnums.Products products, Guid stakeId, int month)
        {
            return SessionManager.GetCurrentSession().QueryOver<BillDetail>()
                                 .Fetch(x => x.BillingPolicy).Eager
                                 .Fetch(x => x.BillingSubpolicy).Eager
                                 .Fetch(x => x.PaymentSource).Eager
                                 .Where(x => x.Stakeholder.Id == stakeId)
                                 .And(x => x.Products == products)
                                 .And(x => x.BillMonth == month)
                                 .List();
        }
    }
}