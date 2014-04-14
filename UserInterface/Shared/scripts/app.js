
var csapp = angular.module("ui.collosys",
[
    'ui.bootstrap', 'ui', 'ngGrid', 'restangular',
    'ngRoute', 'angularFileUpload', 'ngAnimate',
    'ngCookies'
]);

csapp.provider("routeConfiguration", function RouteConfigurationProvider() {

    this.configureRoutes = function (routeProvider) {
        routeProvider
            .when('/', {
                templateUrl: '/Generic/home/home.html',
                controller: 'HomeCtrl'
            }).when('/home', {
                templateUrl: '/Generic/home/home.html',
                controller: 'HomeCtrl'
            }).when('/login', {
                templateUrl: '/Generic/login/login.html',
                controller: 'loginController'
            }).when('/login2', {
                templateUrl: '/Generic/login/login-holder.html',
                controller: 'LoginCtrl'
            }).when('/logout', {
                templateUrl: 'Generic/login/login-holder.html',
                controller: 'logoutController'
            }).when('/generic/profile', {
                templateUrl: '/Generic/profile/profile.html',
                controller: 'profileController'
            })

            //file upload
            .when('/fileupload/filedetail', {
                templateUrl: '/FileUpload/filedetail/file-detail-list.html',
                controller: 'fileDetailsController'
            }).when('/fileupload/filecolumn', {
                templateUrl: '/FileUpload/filecolumn/file-column.html',
                controller: 'fileColumnController'
            }).when('/fileupload/filemapping', {
                templateUrl: '/FileUpload/filemapping/file-mapping.html',
                controller: 'fileMappingController'
            }).when('/fileupload/filescheduler', {
                templateUrl: '/FileUpload/filescheduler/file-scheduler.html',
                controller: 'fileSchedulerController'
            }).when('/fileupload/filestatus', {
                templateUrl: '/FileUpload/filestatus/file-status.html',
                controller: 'fileStatusController'
            }).when('/fileupload/clientdatadownload', {
                templateUrl: '/FileUpload/clientdatadownload/client-data-download.html',
                controller: 'ClientDataDownloadController'
            }).when('/fileupload/customerinfo', {
                templateUrl: '/FileUpload/customerinfo/customer-info.html',
                controller: 'customerInfoController'
            }).when('/fileupload/paymentchanges', {
                templateUrl: '/FileUpload/paymentreversal/view-payments.html',
                controller: 'paymentManagerController'
            }).when('/fileupload/uploadpincode/', {
                templateUrl: '/FileUpload/uploadpincode/upload-pincode.html',
                controller: 'uploadPincodeController',
                resolve: {
                    dataService: function () { return "pincode"; }
                }
            }).when('/fileupload/uploadrcode/', {
                templateUrl: '/FileUpload/uploadpincode/upload-pincode.html',
                controller: 'uploadPincodeController',
                resolve: {
                    dataService: function () { return "rcode"; }
                }
            }).when('/fileupload/errordata', {
                templateUrl: '/FileUpload/errorcorrection/error-correction.html',
                controller: 'errorDataController'
            })

            //stakeholder
            .when('/stakeholder/add', {
                templateUrl: '/Stakeholder/add/index2.html',
                controller: 'AddStakeHolderCtrl'
            }).when('/stakeholder/edit/:data', {
                templateUrl: '/Stakeholder/add/index2.html',
                controller: 'AddStakeHolderCtrl'
            }).when('/stakeholder/view', {
                templateUrl: '/Stakeholder/view/index.html',
                controller: 'viewStake'
            }).when('/generic/hierarchy', {
                templateUrl: '/Stakeholder/hierarchy/hierarchy-grid.html',
                controller: 'hierarchyController'
            }).when('/generic/hierarchy/add', {
                templateUrl: '/Stakeholder/hierarchy/hierarchy-add.html',
                controller: 'hierarchyAddController'
            })

            //allocation
            .when('/allocation/policy', {
                templateUrl: '/Allocation/policy/allocpolicy.html',
                controller: 'allocPolicyCtrl'
            }).when('/allocation/subpolicy', {
                templateUrl: '/Allocation/subpolicy/allocsubpolicy.html',
                controller: 'allocSubpolicyCtrl'
            }).when('/allocation/viewapprove', {
                templateUrl: '/Allocation/viewapprove/view-approve.html',
                controller: 'approveViewCntrl'
            })

            //billing
            .when('/billing/policy', {
                templateUrl: '/Billing/policy/billingpolicy.html',
                controller: 'payoutPolicyCtrl'
            }).when('/billing/subpolicy', {
                templateUrl: '/Billing/subpolicy/billing-subpolicy.html',
                controller: 'payoutSubpolicyCtrl'
            }).when('/billing/formula', {
                templateUrl: '/Billing/formula/formula.html',
                controller: 'formulaController'
            }).when('/billing/matrix', {
                templateUrl: '/Billing/matrix/matrix.html',
                controller: 'matrixCtrl'
            }).when('/billing/adhoc', {
                templateUrl: '/Billing/adhoc/adhoc.html',
                controller: 'adhocPayoutCtrl'
            }).when('/billing/adhocbulk', {
                templateUrl: '/Billing/adhocbulk/adhocbulk.html',
                controller:'adhocbulkCtrl'
            }).when('/billing/readybilling', {
                templateUrl: '/Billing/readybilling/index.html',
                controller: 'readyForBillingController'
            }).when('/billing/status', {
                templateUrl: '/Billing/status/index.html',
                controller: 'BillingStatusController'
            }).when('/billing/summary', {
                templateUrl: '/Billing/summary/summary.html',
                controller: 'BillAmountCntrl'
            }).when('/billing/billstatus', {
                templateUrl: '/Billing/billstatus/billstatus.html',
                controller: 'billStatusController'
            })

            //generic
            .when('/generic/permission', {
                templateUrl: '/Generic/permissions/NewPermission.html',
                controller: 'newPermissionsController'
            }).when('/generic/product', {
                templateUrl: '/Generic/product/product.html',
                controller: 'ProductConfigController'
            }).when('/generic/keyvalue', {
                templateUrl: '/Generic/keyvalue/keyvalue.html',
                controller: 'keyValueCtrl'
            }).when('/generic/pincode', {
                templateUrl: '/Generic/pincode/pincode.html',
                controller: 'pincodeCtrl'
            }).when('/generic/changepassword', {
                templateUrl: '/Generic/changepassword/changepassword.html',
                controller: 'changepasswordCtrl'
            })

            //developer
            .when('/developer/logdownload', {
                templateUrl: '/Developer/logdownload/logdownload.html',
                controller: 'driveExplorerController'
            }).when('/developer/generatedb', {
                templateUrl: '/Developer/generatedb/generatedb.html',
                controller: 'DbGenerationController'
            }).when('/developer/viewdbtables', {
                templateUrl: '/Developer/dbtable/viewtables.html',
                controller: 'databaseTablesCtrl'
            }).when('/developer/queryexecuter', {
                templateUrl: '/Developer/queryexecuter/query-executer.html',
                controller: 'queryExecuterController'
            })

            //otherwise
            .otherwise({
                redirectTo: '/'
            });
    };

    this.$get = [function routeConfigurationFactory() { return new RouteConfiguration(); }];
});

