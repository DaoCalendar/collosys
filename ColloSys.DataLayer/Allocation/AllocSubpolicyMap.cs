#region references

using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using NHibernate.Mapping.ByCode;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class AllocSubpolicyMap : EntityMap<AllocSubpolicy>
    {
        public AllocSubpolicyMap()
        {
            Property(x => x.Name, map => { map.NotNullable(true); map.UniqueKey("UK_ALLOC_SUBPOLICY"); });
            Property(x => x.Products, map => { map.NotNullable(true); map.UniqueKey("UK_ALLOC_SUBPOLICY"); });

            Property(x => x.AllocateType);
            Property(x => x.Category, map => map.UniqueKey("UK_ALLOC_SUBPOLICY"));
            Property(x => x.ReasonNotAllocate, map => map.NotNullable(false));
            Property(x => x.NoAllocMonth, map => map.NotNullable(true));
            Property(x => x.IsActive);
            Property(x => x.IsInUse);

            ManyToOne(x => x.Stakeholder, map => map.NotNullable(false));
            Bag(x => x.AllocRelations, colmap => {}, map => map.OneToMany(x => { }));
            Bag(x => x.Allocs, colmap => {}, map => map.OneToMany(x => { }));
            Bag(x => x.Conditions, colmap => {}, map => map.OneToMany(x => { }));
        }
    }
}
