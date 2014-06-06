#region references

using ColloSys.DataLayer.BaseEntity;

#endregion

namespace ColloSys.DataLayer.Allocation
{
    public class AllocPolicyMap : EntityMap<AllocPolicy>
    {
        public AllocPolicyMap()
        {
            Property(x => x.Name);
            Property(x => x.Products);

            Bag(x => x.AllocRelations, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.Allocs, colmap => { }, map => map.OneToMany(x => { }));
        }
    }
}