//#region Factory
var alias, sort;
var typeOfView;

(
csapp.factory("fileColumnFactory", ['$http', '$csnotify', '$q', function ($http, $csnotify, $q) {

    // ReSharper disable UseOfImplicitGlobalInFunctionScope
    var urlColumn = mvcBaseUrl + "api/ReportApi/";
    // ReSharper restore UseOfImplicitGlobalInFunctionScope

    var getData = function () {
        var deferred = $q.defer();
        $http({
            url: urlColumn + "GetNames",
            method: "GET"
        }).success(function (data) {
            deferred.resolve(data);
            $csnotify.success("All Files Loaded");
        }).error(function (data) {
            $csnotify.error(data.Message);
        });
        return deferred.promise;
    };

    var getFilterData = function (aliasName, pageSize, currentPage, filter, sortInfo, colFilter) {
        var deferred = $q.defer();
        $http({
            url: urlColumn + "GetFilter",
            method: "POST",
            data: filter,
            params: { aliasName: aliasName, pageSize: pageSize, currentPage: currentPage, sortInfo: sortInfo, colFilter: JSON.stringify(colFilter) }
        }).success(function (data) {
            $csnotify.success("Columns Loaded Successfully");
            deferred.resolve(data);
        }).error(function (data) {
            $csnotify.error(data.Message);
        });

        return deferred.promise;
    };

    var saveReport = function (aliasName, reportName, filter, columns, colfilter) {
        var deferred = $q.defer();
        $http({
            url: urlColumn + "saveReport",
            method: "POST",
            data: filter,
            params: { colfilter: colfilter, aliasName: aliasName, reportName: reportName, columns: columns }
        }).success(function (data) {
            $csnotify.success("Columns Loaded Successfully");
            deferred.resolve(data);
        }).error(function (data) {
            $csnotify.error(data.Message);
        });
        return deferred.promise;
    };

    var getUniqueData = function (tableName, columnName) {
        var deferred = $q.defer();
        $http({
            url: urlColumn + "GetUnique",
            method: "GET",
            params: { aliasName: tableName, columnName: columnName }
        }).success(function (data) {
            deferred.resolve(data);
            $csnotify.success("All Files Loaded");
        }).error(function (data) {
            $csnotify.error(data.Message);
        });
        return deferred.promise;
    };
    return {
        saveReport: saveReport,
        getAllFiles: getData,
        getFilter: getFilterData,
        getUniqueData: getUniqueData
        //,
    };
}])
);
//#endregion

