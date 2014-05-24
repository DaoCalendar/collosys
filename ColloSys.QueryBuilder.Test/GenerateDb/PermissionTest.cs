using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
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


                rx.Rollback();
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

                var fileUpload = from child in root[0].Childrens
                                 where child.Activity == ColloSysEnums.Activities.FileUploader
                                 select child;

                rx.Rollback();
            }
        }

    }
}
