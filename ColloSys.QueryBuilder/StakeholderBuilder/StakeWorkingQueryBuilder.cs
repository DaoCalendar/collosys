#region references

using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;
using NHibernate.Transform;

#endregion

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class StakeWorkingQueryBuilder : Repository<StkhWorking>
    {
        public override QueryOver<StkhWorking,StkhWorking> WithRelation()
        {
            return QueryOver.Of<StkhWorking>()
                            .Fetch(x => x.Stakeholder).Eager
                            .Fetch(x => x.StkhPayment).Eager
                            .TransformUsing(Transformers.DistinctRootEntity);
        }
    }
}