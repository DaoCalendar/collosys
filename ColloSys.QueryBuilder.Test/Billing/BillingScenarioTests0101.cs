using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.Billing
{
    [TestFixture]
    public class BillingScenarioTests0101 : BillingScenarioTestBase
    {
        [Test]
        public void _01_HL()
        {
            RunBillingForBillStatus(201401, "010366");
        }

        [Test]
        public void _02_HL_NHL()
        {
            RunBillingForBillStatus(201401, "009275");
        }

        [Test]
        public void _03_Part_Disbursement()
        {
            RunBillingForBillStatus(201401, "009077");
        }

        [Test]
        public void _04_NHL()
        {
            RunBillingForBillStatus(201401, "008325");
        }

        [Test]
        public void _05_Capping_009057()
        {
            RunBillingForBillStatus(201401, "009057");
        }

        [Test]
        public void _06_Corporate()
        {
            RunBillingForBillStatus(201401, "008012");
        }

        [Test]
        public void _07_HL()
        {
            RunBillingForBillStatus(201401, "002857");
        }
    }
}
