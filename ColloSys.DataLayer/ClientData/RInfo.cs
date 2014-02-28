using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

namespace ColloSys.DataLayer.ClientData
{
    public class RInfo : SharedInfo
    {
        public virtual Iesi.Collections.Generic.ISet<RAlloc> RAllocs { get; set; }
    }
}
