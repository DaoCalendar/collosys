using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Transform;

namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class BillTokenBuilder : Repository<BillTokens>
    {
        [Transaction]
        public IEnumerable<BillTokens> FormulaTokens(Guid formulaId)
        {
            return SessionManager.GetCurrentSession()
                .QueryOver<BillTokens>()
                .Fetch(x => x.BillingSubpolicy).Eager
                .Where(x => x.BillingSubpolicy.Id == formulaId)
                .TransformUsing(Transformers.DistinctRootEntity)
                .List();
        }
    }
}
