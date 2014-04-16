
csapp.factory("errorApprovalDataLayer", [
    "Restangular", "$csnotify", function (rest, $csnotify) {
        var dldata = {};
        var apireport = rest.all('ApproveEditedErrorDataApi');

        var getFileDetails = function () {
            return apireport.customGET('GetFileDetails')
                .then(function (data) {
                    dldata.fileDetails = data;
                }, function (error) {
                    $csnotify.error(error);
                });
        };

        var fetchGrid = function (fileDetail) {
            return apireport.customGET('GetNgGridOptions', { file_detail_id: fileDetail.Id })
            .then(function (data) {
                return data;
            }, function (error) {
                $csnotify.error(error);
            });
        };

        return {
            dldata: dldata,
            getFileDetails: getFileDetails,
            fetchGrid: fetchGrid
        };
    }
]);

csapp.controller("approveErrorDataCtrl", ["$scope", "errorApprovalDataLayer",
    function ($scope, datalayer) {
        "use strict";

        (function () {
            datalayer.getFileDetails();
            $scope.dldata = datalayer.dldata;
        })();

        $scope.showGridData = function (fileDetail) {
            $scope.gridReady = false;
            datalayer.fetchGrid(fileDetail).then(function (data) {
                $scope.setNgGridOptions(data);
                $scope.gridReady = true;
            });
        };

        $scope.setNgGridOptions = function (data) {
            $scope.serverNgGridOptions = data;

            $scope.columns = [];

            $scope.columns.push({
                displayName: 'Action',
                cellTemplate: '<i class="btn icon-file-alt" data-ng-click="openModel(row.rowIndex)"></i>',// +
                width: 70
            });

            for (var i = 0; i < $scope.serverNgGridOptions.columnDefs.length; i++) {
                $scope.columns.push({
                    field: $scope.serverNgGridOptions.columnDefs[i].field,
                    displayName: $scope.serverNgGridOptions.columnDefs[i].displayName,
                    width: $scope.serverNgGridOptions.columnDefs[i].width
                });
            }

            $scope.gridOptions = {
                data: 'serverNgGridOptions.data',
                columnDefs: $scope.columns,
                multiSelect: $scope.serverNgGridOptions.multiSelect,
                enableColumnResize: $scope.serverNgGridOptions.enableColumnResize,
                showSelectionCheckbox: $scope.serverNgGridOptions.showSelectionCheckbox,
                selectedItems: []
            };
        };


    }
]);



//$scope.ApproveRejectSelectedRows = function (approveRows, approved) {

//    var saveData = {
//        approved: approved,
//        fileAliasName: $scope.fileDetail.AliasName,
//        tableName: $scope.fileDetail.ErrorTable,
//        data: approveRows
//    };

//    apireport.costomPOST(saveData, 'PostRows').then(function () {
//        $scope.showGridData($scope.fileDetail);
//    }, function (data) {
//        $csnotify.error(data);
//    });
//};


//$scope.approveRejectErrorData = function (row, approved, index, next) {

//    var saveData = {
//        approved: approved,
//        fileAliasName: $scope.fileDetail.AliasName,
//        tableName: $scope.fileDetail.ErrorTable,
//        data: row
//    };

//    var saveSuccess = function () {
//        $scope.serverNgGridOptions.data.splice(index, 1);
//        $scope.closeModel();

//        if (next) {
//            $scope.openModel(index);
//        }
//    };

//    apireport.costomPOST(saveData, 'PostRow').then(saveSuccess, function (data) {
//        $csnotify.error(data);
//    });
//};


////#region

//$scope.changeSelectedRow = function (index) {
//    $scope.selectedRowIndex = index;
//    $scope.selectedRow = $scope.serverNgGridOptions.data[index];
//};

//$scope.openModel = function (index) {
//    $scope.changeSelectedRow(index);
//    $scope.showModel = true;
//};

//$scope.closeModel = function () {
//    $scope.errorMessage = '';
//    $scope.showModel = false;
//};

////#endregion

