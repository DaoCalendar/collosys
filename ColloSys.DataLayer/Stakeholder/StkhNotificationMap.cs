using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhNotificationMap : EntityMap<StkhNotification>
    {
        public StkhNotificationMap()
        {
            Property(x => x.EntityId);
            Property(x => x.NotificationType);
            Property(x => x.Description);
            Property(x => x.ParamsJson, map => map.NotNullable(false));
            ManyToOne(x => x.ForStakeholder, map => map.NotNullable(true));
        }
    }
}
