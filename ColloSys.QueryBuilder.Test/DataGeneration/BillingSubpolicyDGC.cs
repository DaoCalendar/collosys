using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.QueryBuilder.Test.DataGeneration
{
    public class BillingSubpolicyDGC
    {
        private readonly BillTokensDGC _testingBillTokens;

        public BillingSubpolicyDGC()
        {
            _testingBillTokens = new BillTokensDGC();
        }

        public List<BillingSubpolicy> GetBillingSubpolicies()
        {
            var billingSubpolicies = new List<BillingSubpolicy>();

            var billingSubpolicy = new BillingSubpolicy
            {
                Products = ScbEnums.Products.PL,
                Name = "CycleGreterThen2",
                PayoutSubpolicyType = ColloSysEnums.PayoutSubpolicyType.Formula,
                BillTokens = _testingBillTokens.GreaterThanTokens()
            };
            billingSubpolicies.Add(billingSubpolicy);

            return billingSubpolicies;
        }
    }
}
