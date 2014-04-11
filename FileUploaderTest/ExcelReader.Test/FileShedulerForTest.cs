using ColloSys.FileUploadService.Interfaces;
using NUnit.Framework;
using DbLayer = ColloSys.FileUploadService.Implementers.DbLayer;

namespace ReflectionExtension.Tests.ExcelReader.Test
{
    [TestFixture]
    class FileShedulerForTest
    {
           IDBLayer dbLayer = new DbLayer();
         
        [SetUp]
        public void Init()
        {
          
        }

        [Test]
        public void Test()
        {

        }
    }
}
