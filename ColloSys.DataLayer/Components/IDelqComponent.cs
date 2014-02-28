using System;

namespace ColloSys.DataLayer.Components
{
    public interface IDelqComponent
    {
        string DelqStatus { get; set; }

        DateTime DelqDate { get; set; }

        decimal DelqAmount { get; set; }
    }
}