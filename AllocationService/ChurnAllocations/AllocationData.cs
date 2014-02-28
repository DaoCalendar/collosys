using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.SharedDomain;

namespace ColloSys.AllocationService.ChurnAllocations
{
    public class AllocationData
    {
        public AllocationData()
        {
            AllocList=new List<SharedAlloc>();
        }
        public Stakeholders Stakeholders { get; set; }
        public IList<SharedAlloc> AllocList { get; set; }  
    }
}