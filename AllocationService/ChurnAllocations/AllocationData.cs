#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.SharedDomain;

#endregion

namespace ColloSys.AllocationService.ChurnAllocations
{
    public class AllocationData
    {
        public AllocationData()
        {
            AllocList=new List<Alloc>();
        }
        public Stakeholders Stakeholders { get; set; }
        public IList<Alloc> AllocList { get; set; }  
    }
}