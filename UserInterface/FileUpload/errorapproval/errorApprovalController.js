
csapp.factory("errorApprovalDataLayer", [
    "Restangular", "$csnotify", function (rest, $csnotify) {
        var dldata = {};
        var apireport = rest.all('ApproveEditedErrorDataApi');

        var getFileSchedulers = function () {
            return apireport.customGET('GetFileSchedulers').then(function (data) {
                dldata.fileSchedulers = data;
                dldata.fileDetails = _.uniq(_.pluck(data, 'FileDetail'), 'Id');
            });
        };

        var fetchGrid = function (fileDetail) {
            return apireport.customGET('GetNgGridOptions', { fileDetailId: fileDetail.Id })
            .then(function (data) {
                return data;
            }, function (error) {
                $csnotify.error(error);
            });
        };

        var multiApprove = function (approveRows, status, fileDetail) {

            var saveData = {
                approved: status,
                fileAliasName: fileDetail.AliasName,
                tableName: fileDetail.ErrorTable,
                data: approveRows
            };

            return apireport.customPOST(saveData, 'PostRows')
                .then(function (data) {
                    return data;
                }, function (data) {
                    $csnotify.error(data);
                });
        };

        var singleApprove = function (approveRow, status, fileDetail) { //, index, next

            var saveData = {
                approved: status,
                fileAliasName: fileDetail.AliasName,
                tableName: fileDetail.ErrorTable,
                data: approveRow
            };

            //var saveSuccess = function () {
            //    //$scope.serverNgGridOptions.data.splice(index, 1);
            //    //$scope.closeModel();

            //    //if (next) {
            //    //    $scope.openModel(index);
            //    //}
            //};

            apireport.customPOST(saveData, 'PostRow').then(function (data) {
                return data;
            }, function (data) {
                $csnotify.error(data);
            });
        };

        return {
            dldata: dldata,
            getFileSchedulers: getFileSchedulers,
            fetchGrid: fetchGrid,
            singleApprove: singleApprove,
            multiApprove: multiApprove
        };
    }
]);

csapp.controller("errorApprovalController", ["$scope", "errorApprovalDataLayer", "$modal",
    function ($scope, datalayer, $modal) {
        "use strict";

        (function () {
            datalayer.getFileSchedulers();
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

        $scope.approveRejectAll = function (rows, status) {
            datalayer.multiApprove(rows, status, $scope.fileDetail).then(function () {
                $scope.showGridData($scope.fileDetail);
            });
        };

    }
]);

//var getFileDetails = function () {
//    return apireport.customGET('GetFileDetails')
//        .then(function (data) {
//            dldata.fileDetails = data;
//        }, function (error) {
//            $csnotify.error(error);
//        });
//};
//datalayer.getFileDetails();
//getFileDetails: getFileDetails,


//$scope.openModel = function (index) {
//    $modal.open({

//    });
//};

//$scope.columns.push({
//    displayName: 'Action',
//    cellTemplate: '<i class="btn icon-file-alt" data-ng-click="openModel(row.rowIndex)"></i>',// +
//    width: 70
//});

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

