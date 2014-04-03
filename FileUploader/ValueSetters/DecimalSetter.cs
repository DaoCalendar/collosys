using System;
using System.Globalization;

namespace FileUploader.ValueSetters
{
   public class DecimalSetter:NumberSetter<Decimal>
   {
       protected override decimal SetValue(string s, string format = "")
        {
            var provider = new CultureInfo("en-US");
           return Decimal.Parse(s, NumberStyles.Any, provider);
        }

      
    }
}
