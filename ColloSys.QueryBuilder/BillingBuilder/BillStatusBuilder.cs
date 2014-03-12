#region references

using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class BillStatusBuilder : QueryBuilder<BillStatus>
    {
        public override QueryOver<BillStatus, BillStatus> WithRelation()
        {
            return QueryOver.Of<BillStatus>();
        }

        [Transaction]
        public BillStatus OnProductMonth(ScbEnums.Products products, uint month)
        {
            return SessionManager.GetCurrentSession()
                                 .QueryOver<BillStatus>()
                                 .Where(x => x.Products == products && x.BillMonth == month)
                                 .SingleOrDefault();
        }
    }
}