#region
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.Test.DataGeneration;
using ColloSys.QueryBuilder.Test.QueryExecution;
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

            var tokens = _testingBillTokens.GreaterThanWithPlus2Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
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
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
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
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
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
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
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
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actualDataList.Count);
        }

        #endregion

        
    }
}
