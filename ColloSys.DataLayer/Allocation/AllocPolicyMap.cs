#region references

using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class AllocPolicyMap : EntityMap<AllocPolicy>
    {
        public AllocPolicyMap()
        {
            Property(x => x.Name);
            Property(x => x.Products, map => map.UniqueKey("UK_ALLOC_POLICY"));
            Property(x => x.Category, map => map.UniqueKey("UK_ALLOC_POLICY"));

            Property(x => x.Status);
            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.OrigEntityId);
            Property(x => x.RowStatus);

            Bag(x => x.AllocRelations, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.Allocs, colmap => { }, map => map.OneToMany(x => { }));
        }
    }
}