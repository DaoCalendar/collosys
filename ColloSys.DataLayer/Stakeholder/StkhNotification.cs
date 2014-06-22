using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhNotification : Entity
    {
        public virtual Stakeholders ForStakeholder { get; set; }
        public virtual Stakeholders ByStakeholder { get; set; }
        public virtual string RequestBy { get; set; }
        public virtual ColloSysEnums.NotificationType NoteType { get; set; }
        public virtual Guid EntityId { get; set; }
        public virtual string Description { get; set; }
        public virtual string ParamsJson { get; set; }
        public virtual ColloSysEnums.NotificationStatus NoteStatus { get; set; }
        public virtual bool IsResponse { get; set; }
        public virtual string ResponseType { get; set; }
        public virtual string ResponseBy { get; set; }
        public virtual DateTime RequestDateTime { get; set; }
        public virtual DateTime? ResponseDateTime { get; set; }
    }
}