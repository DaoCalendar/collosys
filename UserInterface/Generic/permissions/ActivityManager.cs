using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using Glimpse.AspNet.Tab;
using Microsoft.Ajax.Utilities;
using NHibernate.SqlCommand;
using NHibernate.Transform;

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

    

        public static GPermission AddActivity(GPermission parent, ColloSysEnums.Activities activity, string desciption = "")
        {
            var perm = new GPermission
            {
                HasAccess = false,
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
                if (perm.Childrens == null || perm.Childrens.Count == 0) return false;
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

        private static void AddFileUploadActivities(GPermission fileUpload)
        {
            var createfile = AddActivity(fileUpload, ColloSysEnums.Activities.CreateFile);
            AddActivity(createfile, ColloSysEnums.Activities.View);
            AddActivity(createfile, ColloSysEnums.Activities.AddEdit);
            AddActivity(createfile, ColloSysEnums.Activities.Approve);

            var schedule = AddActivity(fileUpload, ColloSysEnums.Activities.ScheduleFile);
            AddActivity(schedule, ColloSysEnums.Activities.Schedule);
            AddActivity(schedule, ColloSysEnums.Activities.Status);

            var customerData = AddActivity(fileUpload, ColloSysEnums.Activities.CustomerData);
            AddActivity(customerData, ColloSysEnums.Activities.View);

            var uploadCustInfo = AddActivity(fileUpload, ColloSysEnums.Activities.UploadCustInfo);
            AddActivity(uploadCustInfo, ColloSysEnums.Activities.Update);

            var errorCorrection = AddActivity(fileUpload, ColloSysEnums.Activities.ErrorCorrection);
            AddActivity(errorCorrection, ColloSysEnums.Activities.Approve);
            AddActivity(errorCorrection, ColloSysEnums.Activities.Update);

            var modifyPayment = AddActivity(fileUpload, ColloSysEnums.Activities.ModifyPayment);
            AddActivity(modifyPayment, ColloSysEnums.Activities.View);
            AddActivity(modifyPayment, ColloSysEnums.Activities.Create);
            AddActivity(modifyPayment, ColloSysEnums.Activities.Update);
            AddActivity(modifyPayment, ColloSysEnums.Activities.Approve);

        }

        private static void AddStakeholderActivities(GPermission stakeholder)
        {
            var addStakeholder = AddActivity(stakeholder, ColloSysEnums.Activities.AddStakeholder, "Add,Edit,Approve other users");
            AddActivity(addStakeholder, ColloSysEnums.Activities.Create);

            var viewStakeholder = AddActivity(stakeholder, ColloSysEnums.Activities.ViewStakeholder);
            AddActivity(viewStakeholder, ColloSysEnums.Activities.Update);
            AddActivity(viewStakeholder, ColloSysEnums.Activities.View);
            AddActivity(viewStakeholder, ColloSysEnums.Activities.Approve);

            var addHierarchy = AddActivity(stakeholder, ColloSysEnums.Activities.AddHierarchy);
            AddActivity(addHierarchy, ColloSysEnums.Activities.Create);

            var viewHierarchy = AddActivity(stakeholder, ColloSysEnums.Activities.ViewHierarchy);
            AddActivity(viewHierarchy, ColloSysEnums.Activities.Update);
            AddActivity(viewHierarchy, ColloSysEnums.Activities.View);
            AddActivity(viewHierarchy, ColloSysEnums.Activities.Approve);
        }

        private static void AddAllocationActivities(GPermission allocation)
        {
            var definePolicy = AddActivity(allocation, ColloSysEnums.Activities.DefinePolicy, "define policy");
            AddActivity(definePolicy, ColloSysEnums.Activities.View);
            AddActivity(definePolicy, ColloSysEnums.Activities.Create);
            AddActivity(definePolicy, ColloSysEnums.Activities.Update);
            AddActivity(definePolicy, ColloSysEnums.Activities.Approve);

            var defineSubpolicy = AddActivity(allocation, ColloSysEnums.Activities.DefineSubpolicy, "define subpolicy");
            AddActivity(defineSubpolicy, ColloSysEnums.Activities.View);
            AddActivity(defineSubpolicy, ColloSysEnums.Activities.Create);
            AddActivity(defineSubpolicy, ColloSysEnums.Activities.Update);
            AddActivity(defineSubpolicy, ColloSysEnums.Activities.Approve);

            var chechAllocation = AddActivity(allocation, ColloSysEnums.Activities.CheckAllocation, "check allocation");
            AddActivity(chechAllocation, ColloSysEnums.Activities.View);
            AddActivity(chechAllocation, ColloSysEnums.Activities.Update);
            AddActivity(chechAllocation, ColloSysEnums.Activities.Approve);
        }

        private static void AddBillingActivities(GPermission billing)
        {
            var defineBillingPolicy = AddActivity(billing, ColloSysEnums.Activities.DefineBillingPolicy, "billing");
            AddActivity(defineBillingPolicy, ColloSysEnums.Activities.View);
            AddActivity(defineBillingPolicy, ColloSysEnums.Activities.Create);
            AddActivity(defineBillingPolicy, ColloSysEnums.Activities.Update);
            AddActivity(defineBillingPolicy, ColloSysEnums.Activities.Approve);

            var defineBillingSubpolicy = AddActivity(billing, ColloSysEnums.Activities.DefineBillingSubpolicy, "billing");
            AddActivity(defineBillingSubpolicy, ColloSysEnums.Activities.View);
            AddActivity(defineBillingSubpolicy, ColloSysEnums.Activities.Create);
            AddActivity(defineBillingSubpolicy, ColloSysEnums.Activities.Update);
            AddActivity(defineBillingSubpolicy, ColloSysEnums.Activities.Approve);

            var defineFormulaActivity = AddActivity(billing, ColloSysEnums.Activities.DefineFormula, "billing");
            AddActivity(defineFormulaActivity, ColloSysEnums.Activities.View);
            AddActivity(defineFormulaActivity, ColloSysEnums.Activities.Create);
            AddActivity(defineFormulaActivity, ColloSysEnums.Activities.Update);

            var defineMatrix = AddActivity(billing, ColloSysEnums.Activities.DefineMatrix, "billing");
            AddActivity(defineMatrix, ColloSysEnums.Activities.View);
            AddActivity(defineMatrix, ColloSysEnums.Activities.Create);
            AddActivity(defineMatrix, ColloSysEnums.Activities.Update);

            var adhocPayoutActivity = AddActivity(billing, ColloSysEnums.Activities.AdhocPayout, "billing");
            AddActivity(adhocPayoutActivity, ColloSysEnums.Activities.View);
            AddActivity(adhocPayoutActivity, ColloSysEnums.Activities.Create);
            AddActivity(adhocPayoutActivity, ColloSysEnums.Activities.Update);
            AddActivity(adhocPayoutActivity, ColloSysEnums.Activities.Approve);


            var readyForBilling = AddActivity(billing, ColloSysEnums.Activities.ReadyForBilling, "billing");
            AddActivity(readyForBilling, ColloSysEnums.Activities.View);
            AddActivity(readyForBilling, ColloSysEnums.Activities.Approve);

            var payoutStatusActivity = AddActivity(billing, ColloSysEnums.Activities.PayoutStatus, "billing");
            AddActivity(payoutStatusActivity, ColloSysEnums.Activities.View);
            AddActivity(payoutStatusActivity, ColloSysEnums.Activities.Update);

        }

        private static void AddConfigActivities(GPermission config)
        {
            var permissionActivity = AddActivity(config, ColloSysEnums.Activities.Permission, "Config");
            AddActivity(permissionActivity, ColloSysEnums.Activities.View);
            AddActivity(permissionActivity, ColloSysEnums.Activities.Approve);
            AddActivity(permissionActivity, ColloSysEnums.Activities.Update);

            var productActivity = AddActivity(config, ColloSysEnums.Activities.Product, "Config");
            AddActivity(productActivity, ColloSysEnums.Activities.View);
            AddActivity(productActivity, ColloSysEnums.Activities.Approve);
            AddActivity(productActivity, ColloSysEnums.Activities.Update);

            var keyValueActivity = AddActivity(config, ColloSysEnums.Activities.KeyValue, "Config");
            AddActivity(keyValueActivity, ColloSysEnums.Activities.View);
            AddActivity(keyValueActivity, ColloSysEnums.Activities.Approve);
            AddActivity(keyValueActivity, ColloSysEnums.Activities.Update);

            var pincodeActivity = AddActivity(config, ColloSysEnums.Activities.Pincode, "Config");
            AddActivity(pincodeActivity, ColloSysEnums.Activities.View);
            AddActivity(pincodeActivity, ColloSysEnums.Activities.Create);
            AddActivity(pincodeActivity, ColloSysEnums.Activities.Update);


            var taxlistActivity = AddActivity(config, ColloSysEnums.Activities.Taxlist, "Config");
            AddActivity(taxlistActivity, ColloSysEnums.Activities.View);
            AddActivity(taxlistActivity, ColloSysEnums.Activities.Create);
            AddActivity(taxlistActivity, ColloSysEnums.Activities.Update);

            var taxmasterActivity = AddActivity(config, ColloSysEnums.Activities.Taxmaster, "Config");
            AddActivity(taxmasterActivity, ColloSysEnums.Activities.View);
            AddActivity(taxmasterActivity, ColloSysEnums.Activities.Create);
            AddActivity(taxmasterActivity, ColloSysEnums.Activities.Update);
        }

        private static void AddDeveloperActivities(GPermission root)
        {
            AddActivity(root, ColloSysEnums.Activities.Developer, "cofigure new file for upload");
        }
    }
}