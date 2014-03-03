(
csapp.controller("allocCtrl", ["$scope", "$csnotify", "Restangular", function ($scope, $csnotify, rest) {
    "use strict";

    var restApi = rest.all("AllocationChange");

    //varibales
    $scope.productList = [];
    $scope.categoryList = ['Liner', 'Writeoff'];
    $scope.Stakeholders = [];
    $scope.AllocType = [];
    $scope.ChangeReasonList = [];
    $scope.StakeholderList = [];

    $scope.AllocListModelList = [];

    $scope.changeAllocationModel = {
        list: [],
        Model2: {}
    };

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

    restApi.customGET('GetChangeReasonList').then(function (data) {
        $scope.ChangeReasonList = data;
    });

    restApi.customGET('GetStakeholders').then(function (data) {
        $scope.StakeholderList = data;
    }, function (data) {

    });

    $scope.changeStackHolder = function (index, stkholderId) {
        $scope.filteredTodos[index].Stakeholder = _.find($scope.filteredTodos[index].ReleatedStakes, { Id: stkholderId });
    };

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

    $scope.SaveChangeInAllocation = function (filterdTodos) {
        debugger;
        $scope.changeAllocationModel.list = $scope.AllocListModelList; //_.filter($scope.AllocListModelList, function (item) { return item.Alloc.ChangeReason != ''; });
        $scope.changeAllocationModel.Model2 = $scope.DataModel2;
        restApi.customPOST($scope.changeAllocationModel, 'ChangeAllocationList').then(function (data) {
            $csnotify.success('Allocation changes');
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