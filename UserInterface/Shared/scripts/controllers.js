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

        var saveLastLocation = function() {
            $cookieStore.remove("routelastroute");
            $cookieStore.put("routelastroute", $location.path());
        };

        var getLastLocation = function() {
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

csapp.controller('RootCtrl', ["$scope", "$csAuthFactory", "routeManagerFactory", "$location", "loadingWidget",
    function ($scope, $csAuthFactory, routeManagerFactory, $location, loadingWidget) {

        $scope.$on("$locationChangeStart", routeManagerFactory.$locationChangeStart);
        $scope.$on("$locationChangeSuccess", routeManagerFactory.$locationChangeSuccess);
        $scope.$on("$routeChangeStart", routeManagerFactory.$routeChangeStart);
        $scope.$on("$routeChangeSuccess", routeManagerFactory.$routeChangeSuccess);

        (function () {
            $scope.$csAuthFactory = $csAuthFactory;
            $scope.loadingWidgetParams = loadingWidget.params;
            $csAuthFactory.loadAuthCookie();
        })();

        if (!$csAuthFactory.hasLoggedIn()) {
            $location.path('/login');
        } else {
            //$location.path("/home");
            $location.path(routeManagerFactory.getLastLocation());
        }

    }
]);