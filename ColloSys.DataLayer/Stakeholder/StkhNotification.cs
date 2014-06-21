using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhNotification : Entity
    {
        public virtual Stakeholders ForStakeholder { get; set; }
        public virtual ColloSysEnums.NotificationType NotificationType { get; set; }
        public virtual Guid EntityId { get; set; }
        public virtual String Description { get; set; }
        public virtual String ParamsJson { get; set; }
    }
}