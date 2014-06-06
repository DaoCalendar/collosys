using ColloSys.DataLayer.BaseEntity;
using NHibernate.Mapping.ByCode;

namespace ColloSys.DataLayer.ClientData
{
    public class FilterConditionMap : EntityMap<FilterCondition>
    {
        public FilterConditionMap()
        {
            ManyToOne(x => x.FileDetail, map => map.NotNullable(true));
            Property(x => x.AliasConditionName, map => map.NotNullable(true));
            Bag(x => x.BillTokens, colmap => { }, map => map.OneToMany(x => { }));

        }
    }
}
