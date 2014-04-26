using System;
using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.FileUploader
{
    public class FileDetail : Entity
    {
        public virtual IList<FileColumn> FileColumns { get; set; }
        public virtual IList<FileMapping> FileMappings { get; set; }
        public virtual IList<FileScheduler> FileSchedulers { get; set; }
        public virtual IList<FilterCondition> FilterConditions { get; set; }

        public virtual ColloSysEnums.FileAliasName AliasName { get; set; }
        public virtual string AliasDescription { get; set; }
        public virtual string FileName { get; set; }
        public virtual uint FileCount { get; set; }
        public virtual ColloSysEnums.FileAliasName? DependsOnAlias { get; set; }
        public virtual ColloSysEnums.FileUploadBy FileReaderType { get; set; }
        public virtual string DateFormat { get; set; }
        public virtual ColloSysEnums.FileType FileType { get; set; }
        public virtual string SheetName { get; set; }
        public virtual ColloSysEnums.FileFrequency Frequency { get; set; }
        public virtual UInt32 SkipLine { get; set; }
        public virtual string FileServer { get; set; }
        public virtual string FileDirectory { get; set; }
        public virtual ScbEnums.ClientDataTables ActualTable { get; set; }
        public virtual string TempTable { get; set; }
        public virtual string ErrorTable { get; set; }
        public virtual string EmailId { get; set; }
        public virtual string Description { get; set; }
        public virtual ColloSysEnums.UsedFor UsedFor { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual ScbEnums.ScbSystems ScbSystems { get; set; }
        public virtual ScbEnums.Category Category { get; set; }
    }
}