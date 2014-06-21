using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Generic
{
    public class GNotificationMap : EntityMap<GNotification>
    {
        public GNotificationMap()
        {
            Property( x=> x.NotificationType);
            Property(x => x.EsclationDays);
            Property(x => x.NotifyHierarchy);
        }
    }
}