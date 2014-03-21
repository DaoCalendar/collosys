using System;
using System.Linq.Expressions;
using System.Reflection;
using FileUploader.ValueSetters;

namespace FileUploader.Tests.ValueSetters.Tests
{
    public abstract class AssignTypedTests<T>
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly FieldInfo _fieldInfo;
        protected readonly TypedTestHelper<T> TypeTestHelper;
        protected IValueSetter ValueSetter;

        protected AssignTypedTests()
        {
            TypeTestHelper = new TypedTestHelper<T>();

            var propertyName = GetPropertyName<TypedTestHelper<T>>(x => x.Property);
            _propertyInfo = TypeTestHelper.GetType().GetProperty(propertyName);
            var fieldName = GetPropertyName<TypedTestHelper<T>>(x => x.Field);
            _fieldInfo = TypeTestHelper.GetType().GetField(fieldName);
        }

        public  void SetPropertyValue(string value, string format = "")
        {
            ValueSetter.SetValue(_propertyInfo, TypeTestHelper, value, format);
        }

        protected void SetFieldValue(string value, string format = "")
        {
            ValueSetter.SetValue(_fieldInfo, TypeTestHelper, value, format);
        }

        protected class TypedTestHelper<TU> where TU : T
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Global
            public TU Property { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Global

            // ReSharper disable UnassignedField.Global
            public TU Field;
            // ReSharper restore UnassignedField.Global
        }

        private static string GetPropertyName<TS>(Expression<Func<TS, T>> expression)
        {
            var memberExpretion = expression.Body as MemberExpression;
            return memberExpretion == null ? null : memberExpretion.Member.Name;
        }
    }
}