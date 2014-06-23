using System;
using System.Linq;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class StakePaymentQueryBuilder : Repository<StkhPayment>
    {
        public override QueryOver<StkhPayment, StkhPayment> ApplyRelations()
        {
            return QueryOver.Of<StkhPayment>()
                        .Fetch(x => x.Stakeholder).Eager
                        .TransformUsing(Transformers.DistinctRootEntity);
        }

        public StkhPayment GetActivePayment(Guid stakeholderId)
        {
            var session = SessionManager.GetCurrentSession();
            var payment = session.Query<StkhPayment>()
                .Fetch(x => x.Stakeholder)
                .FirstOrDefault(x => x.Stakeholder.Id == stakeholderId
                                     && (x.StartDate <= DateTime.Today
                                     && (x.EndDate == null || x.EndDate > DateTime.Today)));

            return payment;
        }

        public StkhPayment GetPaymentWithStakeholder(Guid paymentId)
        {
            var session = SessionManager.GetCurrentSession();
            var payment = session.Query<StkhPayment>()
                .Fetch(x => x.Stakeholder)
                .Single(x => x.Id == paymentId);
            return payment;
        }
    }
}