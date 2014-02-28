#region

using System;

#endregion

namespace ColloSys.DataLayer.Components
{
    public interface IPayment : IBillComponent, ITransactionComponent, IApproverComponent
    {
        DateTime FileDate { get; set; }

        string Product { get; set; }
    }
}