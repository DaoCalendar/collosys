#region references

using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Generic
{
    // ReSharper disable ClassNeverInstantiated.Global
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public class GReports : Entity
    {
        public virtual string ReportName { get; set; }

        public virtual ColloSysEnums.GridScreenName ScreenName { get; set; }

        public virtual string User { get; set; }

        public virtual string Description { get; set; }

        public virtual string ReportJson { get; set; }

        public virtual bool DoEmailReport { get; set; }

        public virtual string EmailId { get; set; }

        public virtual ColloSysEnums.FileFrequency Frequency { get; set; }

        public virtual uint FrequencyParam { get; set; }

        public virtual DateTime NextSendingDateTime { get; set; }

        public virtual bool UseFieldName4Header { get; set; }

        public virtual bool SendOnlyIfData { get; set; }

        public virtual bool Send2Hierarchy { get; set; }

        public virtual string StakeholderIds { get; set; }

    }
    // ReSharper restore UnusedAutoPropertyAccessor.Global
    // ReSharper restore ClassNeverInstantiated.Global
}
