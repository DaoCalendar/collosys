#region references

using System;
using System.Linq;
using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using LinqKit;
using NHibernate.Criterion;
using NHibernate.Linq;
using NUnit.Framework;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using DynamicExpression = System.Linq.Dynamic.DynamicExpression;
using Expression = System.Linq.Expressions.Expression;

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

        [Test]
        public void GreaterThan_StringTest()
        {
            var result = _dataList.AsQueryable().Where("Cycle > 2").ToList();
            var actual = _dataList.Where(x => x.Cycle > 2).ToList();
            Assert.AreEqual(result.Count(), actual.Count());
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

        [Test]
        public void SumnGreaterThan_StringTest()
        {
            var result = _dataList.AsQueryable().Where("Cycle + 2 > 0").ToList();
            var actual = _dataList.Where(x => x.Cycle + 2 > 0).ToList();
            Assert.AreEqual(result.Count(), actual.Count());
        }

        #region Token by Mayur

        #region Condition
        // x => x.Product == ScbEnums.Products.PL
        private IList<BillTokens> ProductEqualPL_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.Product", Priority = 0, DataType = "enum"},
                new BillTokens {Type = "Operator", Value = "Equal", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "PL", Priority = 2, DataType = "enum"}
            };
            return query;
        }

        [Test]
        public void ProductEqualPL_Test()
        {
            var condtions = ProductEqualPL_Tokens();
            var query = _builder.GenerateConditionalQuery(condtions);
            var result = _builder.ExecuteCondition(_dataList, query);
            var actual = _dataList.Count(x => x.Product == ScbEnums.Products.PL);
            Assert.AreEqual(result.Count, actual);
        }

        [Test]
        public void ProductEqualPL_StringTest()
        {
            var result = _dataList.AsQueryable().Where("Product = \"PL\"").ToList();
            var actual = _dataList.Where(x => x.Product == ScbEnums.Products.PL).ToList();
            Assert.AreEqual(result.Count(), actual.Count());
        }

        // x => x.CityCategory.IsIn(new object[] { "Metro", "A" })
        private IList<BillTokens> CityCategoryIsIn_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.CityCategory", Priority = 0, DataType = "enum"},
                new BillTokens {Type = "Operator", Value = "IsIn", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "Metro, A", Priority = 2, DataType = "enum"}
            };
            return query;
        }

        [Test]
        public void CityCategoryIsIn_Test()
        {
            var condtions = CityCategoryIsIn_Tokens();
            var query = _builder.GenerateConditionalQuery(condtions);
            var result = _builder.ExecuteCondition(_dataList, query);
            var actual = _dataList.Where(x => x.CityCategory.IsIn(new object[] { "Metro", "A" }));
            Assert.AreEqual(result.Count, actual);
        }

        [Test]
        public void CityCategoryIsIn_StringTest()
        {
            var result = _dataList.AsQueryable().Where("CityCategory.IsIn(\"Metro\",\"A\")").ToList();
            var actual = _dataList.Where(x => x.Product == ScbEnums.Products.PL).ToList();
            Assert.AreEqual(result.Count(), actual.Count());
        }


        // x => x.City == "Pune" && x.CityCategory == ColloSysEnums.CityCategory.Tier1 && x.Flag == ColloSysEnums.DelqFlag.O && x.Product == ScbEnums.Products.PL
        private IList<BillTokens> City_CityCategory_Flag_Product_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table",Value = "CustBillViewModel.City",Priority = 0,DataType = "string"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "Pune", Priority = 2, DataType = "string"},

                new BillTokens {Type = "Operator", Value = "AND", Priority = 3, DataType = "Relational"},

                new BillTokens {Type = "Table", Value = "CustBillViewModel.CityCategory", Priority = 4, DataType = "string"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 5, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "Tier1", Priority = 6, DataType = "string"},

                new BillTokens {Type = "Operator", Value = "AND", Priority = 7, DataType = "Relational"},

                new BillTokens {Type = "Table", Value = "CustBillViewModel.Flag", Priority = 8, DataType = "string"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 9, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "O", Priority = 10, DataType = "string"},

                 new BillTokens {Type = "Operator", Value = "AND", Priority = 11, DataType = "Relational"},

                new BillTokens {Type = "Table", Value = "CustBillViewModel.Product", Priority = 12, DataType = "string"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 13, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "PL", Priority = 14, DataType = "string"}
            };
            return query;
        }

        [Test]
        public void City_CityCategory_Flag_Product_Test()
        {
            var condtions = City_CityCategory_Flag_Product_Tokens();
            var query = _builder.GenerateConditionalQuery(condtions);
            var result = _builder.ExecuteCondition(_dataList, query);
            var actual = _dataList.Count(x => x.City == "Pune" && x.CityCategory == ColloSysEnums.CityCategory.Tier1
                                                && x.Flag == ColloSysEnums.DelqFlag.O && x.Product == ScbEnums.Products.PL);
            Assert.AreEqual(result.Count, actual);
        }

        [Test]
        public void City_CityCategory_Flag_Product_StringTest()
        {
            var result = _dataList.AsQueryable().Where("City = \"pune\" && CityCategory = \"Tier1\" && Flag = \"O\" && Product = \"PL\"").ToList();
            var actual = _dataList.Where(x => x.City == "pune" && x.CityCategory == ColloSysEnums.CityCategory.Tier1
                                                    && x.Flag == ColloSysEnums.DelqFlag.O && x.Product == ScbEnums.Products.PL).ToList();
            Assert.AreEqual(result.Count(), actual.Count());
        }

        // x => (x.TotalAmountRecovered * (decimal)0.02) >= 10000
        private IList<BillTokens> TotalAmountRecoveredMultiPlay2PerGraterThenEqual10000_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table", Value = "CustBillViewModel.TotalAmountRecovered", Priority = 0, DataType = "number"},
                 new BillTokens {Type = "Operator", Value = "Multiply", Priority = 1, DataType = "Arithmetic"},
                  new BillTokens {Type = "Value", Value = "0.02", Priority = 1, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "GreaterThenEqual", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "10000", Priority = 2, DataType = "number"}
            };
            return query;
        }

        [Test]
        public void TotalAmountRecoveredMultiPlay2PerGraterThenEqual10000_Test()
        {
            var condtions = TotalAmountRecoveredMultiPlay2PerGraterThenEqual10000_Tokens();
            var query = _builder.GenerateConditionalQuery(condtions);
            var result = _builder.ExecuteCondition(_dataList, query);
            var actual = _dataList.Count(x => (x.TotalAmountRecovered * (decimal)0.02) >= 10000);
            Assert.AreEqual(result.Count, actual);
        }

        [Test]
        public void TotalAmountRecoveredMultiPlay2PerGraterThenEqual10000_StringTest()
        {
            var result = _dataList.AsQueryable().Where("TotalAmountRecovered * 0.02 > 10000").ToList();
            var actual = _dataList.Where(x => x.TotalAmountRecovered * (decimal)0.02 > 10000).ToList();
            Assert.AreEqual(result.Count(), actual.Count());
        }

        #endregion

        #region Formula

        // x => (x.TotalAmountRecovered * (decimal)0.02) >= 10000
        private IList<BillTokens> FormulaTwoPerTotalAmountRecoveredGraterThenEqual10000_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Formula", Value = "TwoPerTotalAmountRecovered", Priority = 0, DataType = "number"},
                new BillTokens {Type = "Operator", Value = "GreaterThenEqual", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "10000", Priority = 2, DataType = "number"}
            };
            return query;
        }

        [Test]
        public void FormulaTwoPerTotalAmountRecoveredGraterThenEqual10000_Test()
        {
            var condtions = FormulaTwoPerTotalAmountRecoveredGraterThenEqual10000_Tokens();
            var query = _builder.GenerateConditionalQuery(condtions);
            var result = _builder.ExecuteCondition(_dataList, query);
            var actual = _dataList.Count(x => (x.TotalAmountRecovered * (decimal)0.02) >= 10000);
            Assert.AreEqual(result.Count, actual);
        }

        #endregion

        #region Ouput

        // dataList.ForEach(x => x.TotalDueOnAllocation = (x.TotalAmountRecovered * (decimal)0.02))s
        private IList<BillTokens> TotalAmountRecoveredMultiPlay2Per_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalAmountRecovered",Priority = 0,DataType = "number"},
                new BillTokens {Type = "Operator", Value = "Multiply", Priority = 1, DataType = "Arithmetic"},
                new BillTokens {Type = "Value", Value = "0.02", Priority = 1, DataType = "number"}
            };
            return query;
        }

        [Test]
        public void TotalAmountRecoveredMultiPlay2Per_Test()
        {
            var condtions = TotalAmountRecoveredMultiPlay2Per_Tokens();
            var query = _builder.GenerateConditionalQuery(condtions);
            var actual = new List<CustBillViewModel>(_dataList);
            var dataList = new List<CustBillViewModel>(_dataList);
            _builder.ExecuteOutput(dataList, query);
            actual.ForEach(x => x.TotalDueOnAllocation = (x.TotalAmountRecovered * (decimal)0.02));
            Assert.AreEqual(dataList, actual);
        }

        /// <summary>
        /// See this For Output
        /// </summary>
        [Test]
        public void TotalAmountRecoveredMultiPlay2Per_StringTest()
        {
            const string queryString = @"(Cycle + 3)";
            var expression = DynamicExpression.ParseLambda<CustBillViewModel, uint>(queryString);

            var actual = new List<CustBillViewModel>(_dataList);
            var dataList = new List<CustBillViewModel>(_dataList);

            dataList.ForEach(x => x.Bucket = expression.Compile().Invoke(x));
            actual.ForEach(x => x.Bucket = (x.Cycle + 2));

            Assert.AreEqual(dataList.First().Bucket, actual.First().Bucket);
        }

        // dataList.ForEach(x => x.ResolutionPercentage = (x.TotalAmountRecovered / x.ResolutionPercentage))
        private IList<BillTokens> TotalAmountRecoveredDivideResolutionPercentage_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens{Type = "Table",Value = "CustBillViewModel.TotalAmountRecovered",Priority = 0,DataType = "number"},
                new BillTokens {Type = "Operator", Value = "Divide", Priority = 1, DataType = "Arithmetic"},
                new BillTokens {Type = "Value", Value = "CustBillViewModel.ResolutionPercentage", Priority = 1, DataType = "number"}
            };
            return query;
        }

        [Test]
        public void TotalAmountRecoveredDivideResolutionPercentage_Test()
        {
            var actual = new List<CustBillViewModel>(_dataList);
            var dataList = new List<CustBillViewModel>(_dataList);

            var condtions = TotalAmountRecoveredDivideResolutionPercentage_Tokens();
            var query = _builder.GenerateConditionalQuery(condtions);
            _builder.ExecuteOutput(dataList, query);
            actual.ForEach(x => x.TotalDueOnAllocation = (x.TotalAmountRecovered * (decimal)0.02));
            Assert.AreEqual(dataList, actual);
        }


        #endregion

        #endregion
    }

    public class ExpressionBuilder<T> where T : CustBillViewModel
    {
        #region ctor
        private readonly ParameterExpression _parameterExpression;

        public ExpressionBuilder()
        {
            _parameterExpression = Expression.Parameter(typeof(T), "x");
        }
        #endregion

        #region convertor

        private Type GetType(string dataType)
        {
            switch (dataType)
            {
                case "string":
                    return typeof(string);
                case "number":
                    return typeof(decimal);
                case "enum":
                    return typeof(Enum);
                default:
                    throw new ArgumentOutOfRangeException("dataType");
            }
        }

        public Expression ConvertColumnToExpression(BillTokens token)
        {
            var field = token.Value.Replace("CustBillViewModel.", "");
            var exField = Expression.PropertyOrField(_parameterExpression, field);
            return Expression.Convert(exField, GetType(token.DataType));
        }

        public Expression ConvertValueToExpression(BillTokens token)
        {
            var fieldValue = Convert.ChangeType(token.Value, GetType(token.DataType));
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

            var rhsTokens = tokens.Skip(index + 1).ToList();
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

        public List<T> ExecuteOutput(IEnumerable<T> data, Expression condition)
        {
            var expression = Expression.Lambda<decimal>(condition, _parameterExpression);

            TypeHelperExtensionMethods.ForEach(data, x => x.ResolutionPercentage = expression.Compile());



            return data.ToList();
        }
        #endregion
    }
}
