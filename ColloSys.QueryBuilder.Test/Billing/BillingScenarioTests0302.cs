using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.Billing
{
    [TestFixture]
    public class BillingScenarioTests0302 : BillingScenarioTestBase
    {
        [TestCase("011762")]
        [TestCase("003419")]
        [TestCase("004447")]
        public void _03_02_Reversal_Cases(string agentId)
        {
            RunBillingForBillStatus(201403, agentId, 201402);
        }

    }
}
