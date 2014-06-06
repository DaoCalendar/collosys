using NLog;

namespace ColloSys.AllocationService.EmailAllocations
{
    public static class EmailProductsAllocations
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static void Init()
        {
            Log.Info("Email Allocations Process Started");

            IAllocationEmailMessanger emailMessanger = new AllocationEmailMessanger();

            var stakeholdersWithManagerId = emailMessanger.GetStakeholderWithManger();

            var result = emailMessanger.InitSendingMail(stakeholdersWithManagerId);
            Log.Info("Email Allocations Process ended");
        }
    }
}
