using System;
using System.Reflection;

namespace FileUploader.ValueSetters
{
    public interface IValueSetter
    {
        void SetValue(PropertyInfo propertyInfo, Object helper, string s,string format="");
        void SetValue(FieldInfo fieldInfo, Object helper, string s,string format="");
    }
}