#region
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using ColloSys.DataLayer.Billing;
using NHibernate.Linq;
using NUnit.Framework;
#endregion

namespace ColloSys.QueryBuilder.Test.BillingTest
{
    public class TokenExeculterTests
    {
        private List<CustBillViewModel> _dataList;
        private readonly TestingBillTokens _testingBillTokens = new TestingBillTokens();

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

        /// <summary>
        /// condition : Cycle = 0 
        /// output : Cycle + 2
        /// </summary>
        [Test]
        public void GreaterThanWithPlas2TokensTest()
        {
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Cycle > 0).ToList();
            actual.ForEach(x => x.Bucket = (x.Cycle + 2));

            var tokens = _testingBillTokens.GreaterThanWithPlas2Tokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            var result = tokenBuilder.ExeculteOnList(_dataList);
        }

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

        //
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
        /// condition : Cycle = 0 
        /// </summary>
        [Test]
        public void City_CityCategory_Flag_Product_Tokens_Test()
        {
            var actualDataList = _testingBillTokens.GenerateData();
            var actual = actualDataList.Where(x => x.Cycle > 0).ToList();
            actual.ForEach(x => x.Bucket = (x.Cycle + 2));

            var tokens = _testingBillTokens.City_CityCategory_Flag_Product_Tokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            tokenBuilder.ExeculteOnList(_dataList);
        }

        [Test]
        public void SumNGreaterThanTest()
        {
            var tokens = _testingBillTokens.SumNGreaterThanTokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            tokenBuilder.ExeculteOnList(_dataList);
        }




        [Test]
        public void GreaterThanNSumTokensTest()
        {
            var tokens = _testingBillTokens.GreaterThanNSumTokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            tokenBuilder.ExeculteOnList(_dataList);
        }

        [Test]
        public void ProductEqualPL_TokensTest()
        {
            var tokens = _testingBillTokens.ProductEqualPL_Tokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            tokenBuilder.ExeculteOnList(_dataList);
        }

        [Test]
        public void CityCategoryIsIn_TokensTest()
        {
            var tokens = _testingBillTokens.CityCategoryIsIn_Tokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            tokenBuilder.ExeculteOnList(_dataList);
        }

        [Test]
        public void TotalAmountRecoveredMultiPlay2PerGraterThenEqual10000_Tokens_TokensTest()
        {
            var tokens = _testingBillTokens.TotalAmountRecoveredMultiPlay2PerGraterThenEqual10000_Tokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            tokenBuilder.ExeculteOnList(_dataList);
        }

        [Test]
        public void TotalAmountRecoveredMultiPlay2Per_TokensTest()
        {
            var tokens = _testingBillTokens.TotalAmountRecoveredMultiPlay2Per_Tokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            tokenBuilder.ExeculteOnList(_dataList);
        }

        [Test]
        public void TotalAmountRecoveredDivideResolutionPercentage_TokensTest()
        {
            var tokens = _testingBillTokens.TotalAmountRecoveredMultiPlay2Per_Tokens();
            var tokenBuilder = new TokenExeculter<CustBillViewModel>(tokens);
            tokenBuilder.ExeculteOnList(_dataList);
        }
    }

    public class TokenExeculter<T> where T : CustBillViewModel
    {
        private readonly IList<BillTokens> _billTokenses;
        private readonly QueryGenerator _stringQueryBuilder;

        public TokenExeculter(IList<BillTokens> billTokenses)
        {
            _billTokenses = billTokenses;
            _stringQueryBuilder = new QueryGenerator();
        }

        public IList<T> ConditionExecuter(IList<T> dataList)
        {
            var conditionToken = _billTokenses.Where(x => x.GroupType == "Condition").ToList();

            if (conditionToken.Count <= 0)
                return dataList;

            var stringConditionQuery = _stringQueryBuilder.GenerateAndOrQuery(conditionToken);

            var conditionExpression = DynamicExpression.ParseLambda<CustBillViewModel, bool>(stringConditionQuery);
            var resultData = dataList.Where(x => conditionExpression.Compile().Invoke(x)).ToList();
            return resultData;
        }

        public IList<T> OutputExecuter(IList<T> dataList)
        {
            var outPutToken = _billTokenses.Where(x => x.GroupType == "Output").ToList();

            if (outPutToken.Count <= 0)
                return dataList;

            var stringOutputQuery = _stringQueryBuilder.GenerateOutputQuery(outPutToken);
            var outputExpression = DynamicExpression.ParseLambda<CustBillViewModel, uint>(stringOutputQuery);
            dataList.ForEach(x => x.Bucket = outputExpression.Compile().Invoke(x));

            return dataList;
        }


        public IList<T> ExeculteOnList(IList<T> dataList)
        {
            var filterData = ConditionExecuter(dataList);
            var resultData = OutputExecuter(filterData);

            return resultData;
        }

    }
}
