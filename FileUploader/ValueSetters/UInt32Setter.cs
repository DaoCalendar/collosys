using System;
using System.Globalization;

namespace FileUploader.ValueSetters
{
    public class UInt32Setter : NumberSetter<UInt32>
    {
        protected override uint SetValue(string s, string format = "")
        {
            var provider = new CultureInfo("en-US");
            return UInt32.Parse(s, NumberStyles.Any,provider);
        }
    }
}
