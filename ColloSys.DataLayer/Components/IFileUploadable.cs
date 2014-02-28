#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;

#endregion


namespace ColloSys.DataLayer.Components
{
    public interface IFileUploadable
    {
        FileScheduler FileScheduler { get; set; }
        DateTime FileDate { get; set; }
        ulong FileRowNo { get; set; }
        IList<string> GetExcludeInExcelProperties();
    }
}