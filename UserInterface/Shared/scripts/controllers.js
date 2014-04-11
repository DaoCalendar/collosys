csapp.factory("routeManagerFactory", [
    "Logger", "$location", "$csAuthFactory", "$route", "$cookieStore",
    function (logManager, $location, $csAuthFactory, $route, $cookieStore) {

        var $log = logManager.getInstance("RootController");

        var $locationChangeStart = function () { //evt, next, current
        };

        var $locationChangeSuccess = function () { //evt, next, current
            $log.debug("Changed to location : " + $location.path());
            saveLastLocation();
        };

        var $routeChangeStart = function () {
        };

        var $routeChangeSuccess = function () {
        };

        var saveLastLocation = function () {
            $cookieStore.remove("routelastroute");
            $cookieStore.put("routelastroute", $location.path());
        };

        var getLastLocation = function () {
            var location = $cookieStore.get("routelastroute");
            if (angular.isUndefined(location) || location === null || location === "")
                location = "/home";
            $log.info("previous location was : " + location);
            return location;
        };

        return {
            getLastLocation: getLastLocation,
            $locationChangeStart: $locationChangeStart,
            $locationChangeSuccess: $locationChangeSuccess,
            $routeChangeStart: $routeChangeStart,
            $routeChangeSuccess: $routeChangeSuccess,
        };
    }
]);

csapp.factory("rootDatalayer", ["Restangular", "$csnotify", "$csShared", "Logger", "$csModels",
    function (rest, $csnotify, $csShared, logManager, $csModels) {
        var rootapi = rest.all("SharedEnumsApi");
        var dldata = {};
        var $log = logManager.getInstance("rootDatalayer");

        var fetchWholeEnums = function () {
            return rootapi.customGET("FetchAllEnum").then(function (data) {
                $csShared.enums = data;
                $log.info("enums loaded.");
                $csModels.init();
                $log.info("models initialized.");
                return data;
            }, function (data) {
                $csnotify.error(data);
            });
        };

        return {
            dldata: dldata,
            fetchWholeEnums: fetchWholeEnums,
        };

    }
]);

csapp.controller('RootCtrl', ["$scope", "$csAuthFactory", "routeManagerFactory", "$location", "loadingWidget", "rootDatalayer", "Logger",
    function ($scope, $csAuthFactory, routeManagerFactory, $location, loadingWidget, datalayer, logManager) {

        var $log = logManager.getInstance("RootCtrl");
        $scope.$on("$locationChangeStart", routeManagerFactory.$locationChangeStart);
        $scope.$on("$locationChangeSuccess", routeManagerFactory.$locationChangeSuccess);
        $scope.$on("$routeChangeStart", routeManagerFactory.$routeChangeStart);
        $scope.$on("$routeChangeSuccess", routeManagerFactory.$routeChangeSuccess);

        var redirect = function () {
            if (!$csAuthFactory.hasLoggedIn()) {
                $location.path('/login');
            } else {
                //$location.path("/home");
                $location.path(routeManagerFactory.getLastLocation());
            }
        };
        
        (function () {
            $scope.$csAuthFactory = $csAuthFactory;
            $scope.loadingWidgetParams = loadingWidget.params;
            $csAuthFactory.loadAuthCookie();
            datalayer.fetchWholeEnums().then(redirect);
        })();

        
    }
]);