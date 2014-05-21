#region references

using System;
using System.Linq;
using AngularUI.Developer.generatedb;
using ColloSys.AllocationService.AllocationLayer;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
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
        public void CreatePermissions()
        {
            var session = SessionManager.GetCurrentSession();
            using (var rx = session.BeginTransaction())
            {
                var hierarchy = session.QueryOver<StkhHierarchy>()
                    .Where(x => x.Designation == "Developer")
                    .And(x => x.Hierarchy == "Developer")
                    .SingleOrDefault();
                var root = PermissionManager.CreateDevPermissions(hierarchy);
                root.Role = hierarchy;
                session.SaveOrUpdate(root);
                rx.Commit();
            }
        }

        [Test]
        public void FetchPermissions()
        {
            var session = SessionManager.GetCurrentSession();
            using (var rx = session.BeginTransaction())
            {
                var hierarchy = session.QueryOver<StkhHierarchy>()
                    .Where(x => x.Designation == "Developer")
                    .And(x => x.Hierarchy == "Developer")
                    .SingleOrDefault();
                var root = session.QueryOver<GPermission>()
                    .Where(x => x.Role.Id == hierarchy.Id).List<GPermission>();
                var count = root.Where(x => x.Permission == null);
                rx.Rollback();
            }
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
            AllocationService.StartAllocation.Start();
        }

        [Test]
        public void AllocateAc()
        {
            Allocation.StartAllocationProcessV2(ScbEnums.Products.MORT, ScbEnums.Category.Liner);
        }
    }
}
