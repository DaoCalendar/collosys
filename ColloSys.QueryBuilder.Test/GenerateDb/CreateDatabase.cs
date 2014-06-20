#region references

using System.Linq;
using AngularUI.Developer.generatedb;
using ColloSys.AllocationService.AllocationLayer;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploadService;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.Shared.Encryption;
using NUnit.Framework;

#endregion

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
        public void Check_FileDetail_OnAlias()
        {
            var session = SessionManager.GetCurrentSession();
            var fileDetailOnAlias =
                            session.QueryOver<FileDetail>()
                            .Where(x => x.AliasName == ColloSysEnums.FileAliasName.E_WRITEOFF_AUTO)
                            .SingleOrDefault();
        }

        [Test]
        public void StartAllocation()
        {
            AllocationService.StartAllocation.Start();
        }

        //[Test]
        //public void AllocateAc()
        //{
        //    Allocation.StartAllocationProcessV2(ScbEnums.Products.MORT, ScbEnums.Category.Liner);
        //}
    }
}
