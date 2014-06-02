using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.Billing
{
    [TestFixture]
    public class BillingScenarioTests0201 : BillingScenarioTestBase
    {
        [TestCase("002857")]
        [TestCase("008012")]
        [TestCase("009057")]
        public void _02_Reversal_Cases(string agentId)
        {
            RunBillingForBillStatus(201402, agentId, 201401);
        }
    }
}
