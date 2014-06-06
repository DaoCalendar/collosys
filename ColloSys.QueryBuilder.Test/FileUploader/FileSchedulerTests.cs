using ColloSys.FileUploadService;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.FileUploader
{
     [TestFixture]
    public class FileSchedulerTests
    {
         [Test]
        public void UploadFiles()
        
         {
            FileUploaderService.UploadFiles();
        }
    }
}
