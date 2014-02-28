using System;

namespace ColloSys.DataLayer.Components
{
    public interface IBillComponent
    {
        string BillStatus { get; set; }

        DateTime BillDate { get; set; }

        bool IsExcluded { get; set; }

        string ExcludeReason { get; set; }
    }
}