using System;
using System.Linq.Expressions;
using System.Reflection;
using ColloSys.FileUploader.ValueSetters;
using FileUploader.ValueSetters;

namespace ColloSys.FileUploader.Reflection
{
    public class ReflectionHelper
    {
        public static string GetMemberName<TS>(Expression<Func<TS, object>> expression)
        {
            var memberExpretion = expression.Body as MemberExpression ??
                                  ((UnaryExpression)expression.Body).Operand as MemberExpression;
            return GetExpresionBody(memberExpretion);
        }

        public static object GetValue<T>(T helper, Expression<Func<T, object>> expression)
        {
            return GetValue(helper, expression.Body);
        }

        private static object GetValue<T>(T helper, Expression expression)
        {
            var memberExpression = expression as MemberExpression;

            if (memberExpression == null)
                return helper;

            if (memberExpression.NodeType == ExpressionType.Parameter)
                return string.Empty;

            var newHelper = GetValue(helper, memberExpression.Expression);

            if (memberExpression.Member.MemberType == MemberTypes.Property)
            {
                var prop = newHelper.GetType().GetProperty(memberExpression.Member.Name);
                return prop.GetValue(newHelper);
            }
            else
            {
                var prop = newHelper.GetType().GetField(memberExpression.Member.Name);
                return prop.GetValue(newHelper);
            }
        }

        private static string GetExpresionBody(Expression expression)
        {
            var memberExpression = expression as MemberExpression;

            if (memberExpression == null)
                return string.Empty;

            if (memberExpression.NodeType == ExpressionType.Parameter)
                return string.Empty;

            var classValue = GetExpresionBody(memberExpression.Expression);
            var result = classValue + (string.IsNullOrWhiteSpace(classValue) ? "" : ".") + memberExpression.Member.Name;

            return result;
        }

        public static void SetValue(string propName, string value, object obj)
        {
            var propertyInfo = obj.GetType().GetProperty(propName);
            if (propertyInfo.PropertyType == typeof(string))
            {
                var stringHelper = new StringSetter();
                stringHelper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(int))
            {
                var int32Hepler = new Int32Setter();
                int32Hepler.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(int?))
            {
                if (string.IsNullOrEmpty(value)) return;
                var int32Hepler = new Int32Setter();
                int32Hepler.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(Int16))
            {
                var int32Hepler = new Int16Setter();
                int32Hepler.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(Int16?))
            {
                if (string.IsNullOrEmpty(value)) return;
                var int32Hepler = new Int16Setter();
                int32Hepler.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(DateTime?))
            {
                if (string.IsNullOrEmpty(value)) return;
                var dateTimeHelper = new DateTimeSetter();
                dateTimeHelper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(DateTime))
            {
                if (string.IsNullOrEmpty(value)) return;
                var dateTimeHelper = new DateTimeSetter();
                dateTimeHelper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(double))
            {
                var doubleHelper = new DoubleSetter();
                doubleHelper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(double?))
            {
                if (string.IsNullOrEmpty(value)) return;
                var doubleHelper = new DoubleSetter();
                doubleHelper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(float))
            {
                var floatHelper = new FloatSetter();
                floatHelper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(float?))
            {
                if (string.IsNullOrEmpty(value)) return;
                var floatHelper = new FloatSetter();
                floatHelper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(Int64))
            {
                var int64Helper = new Int64Setters();
                int64Helper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(Int64?))
            {
                if (string.IsNullOrEmpty(value)) return;
                var int64Helper = new Int64Setters();
                int64Helper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(UInt64))
            {
                var uInt64Helper = new UInt64Setter();
                uInt64Helper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(UInt64?))
            {
                if (string.IsNullOrEmpty(value)) return;
                var uInt64Helper = new UInt64Setter();
                uInt64Helper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(UInt32))
            {
                var uInt32Helper = new UInt32Setter();
                uInt32Helper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(UInt32?))
            {
                if (string.IsNullOrEmpty(value)) return;
                var uInt32Helper = new UInt32Setter();
                uInt32Helper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(decimal))
            {
                if (string.IsNullOrEmpty(value)) return;
                var decimalHelper = new DecimalSetter();
                decimalHelper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(decimal?))
            {
                if (string.IsNullOrEmpty(value)) return;
                var decimalHelper = new DecimalSetter();
                decimalHelper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(byte))
            {
                var byteHelper = new ByteSetter();
                byteHelper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(byte?))
            {
                if (string.IsNullOrEmpty(value)) return;
                var byteHelper = new ByteSetter();
                byteHelper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(sbyte))
            {
                var sbyteHelper = new SbyteSetter();
                sbyteHelper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(sbyte?))
            {
                if (string.IsNullOrEmpty(value)) return;
                var sbyteHelper = new SbyteSetter();
                sbyteHelper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType == typeof(char))
            {
                var charHelper = new CharSetter();
                charHelper.SetValue(propertyInfo, obj, value);
            }
            else if (propertyInfo.PropertyType.IsEnum)
            {
                if (value != null)
                {
                    var enumType = new EnumSetter();
                    enumType.SetValue(propertyInfo, obj, value);
                }

            }
        }
    }
}
