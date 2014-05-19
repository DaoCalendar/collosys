#region references

using System;
using System.Collections.Generic;
using System.Globalization;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.Transform;

#endregion


namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class BillingSubpolicyBuilder : Repository<BillingSubpolicy>
    {
        public override QueryOver<BillingSubpolicy, BillingSubpolicy> ApplyRelations()
        {
            return QueryOver.Of<BillingSubpolicy>();
        }

        [Transaction]
        public IEnumerable<BillingSubpolicy> SubpolicyOnPolicy(BillingPolicy billingPolicy, uint billMonth)
        {
            var startDate = DateTime.ParseExact(string.Format("{0}01", billMonth), "yyyyMMdd",
                                                CultureInfo.InvariantCulture);

            if (billingPolicy == null)
                return new List<BillingSubpolicy>();

            return SessionManager.GetCurrentSession().QueryOver<BillingRelation>()
                                 .Fetch(x => x.BillingSubpolicy).Eager
                                 .Fetch(x => x.BillingSubpolicy.BConditions).Eager
                                 .Where(x => x.BillingPolicy.Id == billingPolicy.Id)
                                 .And(x => (x.EndDate == null || x.EndDate >= startDate))
                                 .Select(x => x.BillingSubpolicy)
                                 .OrderBy(x => x.Priority).Asc
                                 .List<BillingSubpolicy>();
        }

        [Transaction]
        public IEnumerable<BillingSubpolicy> FormulaOnProductCategory(ScbEnums.Products product)

        {
            return SessionManager.GetCurrentSession().QueryOver<BillingSubpolicy>()
                .Fetch(x => x.BillTokens).Eager
                .Where(c => c.Products == product
                            && c.PayoutSubpolicyType == ColloSysEnums.PayoutSubpolicyType.Formula)
                .TransformUsing(Transformers.DistinctRootEntity)
                .List();
        }

        [Transaction]
        public BillingSubpolicy FormulaOnProductAndName(ScbEnums.Products products, string formulaName)
        {

            return SessionManager.GetCurrentSession().QueryOver<BillingSubpolicy>()
                                 .Fetch(x => x.BConditions).Eager
                                 .Where(x => x.Products == products
                                             && x.PayoutSubpolicyType == ColloSysEnums.PayoutSubpolicyType.Formula)
                                 .And(x => x.Name == formulaName)
                                 .SingleOrDefault();
        }

        [Transaction]
        public IEnumerable<BillingSubpolicy> SubPoliciesInDb(ScbEnums.Products products, ScbEnums.Category category, List<Guid> savedSubnpoliciesIds)
        {
            return SessionManager.GetCurrentSession().QueryOver<BillingSubpolicy>()
                                 .Where(x => x.PayoutSubpolicyType == ColloSysEnums.PayoutSubpolicyType.Subpolicy
                                             && x.Products == products && x.Category == category)
                                 .WhereRestrictionOn(x => x.Id)
                                 .Not.IsIn(savedSubnpoliciesIds)
                                 .Fetch(x => x.BConditions).Eager
                                 .Fetch(x => x.BillingRelations).Eager
                                 .TransformUsing(Transformers.DistinctRootEntity)
                                 .List();
        }

        [Transaction]
        public IEnumerable<BillingSubpolicy> OnProductCategory(ScbEnums.Products product)
        {
            return SessionManager.GetCurrentSession()
                                 .QueryOver<BillingSubpolicy>()
                                 .Fetch(x=>x.BillTokens).Eager
                                 .Where(c => c.Products == product )
                                 .TransformUsing(Transformers.DistinctRootEntity)
                                 .List();
        }
    }
}