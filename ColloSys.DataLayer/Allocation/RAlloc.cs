using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

namespace ColloSys.DataLayer.Allocation
{
    public class RAlloc : Alloc
    {
        public virtual RLiner RLiner { get; set; }
        public virtual RWriteoff RWriteoff { get; set; }
        public virtual RInfo RInfo { get; set; }
    }
}