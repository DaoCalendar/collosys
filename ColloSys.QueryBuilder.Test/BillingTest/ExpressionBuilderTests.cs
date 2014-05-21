#region references

using System;
using System.Linq;
using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.SessionMgr;
using NUnit.Framework;
using System.Linq.Expressions;

#endregion

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

        [Test]
        public void GreaterThanTest()
        {
            var condtions = GreaterThanTokens();
            var query = _builder.GenerateConditionalQuery(condtions);
            var result = _builder.ExecuteCondition(_dataList, query);
            Assert.AreEqual(result.Count, 0);
        }

        private IList<BillTokens> SumOfTwoTokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number"},
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number"}
            };
            return query;
        }

        [Test]
        public void SumTest()
        {
            var condtions = SumOfTwoTokens();
            var query = _builder.GenerateMathQuery(condtions);
            var result = _builder.ExecuteOutput(_dataList, query);
            Assert.AreEqual(result.Count, _dataList.Count);
        }

        private IList<BillTokens> SumOfTwoTokensReverse()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Value", Value = "2", Priority = 2, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number"},
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number"}
            };
            return query;
        }

        [Test]
        public void SumTestByValue()
        {
            var condtions = SumOfTwoTokensReverse();
            var query = _builder.GenerateMathQuery(condtions);
            var result = _builder.ExecuteOutput(_dataList, query);
            Assert.AreEqual(result.Count, _dataList.Count);
        }

        private IList<BillTokens> SumNGreaterThanTokens()
        {
            // and or : relational & gt, lt : conditional & sum, count , avg : Sql/number
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Cycle", Priority = 0, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "Plus", Priority = 1, DataType = "number"},
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
            var query = _builder.GenerateConditionalQuery(condtions);
            var result = _builder.ExecuteCondition(_dataList, query);
            var actual = _dataList.Count(x => (x.Cycle + 2) > 0);
            Assert.AreEqual(result.Count, actual);
        }
    }

    public class ExpressionBuilder<T>
    {
        #region ctor
        private readonly ParameterExpression _parameterExpression;

        public ExpressionBuilder()
        {
            _parameterExpression = Expression.Parameter(typeof(T), "x");
        }
        #endregion

        #region convertor
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
        #endregion

        #region combine
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

        public Expression CombineExpressionsByRelation(Expression lhsExpression, Expression rhsExpression, BillTokens tokens)
        {
            if (tokens.Type != "Operator") throw new ArgumentOutOfRangeException("tokens");
            switch (tokens.Value)
            {
                case "And":
                    return Expression.And(lhsExpression, rhsExpression);
                case "Or":
                    return Expression.Or(lhsExpression, rhsExpression);
                default:
                    throw new ArgumentOutOfRangeException("tokens");
            }
        }
        #endregion

        #region query generators
        public Expression AppendTokensToExpression(Expression expression, BillTokens operToken, BillTokens fieldToken)
        {
            var rhs = ConvertToken2Expression(fieldToken);
            return CombineExpressionsByOperator(expression, rhs, operToken);
        }

        public Expression GenerateMathQuery(IList<BillTokens> tokens)
        {
            var expression = ConvertToken2Expression(tokens[0]);
            for (var i = 1; i < tokens.Count; i = i + 2)
                expression = AppendTokensToExpression(expression, tokens[i], tokens[i + 1]);
            return expression;
        }

        public Expression GenerateConditionalQuery(IList<BillTokens> tokens)
        {
            var indexToken = tokens.First(x => x.Type == "Operator" && x.DataType == "conditional");
            var index = tokens.IndexOf(indexToken);
            
            var lhsTokens = tokens.Skip(0).Take(index).ToList();
            var lhsExpression = GenerateMathQuery(lhsTokens);

            var rhsTokens = tokens.Skip(index+1).ToList();
            var rhsExpression = GenerateMathQuery(rhsTokens);

            return CombineExpressionsByOperator(lhsExpression, rhsExpression, tokens[index]);
        }

        private Expression GenerateAndOrQuery(Expression expression, IList<BillTokens> tokens)
        {
            var indexToken = tokens.FirstOrDefault(x => x.Type == "Operator" && x.DataType == "conditional");
            IList<BillTokens> lhsTokens, remaingingTokens;
            BillTokens relationToken = tokens[0];
            if (indexToken == null)
            {
                remaingingTokens = null;
                lhsTokens = tokens.Skip(1).ToList();
            }
            else
            {
                var relationIndex = tokens.IndexOf(indexToken);
                lhsTokens = tokens.Skip(1).Take(relationIndex).ToList();
                remaingingTokens = tokens.Skip(relationIndex + 1).ToList();
            }

            var lhsExpression = GenerateConditionalQuery(lhsTokens);
            var expression2 = CombineExpressionsByRelation(expression, lhsExpression, relationToken);

            return remaingingTokens == null
                ? expression
                : GenerateAndOrQuery(expression2, remaingingTokens);
        }

        public Expression GenerateAndOrQuery(IList<BillTokens> tokens)
        {
            var indexToken = tokens.First(x => x.Type == "Operator" && x.DataType == "conditional");
            var index = tokens.IndexOf(indexToken);

            var lhsTokens = tokens.Skip(0).Take(index).ToList();
            var lhsExpression = GenerateConditionalQuery(lhsTokens);

            return GenerateAndOrQuery(lhsExpression, tokens.Skip(index).ToList());
        }
        #endregion

        #region execute
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
        #endregion
    }
}
