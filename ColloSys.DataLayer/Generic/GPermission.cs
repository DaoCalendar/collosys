using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;

namespace ColloSys.DataLayer.Generic
{
    public class GPermission : Entity
    {
        public virtual ColloSysEnums.Activities Activity { get; set; }

        public virtual bool HasAccess { get; set; }

        public virtual uint EscalationDays { get; set; }

        public virtual StkhHierarchy Role { get; set; }

        public virtual GPermission Parent { get; set; }

        public virtual IList<GPermission> Childrens { get; set; }

        public virtual string Description { get; set; }
    }
}