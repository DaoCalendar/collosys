#region References

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class GPermission : Entity
    {
        #region Properties

        public virtual ColloSysEnums.Activities Activity { get; set; }

        public virtual bool HasAccess { get; set; }

        public virtual uint EscalationDays { get; set; }

        public virtual StkhHierarchy Role { get; set; }

        public virtual GPermission Permission { get; set; }
        public virtual IList<GPermission> Childrens { get; set; }

        public virtual string Description { get; set; }

        #endregion
    }
}