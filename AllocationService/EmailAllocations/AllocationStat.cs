using ColloSys.DataLayer.Domain;

namespace ColloSys.AllocationService.EmailAllocations
{
    public class AllocationStat
    {
        public string PolicyName { get; set; }
        public string SubPolicyName { get; set; }
        public string StakeholderName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Product { get; set; }
        public string TotalDue { get; set; }
        public string CustomerName { get; set; }
        public string AccountNo { get; set; }
        public string Pincode { get; set; }

    }

    public class StakeholdersStat
    {
        public Stakeholders AllocatedStakeholder { get; set; }
        public Stakeholders Manager { get; set; }
    }
}