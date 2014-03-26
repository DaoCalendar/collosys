using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Allocation
{
    public class AllocConditionMap : EntityMap<AllocCondition>
    {
        public AllocConditionMap()
        {
            Property(x => x.ColumnName);
            Property(x => x.Operator);
            Property(x => x.Value);
            Property(x => x.RelationType, map=>map.NotNullable(false));
            Property(x=>x.Priority);
            ManyToOne(x => x.AllocSubpolicy, map => map.NotNullable(true));
        }
    }
}
