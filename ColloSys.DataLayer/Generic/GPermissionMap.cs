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
            Table("G_PERMISSIONS");

            #region Property

            Property(x => x.Activity, map => map.UniqueKey("UQ_G_PERMISSIONS"));
            Property(x => x.Permission);
            Property(x => x.EscalationDays);

            #endregion

            ManyToOne(x => x.Role, map =>
                {
                    map.NotNullable(true);
                    map.Index("IX_StakeHierarchy");
                    map.UniqueKey("UQ_G_PERMISSIONS");
                });
        }
    }
}