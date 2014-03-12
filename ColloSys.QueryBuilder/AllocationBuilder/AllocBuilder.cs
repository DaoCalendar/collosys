#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

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
}
