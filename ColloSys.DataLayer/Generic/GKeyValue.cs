using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Generic
{
    public class GKeyValue : Entity
    {
        public virtual ColloSysEnums.Activities Area { get; set; }
        public virtual string ParamName { get; set; }
        public virtual string Value { get; set; }
        public virtual string ValueType { get; set; }

        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
    }
}
