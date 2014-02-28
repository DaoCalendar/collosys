using System.Collections.Generic;

namespace ColloSys.AllocationService.EmailAllocations
{
    public interface IAllocationEmailMessanger
    {
        IEnumerable<StakeholdersStat> GetStakeholderWithManger();
        bool InitSendingMail(IEnumerable<StakeholdersStat> listOfStakeholdersAndMangers);
    }
}