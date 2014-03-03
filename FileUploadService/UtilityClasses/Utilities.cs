#region

using System;
using System.Globalization;

#endregion

namespace ColloSys.FileUploadService.UtilityClasses
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
            return DateTime.ParseExact(value, format, null, DateTimeStyles.None);
        }

        public static DateTime? ParseNullableDateTime(string value, string format)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            try
            {
                return DateTime.ParseExact(value, format, null, DateTimeStyles.None);
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
    }
}
