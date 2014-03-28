using System;
using System.Globalization;

namespace FileUploader.ValueSetters
{
    public class DoubleSetter :NumberSetter<double>
    {
        protected override double SetValue(string s, string format = "")
        {
             var provider = new CultureInfo("en-US");
            return Double.Parse(s,NumberStyles.Any,provider);
        }
    }
}
