#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.ClientData
{
    public abstract class UploadableEntity : Entity, IFileUploadable
    {
        public abstract FileScheduler FileScheduler { get; set; }
        public abstract DateTime FileDate { get; set; }
        public abstract ulong FileRowNo { get; set; }
        public abstract IList<string> GetExcludeInExcelProperties();
    }
}