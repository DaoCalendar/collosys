using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
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
            var fileUpload = home.AddChild("File Upload", new[] { ColloSysEnums.Activities.FileUploader }, "#, ColloSysEnums.Activities.fa-cloud-upload");
            fileUpload.AddChild("File Detail", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.CreateFile }, "#/fileupload/filedetail");
            fileUpload.AddChild("File Mapping", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.CreateFile }, "#/fileupload/filemapping");
            fileUpload.AddChild("File Column", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.CreateFile }, "#/fileupload/filecolumn");
            fileUpload.AddChild("Schedule", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.ScheduleFile }, "#/fileupload/filescheduler");
            fileUpload.AddChild("Chech Status", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.ScheduleFile }, "#/fileupload/filestatus");
            fileUpload.AddChild("Data Download", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.CustomerData }, "#/fileupload/clientdatadownload");
            fileUpload.AddChild("Upload Pincode", new[] { ColloSysEnums.Activities.FileUploader }, "#/fileupload/uploadpincode");
            fileUpload.AddChild("Upload Rcode", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.UploadCustInfo }, "#/fileupload/uploadrcode");
            fileUpload.AddChild("Correct Errors", new[] { ColloSysEnums.Activities.FileUploader, ColloSysEnums.Activities.ErrorCorrection }, "#/fileupload/errorcorrection");
            fileUpload.AddChild("Filter Data", new[] { ColloSysEnums.Activities.FileUploader }, "#/fileupload/filterCondition");

            var stakeholder = home.AddChild("Stakeholder", new[] { ColloSysEnums.Activities.Stakeholder }, "#, ColloSysEnums.Activities.fa-users");
            stakeholder.AddChild("Add", new[] { ColloSysEnums.Activities.Stakeholder, ColloSysEnums.Activities.AddStakeholder }, "#/stakeholder/add");
            stakeholder.AddChild("View", new[] { ColloSysEnums.Activities.Stakeholder, ColloSysEnums.Activities.ViewStakeholder }, "#/stakeholder/view");

            var allocation = home.AddChild("Allocation", new[] { ColloSysEnums.Activities.Allocation }, "#, ColloSysEnums.Activities.fa-briefcase");
            allocation.AddChild("Define Policy", new[] { ColloSysEnums.Activities.Allocation, ColloSysEnums.Activities.DefinePolicy }, "#/allocation/policy");
            allocation.AddChild("Define Subpolicy", new[] { ColloSysEnums.Activities.Allocation, ColloSysEnums.Activities.DefineSubpolicy }, "#/allocation/subpolicy");
            allocation.AddChild("View/Approve", new[] { ColloSysEnums.Activities.Allocation, ColloSysEnums.Activities.CheckAllocation }, "#/allocation/viewapprove");

            var billing = home.AddChild("Billing", new[] { ColloSysEnums.Activities.Billing }, "#, ColloSysEnums.Activities.fa-inr");
            billing.AddChild("Policy", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.DefinePolicy }, "#/billing/policy");
            billing.AddChild("Formula", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.DefineFormula }, "#/billing/formula");
            billing.AddChild("Formula2", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.DefineFormula }, "#/billing/formula2");
            billing.AddChild("Define Matrix", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.DefineMatrix }, "#/billing/matrix");
            billing.AddChild("Manual Payment", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.ModifyPayment }, "#/fileupload/paymentchanges");
            billing.AddChild("Ready For Billing", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.ReadyForBilling }, "#/billing/readybilling");
            billing.AddChild("Execution Status", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.ReadyForBilling }, "#/billing/status");
            billing.AddChild("Bill Status", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.PayoutStatus }, "#/billing/summary");
            billing.AddChild("Pay Clearance Status", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.PayoutStatus }, "#/billing/billstatus");

            var billingExtension = home.AddChild("Billing Extension", new[] { ColloSysEnums.Activities.Billing }, "#, ColloSysEnums.Activities.fa-inr");
            billingExtension.AddChild("Adhoc", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.AdhocPayout }, "#/billing/adhoc");
            billingExtension.AddChild("Adhoc Bulk", new[] { ColloSysEnums.Activities.Billing, ColloSysEnums.Activities.AdhocPayout }, "#/billing/adhocbulk");
            billingExtension.AddChild("Holding Policy", new[] { ColloSysEnums.Activities.Billing }, "#/billing/holdingpolicy");
            billingExtension.AddChild("Manage Holding", new[] { ColloSysEnums.Activities.Billing }, "#/billing/holdingactive");

            var config = home.AddChild("Config", new[] { ColloSysEnums.Activities.Config }, "#, ColloSysEnums.Activities.fa-cogs");
            config.AddChild("Permission", new[] { ColloSysEnums.Activities.Config, ColloSysEnums.Activities.Permission }, "#/generic/permission");
            config.AddChild("Add Hierarchy", new[] { ColloSysEnums.Activities.Config }, "#/generic/hierarchy/add");
            config.AddChild("View/Edit Hierarchy", new[] { ColloSysEnums.Activities.Config }, "#/generic/hierarchy");
            config.AddChild("Products", new[] { ColloSysEnums.Activities.Config, ColloSysEnums.Activities.Product }, "#/generic/product");
            config.AddChild("Key Value", new[] { ColloSysEnums.Activities.Config, ColloSysEnums.Activities.KeyValue }, "#/generic/keyvalue");
            config.AddChild("Pincode", new[] { ColloSysEnums.Activities.Config, ColloSysEnums.Activities.Pincode }, "#/generic/pincode");
            config.AddChild("Tax List", new[] { ColloSysEnums.Activities.Config, ColloSysEnums.Activities.Taxlist }, "#/generic/taxlist");
            config.AddChild("Tax Master", new[] { ColloSysEnums.Activities.Config, ColloSysEnums.Activities.Taxmaster }, "#/generic/taxmaster");

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

    }
}