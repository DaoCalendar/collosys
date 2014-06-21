using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Generic
{
    public class GNotification : Entity
    {
        public virtual ColloSysEnums.NotificationType NotificationType { get; set; }
        public virtual uint EsclationDays { get; set; }
        public virtual ColloSysEnums.NotifyHierarchy NotifyHierarchy { get; set; }
    }
}
