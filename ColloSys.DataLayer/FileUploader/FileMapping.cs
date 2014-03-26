using System;
using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;

namespace ColloSys.DataLayer.Domain
{
    public class FileMapping : Entity
    {
        public virtual IList<FileValueMapping> FileValueMappings { get; set; }

        public virtual string ActualTable { get; set; }
        public virtual FileDetail FileDetail { get; set; }
        public virtual string ActualColumn { get; set; }
        public virtual uint Position { get; set; }
        public virtual uint OutputPosition { get; set; }
        public virtual string OutputColumnName { get; set; }
        public virtual ColloSysEnums.FileMappingValueType ValueType { get; set; }
        public virtual string TempTable { get; set; }
        public virtual string TempColumn { get; set; }
        public virtual string DefaultValue { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
    }
}