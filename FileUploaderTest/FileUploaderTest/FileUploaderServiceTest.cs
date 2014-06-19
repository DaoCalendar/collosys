using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService;
using NUnit.Framework;

namespace ReflectionExtension.Tests.FileUploaderTest
{
    [TestFixture]
    public class FileUploaderServiceTest
    {
        [Test]
        public void Test()
        {
            FileUploaderService1.UploadFiles();
        }

        [Test]
        public void Check_Property_Type()
        {
            var rliner = new RLiner();
            var propertyInfo = rliner.GetType().GetProperty("FileDate");
        }
    }
}
