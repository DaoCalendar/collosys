using ColloSys.BillingService;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.BillingTest
{
    public class TestBillingService
    {
        [Test]
        public void StartBillingService()
        {
            var billService = new BillingServices();
            billService.StartBillingService();
        }
    }
}
