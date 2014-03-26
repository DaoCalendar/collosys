using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

namespace ColloSys.DataLayer.Mapping
{
    public class FileMappingMap : EntityMap<FileMapping>
    {
        public FileMappingMap()
        {
            Property(x => x.ActualTable, map => { map.NotNullable(false); map.UniqueKey("UK_FILE_MAPPING"); });
            Property(x => x.ActualColumn, map => { map.NotNullable(false); map.UniqueKey("UK_FILE_MAPPING"); });
            Property(x => x.Position);
            Property(x => x.OutputPosition);
            Property(x => x.OutputColumnName);
            Property(x => x.TempTable, map => map.UniqueKey("UK_FILE_MAPPING"));
            Property(x => x.TempColumn, map => map.NotNullable(false));
            Property(x => x.ValueType);
            Property(x => x.DefaultValue, map => map.NotNullable(false));
            Property(x => x.StartDate);
            Property(x => x.EndDate);

            ManyToOne(x => x.FileDetail, map => { map.NotNullable(true); map.Index("IX_FILE_MAPPING"); });
            Bag(x => x.FileValueMappings, colmap => { }, map => map.OneToMany());
        }
    }
}