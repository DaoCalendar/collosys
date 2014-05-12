
csapp.factory("menuFactory", [function () {

    var menu = [];

    var initMenu = function (permissions) {
        menu = [
        {
            Title: "File Upload",
            url: "#",
            icon: "fa-cloud-upload",
            display: permissions.FileUpload.access,
            childMenu: [
                {
                    Title: "FileDetail",
                    url: "#/fileupload/filedetail",
                    display: //(permissions.FileUpload.childrens.CreateFile.childrens.Create.access) ||
                    (permissions.FileUpload.childrens.CreateFile.childrens.Update.access) ||
                    (permissions.FileUpload.childrens.CreateFile.childrens.View.access),
                },
                {
                    Title: "File Column",
                    url: "#/fileupload/filecolumn",
                    display: //(permissions.FileUpload.childrens.CreateFile.childrens.Create.access) ||
                    (permissions.FileUpload.childrens.CreateFile.childrens.Update.access) ||
                    (permissions.FileUpload.childrens.CreateFile.childrens.View.access),
                },
                {
                    Title: "File Mapping",
                    url: "#/fileupload/filemapping",
                    display: //(permissions.FileUpload.childrens.CreateFile.childrens.Create.access) ||
                    (permissions.FileUpload.childrens.CreateFile.childrens.Update.access) ||
                    (permissions.FileUpload.childrens.CreateFile.childrens.View.access),

                },
                {
                    Title: "Schedule Files",
                    url: "#/fileupload/filescheduler",
                    display: permissions.FileUpload.childrens.ScheduleFile.childrens.Schedule.access
                },
                {
                    Title: "Check Status",
                    url: "#/fileupload/filestatus",
                    display: permissions.FileUpload.childrens.ScheduleFile.childrens.Status.access,

                },
                {
                    Title: "Data Download",
                    url: "#/fileupload/clientdatadownload",
                    display: permissions.FileUpload.childrens.CustomerData.childrens.View.access,
                },
                {
                    Title: "Customer Info",
                    url: "#/fileupload/customerinfo",
                    display: permissions.FileUpload.childrens.CustomerData.childrens.View.access
                },
                {
                    Title: "Manual Payment",
                    url: "#/fileupload/paymentchanges",
                    display: (permissions.FileUpload.childrens.ModifyPayment.childrens.View.access) ||
                    (permissions.FileUpload.childrens.ModifyPayment.childrens.Create.access) ||
                    (permissions.FileUpload.childrens.ModifyPayment.childrens.Update.access) ||
                    (permissions.FileUpload.childrens.ModifyPayment.childrens.Approve.access)
                },
                {
                    Title: "Upload Pincode",
                    url: "#/fileupload/uploadpincode",
                    display: permissions.FileUpload.childrens.UploadCustInfo.access
                },
                {
                    Title: "Upload Rcode",
                    url: "#/fileupload/uploadrcode",
                    display: permissions.FileUpload.childrens.UploadCustInfo.access
                },
                {
                    Title: "Correct Errors",
                    url: "#/fileupload/errordata",
                    display: permissions.FileUpload.childrens.ErrorCorrection.childrens.Update.access
                },
                {
                    Title: "Approve Corrections",
                    url: "#/fileupload/errordata",
                    display: permissions.FileUpload.childrens.ErrorCorrection.childrens.Approve.access
                }
            ]
        },
        {
            Title: "Stakeholder",
            url: "#",
            icon: "fa-users",
            display: permissions.Stakeholder.access,
            childMenu: [
                {
                    Title: "Add",
                    url: "#/stakeholder/add",
                    display: (permissions.Stakeholder.childrens.AddStakeholder.childrens.Create.access) ||
                    (permissions.Stakeholder.childrens.AddHierarchy.childrens.Create.access),
                },
                {
                    Title: "View",
                    url: "#/stakeholder/view",
                    display: (permissions.Stakeholder.childrens.ViewStakeholder.childrens.Update.access) ||
                    (permissions.Stakeholder.childrens.ViewStakeholder.childrens.View.access) ||
                    (permissions.Stakeholder.childrens.ViewStakeholder.childrens.Approve.access) ||
                    (permissions.Stakeholder.childrens.ViewHierarchy.childrens.Update.access) ||
                    (permissions.Stakeholder.childrens.ViewHierarchy.childrens.View.access) ||
                    (permissions.Stakeholder.childrens.ViewHierarchy.childrens.Approve.access),
                }
            ]
        },
        {
            Title: "Allocation",
            url: "#",
            icon: "fa-briefcase",
            display: permissions.Allocation.access,
            childMenu: [
                {
                    Title: "Policy",
                    url: "#/allocation/policy",
                    display: (permissions.Allocation.childrens.DefinePolicy.childrens.View.access) ||
                    (permissions.Allocation.childrens.DefinePolicy.childrens.Create.access) ||
                    (permissions.Allocation.childrens.DefinePolicy.childrens.Update.access) ||
                    (permissions.Allocation.childrens.DefinePolicy.childrens.Approve.access)

                },
                {
                    Title: "Subpolicy",
                    url: "#/allocation/subpolicy",
                    display: (permissions.Allocation.childrens.DefineSubpolicy.childrens.View.access) ||
                    (permissions.Allocation.childrens.DefineSubpolicy.childrens.Create.access) ||
                    (permissions.Allocation.childrens.DefineSubpolicy.childrens.Update.access) //||
                    //(permissions.Allocation.childrens.DefineSubpolicy.childrens.Approve.access)
                },
                {
                    Title: "View/Approve",
                    url: "#/allocation/viewapprove",
                    display: (permissions.Allocation.childrens.CheckAllocation.childrens.View.access) ||
                    (permissions.Allocation.childrens.CheckAllocation.childrens.Update.access) ||
                    (permissions.Allocation.childrens.CheckAllocation.childrens.Approve.access)
                }
            ]
        },
        {
            Title: "Billing",
            url: "#",
            icon: "fa-inr",
            display: permissions.Billing.access,
            childMenu: [
            {
                Title: "Policy",
                url: "#/billing/policy",
                display: (permissions.Billing.childrens.DefinePolicy.childrens.View.access) ||
                (permissions.Billing.childrens.DefinePolicy.childrens.Create.access) ||
                (permissions.Billing.childrens.DefinePolicy.childrens.Update.access) ||
                (permissions.Billing.childrens.DefinePolicy.childrens.Approve.access)

            },
            {
                Title: "Subpolicy",
                url: "#/billing/subpolicy",
                display: (permissions.Billing.childrens.DefineSubPolicy.childrens.View.access) ||
                (permissions.Billing.childrens.DefineSubPolicy.childrens.Create.access) ||
                (permissions.Billing.childrens.DefineSubPolicy.childrens.Update.access) //||
                //(permissions.Billing.childrens.DefineSubPolicy.childrens.Approve.access),
            },
            {
                Title: "Formula",
                url: "#/billing/formula",
                display: (permissions.Billing.childrens.DefineFormula.childrens.View.access) ||
                (permissions.Billing.childrens.DefineFormula.childrens.Create.access) ||
                (permissions.Billing.childrens.DefineFormula.childrens.Update.access),
            },
            {
                Title: "Matrix",
                url: "#/billing/matrix",
                display: (permissions.Billing.childrens.DefineMatrix.childrens.View.access) ||
                (permissions.Billing.childrens.DefineMatrix.childrens.Create.access) ||
                (permissions.Billing.childrens.DefineMatrix.childrens.Update.access),
            },
            {
                Title: "AdHoc",
                url: "#/billing/adhoc",
                display: true  //(permissions.Billing.childrens.AdhocPayout.childrens.Create.access) ||
        //(permissions.Billing.childrens.AdhocPayout.childrens.Update.access),
    },
                    {
                        Title: "AdHoc Bulk",
                        url: "#/billing/adhocbulk",
                        display: true //(permissions.Billing.childrens.AdhocPayout.childrens.View.access) ||
                                 //(permissions.Billing.childrens.AdhocPayout.childrens.Create.access) ||
                                 //(permissions.Billing.childrens.AdhocPayout.childrens.Update.access) ||
                                 //(permissions.Billing.childrens.AdhocPayout.childrens.Approve.access),
                    },
                    {
                        Title: "Ready For Billing",
                        url: "#/billing/readybilling",
                        display: permissions.Billing.childrens.ReadyForBilling.childrens.View.access,
                    },
                    {
                        Title: "Execution Status",
                        url: "#/billing/status",
                        display: permissions.Billing.childrens.ReadyForBilling.childrens.Approve.access,
                    },
                    {
                        Title: "Bill Details",
                        url: "#/billing/summary",
                        display: permissions.Billing.childrens.PayoutStatus.childrens.View.access,
                    },
                    {
                        Title: "Pay Clearance Status",
                        url: "#/billing/billstatus",
                        display: permissions.Billing.childrens.PayoutStatus.childrens.View.access,
                    }
                ]
            },
            {
                Title: "Config",
                url: "#",
                icon: "fa-cogs",
                display: permissions.Config.access,
                childMenu: [
                    {
                        Title: "Add Hierarchy",
                        url: "#/generic/hierarchy/add",
                        display: ""
                    },
                    {
                        Title: "View/Edit Hierarchy",
                        url: "#/generic/hierarchy",
                        display: '',
                    },
                    {
                        Title: "Permissions",
                        url: "#/generic/permission",
                        display: (permissions.Config.childrens.Permission.childrens.View.access) ||
                                 (permissions.Config.childrens.Permission.childrens.Update.access),
                    },
                    {
                        Title: "Products",
                        url: "#/generic/product",
                        display: (permissions.Config.childrens.Products.childrens.View.access) ||
                                 (permissions.Config.childrens.Products.childrens.Update.access) ||
                                 (permissions.Config.childrens.Products.childrens.Approve.access),
                    },
                    {
                        Title: "KeyValue",
                        url: "#/generic/keyvalue",
                        display: (permissions.Config.childrens.KeyValue.childrens.View.access) ||
                                 (permissions.Config.childrens.KeyValue.childrens.Update.access) ||
                                 (permissions.Config.childrens.KeyValue.childrens.Approve.access),
                    },
                    {
                        Title: "Pincode",
                        url: "#/generic/pincode",
                        display: (permissions.Config.childrens.Pincode.childrens.View.access) ||
                                 (permissions.Config.childrens.Pincode.childrens.Update.access) ||
                                 (permissions.Config.childrens.Pincode.childrens.Create.access),
                    },
                    {
                        Title: "Tax List",
                        url: "#/generic/taxlist",
                        display: (permissions.Config.childrens.TaxList.childrens.View.access) ||
                                 //(permissions.Config.childrens.TaxList.childrens.Update.access) ||
                                 (permissions.Config.childrens.TaxList.childrens.Create.access),
                    },
                    {
                        Title: "Tax Master",
                        url: "#/generic/taxmaster",
                        display: (permissions.Config.childrens.TaxMaster.childrens.View.access) ||
                                 (permissions.Config.childrens.TaxMaster.childrens.Update.access) ||
                                 (permissions.Config.childrens.TaxMaster.childrens.Create.access),
                    }
                ]
            },
            {
                Title: "Dev Tools",
                url: "#",
                icon: "fa-wrench",
                display: true,
                childMenu: [
                    {
                        Title: "System Explorer",
                        url: "#/developer/logdownload",
                        display: '',
                    },
                    {
                        Title: "Generate DB",
                        url: "#/developer/generatedb",
                        display: true,
                    },
                    {
                        Title: "DB Tables",
                        url: "#/developer/viewdbtables",
                        display: '',
                    },
                    {
                        Title: "Execute Query",
                        url: "#/developer/queryexecuter",
                        display: '',
                    }
                ]
            },
            {
                Title: "User",
                url: "#",
                icon: "fa-user",
                display: true,
                childMenu: [
                    {
                        Title: "Profile",
                        url: "#/generic/profile",
                        display: '',
                    },
                    {
                        Title: "Change Password",
                        url: "#/generic/changepassword",
                        display: true,
                    },
                    {
                        Title: "Logout",
                        url: "#/logout",
                        display: '',
                    }
                ]
            }
        ];
        //return createAuthorisedMenu(menu);
        return menu;
    };

    var createAuthorisedMenu = function (menus) {
        menu = [];
        _.forEach(menus, function (module) {
            var menuObj = {};
            if (module.display === true) {
                menuObj.Title = module.Title;
                menuObj.url = module.url;
                menuObj.childMenu = [];
                _.forEach(module.childMenu, function (subMenu) {//push only authorised childMenus
                    if (subMenu.display === true)
                        menuObj.childMenu.push(angular.copy(subMenu));
                });

                menu.push(menuObj);
            }
        });
        return menu;
    };

    return {
        menu: menu,
        initMenu: initMenu,
    };

}]);

csapp.controller("menuController", ["$scope", "menuFactory", "rootDatalayer", "$csAuthFactory", "$csfactory", function ($scope, menuFactory, datalayer, $csAuthFactory, $csfactory) {

    (function () {
        $scope.$watch(function () {
            return $csAuthFactory.getUsername();
        }, function (newval) {
            if (!$csfactory.isNullOrEmptyString(newval)) {
                datalayer.getPermission($csAuthFactory.getUsername()).then(function () {
                    $scope.menus = menuFactory.initMenu(datalayer.dldata.permissions);
                });
            }
        });
    })();

}]);
