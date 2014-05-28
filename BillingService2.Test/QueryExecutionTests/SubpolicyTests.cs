#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Billing;
using ColloSys.QueryBuilder.Test.DataGeneration;
using ColloSys.QueryBuilder.Test.QueryExecution;
using NUnit.Framework;

#endregion

namespace ColloSys.QueryBuilder.Test.QueryExecutionTests
{
    [TestFixture]
    public class SubpolicyTests
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

            CollectionAssert.AreEqual(result,actual);
        }

        #endregion


    }
}
