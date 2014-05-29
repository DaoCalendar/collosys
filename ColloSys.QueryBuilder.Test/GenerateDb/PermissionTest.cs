using System;
using System.Linq;
using AngularUI.Generic.Menu;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.JsonSerialization;
using ColloSys.DataLayer.SessionMgr;
using NHibernate.Linq;
using NUnit.Framework;
using Newtonsoft.Json;


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
                root = PermissionManager.SetAccess(root, true);
                root.Role = hierarchy;
                session.SaveOrUpdate(root);
                rx.Commit();
            }
        }

        [Test]
        public void SetPermissions()
        {
            var session = SessionManager.GetCurrentSession();

            using (var sx = session.BeginTransaction())
            {
                var hierarchy = session.QueryOver<StkhHierarchy>()
                                       .Where(x => x.Designation == "HOC")
                                       .And(x => x.Hierarchy == "Field")
                                       .SingleOrDefault();
                GPermission root = PermissionManager.CreateDevPermissions(hierarchy);
                root = PermissionManager.SetAccess(root, true);

                root.Childrens[0].Childrens[0].Childrens[0].HasAccess = false;
                root = PermissionManager.SetAccess(root, hierarchy);

               

                sx.Rollback();
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
                    //.TransformUsing(Transformers.DistinctRootEntity)
                    .List<GPermission>();



                rx.Rollback();
            }
        }

        [Test]
        public void FetchData()
        {

            var session = SessionManager.GetCurrentSession();
            using (var rx = session.BeginTransaction())
            {
                var permData = session.Query<GPermission>()
                                      .Where(
                                          x =>
                                          x.Role.Id == Guid.Parse("2cdaf45b-52d5-4181-b9f8-a23201155b3c") &&
                                          x.Parent == null)

                                      .ToList();
                //.Fetch(x => x.Role).Eager
                //.Fetch(x => x.Childrens)
                //.Fetch(x => x.Parent)
                //.List<GPermission>();



                var setting = new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.None,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        ContractResolver = new NHibernateContractResolver()
                    };
                var data = JsonConvert.SerializeObject(permData[0], setting);



                rx.Rollback();
            }



        }
    }
}
