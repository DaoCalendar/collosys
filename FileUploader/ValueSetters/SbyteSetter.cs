using System;
using System.Globalization;

namespace FileUploader.ValueSetters
{
   public class SbyteSetter:NumberSetter<sbyte>
   {

       protected override sbyte SetValue(string s, string format = "")
       {
           var provider = new CultureInfo("en-US");
           return SByte.Parse(s, NumberStyles.Any, provider);
       }
   }
}
