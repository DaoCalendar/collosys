using System;
using System.Globalization;

namespace FileUploader.ValueSetters
{
    public class Int16Setter:NumberSetter<Int16>
    {
        protected override short SetValue(string s, string format = "")
        {
            var provider = new CultureInfo("en-US");
            return Int16.Parse(s, NumberStyles.Any,provider);
        }
    }
}
