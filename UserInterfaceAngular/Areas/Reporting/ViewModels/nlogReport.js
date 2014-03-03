//#region Factory
var alias;
csapp.factory("nlogFactory", ['$http', '$csnotify', '$q', function ($http, $csnotify, $q) {

    var urlColumn = "api/nlog/";

    var getAllData = function (pageSize, currentPage) {

        var deferred = $q.defer();

        $http({
            url: urlColumn + "Get",
            method: "GET",
            params: {pageSize: pageSize, currentPage: currentPage }
        }).success(function (data) {
            deferred.resolve(data);
            $csnotify.success("Columns Loaded Successfully");
        }).error(function (data) {
            $csnotify.error(data.Message);
        });
        return deferred.promise;
    };


    return {
        getReport: getAllData
    };
}]);

csapp.controller("nlogReport", ['$scope', "$csnotify", "$csfactory", 'nlogFactory', '$location', function ($scope, $csnotify, $csfactory, nlogFactory, $location) {
    'use strict';

    $scope.filters = [];
    $scope.myData = [];
    $scope.filterOptions = {
        filterText: "",
        useExternalFilter: false,
    };

    $scope.pagingOptions = {
        pageSizes: [5, 10, 15], //page Sizes
        pageSize: 5, //Size of Paging data
        currentPage: 1 //what page they are currently on
    };

    $scope.gridOptions = {
        data: 'myData',
        //		jqueryUITheme: false,
        //		jqueryUIDraggable: false,
        //        selectedItems: $scope.mySelections,
        //        showSelectionCheckbox: true,
        //        multiSelect: true,
        showColumnMenu: true,
        //        enableCellSelection: false,
        enableCellEditOnFocus: false,
        //		plugins: [plugins.ngGridLayoutPlugin],
        plugins: [new ngGridCsvExportPlugin(), new ngGridWYSIWYGPlugin()],
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

    //for selected file columns
    $scope.$watch('pagingOptions', function () { $scope.getReport(alias); }, true);
    $scope.$watch('filterOptions', function () { $scope.getReport(alias); }, true);

    $scope.getReport = function () {
        debugger;
            nlogFactory.getReport($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage)
                .then(function (data) {
                    debugger;
                    $scope.myData = data.DataList;
                    $scope.myDefs = data.Columns;
                    $scope.gridOptions = data.GridOptions[0];
                    debugger;
                    $scope.totalServerItems = data.totalRecords;
                    if (!$scope.$$phase) {
                        $scope.$apply();

                    }
                });
    };
    
}]);
