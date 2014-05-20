using System.Collections.Generic;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.GenerateDb
{
    [TestFixture]
    public class PermissionTests
    {
        [Test]
        public void CreatePermissions()
        {
            var permission = new PermissionManager();

        }
    }

    public class Permission
    {
        public ActivityList Title { get; set; }
        public string Link { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public bool HasAccess { get; set; }
        public IList<Permission> Childrents { get; set; }
    }

    public class PermissionManager
    {
        private Permission Root { get; set; }
        public PermissionManager()
        {
            Root = new Permission();
        }

        public void AddPermission(Permission parent,  Permission child)
        {
            if(parent.Childrents == null)
                parent.Childrents = new List<Permission>();
            parent.Childrents.Add(child);
        }
    }

    public enum ActivityList
    {
        FileUploader,
        Stakeholder,
        Allocation,
        Billing,
        Generic
    }
}
