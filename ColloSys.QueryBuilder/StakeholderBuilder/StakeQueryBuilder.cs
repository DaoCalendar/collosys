#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.Generic;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;

#endregion

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class StakeQueryBuilder : Repository<Stakeholders>
    {
        [Transaction]
        public IList<Stakeholders> OnProduct(ScbEnums.Products products)
        {
            Stakeholders stakeholders = null;
            StkhWorking workings = null;
            StkhHierarchy hierarchy = null;
            StkhPayment payment = null;
            var session = SessionManager.GetCurrentSession();
            var data = session.QueryOver(() => stakeholders)
                              .Fetch(x => x.StkhWorkings).Eager
                              .Fetch(x => x.StkhPayments).Eager
                              .Fetch(x => x.Hierarchy).Eager
                              .JoinAlias(() => stakeholders.StkhPayments, () => payment, JoinType.LeftOuterJoin)
                              .JoinAlias(() => stakeholders.StkhWorkings, () => workings, JoinType.LeftOuterJoin)
                              .JoinAlias(() => stakeholders.Hierarchy, () => hierarchy,
                                         JoinType.LeftOuterJoin)
                              .Where(() => workings.Products == products)
                              .And(() => hierarchy.IsInAllocation)
                //.And(() => hierarchy.IsInField)
                              .And(() => stakeholders.JoiningDate < Util.GetTodayDate())
                              .And(() => stakeholders.LeavingDate == null ||
                                         stakeholders.LeavingDate > Util.GetTodayDate())
                              .And(() => stakeholders.Status == ColloSysEnums.ApproveStatus.Approved)
                              .TransformUsing(Transformers.DistinctRootEntity)
                              .List();
            data.ForEach(x =>
            {
                x.Allocs = null;
                x.AllocSubpolicies = null;
            });
            return data;

        }

        [Transaction]
        public IList<Stakeholders> GetStakeByExtId(string id)
        {
            var session = SessionManager.GetCurrentSession();

            var stakeholder = session.QueryOver<Stakeholders>()
                .Where(x => x.ExternalId == id)
                .Fetch(x => x.StkhWorkings).Eager
                .Fetch(x => x.StkhPayments).Eager
                .Fetch(x => x.Hierarchy).Eager.List();
            return stakeholder;
        }


        [Transaction]
        public IList<Stakeholders> GetAllStakeholders()
        {
            var session = SessionManager.GetCurrentSession();

            var stakeholder = session.Query<Stakeholders>()
                .Fetch(x => x.Hierarchy)
                .Where(x => x.LeavingDate == null || x.LeavingDate > DateTime.Today)
                .ToList();
            return stakeholder;
        }

        [Transaction]
        public IList<Stakeholders> GetStakeByName(string name)
        {
            var session = SessionManager.GetCurrentSession();

            var stakeholder = session.QueryOver<Stakeholders>()
                .Where(x => x.Name == name)
                .Fetch(x => x.StkhWorkings).Eager
                .Fetch(x => x.StkhPayments).Eager
                .Fetch(x => x.Hierarchy).Eager.List();
            return stakeholder;
        }

        [Transaction]
        public IList<Stakeholders> ListForAdhoc(string name, ScbEnums.Products products)
        {
            var list = OnProduct(products);
            var data = list.Where(x => x.Name.ToString().StartsWith(name)).Take(10).ToList();
            return data;
        }

        [Transaction]
        public IList<Stakeholders> ExitedOnProduct(ScbEnums.Products products)
        {
            Stakeholders stakeholders = null;
            StkhWorking working = null;
            StkhHierarchy hierarchy = null;
            var session = SessionManager.GetCurrentSession();

            var listOfStakeholders = session.QueryOver<Stakeholders>(() => stakeholders)
                                             .Fetch(x => x.StkhWorkings).Eager
                                             .Fetch(x => x.Hierarchy).Eager
                                             .JoinQueryOver(() => stakeholders.StkhWorkings, () => working,
                                                            JoinType.InnerJoin)
                                             .JoinQueryOver(() => stakeholders.Hierarchy, () => hierarchy,
                                                            JoinType.InnerJoin)
                                             .Where(() => stakeholders.LeavingDate < DateTime.Now)
                                             .And(() => stakeholders.Status == ColloSysEnums.ApproveStatus.Approved)
                                             .And(() => working.Products == products)
                                             .And(() => hierarchy.IsInAllocation)
                                             .And(() => hierarchy.IsInField)
                                             .TransformUsing(Transformers.DistinctRootEntity)
                                             .List();
            return listOfStakeholders;
        }

        [Transaction]
        public IList<Stakeholders> OnLeaveOnProduct(ScbEnums.Products products)
        {
            Stakeholders stakeholders = null;
            StkhWorking working = null;
            StkhHierarchy hierarchy = null;
            var _session = SessionManager.GetCurrentSession();

            var listOfStakeholders = _session.QueryOver<Stakeholders>(() => stakeholders)
                                             .Fetch(x => x.StkhWorkings).Eager
                                             .Fetch(x => x.Hierarchy).Eager
                                             .JoinQueryOver(() => stakeholders.StkhWorkings, () => working,
                                                            JoinType.LeftOuterJoin)
                                             .JoinQueryOver(() => stakeholders.Hierarchy, () => hierarchy,
                                                            JoinType.LeftOuterJoin)
                                             .Where(() => stakeholders.LeavingDate == null || stakeholders.LeavingDate < DateTime.Now)
                                             .And(() => working.Products == products)
                                             .And(() => working.EndDate <= DateTime.Now)
                                             .And(() => stakeholders.Status == ColloSysEnums.ApproveStatus.Approved)
                                             .And(() => hierarchy.IsInAllocation)
                                             .And(() => hierarchy.IsInField)
                                             .TransformUsing(Transformers.DistinctRootEntity)
                                             .List();
            return listOfStakeholders;
        }

        [Transaction]
        public IList<Stakeholders> AllocationBulkChange()
        {
            var session = SessionManager.GetCurrentSession();
            Stakeholders stake = null;
            StkhHierarchy hierarchy = null;
            var data = session.QueryOver<Stakeholders>(() => stake)
                                      .Fetch(x => x.Hierarchy).Eager
                                      .JoinAlias(() => stake.Hierarchy, () => hierarchy, JoinType.InnerJoin)
                                      .Where(() => hierarchy.IsInAllocation)
                                      .List();
            return data;
        }

        [Transaction]
        public Stakeholders OnIdWithAllReferences(Guid id)
        {
            return SessionManager.GetCurrentSession()
                                       .QueryOver<Stakeholders>()
                                       .Fetch(x => x.Hierarchy).Eager
                                       .Fetch(x => x.StkhRegistrations).Eager
                                       .Fetch(x => x.GAddress).Eager
                                       .Fetch(x => x.StkhPayments).Eager
                                       .Fetch(x => x.StkhWorkings).Eager
                                       .Where(x => x.Id == id)
                                       .TransformUsing(Transformers.DistinctRootEntity)
                                       .List()
                                       .FirstOrDefault();
        }

        [Transaction]
        public Stakeholders OnId(Guid id)
        {
            return SessionManager.GetCurrentSession()
                                 .Query<Stakeholders>().Fetch(x => x.Hierarchy).Single(x => x.Id == id);

        }

        [Transaction]
        public IList<Stakeholders> OnHierarchyId(Guid reporting)
        {
            return SessionManager.GetCurrentSession().Query<Stakeholders>()
                                          .Fetch(x => x.Hierarchy)
                                          .Fetch(x => x.StkhWorkings)
                                          .Where(x => x.Hierarchy.Id == reporting &&
                                                      (x.LeavingDate < DateTime.Now || x.LeavingDate == null))
                                          .ToList();
        }

        [Transaction]
        public IList<Stakeholders> OnHierarchyId(IList<Guid> hierarchyIds)
        {
            return SessionManager.GetCurrentSession().Query<Stakeholders>()
                .Fetch(x => x.StkhWorkings)
                .Fetch(x => x.Hierarchy)
                .Where(x => hierarchyIds.Contains(x.Hierarchy.Id)
                    && (x.LeavingDate == null || x.LeavingDate > DateTime.Today))
                .ToList();
        }

        [Transaction]
        public IEnumerable<Stakeholders> OnHieararchyIdWithPayments(Guid hierarchyid)
        {
            return SessionManager.GetCurrentSession().Query<Stakeholders>()
                                 .Fetch(x => x.StkhPayments)
                                 .Fetch(x => x.Hierarchy)
                                 .Where(
                                     x =>
                                     x.Hierarchy.Id == hierarchyid &&
                                     (x.LeavingDate < DateTime.Now || x.LeavingDate == null))
                                 .Select(x => x)
                                 .OrderByDescending(
                                     x =>
                                     x.StkhPayments.First(y => y.StartDate < DateTime.Now && y.EndDate > DateTime.Now))
                                 .ToList();
        }

        //[Transaction]
        //public IEnumerable<Stakeholders> GetPendingApprovals(string currUser, ColloSysEnums.ApproveStatus status)
        //{
        //    var session = SessionManager.GetCurrentSession();

        //    var pendingStake = session.QueryOver<Stakeholders>();

        //} 

        public override QueryOver<Stakeholders, Stakeholders> ApplyRelations()
        {
            var query = QueryOver.Of<Stakeholders>()
                            .Fetch(x => x.StkhPayments).Eager
                            .Fetch(x => x.StkhRegistrations).Eager
                            .Fetch(x => x.StkhWorkings).Eager
                            .Fetch(x => x.Hierarchy).Eager
                            .Fetch(x => x.GAddress).Eager
                            .TransformUsing(Transformers.DistinctRootEntity);
            return query;
        }
    }
}
