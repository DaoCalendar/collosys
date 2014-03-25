csapp.factory("routeManagerFactory", [
    "Logger", "$location", "$csAuthFactory", "$route", "$cookieStore",
    function (logManager, $location, $csAuthFactory, $route, $cookieStore) {

        var $log = logManager.getInstance("RootController");

        var $locationChangeStart = function () { //evt, next, current

            if (!$csAuthFactory.hasLoggedIn()) {
                $location.path('/login');
                return;
            }

            $log.debug("Changing location : " + $location.path());
            $cookieStore.remove("routelastroute");
            $cookieStore.put("routelastroute", $location.path());
        };

        var $routeChangeStart = function () {
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
            $routeChangeStart: $routeChangeStart
        };
    }
]);

csapp.controller('RootCtrl', ["$scope", "$csAuthFactory", "routeManagerFactory", "$location",
    function ($scope, $csAuthFactory, routeManagerFactory, $location) {

        $scope.$on("$locationChangeStart", routeManagerFactory.$locationChangeStart);
        $scope.$on("$routeChangeStart", routeManagerFactory.$routeChangeStart);

        (function () {
            $scope.$csAuthFactory = $csAuthFactory;
            $csAuthFactory.loadAuthCookie();
            $location.path("/home");
        })();

    }
]);