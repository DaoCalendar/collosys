/// <reference path="../../../Scripts/angular.js" />


//csapp.factory("customerDataNewFactory", ["$http", "$q", "$csnotify", "csNgGridService", function ($http, $q, $csnotify, csNgGridService) {

//    var urlAllocPolicy = "/api/CustomerDataNewApi/";

//    var getSystemCategory = function () {
//        var deferred = $q.defer();

//        $http({
//            url: urlAllocPolicy + "GetSystemCategory",
//            method: "GET"
//        }).success(function (data) {
//            deferred.resolve(data);
//        }).error(function (data) {
//            $csnotify.error(data.Message);
//        });

//        return deferred.promise;
//    };
        
//    var ngGrid = function (gridUrl, system, category) {
//        debugger;
//        var deferred = $q.defer();
//        $http({
//            url: gridUrl,
//            method: "GET",
//            params: { system: system, category: category }
//        }).success(function (data) {
//            var csngGridData =csNgGridService.setNgGridData(data);
//            deferred.resolve(csngGridData);
//        }).error(function (data) {
//            $csnotify.error(data.Message);
//        });
//        return deferred.promise;
//    };

//    return {
//        GetSystemCategory: getSystemCategory,
//        NgGrid: ngGrid
//    };

//}]);



csapp.controller("customerDataNewCtrl", ["$scope", "$csnotify", "Restangular", function ($scope, $csnotify, rest) {

    var restApi = rest.all("CustomerDataNewApi");
    $scope.productsList = [];

    //customerDataFactory.GetSystemCategory().then(function (data) {        
    //    $scope.productsList = data;
    //});
    
    restApi.customGET("GetSystemCategory").then(function (data) {
        $scope.productsList = data;
    }, function (data) {
        $csnotify.error(data);
    });


    //$scope.selectProductCategory = function (product) {
    //    var selproduct = JSON.parse(product);
    //    var system = selproduct.System;
    //    var category = selproduct.Category;

    //    $scope.gridReady = false;
    //    var gridurl = "api/CustomerDataApi/GetNgGridOptions";
    //    customerDataFactory.NgGrid(gridurl, system, category).then(function (data) {
    //        $scope.gridOptions = data.gridOptions;
    //        $scope.gridOptions.data = "ngGridModal.data";
    //        $scope.ngGridModal = data.serverNgGridOptions;            
    //        $scope.gridReady = true;
    //    });
    //};


}]);
