using System;
using System.Globalization;

namespace FileUploader.ValueSetters
{
    public class Int64Setters :NumberSetter<Int64>
    {
        protected override long SetValue(string s, string format = "")
        {
            var provider = new CultureInfo("en-US");
            return Int64.Parse(s, NumberStyles.Any,provider);
        }
    }
}
