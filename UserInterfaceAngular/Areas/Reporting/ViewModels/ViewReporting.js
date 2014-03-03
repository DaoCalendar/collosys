//#region Factory
var alias,sort;
var typeOfView;
var report,reportNew;

(
csapp.factory("fileColumnFactory", ['$http', '$csnotify', '$q', function ($http, $csnotify, $q) {

    var urlColumn = mvcBaseUrl + "api/ReportApi/";

    var getData = function () {
        var deferred = $q.defer();
        $http({
            url: urlColumn + "GetReportNames",
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

    var saveReport = function (report, updateFlag) {
        var deferred = $q.defer();
        debugger;
        $http({
            url: urlColumn + "updateReport",
            method: "POST",
            data: report,
            params: { updateFlag: updateFlag }
        }).success(function (data) {
            $csnotify.success("Columns Loaded Successfully");
            deferred.resolve(data);
        }).error(function (data) {
            $csnotify.error(data.Message);
        });
        return deferred.promise;
    };

    var deleteReport = function (report) {
        var deferred = $q.defer();
        debugger;
        $http({
            url: urlColumn + "deleteReport",
            method: "POST",
            data: report,
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
        getAllReports: getData,
        getFilter: getFilterData,
        deleteReport: deleteReport,
        getUniqueData: getUniqueData
    };
}])
);
//#endregion

(
csapp.controller("fileColumnController", ['$scope', "$csnotify", "$csfactory", 'fileColumnFactory', '$location', function ($scope, $csnotify, $csfactory, fileColumnFactory, $location) {
    'use strict';
    $scope.columnNames = {};
    $scope.flag = false;
    $scope.flagnodata = true;
    $scope.filterflag = false;
    $scope.filterflagMulti = false;
    $scope.filterfieldflag = false;
    $scope.uniqueRecords = [];
    $scope.NoDataMsg = "No data found...";
    $scope.alias = "";
    $scope.filters = [];
    $scope.CompareColFilters = [];
    $scope.cascadeColumns = [];
    $scope.myData = [];
    $scope.filterOptions = {
        filterText: "",
        useExternalFilter: false,
    };

    $scope.searchType = [];
    $scope.reportDetails = [];
    $scope.test = { type: '', propertyName: '', value1: '', value2: '', operatortype: '', operatortypetext: '' };


    $scope.setVisibility = function () {
        $scope.filterflag = false;
        debugger;
        //s.indexOf("oo") != -1
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
            debugger;
            $scope.filterflagMulti = true;
            $scope.getUnique();
        }

        if ($scope.test.operatortype.indexOf("Enum") != -1) {
            debugger;
            $scope.filterflagMulti = false;
            $scope.getUnique();
        }
    }


    $scope.filterOptions = {
        filterText: "",
        useExternalFilter: false,
    };
    //Filter
    $scope.selectSearchType = function () {
        $scope.filterflag = false;
        $scope.test = { type: '', propertyName: '', value1: '', value2: '', operatortype: '', operatortypetext: '' };
        $scope.searchType = $scope.columnNames;// op;
        $scope.test.propertyName = $scope.columnNames.field;// op.field;
        $scope.test.type = $scope.columnNames.type;// op.type;
        //$scope.test.operatortypetext = $scope.columnNames.propertytype;// op.operatortype;
        $scope.cascadeColumns = _.reject(_.where($scope.myDefs, { type: $scope.columnNames.type }), { displayName: $scope.columnNames.displayName });
    };

    $scope.getUnique = function () {
        if (alias) {
            debugger;

            fileColumnFactory.getUniqueData(alias, $scope.test.propertyName)
                .then(function (data) {
                    $scope.uniqueRecords = data;
                });
        }
    }
    $scope.selectColSearchType = function () {
        debugger;
        //$scope.colSearchType = $scope.fieldCompare.value1;// op;
        //$scope.cascadeColumns = _.where($scope.myDefs, { type: $scope.fieldCompare.value1.type });
        $scope.cascadeColumns = _.reject(_.where($scope.myDefs, { type: $scope.columnNames.type }), { displayName: $scope.columnNames.displayName });
        debugger;
    }

    $scope.addFilter = function () {
        //console.log($scope.test.value1);

        if (!$scope.test.value1 || !$scope.test.operatortype || !$scope.test.propertyName) return false;
        debugger;
        var index1 = 0;
        if ($scope.filters.length > 0) {
            index1 = $scope.filters[$scope.filters.length - 1].index + 1;
        }
        //else index = 
        if ($scope.test.operatortype == "In Multiple" || $scope.test.operatortype == "Not In Multiple") {
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
            return true;
        }
        return false;
    };


    //Reset Filter
    $scope.resetFilter = function () {
        debugger;
        for (var i = $scope.filters.length; i >= 0; i--)
            $scope.filters.pop();

        //for (var i = $scope.CompareColFilters.length; i >= 0; i--) $scope.CompareColFilters.pop();

        $scope.test = { type: '', propertyName: '', value1: '', value2: '', operatortype: '', operatortypetext: '' };

        $scope.columnNames = {};


        $scope.reset();
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
        columnDefs: 'myDefs'//,
    };

    //#region DB Operations

    //for selected file columns
    $scope.$watch('pagingOptions', function () { $scope.applyPaging(); }, true);
    $scope.$watch('filterOptions', function () { $scope.applyPaging(); }, true);
    $scope.$watch('sortOptions', function (newVal, oldVal) { if (newVal != oldVal) { $scope.applyPaging(); } }, true);
    $scope.gridColumns = [];

    $scope.getReport = function (aliasName) {
        debugger;
        report = JSON.parse(aliasName);
        reportNew = JSON.parse(aliasName);
        $scope.filters = JSON.parse(report.Filter);
        $scope.gridColumns = eval(report.Columns);
        report.Columns = eval(report.Columns);
        //$scope.CompareColFilters = eval(report.ColumnsFilter);
        $scope.ReportName = report.Name;
        if (report.ColumnsFilter.length > 2) {
            sort = report.ColumnsFilter;
        }
        else sort = $scope.sortOptions.fields[0] + "|" + $scope.sortOptions.directions[0];
        alias = report.TableName;
        $scope.alias = report.TableName;
        $scope.pagingOptions.currentPage = 1;
        $scope.pagingOptions.pageSize = $scope.pagingOptions.pageSizes[0];
        $scope.gridOptions.groupBy(false);
        if (aliasName) {
            fileColumnFactory.getFilter(report.TableName, $scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage, $scope.filters, sort, $scope.CompareColFilters)
                .then(function (data) {
                    $scope.flag = true;
                    $scope.myData = data.DataList;
                    $scope.myDefs = data.Columns;
                    $scope.totalServerItems = data.totalRecords;

                    if ($scope.totalServerItems == 0) $scope.flagnodata = false;
                    else { $scope.flagnodata = true; $scope.flagnotbldata = true; }
                    debugger;

                    var hideArray = _.difference(_.pluck($scope.myDefs, 'field'), $scope.gridColumns)
                    for (var i = 0; i < $scope.myDefs.length; i++) {
                        for (var j = 0; j < hideArray.length; j++) {
                            if ($scope.myDefs[i].field == hideArray[j])
                            { $scope.myDefs[i].visible = false; break; }
                        }
                    }

                });
        }

    };

    $scope.getGridCol = function (col) {
        debugger;
        if (col.visible) {
            // $scope.gridColumns.push(col.field);
            report.Columns.push(col.field);
            $scope.gridColumns = report.Columns;
        }
        else {
            //$scope.findAndRemove($scope.gridColumns, col.field);
            $scope.findAndRemoveCol(report.Columns, col.field);
            $scope.gridColumns = report.Columns;
        }
    }

    $scope.applyFilter = function () {
        var addfilter = $scope.addFilter();
        report.Filter = JSON.stringify($scope.filters);
        sort = $scope.sortOptions.fields[0] + "|" + $scope.sortOptions.directions[0];
        if (alias && $scope.filters.length > 0 && addfilter) $scope.callServer(false);
    };

    $scope.applyPaging = function () {
        debugger;
        $scope.filterflag = false;
        sort = $scope.sortOptions.fields[0] + "|" + $scope.sortOptions.directions[0];
        if (alias) $scope.callServer(false);
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


    $scope.checkDuplicate = function (filters, filterObj) {
        var flag = true;
        $.each(filters, function () {
            if (this.modelName == filterObj.modelName && this.propertyName == filterObj.propertyName && this.propertytype == filterObj.propertytype && this.operatortype == filterObj.operatortype && this.val1 == filterObj.val1) {
                {
                    if (this.val2 == null) flag = false;
                    else if (this.val2 == filterObj.val2) flag = false;
                    else flag = true;
                }
            }
        }
      )
        return flag;
    }

    $scope.findAndRemove = function findAndRemove(array, property, value) {
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
                array.splice(index, 1);
                return false;
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

    $scope.saveReport = function () {
        if ($scope.ReportName.length > 0) {
            report.Filter = JSON.stringify($scope.filters);
            report.Columns = JSON.stringify(report.Columns);
            report.ColumnsFilter = sort;
            //report.ColumnsFilter = JSON.stringify($scope.CompareColFilters);
            fileColumnFactory.saveReport(report, 1)
            .then(function (data) {
                alert(data.replace(/"/g, '').split('|')[1]);
                fileColumnFactory.getAllReports()
                .then(function (data) {
                    $scope.reportDetails = data;
                    $scope.flag = false;
                    console.log(JSON.stringify($location.hash()));
                });
            });
        }

    }

    $scope.saveNewReport = function () {
        if ($scope.SimilarReportName.length > 0) {
            reportNew.Filter = JSON.stringify($scope.filters);
            reportNew.Columns = JSON.stringify(report.Columns);
            reportNew.ColumnsFilter = sort; //JSON.stringify($scope.CompareColFilters);
            reportNew.CreateAction = "";
            reportNew.CreatedBy = "";
            reportNew.CreatedOn = "";
            reportNew.Id = "";
            reportNew.Version = "";
            reportNew.Name = $scope.SimilarReportName;
            $scope.SimilarReportName = undefined;
            debugger;
            fileColumnFactory.saveReport(reportNew, 0)
            .then(function (data) {
                alert(data.replace(/"/g, '').split('|')[1]);
                if (data.replace(/"/g, '').split('|')[0] == 1) {
                    fileColumnFactory.getAllReports()
                     .then(function (data) {
                         $scope.reportDetails = data;
                         $scope.flag = false;
                         console.log(JSON.stringify($location.hash()));
                     });
                }
                else alert(data.replace(/"/g, '').split('|')[1]);
            });
        }

    }

    $scope.deleteReport = function () {
        if ($scope.ReportName.length > 0 && confirm("Are you sure you want delete this report?")) {
            report.Columns = JSON.stringify(report.Columns);
            fileColumnFactory.deleteReport(report)
            .then(function (data) {
                alert(data);
                fileColumnFactory.getAllReports()
                .then(function (data) {
                    $scope.reportDetails = data;
                    $scope.flag = false;
                    console.log(JSON.stringify($location.hash()));
                });
            });
        }

    }


    $scope.call = function (a) {
        if (a.field != "") return true;
    };

    $scope.callServer = function(filterflagmulti) {
        //Common function to call server
        fileColumnFactory.getFilter(alias, $scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage, $scope.filters, sort, $scope.CompareColFilters)
            .then(function(data) {
                $scope.flag = true;
                $scope.myData = data.DataList;
                $scope.myDefs = data.Columns;
                $scope.totalServerItems = data.totalRecords;
                debugger;
                if (parseInt($scope.pagingOptions.pageSize) * parseInt($scope.pagingOptions.currentPage) - parseInt($scope.pagingOptions.pageSize) > parseInt(data.totalRecords)) {
                    $scope.pagingOptions.currentPage = 1;
                }

                if (data.totalRecords == 0) $scope.flagnodata = false;
                else {
                    $scope.flagnodata = true;
                    if (filterflagmulti == true) $scope.filterflagMulti = false;
                }

                $scope.hideColumnsCommon(_.difference(_.pluck($scope.myDefs, 'field'), $scope.gridColumns));

                //var hideArray = _.difference(_.pluck($scope.myDefs, 'field'), $scope.gridColumns)
                //for (var i = 0; i < $scope.myDefs.length; i++) {
                //    for (var j = 0; j < hideArray.length; j++) {
                //        if ($scope.myDefs[i].field == hideArray[j])
                //        { $scope.myDefs[i].visible = false; break; }
                //    }
                //}
            });
    };

    //for getting all file details
    fileColumnFactory.getAllReports().then(function (data) {
        $scope.reportDetails = data;
    });
    //#endregion
}])
);
