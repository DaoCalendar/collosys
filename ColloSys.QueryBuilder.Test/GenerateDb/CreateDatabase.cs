using AngularUI.Developer.generatedb;
using ColloSys.FileUploadService;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.GenerateDb
{
    [TestFixture]
    public class CreateDatabase
    {
        [Test]
        public void GenerateDb()
        {
            CreateDb.CreateDatabse();
        }

        [Test]
        public void UploadFile()
        {
            FileUploaderService.UploadFiles();
        }

        [Test]
        public void StartAllocation()
        {
            AllocationService.StartAllocation.Start();
        }
    }
}
