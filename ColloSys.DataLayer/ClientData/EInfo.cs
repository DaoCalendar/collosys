using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

namespace ColloSys.DataLayer.ClientData
{
    public class EInfo : Info
    {
        public virtual Iesi.Collections.Generic.ISet<EAlloc> EAllocs { get; set; }
    }
}
