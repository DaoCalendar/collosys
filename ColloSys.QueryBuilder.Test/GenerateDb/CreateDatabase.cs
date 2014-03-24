using AngularUI.Developer.generatedb;
using ColloSys.FileUploadService;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.GenerateDb
{
    public class CreateDatabase
    {
        [Test]
        public void GenerateDB()
        {
            CreateDb.CreateDatabse();
        }

        [Test]
        public void UploadFile()
        {
            FileUploaderService.UploadFiles();
        }
    }
}
