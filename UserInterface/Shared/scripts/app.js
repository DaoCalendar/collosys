
var csapp = angular.module("ui.collosys",
[
    'ui.bootstrap', 'ngGrid', 'restangular',
    'ngRoute', 'angularFileUpload', //'ngAnimate',
    'ngCookies', 'chieffancypants.loadingBar',
    'ui.utils', 'ui.multiselect', 'ngStorage', 'decipher.tags'
]);

csapp.provider("routeConfiguration", function RouteConfigurationProvider() {
    baseUrl = baseUrl || '/';
    this.configureRoutes = function (routeProvider) {
        routeProvider
            .when('/', {
                templateUrl: baseUrl + 'Generic/home/home.html',
                controller: 'HomeCtrl'
            }).when('/home', {
                templateUrl: baseUrl + 'Generic/home/home.html',
                controller: 'HomeCtrl'
            }).when('/login', {
                templateUrl: baseUrl + 'Generic/login/login.html',
                controller: 'loginController'
            }).when('/login2', {
                templateUrl: baseUrl + 'Generic/login/login-holder.html',
                controller: 'LoginCtrl'
            }).when('/logout', {
                templateUrl: baseUrl + 'Generic/login/login-holder.html',
                controller: 'logoutController'
            }).when('/generic/profile', {
                templateUrl: baseUrl + 'Generic/profile/profile.html',
                controller: 'profileController'
            }).when('/generic/taxlist', {
                templateUrl: baseUrl + 'Generic/taxlist/taxlist.html',
                controller: 'taxlistCtrl'
            }).when('/generic/taxlist/addedit/:mode/:id', {
                templateUrl: baseUrl + 'Generic/taxlist/taxlist-list.html',
                controller: 'taxlistAddEditCtrl'
            }).when('/generic/taxlist/addedit/:mode', {
                templateUrl: baseUrl + 'Generic/taxlist/taxlist-list.html',
                controller: 'taxlistAddEditCtrl'
            }).when('/generic/taxmaster', {
                templateUrl: baseUrl + 'Generic/taxmaster/taxmaster.html',
                controller: 'taxmasterCtrl'
            }).when('/generic/taxmaster/addedit/:mode/:id', {
                templateUrl: baseUrl + 'Generic/taxmaster/taxmaster-list.html',
                controller: 'taxmasterAddEditCtrl'
            }).when('/generic/taxmaster/addedit/:mode', {
                templateUrl: baseUrl + 'Generic/taxmaster/taxmaster-list.html',
                controller: 'taxmasterAddEditCtrl'
            }).when('/generic/stkhleave', {
                templateUrl: baseUrl + 'Generic/stkhleave/StkhLeave.html',
                controller: 'StkhLeaveManagementCtrl'
            })

            //file upload
            .when('/fileupload/filedetail', {
                templateUrl: baseUrl + 'FileUpload/filedetail/file-detail-list.html',
                controller: 'fileDetailsController'
            }).when('/fileupload/filedetail/addedit/:mode/:id', {
                templateUrl: baseUrl + 'FileUpload/filedetail/file-detail-add.html',
                controller: 'fileDetailsAddEditController'
            }).when('/fileupload/filedetail/addedit/:mode/', {
                templateUrl: baseUrl + 'FileUpload/filedetail/file-detail-add.html',
                controller: 'fileDetailsAddEditController'
            }).when('/fileupload/filecolumn', {
                templateUrl: baseUrl + 'FileUpload/filecolumn/file-column.html',
                controller: 'fileColumnController'
            }).when('/fileupload/filemapping', {
                templateUrl: baseUrl + 'FileUpload/filemapping/file-mapping.html',
                controller: 'fileMappingController'
            }).when('/fileupload/filemapping/editview/:mode/:id', {
                templateUrl: baseUrl + 'FileUpload/filemapping/file-mapping-edit.html',
                controller: 'fileMappingViewEditController',
            }).when('/fileupload/filescheduler', {
                templateUrl: baseUrl + 'FileUpload/filescheduler/file-scheduler.html',
                controller: 'fileSchedulerController'
            }).when('/fileupload/filestatus', {
                templateUrl: baseUrl + 'FileUpload/filestatus/file-status.html',
                controller: 'fileStatusController'
            }).when('/fileupload/clientdatadownload', {
                templateUrl: baseUrl + 'FileUpload/clientdatadownload/client-data-download.html',
                controller: 'ClientDataDownloadController'
            }).when('/fileupload/customerinfo', {
                templateUrl: baseUrl + 'FileUpload/customerinfo/customer-info.html',
                controller: 'customerInfoController'
            }).when('/fileupload/paymentchanges', {
                templateUrl: baseUrl + 'FileUpload/paymentreversal/view-payments.html',
                controller: 'paymentManagerController'
            }).when('/fileupload/uploadpincode/', {
                templateUrl: baseUrl + 'FileUpload/uploadpincode/upload-pincode.html',
                controller: 'uploadPincodeController',
                resolve: {
                    dataService: function () { return "pincode"; }
                }
            }).when('/fileupload/uploadrcode/', {
                templateUrl: baseUrl + 'FileUpload/uploadpincode/upload-pincode.html',
                controller: 'uploadPincodeController',
                resolve: {
                    dataService: function () { return "rcode"; }
                }
            }).when('/fileupload/errorcorrection', {
                templateUrl: baseUrl + 'FileUpload/errorcorrection/error-correction.html',
                controller: 'errorDataController'
            }).when('/fileupload/errorapproval', {
                templateUrl: baseUrl + 'FileUpload/errorapproval/error-approval.html',
                controller: 'errorApprovalController'
            }).when('/fileupload/filterCondition', {
                templateUrl: baseUrl + 'FileUpload/filterCondition/filterCondition.html',
                controller: 'filterConditionController'
            })

            //stakeholder
            .when('/stakeholder/add', {
                templateUrl: baseUrl + 'Stakeholder/addedit/BasicInfo/basic.html',
                controller: 'AddStakeHolderCtrl'
            }).when('/stakeholder/add/:stakeId', {
                templateUrl: baseUrl + 'Stakeholder/addedit/BasicInfo/basic.html',
                controller: 'AddStakeHolderCtrl'
            }).when('/stakeholder/working/:stakeId', {
                templateUrl: baseUrl + 'Stakeholder/addedit/Working/Working.html',
                controller: 'StakeWorkingCntrl'
            }).when('/stakeholder/working/edit/:editStakeId', {
                templateUrl: baseUrl + 'Stakeholder/addedit/Working/Working.html',
                controller: 'StakeWorkingCntrl'
            }).when('/stakeholder/edit/:data', {
                templateUrl: baseUrl + 'Stakeholder/add/index2.html',
                controller: 'AddStakeHolderCtrl'
            }).when('/stakeholder/view', {
                templateUrl: baseUrl + 'Stakeholder/view/index.html',
                controller: 'viewStake'
            }).when('/stakeholder/view/:data', {
                templateUrl: baseUrl + 'Stakeholder/view/index.html',
                controller: 'viewStake'
            }).when('/generic/hierarchy', {
                templateUrl: baseUrl + 'Stakeholder/hierarchy/hierarchy-grid.html',
                controller: 'hierarchyController'
            }).when('/generic/hierarchy/addedit/:mode/:id', {
                templateUrl: baseUrl + 'Stakeholder/hierarchy/hierarchy-edit.html',
                controller: 'hierarchyEditController',
            }).when('/generic/hierarchy/addedit/:mode', {
                templateUrl: baseUrl + 'Stakeholder/hierarchy/hierarchy-edit.html',
                controller: 'hierarchyEditController',
            })


            //legal
            .when('/Legal/RequisitionPreparation', {
                templateUrl: baseUrl + 'Legal/RequisitionPreparation/RequisitionPreparation.html',
                controller: 'RequisitionCtrl'
            }).when('/Legal/RequsitionIntiation', {
                templateUrl: baseUrl + 'Legal/RequsitionIntiation/RequsitionIntiation.html',
                controller: 'RequsitionIntiationCtrl'
            }).when('/Legal/LegalCaseexecution', {
                templateUrl: baseUrl + 'Legal/LegalCaseexecution/LegalCaseexecution.html',
                controller: 'LegalCaseexecutionCtrl'
            }).when('/Legal/FollowUp', {
                templateUrl: baseUrl + 'Legal/FollowUp/Follow-Up.html',
                controller: 'FollowUpCtrl'
            })

            //allocation
            .when('/allocation/policy', {
                templateUrl: baseUrl + 'Allocation/policy/allocpolicy.html',
                controller: 'allocPolicyCtrl'
            }).when('/allocation/subpolicy', {
                templateUrl: baseUrl + 'Allocation/subpolicy/allocsubpolicy.html',
                controller: 'allocSubpolicyCtrl'
            }).when('/allocation/viewapprove', {
                templateUrl: baseUrl + 'Allocation/viewapprove/view-approve.html',
                controller: 'approveViewCntrl'
            })

            //billing
            .when('/billing/policy', {
                templateUrl: baseUrl + 'Billing/policy/billingpolicy.html',
                controller: 'newpolicyController'
            }).when('/billing/subpolicy', {
                templateUrl: baseUrl + 'Billing/subpolicy/billing-subpolicy.html',
                controller: 'payoutSubpolicyCtrl'
            }).when('/billing/formula', {
                templateUrl: baseUrl + 'Billing/formula/formula.html',
                controller: 'formulaController'
            }).when('/billing/formula2', {
                templateUrl: baseUrl + 'Billing/formula2/formula.html',
                controller: 'formulaController'
            }).when('/billing/matrix', {
                templateUrl: baseUrl + 'Billing/matrix/matrix.html',
                controller: 'matrixCtrl'
            }).when('/billing/adhoc', {
                templateUrl: baseUrl + 'Billing/adhoc/adhoc.html',
                controller: 'adhocPayoutCtrl'
            }).when('/billing/adhoc/addedit/:mode', {
                templateUrl: baseUrl + 'Billing/adhoc/add-hoc-payment-details.html',
                controller: 'adhocPaymentCtrl'
            }).when('/billing/adhocbulk', {
                templateUrl: baseUrl + 'Billing/adhocbulk/adhocbulk.html',
                controller: 'adhocbulkCtrl'
            }).when('/billing/readybilling', {
                templateUrl: baseUrl + 'Billing/readybilling/index.html',
                controller: 'readyForBillingController'
            }).when('/billing/status', {
                templateUrl: baseUrl + 'Billing/status/index.html',
                controller: 'BillingStatusController'
            }).when('/billing/summary', {
                templateUrl: baseUrl + 'Billing/summary/summary.html',
                controller: 'BillAmountCntrl'
            }).when('/billing/summary/addedit/:mode', {
                templateUrl: baseUrl + 'Billing/summary/add-billamount-modal.html',
                controller: 'billAmountAddModal'
            }).when('/billing/billstatus', {
                templateUrl: baseUrl + 'Billing/billstatus/billstatus.html',
                controller: 'billStatusController'
            }).when('/billing/holdingpolicy', {
                templateUrl: baseUrl + 'Billing/holdingpolicy/holding-policy-list.html',
                controller: 'holdingpolicyCtrl'
            }).when('/billing/holdingpolicy/addedit/:mode/:id', {
                templateUrl: baseUrl + 'Billing/holdingpolicy/holding-policy.html',
                controller: 'holdingpolicyAddEditCtrl'
            }).when('/billing/holdingpolicy/addedit/:mode', {
                templateUrl: baseUrl + 'Billing/holdingpolicy/holding-policy.html',
                controller: 'holdingpolicyAddEditCtrl'
            }).when('/billing/holdingactive', {
                templateUrl: baseUrl + 'Billing/holdingactivate/holding-policy-activate-list.html',
                controller: 'holdingactiveCtrl'
            }).when('/billing/holdingactive/addedit/:mode/:id', {
                templateUrl: baseUrl + 'Billing/holdingactivate/holding-policy-active.html',
                controller: 'holdingactiveAddEditCtrl'
            }).when('/billing/holdingactive/addedit/:mode', {
                templateUrl: baseUrl + 'Billing/holdingactivate/holding-policy-active.html',
                controller: 'holdingactiveAddEditCtrl'
            })

            //generic
            .when('/generic/permission', {
                templateUrl: baseUrl + 'Generic/permissions/NewPermission.html',
                controller: 'newPermissionsController'
            }).when('/generic/product', {
                templateUrl: baseUrl + 'Generic/product/product.html',
                controller: 'ProductConfigController'
            }).when('/generic/product/addedit/:mode/:id', {
                templateUrl: baseUrl + 'Generic/product/updateViewConfiguration.html',
                controller: 'updateViewController'
            }).when('/generic/keyvalue', {
                templateUrl: baseUrl + 'Generic/keyvalue/keyvalue.html',
                controller: 'keyValueCtrl'
            }).when('/generic/pincode', {
                templateUrl: baseUrl + 'Generic/pincode/pincode.html',
                controller: 'pincodeCtrl'
            }).when('/generic/pincode/addedit/:mode/:id', {
                templateUrl: baseUrl + 'Generic/pincode/editPincode-modal.html',
                controller: 'editPincodeModalController',
            }).when('/generic/pincode/addedit/:mode', {
                templateUrl: baseUrl + 'Generic/pincode/editPincode-modal.html',
                controller: 'editPincodeModalController',
            }).when('/generic/changepassword', {
                templateUrl: baseUrl + 'Generic/changepassword/changepassword.html',
                controller: 'changepasswordCtrl'
            }).when('/generic/esclationmatrix', {
                templateUrl: baseUrl + 'Generic/esclationmatrix/esclationmatrix.html',
                controller: 'EsclationMatrixCtrl'
            }).when('/generic/GNotifications', {
                templateUrl: baseUrl + 'Generic/GNotifications/Gnotifications.html',
                controller: 'GNotificationsCtrl'
            }).when('/generic/Encrypt', {
                templateUrl: baseUrl + 'Generic/general/index.html',
                controller: 'config'
            })

            //developer
            .when('/developer/logdownload', {
                templateUrl: baseUrl + 'Developer/logdownload/logdownload.html',
                controller: 'driveExplorerController'
            }).when('/developer/generatedb', {
                templateUrl: baseUrl + 'Developer/generatedb/generatedb.html',
                controller: 'DbGenerationController'
            }).when('/developer/viewdbtables', {
                templateUrl: baseUrl + 'Developer/dbtable/viewtables.html',
                controller: 'databaseTablesCtrl'
            }).when('/developer/queryexecuter', {
                templateUrl: baseUrl + 'Developer/queryexecuter/query-executer.html',
                controller: 'queryExecuterController'
            })

            //PerformanaceParameter
            .when('/performance/performanceparameter', {
                templateUrl: baseUrl + 'performance/performanceparameter/PerformanaceParams.html',
                controller: 'PerformanaceParameterCtrl'
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
        restangularProvider.setBaseUrl(baseUrl + "api/");

        restangularProvider.setDefaultHeaders('Access-Control-Allow-Origin: *');
        restangularProvider.setDefaultHeaders('Access-Control-Allow-Methods: GET, POST, PUT, DELETE');
        restangularProvider.setDefaultHeaders('Access-Control-Allow-Headers: Accept, X-Requested-With');
    }
]);

csapp.run(["$rootScope", "$location", "$templateCache", "Logger",
    function ($rootScope, $location, $templateCache, logManager) {

        var $log = logManager.getInstance("appRun");
        $log.info("base url is : " + baseUrl);

        $rootScope.$on("$csLoginRequired", function () {
            $location.path("/login");
        });
    }
]);

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

