#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.Test.DataGeneration;
using ColloSys.QueryBuilder.Test.QueryExecution;
using NUnit.Framework;

#endregion

namespace ColloSys.QueryBuilder.Test.QueryExecutionTests
{
    //TODO: HARISH: <, <=, >= tests for String, Number, Date 
    //TODO: HARISH: needs atleast 20-30 more tests, grouped by region per datatype
    //1. tc > f, f >= number, delqDate > Today/tomorrow/yesterday/etc
    public class ConditionalQueryTests
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
        public void GreaterThan_ConditionTest()
        {
            // condition : Cycle > 0
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Cycle > 0).ToList();

            var tokens = _testingBillTokens.GreaterThanTokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Sum_GreaterThan_ConditionTest()
        {
            // condition : Cycle + 2 > 0
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Cycle + 2 > 0).ToList();

            var tokens = _testingBillTokens.SumNGreaterThanTokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void GreaterThanNSum_ConditionTest()
        {
            // condition : Cycle > 2 + 0
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Cycle > 2 + 0).ToList();

            var tokens = _testingBillTokens.GreaterThanNSumTokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void EqualEnum_ConditionTest()
        {
            // condition : Product = PL
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Product == ScbEnums.Products.PL).ToList();

            var tokens = _testingBillTokens.ProductEqualPL_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void StringIsIn_ConditionTest()
        {
            // condition : CityCategory IsIn ("Metro", "A")
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Product == ScbEnums.Products.PL).ToList();

            var tokens = _testingBillTokens.CityCategoryIsIn_Tokens();
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

        [Test]
        public void Multiply_GreaterThan_ConditionTest()
        {
            // condition : TotalAmountRecovered * 0.02 > 10000
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.TotalAmountRecovered * (decimal)0.02 > 10000).ToList();

            var tokens = _testingBillTokens.TotalAmountRecoveredMultiPlay2PerGraterThenEqual10000_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }
    }
}
