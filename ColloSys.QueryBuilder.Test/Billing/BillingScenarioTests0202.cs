using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.Billing
{
    [TestFixture]
    public class BillingScenarioTests0202 : BillingScenarioTestBase
    {
        [Test]
        public void _01_Reversal_Jan_002857()
        {
            RunBillingForBillStatus(201402, "002857");
        }

        [Test]
        public void _02_D_Category()
        {
            RunBillingForBillStatus(201402, "003419");
        }

        [Test]
        public void _03_PRM_NPRM_PF()
        {
            RunBillingForBillStatus(201402, "003939");
        }

        [Test]
        public void _04_Subvention()
        {
            RunBillingForBillStatus(201402, "004447");
        }

        [Test]
        public void _05_Reversal_Jan_008012()
        {
            RunBillingForBillStatus(201402, "008012");
        }

        [Test]
        public void _06_HL_NHL()
        {
            RunBillingForBillStatus(201402, "008471");
        }

        [Test]
        public void _07_Zero_Disbursement()
        {
            RunBillingForBillStatus(201402, "008844");
        }

        [Test]
        public void _08_Reversal_Partial_009057()
        {
            RunBillingForBillStatus(201402, "009057");
        }

        [Test]
        public void _09_Disbursement_JFM()
        {
            RunBillingForBillStatus(201402, "009077");
        }

        [Test]
        public void _10_Processing_Fees()
        {
            RunBillingForBillStatus(201402, "010366");
        }

        [Test]
        public void _11_NHL()
        {
            RunBillingForBillStatus(201402, "011762");
        }

        [Test]
        public void _12_Matrix_Boundry()
        {
            RunBillingForBillStatus(201402, "011952");
        }
    }
}
