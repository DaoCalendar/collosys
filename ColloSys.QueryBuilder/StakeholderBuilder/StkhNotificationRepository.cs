using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class StkhNotificationRepository : Repository<StkhNotification>
    {
        public override QueryOver<StkhNotification, StkhNotification> ApplyRelations()
        {
            return QueryOver.Of<StkhNotification>()
                .Fetch(x => x.ForStakeholder).Eager;
        }
    }
}
