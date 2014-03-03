/*global csapp*/

(
csapp.controller("errorDataCtrl", ["$scope", "$csnotify", 'Restangular', function ($scope, $csnotify, rest) {
    "use strict";

    //#region variable initialization

    $scope.gridReady = false;
    $scope.showModel = false;
    $scope.showProgressBar = false;
    $scope.selectedRowIndex = 0;
    $scope.fileDetail = '';

    $scope.fileDetails = [];
    $scope.fileSchedulers = [];
    $scope.dateFileSchedulers = [];
    $scope.col = [];

    $scope.errorMessage = '';
    //#endregion

    //#region db Operations

    var errorapi = rest.all('ErrorDataApi');

    //errorapi.customGET('GetFileDetails').then(function (data) {
    //    $scope.fileDetails = data;
    //});

    //$scope.getFileSchedulers = function (fileDetail) {
    //    errorapi.customGET('GetFileSchedulers', { fileDetailId: fileDetail.Id }).then(function (data) {
    //        $scope.fileSchedulers = data;
    //    });
    //};



    errorapi.customGET('GetFileSchedulers').then(function (data) {
        debugger;
        $scope.fileSchedulers = data;
        var test = _.pluck(data, 'FileDetail');
        $scope.fileDetails = _.uniq(_.pluck(data, 'FileDetail'), 'Id');
    });

    $scope.getDateFileSchedulers = function (fileDetail) {
        $scope.dateFileSchedulers = _.filter($scope.fileSchedulers, function (fs) { return fs.FileDetail.Id == fileDetail.Id; });
    };

    $scope.saveErrorData = function (row, index, validate) {
        debugger;
        var saveData = {
            validate: validate,
            fileAliasName: $scope.fileDetail.AliasName,
            tableName: $scope.fileDetail.ErrorTable,
            data: row
        };

        var savedSuccess = function () {
            debugger;
            if (validate) {
                $scope.serverNgGridOptions.data.splice(index, 1);
            }
            $scope.closeModel();
        };

        var savedError = function (message) {
            $scope.errorMessage = message;
        };

        errorapi.customPOST(saveData, 'Post').then(savedSuccess, savedError);
    };

    $scope.showErrorGrid = function (fileScheduler) {
        $scope.gridReady = false;
        fileScheduler = JSON.parse(fileScheduler);
        debugger;
        errorapi.customGET('GetNgGridOptions', { fileSchedulerId: fileScheduler.Id }).then(function (data) {
            $scope.setNgGridOptions(data);
            $scope.gridReady = true;
        });
    };

    $scope.deleteRow = function (index) {
        debugger;
        var row = $scope.serverNgGridOptions.data[index];

        errorapi.customDELETE('Delete', { tableName: $scope.fileDetail.ErrorTable, id: row.RowId }).then(function () {
            $scope.serverNgGridOptions.data.splice(index, 1);
            $csnotify.success("Error Data Deleted");
        }, function (data) {
            $csnotify.error(data);
        });
    };

    //#endregion

    //#region set Ng-Grid Options

    $scope.setNgGridOptions = function (data) {
        debugger;
        $scope.serverNgGridOptions = data;
        $scope.col = [];

        $scope.col.push({
            displayName: 'Action',
            cellTemplate:
                '<i class="btn icon-edit" data-ng-click="openModel(row.rowIndex)"></i>' +
                '<i class="btn icon-remove" data-ng-click="showDeleteModelPopup(row.rowIndex)"></i>',
            width: 150
        });

        for (var i = 0; i < $scope.serverNgGridOptions.columnDefs.length; i++) {
            $scope.col.push({
                field: $scope.serverNgGridOptions.columnDefs[i].field,
                displayName: $scope.serverNgGridOptions.columnDefs[i].displayName,
                width: $scope.serverNgGridOptions.columnDefs[i].width
            });
        }

        //$scope.gridData = $scope.serverNgGridOptions.data;

        $scope.gridOptions = {
            data: 'serverNgGridOptions.data',
            columnDefs: $scope.col,
            multiSelect: $scope.serverNgGridOptions.multiSelect,
            enableColumnResize: $scope.serverNgGridOptions.enableColumnResize
        };
    };

    //#endregion

    //#region   retry
    $scope.retryErrorData = function (fileScheduler) {

        $scope.showProgressBar = true;
        errorapi.customPOST(fileScheduler, 'RetryErrorRows').then(function () {
            $scope.showProgressBar = false;
            $scope.showErrorGrid($scope.fileDetail);
            $csnotify.success("Retry Done");
        }, function (response) {
            $scope.showProgressBar = false;
            $csnotify.error(response.data);
        });
    };
    //#endregion

    //#region Model

    $scope.openModel = function (index) {
        $scope.selectedRowIndex = index;
        $scope.selectedRow = $scope.serverNgGridOptions.data[index];
        $scope.errorMessage = $scope.selectedRow.ErrorDescription;
        $scope.showModel = true;
    };

    $scope.closeModel = function () {
        $scope.errorMessage = '';
        $scope.showModel = false;
    };

    //#endregion

    //#region delete
    $scope.yesToDelete = function () {
        debugger;
        $scope.deleteRow($scope.selectedRowIndex);
        $scope.showDeleteModel = false;
    };

    $scope.noToDelete = function () {
        debugger;
        $scope.editIndex = -1;
        $scope.showDeleteModel = false;
    };

    $scope.showDeleteModelPopup = function (index) {
        debugger;
        $scope.selectedRowIndex = index;
        $scope.selectedRow = $scope.serverNgGridOptions.data[index];
        $scope.showDeleteModel = true;
    };
    //#endregion
}])   
);