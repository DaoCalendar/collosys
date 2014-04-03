using System.Runtime.InteropServices;
using ColloSys.FileUploader.Reflection;
using NUnit.Framework;
using FileUploader.ExcelReader;

namespace FileUploader.Tests.Reflection
{
    [TestFixture]
    public class ReflectionPropertyNameTest
    {
        #region helper classes

        public class ReflectionTestHelperClass
        {
            public ReflectionTestHelperClass()
            {
                ReflectionTestHelperClass1 = new ReflectionTestHelperClass1();
            }

            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public string Name { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local

            // ReSharper disable once UnassignedField.Compiler
            public string Field;

            // ReSharper disable MemberCanBePrivate.Local
            public ReflectionTestHelperClass1 ReflectionTestHelperClass1 { get; set; }
            // ReSharper restore MemberCanBePrivate.Local
        }

        public class ReflectionTestHelperClass1
        {
            public ReflectionTestHelperClass1()
            {
                ReflectionTestHelperClass2 = new ReflectionTestHelperClass2();
            }

            public string Property { get; set; }
            public ReflectionTestHelperClass2 ReflectionTestHelperClass2 { get; set; }
        }

        public class ReflectionTestHelperClass2
        {
            public string Innerprop { get; set; }
            public string InnerField;
        }

        #endregion

        [Test]
        public void Compare_PropertyName()
        {
            var propertyName = ReflectionHelper.GetMemberName<ReflectionTestHelperClass>(x => x.Name);
            Assert.AreEqual(propertyName, "Name");
        }

        [Test]
        public void Compare_FieldName()
        {
            var propertyName = ReflectionHelper.GetMemberName<ReflectionTestHelperClass>(x => x.Field);
            Assert.AreEqual(propertyName, "Field");
        }
        
        [Test]
        public void Compare_PropertyName_Multiple(string propName,string exp)
        {
           var propertyName = ReflectionHelper.GetMemberName<ExcelReaderHelper>(x => x.Property1);
           var propertyName2 = ReflectionHelper.GetMemberName<ExcelReaderHelper>(x => x.Property2);
           Assert.AreEqual(propertyName2, "Property2");
        }

        [Test]
        public void Compare_Nested_PropertyName()
        {
            var propertyName = ReflectionHelper.GetMemberName<ReflectionTestHelperClass>(x => x.ReflectionTestHelperClass1.Property);
            Assert.AreEqual(propertyName, "ReflectionTestHelperClass1.Property");
        }

        [Test]
        public void Compare_Nested_FieldName()
        {
            var propertyName = ReflectionHelper.GetMemberName<ReflectionTestHelperClass>(x => x.ReflectionTestHelperClass1.ReflectionTestHelperClass2.InnerField);
            Assert.AreEqual(propertyName, "ReflectionTestHelperClass1.ReflectionTestHelperClass2.InnerField");
        }

       

    }
}
