using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Domain
{
    public class FileStatus : Entity
    {
        public virtual FileScheduler FileScheduler { get; set; }
        public virtual UInt64 TotalRows { get; set; }
        public virtual UInt64 ValidRows { get; set; }
        public virtual UInt64 UploadedRows { get; set; }
        public virtual UInt64 DuplicateRows { get; set; }
        public virtual UInt64 ErrorRows { get; set; }
        public virtual UInt64 IgnoredRows { get; set; }
        public virtual ColloSysEnums.UploadStatus UploadStatus { get; set; }
        public virtual DateTime EntryDateTime { get; set; }
    }
}