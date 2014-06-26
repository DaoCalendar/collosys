using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Generic
{
    public class GNotifyConfigMap : EntityMap<GNotifyConfig>
    {
        public GNotifyConfigMap()
        {
            Property( x=> x.NotificationType);
            Property(x => x.EsclationDays);
            Property(x => x.NotifyHierarchy);
        }
    }
}