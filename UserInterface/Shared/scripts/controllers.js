csapp.controller('RootCtrl', ["$scope", "$rootScope", "$csStopWatch", "$csAuthFactory", "$location", "Logger",
    function ($scope, $rootScope, $csStopWatch, $csAuthFactory, $location, logManager) {

        var $log = logManager.getInstance("RootController");
        $scope.$csAuthFactory = $csAuthFactory;

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

        $csAuthFactory.loadAuthCookie();
        if ($csAuthFactory.hasLoggedIn()) {
            $log.info("Routing user to home page.");
            $location.path("/home");
        } else {
            $log.info("Routing user to login page.");
            $location.path("/login");
        }
    }
]);