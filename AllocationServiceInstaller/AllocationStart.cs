using ColloSys.AllocationService;
using NLog;

namespace ColloSys.AllocationServiceInstaller
{
    public class AllocationStart:IAllocationStart
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static bool allocationInProcess;

        static AllocationStart()
        {
            allocationInProcess = false;
        }

        public void StartAllocationProcess()
        {
            if (allocationInProcess)
            {
                Logger.Info("Waiting for another allocation to complete.");
                return;
            }

            allocationInProcess = true;
            StartAllocation.Start();
            allocationInProcess = false;
        }
    }
}
