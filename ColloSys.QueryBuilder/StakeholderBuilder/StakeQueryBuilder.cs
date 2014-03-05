using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.Generic;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class StakeQueryBuilder : QueryBuilder<Stakeholders>
    {
        [Transaction]
        public IList<Stakeholders> GetStakeholdersOnProduct(ScbEnums.Products products)
        {
            Stakeholders stakeholders = null;
            StkhWorking workings = null;
            StkhHierarchy hierarchy = null;
            StkhPayment payment = null;
            var session = SessionManager.GetCurrentSession();
            var data = session.QueryOver(() => stakeholders)
                              .Fetch(x => x.StkhWorkings).Eager
                              .Fetch(x => x.StkhPayments).Eager
                              .Fetch(x=>x.Hierarchy).Eager
                              .JoinAlias(() => stakeholders.StkhPayments, () => payment, JoinType.LeftOuterJoin)
                              .JoinAlias(() => stakeholders.StkhWorkings, () => workings, JoinType.LeftOuterJoin)
                              .JoinAlias(() => stakeholders.Hierarchy, () => hierarchy,
                                         JoinType.LeftOuterJoin)
                              .Where(() => workings.Products == products)
                              .And(() => hierarchy.IsInAllocation)
                              .And(() => hierarchy.IsInField)
                              .And(() => stakeholders.JoiningDate < Util.GetTodayDate())
                              .And(() => stakeholders.LeavingDate == null ||
                                         stakeholders.LeavingDate > Util.GetTodayDate())
                              .And(() => stakeholders.Status == ColloSysEnums.ApproveStatus.Approved)
                              .TransformUsing(Transformers.DistinctRootEntity)
                              .List();
            return data;

        }

        public IList<Stakeholders> GetStakeholdersExit(ScbEnums.Products products)
        {
            Stakeholders stakeholders = null;
            StkhWorking working = null;
            StkhHierarchy hierarchy = null;
            var _session = SessionManager.GetCurrentSession();

            var listOfStakeholders = _session.QueryOver<Stakeholders>(() => stakeholders)
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

        public IList<Stakeholders> GetStakeholdersOnLeave(ScbEnums.Products products)
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

        public override QueryOver<Stakeholders> DefaultQuery()
        {
            return QueryOver.Of<Stakeholders>()
                            .Fetch(x => x.StkhPayments).Eager
                            .Fetch(x => x.StkhRegistrations).Eager
                            .Fetch(x => x.StkhWorkings).Eager
                            .Fetch(x => x.Hierarchy).Eager
                            .Fetch(x => x.GAddress).Eager
                            .TransformUsing(Transformers.DistinctRootEntity);

        }
    }

    public class StakeWorkingQueryBuilder : QueryBuilder<StkhWorking>
    {
        public override QueryOver<StkhWorking> DefaultQuery()
        {
            return QueryOver.Of<StkhWorking>()
                            .Fetch(x => x.Stakeholder).Eager
                            .Fetch(x => x.StkhPayment).Eager
                            .TransformUsing(Transformers.DistinctRootEntity);
        }
    }
}
