csapp.controller("customerInfoController", [
    "$scope", "$csnotify", "Restangular", "$csModels",
    function ($scope, $csnotify, rest, $csModels) {
        "use strict";

        $scope.custInfoModel = $csModels.getColumns("CustomerInfo");

        var restApi = rest.all("CustomerInfoApi");

        $scope.productsList = [];
        $scope.customerInfos = [];

        restApi.customGET("GetProducts").then(function (data) {
            $scope.productsList = data;
        }, function (data) {
            $csnotify.error(data.data.Message);
        });

        $scope.GetCustomerDetails = function (custInfo) {
            return restApi.customGET("GetCustomerInfo", { products: custInfo.Product, accountNo: custInfo.AccountNo })
                .then(function (data) {
                    $scope.customerInfos = {
                        CustInfo: data.CustInfo,
                        Payments: data.Payments
                    };
                    if (angular.isUndefined($scope.customerInfos.CustInfo)) {
                        $csnotify.success("No data found!!!");
                    }
                }, function (data) {
                    $csnotify.error(data.data.Message);
                });
        };
    }
]);