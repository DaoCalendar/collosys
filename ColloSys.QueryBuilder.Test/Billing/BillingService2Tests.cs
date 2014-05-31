using BillingService2;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.Billing
{
    [TestFixture]
    public class BillingService2Tests
    {
        [Test]
        public void StartService()
        {
            var billingService = new BillingServices();
            billingService.StartBillingService();
        }
    }
}
