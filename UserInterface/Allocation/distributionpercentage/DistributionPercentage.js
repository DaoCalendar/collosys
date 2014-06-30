csapp.factory("distributionPercentageDatalayer",["Restangular", function(rest) {

    var restApi = rest.all("DistributionPercentageApi");

    return {
        
    };

}]);

csapp.controller("distributionPercentageCtrl", ["$scope", "distributionPercentageDatalayer", function ($scope,datalayer) {

    (function () {

    })();

}]);