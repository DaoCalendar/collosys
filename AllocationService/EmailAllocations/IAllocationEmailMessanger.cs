using System.Collections.Generic;

namespace ColloSys.AllocationService.EmailAllocations
{
    public interface IAllocationEmailMessanger
    {
        IList<StakeholdersStat> GetStakeholderWithManger();
        bool InitSendingMail(IList<StakeholdersStat> listOfStakeholdersAndMangers);
    }
}