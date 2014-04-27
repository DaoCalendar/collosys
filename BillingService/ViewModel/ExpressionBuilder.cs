using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using NLog;

namespace BillingService.ViewModel
{
    public static class ExpressionBuilder
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region where Condition Expression

        public static Expression<Func<T, bool>> GetConditionExpression<T>(BillDetail billDetail, List<BCondition> bConditions, List<T> data, TraceLogs traceLogs)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression expression = null;

            for (var i = 0; i < bConditions.Count; i++)
            {
                var bCondition = bConditions[i];
                expression = (expression == null)
                                 ? GetConditionExpression(parameter, billDetail, bCondition, data, traceLogs)
                                 : Expression.AndAlso(expression, GetConditionExpression(parameter, billDetail, bCondition, data, traceLogs));
            }

            return Expression.Lambda<Func<T, bool>>(expression, parameter);
        }

        private static Expression GetConditionExpression<T>(ParameterExpression parameter, BillDetail billDetail, BCondition bCondition, List<T> data, TraceLogs traceLogs)
        {
            Expression left = GetLeftExpression(parameter, billDetail, bCondition, data, traceLogs);
            Expression right = GetRightExpression(parameter, bCondition, left.Type, traceLogs);

            switch (bCondition.Operator)
            {
                case ColloSysEnums.Operators.GreaterThan:
                    return Expression.GreaterThan(left, right);

                case ColloSysEnums.Operators.GreaterThanEqualTo:
                    return Expression.GreaterThanOrEqual(left, right);

                case ColloSysEnums.Operators.LessThan:
                    return Expression.LessThan(left, right);

                case ColloSysEnums.Operators.LessThanEqualTo:
                    return Expression.LessThanOrEqual(left, right);

                case ColloSysEnums.Operators.NotEqualTo:
                    return Expression.NotEqual(left, right);

                case ColloSysEnums.Operators.EqualTo:
                    return Expression.Equal(left, right);
                 //TODO:Done please check case
                case ColloSysEnums.Operators.IsIn:
                    return Expression.Call(parameter, typeof(string[]).GetMethod("Contains"), right, left);

                //case ColloSysEnums.Operators.StartsWith:
                //case ColloSysEnums.Operators.EndsWith:
                //case ColloSysEnums.Operators.Contains:
                //case ColloSysEnums.Operators.Like:
                default:
                    throw new ArgumentOutOfRangeException("operators");
            }
        }

        private static Expression GetLeftExpression<T>(ParameterExpression parameter,BillDetail billDetail, BCondition bCondition, List<T> data, TraceLogs traceLogs)
        {
            switch (bCondition.Ltype)
            {
                case ColloSysEnums.PayoutLRType.None:
                    return null;
                case ColloSysEnums.PayoutLRType.Value:
                    return null;
                case ColloSysEnums.PayoutLRType.Table:
                case ColloSysEnums.PayoutLRType.Column:
                    if (bCondition.LtypeName.IndexOf('.') < 0)
                        return Expression.Property(parameter, bCondition.LtypeName);

                    return PropertyOfProperty(parameter, bCondition.LtypeName);
                case ColloSysEnums.PayoutLRType.Formula:
                    var value = FormulaBuilder.SolveFormula<T>(billDetail, bCondition.LtypeName, data, traceLogs);
                    traceLogs.AddFormula(bCondition.LtypeName, value);
                    var log = string.Format("Product : {0},Formula : {1} give value : {2}", billDetail.Products,
                                            bCondition.LtypeName, value);
                    traceLogs.SetLog(log);
                    Logger.Info(log);

                    return (value.GetType() == typeof(Expression)) ? value : Expression.Constant(value);
                case ColloSysEnums.PayoutLRType.Matrix:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Expression GetRightExpression(ParameterExpression parameter, BCondition bCondition, Type rightType, TraceLogs traceLogs)
        {
            switch (bCondition.Rtype)
            {
                case ColloSysEnums.PayoutLRType.None:
                    return null;
                case ColloSysEnums.PayoutLRType.Value:
                    var value = ConvertToType(bCondition.Rvalue, rightType);
                    return Expression.Constant(value);
                case ColloSysEnums.PayoutLRType.Table:
                case ColloSysEnums.PayoutLRType.Column:
                    return Expression.Property(parameter, bCondition.RtypeName);
                case ColloSysEnums.PayoutLRType.Formula:
                    return null;
                case ColloSysEnums.PayoutLRType.Matrix:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region select output expression

        public static decimal GetOutputExpression<T>(BillDetail billDetail, List<BCondition> bConditions, List<T> data, TraceLogs traceLogs)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            decimal ouput = 0;
            for (var i = 0; i < bConditions.Count; i++)
            {
                var bCondition = bConditions[i];
                ouput = MathOperation(bCondition.Operator, ouput,
                                      GetOutputExpression(parameter, billDetail, bCondition, data, traceLogs));
            }

            return ouput;
        }

        private static decimal MathOperation(ColloSysEnums.Operators operater, decimal decimal1, decimal decimal2)
        {
            switch (operater)
            {
                case ColloSysEnums.Operators.None:
                case ColloSysEnums.Operators.Plus:
                    return decimal1 + decimal2;
                case ColloSysEnums.Operators.Minus:
                    return decimal1 - decimal2;
                case ColloSysEnums.Operators.Multiply:
                    return decimal1 * decimal2;
                case ColloSysEnums.Operators.Divide:
                    return (decimal2 == 0) ? 0 : decimal1 / decimal2;
                case ColloSysEnums.Operators.ModuloDivide:
                    return (decimal2 == 0) ? 0 : decimal1 % decimal2;
                default:
                    throw new ArgumentOutOfRangeException("operater");
            }
        }

        private static decimal GetOutputExpression<T>(ParameterExpression parameter, BillDetail billDetail, BCondition bCondition, List<T> data, TraceLogs traceLogs)
        {
            dynamic right = GetRightOutputExpression<T>(parameter, billDetail, bCondition, typeof(decimal), traceLogs, data);

            if (right.GetType() == typeof(decimal))
            {
                return right;
            }

            var expression = Expression.Lambda<Func<T, decimal>>(right, parameter);

            Func<T, decimal> selectExpression = expression.Compile();

            var test = data.Sum(selectExpression);
            switch (bCondition.Lsqlfunction)
            {
                case ColloSysEnums.Lsqlfunction.Sum:
                    return data.Sum(selectExpression);
                case ColloSysEnums.Lsqlfunction.Max:
                    return data.Max(selectExpression);
                case ColloSysEnums.Lsqlfunction.Min:
                    return data.Min(selectExpression);
                case ColloSysEnums.Lsqlfunction.Count:
                    return data.Count();
                case ColloSysEnums.Lsqlfunction.Average:
                    return data.Average(selectExpression);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static dynamic GetRightOutputExpression<T>(ParameterExpression parameter, BillDetail billDetail, BCondition bCondition, Type rightType, TraceLogs traceLogs, List<T> data = null)
        {
            switch (bCondition.Rtype)
            {
                case ColloSysEnums.PayoutLRType.None:
                    return null;
                case ColloSysEnums.PayoutLRType.Value:
                    var value = ConvertToType(bCondition.Rvalue, rightType);
                    return value;
                case ColloSysEnums.PayoutLRType.Table:
                case ColloSysEnums.PayoutLRType.Column:
                    if (bCondition.RtypeName.IndexOf('.') < 0)
                        return Expression.Property(parameter, bCondition.RtypeName);

                    return PropertyOfProperty(parameter, bCondition.RtypeName);
                case ColloSysEnums.PayoutLRType.Formula:
                    var formulaValue = FormulaBuilder.SolveFormula<T>(billDetail, bCondition.RtypeName, data, traceLogs);
                    traceLogs.AddFormula(bCondition.RtypeName, formulaValue);
                    var log = string.Format("Product : {0}, Formula : {1} give value : {2}", billDetail.Products,
                                            bCondition.RtypeName, formulaValue);
                    traceLogs.SetLog(log);
                    Logger.Info(log);
                    return formulaValue;
                case ColloSysEnums.PayoutLRType.Matrix:
                    var matrixValue = MatrixCalulater.CalculateMatrix<T>(billDetail, bCondition.RtypeName, data, traceLogs);

                    traceLogs.AddMatrixValue(bCondition.RtypeName,matrixValue);
                    var log2 = string.Format("Product : {0}, Matrix : {1} give value : {2}", billDetail.Products,
                                            bCondition.RtypeName, matrixValue);
                    traceLogs.SetLog(log2);
                    Logger.Info(log2);
                    return matrixValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Group by Object

        public static List<object> GetGroupByObjects<T>(string gropByColumn, List<T> data)
        {
            if(string.IsNullOrWhiteSpace(gropByColumn))
                return new List<object>();

            var parameter = Expression.Parameter(typeof(T), "x");

            var expression = PropertyOfProperty(parameter, gropByColumn);

            return data.Select(Expression.Lambda<Func<T, object>>(expression, parameter).Compile()).Distinct().ToList();
        }

        #endregion

        #region Helper

        private static
        dynamic ConvertToType(string value, Type type)
        {
            if (type.IsEnum)
            {
                return Enum.Parse(type, value);
            }

            if (type == typeof(decimal))
            {
                return Convert.ToDecimal(value);
            }

            if (type == typeof(int))
            {
                return Convert.ToInt32(value);
            }

            if (type == typeof(uint))
            {
                return Convert.ToUInt32(value);
            }

            if (type == typeof(DateTime))
            {
                return Convert.ToDateTime(value);
            }

            return value;
        }

        private static Expression PropertyOfProperty(Expression expr, string propertyName)
        {
            return propertyName
                       .Split('.')
                       .Aggregate<string, Expression>(
                          expr,
                           (current, property) =>
                               Expression.Property(
                                   current,
                                   GetProperty(current.Type, property)));
        }

        private static PropertyInfo GetProperty(Type type, string propertyName)
        {
            PropertyInfo prop = type.GetProperty(propertyName);
            if (prop == null)
            {
                var baseTypesAndInterfaces = new List<Type>();
                if (type.BaseType != null) baseTypesAndInterfaces.Add(type.BaseType);
                baseTypesAndInterfaces.AddRange(type.GetInterfaces());
                foreach (Type t in baseTypesAndInterfaces)
                {
                    prop = GetProperty(t, propertyName);
                    if (prop != null)
                        break;
                }
            }
            return prop;
        }
        #endregion
    }
}
