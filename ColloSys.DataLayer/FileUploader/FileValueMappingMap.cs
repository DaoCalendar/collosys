using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.FileUploader
{
    public class FileValueMappingMap : EntityMap<FileValueMapping>
    {
        public FileValueMappingMap()
        {
            Table("FILE_VALUE_MAPPING");

            #region properties

            Property(x => x.DestinationValue);
            Property(x => x.SourceValue);
            Property(x => x.Priority);

            #endregion

            #region Relationship
            ManyToOne(x => x.FileMapping,
                map =>
                {
                    map.ForeignKey("FK_FILE_VALUE_MAPPING");
                    map.NotNullable(true);
                });
            #endregion
        }
    }
}
