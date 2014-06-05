using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.ClientData;
using ColloSys.FileUploaderService;
using NUnit.Framework;
using ReflectionExtension.Tests.DataCreator.FileUploader;

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
    }
}
