#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Stakeholder;

#endregion

namespace ColloSys.AllocationService.ChurnAllocations
{
    public class AllocationData
    {
        public AllocationData()
        {
            AllocList=new List<Allocations>();
        }
        public Stakeholders Stakeholders { get; set; }
        public IList<Allocations> AllocList { get; set; }  
    }
}