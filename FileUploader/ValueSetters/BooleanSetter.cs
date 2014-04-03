using System;

namespace FileUploader.ValueSetters
{
   public class BooleanSetter : ValueSetter<bool>
    {
        protected override bool GetValue(string s,string format="")
        {
            var value = (s == null ? null : s.Trim());
            try
            {
                if (value=="0" || value=="1" )
                {
                    return Convert.ToBoolean(Convert.ToInt32(value));
                }
                return Convert.ToBoolean(value);
            }
            catch (Exception e)
            {

                throw new Exception(string.Format("Value {0} is not a valid Boolean Value.", value), e);
            }

        }
        }
    }

