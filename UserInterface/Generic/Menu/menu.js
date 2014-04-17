
csapp.factory("menuFactory", [function () {

    var menu = [];

    var initMenu = function (permissions) {
        console.log(permissions);
        menu = [
            {
                Title: "File Upload",
                url: "#",
                display: permissions.FileUpload.access,
                childMenu: [
                    {
                        Title: "FileDetail",
                        url: "#/fileupload/filedetail",
                        display: permissions.FileUpload.childrens.CreateFile.access,
                    },
                    {
                        Title: "File Column",
                        url: "#/fileupload/filecolumn",
                        display: permissions.FileUpload.childrens.CreateFile.access,
                    },
                    {
                        Title: "File Mapping",
                        url: "#/fileupload/filemapping",
                        display: permissions.FileUpload.childrens.CreateFile.access,

                    },
                    {
                        Title: "Schedule Files",
                        url: "#/fileupload/filescheduler",
                        display: permissions.FileUpload.childrens.ScheduleFile.childrens.Create.access
                    },
                    {
                        Title: "Check Status",
                        url: "#/fileupload/filestatus",
                        display: permissions.FileUpload.childrens.ScheduleFile.childrens.View.access,

                    },
                    {
                        Title: "Data Download",
                        url: "#/fileupload/clientdatadownload",
                        display: permissions.FileUpload.childrens.ScheduleFile.childrens.View.access,
                    },
                    {
                        Title: "Customer Info",
                        url: "#/fileupload/customerinfo",
                        display: permissions.FileUpload.childrens.ScheduleFile.childrens.View.access,
                    },
                    {
                        Title: "Manual Payment",
                        url: "#/fileupload/paymentchanges"
                    },
                    {
                        Title: "Upload Pincode",
                        url: "#/fileupload/uploadpincode"
                    },
                    {
                        Title: "Upload Rcode",
                        url: "#/fileupload/uploadrcode"
                    },
                    {
                        Title: "View Upload Errors",
                        url: "#/fileupload/errordata"
                    }
                ]
            },
            {
                Title: "Stakeholder",
                url: "#",
                display: true,
                childMenu: [
                    {
                        Title: "Add",
                        url: "#/stakeholder/add",
                        display: true
                    },
                    {
                        Title: "View",
                        url: "#/stakeholder/view",
                        display: true
                    }
                ]
            },
            {
                Title: "Allocation",
                url: "#",
                display: '',
                childMenu: [
                    {
                        Title: "Policy",
                        url: "#/allocation/policy",
                        display: '',
                    },
                    {
                        Title: "Subpolicy",
                        url: "#/allocation/subpolicy",
                        display: '',
                    },
                    {
                        Title: "View/Approve",
                        url: "#/allocation/viewapprove",
                        display: '',
                    }
                ]
            },
            {
                Title: "Billing",
                url: "#",
                display: '',
                childMenu: [
                    {
                        Title: "Policy",
                        url: "#/billing/policy",
                        display: '',
                    },
                    {
                        Title: "Subpolicy",
                        url: "#/billing/subpolicy",
                        display: '',
                    },
                    {
                        Title: "Formula",
                        url: "#/billing/formula",
                        display: '',
                    },
                    {
                        Title: "Matrix",
                        url: "#/billing/matrix",
                        display: '',
                    },
                    {
                        Title: "AdHoc",
                        url: "#/billing/adhoc",
                        display: '',
                    },
                    {
                        Title: "AdHoc Bulk",
                        url: "#/billing/adhocbulk",
                        display: '',
                    },
                    {
                        Title: "Ready Billing",
                        url: "#/billing/readybilling",
                        display: '',
                    },
                    {
                        Title: "Status",
                        url: "#/billing/status",
                        display: '',
                    },
                    {
                        Title: "Summary",
                        url: "#/billing/summary",
                        display: '',
                    },
                    {
                        Title: "Payment Status",
                        url: "#/billing/billstatus",
                        display: '',
                    }
                ]
            },
            {
                Title: "Config",
                url: "#",
                display: '',
                childMenu: [
                    {
                        Title: "Add Hierarchy",
                        url: "#/generic/hierarchy/add",
                        display: '',
                    },
                    {
                        Title: "View/Edit Hierarchy",
                        url: "#/generic/hierarchy",
                        display: '',
                    },
                    {
                        Title: "Permissions",
                        url: "#/generic/permission",
                        display: '',
                    },
                    {
                        Title: "Products",
                        url: "#/generic/product",
                        display: '',
                    },
                    {
                        Title: "KeyValue",
                        url: "#/generic/keyvalue",
                        display: '',
                    },
                    {
                        Title: "Pincode",
                        url: "#/generic/pincode",
                        display: '',
                    }
                ]
            },
            {
                Title: "Dev Tools",
                url: "#",
                display: '',
                childMenu: [
                    {
                        Title: "System Explorer",
                        url: "#/developer/logdownload",
                        display: '',
                    },
                    {
                        Title: "Generate DB",
                        url: "#/developer/generatedb",
                        display: '',
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
            }
        ];
        createAuthorisedMenu(menu);
    };


    var createAuthorisedMenu = function (menus) {
        var authorisedMenu = [];

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

                authorisedMenu.push(menuObj);
            }
        });

        console.log("authorised menu: ", authorisedMenu);
    };

    return {
        menu: menu,
        initMenu: initMenu,
    };

}]);

csapp.controller("menuController", ["$scope", "menuFactory", function ($scope, menuFactory) {
    $scope.menu = menuFactory.menu;
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

