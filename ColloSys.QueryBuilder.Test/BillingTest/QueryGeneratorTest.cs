#region
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using NUnit.Framework;
#endregion

namespace ColloSys.QueryBuilder.Test.BillingTest
{
    public class QueryGeneratorTest
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

        #region Billing Subpolicy

        /// <summary>
        /// condition : Cycle = 0 
        /// output : Cycle + 2
        /// </summary>
        [Test]
        public void GreaterThanWithPlas2TokensTest()
        {
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Cycle == 0).ToList();
            actual.ForEach(x => x.Bucket = (x.Cycle + 2));

            var tokens = _testingBillTokens.GreaterThanWithPlas2Tokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            //CollectionAssert.AreEqual(result,actual);
        }

        #endregion

        #region Output

        /// <summary>
        /// output : Cycle + 2
        /// </summary>
        [Test]
        public void SumOfTwoTokensTest()
        {
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.Bucket = x.Cycle + 2);

            var tokens = _testingBillTokens.SumOfTwoTokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actualDataList.Count);
        }

        /// <summary>
        /// output : 2 + Cycle
        /// </summary>
        [Test]
        public void SumOfTwoTokensReverseTest()
        {
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.Bucket = 2 + x.Cycle);

            var tokens = _testingBillTokens.SumOfTwoTokensReverse();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actualDataList.Count);
        }

        /// <summary>
        /// output : TotalDueOnAllocation * 0.02
        /// </summary>
        [Test]
        public void TotalAmountRecoveredMultiPlay2Per_TokensTest()
        {
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.Bucket = (uint)(x.TotalAmountRecovered * (decimal)0.02));

            var tokens = _testingBillTokens.TotalAmountRecoveredMultiPlay2Per_Tokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actualDataList.Count);
        }

        /// <summary>
        /// output : TotalAmountRecovered / ResolutionPercentage
        /// </summary>
        [Test]
        public void TotalAmountRecoveredDivideResolutionPercentage_TokensTest()
        {
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.Bucket = (uint)(x.TotalAmountRecovered * x.ResolutionPercentage));

            var tokens = _testingBillTokens.TotalAmountRecoveredDivideResolutionPercentage_Tokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actualDataList.Count);
        }

        #endregion

        #region Condition

        /// <summary>
        /// condition : Cycle > 0
        /// </summary>
        [Test]
        public void GreaterThanTokensTest()
        {
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Cycle > 0).ToList();

            var tokens = _testingBillTokens.GreaterThanTokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        /// <summary>
        /// condition : Cycle + 2 > 0
        /// </summary>
        [Test]
        public void SumNGreaterThanTest()
        {
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Cycle + 2 > 0).ToList();

            var tokens = _testingBillTokens.SumNGreaterThanTokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        /// <summary>
        /// condition : Cycle > 2 + 0
        /// </summary>
        [Test]
        public void GreaterThanNSumTokensTest()
        {
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Cycle > 2 + 0).ToList();

            var tokens = _testingBillTokens.GreaterThanNSumTokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        /// <summary>
        /// condition : Product = PL
        /// </summary>
        [Test]
        public void ProductEqualPL_TokensTest()
        {
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Product == ScbEnums.Products.PL).ToList();

            var tokens = _testingBillTokens.ProductEqualPL_Tokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        /// <summary>
        /// condition : CityCategory IsIn ("Metro", "A")
        /// </summary>
        [Test]
        public void CityCategoryIsIn_TokensTest()
        {
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Product == ScbEnums.Products.PL).ToList();

            var tokens = _testingBillTokens.CityCategoryIsIn_Tokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        /// <summary>
        /// condition : City = "Pune" &&  CityCategory = "Tier1" && Flag = "O" && Product = "PL"
        /// </summary>
        [Test]
        public void City_CityCategory_Flag_Product_Tokens_Test()
        {
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.City == "Pune" && x.CityCategory == ColloSysEnums.CityCategory.Tier1
                                && x.Flag == ColloSysEnums.DelqFlag.O && x.Product == ScbEnums.Products.PL).ToList();

            var tokens = _testingBillTokens.City_CityCategory_Flag_Product_Tokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);

        }

        /// <summary>
        /// condition : TotalAmountRecovered * 0.02 > 10000
        /// </summary>
        [Test]
        public void TotalAmountRecoveredMultiPlay2PerGraterThenEqual10000_Tokens_TokensTest()
        {
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.TotalAmountRecovered * (decimal)0.02 > 10000).ToList();

            var tokens = _testingBillTokens.TotalAmountRecoveredMultiPlay2PerGraterThenEqual10000_Tokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        #endregion
    }
}
