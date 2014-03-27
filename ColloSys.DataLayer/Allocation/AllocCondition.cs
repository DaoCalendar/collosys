using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Allocation
{
    public class AllocCondition : Entity
    {
        public virtual AllocSubpolicy AllocSubpolicy { get; set; }
        public virtual uint Priority { get; set; }
        public virtual string ColumnName { get; set; }
        public virtual ColloSysEnums.Operators Operator { get; set; }
        public virtual string Value { get; set; }
        public virtual string RelationType { get; set; }
    }
}
