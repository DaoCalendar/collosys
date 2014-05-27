using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Billing;
using ColloSys.QueryBuilder.Test.DataGeneration;
using ColloSys.QueryBuilder.Test.QueryExecution;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.QueryExecutionTests
{
    //test fomula in formula, test if else formula, test multi if else formula, etc.
    [TestFixture]
    public class FormulaTests
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
        public void Test_Formula_Within_Formula()
        {
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.TotalDueOnAllocation > 0).ToList();
            actualDataList.ForEach(x => x.TotalDueOnAllocation += 500 + 200);
            var tokens = _testingBillTokens.Formula_within_Formula_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result.Count, actual.Count);
        }

        [Test]
        public void Test_IfElse_Formula()
        {
            
        }

        [Test]
        public void Test_Multi_IfElse_Formula()
        {
            
        }
    }
}
