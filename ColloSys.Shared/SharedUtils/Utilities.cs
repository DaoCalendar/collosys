#region

using System;
using System.Collections.Generic;
using System.Globalization;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.Shared.SharedUtils
{
    public static class Utilities
    {
        #region substring

        public static string Substring2(this string inputLine, uint startIndex, uint length = 0)
        {
            if (String.IsNullOrWhiteSpace(inputLine))
                return String.Empty;

            if (inputLine.Length < startIndex)
                return String.Empty;
            if (length == 0)
                length = (uint)inputLine.Length - startIndex;


            if (inputLine.Length <= startIndex + length)
                return inputLine.Substring((int)startIndex).Trim();


            return inputLine.Substring((int)startIndex, (int)length).Trim();
        }

        #endregion

        #region date time parse

        public static DateTime ParseDateTime(string value, string format)
        {
            return DateTime.ParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        public static DateTime? ParseNullableDateTime(string value, string format)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            try
            {
                return DateTime.ParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region toDecimal

        public static decimal ConvertToDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            return ((string.IsNullOrWhiteSpace(Convert.ToString(value)) || (value == "-"))
                        ? 0
                        : Convert.ToDecimal(value));
        }

        #endregion

        #region long

        public static bool IsNumberLong(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            long number;
            if (long.TryParse(value, out number))
            {
                return true;
            }

            return false;
        }

        public static long Convert2Long(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            long number;
            if (long.TryParse(value, out number))
            {
                return number;
            }

            return 0;
        }

        #endregion

        public static ColloSysEnums.BasicValueTypes GetBaseValueType(string name, Type type)
        {
            // nullable to underlying
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);
            }

            // string
            if (type == typeof(string))
            {
                return ColloSysEnums.BasicValueTypes.Text;
            }

            // non-basic type, cannot map
            if (!type.IsValueType)
            {
                return ColloSysEnums.BasicValueTypes.Unknown;
            }

            if (type == typeof(bool))
            {
                return ColloSysEnums.BasicValueTypes.Bool;
            }

            // number
            IList<Type> numberType = new List<Type>
                {
                    typeof (byte),
                    typeof (sbyte),
                    typeof (short),
                    typeof (ushort),
                    typeof (int),
                    typeof (uint),
                    typeof (long),
                    typeof (ulong)
                };
            if (numberType.Contains(type))
            {
                return ColloSysEnums.BasicValueTypes.Number;
            }

            // number with precision
            IList<Type> precisionType = new List<Type>
                {
                    typeof (float),
                    typeof (double),
                    typeof (decimal),
                };
            if (precisionType.Contains(type))
            {
                return ColloSysEnums.BasicValueTypes.NumberWithPrecision;
            }

            // datetime
            if (type == typeof(DateTime))
            {
                return name.ToUpperInvariant().EndsWith("DATETIME")
                           ? ColloSysEnums.BasicValueTypes.DateTime
                           : ColloSysEnums.BasicValueTypes.Date;
            }

            if (type == typeof(Guid) || type.IsEnum)
            {
                return ColloSysEnums.BasicValueTypes.Text;
            }

            return ColloSysEnums.BasicValueTypes.Unknown;
        }

    }
}
