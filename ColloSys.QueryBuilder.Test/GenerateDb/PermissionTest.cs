using AngularUI.Generic.Menu;
using AngularUI.Generic.permissions;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.SessionMgr;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.GenerateDb
{
    [TestFixture]
    public class PermissionTest
    {
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

                var hierarchyNcm = session.QueryOver<StkhHierarchy>()
                                   .Where(x => x.Designation == "HOS")
                                   .And(x => x.Hierarchy == "DSA")
                                   .SingleOrDefault();
                var ncmRoot = PermissionManager.CreateNcmPermissions(hierarchyNcm);
                ncmRoot.Role = hierarchy;
                session.SaveOrUpdate(ncmRoot);
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
                    .Where(x => x.Role.Id == hierarchy.Id)
                    .And(x => x.Parent == null)
                    //.TransformUsing(Transformers.DistinctRootEntity)
                    .List<GPermission>();

                var menu = new MenuManager();
                var ma = menu.CreateMenu();
                ma = MenuManager.CreateAutherizedMenu(root[0], ma);
                Assert.IsTrue( ma.Childrens.Count != 0);
                rx.Rollback();
            }
        }

        [Test]
        public void GetMenuForUserWithPermissionUndefinedOrNull()
        {
            var session = SessionManager.GetCurrentSession();

            using (var sx = session.BeginTransaction())
            {
                var hierarchy = session.QueryOver<StkhHierarchy>()
                 .Where(x => x.Designation == "HOC")
                 .And(x => x.Hierarchy == "Field")
                 .SingleOrDefault();

                var root = session.QueryOver<GPermission>()
                                  .Where(x => x.Role.Id == hierarchy.Id)
                                  .And(x => x.Parent == null)
                                  .List<GPermission>();

                var menu = new MenuManager();
                var ma = menu.CreateMenu();

                if (root == null || root.Count == 0)
                {
                    ma = MenuManager.DefaultMenu(ma);
                }
                Assert.IsTrue( ma.Childrens.Count != 0);

                sx.Rollback();
            }

        }

        [Test]
        public void SaveChangedPermission()
        {
            var session = SessionManager.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                var hierarchy = session.QueryOver<StkhHierarchy>()
                     .Where(x => x.Designation == "Developer")
                     .And(x => x.Hierarchy == "Developer")
                     .SingleOrDefault();
                var root = session.QueryOver<GPermission>()
                    .Where(x => x.Role.Id == hierarchy.Id)
                    .And(x => x.Parent == null)
                    //.TransformUsing(Transformers.DistinctRootEntity)
                    .List<GPermission>();

                foreach (var gPermission in root[0].Childrens)
                {
                    gPermission.HasAccess = true;
                }

                session.SaveOrUpdate(root[0]);
                tx.Commit();
            }

        }

        [Test]
        public void CheckChildern()
        {
            var session = SessionManager.GetCurrentSession();
            using (var rx = session.BeginTransaction())
            {
                var hierarchy = session.QueryOver<StkhHierarchy>()
                   .Where(x => x.Designation == "Developer")
                   .And(x => x.Hierarchy == "Developer")
                   .SingleOrDefault();

                var root = session.QueryOver<GPermission>()
                    .Where(x => x.Role.Id == hierarchy.Id)
                    .And(x => x.Parent == null)
                    .List<GPermission>();

                Assert.IsTrue(root.Count != 0);

                rx.Rollback();
            }
        }

    }
}
