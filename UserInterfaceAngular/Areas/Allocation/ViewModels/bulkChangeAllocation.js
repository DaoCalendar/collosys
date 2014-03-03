(
csapp.controller("bulkCtrl", ["$scope", "$csnotify", "Restangular", function ($scope, $csnotify, rest) {
    'use strict';
    var restApi = rest.all('AllocationBulkChange');

    $scope.productList = [];
    $scope.categoryList = ['Liner', 'Writeoff'];
    $scope.Stakeholders = [];
    $scope.AllocationsList = [];

    $scope.filteredTodos = [],
    $scope.currentPage = 1,
    $scope.numPerPage = 10,
    $scope.maxSize = 5;

    $scope.bulkModelTemp = {};

    //get calls
    restApi.customGET('GetProducts').then(function (data) {
        $scope.productList = data;
    }, function (data) {
        $csnotify.error(data.data.Message);
    });

    restApi.customGET('GetStakeholders').then(function (data) {
        $scope.Stakeholders = data;
    }, function (data) {
        $csnotify.error(data.data.Message);
    });

    $scope.GetAllocations = function (bulkModel) {
        debugger;
        restApi.customPOST(bulkModel, 'GetAllocations').then(function (data) {
            console.log(data);
            assignList(data);
        }, function (data) {
            $csnotify.error(data.data.Message);
        });
    };

    var assignList = function (data) {
        $scope.bulkModelTemp = data;
        debugger;
        if (angular.isDefined(data.RAllocs) && data.RAllocs.length > 0) {
            $scope.AllocationsList = data.RAllocs;
        }
        if (angular.isDefined(data.CAllocs) && data.CAllocs.length > 0) {
            $scope.AllocationsList = data.CAllocs;
        }
        if (angular.isDefined(data.EAllocs) && data.EAllocs.length > 0) {
            $scope.AllocationsList = data.EAllocs;
        }
        $scope.filteredTodos = $scope.AllocationsList.slice(0, 10);
        console.log($scope.filteredTodos);
        return;
    };

    $scope.changeAllocations = function (bulkmodle) {
        debugger;
        bulkmodle.RAllocs = $scope.bulkModelTemp.RAllocs;
        bulkmodle.CAllocs = $scope.bulkModelTemp.CAllocs;
        bulkmodle.EAllocs = $scope.bulkModelTemp.EAllocs;
        restApi.customPOST($scope.BulkAllocationModel, 'ChangeAllocations').then(function (data) {

        }, function (data) {
            $csnotify.error(data.data.Message);
        });
    };

    $scope.numPages = function () {
        return Math.ceil($scope.AllocationsList.length / $scope.numPerPage);
    };

    $scope.$watch('currentPage + numPerPage', function () {
        debugger;
        var begin = (($scope.currentPage - 1) * $scope.numPerPage)
        , end = begin + $scope.numPerPage;

        $scope.filteredTodos = $scope.AllocationsList.slice(begin, end);
    });
}])
);