﻿csapp.controller("customerInfoController", [
    "$scope", "$csnotify", "Restangular","$csModels",
    function ($scope, $csnotify, rest, $csModels) {
        "use strict";

        $scope.custInfoModel = $csModels.getColumns("CustomerInfo");

        var restApi = rest.all("CustomerInfoApi");

        $scope.productsList = [];
        $scope.customerInfos = [];
       
        restApi.customGET("GetProducts").then(function(data) {
            $scope.productsList = data;
        }, function(data) {
            $csnotify.error(data.data.Message);
        });

        $scope.GetCustomerDetails = function(custInfo) {
            if (angular.isDefined($scope.custInfo.Product) && angular.isDefined($scope.custInfo.AccountNo)) {
                restApi.customGET("GetCustomerInfo", { products: custInfo.Product, accountNo: custInfo.AccountNo }).then(function(data) {
                    $scope.customerInfos = data;
                }, function(data) {
                    $csnotify.error(data.data.Message);
                });
            }
        };
    }
]);