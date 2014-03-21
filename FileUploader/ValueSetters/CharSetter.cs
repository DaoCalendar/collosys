using System;

namespace FileUploader.ValueSetters
{
   public class CharSetter:ValueSetter<char>
    {
        protected override char GetValue(string s,string format="")
        {
            var value = (s == null ? null : s.Trim());
            try
            {

                return Convert.ToChar(value);
            }
            catch (Exception e)
            {

                throw new Exception(string.Format("Value {0} is Too Long Or blank .", value), e);
            }
        }
    }
}
