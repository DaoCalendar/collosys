using System;
using System.Globalization;

namespace FileUploader.ValueSetters
{
    public class ByteSetter:NumberSetter<byte>
    {
        protected override byte SetValue(string s, string format = "")
        {
            var provider = new CultureInfo("en-US");
            return Byte.Parse(s, NumberStyles.Any, provider);
        }
    }
}
