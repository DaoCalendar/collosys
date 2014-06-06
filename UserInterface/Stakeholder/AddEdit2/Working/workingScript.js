
csapp.factory("StakeWorkingDatalayer", ["$csnotify", "Restangular", function ($csnotify, rest) {

    var restApi = rest.all('WorkingApi');

    var getWorkingData = function (stakeId) {
        return restApi.customGET('GetStakeWorkingData', { stakeholderId: stakeId })
            .then(function (data) {
                return data;
            });
    };

    return {
        GetWorkingData: getWorkingData
    };
}]);

csapp.controller("StakeWorkingCntrl", ["$scope", "$routeParams", "StakeWorkingDatalayer", "$csModels", function ($scope, $routeParams, datalayer, $csModels) {

    (function () {
        datalayer.GetWorkingData($routeParams.stakeId).then(function (data) {
            console.log("WorkingData: ", data);
        });

        $scope.workingModel = $csModels.getColumns("StkhWorking");
    })();

}]);