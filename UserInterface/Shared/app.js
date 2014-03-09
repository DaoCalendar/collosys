
var csapp = angular.module("ui.collosys",
    ['ui.bootstrap', 'ui', 'ngUpload', 'ngGrid', 'restangular', "ngRoute"]);

csapp.config(["RestangularProvider", "$logProvider", "$provide", "$httpProvider", "$routeProvider", "$locationProvider",
    function(restangularProvider, $logProvider, $provide, $httpProvider, $routeProvider,$locationProvider) {

        var httpInterceptor = function() {
            $provide.factory('MyHttpInterceptor', ["$q", "$rootScope", function($q, $rootScope) {
                var requestInterceptor = function(config) {
                    if (config.url.indexOf("/api/") !== -1) {
                        $rootScope.$broadcast('$csShowSpinner');
                        console.log("HttpInterceptor : Request : " + moment().format("HH:mm:ss:SSS") + " : " + config.url);
                        //console.log(config);
                    }

                    // Return the config or wrap it in a promise if blank.
                    return config || $q.when(config);
                };

                var requestErrorInterceptor = function(rejection) {
                    if (rejection.config.url.indexOf("/api/") !== -1) {
                        $rootScope.$broadcast('$csHideSpinner');
                        console.log("HttpInterceptor : RequestError : " + moment().format("HH:mm:ss:SSS") + " : " + rejection.config.url);
                        console.log(rejection);
                    }

                    // Return the promise rejection.
                    return $q.reject(rejection);
                };

                var responseInterceptor = function(response) {
                    if (response.config.url.indexOf("/api/") !== -1) {
                        $rootScope.$broadcast('$csHideSpinner');
                        console.log("HttpInterceptor : Response : " + moment().format("HH:mm:ss:SSS") + " : " + response.config.url);
                        //console.log(response);
                    }

                    // Return the response or promise.
                    return response || $q.when(response);
                };

                var responseErrorInterceptor = function(rejection) {
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

        var routeConfig = function() {
            $routeProvider
                .when('/', {
                    templateUrl: 'Shared/templates/main.html',
                    controller: 'MainCtrl'
                })
                .when('/login', {
                    templateUrl: 'Shared/templates/login.html',
                    controller: 'LoginCtrl'
                }).when('/fileupload/filedetail', {
                    templateUrl: '/FileUpload/filedetail/file-detail-list.html',
                    controller: 'fileDetailsController'
                })
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
