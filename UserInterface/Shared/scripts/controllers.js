csapp.controller('RootCtrl', ["$scope", "$csStopWatch", "$csAuthFactory", "$location", "Logger",
    function ($scope, $csStopWatch, $csAuthFactory, $location, logManager) {

        var $log = logManager.getInstance("RootController");
        $scope.$csAuthFactory = $csAuthFactory;
        
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