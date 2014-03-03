using ColloSys.BillingService;
using NLog;

namespace ColloSys.BillingServiceInstaller
{
    public class BillingStart : IBillingStart
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static bool billingInProcess;

        static BillingStart()
        {
            billingInProcess = false;
        }

        public void StartBillingProcess()
        {
            if (billingInProcess)
            {
                Logger.Info("Waiting for another allocation to complete.");
                return;
            }

            billingInProcess = true;
            var billingService = new BillingServices();
            billingService.StartBillingService();
            billingInProcess = false;
        }
    }
}
