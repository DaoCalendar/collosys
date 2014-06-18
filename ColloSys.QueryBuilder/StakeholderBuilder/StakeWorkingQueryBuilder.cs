#region references

using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;
using NHibernate.Transform;

#endregion

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class StakeWorkingQueryBuilder : Repository<StkhWorking>
    {
        public override QueryOver<StkhWorking,StkhWorking> ApplyRelations()
        {
            return QueryOver.Of<StkhWorking>()
                            .Fetch(x => x.Stakeholder).Eager
                            .TransformUsing(Transformers.DistinctRootEntity);
        }

       
    }
}