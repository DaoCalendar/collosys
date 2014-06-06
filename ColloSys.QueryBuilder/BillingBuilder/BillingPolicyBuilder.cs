#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.Linq;

#endregion


namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class BillingPolicyBuilder : Repository<BillingPolicy>
    {
        public override QueryOver<BillingPolicy, BillingPolicy> ApplyRelations()
        {
            return QueryOver.Of<BillingPolicy>();
        }

        [Transaction]
        public BillingPolicy OnProductCategory(ScbEnums.Products products)
        {
            return SessionManager.GetCurrentSession().Query<BillingPolicy>()
                                 .Where(x => x.Products == products)
                                 .FetchMany(x => x.BillingRelations)
                                 .ThenFetch(r => r.BillingSubpolicy)
                                 .SingleOrDefault();
        }

        [Transaction]
        public BillingPolicy OnProductCategory(ScbEnums.Products products, ScbEnums.Category category)
        {
            return SessionManager.GetCurrentSession().Query<BillingPolicy>()
                                 .Where(x => x.Products == products && x.Category == category)
                                 .FetchMany(x => x.BillingRelations)
                                 .ThenFetch(r => r.BillingSubpolicy)
                                 .SingleOrDefault();
        }

        [Transaction]
        public IEnumerable<BillingPolicy> LinePolicies(ScbEnums.Products products)
        {
            return SessionManager.GetCurrentSession().QueryOver<BillingPolicy>()
                                 .Where(x => x.Products == products && x.Category == ScbEnums.Category.Liner)
                                 .List();
        }

        [Transaction]
        public IEnumerable<BillingPolicy> WriteoffPolicies(ScbEnums.Products products)
        {
            return SessionManager.GetCurrentSession().QueryOver<BillingPolicy>()
                                 .Where(x => x.Products == products && x.Category == ScbEnums.Category.WriteOff)
                                 .List();
        }
    }
}