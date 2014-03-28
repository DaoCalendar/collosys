using System.IO;
using ColloSys.UserInterface.Areas.Developer.Models.Excel2Db;
using NUnit.Framework;

namespace ReflectionExtension.Tests
{
    [SetUpFixture]
    class SetUpAssemblies
    {

        protected readonly FileStream FileStream;
        protected readonly FileInfo FileInfo;

        public SetUpAssemblies()
        {
             FileStream = ResourceReader.GetEmbeddedResourceAsFileStream("ExampleData.xls");
            FileInfo=new FileInfo(FileStream.Name);
        }
    }
}
