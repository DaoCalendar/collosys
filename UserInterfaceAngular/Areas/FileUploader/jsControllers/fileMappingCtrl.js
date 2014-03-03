
(
csapp.controller("fileMappingCtrl", ["$scope", "$csnotify", "Restangular", function ($scope, $csnotify, rest) {
    "use strict";

    var restApi = rest.all("FileMappingApi");

    $scope.fileDetails = [];
    $scope.fileDetail = {};
    $scope.fileMappings = [];
    $scope.valueTypes = [];
    $scope.fileColumns = [];
    $scope.actualTable = '';
    $scope.tempTable = '';
    $scope.fileMapping = {};
    $scope.shouldBeOpen = false;
    $scope.selectedIndex = 0;
    $scope.isReadOnly = true;


    restApi.customGET("GetValueTypes").then(function (data) {
        $scope.valueTypes = data;
    }, function (data) {
        $csnotify.error(data);
    });

    restApi.customGET("GetFileDetails").then(function (data) {
        debugger;
        $scope.fileDetails = data;
    }, function (data) {
        $csnotify.error(data);
    });

    $scope.getFileMappings = function (fileDetail) {

        restApi.customGET("GetFileMappings", { fileDetailId: fileDetail.Id }).then(function (data) {
            $scope.fileMappings = data;

            if ($scope.fileMappings.length < 1)
                return;

            $scope.actualTable = $scope.fileMappings[0].ActualTable;
            $scope.tempTable = $scope.fileMappings[0].TempTable;

        }, function (data) {
            $csnotify.error(data);
        });


        restApi.customGET("GetFileColumns", { fileDetailId: fileDetail.Id }).then(function (data) {
            $scope.fileColumns = data;

        }, function (data) {
            $csnotify.error(data);
        });
    };


    $scope.$watch('fileMapping.ValueType', function () {
        debugger;
        if ($scope.fileMapping.ValueType == 'ExcelValue' || $scope.fileMapping.ValueType == 'MappedValue') {
            $scope.fileMapping.DefaultValue = '';
        } else {
            $scope.fileMapping.TempColumn = '';
            $scope.fileMapping.Position = 0;
        }
    });

    $scope.changeFileColumn = function (tempColumnName) {
        debugger;
        $scope.fileMapping.Position = _.find($scope.fileColumns, { 'TempColumnName': tempColumnName }).Position;
        $scope.fileMapping.OutputPosition = _.find($scope.fileColumns, { 'TempColumnName': tempColumnName }).Position;
        $scope.fileMapping.OutputColumnName = _.find($scope.fileColumns, { 'TempColumnName': tempColumnName }).FileColumnName;
    };

    $scope.saveFileMapping = function (fileMapping) {
        debugger;
        fileMapping.FileDetail = $scope.fileDetail;

        restApi.customPOST(fileMapping, 'SaveMapping').then(function (data) {
            $scope.addinLocal(data);
            $csnotify.success("File Column Updated Successfully");
            $scope.closeModel();
        }, function (data) {
            $csnotify.error(data);
        });

    };

    $scope.addinLocal = function (fileMapping) {
        $scope.fileMappings = _.reject($scope.fileMappings, function (mapping) { return mapping.Id == fileMapping.Id; });
        $scope.fileMappings.push(fileMapping);
    };

    $scope.openModel = function (fileMapping, index, isReadOnly) {
        debugger;
        $scope.fileMapping = angular.copy(fileMapping);
        $scope.shouldBeOpen = true;
        $scope.selectedIndex = index;
        $scope.isReadOnly = isReadOnly;
    };

    $scope.closeModel = function () {
        $scope.shouldBeOpen = false;
    };

    $scope.modelOption = {
        backdropFade: true,
        dialogFade: true
    };

}])
);