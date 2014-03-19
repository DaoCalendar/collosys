
var csapp = angular.module("ui.collosys",
    ['ui.bootstrap', 'ui', 'ngGrid', 'restangular', "ngRoute", "angularFileUpload", 'ngAnimate']);


csapp.value("lodash", "_");

csapp.config(["RestangularProvider", "$logProvider", "$provide", "$httpProvider", "$routeProvider", "$locationProvider",
    function (restangularProvider, $logProvider, $provide, $httpProvider, $routeProvider, $locationProvider) {

        var httpInterceptor = function () {
            $provide.factory('MyHttpInterceptor', ["$q", "$rootScope", function ($q, $rootScope) {
                var requestInterceptor = function (config) {
                    if (config.url.indexOf("/api/") !== -1) {
                        $rootScope.$broadcast('$csShowSpinner');
                        console.log("HttpInterceptor : Request : " + moment().format("HH:mm:ss:SSS") + " : " + config.url);
                        //console.log(config);
                    }

                    // Return the config or wrap it in a promise if blank.
                    return config || $q.when(config);
                };

                var requestErrorInterceptor = function (rejection) {
                    if (rejection.config.url.indexOf("/api/") !== -1) {
                        $rootScope.$broadcast('$csHideSpinner');
                        console.log("HttpInterceptor : RequestError : " + moment().format("HH:mm:ss:SSS") + " : " + rejection.config.url);
                        console.log(rejection);
                    }

                    // Return the promise rejection.
                    return $q.reject(rejection);
                };

                var responseInterceptor = function (response) {
                    if (response.config.url.indexOf("/api/") !== -1) {
                        $rootScope.$broadcast('$csHideSpinner');
                        console.log("HttpInterceptor : Response : " + moment().format("HH:mm:ss:SSS") + " : " + response.config.url);
                        //console.log(response);
                    }

                    // Return the response or promise.
                    return response || $q.when(response);
                };

                var responseErrorInterceptor = function (rejection) {
                    if (rejection.config.url.indexOf("/api/") !== -1) {
                        $rootScope.$broadcast('$csHideSpinner');
                        console.log("HttpInterceptor : ResponseError : " + moment().format("HH:mm:ss:SSS") + " : " + rejection.config.url);
                        console.log(rejection);
                    }

                    if (rejection.status == 401) {
                        var deferred = $q.defer();
                        $rootScope.$broadcast('$csLoginRequired');
                        return deferred.promise;
                    }

                    if (rejection.status == 0 || rejection.status == 404) {
                        rejection.Message = "You are offline !";
                    }

                    // Return the promise rejection.
                    return $q.reject(rejection);
                };

                return {
                    request: requestInterceptor,
                    requestError: requestErrorInterceptor,
                    response: responseInterceptor,
                    responseError: responseErrorInterceptor
                };
            }]);

            $httpProvider.interceptors.push('MyHttpInterceptor');
        };
        httpInterceptor();

        var routeConfig = function () {
            $routeProvider
                .when('/', {
                    templateUrl: 'Shared/templates/home.html',
                    controller: 'HomeCtrl'
                }).when('/login', {
                    templateUrl: 'Shared/templates/login.html',
                    controller: 'LoginCtrl'
                }).when('/generic/profile', {
                    templateUrl: '/Generic/profle/userprofile.html',
                    controller: 'ProfileController'
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
                })

                //stakeholder
                .when('/stakeholder/add', {
                    templateUrl: '/Stakeholder/add/index.html',
                    controller: 'AddStakeHolderCtrl'
                }).when('/generic/hierarchy', {
                    templateUrl: '/Stakeholder/hierarchy/hierarchy-grid.html',
                    controller: 'hierarchyController'
                }).when('/generic/hierarchy/add', {
                    templateUrl: '/Stakeholder/hierarchy/hierarchy-add.html',
                    controller: 'hierarchyAddController'
                }).when('/stakeholder/view', {
                    templateUrl: '/Stakeholder/view/index.html',
                    controller: 'viewStake'
                })
                //allocation
                .when('/allocation/policy', {
                    templateUrl: '/Allocation/policy/allocpolicy.html',
                    controller: 'allocPolicyCtrl'
                }).when('/allocation/subpolicy', {
                    templateUrl: '/Allocation/subpolicy/allocsubpolicy.html',
                    controller: 'allocSubpolicyCtrl'
                }).when('/allocation/viewapprove', {
                    templateUrl: '/Allocation/viewapprove/index.html',
                    controller: 'approveViewCntrl'
                })

                //billing
                .when('/billing/policy', {
                    templateUrl: '/Billing/policy/index.html',
                    controller: 'payoutPolicyCtrl'
                }).when('/billing/subpolicy', {
                    templateUrl: '/Billing/subpolicy/index.html',
                    controller: 'payoutSubpolicyCtrl'
                }).when('/billing/formula', {
                    templateUrl: '/Billing/formula/index.html',
                    controller: 'formulaCtrl'
                }).when('/billing/matrix', {
                    templateUrl: '/Billing/matrix/index.html',
                    controller: 'matrixCtrl'
                }).when('/billing/adhoc', {
                    templateUrl: '/Billing/adhoc/index.html',
                    controller: 'adhocPayoutCtrl'
                }).when('/billing/readybilling', {
                    templateUrl: '/Billing/readybilling/index.html',
                    controller: 'readyForBillingController'
                }).when('/billing/status', {
                    templateUrl: '/Billing/status/index.html',
                    controller: 'BillingStatusController'
                }).when('/billing/summary', {
                    templateUrl: '/Billing/summary/index.html',
                    controller: 'BillAmountCntrl'
                })

                //generic
                .when('/generic/permissionscreen', {
                    templateUrl: '/Generic/permissionscreen/permissionscreen.html',
                    controller: 'stkPermissionCtrl'
                }).when('/generic/product', {
                    templateUrl: '/Generic/product/product.html',
                    controller: 'ProductConfigController'
                }).when('/generic/keyvalue', {
                    templateUrl: '/Generic/keyvalue/keyvalue.html',
                    controller: 'keyValueCtrl'
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
        routeConfig();

        $logProvider.debugEnabled(true);

        restangularProvider.setBaseUrl("/api/");

        $locationProvider.html5Mode(true);
    }
]);

csapp.run(function ($rootScope, $log, $window) {
    $rootScope.$log = $log;
    $rootScope.loadingElement = { waitingForServerResponse: false };

    $rootScope.$on("$csHideSpinner", function () {
        $rootScope.loadingElement.waitingForServerResponse = false;
    });

    $rootScope.$on("$csShowSpinner", function () {
        $rootScope.loadingElement.waitingForServerResponse = true;
    });

    $rootScope.$on("$csLoginRequired", function () {
        $log.info("FATAL: Unauthorized access");
        $window.location = $window.mvcBaseUrl + "Account/Login";
    });
});

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

csapp.controller("HomeCtrl", [
    function() {

    }
]);

csapp.controller('RootCtrl', ["$rootScope", "$csStopWatch", function ($rootScope, $csStopWatch) {
    $rootScope.loadingElement.stopwatch = $csStopWatch;
    $rootScope.loadingElement.showLoadingElement = false;
    $rootScope.loadingElement.loadingElementText = "processing...";
    $rootScope.loadingElement.disableSpiner = false;

    $rootScope.$watch("loadingElement.waitingForServerResponse", function () {
        if ($rootScope.loadingElement.disableSpiner === true) {
            $rootScope.loadingElement.disableSpiner = false;
            return;
        }

        if ($rootScope.loadingElement.waitingForServerResponse === true) {
            $rootScope.loadingElement.stopwatch.start();
            $rootScope.loadingElement.showLoadingElement = true;
        } else {
            $rootScope.loadingElement.showLoadingElement = false;
            $rootScope.loadingElement.stopwatch.reset();
            $rootScope.loadingElement.loadingElementText = "processing...";
        }
    });
}]);
