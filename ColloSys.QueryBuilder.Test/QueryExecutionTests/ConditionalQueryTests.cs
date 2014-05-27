#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.Test.DataGeneration;
using ColloSys.QueryBuilder.Test.QueryExecution;
using NHibernate.Criterion;
using NUnit.Framework;

#endregion

namespace ColloSys.QueryBuilder.Test.QueryExecutionTests
{
    //TODO: HARISH: <, <=, >= tests for String, Number, Date 
    //TODO: HARISH: needs atleast 20-30 more tests, grouped by region per datatype
    //TODO:=,contains,startwith, endswith,notequal to not in
    //1. tc > f, f >= number, string -> contains, equal etc, date >, < etc.

    [TestFixture]
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

        #region number data type tests

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
        public void GreaterThanEqualTo_ConditionTest()
        {
            //condition : Cycle >= 0
            var actualDataLIst = _testingBillTokens.GenerateData();
            var actual = actualDataLIst.Where(x => x.Cycle >= 3).ToList();

            var tokens = _testingBillTokens.GreaterThanEqualToTokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void LessThan_ConditionTest()
        {
            //condition: Cycle < 4
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Cycle < 4).ToList();

            var tokens = _testingBillTokens.LessThanTokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void LessThanEqualTo_ConditionTest()
        {
            //condition: Cycle<=3
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Cycle <= 3).ToList();

            var tokens = _testingBillTokens.LessThanEqualToTokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Sum_LessThan_ConditionTest()
        {
            //condition: Cycle + 2 < 5
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Cycle + 2 < 5).ToList();

            var tokens = _testingBillTokens.SumNLessThanTokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void LessThan_Sum_ConditionTest()
        {
            //condition: Cycle < 0 + 4
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Cycle < 0 + 4).ToList();

            var tokens = _testingBillTokens.LessThanNSumTokens();
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
        public void EqualTo_ConditionTest()
        {
            //condition : Cycle = 2
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Cycle == 3).ToList();

            var tokens = _testingBillTokens.EqualToTokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void NotEqualTo_ConditionTest()
        {
            //condition : Cycle != 2
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Cycle != 2).ToList();

            var tokens = _testingBillTokens.NotEqualToTokens();
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

        [Test]
        public void Multiply_LessThan_ConditionTest()
        {
            // condition : TotalAmountRecovered * 0.02 < 10000
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.TotalAmountRecovered * (decimal)0.02 < 10000).ToList();

            var tokens = _testingBillTokens.TotalAmountRecoveredMultiPlay2PerLessThenEqual10000_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        #endregion

        #region string type tests

        [Test]
        public void String_Equals_Test()
        {
            //condition : City ='pune'
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.City == "Pune").ToList();

            var tokens = _testingBillTokens.EqualTo_String_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void String_NotEquals_Test()
        {
            //condition : City !='pune'
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.City != "Pune").ToList();

            var tokens = _testingBillTokens.Not_EqualTo_String_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void String_Contains_Test()
        {
            //condition : City contains 'pune'
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.City.Contains("Pune")).ToList();

            var tokens = _testingBillTokens.Contains_String_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void String_StartsWith_Test()
        {
            //condition : City contains 'pune'
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.City.StartsWith("p")).ToList();

            var tokens = _testingBillTokens.StartsWith_String_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void String_EndsWith_Test()
        {
            //condition : City contains 'pune'
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.City.EndsWith("e")).ToList();

            var tokens = _testingBillTokens.EndsWith_String_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void String_NotIn_Test()
        {
            //condition : City contains 'pune'
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.City.EndsWith("e")).ToList();

            var tokens = _testingBillTokens.NotIn_String_Tokens();
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

        #endregion

        #region date type tests

        [Test]
        public void Greater_Than_Date_Test()
        {
            // condition : ChargeofDate> 01/02/2014 
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.ChargeofDate > DateTime.Parse("01/02/2014")).ToList();

            var tokens = _testingBillTokens.GreaterThan_Date_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Greater_Than_EqualTo_Date_Test()
        {
            // condition : ChargeofDate >= 01/02/2014 
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.ChargeofDate >= DateTime.Parse("01/02/2014")).ToList();

            var tokens = _testingBillTokens.GreaterThan_EqualTo_Date_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Less_Than_Date_Test()
        {
            // condition : ChargeofDate< 01/02/2014 
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.ChargeofDate < DateTime.Parse("01/02/2014")).ToList();

            var tokens = _testingBillTokens.LessThan_Date_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Less_Than_EqualTo_Date_Test()
        {
            // condition : ChargeofDate<= 01/02/2014 
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.ChargeofDate <= DateTime.Parse("01/02/2014")).ToList();

            var tokens = _testingBillTokens.LessThan_EqualTo_Date_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void EqualTo_Date_Test()
        {
            // condition : ChargeofDate<= 01/02/2014 
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.ChargeofDate == DateTime.Parse("01/02/2014")).ToList();

            var tokens = _testingBillTokens.EqualTo_Date_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void NotEqualTo_Date_Test()
        {
            // condition : ChargeofDate<= 01/02/2014 
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.ChargeofDate != DateTime.Parse("01/02/2014")).ToList();

            var tokens = _testingBillTokens.NotEqualToTokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }
        #endregion

        #region enum type tests

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
        public void NotEqualEnum_ConditionTest()
        {
            // condition : Product != PL
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Product != ScbEnums.Products.PL).ToList();

            var tokens = _testingBillTokens.ProductNotEqualPL_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        #endregion

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
