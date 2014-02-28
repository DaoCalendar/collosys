#region References

using ColloSys.DataLayer.BaseEntity;

#endregion

namespace ColloSys.DataLayer.Allocation
{
    public class AllocConditionMap : EntityMap<AllocCondition>
    {
        public AllocConditionMap()
        {
            Table("ALLOC_CONDITIONS");

            #region property

            Property(x => x.ColumnName);
            Property(x => x.Operator);
            Property(x => x.Value);
            Property(x => x.RelationType, map=>map.NotNullable(false));
            Property(x=>x.Priority);

            #endregion

            #region ManyToOne
            ManyToOne(x => x.AllocSubpolicy, map => map.NotNullable(true));
            #endregion
        }
    }
}
