using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.Billing
{
    [TestFixture]
    public class BillingScenarioTest0303 : BillingScenarioTestBase
    {
        [Test]
        public void _01_E_Category_Cases()
        {
            RunBillingForBillStatus(201403, "002969");
        }

        [Test]
        public void _02_Reversal_003419()
        {
            RunBillingForBillStatus(201403, "003419");
        }

        [Test]
        public void _03_Reversal_004447()
        {
            RunBillingForBillStatus(201403, "004447");
        }

        [Test]
        public void _04_PF_SNEP_SAl_HL()
        {
            RunBillingForBillStatus(201403, "005305");
        }

        [Test]
        public void _05_PF_SAL_SEP_HL()
        {
            RunBillingForBillStatus(201403, "005536");
        }

        [Test]
        public void _06_Part_Disb_Capping_HL()
        {
            RunBillingForBillStatus(201403, "008012");   
        }

        [Test]
        public void _07_Bulk_Cases_HL_NHL()
        {
            RunBillingForBillStatus(201403, "008471");    
        }

        [Test]
        public void _08_Part_Disb_Of_Cancellation()
        {
            RunBillingForBillStatus(201403, "009057");
        }

        [Test]
        public void _09_Part_Disb_From_Previous_Month()
        {
            RunBillingForBillStatus(201403, "009077");
        }

        [Test]
        public void _10_Cancellation_From_Previous_Month()
        {
            RunBillingForBillStatus(201403, "011762");
        }
    }
}
