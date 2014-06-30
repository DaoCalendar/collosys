using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using NHibernate.Linq;

namespace AngularUI.Generic.permissions
{
    public static class PermissionManager
    {
        private const string Username2 = "1469319";
        private static string _loggedinUser;

        private static void AddActivity(GPermission parent, GPermission child)
        {
            if (parent.Childrens == null)
                parent.Childrens = new List<GPermission>();
            parent.Childrens.Add(child);
        }

        private static GPermission AddActivity(GPermission parent, ColloSysEnums.Activities activity, string desciption = "")
        {
            var perm = new GPermission
            {
                Description = desciption,
                Activity = activity,
                Role = _hierarchy,
                Parent = parent
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
                if (perm.Childrens == null || perm.Childrens.Count == 0)
                {
                    if (i == list.Count - 1) return perm.HasAccess;
                    return false;
                }
                permission = perm;
            }
            return permission.HasAccess;
        }

        private static void ParseList(GPermission permissions)
        {
            if (permissions.Childrens == null || permissions.Childrens.Count == 0) return;
            foreach (var gPermission in permissions.Childrens)
            {
                ParseList(gPermission);
            }
        }


        public static GPermission GetPermission(StkhHierarchy hierarchy, string user)
        {
            _loggedinUser = user;
            var devPermission = CreateDevPermissions(hierarchy);

            if (hierarchy.Hierarchy == "Admin" && hierarchy.Designation == "SuperAdmin")
            {
                SetAccess(devPermission, true);
                return devPermission;
            }

            var session = SessionManager.GetCurrentSession();
            var userPermission = session.Query<GPermission>()
                                     .Where(x => x.Role.Id == hierarchy.Id && x.Parent == null)
                                     .Fetch(x => x.Role)
                                     .FirstOrDefault();

            var usermenu = devPermission.Childrens
                .FirstOrDefault(x => x.Activity == ColloSysEnums.Activities.User);
            if (userPermission == null)
            {
                SetAccess(devPermission, false);
                SetAccess(usermenu, true);
                return devPermission;
            }

            ParseList(userPermission);
            devPermission.HasAccess = true;
            userPermission.HasAccess = true;
            UpdateRoot(devPermission, userPermission);

            SetAccess(usermenu, true);
            return devPermission;
        }

        public static GPermission SetAccess(GPermission root, bool access)
        {
            if (root == null) return null;

            root.HasAccess = access;
            if (root.Childrens == null || root.Childrens.Count == 0)
                return root;
            foreach (var child in root.Childrens)
            {
                SetAccess(child, access);
            }

            return root;
        }

        public static void UpdateRoot(GPermission devPermission, GPermission userPermission)
        {

            devPermission.Id = userPermission.Id;
            devPermission.Version = userPermission.Version;

            if (devPermission.Activity != userPermission.Activity) return;
            devPermission.HasAccess = userPermission.HasAccess;

            foreach (var child in devPermission.Childrens)
            {
                var userPerm = userPermission.Childrens.SingleOrDefault(x => x.Activity == child.Activity);
                if (userPerm != null) UpdateRoot(child, userPerm);
            }
        }

        private static StkhHierarchy _hierarchy;

        public static GPermission CreateDevPermissions(StkhHierarchy hierarchy)
        {
            if (hierarchy == null)
            {
                throw new ArgumentNullException("hierarchy");
            }

            _hierarchy = hierarchy;
            var root = new GPermission
            {
                Activity = ColloSysEnums.Activities.Root,
                Description = "root member",
                HasAccess = true,
                Parent = null
            };
            if (Username2 == _loggedinUser)
            {
                var developer = AddActivity(root, ColloSysEnums.Activities.Developer);
                AddDeveloperActivities(developer);
                var legal = AddActivity(root, ColloSysEnums.Activities.Legal);
                AddLegalActivities(legal);
            }

            var user = AddActivity(root, ColloSysEnums.Activities.User);
            AddUserActivity(user);

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

            var performance = AddActivity(root, ColloSysEnums.Activities.Performance);
            AddPerformanceActivities(performance);

            return root;
        }

