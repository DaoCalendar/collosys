using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.SessionMgr;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.BillingTest
{
    [TestFixture]
    class StringExpressionBuilderTests
    {
        IList<CustBillViewModel> _dataList = new List<CustBillViewModel>();
        private ExpressionBuilder<CustBillViewModel> _builder;

        [SetUp]
        public void InitData()
        {
            var session = SessionManager.GetCurrentSession();
            _dataList = session.QueryOver<CustBillViewModel>().List();
            _builder = new ExpressionBuilder<CustBillViewModel>();
        }

        private IList<BillTokens> GreaterThanTokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "0", Priority = 2, DataType = "number"}
            };
            return query;
        }
    }
}
