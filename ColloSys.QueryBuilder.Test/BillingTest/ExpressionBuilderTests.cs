using System;
using System.Linq;
using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.SessionMgr;
using NUnit.Framework;
using System.Linq.Expressions;


namespace ColloSys.QueryBuilder.Test.BillingTest
{
    [TestFixture]
    public class ExpressionBuilderTests
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

        private IEnumerable<BillTokens> GreaterThanTokens()
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

        [Test]
        public void GreaterThanTest()
        {
            var condtions = GreaterThanTokens();
            var query = _builder.GenerateQuery(condtions);
            var result = _builder.ExecuteCondition(_dataList, query);
            Assert.AreEqual(result.Count, 0);
        }

        private IEnumerable<BillTokens> SumOfTwoTokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number"}
            };
            return query;
        }

        [Test]
        public void SumTest()
        {
            var condtions = SumOfTwoTokens();
            var query = _builder.GenerateQuery(condtions);
            var result = _builder.ExecuteOutput(_dataList, query);
            Assert.AreEqual(result.Count, _dataList.Count);
        }

        private IEnumerable<BillTokens> SumOfTwoTokensByValue()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number"}
            };
            return query;
        }

        [Test]
        public void SumTestByValue()
        {
            var condtions = SumOfTwoTokensByValue();
            var query = _builder.GenerateQuery(condtions);
            var result = _builder.ExecuteOutput(_dataList, query);
            Assert.AreEqual(result.Count, _dataList.Count);
        }

        private IEnumerable<BillTokens> SumNGreaterThanTokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "GreaterThan", Priority = 3, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "0", Priority = 4, DataType = "number"}
            };
            return query;
        }

        [Test]
        public void SumnGreaterThanTest()
        {
            var condtions = SumNGreaterThanTokens();
            var query = _builder.GenerateQuery(condtions);
            var result = _builder.ExecuteCondition(_dataList, query);
            Assert.AreNotEqual(result.Count, 0);
        }
    }

    public class ExpressionBuilder<T>
    {
        private readonly ParameterExpression _parameterExpression;

        public ExpressionBuilder()
        {
            _parameterExpression = Expression.Parameter(typeof(T), "x");
        }

        public Expression ConvertColumnToExpression(BillTokens token)
        {
            var field = token.Value.Replace("CustBillViewModel.", "");
            var exField = Expression.PropertyOrField(_parameterExpression, field);
            return Expression.Convert(exField, typeof(decimal));
        }

        public Expression ConvertValueToExpression(BillTokens token)
        {
            var fieldValue = Convert.ChangeType(token.Value, typeof(decimal));
            return Expression.Constant(fieldValue);
        }

        public Expression ConvertToken2Expression(BillTokens token)
        {
            switch (token.Type)
            {
                case "Table":
                    return ConvertColumnToExpression(token);
                case "Value":
                    return ConvertValueToExpression(token);
                default:
                    throw new ArgumentOutOfRangeException("token");
            }
        }

        public Expression CombineExpressionsByOperator(Expression lhsExpression, Expression rhsExpression, BillTokens tokens)
        {
            if (tokens.Type != "Operator") throw new ArgumentOutOfRangeException("tokens");
            switch (tokens.Value)
            {
                case "GreaterThan":
                    return Expression.GreaterThan(lhsExpression, rhsExpression);
                case "Plus":
                    return Expression.Add(lhsExpression, rhsExpression);
                default:
                    throw new ArgumentOutOfRangeException("tokens");
            }
        }

        public Expression AppendTokensToExpression(Expression expression, BillTokens operToken, BillTokens fieldToken)
        {
            var rhs = ConvertToken2Expression(fieldToken);
            return CombineExpressionsByOperator(expression, rhs, operToken);
        }

        public Expression GenerateQuery(IEnumerable<BillTokens> tokens)
        {
            var billTokenses = tokens as BillTokens[] ?? tokens.ToArray();
            var tokenArray = billTokenses.ToArray();

            var expression = ConvertToken2Expression(tokenArray[0]);
            for (var i = 1; i < tokenArray.Length; i = i + 2)
                expression = AppendTokensToExpression(expression, tokenArray[i], tokenArray[i + 1]);
            return expression;
        }

        public IList<T> ExecuteCondition(IEnumerable<T> data, Expression condition)
        {
            var expression = Expression.Lambda<Func<T, bool>>(condition, _parameterExpression);
            return data.Where(expression.Compile()).ToList();
        }

        public List<decimal> ExecuteOutput(IEnumerable<T> data, Expression condition)
        {
            var expression = Expression.Lambda<Func<T, decimal>>(condition, _parameterExpression);
            return data.Select(expression.Compile()).ToList();
        }
    }
}