csapp.config([
    "RestangularProvider", "$logProvider", "$provide", "$httpProvider", "routeConfigurationProvider", "$routeProvider",
    function (restangularProvider, $logProvider, $provide, $httpProvider, routeConfig, $routeProvider) {
        $httpProvider.interceptors.push('MyHttpInterceptor');
        routeConfig.configureRoutes($routeProvider);
        $logProvider.debugEnabled(true);
        restangularProvider.setBaseUrl("/api/");
    }
]);

csapp.run(["$rootScope", "$location", "$templateCache",
    function ($rootScope, $location, $templateCache) {
    $rootScope.$on("$csLoginRequired", function () {
        $location.path("/login");
    });

    //$rootScope.$on('$viewContentLoaded', function () {
    //    $templateCache.removeAll();
    //});
}]);

csapp.constant("$csConstants", {
    GUID_EMPTY: '00000000-0000-0000-0000-000000000000',
    DaysOfWeek: {
        list: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
        value: { Mon: "Mon", Tue: "Tue", Wed: "Wed", Thu: "Thu", Fri: "Fri", Sat: "Sat", Sun: "Sun" }
    },
    EmailFrequency: {
        list: ["Daily", "Weekly", "Monthly"],
        value: { Daily: "Daily", Weekly: "Weekly", Monthly: "Monthly" }
    }
});

