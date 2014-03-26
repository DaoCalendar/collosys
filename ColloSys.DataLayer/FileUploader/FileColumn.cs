using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;

namespace ColloSys.DataLayer.Domain
{
    public class FileColumn : Entity
    {
        public virtual FileDetail FileDetail { get; set; }

        public virtual uint Position { get; set; }
        public virtual string FileColumnName { get; set; }
        public virtual string Description { get; set; }
        public virtual uint Length { get; set; }
        public virtual ColloSysEnums.FileDataType ColumnDataType { get; set; }
        public virtual string TempColumnName { get; set; }
        public virtual string DateFormat { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
    }
}