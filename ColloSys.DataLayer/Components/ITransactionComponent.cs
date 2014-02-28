using System;

namespace ColloSys.DataLayer.Components
{
    public interface ITransactionComponent
    {
        int TransCode { get; set; }

        DateTime TransDate { get; set; }

        string TransDesc { get; set; }

        decimal TransAmount { get; set; }

        bool IsDebit { get; set; }
    }
}