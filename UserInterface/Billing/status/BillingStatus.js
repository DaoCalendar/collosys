(
    //#region "Controller"
csapp.controller('BillingStatusController', ["$scope", "$csnotify", "factoryBillingStatus", function ($scope, $csnotify, factoryBillingStatus) {

    var init = function () {
        $scope.billingList = [];
        factoryBillingStatus.FetchData().then(function (data) {
            $scope.billingList = data;
        }, function () {
            $csnotify.error("No data In DB");
        });
    };
    init();
}])
//#endregion
);
(
    //#region Factory
csapp.factory('factoryBillingStatus', ["Restangular", function (rest) {
    var restApi = rest.all('ReadyForBillingApi');

    var fetchData = function () {
        return restApi.customGET('GetBillingStatus');
    };

    return {
        FetchData: fetchData
    };
}])
//#endregion
);