using System;
using System.Globalization;

namespace FileUploader.ValueSetters
{
    public class Int32Setter : NumberSetter<Int32>
    {
        protected override int SetValue(string s, string format = "")
        {
            var provider = new CultureInfo("en-US");
            return Int32.Parse(s, NumberStyles.Any, provider);
        }
    }
}