using System.Collections.Generic;
using System.Linq;
using BillingService2.QueryExecution;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.Test.DataGeneration;
using ColloSys.QueryBuilder.Test.QueryExecution;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.QueryExecutionTests
{
    //TODO: MAHENDRA
    //1. formula(output) > number and number > column
    //2. number > formula and column > number
    //3. formula(conditional) and number > formula(output)
    //a. and similar all combinations
    //b. also one more thing: formula can contain other fomulas but only condtion/output formula
    //c. also while string generation formula should be replaced by its generated string
    //d. so in case 1 above output shoudl be e.g. (number + column) > number and ....

    //also name the test as per the logic and not as per the field
    //do i need to tell you, how to name variables/methods???????

    [TestFixture]
    public class AndOrQueryTests
    {
        #region ctor

        private List<CustBillViewModel> _dataList;
        private readonly BillTokensDGC _testingBillTokens = new BillTokensDGC();

        [SetUp]
        public void SetUp()
        {
            _dataList = _testingBillTokens.GenerateData();
        }

        [TearDown]
        public void TearDown()
        {
            _dataList = null;
        }

        #endregion

        [Test]
        public void Condition_And_Condition_Test()
        {
            //Product=PL And Cycle>2
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Product == ScbEnums.Products.PL && x.Cycle > 2).ToList();

            var tokens = _testingBillTokens.Condition_And_Condition_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);

        }

        [Test]
        public void Condition_Or_Condition_Test()
        {
            //Product=PL Or Cycle>2
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Product == ScbEnums.Products.PL || x.Cycle > 2).ToList();

            var tokens = _testingBillTokens.Condition_Or_Condition_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Condition_And_Condition_And_Condtion_Test()
        {
            //Product=PL And Cycle>2 And TotalDueOnAllocation < 7000
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Product == ScbEnums.Products.PL
                && x.Cycle > 2 && x.TotalDueOnAllocation < 7000).ToList();

            var tokens = _testingBillTokens.Condition_And_Condition_And_Condition_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Condition_Or_Condition_Or_Condition_Test()
        {
            //Product=PL Or Cycle>2 Or TotalDueOnAllocation < 7000
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Product == ScbEnums.Products.PL
                || x.Cycle > 2 || x.TotalDueOnAllocation < 7000).ToList();

            var tokens = _testingBillTokens.Condition_Or_Condition_Or_Condition_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Condition_And_Condition_Or_Condition_Test()
        {
            //Product=PL And Cycle>2 Or TotalDueOnAllocation < 7000
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Product == ScbEnums.Products.PL
                && x.Cycle > 2 || x.TotalDueOnAllocation < 7000).ToList();

            var tokens = _testingBillTokens.Condition_And_Condition_Or_Condition_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Condition_Or_Condition_And_Condition_Test()
        {
            //Product=PL Or Cycle>2 And TotalDueOnAllocation < 7000
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => (x.Product == ScbEnums.Products.PL)
                || (x.Cycle > 2 && x.TotalDueOnAllocation < 7000)).ToList();

            var tokens = _testingBillTokens.Condition_Or_Condition_And_Condition_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Condition_And_Condition_And_Condition_And_Condition()
        {
            //Product=PL And Cycle>2 And TotalDueOnAllocation < 7000 And TotalAmountRecovered > 1000
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Product == ScbEnums.Products.PL
                && x.Cycle > 2 && x.TotalDueOnAllocation < 7000
                && x.TotalAmountRecovered > 1000).ToList();

            var tokens = _testingBillTokens.Condition_And_Condition_And_Condition_And_Condition();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Condition_Or_Condition_Or_Condition_Or_Condition()
        {
            //Product=PL Or Cycle>2 Or TotalDueOnAllocation < 7000 Or TotalAmountRecovered > 1000
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Product == ScbEnums.Products.PL
                || x.Cycle > 2
                || x.TotalDueOnAllocation < 7000
                || x.TotalAmountRecovered > 1000).ToList();

            var tokens = _testingBillTokens.Condition_Or_Condition_Or_Condition_Or_Condition();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Condition_And_Condition_Or_Condition_And_Condition()
        {
            //Product=PL And Cycle>2 Or TotalDueOnAllocation < 7000 And TotalAmountRecovered > 1000
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => (x.Product == ScbEnums.Products.PL
                && x.Cycle > 2)
                || (x.TotalDueOnAllocation < 7000
                && x.TotalAmountRecovered > 1000)).ToList();

            var tokens = _testingBillTokens.Condition_And_Condition_Or_Condition_And_Condition();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Condition_And_Condition_And_Condition_Or_Condition()
        {
            //Product=PL And Cycle>2 And TotalDueOnAllocation < 7000 Or TotalAmountRecovered > 1000
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Product == ScbEnums.Products.PL
                && x.Cycle > 2
                && x.TotalDueOnAllocation < 7000
                || x.TotalAmountRecovered > 1000).ToList();

            var tokens = _testingBillTokens.Condition_And_Condition_And_Condition_Or_Condition();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Condition_Or_Condition_And_Condition_Or_Condition()
        {
            //Product=PL Or Cycle>2 And TotalDueOnAllocation < 7000 Or TotalAmountRecovered > 1000
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Product == ScbEnums.Products.PL
                || x.Cycle > 2
                && x.TotalDueOnAllocation < 7000
                || x.TotalAmountRecovered > 1000).ToList();

            var tokens = _testingBillTokens.Condition_Or_Condition_And_Condition_Or_Condition();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Condition_Or_Condition_Or_Condition_And_Condition()
        {
            //Product=PL Or Cycle>2 Or TotalDueOnAllocation < 7000 And TotalAmountRecovered > 1000
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Product == ScbEnums.Products.PL
                || x.Cycle > 2
                || x.TotalDueOnAllocation < 7000
                && x.TotalAmountRecovered > 1000).ToList();

            var tokens = _testingBillTokens.Condition_Or_Condition_Or_Condition_And_Condition();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void MultipleAnd_ConditionTest()
        {
            // condition : City = "Pune" &&  CityCategory = "Tier1" && Flag = "O" && Product = "PL"
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.City == "Pune" && x.CityCategory == ColloSysEnums.CityCategory.Tier1
                                && x.Flag == ColloSysEnums.DelqFlag.O && x.Product == ScbEnums.Products.PL).ToList();

            var tokens = _testingBillTokens.City_CityCategory_Flag_Product_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);

        }
    }
}
//[Test]
//public void Output_And_Condition_Test()
//{

//}

//[Test]
//public void Condition_And_Output_Test()
//{

//}

//[Test]
//public void Output_Or_Condition_Test()
//{

//}

//[Test]
//public void Condition_Or_Output_Test()
//{

//}