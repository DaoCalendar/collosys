using System;
using ColloSys.DataLayer.Enumerations;

namespace AngularUI.FileUpload.filescheduler
{
    public class ScheduledFiles
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public ColloSysEnums.FileAliasName AliasName { get; set; }
        public ColloSysEnums.FileType FileType { get; set; }
        public bool IsScheduled { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string FileName { get; set; }
        public UInt64 FileSize { get; set; }
        public ColloSysEnums.UploadStatus FileStatus { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public string UploadPath { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }
}