(
csapp.controller("allocCtrl", ["$scope", "$csnotify", "Restangular", function ($scope, $csnotify, rest) {
    "use strict";

    var restApi = rest.all("Allocations");

    //varibales
    $scope.productList = [];
    $scope.categoryList = ['Liner', 'Writeoff'];
    $scope.Stakeholders = [];
    $scope.AllocType = [];

    $scope.AllocListModelList = [];

    $scope.filteredTodos = [],
    $scope.currentPage = 1,
    $scope.numPerPage = 10,
    $scope.maxSize = 5;

    //get calls
    restApi.customGET('GetProducts').then(function (data) {
        $scope.productList = data;
    }, function (data) {
        $csnotify.error(data.data.Message);
    });

    restApi.customGET('GetAllocationStatus').then(function (data) {
        console.log(data);
        $scope.AllocType = data;
    }, function (data) {
        $csnotify.error(data.data.Message);
    });


    $scope.GetAllocList = function (data) {
        restApi.customPOST(data, 'GetAllocations').then(function (data) {
            $scope.AllocListModelList = data;
            $scope.filteredTodos = $scope.AllocListModelList.slice(0, 10);
            console.log(data);
        }, function (data) {
            $csnotify.error(data.data.Message);
        });
    };

    $scope.numPages = function () {
        return Math.ceil($scope.AllocListModelList.length / $scope.numPerPage);
    };

    $scope.$watch('currentPage + numPerPage', function () {

        var begin = (($scope.currentPage - 1) * $scope.numPerPage)
        , end = begin + $scope.numPerPage;

        $scope.filteredTodos = $scope.AllocListModelList.slice(begin, end);
    });
}])
);