(
csapp.controller("fileColumnController", ['$scope', "$csnotify", "$csfactory", 'fileColumnFactory', '$location', function ($scope, $csnotify, $csfactory, fileColumnFactory, $location) {
    'use strict';
    // Local Variable settings starts
    $scope.columnNames = {};
    $scope.flag = false;
    $scope.flagmsg = false;
    $scope.flagnodata = true;
    $scope.flagnotbldata = true;
    $scope.filterflag = false;
    $scope.filterflagMulti = false;
    $scope.filterfieldflag = false;
    $scope.uniqueRecords = [];
    $scope.NoDataMsg = "No data found...";
    $scope.filters = [];
    $scope.CompareColFilters = [];
    $scope.cascadeColumns = [];
    $scope.myData = [];
    $scope.gridColumns = [];
    $scope.searchType = [];

    $scope.filterOptions = {
        filterText: "",
        useExternalFilter: false,
    };

    $scope.pagingOptions = {
        pageSizes: [100, 500, 1000], //page Sizes
        pageSize: 100, //Size of Paging data
        currentPage: 1 //what page they are currently on
    };

    $scope.sortOptions = {
        fields: [""],
        directions: [""]
    };

    $scope.test = { type: '', propertyName: '', value1: null, value2: null, operatortype: '', operatortypetext: '' };

    $scope.gridOptions = {
        data: 'myData',
        showColumnMenu: false,
        enableCellEditOnFocus: false,
        showGroupPanel: true,
        enablePaging: true,
        resizable: true,
        showFilter: false,
        showFooter: true,
        sortInfo: $scope.sortOptions,
        totalServerItems: 'totalServerItems',
        filterOptions: $scope.filterOptions,
        pagingOptions: $scope.pagingOptions,
        useExternalSorting: true,
        columnDefs: 'myDefs'
    };

    // Local Variable settings ends 

    $scope.setVisibility = function () {
        $scope.filterflag = false;
        if ($scope.test.operatortype.indexOf("Between") != -1) {
            $scope.filterflag = true;
        }

        if ($scope.test.operatortype.indexOf("Field") != -1) {
            $scope.filterfieldflag = true;
        }
        else if ($scope.test.operatortype.indexOf("Field") == -1) {
            $scope.filterfieldflag = false;
        }

        if ($scope.test.operatortype.indexOf("Multiple") != -1) {
            $scope.filterflagMulti = true;
            $scope.getUnique();
        }

        if ($scope.test.operatortype.indexOf("Enum") != -1) {
            $scope.filterflagMulti = false;
            $scope.getUnique();
        }
    }

    $scope.selectSearchType = function () {

        $scope.filterflag = false;
        $scope.test = { type: '', propertyName: '', value1: null, value2: null, operatortype: '', operatortypetext: '' };
        $scope.searchType = $scope.columnNames;// op;
        $scope.test.propertyName = $scope.columnNames.field;// op.field;
        $scope.test.type = $scope.columnNames.type;// op.type;
        // $scope.test.operatortypetext = $scope.columnNames.propertytype;// op.operatortype;
        $scope.cascadeColumns = _.reject(_.where($scope.myDefs, { type: $scope.columnNames.type }), { displayName: $scope.columnNames.displayName });
    };

    $scope.getUnique = function () {
        if (alias) {
            fileColumnFactory.getUniqueData(alias, $scope.test.propertyName)
                .then(function (data) {
                    $scope.uniqueRecords = data;
                });
        }
    }

    $scope.selectColSearchType = function () {
        // This is used to filter out columns from the value dropdrop
        $scope.cascadeColumns = _.reject(_.where($scope.myDefs, { type: $scope.columnNames.type }), { displayName: $scope.columnNames.displayName });
    }

    $scope.addFilter = function () {
        if (!$scope.test.value1 || !$scope.test.operatortype || !$scope.test.propertyName) return false;
        var index1 = 0;
        if ($scope.filters.length > 0) {
            index1 = $scope.filters[$scope.filters.length - 1].index + 1;
        }
        if ($scope.test.operatortype.indexOf("Multiple") != -1) {
            $scope.test.value1 = $scope.test.value1.toString();
        }

        if ($scope.test.operatortype.indexOf("Field") != -1) {
            $scope.test.value1 = $scope.test.value1.field;
        }

        var filterObject =
            {
                modelName: alias,
                propertyName: $scope.test.propertyName,// $scope.Columns.split('|')[1],
                propertytype: $scope.test.type,
                operatortype: $scope.test.operatortype,
                val1: $scope.test.value1,
                val2: $scope.test.value2,
                index: index1  //$scope.filters.length - 1
            };

        if ($scope.test.operatortype != "" && $scope.checkDuplicate($scope.filters, filterObject)) {
            $scope.filters.push(filterObject);
            $scope.searchType = [];
            $scope.test = { type: '', propertyName: '', value1: null, value2: null, operatortype: '', operatortypetext: '' };
            $scope.columnNames = {};
            return true;
        }
        return false;
    };

    //Reset Filter
    $scope.resetFilter = function () {

        for (var i = $scope.filters.length; i >= 0; i--) $scope.filters.pop();
        $scope.test = { type: '', propertyName: '', value1: '', value2: '', operatortype: '', operatortypetext: '' };
        $scope.columnNames = {};
        $scope.reset();
    };

    $scope.getGridCol = function (col) {

        if (col.visible) {
            $scope.gridColumns.push(col.field);
        }
        else {
            $scope.findAndRemoveCol($scope.gridColumns, col.field);
        }
    }

    $scope.checkDuplicate = function (filters, filterObj) {
        var flag = true;
        $.each(filters, function () {
            if (this.modelName == filterObj.modelName && this.alias == filterObj.alias && this.propertyName == filterObj.propertyName && this.propertytype == filterObj.propertytype && this.operatortype == filterObj.operatortype && this.val1 == filterObj.val1 && this.val2 == filterObj.val2) {
                {
                    flag = false;
                }
            }
        }
      )
        return flag;
    }
    $scope.findAndRemove = function findAndRemove(array, property, value) {
        //Function is used to Remove item from JSON Array, i.e. from $scope.filters in this case
        $.each(array, function (index, result) {
            if (result[property] == value) {
                array.splice(index, 1);
                return false;
            }
        });
    }
    $scope.findAndRemoveCol = function findAndRemove(array, value) {
        $.each(array, function (index, result) {
            if (result == value) {
                //Remove from array
                array.splice(index, 1);
            }
        });
    }

    $scope.hideColumnsCommon = function (hideArray) {
        //This function is used to set visibility of columns
        for (var i = 0; i < $scope.myDefs.length; i++) {
            for (var j = 0; j < hideArray.length; j++) {
                if ($scope.myDefs[i].field == hideArray[j])
                { $scope.myDefs[i].visible = false; break; }
            }
        }
    };

    //#region DB Operations
    $scope.groupresetflag = false;
    //Watch are used to grab Grid events i.e. paging, filter change and sort options
    $scope.$watch('pagingOptions', function () { if (!$scope.groupresetflag) { $scope.applyPaging(); } }, true);
    $scope.$watch('filterOptions', function () { if (!$scope.groupresetflag) { $scope.applyPaging(); } }, true);
    $scope.$watch('sortOptions', function (newVal, oldVal) { if (!$scope.groupresetflag) { if (newVal != oldVal) { $scope.applyPaging(); } } }, true);


    //Default function get called when user selects table name from dropdown list.
    $scope.getReport = function (aliasName) {
        alias = aliasName;
        $scope.pagingOptions.pageSize = $scope.pagingOptions.pageSizes[0];
        $scope.pagingOptions.currentPage = 1;
        sort = $scope.sortOptions.fields[0] + "|" + $scope.sortOptions.directions[0];
        if (aliasName) {
            $scope.groupresetflag = true;
            $scope.gridOptions.groupBy();
            fileColumnFactory.getFilter(aliasName, $scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage, $scope.filters, sort, $scope.CompareColFilters)
                .then(function (data) {
                    $scope.flag = true;
                    $scope.myData = data.DataList;
                    $scope.myDefs = data.Columns;
                    $scope.gridColumns = _.pluck(data.Columns, 'field');
                    $scope.totalServerItems = data.totalRecords;
                    if (data.totalRecords == 0) { $scope.flagnodata = false; $scope.flagnotbldata = false; }
                    else { $scope.flagnodata = true; $scope.flagnotbldata = true; }
                    $scope.groupresetflag = false;
                });
        }

    };

    //#region Operations on data received from server on selection of table
    $scope.applyFilter = function () {
        $scope.filterflag = false;
        sort = $scope.sortOptions.fields[0] + "|" + $scope.sortOptions.directions[0];
        if (alias && $scope.filters.length >= 0 && $scope.addFilter()) $scope.callServer(true);
    };

    $scope.applyPaging = function () {
        $scope.filterflag = false;
        sort = $scope.sortOptions.fields[0] + "|" + $scope.sortOptions.directions[0];
        if (alias) $scope.callServer(false);
    };

    $scope.call = function (a) {
        if (a.field != "") return true;
    };

    $scope.reset = function () {
        $scope.filterflag = false;
        sort = $scope.sortOptions.fields[0] + "|" + $scope.sortOptions.directions[0];
        if (alias) $scope.callServer(false);
    };

    $scope.remove = function (obj) {
        $scope.findAndRemove($scope.filters, 'index', obj.index);
        sort = $scope.sortOptions.fields[0] + "|" + $scope.sortOptions.directions[0];
        if (alias) $scope.callServer(false);
    }

    $scope.callServer = function (filterflagmulti) {
        //Common function to call server
        fileColumnFactory.getFilter(alias, $scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage, $scope.filters, sort, $scope.CompareColFilters)
                .then(function (data) {
                    $scope.flag = true;
                    $scope.myData = data.DataList;
                    $scope.myDefs = data.Columns;
                    $scope.totalServerItems = data.totalRecords;

                    if (parseInt($scope.pagingOptions.pageSize) * parseInt($scope.pagingOptions.currentPage) - parseInt($scope.pagingOptions.pageSize) > parseInt(data.totalRecords))
                    { $scope.pagingOptions.currentPage = 1; }

                    if (data.totalRecords == 0) $scope.flagnodata = false;
                    else {
                        $scope.flagnodata = true;
                        if (filterflagmulti == true) $scope.filterflagMulti = false;
                    }

                    $scope.hideColumnsCommon(_.difference(_.pluck($scope.myDefs, 'field'), $scope.gridColumns));
                });
    }

    $scope.saveReport = function () {
        if ($scope.ReportName.length > 0) {
            fileColumnFactory.saveReport(alias, $scope.ReportName, $scope.filters, JSON.stringify($scope.gridColumns), sort)
            .then(function (data) {
                alert(data.replace(/"/g, ''));
            });
        }

    };
    //#end region Operations on data received from server on selection of table

    fileColumnFactory.getAllFiles().then(function (data) {
        $scope.fileDetails = data;
        console.log(JSON.stringify($location.hash()));
    });
    //#endregion
}])
);

