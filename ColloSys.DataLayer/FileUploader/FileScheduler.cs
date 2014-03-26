using System;
using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;

namespace ColloSys.DataLayer.Domain
{
    public class FileScheduler : Entity
    {
        public virtual IList<FileStatus> FileStatuss { get; set; }
        public virtual IList<CLiner> CLiners { get; set; }
        public virtual IList<CUnbilled> CUnbilleds { get; set; }
        public virtual IList<Payment> Payments { get; set; }
        public virtual IList<CWriteoff> CWriteoffs { get; set; }
        public virtual IList<CacsActivity> CacsActivities { get; set; }
        public virtual IList<ELiner> ELiners { get; set; }
        public virtual IList<EWriteoff> EWriteoffs { get; set; }
        public virtual IList<RLiner> RLiners { get; set; }
        public virtual IList<RWriteoff> RWriteoffs { get; set; }

        public virtual FileDetail FileDetail { get; set; }
        public virtual string FileServer { get; set; }
        public virtual string FileDirectory { get; set; }
        public virtual string FileName { get; set; }
        public virtual ulong FileSize { get; set; }
        public virtual bool IsImmediate { get; set; }
        public virtual ColloSysEnums.UploadStatus UploadStatus { get; set; }
        public virtual string StatusDescription { get; set; }
        public virtual DateTime FileDate { get; set; }
        public virtual ulong TotalRows { get; set; }
        public virtual ulong ValidRows { get; set; }
        public virtual ulong ErrorRows { get; set; }
        public virtual string ImmediateReason { get; set; }
        public virtual DateTime StartDateTime { get; set; }
        public virtual DateTime? EndDateTime { get; set; }
        public virtual ScbEnums.ScbSystems ScbSystems { get; set; }
        public virtual ScbEnums.Category Category { get; set; }
        public virtual bool AllocBillDone { get; set; }
    }
}