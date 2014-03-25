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
            Table("ALLOC_SUBPOLICY");

            #region properties
            Property(x => x.Name, map =>
                {
                    map.NotNullable(true);
                    map.UniqueKey("UK_ALLOC_SUBPOLICY");
                });
            Property(x => x.NoAllocMonth, map => map.NotNullable(true));
            Property(x => x.Products, map =>
            {
                map.NotNullable(true);
                map.UniqueKey("UK_ALLOC_SUBPOLICY");
            });

            Property(x => x.Name, map => map.UniqueKey("UK_ALLOC_SUBPOLICY"));
            Property(x => x.AllocateType);
            Property(x => x.NoAllocMonth);
            Property(x => x.Products, map => map.UniqueKey("UK_ALLOC_SUBPOLICY"));
            Property(x => x.Category, map => map.UniqueKey("UK_ALLOC_SUBPOLICY"));
            Property(x => x.ReasonNotAllocate, map => map.NotNullable(false));
            Property(x => x.IsActive);
            Property(x => x.IsInUse);
            

            #endregion

            #region relationships - many2one
            ManyToOne(x => x.Stakeholder, map => map.NotNullable(false));
            #endregion

            #region relationships - bags
            Set(x => x.AllocRelations, colmap => colmap.Lazy(CollectionLazy.NoLazy), map => map.OneToMany(x => { }));
            Set(x => x.Allocs, colmap => colmap.Lazy(CollectionLazy.NoLazy), map => map.OneToMany(x => { }));
            Set(x => x.Conditions, colmap => colmap.Lazy(CollectionLazy.NoLazy), map => map.OneToMany(x => { }));
            //Set(x => x.CInfos, colmap => { }, map => map.OneToMany(x => { }));
            //Set(x => x.RInfos, colmap => { }, map => map.OneToMany(x => { }));
            //Set(x => x.EInfos, colmap => { }, map => map.OneToMany(x => { }));
            #endregion
        }
    }
}
