
csapp.factory("fileMappingDataLayer", ["Restangular", "$csnotify", "$csfactory", function (rest, $csnotify, $csfactory) {
    var restApi = rest.all("FileMappingApi");
    var dldata = {};

    var errorDisplay = function (response) {
        $csnotify.error(response.Message);
    };

    var getValueTypes = function () {
        dldata.valueTypes = [];
        restApi.customGET("GetValueTypes")
            .then(function (data) {
                dldata.valueTypes = data;
            }, errorDisplay);
    };

    var getFileDetails = function () {
        restApi.customGET("GetFileDetails")
            .then(function (data) {
                dldata.fileDetails = data;
            }, errorDisplay);
    };

    var getFileMappings = function (id) {

        restApi.customGET("GetFileMappings", { fileDetailId: id })
            .then(function (data) {
                $csnotify.success("All Files loaded Successfully");
                if ($csfactory.isNullOrEmptyArray(data)) {
                    dldata.fileMappings = [];
                    return;
                }
                dldata.fileMappings = data;
                dldata.actualTable = data[0].ActualTable;
                dldata.tempTable = data[0].TempTable;
            }, errorDisplay);
    };

    var getFileColumns = function (id) {

        restApi.customGET("GetFileColumns", { fileDetailId: id })
            .then(function (data) {
                dldata.fileColumns = data;
            }, errorDisplay);
    };

    var saveFileMapping = function (fileMapping, fileDetail) {
        fileMapping.FileDetail = fileDetail;

        return restApi.customPOST(fileMapping, 'SaveMapping')
            .then(function () {
                $csnotify.success("File Updated Successfully");
            }, errorDisplay);
    };

    return {
        fetchValueTypeEnum: getValueTypes,
        GetAllFileDetails: getFileDetails,
        GetFileMappings: getFileMappings,
        GetFileColumns: getFileColumns,
        Save: saveFileMapping,
        dldata: dldata
    };
}]);

csapp.controller("fileMappingViewEditController", [
    "$scope", "FileMapping", "$modalInstance", "fileMappingDataLayer", "$csModels",
    function ($scope, fileMapping, $modalInstance, datalayer, $csModels) {

        (function () {
            //2 file mappings - 1 on scope - mapping edited and another is just params
            $scope.fileMapping = angular.copy(fileMapping.mapping);
            $scope.isReadOnly = fileMapping.mode === 'view';
            $scope.datalayer = datalayer;
            datalayer.GetFileColumns(fileMapping.fileDetail);
        })();


        $scope.fileMappingModel = $csModels.getColumns("FileMapping");


        $scope.close = function () {
            $modalInstance.dismiss();
        };

        $scope.changeValueType = function () {
            if ($scope.fileMapping.ValueType == 'ExcelValue' || $scope.fileMapping.ValueType == 'MappedValue') {
                $scope.fileMapping.DefaultValue = '';
            } else {
                $scope.fileMapping.TempColumn = '';
                $scope.fileMapping.Position = 0;
            }
        };

        $scope.changeFileColumn = function (tempColumnName) {
            $scope.fileMapping.Position = _.find(datalayer.dldata.fileColumns, { 'TempColumnName': tempColumnName }).Position;
            $scope.fileMapping.OutputPosition = _.find(datalayer.dldata.fileColumns, { 'TempColumnName': tempColumnName }).Position;
            $scope.fileMapping.OutputColumnName = _.find(datalayer.dldata.fileColumns, { 'TempColumnName': tempColumnName }).FileColumnName;
        };

        $scope.save = function (filemapping) {
            datalayer.Save(filemapping, fileMapping.fileDetail)
                .then(function (data) {
                    var index = _.findIndex(datalayer.dldata.fileMappings, { 'Id': filemapping.Id });
                    if (index !== -1) datalayer.dldata.fileMappings[index] = filemapping;
                    $modalInstance.close(data);
                });
        };

        (function (mode) {
            switch (mode) {
                //case "add":
                //    $scope.modelTitle = "Add New Mappings";
                //    break;
                case "edit":
                    $scope.modelTitle = "Edit File Mappings";
                    break;
                case "view":
                    $scope.modelTitle = "View File Mappings";
                    break;
                default:
                    throw ("Invalid display mode : " + JSON.stringify(fileMapping));
            }
            $scope.mode = mode;
        })(fileMapping.mode);


    }]);

csapp.controller("fileMappingController", ["$scope", "fileMappingDataLayer", "$modal", "$csModels",
    function ($scope, datalayer, $modal, $csModels) {
    "use strict";

    (function () {
        $scope.datalayer = datalayer;
        datalayer.fetchValueTypeEnum();
        datalayer.GetAllFileDetails();
        datalayer.dldata.actualTable = '';
        $scope.fileMappingModel = $csModels.getColumns("FileMapping");
    })();

    $scope.openEditModalPopup = function (mode, filemapping) {
        $modal.open({
            templateUrl: baseUrl + 'FileUpload/filemapping/file-mapping-edit.html',
            controller: 'fileMappingViewEditController',
            resolve: {
                FileMapping: function () {
                    return {
                        mapping: filemapping,
                        mode: mode,
                        fileDetail: $scope.fileDetail
                    };
                }
            }
        });
    };
}]);