
csapp.factory("errorDataLayer", ["Restangular", "$csnotify", "$csfactory",
    function (rest, $csnotify, $csfactory) {

        var errorapi = rest.all('ErrorDataApi');
        var dldata = {};

        var getFileSchedulers = function () {
            return errorapi.customGET('GetFileSchedulers').then(function (data) {
                dldata.fileSchedulers = data;
                dldata.fileDetails = _.uniq(_.pluck(data, 'FileDetail'), 'Id');
            });
        };

        var getGridDetails = function (fileScheduler) {
            $csfactory.enableSpinner();
            return errorapi.customGET('GetNgGridOptions', { fileSchedulerId: fileScheduler.Id });
        };

        var deleteRow = function (params) {
            return errorapi.customDELETE('Delete', params).then(function () {
                $csnotify.success("Row Deleted");
            }, function (data) {
                $csnotify.error(data);
            });
        };

        var retry = function (fileScheduler) {
            $csfactory.enableSpinner();
            return errorapi.customPOST(fileScheduler, 'RetryErrorRows')
                .then(function () {
                    $csnotify.success("Retry Done");
                }, function (response) {
                    $csnotify.error(response.data);
                });
        };

        var saveErrorData = function (saveData) {
            return errorapi.customPOST(saveData, 'Post')
                .then(function (data) {
                    return data;
                }, function (message) {
                    return message;
                });
        };

        return {
            dldata: dldata,
            getFileSchedulers: getFileSchedulers,
            saveErrorData: saveErrorData,
            getGridDetails: getGridDetails,
            deleteRow: deleteRow,
            retry: retry
        };
    }
]);

csapp.controller("errorDataEditController", ["$scope", "errorDataLayer", "grid", "$modalInstance",
    function ($scope, datalayer, grid, $modalInstance) {
        $scope.serverNgGridOptions = grid.options;
        $scope.selectedRow = grid.options.data[grid.rowIndex];

        $scope.closeModel = function () {
            $modalInstance.dismiss();
        };

        $scope.save = function (validate) {
            var params = {
                validate: validate,
                fileAliasName: grid.fileDetail.AliasName,
                tableName: grid.fileDetail.ErrorTable,
                data: $scope.selectedRow
            };

            datalayer.saveErrorData(params).then(function () {
                if (validate) grid.options.data.splice(grid.rowIndex, 1);
                $modalInstance.close();
            }, function (message) {
                $scope.errorMessage = message;
            });
        };
    }
]);

csapp.controller("errorDataController", ["$scope", "errorDataLayer", "modalService", "$modal",
    function ($scope, datalayer, deleteModal, $modal) {

        //#region init data
        (function () {
            datalayer.getFileSchedulers();
            $scope.dldata = datalayer.dldata;
        })();

        $scope.getDateFileSchedulers = function (fileDetail) {
            $scope.dateFileSchedulers = _.filter($scope.dldata.fileSchedulers,
                function (fs) { return fs.FileDetail.Id == fileDetail.Id; });
        };
        //#endregion

        //#region grid init
        $scope.setNgGridOptions = function (data) {
            $scope.serverNgGridOptions = data;
            $scope.col = [];

            $scope.col.push({
                displayName: 'Action',
                cellTemplate:
                    '<i class="btn icon-edit" data-ng-click="showEditRowModalPopup(row.rowIndex)"></i>' +
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

            $scope.gridOptions = {
                data: 'serverNgGridOptions.data',
                columnDefs: $scope.col,
                multiSelect: $scope.serverNgGridOptions.multiSelect,
                enableColumnResize: $scope.serverNgGridOptions.enableColumnResize
            };
        };

        $scope.showErrorGrid = function (fileScheduler) {
            $scope.gridReady = false;
            datalayer.getGridDetails(fileScheduler)
                .then(function (data) {
                    $scope.fileScheduler = fileScheduler;
                    $scope.setNgGridOptions(data);
                    $scope.gridReady = true;
                });
        };
        //#endregion

        //#region grid retry/delete
        $scope.deleteRow = function (index) {
            var params = {
                tableName: $scope.fileDetail.ErrorTable,
                id: $scope.serverNgGridOptions.data[index].RowId
            };

            datalayer.deleteRow(params).then(function () {
                $scope.serverNgGridOptions.data.splice(index, 1);
            });
        };

        $scope.showDeleteModelPopup = function (index) {
            var modalOptions = {
                closeButtonText: 'Cancel',
                actionButtonText: 'Delete',
                headerText: 'Delete Row?',
                bodyText: 'Are you sure you want to delete the selected row?'
            };

            deleteModal.showModal({}, modalOptions).then(function () {
                $scope.deleteRow(index);
            });
        };

        $scope.retry = function (fileScheduler) {
            datalayer.retry(fileScheduler).then(function () {
                $scope.showErrorGrid(fileScheduler);
            });
        };
        //#endregion

        //#region grid edit
        $scope.showEditRowModalPopup = function (index) {
            $modal.open({
                templateUrl: baseUrl + 'FileUpload/errorcorrection/error-record.html',
                controller: 'errorDataEditController',
                resolve: {
                    grid: function () {
                        return {
                            options: $scope.serverNgGridOptions,
                            rowIndex: index,
                            fileDetail: $scope.fileDetail
                        };
                    }
                }
            });
        };
        //#endregion
    }
]);

