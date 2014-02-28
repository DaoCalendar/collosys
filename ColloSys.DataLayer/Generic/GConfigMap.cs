#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class GConfigMap : EntityMap<GConfig>
    {
        public GConfigMap()
        {
            Table("G_CONFIG");

            #region Properties

            Property(x => x.ParamName);
            Property(x => x.Value);
            
            #endregion
            
            #region Product
            Property(x => x.ParamSubcategory);
            Property(x => x.ParamCategory);
            #endregion

            #region Approver Component
            Property(p => p.Status);
            Property(p => p.Description, map => map.NotNullable(false));
            Property(p => p.ApprovedBy, map=> map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.OrigEntityId);
            Property(x => x.RowStatus);

            #endregion
        }
    }
}
