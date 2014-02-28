using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

namespace ColloSys.DataLayer.Allocation
{
    public class EAlloc : SharedAlloc
    {
        public virtual ELiner ELiner { get; set; }
        public virtual EWriteoff EWriteoff { get; set; }
        public virtual EInfo EInfo { get; set; }
        
    }
}