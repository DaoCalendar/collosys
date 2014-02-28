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
            Table("ALLOC_POLICY");

            #region properties
            Property(x => x.Name);
            Property(x => x.Products, map => map.UniqueKey("UK_ALLOC_POLICY"));

            Property(x => x.Category, map => map.UniqueKey("UK_ALLOC_POLICY"));
            #endregion

            #region IApprove
            Property(x => x.Status);
            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.OrigEntityId);
            Property(x => x.RowStatus);

            #endregion

            #region relationships - bags
            Set(x => x.AllocRelations, colmap => { }, map => map.OneToMany(x => { }));
            Set(x => x.Allocs, colmap => { }, map => map.OneToMany(x => { }));
           
            //Set(x => x.CInfos, colmap => { }, map => map.OneToMany(x => { }));
            //Set(x => x.RInfos, colmap => { }, map => map.OneToMany(x => { }));
            //Set(x => x.EInfos, colmap => { }, map => map.OneToMany(x => { }));

            #endregion
        }
    }
}