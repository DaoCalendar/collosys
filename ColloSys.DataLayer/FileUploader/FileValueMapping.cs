using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

namespace ColloSys.DataLayer.FileUploader
{
    public class FileValueMapping : Entity
    {
        public virtual string SourceValue { get; set; }
        public virtual string DestinationValue { get; set; }
        public virtual uint Priority { get; set; }
        public virtual FileMapping FileMapping { get; set; }
    }
}
