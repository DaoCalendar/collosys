using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using FileUploader.Tests.ValueSetters.Tests;
using NUnit.Framework;

namespace ReflectionExtension.Tests.FileUploader.Test
{
    [TestFixture]
    internal class ColumnMappingTest
    {
        private IList<FileMapping> data;
        [SetUp]
        public void Init()
        {
            data = SessionManager.GetCurrentSession().QueryOver<FileMapping>().List<FileMapping>();
        }

        [Test]
        public void Test_ActualColumn_OR_ActualTable_IsNotEmpty()
        {
            var actual = data.FirstOrDefault().ActualColumn;


        }

    }
}
