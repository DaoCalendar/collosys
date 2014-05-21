using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using Microsoft.Ajax.Utilities;

namespace ColloSys.QueryBuilder.Test.GenerateDb
{
    public static class PermissionManager
    {
        private static void AddActivity(GPermission parent, GPermission child)
        {
            if (parent.Childrens == null)
                parent.Childrens = new List<GPermission>();
            parent.Childrens.Add(child);
        }

        public static GPermission AddActivity(GPermission parent, ColloSysEnums.Activities activity, string desciption="")
        {
            var perm = new GPermission
            {
                HasAccess = false,
                Description = desciption,
                Activity = activity,
                Role = _hierarchy,
                Permission = parent
            };
            AddActivity(parent, perm);
            return perm;
        }

        public static bool CheckAccess(GPermission root, IList<ColloSysEnums.Activities> list)
        {
            var permission = root;
            for (int i = 0; i < list.Count; i++)
            {
                var perm = permission.Childrens.FirstOrDefault(x => x.Activity == list.ElementAt(i));
                if (perm == null || perm.HasAccess == false) return false;
                if (perm.Childrens == null || perm.Childrens.Count == 0) return true;
                permission = perm;
            }
            return false;
        }

        private static StkhHierarchy _hierarchy;
        public static GPermission CreateDevPermissions(StkhHierarchy hierarchy)
        {
            _hierarchy = hierarchy;
            var root = new GPermission();

            var fileupload = AddActivity(root, ColloSysEnums.Activities.FileUploader);
            AddFileUploadActivities(fileupload);

            var stakeholder = AddActivity(root, ColloSysEnums.Activities.Stakeholder);
            AddStakeholderActivities(stakeholder);

            var billing = AddActivity(root, ColloSysEnums.Activities.Billing);
            AddBillingActivities(billing);

            var allocation = AddActivity(root, ColloSysEnums.Activities.Allocation);
            AddAllocationActivities(allocation);

            var config = AddActivity(root, ColloSysEnums.Activities.Config);
            AddConfigActivities(config);

            var developer = AddActivity(root, ColloSysEnums.Activities.Developer);
            AddDeveloperActivities(config);

            return root;
        }

        private static void AddFileUploadActivities(GPermission root)
        {
            var createfile = AddActivity(root, ColloSysEnums.Activities.CreateFile);
            AddActivity(createfile, ColloSysEnums.Activities.View);
            AddActivity(createfile, ColloSysEnums.Activities.AddEdit);
            AddActivity(createfile, ColloSysEnums.Activities.Approve);
            var schedule = AddActivity(root, ColloSysEnums.Activities.ScheduleFile);
            AddActivity(schedule, ColloSysEnums.Activities.Schedule);
            AddActivity(schedule, ColloSysEnums.Activities.Status);
        }

        private static void AddStakeholderActivities(GPermission root)
        {
            AddActivity(root, ColloSysEnums.Activities.CreateFile, "cofigure new file for upload");
        }

        private static void AddAllocationActivities(GPermission root)
        {
            AddActivity(root, ColloSysEnums.Activities.CreateFile, "cofigure new file for upload");
        }

        private static void AddBillingActivities(GPermission root)
        {
            AddActivity(root, ColloSysEnums.Activities.CreateFile, "cofigure new file for upload");
        }

        private static void AddConfigActivities(GPermission root)
        {
            AddActivity(root, ColloSysEnums.Activities.CreateFile, "cofigure new file for upload");
        }

        private static void AddDeveloperActivities(GPermission root)
        {
            AddActivity(root, ColloSysEnums.Activities.Developer, "cofigure new file for upload");
        }
    }
}