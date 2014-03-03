using System;

namespace ColloSys.UserInterface.Shared
{
    public static class Utilities
    {
        private static readonly string[] SizeSuffixes = {
        "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string ByteSize(long size)
        {
            const string formatTemplate = "{0}{1:0.#} {2}";

            if (size == 0)
            {
                return string.Format(formatTemplate, null, 0, SizeSuffixes[0]);
            }

            var absSize = Math.Abs((double)size);
            var fpPower = Math.Log(absSize, 1000);
            var intPower = (int)fpPower;
            var iUnit = intPower >= SizeSuffixes.Length
                ? SizeSuffixes.Length - 1 : intPower;
            var normSize = absSize / Math.Pow(1000, iUnit);

            return string.Format(
                formatTemplate,
                size < 0 ? "-" : null, normSize, SizeSuffixes[iUnit]);
        }

        public static string GetReadableTimeSpan(TimeSpan value)
        {
            string duration;

            // convert it into string
            if (value.TotalMinutes < 1)
                duration = value.Seconds + " Seconds";
            else if (value.TotalHours < 1)
                duration = value.Minutes + " Minutes, " + value.Seconds + " Seconds";
            else if (value.TotalDays < 1)
                duration = value.Hours + " Hours, " + value.Minutes + " Minutes";
            else
                duration = value.Days + " Days, " + value.Hours + " Hours";

            // fix the string value 's in the end
            if (duration.StartsWith("1 Seconds") || duration.EndsWith(" 1 Seconds"))
                duration = duration.Replace("1 Seconds", "1 Second");

            if (duration.StartsWith("1 Minutes") || duration.EndsWith(" 1 Minutes"))
                duration = duration.Replace("1 Minutes", "1 Minute");

            if (duration.StartsWith("1 Hours") || duration.EndsWith(" 1 Hours"))
                duration = duration.Replace("1 Hours", "1 Hour");

            if (duration.StartsWith("1 Days"))
                duration = duration.Replace("1 Days", "1 Day");

            return duration;
        }
    }
}