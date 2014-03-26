using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

namespace ColloSys.DataLayer.Mapping
{
    public class FileColumnMap : EntityMap<FileColumn>
    {
        public FileColumnMap()
        {
            Property(x => x.Position, map => map.Index("IX_FILE_COLUMN"));
            Property(x => x.FileColumnName);
            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.Length);
            Property(x => x.ColumnDataType);
            Property(x => x.TempColumnName);
            Property(x => x.DateFormat, map => map.NotNullable(false));
            Property(x => x.StartDate);
            Property(x => x.EndDate);

            ManyToOne(x => x.FileDetail, map => { map.NotNullable(true); map.Index("IX_FILE_COLUMN"); });
        }
    }
}