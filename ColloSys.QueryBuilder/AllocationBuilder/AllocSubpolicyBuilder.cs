#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.Transform;

#endregion


namespace ColloSys.QueryBuilder.AllocationBuilder
{
    public class AllocSubpolicyBuilder : Repository<AllocSubpolicy>
    {
        public override QueryOver<AllocSubpolicy, AllocSubpolicy> ApplyRelations()
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