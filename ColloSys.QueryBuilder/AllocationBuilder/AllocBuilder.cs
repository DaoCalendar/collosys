#region references

using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Linq;

#endregion

namespace ColloSys.QueryBuilder.AllocationBuilder
{
    public class AllocBuilder : QueryBuilder<Alloc>
    {
        public override QueryOver<Alloc, Alloc> DefaultQuery()
        {
            return QueryOver.Of<Alloc>()
                            .Fetch(x => x.AllocPolicy).Eager
                            .Fetch(x => x.AllocSubpolicy).Eager
                            .Fetch(x => x.Info).Eager
                            .Fetch(x => x.Stakeholder).Eager
                            .TransformUsing(Transformers.DistinctRootEntity);
        }
    }

    public class AllocConditionBuilder:QueryBuilder<AllocCondition>
    {
        public override QueryOver<AllocCondition, AllocCondition> DefaultQuery()
        {
            throw new System.NotImplementedException();
        }
    }

    public class AllocPolicyBuilder:QueryBuilder<AllocPolicy>
    {
        public override QueryOver<AllocPolicy, AllocPolicy> DefaultQuery()
        {
            throw new System.NotImplementedException();
        }
    }

    public class AllocRelationBuilder:QueryBuilder<AllocRelation>
    {
        public override QueryOver<AllocRelation, AllocRelation> DefaultQuery()
        {
            throw new System.NotImplementedException();
        }
    }

    public class AllocSubpolicyBuilder:QueryBuilder<AllocSubpolicy>
    {
        public override QueryOver<AllocSubpolicy, AllocSubpolicy> DefaultQuery()
        {
            throw new System.NotImplementedException();
        }
    }
}
