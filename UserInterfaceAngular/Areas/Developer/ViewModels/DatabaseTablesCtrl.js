
(
csapp.controller("databaseTablesCtrl",
    ["$scope", "Restangular", "$csnotify", "$csGrid", "$csConstants", function ($scope, rest, $csnotify, $grid, $csConstants) {
        "use strict";

        var restApi = rest.all("DatabaseTablesApi");

        $scope.selectedTable = '';
        $scope.ckbTables = [];

        var showErrorMessage = function (response) {
            $csnotify.error(response.data);
        };

        restApi.customGETLIST("GetTableNames").then(function (data) {
            $scope.dbTables = data;

            _.forEach($scope.dbTables, function (table) {
                $scope.ckbTables.push({ Name: table, Checked: false });
            });
        }, showErrorMessage);


        $scope.fetchData = function () {
            $scope.$grid = $grid;
            $scope.gridOptions = {};
            debugger;
            restApi.customGET("FetchPageData", { tableName: $scope.selectedTable }).then(function (data) {
                if (angular.isUndefined(data.QueryParams) || angular.isUndefined(data.QueryResult)) { return; }
                $scope.gridOptions = $grid.InitGrid(data.QueryParams); // query params
                $grid.SetData($scope.gridOptions, data.QueryResult); // query result
            }, function (response) {
                $csnotify.error(response.data);
            });
        };

        $scope.downloadTables = function () {
            var selectedTables = _.pluck(_.filter($scope.ckbTables, { Checked: true }), 'Name');

            $scope.isInProcessing = true;
            restApi.customPOST(selectedTables, "DownLoadTables").then(function (data) {
                var downloadpath = $csConstants.MVC_BASE_URL + "Developer/DatabaseTables/Download?fullfilename='" + data + "'";
                window.location = downloadpath;
                $scope.isInProcessing = false;
            }, function (response) {
                $csnotify.error(response.data);
                $scope.isInProcessing = false;
            });
        };

    }])
);