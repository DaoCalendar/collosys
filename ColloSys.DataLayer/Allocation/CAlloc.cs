using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

namespace ColloSys.DataLayer.Allocation
{
    public class CAlloc : Alloc
    {
        public virtual CLiner CLiner { get; set; }
        public virtual CWriteoff CWriteoff { get; set; }
        public virtual CInfo CInfo { get; set; }
    }
}