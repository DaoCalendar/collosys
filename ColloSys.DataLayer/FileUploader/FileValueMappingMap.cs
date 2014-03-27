using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.FileUploader
{
    public class FileValueMappingMap : EntityMap<FileValueMapping>
    {
        public FileValueMappingMap()
        {
            Property(x => x.DestinationValue);
            Property(x => x.SourceValue);
            Property(x => x.Priority);
            ManyToOne(x => x.FileMapping, map => { map.ForeignKey("FK_FILE_VALUE_MAPPING"); map.NotNullable(true); });
        }
    }
}