        private static void AddFileUploadActivities(GPermission fileUpload)
        {
            if (Username2 == _loggedinUser)
            {
                var createfile = AddActivity(fileUpload, ColloSysEnums.Activities.CreateFile);
                AddActivity(createfile, ColloSysEnums.Activities.AddEdit);
                AddActivity(createfile, ColloSysEnums.Activities.Approve);
            }

            var status = AddActivity(fileUpload, ColloSysEnums.Activities.ScheduleFile);
            AddActivity(status, ColloSysEnums.Activities.Retry);
            AddActivity(status, ColloSysEnums.Activities.Delete);

            AddActivity(fileUpload, ColloSysEnums.Activities.CustomerData);

            AddActivity(fileUpload, ColloSysEnums.Activities.UploadCustInfo);

            var errorCorrection = AddActivity(fileUpload, ColloSysEnums.Activities.ErrorCorrection);
            AddActivity(errorCorrection, ColloSysEnums.Activities.AddEdit);
            AddActivity(errorCorrection, ColloSysEnums.Activities.Approve);
        }

        private static void AddStakeholderActivities(GPermission stakeholder)
        {
            var addStakeholder = AddActivity(stakeholder, ColloSysEnums.Activities.Stakeholder, desciption: "Add,Edit,Approve other users");
            AddActivity(addStakeholder, ColloSysEnums.Activities.AddEdit);
            AddActivity(addStakeholder, ColloSysEnums.Activities.Approve);

            //var viewStakeholder = AddActivity(stakeholder, ColloSysEnums.Activities.ViewStakeholder);
            //AddActivity(viewStakeholder, ColloSysEnums.Activities.Update);
            //AddActivity(viewStakeholder, ColloSysEnums.Activities.View);
            //AddActivity(viewStakeholder, ColloSysEnums.Activities.Approve);

            //var viewHierarchy = AddActivity(stakeholder, ColloSysEnums.Activities.ViewHierarchy);
            //AddActivity(viewHierarchy, ColloSysEnums.Activities.Update);
            //AddActivity(viewHierarchy, ColloSysEnums.Activities.View);
            //AddActivity(viewHierarchy, ColloSysEnums.Activities.Approve);
        }

        private static void AddAllocationActivities(GPermission allocation)
        {
            var definePolicy = AddActivity(allocation, ColloSysEnums.Activities.AllocationPolicy, desciption: "define policy");
            AddActivity(definePolicy, ColloSysEnums.Activities.AddEdit);
            AddActivity(definePolicy, ColloSysEnums.Activities.Approve);

            var defineSubpolicy = AddActivity(allocation, ColloSysEnums.Activities.AllocationSubpolicy, desciption: "define subpolicy");
            AddActivity(defineSubpolicy, ColloSysEnums.Activities.AddEdit);
            AddActivity(defineSubpolicy, ColloSysEnums.Activities.Approve);

            var chechAllocation = AddActivity(allocation, ColloSysEnums.Activities.CheckAllocation, desciption: "view/change allocation");
            AddActivity(chechAllocation, ColloSysEnums.Activities.AddEdit);
            AddActivity(chechAllocation, ColloSysEnums.Activities.Approve);

            var distributionPercentage = AddActivity(allocation, ColloSysEnums.Activities.SpecifyAllocPercentage, desciption: "Specify Allocation Percentage");
            AddActivity(distributionPercentage, ColloSysEnums.Activities.AddEdit);
        }

        private static void AddBillingActivities(GPermission billing)
        {
            var defineBillingPolicy = AddActivity(billing, ColloSysEnums.Activities.BillingPolicy, desciption: "Define Policy");
            AddActivity(defineBillingPolicy, ColloSysEnums.Activities.AddEdit);
            AddActivity(defineBillingPolicy, ColloSysEnums.Activities.Approve);

            var defineBillingSubpolicy = AddActivity(billing, ColloSysEnums.Activities.BillingSubpolicy, desciption: "Define Subpolicy");
            AddActivity(defineBillingSubpolicy, ColloSysEnums.Activities.AddEdit);
            AddActivity(defineBillingSubpolicy, ColloSysEnums.Activities.Approve);

            var defineFormulaActivity = AddActivity(billing, ColloSysEnums.Activities.Formula, desciption: "Define Formula");
            AddActivity(defineFormulaActivity, ColloSysEnums.Activities.AddEdit);

            var defineMatrix = AddActivity(billing, ColloSysEnums.Activities.Matrix, desciption: "Define Matrix");
            AddActivity(defineMatrix, ColloSysEnums.Activities.AddEdit);

            var adhocPayoutActivity = AddActivity(billing, ColloSysEnums.Activities.AdhocPayout, desciption: "Adhoc Payout");
            AddActivity(adhocPayoutActivity, ColloSysEnums.Activities.AddEdit);
            AddActivity(adhocPayoutActivity, ColloSysEnums.Activities.Approve);

            if (Username2 == _loggedinUser)
            {
                var holdingPayoutActivity = AddActivity(billing, ColloSysEnums.Activities.HoldingPolicy, desciption: "Holding Policy");
                AddActivity(holdingPayoutActivity, ColloSysEnums.Activities.AddEdit);
                AddActivity(holdingPayoutActivity, ColloSysEnums.Activities.Approve);

                var manageHoldingActivity = AddActivity(billing, ColloSysEnums.Activities.ManageHolidng, desciption: "Manage Holding");
                AddActivity(manageHoldingActivity, ColloSysEnums.Activities.AddEdit);
                AddActivity(manageHoldingActivity, ColloSysEnums.Activities.Delete);
            }

            var modifyPayment = AddActivity(billing, ColloSysEnums.Activities.ModifyPayment);
            AddActivity(modifyPayment, ColloSysEnums.Activities.AddEdit);
            AddActivity(modifyPayment, ColloSysEnums.Activities.Approve);


            var readyForBilling = AddActivity(billing, ColloSysEnums.Activities.ReadyForBilling, desciption: "Ready for Billing");
            AddActivity(readyForBilling, ColloSysEnums.Activities.Approve);

            var payoutStatusActivity = AddActivity(billing, ColloSysEnums.Activities.PayoutStatus, desciption: "Billing Status");
            AddActivity(payoutStatusActivity, ColloSysEnums.Activities.AddEdit);
            AddActivity(payoutStatusActivity, ColloSysEnums.Activities.Approve);

        }

