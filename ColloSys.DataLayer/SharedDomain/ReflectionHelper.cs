using System;
using System.Linq.Expressions;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.SharedDomain
{
    internal static class ReflectionHelper
    {
        public static string GetMemberName<T>(Expression<Func<T, object>> expression)
        {
            var expressionBody = expression.Body;

            if (expressionBody == null)
            {
                throw new ArgumentException(
                    "The expression cannot be null.");
            }

            var body = expressionBody as UnaryExpression;
            if (body != null)
            {
                // Reference type property or field
                var unaryExpression = body;

                var operand = unaryExpression.Operand as MethodCallExpression;
                if (operand != null)
                {
                    var methodExpression =
                        operand;
                    return methodExpression.Method.Name;
                }

                return ((MemberExpression)unaryExpression.Operand)
                    .Member.Name;
            }

            throw new ArgumentException("Invalid expression");
        }

        public static string GetMemberName<T>(this Entity obj, Expression<Func<T, object>> expression)
            where T : Entity
        {
            var expressionBody = expression.Body;

            if (expressionBody == null)
            {
                throw new ArgumentException(
                    "The expression cannot be null.");
            }

            var body = expressionBody as UnaryExpression;
            if (body != null)
            {
                // Reference type property or field
                var unaryExpression = body;

                var operand = unaryExpression.Operand as MethodCallExpression;
                if (operand != null)
                {
                    var methodExpression =
                        operand;
                    return methodExpression.Method.Name;
                }

                return ((MemberExpression)unaryExpression.Operand)
                    .Member.Name;
            }

            throw new ArgumentException("Invalid expression");
        }

    }

    public class MemberHelper<T>
    {
        public string GetName<TU>(Expression<Func<T, TU>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression != null)
                return memberExpression.Member.Name;

            throw new InvalidOperationException("Member expression expected");
        }
    }

}
