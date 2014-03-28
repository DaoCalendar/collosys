using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
