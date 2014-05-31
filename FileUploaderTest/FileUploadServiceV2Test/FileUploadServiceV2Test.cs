using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests.FileUploadServiceV2Test
{
    [TestFixture]
    public class FileUploadServiceV2Test
    {
        readonly FileMappingData _data = new FileMappingData();
        [SetUp]
        public void Init()
        {

        }

        [Test]
        public void Test()
        {
            FileUploaderService.FileUploaderService.UploadFiles();
        }
    }
}
