using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhNotificationMap : EntityMap<StkhNotification>
    {
        public StkhNotificationMap()
        {
            Property(x => x.EntityId);
            Property(x => x.Description);
            Property(x => x.NotificationType);
            Property(x => x.ParamsJson);

            ManyToOne(x => x.ForStakeholder, map => map.NotNullable(true));
            ManyToOne(x => x.DelegatedTo);
        }
    }
}
