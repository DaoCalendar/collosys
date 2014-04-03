using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using NUnit.Framework;

namespace ReflectionExtension.Tests.FileUploader.Test
{
    [TestFixture]
    internal class ColumnMappingTest
    {
        private IList<FileMapping> _data;
        [SetUp]
        public void Init()
        {
            _data = SessionManager.GetCurrentSession().QueryOver<FileMapping>().List<FileMapping>();
        }

        [Test]
        public void Test_ActualColumn_IsNotEmpty()
        {
            var actual = from d in _data select (d.ActualColumn);
            foreach (var info in actual)
            {
                Assert.IsNotEmpty(info);
            }
         
        }
        [Test]
        public void Test_ActualProduct_IsNotEmpty()
        {
            var actual = from d in _data select (d.ActualTable);
            foreach (var info in actual)
            {
                Assert.IsNotEmpty(info);
            }

        }

        [Test]
        public void Test_Cloumn_ValueType_DefaultValue_With_DefaultValue()
        {
           var defaultValue = from d in _data where (d.ValueType==ColloSysEnums.FileMappingValueType.DefaultValue) select (d.DefaultValue);
            foreach (var info in defaultValue)
            {
                Assert.IsNotNull(info);

            }
        }

        [Test]
        public void Test_Cloumn_ValueType_ExcelValue_With_Position()//if valueType is ExcelValue then Position shouldn't be 0
        {
           var position = from d in _data where (d.ValueType==ColloSysEnums.FileMappingValueType.ExcelValue) select (d.Position);

            foreach (var info in position)
            {
                Assert.AreNotEqual(info,0);
            }
        }

        [Test]
        public void Test_Cloumn_ValueType_CoumputedValue_With_Position()//if valueType is computedvalue then Position should be 0
        {
            var computedValue = from d in _data where (d.ValueType == ColloSysEnums.FileMappingValueType.ComputedValue) select (d.Position);

            foreach (var info in computedValue)
            {
                Assert.AreEqual(info, 0);
            }
        }

    }
}
