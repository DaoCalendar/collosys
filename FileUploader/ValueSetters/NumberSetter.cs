using System;

namespace FileUploader.ValueSetters
{
    public abstract class NumberSetter<T> : ValueSetter<T>
    {
        protected override T GetValue(string s, string format = "")
        {
            var value = (s == null ? null : s.Trim());
            try
            {
                return SetValue(s, format);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Value {0} is not a valid number.", value), e);
            }
        }

        protected abstract T SetValue(string s, string format = "");
    }
}
