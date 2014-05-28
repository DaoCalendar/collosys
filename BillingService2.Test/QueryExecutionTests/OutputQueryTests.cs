#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using ColloSys.QueryBuilder.Test.DataGeneration;
using ColloSys.QueryBuilder.Test.QueryExecution;
using NUnit.Framework;

#endregion

namespace ColloSys.QueryBuilder.Test.QueryExecutionTests
{
    [TestFixture]
    public class OutputQueryTests
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
        public void SumOfTwoTokensTest()
        {
            // output : Cycle + 2
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.Bucket = x.Cycle + 2);

            var tokens = _testingBillTokens.SumOfTwoTokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.Inconclusive(result.ToString());
        }

        [Test]
        public void SumOfTwoTokensReverseTest()
        {
            // output : 2 + Cycle
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.Bucket = 2 + x.Cycle);

            var tokens = _testingBillTokens.SumOfTwoTokensReverse();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actualDataList.Count);
        }

        [Test]
        public void TotalAmountRecoveredMultiPlay2Per_TokensTest()
        {
            // output : TotalDueOnAllocation * 0.02
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.Bucket = (uint)(x.TotalAmountRecovered * (decimal)0.02));

            var tokens = _testingBillTokens.TotalAmountRecoveredMultiPlay2Per_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actualDataList.Count);
        }

        //TODO: MAHENDRA: is this correct name - are you testing variable names or logic??
        [Test]
        public void TotalAmountRecoveredDivideResolutionPercentage_TokensTest()
        {
            // output : TotalAmountRecovered / ResolutionPercentage
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.Bucket = (uint)(x.TotalAmountRecovered * x.ResolutionPercentage));

            var tokens = _testingBillTokens.TotalAmountRecoveredDivideResolutionPercentage_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actualDataList.Count);
        }
    }
}
