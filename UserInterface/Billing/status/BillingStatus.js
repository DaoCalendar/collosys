csapp.controller('BillingStatusController', ["$scope", "$csnotify", "factoryBillingStatus", function ($scope, $csnotify, factoryBillingStatus) {

    (function () {
        $scope.billingList = [];
        factoryBillingStatus.FetchData().then(function (data) {
            $scope.billingList = data;
        }, function () {
            $csnotify.error("No data In DB");
        });
    })();

}]);

csapp.factory('factoryBillingStatus', ["Restangular", function (rest) {
    var restApi = rest.all('ReadyForBillingApi');

    var fetchData = function () {
        return restApi.customGET('GetBillingStatus');
    };

    return {
        FetchData: fetchData
    };
}]);