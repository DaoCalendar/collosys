using BillingService2;
using NLog;

namespace ColloSys.BillingServiceInstaller
{
    public class BillingStart : IBillingStart
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static bool _billingInProcess;

        static BillingStart()
        {
            _billingInProcess = false;
        }

        public void StartBillingProcess()
        {
            if (_billingInProcess)
            {
                Logger.Info("Waiting for another allocation to complete.");
                return;
            }

            _billingInProcess = true;
            var billingService = new BillingServices();
            billingService.StartBillingService();
            _billingInProcess = false;
        }
    }
}
