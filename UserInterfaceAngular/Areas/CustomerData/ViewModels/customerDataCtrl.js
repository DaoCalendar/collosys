/// <reference path="../../../Scripts/angular.js" />

//var customerDataApp = angular.module("customerDataApp", []);

////customerDataApp.controller("customerDataCtrl", ["$scope", "$http", function($scope, $http) {

////}]);
//customerDataApp.directive("tdinfo", function () {
//    return {
//        restrict: 'E',
//        scope: {
//            info: '='
//        },
//        template:
//             "<td>{{info.DelqStatus}}</td>" +
//             "<td>{{info.DelqDate | date:'yyyy-MM-dd'}}</td>" +
//             "<td>{{info.DelqAmount}}</td>" +
//             "<td>{{info.Products}}</td>" +
//             "<td>{{info.StartDate | date:'yyyy-MM-dd'}}</td>" +
//             "<td>{{info.EndDate | date:'yyyy-MM-dd'}}</td>" +
//             "<td>{{info.Cycle}}</td>" +
//             "<td>{{info.InterestPct}}</td>" +
//             "<td>{{info.Pincode}}</td>" +
//             "<td>{{info.Cluster}}</td>" +
//             "<td>{{info.DoAllocate}}</td>"
//    };
//});

//customerDataApp.directive("tdtest", function () {
//    return {
//        restrict: 'E',
//        template:
//             "<td></td>" +
//             "<td></td>" +
//             "<td></td>" +
//             "<td></td>" +
//             "<td></td>" +
//             "<td></td>" +
//             "<td></td>" +
//             "<td></td>" +
//             "<td></td>" +
//             "<td></td>" +
//             "<td></td>"
//    };
//});



//csapp.factory("customerDataFactory", ["$http", "$q", "$csnotify", "csNgGridService", function ($http, $q, $csnotify, csNgGridService) {

//    var urlAllocPolicy = "api/CustomerDataApi/";

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
//            var csngGridData = csNgGridService.setNgGridData(data);
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



csapp.controller("customerDataCtrl", ["$scope", "$csnotify","csNgGridService", "Restangular", function ($scope, $csnotify,csNgGridService, rest) {

    var restApi = rest.all("CustomerDataApi");

    //$scope.systemList = [];
    //$scope.categoryList = [];
    $scope.productsList = [];

    //customerDataFactory.GetSystemCategory().then(function (data) {
    //    var systemList = data.systems;
    //    var categoryList = data.categories;

    //    for (var i = 0; i < systemList.length; i++) {
    //        for (var j = 0; j < categoryList.length; j++) {
    //            var product = {};
    //            product.System = systemList[i];
    //            product.Category = categoryList[j];
    //            $scope.productsList.push(product);
    //        }
    //    }
    //});
    
    restApi.customGET("GetSystemCategory").then(function (data) {
        var systemList = data.systems;
        var categoryList = data.categories;

        for (var i = 0; i < systemList.length; i++) {
            for (var j = 0; j < categoryList.length; j++) {
                var product = {};
                product.System = systemList[i];
                product.Category = categoryList[j];
                $scope.productsList.push(product);
            }
        }
    }, function (data) {
        $csnotify.error(data);
    });


    $scope.selectProductCategory = function (product) {
        var selproduct = JSON.parse(product);
        var system = selproduct.System;
        var category = selproduct.Category;

        $scope.gridReady = false;
        //var gridurl = "api/CustomerDataApi/GetNgGridOptions";
        
        //customerDataFactory.NgGrid(gridurl, system, category).then(function (data) {
        //    $scope.gridOptions = data.gridOptions;
        //    $scope.gridOptions.data = "ngGridModal.data";
        //    $scope.ngGridModal = data.serverNgGridOptions;
        //    $scope.gridReady = true;
        //});
        
        restApi.customGET("GetNgGridOptions", { system: system, category: category }).then(function (data) {
            var csngGridData = csNgGridService.setNgGridData(data);
            $scope.gridOptions = csngGridData.gridOptions;
            $scope.gridOptions.data = "ngGridModal.data";
            $scope.ngGridModal = csngGridData.serverNgGridOptions;
            $scope.gridReady = true;
        }, function (data) {
            $csnotify.error(data);
        });
        

    };


}]);
