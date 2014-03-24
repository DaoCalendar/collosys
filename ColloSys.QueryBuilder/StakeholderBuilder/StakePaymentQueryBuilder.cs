using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class StakePaymentQueryBuilder : Repository<StkhPayment>
    {
        public override QueryOver<StkhPayment, StkhPayment> ApplyRelations()
        {
            return QueryOver.Of<StkhPayment>()
                        .Fetch(x => x.Stakeholder).Eager
                        .TransformUsing(Transformers.DistinctRootEntity);
        }
    }
}