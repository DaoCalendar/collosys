using System;

namespace ColloSys.DataLayer.Components
{
    public interface IUniqueKey
    {
        string AccountNo { get; set; }
        DateTime FileDate { get; set; }
    }
}
