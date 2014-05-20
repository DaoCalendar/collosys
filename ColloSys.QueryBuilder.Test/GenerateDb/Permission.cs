using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.GenerateDb
{
    [TestFixture]
    public class PermissionTests
    {
        [Test]
        public void CreatePermissions()
        {
            ActivityManager.CreateDevPermissions();
        }
    }

    public class AppActivity
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public ActivityList Name { get; set; }
        public string Description { get; set; }
        public bool HasAccess { get; set; }
        public IList<AppActivity> Childrens { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
    }

    public static class ActivityManager
    {
        private static void AddActivity(AppActivity parent, AppActivity child)
        {
            if (parent.Childrens == null)
                parent.Childrens = new List<AppActivity>();
            parent.Childrens.Add(child);
        }

        public static AppActivity AddActivity(AppActivity parent, ActivityList activity, string desciption="")
        {
            var perm = new AppActivity
            {
                HasAccess = false,
                Description = desciption,
                Name = activity
            };
            AddActivity(parent, perm);
            return perm;
        }

        public static bool CheckAccess(AppActivity root, IList<ActivityList> list)
        {
            var permission = root;
            for (int i = 0; i < list.Count; i++)
            {
                var perm = permission.Childrens.FirstOrDefault(x => x.Name == list.ElementAt(i));
                if (perm == null || perm.HasAccess == false) return false;
                if (perm.Childrens == null || perm.Childrens.Count == 0) return true;
                permission = perm;
            }
            return false;
        }

        public static AppActivity CreateDevPermissions()
        {
            var root = new AppActivity();

            var fileupload = AddActivity(root, ActivityList.FileUploader);
            AddFileUploadActivities(fileupload);

            var stakeholder = AddActivity(root, ActivityList.Stakeholder);
            AddStakeholderActivities(stakeholder);

            var billing = AddActivity(root, ActivityList.Billing);
            AddBillingActivities(billing);

            var allocation = AddActivity(root, ActivityList.Allocation);
            AddAllocationActivities(allocation);

            var config = AddActivity(root, ActivityList.Config);
            AddConfigActivities(config);

            return root;
        }

        private static void AddFileUploadActivities(AppActivity root)
        {
            var createfile = AddActivity(root, ActivityList.CreateFile);
            AddActivity(createfile, ActivityList.View);
            AddActivity(createfile, ActivityList.AddEdit);
            AddActivity(createfile, ActivityList.Approve);
        }

        private static void AddStakeholderActivities(AppActivity root)
        {
            AddActivity(root, ActivityList.CreateFile, "cofigure new file for upload");
        }

        private static void AddAllocationActivities(AppActivity root)
        {
            AddActivity(root, ActivityList.CreateFile, "cofigure new file for upload");
        }

        private static void AddBillingActivities(AppActivity root)
        {
            AddActivity(root, ActivityList.CreateFile, "cofigure new file for upload");
        }

        private static void AddConfigActivities(AppActivity root)
        {
            AddActivity(root, ActivityList.CreateFile, "cofigure new file for upload");
        }
    }

    public enum ActivityList
    {
        AddEdit,
        View,
        Approve,
        FileUploader,
        CreateFile,
        ScheduleFile,
        CustomerData,
        UploadPincode,
        ErrorCorrection,
        Stakeholder,
        Allocation,
        Billing,
        Config,
        AddHierarchy,
    }
}
