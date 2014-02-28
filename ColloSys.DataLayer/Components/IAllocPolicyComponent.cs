#region

using System;

#endregion

namespace ColloSys.DataLayer.Components
{
    public interface IAllocPolicyComponent
    {
        bool IsAllocated { get; set; }

        int Bucket { get; set; }

        decimal AmountDue { get; set; }

        string ChangeReason { get; set; }
    }
}
