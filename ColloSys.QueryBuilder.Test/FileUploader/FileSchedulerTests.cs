using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
