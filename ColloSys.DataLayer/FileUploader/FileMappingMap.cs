#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class FileMappingMap : EntityMap<FileMapping>
    {
        public FileMappingMap()
        {
            Table("FILE_MAPPINGS");

            #region properties

            Property(x => x.ActualTable, map =>
                {
                    map.NotNullable(false);
                    map.UniqueKey("UK_FILE_MAPPING");
                });
            Property(x => x.ActualColumn, map =>
                {
                    map.NotNullable(false);
                    map.NotNullable(false); map.UniqueKey("UK_FILE_MAPPING");
                });
            Property(x => x.Position);
            Property(x => x.OutputPosition);
            Property(x => x.OutputColumnName);
            Property(x => x.TempTable, map => map.UniqueKey("UK_FILE_MAPPING"));
            Property(x => x.TempColumn, map => map.NotNullable(false));
            Property(x => x.ValueType);
            Property(x => x.DefaultValue, map => map.NotNullable(false));

            #endregion

            #region IDateRange

            Property(x => x.StartDate);
            Property(x => x.EndDate);

            #endregion

            ManyToOne(x => x.FileDetail, map => { map.NotNullable(true); map.Index("IX_FILE_MAPPING"); });

            Set(x => x.FileValueMappings, colmap => { }, map => map.OneToMany());

        }
    }
}