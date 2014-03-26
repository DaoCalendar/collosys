#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

#endregion

namespace ColloSys.QueryBuilder.AllocationBuilder
{
    public class AllocBuilder : Repository<Allocations>
    {
        public override QueryOver<Allocations, Allocations> ApplyRelations()
        {
            return QueryOver.Of<Allocations>()
                            .Fetch(x => x.AllocPolicy).Eager
                            .Fetch(x => x.AllocSubpolicy).Eager
                            .Fetch(x => x.Info).Eager
                            .Fetch(x => x.Stakeholder).Eager
                            .TransformUsing(Transformers.DistinctRootEntity);
        }

        [Transaction]
        public IEnumerable<Info> AllocationsForStakeholder(Stakeholders stakeholders)
        {

            return SessionManager.GetCurrentSession().QueryOver<Allocations>()
                                             .Fetch(x => x.Info).Eager
                                             .Fetch(x => x.Stakeholder).Eager
                                             .Where(x => x.Stakeholder != null)
                                             .And(x => x.Stakeholder.Id == stakeholders.Id)
                                             .Select(x => x.Info)
                                             .List<Info>();
        }

        [Transaction]
        public IEnumerable<Allocations> AllocationsForStakeholder(Guid stakeholderId, ScbEnums.Products products)
        {
            return SessionManager.GetCurrentSession().QueryOver<Allocations>()
                                      .Fetch(x => x.AllocPolicy).Eager
                                      .Fetch(x => x.AllocSubpolicy).Eager
                                      .Fetch(x => x.Info).Eager
                                      .Fetch(x => x.Stakeholder).Eager
                                      .Where(x => x.Stakeholder.Id == stakeholderId)
                                      .And(x => x.Info.Product == products)
                                      .List();
        }

        [Transaction]
        public IEnumerable<Allocations> ForBilling(ScbEnums.Products products, bool isInRecovery)
        {
            Allocations alloc = null;
            Info info = null;
            GPincode pincode = null;
            Stakeholders stakeholders = null;
            return SessionManager.GetCurrentSession().QueryOver<Allocations>(() => alloc)
                                    .Fetch(x => x.Info).Eager
                                    .Fetch(x => x.Info.GPincode).Eager
                                    .Fetch(x => x.Stakeholder).Eager
                                    .JoinQueryOver(() => alloc.Info, () => info, JoinType.InnerJoin)
                                    .JoinQueryOver(() => info.GPincode, () => pincode, JoinType.InnerJoin)
                                    .JoinQueryOver(() => alloc.Stakeholder, () => stakeholders, JoinType.InnerJoin)
                                    .Where(() => info.Product == products)
                                    .And(() => info.IsInRecovery == isInRecovery)
                                    .List<Allocations>();

        }

        [Transaction]
        public IEnumerable<Allocations> ForBilling(ScbEnums.Products products, DateTime startDate, DateTime endDate)
        {
            Allocations alloc = null;
            Info info = null;
            GPincode pincode = null;
            Stakeholders stakeholders = null;
            return SessionManager.GetCurrentSession()
                                 .QueryOver<Allocations>(() => alloc)
                                 .Fetch(x => x.Info).Eager
                                 .Fetch(x => x.Info.GPincode).Eager
                                 .Fetch(x => x.Stakeholder).Eager
                                 .JoinQueryOver(() => alloc.Info, () => info, JoinType.InnerJoin)
                                 .JoinQueryOver(() => info.GPincode, () => pincode, JoinType.InnerJoin)
                                 .JoinQueryOver(() => alloc.Stakeholder, () => stakeholders, JoinType.InnerJoin)
                                 .Where(() => info.Product == products)
                                 .And(() => info.AllocStartDate >= startDate && info.AllocEndDate <= endDate)
                                 .List<Allocations>();
        }

        [Transaction]
        public ICriteria CriteriaForEmail()
        {
            var criteria = SessionManager.GetCurrentSession().CreateCriteria(typeof(Allocations), "Alloc");

            criteria.CreateAlias("Alloc.Info", "Info", JoinType.InnerJoin);
            criteria.CreateAlias("Alloc.Stakeholder", "Stakeholder", JoinType.InnerJoin);
            criteria.CreateAlias("Alloc.AllocPolicy", "AllocPolicy", JoinType.InnerJoin);
            criteria.CreateAlias("Alloc.AllocSubpolicy", "AllocSubpolicy", JoinType.InnerJoin);
            //add condition for createdon and alloc status
            criteria.Add(Restrictions.Ge("CreatedOn", DateTime.Today));
            criteria.Add(Restrictions.Le("CreatedOn", DateTime.Today.AddDays(1)));
            criteria.Add(Restrictions.Or(
                Restrictions.Eq("Info.AllocStatus", ColloSysEnums.AllocStatus.AsPerWorking),
                Restrictions.Eq("Info.AllocStatus", ColloSysEnums.AllocStatus.AllocateToStakeholder)));
            return criteria;
        }
    }

    public class AllocGenericCalls
    {
        public static TLinerWriteOff CheckInInfo<TLinerWriteOff>(TLinerWriteOff linerWriteOff)
            where TLinerWriteOff : Entity, IDelinquentCustomer
        {

            return SessionManager.GetCurrentSession().QueryOver<TLinerWriteOff>()
                                 .Where(x => x.AccountNo == linerWriteOff.AccountNo)
                                 .And(x => x.AllocStatus != ColloSysEnums.AllocStatus.None)
                                 .OrderBy(x => x.CreatedOn).Desc
                                 .List()
                                 .FirstOrDefault();
        }
    }
}
