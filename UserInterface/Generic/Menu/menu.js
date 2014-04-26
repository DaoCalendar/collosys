
csapp.factory("menuFactory", [function () {

    var menu = [];

    var initMenu = function (permissions) {
        //console.log(permissions);
        //menu = [
        //    {
        //        Title: "File Upload",
        //        url: "#",
        //        display: permissions.FileUpload.access,
        //        childMenu: [
        //            {
        //                Title: "FileDetail",
        //                url: "#/fileupload/filedetail",
        //                display: permissions.FileUpload.childrens.CreateFile.access,
        //            },
        //            {
        //                Title: "File Column",
        //                url: "#/fileupload/filecolumn",
        //                display: permissions.FileUpload.childrens.CreateFile.access,
        //            },
        //            {
        //                Title: "File Mapping",
        //                url: "#/fileupload/filemapping",
        //                display: permissions.FileUpload.childrens.CreateFile.access,

        //            },
        //            {
        //                Title: "Schedule Files",
        //                url: "#/fileupload/filescheduler",
        //                display: permissions.FileUpload.childrens.ScheduleFile.childrens.Create.access
        //            },
        //            {
        //                Title: "Check Status",
        //                url: "#/fileupload/filestatus",
        //                display: permissions.FileUpload.childrens.ScheduleFile.childrens.View.access,

        //            },
        //            {
        //                Title: "Data Download",
        //                url: "#/fileupload/clientdatadownload",
        //                display: permissions.FileUpload.childrens.CustomerData.access,
        //            },
        //            {
        //                Title: "Customer Info",
        //                url: "#/fileupload/customerinfo",
        //                display: permissions.FileUpload.childrens.CustomerData.access,
        //            },
        //            {
        //                Title: "Manual Payment",
        //                url: "#/fileupload/paymentchanges",
        //                display: permissions.FileUpload.childrens.ManualPayment.access
        //            },
        //            {
        //                Title: "Upload Pincode",
        //                url: "#/fileupload/uploadpincode",
        //                display: permissions.FileUpload.childrens.UploadPincode
        //            },
        //            {
        //                Title: "Upload Rcode",
        //                url: "#/fileupload/uploadrcode",
        //                display: permissions.FileUpload.childrens.UploadRcode
        //            },
        //            {
        //                Title: "View Upload Errors",
        //                url: "#/fileupload/errordata"
        //            }
        //        ]
        //    },
        //    {
        //        Title: "Stakeholder",
        //        url: "#",
        //        display: true,
        //        childMenu: [
        //            {
        //                Title: "Add",
        //                url: "#/stakeholder/add",
        //                display: true
        //            },
        //            {
        //                Title: "View",
        //                url: "#/stakeholder/view",
        //                display: true
        //            }
        //        ]
        //    },
        //    {
        //        Title: "Allocation",
        //        url: "#",
        //        display: permissions.Allocation.access,
        //        childMenu: [
        //            {
        //                Title: "Policy",
        //                url: "#/allocation/policy",
        //                display: permissions.Allocation.childrens.DefinePolicy.access
        //            },
        //            {
        //                Title: "Subpolicy",
        //                url: "#/allocation/subpolicy",
        //                display: permissions.Allocation.childrens.DefineSubpolicy.access,
        //            },
        //            {
        //                Title: "View/Approve",
        //                url: "#/allocation/viewapprove",
        //                display: permissions.Allocation.childrens.CheckAllocation.access,
        //            }
        //        ]
        //    },
        //    {
        //        Title: "Billing",
        //        url: "#",
        //        display: true,
        //        childMenu: [
        //            {
        //                Title: "Policy",
        //                url: "#/billing/policy",
        //                display: permissions.Billing.childrens.CreateFile.access
        //            },
        //            {
        //                Title: "Subpolicy",
        //                url: "#/billing/subpolicy",
        //                display: '',
        //            },
        //            {
        //                Title: "Formula",
        //                url: "#/billing/formula",
        //                display: '',
        //            },
        //            {
        //                Title: "Matrix",
        //                url: "#/billing/matrix",
        //                display: '',
        //            },
        //            {
        //                Title: "AdHoc",
        //                url: "#/billing/adhoc",
        //                display: '',
        //            },
        //            {
        //                Title: "AdHoc Bulk",
        //                url: "#/billing/adhocbulk",
        //                display: '',
        //            },
        //            {
        //                Title: "Ready Billing",
        //                url: "#/billing/readybilling",
        //                display: '',
        //            },
        //            {
        //                Title: "Status",
        //                url: "#/billing/status",
        //                display: '',
        //            },
        //            {
        //                Title: "Summary",
        //                url: "#/billing/summary",
        //                display: '',
        //            },
        //            {
        //                Title: "Payment Status",
        //                url: "#/billing/billstatus",
        //                display: '',
        //            }
        //        ]
        //    },
        //    {
        //        Title: "Config",
        //        url: "#",
        //        display: true,
        //        childMenu: [
        //            {
        //                Title: "Add Hierarchy",
        //                url: "#/generic/hierarchy/add",
        //                display: '',
        //            },
        //            {
        //                Title: "View/Edit Hierarchy",
        //                url: "#/generic/hierarchy",
        //                display: '',
        //            },
        //            {
        //                Title: "Permissions",
        //                url: "#/generic/permission",
        //                display: true,
        //            },
        //            {
        //                Title: "Products",
        //                url: "#/generic/product",
        //                display: '',
        //            },
        //            {
        //                Title: "KeyValue",
        //                url: "#/generic/keyvalue",
        //                display: '',
        //            },
        //            {
        //                Title: "Pincode",
        //                url: "#/generic/pincode",
        //                display: '',
        //            }
        //        ]
        //    },
        //    {
        //        Title: "Dev Tools",
        //        url: "#",
        //        display: true,
        //        childMenu: [
        //            {
        //                Title: "System Explorer",
        //                url: "#/developer/logdownload",
        //                display: '',
        //            },
        //            {
        //                Title: "Generate DB",
        //                url: "#/developer/generatedb",
        //                display: true,
        //            },
        //            {
        //                Title: "DB Tables",
        //                url: "#/developer/viewdbtables",
        //                display: '',
        //            },
        //            {
        //                Title: "Execute Query",
        //                url: "#/developer/queryexecuter",
        //                display: '',
        //            }
        //        ]
        //    }
        //];
        //return createAuthorisedMenu(menu);
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
        console.log("authorised menu: ", menu);
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
                    console.log('creating menu by permission');
                    $scope.menus = menuFactory.initMenu(datalayer.dldata.permissions);
                });
            }
        });
    })();

}]);

//var menuByPerm = [];

//var setMenuByPerm = function (permission) {
//    if (permission.access) {

//    }
//};

//var createByPermission = function (permissions) {

//    menuByPerm = [];

//    var stakeholder = _.find(permissions, { 'area': 'Stakeholder' });
//    setMenuByPerm(stakeholder);


//    var allocation = _.find(permissions, { 'area': 'Allocation' });
//    var billing = _.find(permissions, { 'area': 'Billing' });
//    var fileUpload = _.find(permissions, { 'area': 'File Upload' });





//    //_.forEach(permissions, function (permission) {
//    //    if (permission.access === true) {
//    //        var authorisedMenu = _.find(menus, function (menu) {
//    //            if (menu.Title === permission.area)
//    //                return menu;
//    //        });
//    //        if (angular.isDefined(authorisedMenu))
//    //            menuByPerm.push(authorisedMenu);
//    //    }
//    //});
//    console.log("authorised permissions: ", menuByPerm);
//};

