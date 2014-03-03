
(
csapp.controller("CustomerInfoCtrl", ["$scope", "$csnotify", "Restangular", function ($scope, $csnotify, rest) {
    "use strict";

    var restApi = rest.all("CustomerInfoApi");

    $scope.productsList = [];
    $scope.customerInfos = [];


    debugger;
    restApi.customGET("GetProducts").then(function (data) {
        $scope.productsList = data;
    }, function (data) {
        $csnotify.error(data.data.Message);
    });

    $scope.GetCustomerDetails = function (custInfo) {
        debugger;
        if (angular.isDefined($scope.custInfo.Products) && angular.isDefined($scope.custInfo.AccountNo)) {
            restApi.customGET("GetCustomerInfo", { products: custInfo.Products, accountNo: custInfo.AccountNo }).then(function (data) {
                $scope.customerInfos = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });
        }
    };
}])
);