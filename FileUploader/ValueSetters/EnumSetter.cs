using System;
using System.Reflection;
using FileUploader.ValueSetters;

namespace ColloSys.FileUploader.ValueSetters
{
    public class EnumSetter : IValueSetter
    {
        public void SetValue(PropertyInfo propertyInfo, object helper, string s, string format = "")
        {
            var value = Enum.Parse(propertyInfo.PropertyType, s);
            propertyInfo.SetValue(helper, value);
        }

        public void SetValue(FieldInfo fieldInfo, object helper, string s, string format = "")
        {
            var value = Enum.Parse(fieldInfo.FieldType, s);
            fieldInfo.SetValue(helper, value);
        }
    }
}
