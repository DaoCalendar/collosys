using NUnit.Framework;

namespace ReflectionExtension.Tests.FileUploadServiceV2Test
{
    [TestFixture]
    public class FileUploadServiceV2Test
    {
        [Test]
        public void UploadFile()
        {
            FileUploaderService.FileUploaderService.UploadFiles();
        }
    }
}
