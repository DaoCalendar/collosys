
csapp.factory("menuFactory", [function () {

    var menu = [];

    var initMenu = function(permissions) {


        menu = [
            {
                Title: "File Upload",
                url: "#",
                display: false,
                permission: ["File Upload"],
                childMenu: [
                    {
                        Title: "FileDetail",
                        url: "#/fileupload/filedetail",
                        display: false,
                        permission: ["CreateFile", "view"]
                    },
                    {
                        Title: "File Column",
                        url: "#/fileupload/filecolumn",
                        display: false,
                        permission: ["CreateFile", "view"]
                    },
                    {
                        Title: "File Mapping",
                        url: "#/fileupload/filemapping",
                        display: false,
                        permission: ["CreateFile", "view"]
                    },
                    {
                        Title: "Schedule Files",
                        url: "#/fileupload/filescheduler",
                        display: false,
                        permission: ["CreateFile", "view"]
                    },
                    {
                        Title: "Check Status",
                        url: "#/fileupload/filestatus",
                        display: false,
                        permission: ["CreateFile", "view"]
                    },
                    {
                        Title: "Data Download",
                        url: "#/fileupload/clientdatadownload"
                    },
                    {
                        Title: "Customer Info",
                        url: "#/fileupload/customerinfo"
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
                childMenu: [
                    {
                        Title: "Add",
                        url: "#/stakeholder/add"
                    },
                    {
                        Title: "View",
                        url: "#/stakeholder/view"
                    }
                ]
            },
            {
                Title: "Allocation",
                url: "#",
                childMenu: [
                    {
                        Title: "Policy",
                        url: "#/allocation/policy"
                    },
                    {
                        Title: "Subpolicy",
                        url: "#/allocation/subpolicy"
                    },
                    {
                        Title: "View/Approve",
                        url: "#/allocation/viewapprove"
                    }
                ]
            },
            {
                Title: "Billing",
                url: "#",
                childMenu: [
                    {
                        Title: "Policy",
                        url: "#/billing/policy"
                    },
                    {
                        Title: "Subpolicy",
                        url: "#/billing/subpolicy"
                    },
                    {
                        Title: "Formula",
                        url: "#/billing/formula"
                    },
                    {
                        Title: "Matrix",
                        url: "#/billing/matrix"
                    },
                    {
                        Title: "AdHoc",
                        url: "#/billing/adhoc"
                    },
                    {
                        Title: "AdHoc Bulk",
                        url: "#/billing/adhocbulk"
                    },
                    {
                        Title: "Ready Billing",
                        url: "#/billing/readybilling"
                    },
                    {
                        Title: "Status",
                        url: "#/billing/status"
                    },
                    {
                        Title: "Summary",
                        url: "#/billing/summary"
                    },
                    {
                        Title: "Payment Status",
                        url: "#/billing/billstatus"
                    }
                ]
            },
            {
                Title: "Config",
                url: "#",
                childMenu: [
                    {
                        Title: "Add Hierarchy",
                        url: "#/generic/hierarchy/add"
                    },
                    {
                        Title: "View/Edit Hierarchy",
                        url: "#/generic/hierarchy"
                    },
                    {
                        Title: "Permissions",
                        url: "#/generic/permission"
                    },
                    {
                        Title: "Products",
                        url: "#/generic/product"
                    },
                    {
                        Title: "KeyValue",
                        url: "#/generic/keyvalue"
                    },
                    {
                        Title: "Pincode",
                        url: "#/generic/pincode"
                    }
                ]
            },
            {
                Title: "Dev Tools",
                url: "#",
                childMenu: [
                    {
                        Title: "System Explorer",
                        url: "#/developer/logdownload"
                    },
                    {
                        Title: "Generate DB",
                        url: "#/developer/generatedb"
                    },
                    {
                        Title: "DB Tables",
                        url: "#/developer/viewdbtables"
                    },
                    {
                        Title: "Execute Query",
                        url: "#/developer/queryexecuter"
                    }
                ]
            }
        ];
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

