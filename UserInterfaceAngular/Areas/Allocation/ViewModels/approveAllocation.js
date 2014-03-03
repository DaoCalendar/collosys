(
csapp.controller("allocCtrl", ["$scope", "$csnotify", "Restangular", function ($scope, $csnotify, rest) {
    "use strict";

    var restApi = rest.all("AllocationApproveApi");

    //varibales
    $scope.productList = [];
    $scope.categoryList = ['Liner', 'Writeoff'];
    $scope.Stakeholders = [];
    $scope.AllocType = [];
    $scope.ChangeReasonList = [];
    $scope.StakeholderList = [];

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

    $scope.ApproveAllocation = function () {
        debugger;
        //var saveArray = _.filter($scope.AllocListModelList, function (item) { return item.Alloc.ChangeReason != ''; });
        restApi.customPOST($scope.AllocListModelList, 'ApproveAllocations').then(function (data) {
            $csnotify.success('Allocation Approved');
            clear();
        }, function (data) {

        });
    };

    $scope.$watch('currentPage + numPerPage', function () {
        debugger;
        var begin = (($scope.currentPage - 1) * $scope.numPerPage)
        , end = begin + $scope.numPerPage;

        $scope.filteredTodos = $scope.AllocListModelList.slice(begin, end);
    });

    var clear = function () {
        $scope.DataModel2 = null;
        $scope.Stakeholders = [];
        $scope.AllocType = [];
        $scope.ChangeReasonList = [];
        $scope.StakeholderList = [];
        $scope.AllocListModelList = [];
        $scope.filteredTodos = [];
    };
}])
);