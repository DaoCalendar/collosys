using System.Collections.Generic;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.QueryBuilder.Test.GenerateDb;

namespace AngularUI.Generic.Menu
{
    public class Menu
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public Menu Parent { get; set; }
        public IList<Menu> Childrens { get; set; }
        public ushort Level { get; private set; }
        public bool IsActive { get; set; }
        public bool HasAccess { get; set; }
        public ColloSysEnums.Activities[] Permissions { get; set; }


        private void AddMenu(Menu child)
        {
            if (Childrens == null)
                Childrens = new List<Menu>();
            Childrens.Add(child);
        }

        public Menu AddChild(string title, ColloSysEnums.Activities[] permission, string url = "#", string icon = "")
        {
            var menu = new Menu
            {
                Title = title,
                Url = url,
                Icon = icon,
                Level = (ushort)(Level + 1),
                Parent = this,
                Permissions = permission,

            };
            AddMenu(menu);
            return menu;
        }
    }

    public class MenuManager
    {
        public Menu CreateMenu()
        {
            var home = new Menu { Title = "Home" };
            var fileUpload = home.AddChild("File Upload", new[] { ColloSysEnums.Activities.FileUploader }, "#", "fa-cloud-upload");
            fileUpload.AddChild("File Detail", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.CreateFile }, "#/fileupload/filedetail");
            fileUpload.AddChild("File Column", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.CreateFile }, "#/fileupload/filecolumn");
            fileUpload.AddChild("File Mapping", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.CreateFile }, "#/fileupload/filemapping");
            //fileUpload.AddChild("Filter Data", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.CreateFile }, "#/fileupload/filterCondition");

            fileUpload.AddChild("Schedule", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.ScheduleFile }, "#/fileupload/filescheduler");
            fileUpload.AddChild("Check Status", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.Status }, "#/fileupload/filestatus");

            fileUpload.AddChild("Data Download", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.CustomerData }, "#/fileupload/clientdatadownload");
            fileUpload.AddChild("CustomerInfo", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.CustomerData }, "#/fileupload/customerinfo");

            fileUpload.AddChild("Upload Pincode", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.UploadCustInfo }, "#/fileupload/uploadpincode");
            fileUpload.AddChild("Upload Rcode", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.UploadCustInfo }, "#/fileupload/uploadrcode");

            fileUpload.AddChild("Correct Errors", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.ErrorCorrection }, "#/fileupload/errorcorrection");


            var stakeholder = home.AddChild("Stakeholder", new[] { ColloSysEnums.Activities.Stakeholder }, "#", "fa-users");
            stakeholder.AddChild("Add", new[] { ColloSysEnums.Activities.Stakeholder, ColloSysEnums.Activities.Stakeholder }, "#/stakeholder/add");
            stakeholder.AddChild("View", new[] { ColloSysEnums.Activities.Stakeholder, ColloSysEnums.Activities.Stakeholder }, "#/stakeholder/view");

            var allocation = home.AddChild("Allocation", new[] { ColloSysEnums.Activities.Allocation }, "#", "fa-briefcase");
            allocation.AddChild("Policy", new[] { ColloSysEnums.Activities.Allocation, ColloSysEnums.Activities.AllocationPolicy }, "#/allocation/policy");
            allocation.AddChild("Subpolicy", new[] { ColloSysEnums.Activities.Allocation, ColloSysEnums.Activities.AllocationSubpolicy }, "#/allocation/subpolicy");
            allocation.AddChild("View/Approve", new[] { ColloSysEnums.Activities.Allocation, ColloSysEnums.Activities.CheckAllocation }, "#/allocation/viewapprove");

            var billing = home.AddChild("Billing", new[] { ColloSysEnums.Activities.Billing }, "#", "fa-inr");
            billing.AddChild("Policy", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.BillingPolicy }, "#/billing/policy");
            billing.AddChild("Subpolicy", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.BillingSubpolicy }, "#/billing/subpolicy");
            billing.AddChild("Formula", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.Formula }, "#/billing/formula2");
            billing.AddChild("Matrix", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.Matrix }, "#/billing/matrix");
            billing.AddChild("Holding Policy", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.HoldingPolicy }, "#/billing/holdingpolicy");
            billing.AddChild("Manage Holding", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.ManageHolidng }, "#/billing/holdingactive");
            billing.AddChild("Manual Payment", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.ModifyPayment }, "#/fileupload/paymentchanges");
            billing.AddChild("Adhoc", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.AdhocPayout }, "#/billing/adhoc");
            billing.AddChild("Adhoc Bulk", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.AdhocPayout }, "#/billing/adhocbulk");

            var billingStatus = home.AddChild("Billing Status", new[] { ColloSysEnums.Activities.Billing }, "#", "fa-inr");
            billingStatus.AddChild("Ready For Billing", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.ReadyForBilling }, "#/billing/readybilling");
            billingStatus.AddChild("Execution Status", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.ReadyForBilling }, "#/billing/status");
            billingStatus.AddChild("Bill Status", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.PayoutStatus }, "#/billing/summary");
            billingStatus.AddChild("Pay Clearance Status", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.PayoutStatus }, "#/billing/billstatus");

            var config = home.AddChild("Config", new[] { ColloSysEnums.Activities.Config }, "#", "fa-cogs");
            config.AddChild("Permission", new[] { ColloSysEnums.Activities.Config, ColloSysEnums.Activities.Permission }, "#/generic/permission");
            //config.AddChild("Add Hierarchy", new[] { ColloSysEnums.Activities.Config }, "#/generic/hierarchy/add");
            config.AddChild("Hierarchy", new[] { ColloSysEnums.Activities.Config }, "#/generic/hierarchy");
            config.AddChild("Products", new[] { ColloSysEnums.Activities.Config, ColloSysEnums.Activities.Product }, "#/generic/product");
            config.AddChild("Key Value", new[] { ColloSysEnums.Activities.Config, ColloSysEnums.Activities.KeyValue }, "#/generic/keyvalue");
            config.AddChild("Pincode", new[] { ColloSysEnums.Activities.Config, ColloSysEnums.Activities.Pincode }, "#/generic/pincode");
            config.AddChild("Tax List", new[] { ColloSysEnums.Activities.Config, ColloSysEnums.Activities.Taxlist }, "#/generic/taxlist");
            config.AddChild("Tax Master", new[] { ColloSysEnums.Activities.Config, ColloSysEnums.Activities.Taxmaster }, "#/generic/taxmaster");

            var user = home.AddChild("User", new[] { ColloSysEnums.Activities.User }, "#", "fa fa-users");
            user.AddChild("Profile", new[] { ColloSysEnums.Activities.User }, "#/generic/profile");
            user.AddChild("Change Password", new[] { ColloSysEnums.Activities.User }, "#/generic/changepassword");
            user.AddChild("Logout", new[] { ColloSysEnums.Activities.User }, "#/logout");

            var devTools = home.AddChild("Dev Tools", new[] { ColloSysEnums.Activities.Developer }, "#", "fa fa-wrench");
            devTools.AddChild("Generate Db", new[] { ColloSysEnums.Activities.Developer }, "#/developer/generatedb");
            devTools.AddChild("Db Tables", new[] { ColloSysEnums.Activities.Developer }, "#/developer/viewdbtables");
            devTools.AddChild("Execute Query", new[] { ColloSysEnums.Activities.Developer }, "#/developer/queryexecuter");
            devTools.AddChild("System Explorer", new[] { ColloSysEnums.Activities.Developer }, "#/developer/logdownload");

            var legal = home.AddChild("Legal", new[] { ColloSysEnums.Activities.Legal }, "#", "fa-legal");
            legal.AddChild("RequisitionPreparation", new[] { ColloSysEnums.Activities.Legal }, "#/Legal/RequisitionPreparation");
            legal.AddChild("RequsitionIntiation", new[] { ColloSysEnums.Activities.Legal }, "#/Legal/RequsitionIntiation");
            legal.AddChild("FollowUp", new[] { ColloSysEnums.Activities.Legal }, "#/Legal/FollowUp");
            legal.AddChild("LegalCaseexecution", new[] { ColloSysEnums.Activities.Legal }, "#/Legal/LegalCaseexecution");

            return home;
        }

        public static Menu CreateAutherizedMenu(GPermission permission, Menu menulist)
        {
            if (menulist.Permissions == null || menulist.Permissions.Length == 0)
            {
                menulist.HasAccess = true;
            }
            else
            {
                menulist.HasAccess = PermissionManager.CheckAccess(permission, menulist.Permissions);
            }

            if (menulist.Childrens != null && menulist.Childrens.Count > 0)
            {
                foreach (var menu in menulist.Childrens)
                {
                    menu.HasAccess = PermissionManager.CheckAccess(permission, menu.Permissions);
                    if (menu.HasAccess && menu.Childrens != null && menu.Childrens.Count > 0)
                    {
                        foreach (var children in menu.Childrens)
                        {
                            CreateAutherizedMenu(permission, children);
                        }
                    }
                }
            }
            return menulist;
        }

        public static Menu DefaultMenu(Menu menulist)
        {
            foreach (var menu in menulist.Childrens)
            {
                if (menu.Title == "User")
                {
                    menu.HasAccess = true;
                    SetChild(menu, true);

                }
                else
                {
                    menu.HasAccess = false;
                    SetChild(menu, false);
                }
            }

            return menulist;
        }

        private static void SetChild(Menu menu, bool access)
        {
            foreach (var child in menu.Childrens)
            {
                child.HasAccess = access;
            }
        }
    }
}