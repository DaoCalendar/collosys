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

        public void DefaultStakeholders()
        {
            var query = DefaultQuery();
            var session = SessionManager.GetCurrentSession();
            var data = session.QueryOver<Stakeholders>()
                              .WithSubquery
                              .WhereProperty(x => x.Id)
                              .In(query)
                              .List();
        }

        public override QueryOver<Stakeholders> DefaultQuery()
        {
            return QueryOver.Of<Stakeholders>()
                            .Fetch(x => x.StkhPayments).Eager
                            .Fetch(x => x.StkhRegistrations).Eager
                            .Fetch(x => x.StkhWorkings).Eager
                            .Fetch(x => x.Hierarchy).Eager
                            .Fetch(x => x.GAddress).Eager
                            .TransformUsing(new DistinctRootEntityResultTransformer());

        }
    }

    public class StakeWorkingQueryBuilder : QueryBuilder<StkhWorking>
    {
        public override QueryOver<StkhWorking> DefaultQuery()
        {
            return QueryOver.Of<StkhWorking>()
                            .Fetch(x => x.Stakeholder).Eager
                            .Fetch(x => x.StkhPayment).Eager
                            .TransformUsing(new DistinctRootEntityResultTransformer());
        }
    }
}
