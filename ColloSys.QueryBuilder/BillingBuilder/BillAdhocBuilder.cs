#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class BillAdhocBuilder : Repository<BillAdhoc>
    {
        public override QueryOver<BillAdhoc, BillAdhoc> ApplyRelations()
        {
            return QueryOver.Of<BillAdhoc>()
                            .Fetch(x => x.Stakeholder).Eager
                            .Fetch(x => x.BillDetails).Eager;
        }

        [Transaction]
        public IEnumerable<BillAdhoc> ForStakeholder(Stakeholders stakeholders, ScbEnums.Products products, uint month)
        {
            return SessionManager.GetCurrentSession().QueryOver<BillAdhoc>()
                                 .Where(x => x.Stakeholder.Id == stakeholders.Id
                                             && x.Products == products
                                             && x.StartMonth <= month
                                             && x.EndMonth >= month)
                                 .List<BillAdhoc>();
        }
    }
}