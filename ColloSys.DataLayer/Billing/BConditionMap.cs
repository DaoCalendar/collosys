#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class BConditionMap : EntityMap<BCondition>
    {
        public BConditionMap()
        {
            Table("B_CONDITIONS");

            #region property
            Property(x => x.Ltype, map => map.NotNullable(false));
            Property(x => x.LtypeName, map => map.NotNullable(false));
            Property(x => x.Lsqlfunction, map => map.NotNullable(false));
            Property(x => x.Operator);
            Property(x => x.Rtype);
            Property(x => x.RtypeName, map => map.NotNullable(false));
            Property(x => x.Rvalue, map =>
                {
                    map.Length(500);
                    map.NotNullable(false);
                });
            Property(x => x.RelationType, map => map.NotNullable(false));
            Property(x => x.Priority);
            Property(x => x.ConditionType);

            Property(x=>x.Formula,map=>map.NotNullable(false));
            #endregion

            #region ManyToOne
            ManyToOne(x => x.BillingSubpolicy, map => map.NotNullable(false));
            #endregion
        }
    }
}