#region references

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using ColloSys.DataLayer.BaseEntity;

#endregion

namespace ColloSys.Shared.SharedUtils
{
    public static class ReflectionUtil
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

        public static void SetValueWithType(this PropertyInfo prop, Object record, string value, string dateFormat = null)
        {
            value = value.Trim();
            if (!String.IsNullOrWhiteSpace(value) && value == "-")
                value = String.Empty;

            //if (!string.IsNullOrWhiteSpace(value) && value[0] == '(' && value[value.Length - 1] == ')')
            //    value = "-" + value.Substring2(1, Convert.ToUInt32(value.Length - 2));

            var underlyingType = prop.PropertyType;
            if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    prop.SetValue(record, null);
                    return;
                }

                var nullableConverter = new NullableConverter(prop.PropertyType);
                underlyingType = nullableConverter.UnderlyingType;
            }

            if (String.IsNullOrWhiteSpace(value))
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
                    if (String.IsNullOrWhiteSpace(dateFormat))
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

                return;
            }

            if (underlyingType == typeof(decimal))
            {
                decimal decimalValue;
                if (Decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimalValue))
                {
                    prop.SetValue(record, Math.Round(decimalValue, 2));
                    return;
                }

                throw new InvalidCastException("Number is not in valid format");
            }

            if (underlyingType == typeof(long))
            {
                long longValue;
                if (Int64.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out longValue))
                {
                    prop.SetValue(record, longValue);
                    return;
                }

                throw new InvalidCastException("Number is not in valid format");
            }

            if (underlyingType == typeof(ulong))
            {
                ulong ulongValue;
                if (UInt64.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out ulongValue))
                {
                    prop.SetValue(record, ulongValue);
                    return;
                }

                throw new InvalidCastException("Number is not in valid format");
            }

            if (underlyingType == typeof(int))
            {
                int intValue;
                if (Int32.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out intValue))
                {
                    prop.SetValue(record, intValue);
                    return;
                }

                throw new InvalidCastException("Number is not in valid format");
            }

            if (underlyingType == typeof(uint))
            {
                uint uintValue;
                if (UInt32.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out uintValue))
                {
                    prop.SetValue(record, uintValue);
                    return;
                }

                throw new InvalidCastException("Number is not in valid format");
            }

            if (underlyingType == typeof(Int16))
            {
                Int16 int16Value;
                if (Int16.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out int16Value))
                {
                    prop.SetValue(record, int16Value);
                    return;
                }

                throw new InvalidCastException("Number is not in valid format");
            }

            if (underlyingType == typeof(UInt16))
            {
                UInt16 uint16Value;
                if (UInt16.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out uint16Value))
                {
                    prop.SetValue(record, uint16Value);
                    return;
                }

                throw new InvalidCastException("Number is not in valid format");
            }

            prop.SetValue(record, Convert.ChangeType(value, underlyingType));
        }

        public static object GetPropertyValue(object obj, string fieldName)
        {
            var childValue = obj;
            try
            {
                foreach (var part in fieldName.Split('.'))
                {
                    childValue = childValue.GetType().GetProperty(part).GetValue(childValue);
                }
            }
            catch (Exception)
            {
                childValue = null;
            }
            return childValue;
        }

        public static Type GetPropertyType(Type obj, string fieldName)
        {
            var childType = obj;
            try
            {
                foreach (var part in fieldName.Split('.'))
                {
                    childType = childType.GetProperty(part).PropertyType;
                }
            }
            catch (Exception)
            {
                childType = null;
            }
            return childType;
        }

        public static T CloneObject<T>(T objSource) where T:new ()
        {
            //step : 1 Get the type of source object and create a new instance of that type
            Type typeSource = objSource.GetType();
            object objTarget = Activator.CreateInstance(typeSource);

            //Step2 : Get all the properties of source object type
            PropertyInfo[] propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            //Step : 3 Assign all source property to taget object 's properties
            foreach (PropertyInfo property in propertyInfo)
            {
                //Check whether property can be written to
                if (property.CanWrite)
                {
                    //Step : 4 check whether property type is value type, enum or string type
                    if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType.Equals(typeof(System.String)))
                    {
                        property.SetValue(objTarget, property.GetValue(objSource, null), null);
                    }
                    //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
                    //else
                    //{
                    //    object objPropertyValue = property.GetValue(objSource, null);
                    //    if (objPropertyValue == null)
                    //    {
                    //        property.SetValue(objTarget, null, null);
                    //    }
                    //    else
                    //    {
                    //        property.SetValue(objTarget, CloneObject(objPropertyValue), null);
                    //    }
                    //}
                }
            }
            return (T) objTarget;
        }
    }
}