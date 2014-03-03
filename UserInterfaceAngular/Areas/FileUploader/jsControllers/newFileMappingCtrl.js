/*global csapp*/
/// <reference path="../../../Scripts/angular.js" />
/// <reference path="../../../Scripts/underscore.js" />



csapp.controller("newFileMappingCtrl", ["$scope", "csNgGridService", "$csnotify", "Restangular", function ($scope, csNgGridService, $csnotify, rest) {

    //#region variable intialization

    var restApi = rest.all("FileMappingApi");

    $scope.addFileMappings = false;
    $scope.fileDetails = [];
    $scope.fileDetail = '';
    $scope.fileMappings = [];
    $scope.fileColumns = [];

    $scope.valueTypes = ['ComputedValue', 'DefaultValue', 'ExpressionValue', 'ExcelValue'];

    //#endregion

    //#region Db Opertaion

    var filterBarPlugin = {
        init: function (scope, grid) {
            debugger;
            filterBarPlugin.scope = scope;
            filterBarPlugin.grid = grid;
            $scope.$watch(function () {
                debugger;
                var searchQuery = "";
                angular.forEach(filterBarPlugin.scope.columns, function (col) {
                    if (col.visible && col.filterText) {
                        var filterText = (col.filterText.indexOf('*') == 0 ? col.filterText.replace('*', '') : "^" + col.filterText) + ";";
                        searchQuery += col.displayName + ": " + filterText;
                    }
                });
                return searchQuery;
            }, function (searchQuery) {
                filterBarPlugin.scope.$parent.filterText = searchQuery;
                filterBarPlugin.grid.searchProvider.evalFilter();
            });
        },
        scope: undefined,
        grid: undefined,
    };

    restApi.customGET("GetFileDetails").then(function (data) {
        $scope.fileDetails = data;
    }, function (data) {
        $csnotify.error(data);
    });
    
    $scope.saveFileMappings = function () {
        restApi.customPOST($scope.fileMappings, "Post").then(function (data) {
            $csnotify.success("Data Saved");
        }, function (data) {
            $csnotify.error(data);
        });
    };

    //#endregion

    //#region Set File Column Drop Down

    $scope.setFileColumn = function (fileMapping) {
        var fileColumn = _.findWhere($scope.fileColumns,
            {
                TempColumnName: fileMapping.TempColumn,
                Position: fileMapping.Position
            });

        if (fileColumn) {
            fileMapping.DefaultValue = null;
            return fileColumn;
        } else {

            if (fileMapping.DefaultValue === null) {
                fileMapping.DefaultValue = '';
            }

            return null;
        }
    };

    //#endregion

    //#region change event Drop Down

    $scope.changeFileDetail = function (fileDetail) {

        if (!fileDetail) {
            $scope.fileMappings = [];
            return;
        }

        $scope.fileColumns = fileDetail.FileColumns;

        restApi.customGET("GetFileMappings",{ file_detail_id: id }).then(function (data) {
            $scope.fileMappings = data;
        }, function (data) {
            $csnotify.error(data);
        });
    };

    $scope.changeTempColumn = function (fileMapping, fileColumn) {

        if (fileColumn) {
            fileMapping.TempColumn = fileColumn.TempColumnName;
            fileMapping.Position = fileColumn.Position;
        } else {
            fileMapping.TempColumn = '';
            fileMapping.Position = 0;
        }
    };

    $scope.changeValueType = function (fileMapping, selectedfileColumn) {

        if (fileMapping.ValueType === "ExcelValue" || fileMapping.ValueType === "ComputedValue") {
            selectedfileColumn = '';
            fileMapping.DefaultValue = null;
        }

        if (fileMapping.ValueType === "DefaultValue" || fileMapping.ValueType === "ExpressionValue") {
            selectedfileColumn = null;
            fileMapping.DefaultValue = '';
        }

        return selectedfileColumn;
    };

    //#endregion



    $scope.addNew = function () {
        $scope.addFileMappings = true;
    };

}]);


//$scope.searchText = '';

//$scope.searchText = function (renderedColumns) {
//    debugger;
//    var searchQuery = "";
//    angular.forEach(renderedColumns, function (col) {
//        if (col.visible && col.filterText) {
//            var filterText = (col.filterText.indexOf('*') == 0 ? col.filterText.replace('*', '') : "^" + col.filterText) + ";";
//            searchQuery += col.displayName + ": " + filterText;
//        }
//    });

//    debugger;
//    //$scope.searchText = searchQuery;

//    $scope.gridOptions.filterOptions.filterText = searchQuery;
//    //scope.$parent.filterText = searchQuery;
//    //grid.searchProvider.evalFilter();

//    //col in renderedColumns

//};


//fileMappingService.GetNgGridOptions().then(function (gridData) {
//    $scope.setNgGridOptions(gridData);
//    $scope.gridReady = true;
//});

//$scope.setNgGridOptions = function (data) {

//    $scope.serverNgGridOptions = data;
//    $scope.columns = [];

//    $scope.columns.push({
//        headerCellTemplate: '<div class="btn-toolbar" style="width: 70px">' +
//            '<i class="btn icon-plus" data-ng-click="addNew()"></i>' +
//            '</div>',
//        width: 50
//    });

//    for (var i = 0; i < $scope.serverNgGridOptions.columnDefs.length; i++) {
//        $scope.columns.push({
//            field: $scope.serverNgGridOptions.columnDefs[i].field,
//            displayName: $scope.serverNgGridOptions.columnDefs[i].displayName,
//            width: $scope.serverNgGridOptions.columnDefs[i].width
//        });
//    }

//    $scope.gridOptions = {
//        data: 'serverNgGridOptions.data',
//        columnDefs: $scope.columns,
//        multiSelect: $scope.serverNgGridOptions.multiSelect,
//        enableColumnResize: $scope.serverNgGridOptions.enableColumnResize,
//        showSelectionCheckbox: $scope.serverNgGridOptions.showSelectionCheckbox,
//        selectedItems: []
//    };
//};


//$http({
//    url: urlFileMapping + "GetNgGridOptions",
//    method: "GET"
//}).success(function (data) {
//    $scope.setNgGridOptions(data);
//    $scope.gridReady = true;
//}).error(function (data) {
//    Error(data.Message);
//});

//$http({
//    url: urlFileMapping + "GetFileDetails",
//    method: "GET"
//}).success(function (data) {
//    $scope.fileDetails = data;
//}).error(function (data) {
//    Error(data.Message);
//});


//$scope.getFileMappings = function (id) {

//    $http({
//        url: urlFileMapping + "GetFileMappings",
//        method: "GET",
//        params: { file_detail_id: id }
//    }).success(function (data) {
//        $scope.fileMappings = data;
//    }).error(function (data) {
//        Error(data.Message);
//    });
//};

//$http({
//    url: urlFileMapping + "Post",
//    method: "POST",
//    data: $scope.fileMappings
//}).success(function (data) {

//}).error(function (data) {
//    Error(data.Message);
//});