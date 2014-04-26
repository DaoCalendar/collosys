//Datalayer
csapp.factory("filterConditionDatalayer", ["Restangular", "$csnotify", function (rest, $csnotify) {

    var restApi = rest.all("");
    var dldata = {};
    return {
        dldata:dldata
    };

}]);

//factory
csapp.factory("filterconditionFactory", ["filterConditionDatalayer", function (datalayer) {
    var dldata = datalayer.dldata;

    return {
        
    };
}]);

//Controller
csapp.controller("filterConditionController", ["$scope", "filterConditionDatalayer", "filterconditionFactory", "$csFileUploadModels",
    function ($scope, datalayer, factory, $csFileUploadModels) {

    (function () {
        $scope.datalayer = datalayer;
        $scope.dldata = datalayer.dldata;
        $scope.factory = factory;
        $scope.FilterCondition = $csFileUploadModels.models.FilterCondition();
    })();

}]);