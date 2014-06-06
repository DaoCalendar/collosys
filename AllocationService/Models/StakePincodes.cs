#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.Stakeholder;

#endregion


namespace ColloSys.AllocationService.Models
{
    public class StakePincodes
    {
        public StakePincodes()
        {
            Pincodes=new List<GPincode>();
            Allocations=new List<Allocations>();
        }

        public Stakeholders Stakeholders { get; set; }
        public List<GPincode> Pincodes { get; set; }
        public List<Allocations> Allocations { get; set; }
    }
}
