using System.Reflection;

namespace FileUploader.ValueSetters
{
    public abstract class ValueSetter<T> : IValueSetter
    {

        public void SetValue(PropertyInfo propertyInfo, object helper, string s, string format = "")
        {
            var value = GetValue(s, format);
            propertyInfo.SetValue(helper, value);
        }

        public void SetValue(FieldInfo fieldInfo, object helper, string s, string format = "")
        {
            var value = GetValue(s, format);
            fieldInfo.SetValue(helper, value);
        }
        protected abstract T GetValue(string s, string format = "");
    }
}