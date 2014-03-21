using System;
using System.Globalization;
using System.Reflection;

namespace FileUploader.ValueSetters
{
    public class DateTimeSetter : ValueSetter<DateTime>
    {
        protected override DateTime GetValue(string s, string format = "")
        {
            var value = (s == null ? null : s.Trim());
            var indtInfo = new CultureInfo("hi-IN", false).DateTimeFormat;
            const DateTimeStyles styles = DateTimeStyles.AssumeLocal;
            try
            {
                if (format != "")
                {
                    return DateTime.ParseExact(value, format, indtInfo);
                }
                return DateTime.Parse(value, indtInfo, styles);
              
            }
           
                catch (Exception e)
                {
                    try
                    {
                        if (value != null)
                        {
                            double oaDateTime = Convert.ToDouble(value);
                            return DateTime.FromOADate(oaDateTime);
                        }
                        return DateTime.Parse(null, indtInfo, styles);
                    }


                    catch (Exception)
                    {
                        throw new Exception(string.Format("Value {0} is not a valid date.", value), e);
                    }
                }
            }
        }

    }

