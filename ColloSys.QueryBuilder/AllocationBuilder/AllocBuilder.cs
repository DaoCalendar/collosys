#region references

using System;
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

        [Transaction]
        public IEnumerable<Alloc> AllocationsForStakeholder(Guid stakeholderId, ScbEnums.Products products)
        {
           return SessionManager.GetCurrentSession().QueryOver<Alloc>()
                                     .Fetch(x => x.AllocPolicy).Eager
                                     .Fetch(x => x.AllocSubpolicy).Eager
                                     .Fetch(x => x.Info).Eager
                                     .Fetch(x => x.Stakeholder).Eager
                                     .Where(x => x.Stakeholder.Id == stakeholderId)
                                     .And(x => x.Info.Product == products)
                                     .List();
        }
        
        [Transaction]
        public IEnumerable<Alloc> ForBilling(ScbEnums.Products products, bool isInRecovery)
        {
            Alloc alloc = null;
            Info info = null;
            GPincode pincode = null;
            Stakeholders stakeholders = null;
            return SessionManager.GetCurrentSession().QueryOver<Alloc>(() => alloc)
                                    .Fetch(x => x.Info).Eager
                                    .Fetch(x => x.Info.GPincode).Eager
                                    .Fetch(x => x.Stakeholder).Eager
                                    .JoinQueryOver(() => alloc.Info, () => info, JoinType.InnerJoin)
                                    .JoinQueryOver(() => info.GPincode, () => pincode, JoinType.InnerJoin)
                                    .JoinQueryOver(() => alloc.Stakeholder, () => stakeholders, JoinType.InnerJoin)
                                    .Where(() => info.Product == products)
                                    .And(() => info.IsInRecovery == isInRecovery)
                                    .List<Alloc>();
            
        }

        [Transaction]
        public IEnumerable<Alloc> ForBilling(ScbEnums.Products products, DateTime startDate, DateTime endDate)
        {
            Alloc alloc = null;
            Info info = null;
            GPincode pincode = null;
            Stakeholders stakeholders = null;
            return SessionManager.GetCurrentSession().QueryOver<Alloc>(() => alloc)
                                    .Fetch(x => x.Info).Eager
                                     .Fetch(x => x.Info.GPincode).Eager
                                    .Fetch(x => x.Stakeholder).Eager
                                    .JoinQueryOver(() => alloc.Info, () => info, JoinType.InnerJoin)
                                    .JoinQueryOver(() => info.GPincode, () => pincode, JoinType.InnerJoin)
                                    .JoinQueryOver(() => alloc.Stakeholder, () => stakeholders, JoinType.InnerJoin)
                                    .Where(() => info.Product == products)
                                    .And(() => info.AllocStartDate >= startDate && info.AllocEndDate <= endDate)
                                    .List<Alloc>();
        }
    }

    public class AllocConditionBuilder : QueryBuilder<AllocCondition>
    {
        public override QueryOver<AllocCondition, AllocCondition> DefaultQuery()
        {
            return QueryOver.Of<AllocCondition>();
        }

        [Transaction]
        public IEnumerable<AllocCondition> OnSubpolicyId(Guid subpolicyId)
        {
            return SessionManager.GetCurrentSession()
                                 .QueryOver<AllocCondition>()
                                 .Where(x => x.AllocSubpolicy.Id == subpolicyId)
                                 .List();
        }
    }

    public class AllocPolicyBuilder : QueryBuilder<AllocPolicy>
    {
        public override QueryOver<AllocPolicy, AllocPolicy> DefaultQuery()
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

    public class AllocRelationBuilder : QueryBuilder<AllocRelation>
    {
        public override QueryOver<AllocRelation, AllocRelation> DefaultQuery()
        {
            return QueryOver.Of<AllocRelation>();
        }

        [Transaction]
        public AllocRelation OnAllocSubpolicy(AllocSubpolicy subpolicy)
        {
            return SessionManager.GetCurrentSession()
                                 .QueryOver<AllocRelation>()
                                 .Where(x => x.AllocSubpolicy.Id == subpolicy.Id)
                                 .SingleOrDefault();
        }
    }

    public class AllocSubpolicyBuilder : QueryBuilder<AllocSubpolicy>
    {
        public override QueryOver<AllocSubpolicy, AllocSubpolicy> DefaultQuery()
        {
            return QueryOver.Of<AllocSubpolicy>()
                .Fetch(x=>x.Stakeholder).Eager;
        }

        [Transaction]
        public IEnumerable<AllocSubpolicy> OnProductCategorySubPolicyGuids(ScbEnums.Products products, ScbEnums.Category category,
                                                              IEnumerable<Guid> savedAllocSubpolicyIds)
        {
            return SessionManager.GetCurrentSession().QueryOver<AllocSubpolicy>()
                            .Where(x => x.Products == products && x.Category == category)
                            .WhereRestrictionOn(x => x.Id)
                            .Not.IsIn(savedAllocSubpolicyIds.ToList())
                            .Fetch(x => x.Stakeholder).Eager
                            .Fetch(x => x.Conditions).Eager
                            .TransformUsing(Transformers.DistinctRootEntity)
                            .List();
        }

        [Transaction]
        public IEnumerable<AllocSubpolicy> OnProductCategory(ScbEnums.Products products, ScbEnums.Category category)
        {
            return SessionManager.GetCurrentSession()
                                 .QueryOver<AllocSubpolicy>()
                                 .Fetch(x => x.Stakeholder).Eager
                                 .Where(x => x.Products == products && x.Category == category)
                                 .List();
        }
    }
}
