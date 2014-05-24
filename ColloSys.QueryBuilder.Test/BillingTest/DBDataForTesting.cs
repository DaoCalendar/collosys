using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.QueryBuilder.Test.BillingTest
{
    public class DBDataForTesting
    {
        private TestingBillTokens testingBillTokens;

        public DBDataForTesting()
        {
            testingBillTokens = new TestingBillTokens();
        }

        public List<BillingSubpolicy> GetBillingSubpolicies()
        {
            var billingSubpolicies = new List<BillingSubpolicy>();

            var billingSubpolicy = new BillingSubpolicy()
            {
                Products = ScbEnums.Products.PL,
                Name = "CycleGreterThen2",
                PayoutSubpolicyType = ColloSysEnums.PayoutSubpolicyType.Formula,
                BillTokens = testingBillTokens.GreaterThanTokens()
            };


            return billingSubpolicies;
        }
    }
}
