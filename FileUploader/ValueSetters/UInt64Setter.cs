using System;
using System.Globalization;

namespace FileUploader.ValueSetters
{
   public class UInt64Setter:NumberSetter<UInt64>
   {
        
       protected override ulong SetValue(string s, string format = "")
       {
           var provider = new CultureInfo("en-US");
           return UInt64.Parse(s, NumberStyles.Any,provider);
       }
   }
}
