#region references

using System.Linq;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.Generic;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

#endregion


namespace ColloSys.QueryBuilder.AllocationBuilder
{
    public class AllocPolicyBuilder : Repository<AllocPolicy>
    {
        public override QueryOver<AllocPolicy, AllocPolicy> WithRelation()
        {
            return QueryOver.Of<AllocPolicy>();
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

        [Transaction]
        public AllocPolicy NonApproved(ScbEnums.Products products, ScbEnums.Category category)
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
                                 .SingleOrDefault();
        }
    }
}