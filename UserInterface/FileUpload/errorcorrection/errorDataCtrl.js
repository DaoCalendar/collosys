
csapp.factory("errorDataLayer", ["Restangular",
    function (rest) {

        var errorapi = rest.all('ErrorDataApi');
        var dldata = {};

        var getFileSchedulers = function () {
            errorapi.customGET('GetFileSchedulers').then(function (data) {
                dldata.fileSchedulers = data;
                dldata.fileDetails = _.uniq(_.pluck(data, 'FileDetail'), 'Id');
            });
        };

        var saveErrorData = function (row, index, validate) {
            var saveData = {
                validate: validate,
                fileAliasName: $scope.fileDetail.AliasName,
                tableName: $scope.fileDetail.ErrorTable,
                data: row
            };

            errorapi.customPOST(saveData, 'Post').then(function () {
                if (validate) {
                    $scope.serverNgGridOptions.data.splice(index, 1);
                }
                $scope.closeModel();
            }, function (message) {
                $scope.errorMessage = message;
            });
        };

        return {
            dldata: dldata,
            getFileSchedulers: getFileSchedulers,
            saveErrorData: saveErrorData
        };
    }
]);


csapp.controller("errorDataController",["$scope", function() {

}]);

//csapp.controller("errorDataCtrl", ["$scope", "$csnotify", 'Restangular', "errorDataLayer",
//    function ($scope, $csnotify, rest, datalayer) {
//    "use strict";

//    //#region variable initialization
//    $scope.gridReady = false;
//    $scope.selectedRowIndex = 0;
//    $scope.fileDetail = '';

//    $scope.fileDetails = [];
//    $scope.fileSchedulers = [];
//    $scope.dateFileSchedulers = [];
//    $scope.col = [];

//    $scope.errorMessage = '';
//    //#endregion

//    //#region db Operations

//    $scope.getDateFileSchedulers = function (fileDetail) {
//        $scope.dateFileSchedulers = _.filter($scope.fileSchedulers,
//            function (fs) { return fs.FileDetail.Id == fileDetail.Id; });
//    };

//    $scope.showErrorGrid = function (fileScheduler) {
//        $scope.gridReady = false;
//        fileScheduler = JSON.parse(fileScheduler);
//        errorapi.customGET('GetNgGridOptions', { fileSchedulerId: fileScheduler.Id }).then(function (data) {
//            $scope.setNgGridOptions(data);
//            $scope.gridReady = true;
//        });
//    };

//    $scope.deleteRow = function (index) {
//        var row = $scope.serverNgGridOptions.data[index];

//        errorapi.customDELETE('Delete', { tableName: $scope.fileDetail.ErrorTable, id: row.RowId }).then(function () {
//            $scope.serverNgGridOptions.data.splice(index, 1);
//            $csnotify.success("Error Data Deleted");
//        }, function (data) {
//            $csnotify.error(data);
//        });
//    };

//    //#endregion

//    //#region set Ng-Grid Options

//    $scope.setNgGridOptions = function (data) {
//        $scope.serverNgGridOptions = data;
//        $scope.col = [];

//        $scope.col.push({
//            displayName: 'Action',
//            cellTemplate:
//                '<i class="btn icon-edit" data-ng-click="openModel(row.rowIndex)"></i>' +
//                '<i class="btn icon-remove" data-ng-click="showDeleteModelPopup(row.rowIndex)"></i>',
//            width: 150
//        });

//        for (var i = 0; i < $scope.serverNgGridOptions.columnDefs.length; i++) {
//            $scope.col.push({
//                field: $scope.serverNgGridOptions.columnDefs[i].field,
//                displayName: $scope.serverNgGridOptions.columnDefs[i].displayName,
//                width: $scope.serverNgGridOptions.columnDefs[i].width
//            });
//        }

//        //$scope.gridData = $scope.serverNgGridOptions.data;

//        $scope.gridOptions = {
//            data: 'serverNgGridOptions.data',
//            columnDefs: $scope.col,
//            multiSelect: $scope.serverNgGridOptions.multiSelect,
//            enableColumnResize: $scope.serverNgGridOptions.enableColumnResize
//        };
//    };

//    //#endregion

//    //#region   retry
//    $scope.retryErrorData = function (fileScheduler) {

//        $scope.showProgressBar = true;
//        errorapi.customPOST(fileScheduler, 'RetryErrorRows').then(function () {
//            $scope.showProgressBar = false;
//            $scope.showErrorGrid($scope.fileDetail);
//            $csnotify.success("Retry Done");
//        }, function (response) {
//            $scope.showProgressBar = false;
//            $csnotify.error(response.data);
//        });
//    };
//    //#endregion

//    //#region Model

//    $scope.openModel = function (index) {
//        $scope.selectedRowIndex = index;
//        $scope.selectedRow = $scope.serverNgGridOptions.data[index];
//        $scope.errorMessage = $scope.selectedRow.ErrorDescription;
//        $scope.showModel = true;
//    };

//    $scope.closeModel = function () {
//        $scope.errorMessage = '';
//        $scope.showModel = false;
//    };

//    //#endregion

//    //#region delete
//    $scope.yesToDelete = function () {
//        $scope.deleteRow($scope.selectedRowIndex);
//        $scope.showDeleteModel = false;
//    };

//    $scope.noToDelete = function () {
//        $scope.editIndex = -1;
//        $scope.showDeleteModel = false;
//    };

//    $scope.showDeleteModelPopup = function (index) {
//        $scope.selectedRowIndex = index;
//        $scope.selectedRow = $scope.serverNgGridOptions.data[index];
//        $scope.showDeleteModel = true;
//    };
//    //#endregion
//}]);