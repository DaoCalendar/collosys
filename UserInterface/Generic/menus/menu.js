csapp.controller("menuController", ["$scope", "rootDatalayer", "$csAuthFactory", "$csfactory",
    function ($scope, datalayer, $csAuthFactory, $csfactory) {

    (function () {
        $scope.$watch(function () {
            return $csAuthFactory.getUsername();
        }, function (newval) {
            if (!$csfactory.isNullOrEmptyString(newval)) {
                datalayer.getPermission($csAuthFactory.getUsername()).then(function (data) {
                    $scope.menus = data.menus;
                });
            }
        });
    })();

}]);
