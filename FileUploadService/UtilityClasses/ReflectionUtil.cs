#region references

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.FileUploadService.BaseReader;
using ColloSys.FileUploadService.UtilityClasses;

#endregion


namespace ColloSys.FileUploadService.Reader
{
    public static class ReflectionUtil
    {
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

        public static void SetValueWithType(this PropertyInfo prop, Object record, string value, string dateFormat = null)
        {
            value = value.Trim();
            if (!string.IsNullOrWhiteSpace(value) && value == "-")
                value = string.Empty;

            if (!string.IsNullOrWhiteSpace(value) && value[0] == '(' && value[value.Length - 1] == ')')
                value = "-" + value.Substring2(1, Convert.ToUInt32(value.Length - 2));

            var underlyingType = prop.PropertyType;
            if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    prop.SetValue(record, null);
                    return;
                }

                var nullableConverter = new NullableConverter(prop.PropertyType);
                underlyingType = nullableConverter.UnderlyingType;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            if (underlyingType.IsEnum)
            {
                prop.SetValue(record, Enum.Parse(underlyingType, value));
                return;
            }

            if (underlyingType == typeof(DateTime))
            {
                try
                {
                    prop.SetValue(record, DateTime.FromOADate(Convert.ToDouble(value)));
                }
                catch (Exception)
                {
                    if (string.IsNullOrWhiteSpace(dateFormat))
                        throw new InvalidCastException("format is not specify for date in file column.");

                    var dateFormats = dateFormat.Split(',');
                    var success = false;
                    foreach (var format in dateFormats)
                    {
                        DateTime dateTime;
                        success = DateTime.TryParseExact(value,
                            format.Trim(), CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out  dateTime);

                        if (!success) continue;

                        prop.SetValue(record, dateTime);
                        break;
                    }

                    if (!success)
                        throw new InvalidCastException("Date is not in valid format");
                }

                //if (dateFormat == UploaderConstants.OADATE)
                //{
                //    prop.SetValue(record, DateTime.FromOADate(Convert.ToDouble(value)));
                //    return;
                //}

                //prop.SetValue(record, DateTime.ParseExact(value, dateFormat, CultureInfo.InvariantCulture));
                return;
            }

            prop.SetValue(record, Convert.ChangeType(value, underlyingType));
        }
    }
}