        private static void AddConfigActivities(GPermission config)
        {
            var permissionActivity = AddActivity(config, ColloSysEnums.Activities.Permission, desciption: "Config");
            AddActivity(permissionActivity, ColloSysEnums.Activities.Approve);
            AddActivity(permissionActivity, ColloSysEnums.Activities.AddEdit);

            var hierarchyActivity = AddActivity(config, ColloSysEnums.Activities.Hierarchy);
            AddActivity(hierarchyActivity, ColloSysEnums.Activities.AddEdit);

            var productActivity = AddActivity(config, ColloSysEnums.Activities.Product, desciption: "Config");
            AddActivity(productActivity, ColloSysEnums.Activities.Approve);
            AddActivity(productActivity, ColloSysEnums.Activities.AddEdit);

            var keyValueActivity = AddActivity(config, ColloSysEnums.Activities.KeyValue, desciption: "Config");
            AddActivity(keyValueActivity, ColloSysEnums.Activities.Approve);
            AddActivity(keyValueActivity, ColloSysEnums.Activities.AddEdit);

            var pincodeActivity = AddActivity(config, ColloSysEnums.Activities.Pincode, desciption: "Config");
            AddActivity(pincodeActivity, ColloSysEnums.Activities.AddEdit);

            var esclationmatrix = AddActivity(config, ColloSysEnums.Activities.EsclationMatrix, desciption: "Config");
            AddActivity(esclationmatrix, ColloSysEnums.Activities.AddEdit);


            var taxlistActivity = AddActivity(config, ColloSysEnums.Activities.Taxlist, desciption: "Config");
            AddActivity(taxlistActivity, ColloSysEnums.Activities.AddEdit);

            var taxmasterActivity = AddActivity(config, ColloSysEnums.Activities.Taxmaster, desciption: "Config");
            AddActivity(taxmasterActivity, ColloSysEnums.Activities.AddEdit);

        }

        private static void AddDeveloperActivities(GPermission dev)
        {
            AddActivity(dev, ColloSysEnums.Activities.GenerateDb, desciption: "cofigure new file for upload");
            AddActivity(dev, ColloSysEnums.Activities.SystemExplorer, desciption: "cofigure new file for upload");
            AddActivity(dev, ColloSysEnums.Activities.DbTables, desciption: "cofigure new file for upload");
            AddActivity(dev, ColloSysEnums.Activities.ExecuteQuery, desciption: "cofigure new file for upload");
            AddActivity(dev, ColloSysEnums.Activities.EncryptDecrypt, desciption: "Encrypt or Decrypt the connection strings");
        }

        private static void AddPerformanceActivities(GPermission performance)
        {
            AddActivity(performance, ColloSysEnums.Activities.PerformanceParam, desciption: "Performance parameter");

        }

        private static void AddLegalActivities(GPermission user)
        {
            AddActivity(user, ColloSysEnums.Activities.Legal);
        }

        private static void AddUserActivity(GPermission user)
        {
            AddActivity(user, ColloSysEnums.Activities.Logout);
            AddActivity(user, ColloSysEnums.Activities.Profile);
            AddActivity(user, ColloSysEnums.Activities.ChangePassword);
        }



    }
}