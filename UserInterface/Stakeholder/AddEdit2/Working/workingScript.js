
csapp.factory("StakeWorkingDatalayer", ["$csnotify", "Restangular", function ($csnotify, rest) {


    var restApi = rest.all('WorkingApi');

    var getHierarchyById = function (id) {
        return restApi.customGET('Get', { 'id': id }).then(function (data) {
            return data;
        });
    };

    return {
        GetHierarchyById: getHierarchyById
    };
}]);

csapp.controller("StakeWorkingCntrl", ["$scope", "$routeParams", "StakeWorkingDatalayer", function ($scope, $routeParams, datalayer) {

    (function () {
        console.log('hierarchy ID: ', $routeParams.hierarchy);
        datalayer.GetHierarchyById($routeParams.hierarchy).then(function (data) {
            $scope.selectedHierarchy = data;
            console.log("current hierarchy: ", data);
        });
    })();

}]);