using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class StakePaymentQueryBuilder : QueryBuilder<StkhPayment>
    {
        public override IQueryOver<StkhPayment> DefaultQuery(ISession session)
        {
            return QueryOver.Of<StkhPayment>()
                            .Fetch(x => x.Stakeholder).Eager
                            .TransformUsing(Transformers.DistinctRootEntity)
                            .GetExecutableQueryOver(session);
        }
    }
}