using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Enumerations;

namespace AngularUI.FileUpload.filescheduler
{
    public class FileUploadViewModel
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public ScbEnums.ScbSystems SelectedSystem { get; set; }
        public ScbEnums.Category SelectedCategory { get; set; }
        public DateTime ScheduleDate { get; set; }
        public bool IsImmediate { get; set; }
        public string ImmediateReason { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global

        public IList<ScheduledFiles> ScheduleInfo { get; set; }
    }
}