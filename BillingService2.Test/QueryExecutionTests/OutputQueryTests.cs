#region references

using System;
using System.Collections.Generic;
using BillingService2.Calculation;
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

        private void ForEachFunction(CustBillViewModel cust, decimal value,BillingInfoManager billingInfoManager)
        {
            cust.TotalAmountRecovered = value;
        }

        [Test]
        public void Field_Plus_Value_Test()
        {
            // output : Cycle + 2
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.TotalAmountRecovered = x.Cycle + 2);

            var tokens = _testingBillTokens.Field_Plus_Value_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens)
              {
                  ForEachFuction = ForEachFunction
              };
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result[0].TotalAmountRecovered, actualDataList[0].TotalAmountRecovered);
        }

        [Test]
        public void Value_Plus_Field()
        {
            // output : 2 + Cycle
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.TotalAmountRecovered = 2 + x.Cycle);

            var tokens = _testingBillTokens.Value_Plus_Field_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens)
            {
                ForEachFuction = ForEachFunction
            };
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result[0].TotalAmountRecovered, actualDataList[0].TotalAmountRecovered);
        }

        [Test]
        public void Field_Multiply_By_Value_Test()
        {
            // output : TotalDueOnAllocation * 0.02
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.TotalAmountRecovered = (x.TotalDueOnAllocation * (decimal)0.02));

            var tokens = _testingBillTokens.Field_Multiply_By_Value_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens)
            {
                ForEachFuction = ForEachFunction
            };
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result[0].Bucket, actualDataList[0].Bucket);
        }

        [Test]
        public void Value_Multiply_By_Field_Test()
        {
            // output :   0.02 * TotalDueOnAllocation
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.TotalAmountRecovered = ((decimal)0.02 * x.TotalDueOnAllocation));

            var tokens = _testingBillTokens.Value_Multiply_By_Field_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens)
            {
                ForEachFuction = ForEachFunction
            };
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result[0].Bucket, actualDataList[0].Bucket);
        }

        [Test]
        public void Field_Divide_Value_Test()
        {
            // output : TotalDueOnAllocation / 2
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.TotalAmountRecovered = (x.TotalDueOnAllocation / 2));

            var tokens = _testingBillTokens.Field_Divide_By_Value_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens)
            {
                ForEachFuction = ForEachFunction
            };
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result[0].Bucket, actualDataList[0].Bucket);
        }

        [Test]
        public void Value_Divide_By_Field_Test()
        {
            // output :   2000 / TotalDueOnAllocation
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.TotalAmountRecovered = (2000 / x.TotalDueOnAllocation));

            var tokens = _testingBillTokens.Value_Divide_By_Field_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens)
            {
                ForEachFuction = ForEachFunction
            };
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result[0].Bucket, actualDataList[0].Bucket);
        }

        [Test]
        public void Field_Minus_Value_Test()
        {
            // output : TotalDueOnAllocation - 100
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.TotalAmountRecovered = (x.TotalDueOnAllocation - 100));

            var tokens = _testingBillTokens.Field_Minus_Value_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens)
            {
                ForEachFuction = ForEachFunction
            };
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result[0].Bucket, actualDataList[0].Bucket);
        }

        [Test]
        public void Value_Minus_Field_Test()
        {
            // output :   2000-TotalDueOnAllocation
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.TotalAmountRecovered = (2000 - x.TotalDueOnAllocation));

            var tokens = _testingBillTokens.Value_Minus_Field_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens)
            {
                ForEachFuction = ForEachFunction
            };
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result[0].Bucket, actualDataList[0].Bucket);
        }

        [Test]
        public void Field_Plus_Field_Multiply_By_Value_Test()
        {
            // output :   TotalDueOnAllocation + TotalAmountRecovered * 2
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.TotalAmountRecovered = (x.TotalDueOnAllocation + x.TotalAmountRecovered * 2));

            var tokens = _testingBillTokens.Field_Plus_Field_Multiply_By_Value_Tokens();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens)
            {
                ForEachFuction = ForEachFunction
            };
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result[0].Bucket, actualDataList[0].Bucket);
        }

        [Test]
        public void Field_Multiply_By_Value_Plus_Field()
        {
            // output :   TotalDueOnAllocation * 2 + TotalAmountRecovered 
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.TotalAmountRecovered = (x.TotalDueOnAllocation * 2 + x.TotalAmountRecovered));

            var tokens = _testingBillTokens.Field_Multiply_By_Value_Plus_Field();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens)
            {
                ForEachFuction = ForEachFunction
            };
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result[0].Bucket, actualDataList[0].Bucket);
        }

        [Test]
        public void Field_Multiply_By_Field_Divided_By_Value()
        {
            // output :   TotalDueOnAllocation * TotalAmountRecovered / 2
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.TotalAmountRecovered = (x.TotalDueOnAllocation * x.TotalAmountRecovered / 2));

            var tokens = _testingBillTokens.Field_Multiply_By_Field_Divided_By_Value();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens)
            {
                ForEachFuction = ForEachFunction
            };
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result[0].Bucket, actualDataList[0].Bucket);
        }

        [Test]
        public void Field_Minus_Value_Multiply_By_Vaue()
        {
            // output :   TotalDueOnAllocation - 100 * 2
            var actualDataList = _testingBillTokens.GenerateData();
            actualDataList.ForEach(x => x.TotalAmountRecovered = (x.TotalDueOnAllocation - 100 * 2));

            var tokens = _testingBillTokens.Field_Minus_Value_Multiply_By_Value();
            var tokenBuilder = new QueryExecuter<CustBillViewModel>(tokens)
            {
                ForEachFuction = ForEachFunction
            };
            var result = tokenBuilder.ExeculteOnList(_dataList);

            Assert.AreEqual(result[0].Bucket, actualDataList[0].Bucket);
        }

    }
}
