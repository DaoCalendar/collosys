#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class GPermissionMap : EntityMap<GPermission>
    {
        public GPermissionMap()
        {
            Property(x => x.Activity);
            Property(x => x.HasAccess);
            Property(x => x.EscalationDays);
            Property( x=> x.Description);

            ManyToOne(x => x.Role, map =>{map.NotNullable(true);map.Index("IX_StakeHierarchy");});
            Bag( x=> x.Childrens, map => map.Key( x=> x.Column("ParentId")), colmap => colmap.OneToMany());
            ManyToOne(x => x.Parent, map => map.Column("ParentId"));
        }
    }
}