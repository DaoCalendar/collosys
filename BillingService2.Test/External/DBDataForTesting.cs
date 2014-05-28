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
        private readonly TestingBillTokens _testingBillTokens;

        public DBDataForTesting()
        {
            _testingBillTokens = new TestingBillTokens();
        }

        public List<BillingSubpolicy> GetFormulaList()
        {
            var formulas = new List<BillingSubpolicy>();

            var formula1 = new BillingSubpolicy()
            {
                Products = ScbEnums.Products.PL,
                Name = "CycleGreterThen2",
                PayoutSubpolicyType = ColloSysEnums.PayoutSubpolicyType.Formula,
                OutputType = ColloSysEnums.OutputType.Boolean,
                BillTokens = _testingBillTokens.GreaterThanTokens()
            };

            var formula2 = new BillingSubpolicy()
            {
                Products = ScbEnums.Products.PL,
                Name = "CyclePlus2",
                PayoutSubpolicyType = ColloSysEnums.PayoutSubpolicyType.Subpolicy,
                OutputType = ColloSysEnums.OutputType.Number,
                BillTokens = _testingBillTokens.SumOfTwoTokens()
            };

            var formula3 = new BillingSubpolicy()
            {
                Products = ScbEnums.Products.PL,
                Name = "2PluseCycle",
                PayoutSubpolicyType = ColloSysEnums.PayoutSubpolicyType.Subpolicy,
                OutputType = ColloSysEnums.OutputType.Number,
                BillTokens = _testingBillTokens.SumOfTwoTokensReverse()
            };

            formulas.Add(formula1);
            formulas.Add(formula2);
            formulas.Add(formula3);

            return formulas;
        }

        public List<BillingSubpolicy> GetBillingSubpolicies()
        {
            var billingSubpolicies = new List<BillingSubpolicy>();

            var subpolicy1 = new BillingSubpolicy()
            {
                Products = ScbEnums.Products.PL,
                Name = "CycleEqualTo2AndCyclePlus2",
                PayoutSubpolicyType = ColloSysEnums.PayoutSubpolicyType.Subpolicy,
                BillTokens = _testingBillTokens.EqualToWithPlas2SubpolicTokens()
            };

            var subpolicy2 = new BillingSubpolicy()
            {
                Products = ScbEnums.Products.PL,
                Name = "CycleEqualTo2AndCyclePlus2",
                PayoutSubpolicyType = ColloSysEnums.PayoutSubpolicyType.Subpolicy,
                BillTokens = _testingBillTokens.EqualToWithPlas2SubpolicTokens()
            };

            billingSubpolicies.Add(subpolicy1);
            billingSubpolicies.Add(subpolicy2);

            return billingSubpolicies;
        }
    }
}
