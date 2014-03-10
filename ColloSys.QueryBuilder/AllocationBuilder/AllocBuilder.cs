#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.Generic;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
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

        [Transaction]
        public IEnumerable<Info> AllocationsForStakeholder(Stakeholders stakeholders)
        {

            return SessionManager.GetCurrentSession().QueryOver<Alloc>()
                                             .Fetch(x => x.Info).Eager
                                             .Fetch(x => x.Stakeholder).Eager
                                             .Where(x => x.Stakeholder != null)
                                             .And(x => x.Stakeholder.Id == stakeholders.Id)
                                             .Select(x => x.Info)
                                             .List<Info>();
        }
    }

    public class AllocConditionBuilder : QueryBuilder<AllocCondition>
    {
        public override QueryOver<AllocCondition, AllocCondition> DefaultQuery()
        {
            throw new System.NotImplementedException();
        }
    }

    public class AllocPolicyBuilder : QueryBuilder<AllocPolicy>
    {
        public override QueryOver<AllocPolicy, AllocPolicy> DefaultQuery()
        {
            throw new System.NotImplementedException();
        }

        [Transaction]
        public AllocPolicy OnProductAndSystem(ScbEnums.Products products, ScbEnums.Category category)
        {
            AllocPolicy policy = null;
            AllocRelation relation = null;
            AllocSubpolicy subpolicy = null;
            AllocCondition condition = null;
            Stakeholders stakeholder = null;

            return SessionManager.GetCurrentSession().QueryOver(() => policy)
                                     .Fetch(x => x.AllocRelations).Eager
                                     .Fetch(x => x.AllocRelations.First().AllocSubpolicy).Eager
                                     .Fetch(x => x.AllocRelations.First().AllocSubpolicy.Conditions).Eager
                                     .Fetch(x => x.AllocRelations.First().AllocSubpolicy.Stakeholder).Eager
                                     .JoinAlias(() => policy.AllocRelations, () => relation, JoinType.LeftOuterJoin)
                                     .JoinAlias(() => relation.AllocSubpolicy, () => subpolicy, JoinType.LeftOuterJoin)
                                     .JoinAlias(() => subpolicy.Conditions, () => condition, JoinType.LeftOuterJoin)
                                     .JoinAlias(() => subpolicy.Stakeholder, () => stakeholder, JoinType.LeftOuterJoin)
                                     .Where(() => policy.Products == products && policy.Category == category)
                                     .And(() => relation.Status == ColloSysEnums.ApproveStatus.Approved)
                                     .And(() => relation.StartDate <= Util.GetTodayDate() &&
                                                (relation.EndDate == null ||
                                                 relation.EndDate.Value >= Util.GetTodayDate()))
                                     .SingleOrDefault();

        }
    }

    public class AllocRelationBuilder : QueryBuilder<AllocRelation>
    {
        public override QueryOver<AllocRelation, AllocRelation> DefaultQuery()
        {
            throw new System.NotImplementedException();
        }
    }

    public class AllocSubpolicyBuilder : QueryBuilder<AllocSubpolicy>
    {
        public override QueryOver<AllocSubpolicy, AllocSubpolicy> DefaultQuery()
        {
            throw new System.NotImplementedException();
        }
    }
}
