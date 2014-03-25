#region references

using System.Linq;
using AngularUI.Developer.generatedb;
using ColloSys.AllocationService.AllocationLayer;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.FileUploadService;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.Shared.Encryption;
using NUnit.Framework;

#endregion

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

        [Test]
        public void UpdateScbuserPassword()
        {
            var usersRepo = new GUsersRepository();
            var users = usersRepo.FilterBy(x => x.Username == "scbuser");
            if (users.Count != 1) return;
            var user = users.First();
            var session = SessionManager.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                user.Password = PasswordUtility.EncryptText("password");
                session.SaveOrUpdate(user);
                tx.Commit();
            }
        }

        [Test]
        public void StartAllocation()
        {
           ColloSys.AllocationService.StartAllocation.Start();
        }

        [Test]
        public void AllocateAc()
        {
            Allocation.StartAllocationProcessV2(ScbEnums.Products.MORT, ScbEnums.Category.Liner);
        }
    }
}
