//#region Factory
var alias;
var typeOfView;

(
csapp.factory("fileColumnFactory", ['$http', '$csnotify', '$q', function ($http, $csnotify, $q) {

    var urlColumn = mvcBaseUrl + "api/ClientDataApi/";

    var getData = function () {
        var deferred = $q.defer();
        $http({
            url: urlColumn + "GetNames",
            method: "GET"
        }).success(function (data) {
            deferred.resolve(data);
            $csnotify.success("All Files Loaded");
        }).error(function (data) {
            $csnotify.error(data.Message);
        });
        return deferred.promise;
    };

    var getFilterData = function (aliasName, pageSize, currentPage, date) {
        debugger;
        var deferred = $q.defer();
        $http({
            url: urlColumn + "GetFilter",
            method: "POST",
            params: { aliasName: aliasName, pageSize: pageSize, currentPage: currentPage, date: date }
        }).success(function (data) {
            $csnotify.success("Columns Loaded Successfully");
            deferred.resolve(data);
        }).error(function (data) {
            $csnotify.error(data.Message);
        });

        return deferred.promise;
    };


    return {
        getAllFiles: getData,
        getFilter: getFilterData//,
    };
}])
);
//#endregion

(
csapp.controller("fileColumnController", ['$scope', "$csnotify", "$csfactory", 'fileColumnFactory', '$location', function ($scope, $csnotify, $csfactory, fileColumnFactory, $location) {
    'use strict';
    $scope.columnNames = {};
    //$scope.clientData.date = [];
    $scope.flag = false;
    $scope.filters = [];
    $scope.myData = [];
    $scope.filterOptions = {
        filterText: "",
        useExternalFilter: false,
    };

    $scope.clientData = { "table": "" };
    //$scop.clientData.date = null;
    $scope.SelectedDate = null;
    $scope.pagingOptions = {
        pageSizes: [100, 500, 1000], //page Sizes
        pageSize: 100, //Size of Paging data
        totalServerItems: 'totalServerItemsNo',
        currentPage: 1 //what page they are currently on
    };

    $scope.gridOptions = {
        data: 'myData',
        showColumnMenu: false,
        enableCellEditOnFocus: false,
        showGroupPanel: true,
        enablePaging: true,
        resizable: true,
        showFilter: true,
        showFooter: true,
        totalServerItems: 'totalServerItems',
        filterOptions: $scope.filterOptions,
        pagingOptions: $scope.pagingOptions,
        columnDefs: 'myDefs'//,
    };


    //#region DB Operations

    //for selected file columns
    $scope.$watch('pagingOptions', function () { $scope.getReport(alias); }, true);
    $scope.$watch('filterOptions', function () { $scope.getReport(alias); }, true);

    $scope.gridColumns = [];
    $scope.getGridCol = function(col) {
        debugger;
        if (col.visible) {
            $scope.gridColumns.push(col.field);
        } else {
            $scope.findAndRemove($scope.gridColumns, col.field);
        }
    };

    $scope.resetDate = function () { $scope.SelectedDate = null; }

    $scope.getReport = function () {
        alias = $scope.clientData.table;
        if (alias && $scope.SelectedDate != null) {
            debugger;
            fileColumnFactory.getFilter($scope.clientData.table, $scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage, $scope.SelectedDate)
                .then(function (data) {
                    $scope.flag = true;
                    $scope.myData = data.DataList;
                    $scope.myDefs = data.Columns;
                    $scope.gridColumns = _.pluck(data.Columns, 'field');
                    $scope.totalServerItems = data.totalRecords;
                    //if (!$scope.$$phase) {
                    //    $scope.$apply();
                    //}
                });
        }

    };

    fileColumnFactory.getAllFiles().then(function (data) {
        $scope.fileDetails = data;
        console.log(JSON.stringify($location.hash()));
    });
    //#endregion
}])
);


//$scope.gridOptions.$gridScope.columns[0]