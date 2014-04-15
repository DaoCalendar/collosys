

csapp.factory("menuDataLayer", ['Restangular', "$csfactory", "$csnotify", function (rest, $csfactory, $csnotify) {

    var dldata = {};
    var restApi = rest.all("MenuApi");

    var getPermission = function (user) {
        restApi.customGET("GetPermission", { 'user': user }).then(function (data) {
            if ($csfactory.isNullOrEmptyString(data)) {
                $csnotify.error('Hierarchy not found');
                return;
            }
            dldata.userHierarchy = data;
            dldata.Permissions = JSON.parse(data.Permissions);
            setMenu(dldata.Permissions);
        });
    };

    var setMenu = function (permission) {
        console.log("permission: ", permission);
    };


    return {
        dldata: dldata,
        getPermission: getPermission
    };

}]);

csapp.factory("MenuFactory", ["menuDataLayer", function (datalayer) {

    var menu = [
        {
            Title: "File Upload",
            url: "#",
            childMenu: [
                {
                    Title: "File Detail",
                    url: "#/fileupload/filedetail"
                },
                {
                    Title: "File Column",
                    url: "#/fileupload/filecolumn"
                },
                {
                    Title: "File Mapping",
                    url: "#/fileupload/filemapping"
                },
                {
                    Title: "Schedule Files",
                    url: "#/fileupload/filescheduler"
                },
                {
                    Title: "Check Status",
                    url: "#/fileupload/filestatus"
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

    var getPermission = function (user) {
        datalayer.getPermission(user);
    };

    return {
        Menu: menu,
        getPermission: getPermission
    };

}]);

