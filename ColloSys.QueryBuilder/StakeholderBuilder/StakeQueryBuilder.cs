#region references

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.Generic;
using ColloSys.QueryBuilder.TransAttributes;
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
                              .And(() => stakeholders.ApprovalStatus == ColloSysEnums.ApproveStatus.Approved)
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
        public Stakeholders GetStakeByExtId(string id)
        {
            var session = SessionManager.GetCurrentSession();

            var stakeholder = session.QueryOver<Stakeholders>()
                .Where(x => x.ExternalId == id)
                .Fetch(x => x.StkhWorkings).Eager
                .Fetch(x => x.StkhPayments).Eager
                .Fetch(x => x.Hierarchy).Eager
                .SingleOrDefault();
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
            var data = list.Where(x => x.Name.ToString(CultureInfo.InvariantCulture).StartsWith(name)).Take(10).ToList();
            return data;
        }

        [Transaction]
        public IList<Stakeholders> ExitedOnProduct(ScbEnums.Products products)
        {
            Stakeholders stakeholders = null;
            StkhWorking working = null;
            StkhHierarchy hierarchy = null;
            var session = SessionManager.GetCurrentSession();

            var listOfStakeholders = session.QueryOver(() => stakeholders)
                                             .Fetch(x => x.StkhWorkings).Eager
                                             .Fetch(x => x.Hierarchy).Eager
                                             .JoinQueryOver(() => stakeholders.StkhWorkings, () => working,
                                                            JoinType.InnerJoin)
                                             .JoinQueryOver(() => stakeholders.Hierarchy, () => hierarchy,
                                                            JoinType.InnerJoin)
                                             .Where(() => stakeholders.LeavingDate < DateTime.Now)
                                             .And(() => stakeholders.ApprovalStatus == ColloSysEnums.ApproveStatus.Approved)
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
            var session = SessionManager.GetCurrentSession();

            var listOfStakeholders = session.QueryOver(() => stakeholders)
                                             .Fetch(x => x.StkhWorkings).Eager
                                             .Fetch(x => x.Hierarchy).Eager
                                             .JoinQueryOver(() => stakeholders.StkhWorkings, () => working,
                                                            JoinType.LeftOuterJoin)
                                             .JoinQueryOver(() => stakeholders.Hierarchy, () => hierarchy,
                                                            JoinType.LeftOuterJoin)
                                             .Where(() => stakeholders.LeavingDate == null || stakeholders.LeavingDate < DateTime.Now)
                                             .And(() => working.Products == products)
                                             .And(() => working.EndDate <= DateTime.Now)
                                             .And(() => stakeholders.ApprovalStatus == ColloSysEnums.ApproveStatus.Approved)
                                             .And(() => hierarchy.IsInAllocation)
                                             .And(() => hierarchy.IsInField)
                                             .TransformUsing(Transformers.DistinctRootEntity)
                                             .List();
            return listOfStakeholders;
        }

        [Transaction]
        public Stakeholders OnIdWithAllReferences(Guid id)
        {
            return SessionManager.GetCurrentSession()
                                       .QueryOver<Stakeholders>()
                                       .Fetch(x => x.Hierarchy).Eager
                                       .Fetch(x => x.StkhRegistrations).Eager
                                       .Fetch(x => x.StkhAddress).Eager
                                       .Fetch(x => x.StkhPayments).Eager
                                       .Fetch(x => x.StkhWorkings).Eager
                                       .Where(x => x.Id == id)
                                       .SingleOrDefault();
        }

        [Transaction]
        public Stakeholders OnId(Guid id)
        {
            return SessionManager.GetCurrentSession()
                                 .Query<Stakeholders>()
                                 .Fetch(x => x.Hierarchy)
                                 .Fetch(x => x.StkhAddress)
                                 .Fetch(x => x.StkhRegistrations)
                                 .Single(x => x.Id == id);

        }

        [Transaction]
        public IList<Stakeholders> GetByIdList(List<Guid> stakeholderIds)
        {
            return SessionManager.GetCurrentSession()
                                 .Query<Stakeholders>()
                                 .Where(x => stakeholderIds.Contains(x.Id))
                                 .ToList();
        }

        [Transaction]
        public Stakeholders GetById(Guid id)
        {
            var session = SessionManager.GetCurrentSession();
            var stkh = session.Query<Stakeholders>()
                                 .Fetch(x => x.Hierarchy)
                                 .Single(x => x.Id == id);
            return stkh;
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

        public override QueryOver<Stakeholders, Stakeholders> ApplyRelations()
        {
            var query = QueryOver.Of<Stakeholders>()
                            .Fetch(x => x.StkhPayments).Eager
                            .Fetch(x => x.StkhRegistrations).Eager
                            .Fetch(x => x.StkhWorkings).Eager
                            .Fetch(x => x.Hierarchy).Eager
                            .Fetch(x => x.StkhAddress).Eager
                            .TransformUsing(Transformers.DistinctRootEntity);
            return query;
        }

        [Transaction]
        public IList<Stakeholders> GetReportingList(Guid managerId)
        {
            if (managerId == Guid.Empty) return null;
            var session = SessionManager.GetCurrentSession();
            return session.Query<Stakeholders>()
                .Where(x => x.ReportingManager == managerId
                    && (x.LeavingDate == null || x.LeavingDate > DateTime.Today))
                .ToList();
        }

        [Transaction]
        public uint GetReportingCount(Guid managerId)
        {
            if (managerId == Guid.Empty) return 0;
            var session = SessionManager.GetCurrentSession();
            var count = session.Query<Stakeholders>()
                .Count(x => x.ReportingManager == managerId
                    && (x.LeavingDate == null || x.LeavingDate > DateTime.Today));
            return (uint)count;
        }
    }
}
