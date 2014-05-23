
csapp.controller("gridSettingsController", [
    "$scope", "gridOptions", "$csGrid", "$modalInstance", "$csModels",
    function ($scope, gridOptions, $grid, $modalInstance, $csModels) {
        $scope.gridOptions = gridOptions;
        $scope.$grid = $grid;
        $scope.gridfield = $csModels.getColumns("Grid");

        $scope.FrequencyParamDaily = [{ key: '9', display: 'At 8 AM' }, { key: '12', display: 'At 12 AM' }, { key: '12', display: 'At 6 PM' }, { key: '24', display: 'At EOD' }];
        $scope.FrequencyParamWeekly = [{ key: 'Sun', display: 'Sun' },
                                        { key: 'Mon', display: 'Mon' },
                                        { key: 'Tue', display: 'Tue' },
                                        { key: 'Wed', display: 'Wed' },
                                        { key: 'Thu', display: 'Thu' },
                                        { key: 'Fri', display: 'Fri' },
                                        { key: 'Sat', display: 'Sat' }];

        $scope.FrequencyParamMonthly = [{ key: '5', display: '5' },
                                      { key: '10', display: '10' },
                                      { key: '15', display: '15' },
                                      { key: '20', display: '20' },
                                      { key: '25', display: '25' },
                                      { key: '31', display: '31' }];


        $scope.close = function () {
            $modalInstance.dismiss();
        };
    }
]);


csapp.factory("$csGrid", ["Restangular", "$timeout", "$csnotify", "$csfactory", "$log", "$csConstants", "$modal",
    function ($restangular, $timeout, $csnotify, $csfactory, $log, $csConstants, $modal) {

        //#region serverData
        var restapi = $restangular.all("GridApi");

        var serverData = {
            currentTimeout: null,
            GetDataParams: function (gridOpts) {
                var queryParams = {
                    GridConfig: {
                        sortInfo: gridOpts.customSort,
                        columnDefs: gridOpts.$gridScope.columns,
                        pagingOptions: gridOpts.pagingOptions,
                        filterOptions: gridOpts.filterOptions,
                        primaryKey: gridOpts.primaryKey
                    },
                    FiltersList: gridOpts.filtersArray,
                    Criteria: gridOpts.QueryParams.Criteria,
                    CriteriaOnType: gridOpts.QueryParams.CriteriaOnType
                };

                return queryParams;
            },
            fixpaging: function (gridOptions) {
                try {
                    if (angular.isUndefined(gridOptions.pagingOptions.currentPage)) return false;
                    if (!angular.isNumber(gridOptions.pagingOptions.currentPage)) return false;
                    if (gridOptions.pagingOptions.currentPage < 1) {
                        gridOptions.pagingOptions.currentPage = 1;
                        $log.debug("$csgrid : paging fixed - negative value.");
                        return true;
                    }
                    var maxPageNo = Math.ceil(gridOptions.TotalRowCount / gridOptions.pagingOptions.pageSize);
                    if ((gridOptions.TotalRowCount !== 0) && (gridOptions.pagingOptions.currentPage > maxPageNo)) {
                        gridOptions.pagingOptions.currentPage = maxPageNo;
                        $log.debug("$csgrid : paging fixed - value > than max.");
                        $log.debug("$csgrid : paging  - " + gridOptions.TotalRowCount + "," + gridOptions.pagingOptions.pageSize
                        + "," + maxPageNo + "," + gridOptions.pagingOptions.currentPage);
                        return true;
                    }
                    return true;
                } catch (e) {
                    return false;
                }
            },
            timedFetchData: function (dataParams, gridOptions) {
                if (serverData.currentTimeout !== null) {
                    $timeout.cancel(serverData.currentTimeout);
                    $log.info("$csgrid : timeout has been cancelled");
                }
                serverData.currentTimeout = $timeout(function () {
                    $log.debug("$csgrid: fetching data from server => ");
                    $log.debug(dataParams);
                    serverData.fixpaging(gridOptions);

                    restapi.customPOST(dataParams, "FetchGridData")
                        .then(
                            function (data) {
                                $log.debug("$csgrid: data fetched : count " + data.PageData.length);
                                gridOptions.PageData = data.PageData;
                                gridOptions.TotalRowCount = data.TotalRowCount;
                            },
                            function (response) {
                                $csnotify.error("Failed to fetch data." + response.data.Message);
                            }
                        );
                }, 500);
            },
            GetGridData: function (gridOptions) {
                if (angular.isUndefined(gridOptions)) return;
                var dataParams = serverData.GetDataParams(gridOptions);
                serverData.timedFetchData(dataParams, gridOptions);
            },
            GetEnumValues: function (gridOptions) {
                var params = {
                    type: gridOptions.QueryParams.CriteriaOnType,
                    property: gridOptions.customFilter.selectedField
                };
                restapi.customGETLIST("GetEnumValues", params)
                    .then(function (data) {
                        gridOptions.customFilter.filterValueList = data;
                    }, function () {
                        $csnotify.error("Could not find property on type.");
                        $log.error("$csgrid : Type '" + params.type + "' does not contain property '" + params.property + "'.");
                    });
                return;
            }
        };
        //#endregion

        //#region plugins
        var serverSideDataPlugin = {
            init: function (scope, grid) {
                $log.info("$csgrid: serverSideDataPlugin initialized");
                serverSideDataPlugin.scope = scope;
                serverSideDataPlugin.grid = grid;

                scope.$watch('gridOptions.pagingOptions', function (newVal, oldVal) {
                    if (newVal !== oldVal) {
                        $log.debug("$csgrid: paging options changed ");
                        serverData.GetGridData(scope.gridOptions);
                    }
                }, true);

            },
            scope: undefined,
            grid: undefined
        };

        //#endregion

        //#region init-grid
        var initGrid = function (queryParams, gridOptions) {
            var opts = setDefaultGridOptions(queryParams, gridOptions);
            setColumnDefs(opts, queryParams.GridConfig.columnDefs);
            opts.QueryParams = queryParams;
            opts.showGrid = true;
            return opts;
        };

        var setDefaultGridOptions = function (queryParams, gridOptions) {
            $log.info("$csgrid : initialized grid with default options.");
            if (angular.isUndefined(gridOptions)) gridOptions = {};
            var gridOpts = queryParams.GridConfig;
            var fromServer = angular.isDefined(gridOpts);

            // initial data
            gridOptions.data = 'gridOptions.PageData';
            gridOptions.PageData = [];
            gridOptions.totalServerItems = 'gridOptions.TotalRowCount';
            gridOptions.TotalRowCount = 0;
            gridOptions.columnDefs = 'gridOptions.Columns';
            gridOptions.Columns = [];

            // row selection using checkbox
            gridOptions.multiSelect = fromServer ? angular.copy(gridOpts.multiSelect) : true;
            gridOptions.enableRowSelection = fromServer ? angular.copy(gridOpts.enableRowSelection) : true;
            gridOptions.showSelectionCheckbox = fromServer ? angular.copy(gridOpts.showSelectionCheckbox) : true;
            gridOptions.pinSelectionCheckbox = true;
            gridOptions.selectWithCheckboxOnly = false;
            if (fromServer) gridOptions.primaryKey = angular.copy(gridOpts.primaryKey);

            //grouping
            gridOptions.groupsCollapsedByDefault = true;

            //column  option
            gridOptions.enableColumnResize = true;
            gridOptions.enableColumnReordering = true;
            gridOptions.enablePinning = true;
            gridOptions.showColumnMenu = false;

            // paging
            gridOptions.showFooter = true;
            gridOptions.enablePaging = true;
            gridOptions.pageOptions = fromServer ? angular.copy(gridOpts.pagingOptions) : {
                pageSizes: [20, 50, 100, 200, 500, 1000],
                pageSize: 20,
                currentPage: 1,
                totalServerItems: 0
            };
            gridOptions.pagingOptions = gridOptions.pageOptions;

            // filtering        
            gridOptions.showFilter = false;
            gridOptions.plugins = [serverSideDataPlugin]; //filterBarPlugin, 
            //gridOptions.headerRowHeight = fromServer ? gridOpts.headerRowHeight : 30;
            gridOptions.filtering = fromServer ? angular.copy(gridOpts.filterOptions) : {
                useExternalFilter: true,
                filterText: ""
            };
            gridOptions.filterOptions = gridOptions.filtering;
            // new filter
            gridOptions.customFilter = {};
            gridOptions.filtersArray = [];

            //sorting
            gridOptions.useExternalSorting = true;
            gridOptions.customSort = fromServer ? angular.copy(gridOpts.sortInfo) : {
                columns: [],
                fields: [],
                directions: []
            };
            // BELOW IS A HACK $$$$
            // as per docs, sortInfo = customSort, 
            // but for implementing multiple sorting we are removed the reference
            gridOptions.sortInfo = {
                columns: [],
                fields: [],
                directions: []
            };

            return gridOptions;
        };

        var setColumnDefs = function (gridOptions, columns) {
            if (!angular.isArray(columns) || columns.length < 1) {
                return;
            }
            var myheadertemplate = "<div class=\"ngHeaderSortColumn {{col.headerClass}}\" ng-class=\"{ 'ngSorted': !noSortVisible }\">" +
                                    "    <div ng-class=\"colt{{col.index}}\" class=\"ngHeaderText\">{{col.displayName}}</div>" +
                                    //"    <div class=\"ngSettingIcon\" ng-click=\"showColumnMenu(col)\"> </div>" +
                                    "    <div class=\"ngSortButtonDown\" ng-show=\"col.showSortButtonDown()\"></div>" +
                                    "    <div class=\"ngSortButtonUp\" ng-show=\"col.showSortButtonUp()\"></div>" +
                                    "    <div class=\"ngSortPriority\">{{col.sortPriority}}</div>" +
                                    "    <div ng-class=\"{ ngPinnedIcon: col.pinned }\"></div>" + //, ngUnPinnedIcon: !col.pinned - ng-click=\"togglePin(col)\" 
                                    "</div>" +
                "<div ng-show=\"col.resizable\" class=\"ngHeaderGrip\" ng-click=\"col.gripClick($event)\" ng-mousedown=\"col.gripOnMouseDown($event)\"></div>";

            $log.info("$csgrid : initializing column details.");
            for (var i = 0; i < columns.length; i++) {
                var col = {};
                col.minWidth = 150;
                col.maxWidth = 1000;
                col.displayName = columns[i].displayName;
                col.field = columns[i].field;
                col.width = columns[i].width;
                col.cellFilter = columns[i].cellFilter;
                col.cellClass = columns[i].cellClass;
                col.headerCellTemplate = myheadertemplate;
                col.cellType = columns[i].cellType;
                //col.width = "auto";
                gridOptions.Columns.push(col);
            }
            $log.info("$csgrid : total column count : " + gridOptions.Columns.length);

            return;
        };
        //#endregion

        //#region freeze-sort-hide-rename-filter
        var freezeColumn = function (gridOptions, col) {
            $log.debug("$csgrid : freeze column : " + col.field);
            if (col.visible === false) return;
            gridOptions.$gridScope.togglePin(col);
        };

        var showHideColumn = function (gridOptions, col) {
            $log.debug("$csgrid : show/hide column : " + col.field);
            if (col.pinned === true) return;
            col.visible = !col.visible;
        };

        var changeColumnPosition = function (gridOptions, index, direction) {
            if (direction !== "UP" && direction !== "DOWN") return;
            if ((index === 1) && (direction === "UP")) return;
            if (((index + 1) === gridOptions.$gridScope.columns.length) && (direction === "DOWN")) return;

            // swap columns
            var newindex = (direction === "UP") ? (index - 1) : (index + 1);
            $log.debug("$csgrid : moving column " + gridOptions.$gridScope.columns[index].field + " "
                + direction + ", from postion " + index + " to " + newindex);
            var temp = gridOptions.$gridScope.columns[newindex];
            gridOptions.$gridScope.columns[newindex] = gridOptions.$gridScope.columns[index];
            gridOptions.$gridScope.columns[index] = temp;
            //swap index as well
            var tempindex = gridOptions.$gridScope.columns[newindex].index;
            gridOptions.$gridScope.columns[newindex].index = gridOptions.$gridScope.columns[index].index;
            gridOptions.$gridScope.columns[index].index = tempindex;

            $log.debug("$csgrid : moved column : " + gridOptions.$gridScope.columns[newindex].field + " " + direction);

            //dummy data swap to refresh the page
            var tempdata = gridOptions.PageData;
            gridOptions.PageData = [];
            gridOptions.PageData = tempdata;
        };

        var sortingHelper = {
            removeSort: function (gridOptions, fieldname) {
                var index = _.indexOf(gridOptions.customSort.fields, fieldname);
                $log.debug("$csgrid : sorting : removing sorting for : " + fieldname);
                gridOptions.customSort.columns[index].sortDirection = '';
                gridOptions.customSort.columns.splice(index, 1);
                gridOptions.customSort.fields.splice(index, 1);
                gridOptions.customSort.directions.splice(index, 1);
                // fetch data
                serverData.GetGridData(gridOptions);
                return;
            },
            toggleSort: function (gridOptions, fieldname, direction) {
                if ($csfactory.isNullOrEmptyArray(gridOptions.$gridScope.columns)
                    || angular.isUndefined(fieldname) || angular.isUndefined(gridOptions.customSort)
                    || angular.isUndefined(gridOptions.customSort.fields)) {
                    return;
                }
                if ($csfactory.isNullOrEmptyString(direction)) direction = "asc";
                direction = (direction !== "asc" && direction !== "desc") ? "asc" : direction;

                var index = _.indexOf(gridOptions.customSort.fields, fieldname);
                if (index === -1) {
                    $log.debug("$csgrid : sorting : added sorting for : " + fieldname);
                    var col = _.findWhere(gridOptions.$gridScope.columns, { 'field': fieldname });
                    if (angular.isUndefined(col)) {
                        $log.error("$csgrid : invalid column/field name : " + fieldname);
                        return;
                    }
                    col.sortDirection = direction;
                    gridOptions.customSort.columns.push(col);
                    gridOptions.customSort.fields.push(fieldname);
                    gridOptions.customSort.directions.push(direction);
                } else {
                    $log.debug("$csgrid : sorting : toggle sorting for : " + fieldname);
                    gridOptions.customSort.directions[index] = (gridOptions.customSort.directions[index] === "asc" ? "desc" : "asc");
                    gridOptions.customSort.columns[index].sortDirection = (gridOptions.customSort.directions[index] === "asc" ? "desc" : "asc");
                }
                // fetch data
                serverData.GetGridData(gridOptions);
                return;
            },
            showDirection: function (gridOptions, fieldname, direction) {
                if ($csfactory.isNullOrEmptyArray(gridOptions.$gridScope.columns)
                    || angular.isUndefined(gridOptions.customSort)) {
                    //$log.error("$csgrid: looks like the grid is not initialized yet.");
                    return false;
                }
                if ($csfactory.isNullOrEmptyString(fieldname)) {
                    $log.error("$csgrid: Please specify valid fieldname & direction");
                    return false;
                }
                if (direction !== "asc" && direction !== "desc") direction = '';

                //$log.debug("$csgrid: checking display for field : " + fieldname);
                switch (direction) {
                    case "asc":
                        if ($csfactory.isNullOrEmptyArray(gridOptions.customSort.fields)) { return true; }
                        var indexa = _.indexOf(gridOptions.customSort.fields, fieldname);
                        return gridOptions.customSort.directions[indexa] !== "asc";
                    case "desc":
                        if ($csfactory.isNullOrEmptyArray(gridOptions.customSort.fields)) { return true; }
                        var indexb = _.indexOf(gridOptions.customSort.fields, fieldname);
                        return gridOptions.customSort.directions[indexb] !== "desc";
                    default:
                        if ($csfactory.isNullOrEmptyArray(gridOptions.customSort.fields)) { return false; }
                        return _.indexOf(gridOptions.customSort.fields, fieldname) !== -1;
                }
            }
        };

        var filteringHelper = {
            getFilterDetails: function (gridOptions, type, code) {
                return _.find(gridOptions.allFiltersList, { 'type': type, 'code': code });
            },
            populateAllFilters: function (gridOptions) {
                if (angular.isDefined(gridOptions.allFiltersList)
                    && !$csfactory.isNullOrEmptyArray(gridOptions.allFiltersList)) {
                    return gridOptions.allFiltersList;
                }
                gridOptions.allFiltersList = [];
                var opid = 0;
                var equal = { id: ++opid, code: "Equal", text: "Equal", operator: "=", type: "Value" };
                gridOptions.allFiltersList.push(equal);
                var notequal = { id: ++opid, code: "NotEqual", text: "NotEqual", operator: "!=", type: "Value" };
                gridOptions.allFiltersList.push(notequal);
                var lessthan = { id: ++opid, code: "LessThan", text: "LessThan", operator: "<", type: "Value" };
                gridOptions.allFiltersList.push(lessthan);
                var lessthanequal = { id: ++opid, code: "LessThanEqual", text: "LessOrEqual", operator: "<=", type: "Value" };
                gridOptions.allFiltersList.push(lessthanequal);
                var greaterthan = { id: ++opid, code: "GreaterThan", text: "GreaterThan", operator: ">", type: "Value" };
                gridOptions.allFiltersList.push(greaterthan);
                var greaterthanequal = { id: ++opid, code: "GreaterThanEqual", text: "GreaterOrEqual", operator: ">=", type: "Value" };
                gridOptions.allFiltersList.push(greaterthanequal);
                var beingswith = { id: ++opid, code: "BeginsWith", text: "StartWith", operator: "SW", type: "Value" };
                gridOptions.allFiltersList.push(beingswith);
                var endswith = { id: ++opid, code: "EndsWith", text: "EndWith", operator: "EW", type: "Value" };
                gridOptions.allFiltersList.push(endswith);
                var contains = { id: ++opid, code: "Contains", text: "Contains", operator: "CT", type: "Value" };
                gridOptions.allFiltersList.push(contains);
                var notcontains = { id: ++opid, code: "NotContains", text: "NotContains", operator: "!CT", type: "Value" };
                gridOptions.allFiltersList.push(notcontains);
                var isInList = { id: ++opid, code: "IsInList", text: "IsInList", operator: "LIST", type: "Value" };
                gridOptions.allFiltersList.push(isInList);
                var notInList = { id: ++opid, code: "NotInList", text: "NotInList", operator: "!LIST", type: "Value" };
                gridOptions.allFiltersList.push(notInList);
                //field
                var fieldEqual = { id: ++opid, code: "Equal", text: "Equal", operator: "=", type: "Field" };
                gridOptions.allFiltersList.push(fieldEqual);
                var fieldNotequal = { id: ++opid, code: "NotEqual", text: "NotEqual", operator: "!=", type: "Field" };
                gridOptions.allFiltersList.push(fieldNotequal);
                var fieldLessthan = { id: ++opid, code: "LessThan", text: "LessThan", operator: "<", type: "Field" };
                gridOptions.allFiltersList.push(fieldLessthan);
                var fieldLessthanequal = { id: ++opid, code: "LessThanEqual", text: "LessOrEqual", operator: "<=", type: "Field" };
                gridOptions.allFiltersList.push(fieldLessthanequal);
                var fieldGreaterthan = { id: ++opid, code: "GreaterThan", text: "GreaterThan", operator: ">", type: "Field" };
                gridOptions.allFiltersList.push(fieldGreaterthan);
                var fieldGreaterthanequal = { id: ++opid, code: "GreaterThanEqual", text: "GreaterOrEqual", operator: ">=", type: "Field" };
                gridOptions.allFiltersList.push(fieldGreaterthanequal);
                //var fieldContains = { id: "17", code: "Contains", text: "Contains", operator: "CT", type: "Field" };
                //gridOptions.allFiltersList.push(fieldContains);
                //var fieldNotcontains = { id: "18", code: "NotContains", text: "NotContains", operator: "!CT", type: "Field" };
                //gridOptions.allFiltersList.push(fieldNotcontains);
                // date
                var relationDateToday = { id: ++opid, code: "Today", text: "Today", operator: "Today", type: "Runtime" };
                gridOptions.allFiltersList.push(relationDateToday);
                var relationDateYesterday = { id: ++opid, code: "Tomorrow", text: "Tomorrow", operator: "Tomorrow", type: "Runtime" };
                gridOptions.allFiltersList.push(relationDateYesterday);
                var relationDateTomorrow = { id: ++opid, code: "Yesterday", text: "Yesterday", operator: "Yesterday", type: "Runtime" };
                gridOptions.allFiltersList.push(relationDateTomorrow);
                var relationDateCWeek = { id: ++opid, code: "CurrentWeek", text: "CurrentWeek", operator: "CurrentWeek", type: "Runtime" };
                gridOptions.allFiltersList.push(relationDateCWeek);
                var relationDateCMonth = { id: ++opid, code: "CurrentMonth", text: "CurrentMonth", operator: "CurrentMonth", type: "Runtime" };
                gridOptions.allFiltersList.push(relationDateCMonth);
                var relationDateCQuarter = { id: ++opid, code: "CurrentQuarter", text: "CurrentQuarter", operator: "CurrentQuarter", type: "Runtime" };
                gridOptions.allFiltersList.push(relationDateCQuarter);
                var relationDateCYear = { id: ++opid, code: "CurrentYear", text: "CurrentYear", operator: "CurrentYear", type: "Runtime" };
                gridOptions.allFiltersList.push(relationDateCYear);
                var relationDateLWeek = { id: ++opid, code: "LastWeek", text: "LastWeek", operator: "LastWeek", type: "Runtime" };
                gridOptions.allFiltersList.push(relationDateLWeek);
                var relationDateLMonth = { id: ++opid, code: "LastMonth", text: "LastMonth", operator: "LastMonth", type: "Runtime" };
                gridOptions.allFiltersList.push(relationDateLMonth);
                var relationDateLQuarter = { id: ++opid, code: "LastQuarter", text: "LastQuarter", operator: "LastQuarter", type: "Runtime" };
                gridOptions.allFiltersList.push(relationDateLQuarter);
                var relationDateLYear = { id: ++opid, code: "LastYear", text: "LastYear", operator: "LastYear", type: "Runtime" };
                gridOptions.allFiltersList.push(relationDateLYear);
                var relationDateNWeek = { id: ++opid, code: "NextWeek", text: "NextWeek", operator: "NextWeek", type: "Runtime" };
                gridOptions.allFiltersList.push(relationDateNWeek);
                var relationDateNMonth = { id: ++opid, code: "NextMonth", text: "NextMonth", operator: "NextMonth", type: "Runtime" };
                gridOptions.allFiltersList.push(relationDateNMonth);
                var relationDateNQuarter = { id: ++opid, code: "NextQuarter", text: "NextQuarter", operator: "NextQuarter", type: "Runtime" };
                gridOptions.allFiltersList.push(relationDateNQuarter);
                var relationDateNYear = { id: ++opid, code: "NextYear", text: "NextYear", operator: "NextYear", type: "Runtime" };
                gridOptions.allFiltersList.push(relationDateNYear);

                return gridOptions.allFiltersList;
            },
            getSelectedFieldType: function (gridOptions, fieldName) {
                if ($csfactory.isNullOrEmptyString(fieldName)) {
                    gridOptions.customFilter.selectedFieldType = '';
                    return gridOptions.customFilter.selectedFieldType;
                }
                if (angular.isUndefined(gridOptions.Columns)) {
                    gridOptions.customFilter.selectedFieldType = '';
                } else {
                    var col = _.filter(gridOptions.Columns, { 'field': fieldName });
                    if ($csfactory.isNullOrEmptyArray(col)) {
                        gridOptions.customFilter.selectedFieldType = '';
                    } else {
                        gridOptions.customFilter.selectedFieldType = col[0].cellType;
                    }
                }
                $log.debug("$csgrid : field " + fieldName + " is of type " + gridOptions.customFilter.selectedFieldType);
                return gridOptions.customFilter.selectedFieldType;
            },
            populateFilterList: function (gridOptions, fieldType) {
                gridOptions.customFilter.validFilterList = [];
                filteringHelper.populateAllFilters(gridOptions);
                switch (fieldType) {
                    case "Text":
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'Equal'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'NotEqual'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'BeginsWith'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'EndsWith'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'Contains'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'NotContains'));
                        break;
                    case "Number":
                    case "Amount":
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'Equal'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'NotEqual'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'LessThan'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'LessThanEqual'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'GreaterThan'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'GreaterThanEqual'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Field', 'Equal'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Field', 'NotEqual'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Field', 'LessThan'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Field', 'LessThanEqual'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Field', 'GreaterThan'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Field', 'GreaterThanEqual'));
                        break;
                    case "Date":
                    case "DateTime":
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'Equal'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'NotEqual'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'LessThan'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'LessThanEqual'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'GreaterThan'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'GreaterThanEqual'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Field', 'Equal'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Field', 'NotEqual'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Field', 'LessThan'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Field', 'LessThanEqual'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Field', 'GreaterThan'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Field', 'GreaterThanEqual'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Runtime', 'Today'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Runtime', 'Tomorrow'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Runtime', 'Yesterday'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Runtime', 'CurrentWeek'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Runtime', 'CurrentMonth'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Runtime', 'CurrentQuarter'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Runtime', 'CurrentYear'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Runtime', 'LastWeek'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Runtime', 'LastMonth'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Runtime', 'LastQuarter'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Runtime', 'LastYear'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Runtime', 'NextWeek'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Runtime', 'NextMonth'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Runtime', 'NextQuarter'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Runtime', 'NextYear'));
                        break;
                    case "Bool":
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'Equal'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'NotEqual'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Field', 'Equal'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Field', 'NotEqual'));
                        break;
                    case "Enum":
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'Equal'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'NotEqual'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'IsInList'));
                        gridOptions.customFilter.validFilterList.push(filteringHelper.getFilterDetails(gridOptions, 'Value', 'NotInList'));
                        break;
                    default:
                        break;
                }
                $log.debug("$csgrid : applicable filters count is : " + gridOptions.customFilter.validFilterList.length);
            },
            filterChanged: function (gridOptions) {
                if (gridOptions.customFilter.selectedFilter.type === 'Field') {
                    var cols = _.where(gridOptions.Columns, { 'cellType': gridOptions.customFilter.selectedFieldType });
                    gridOptions.customFilter.relevantFields = _.sortBy(_.pluck(cols, 'field'), function (name) { return name; });
                    var index = _.indexOf(gridOptions.customFilter.relevantFields, gridOptions.customFilter.selectedField);
                    gridOptions.customFilter.relevantFields.splice(index, 1);
                    return;
                }
                if ((gridOptions.customFilter.selectedFilter.type === 'Value') && (gridOptions.customFilter.selectedFieldType === 'Enum')) {
                    serverData.GetEnumValues(gridOptions);
                }
            },
            ResetFilter: function (gridOptions, fieldName) {
                gridOptions.customFilter = {};
                gridOptions.customFilter.selectedField = fieldName;
                filteringHelper.getSelectedFieldType(gridOptions, fieldName);
                filteringHelper.populateFilterList(gridOptions, gridOptions.customFilter.selectedFieldType);
                resetSelectedRow(gridOptions);
            },
            AddFilter: function (gridOptions) {
                if ($csfactory.isNullOrEmptyArray(gridOptions.filtersArray)) {
                    gridOptions.filtersArray = [];
                }
                var filter = {
                    FieldName: gridOptions.customFilter.selectedField,
                    FieldType: gridOptions.customFilter.selectedFieldType,
                    Operator: gridOptions.customFilter.selectedFilter.code,
                    FilterGroup: gridOptions.customFilter.selectedFilter.type,
                    FilterValue: gridOptions.customFilter.filterValue,
                    FilterValueList: gridOptions.customFilter.filterValues
                };
                if (filter.FieldType === "Enum" && !angular.isArray(filter.FilterValueList)) {
                    filter.FilterValue = filter.FilterValueList;
                    filter.FilterValueList = [];
                }
                gridOptions.customFilter = {};
                gridOptions.filtersArray.push(filter);
                resetSelectedRow(gridOptions);
                // fetch data
                serverData.GetGridData(gridOptions);
            },
            RemoveFilter: function (gridOptions, index) {
                if ($csfactory.isNullOrEmptyArray(gridOptions.filtersArray) || index < 0) {
                    return;
                }
                gridOptions.filtersArray.splice(index, 1);
                resetSelectedRow(gridOptions);
                // fetch data
                serverData.GetGridData(gridOptions);
            }
        };
        //#endregion

        //#region random-helpers
        var helpers = {
            showSettings: function (gridOptions) {
                helpers.showModal(gridOptions);
                gridOptions.totalSelectedStake = false;
                gridOptions.SelectedStakeholderIds = [];
                gridOptions.Reporting.Params.Send2Hierarchy = false;
                gridOptions.selectAll = false;
                $log.info("$csgrid : showing settings.");
            },
            showModal: function (gridOptions) {
                $modal.open({
                    templateUrl: baseUrl + 'Generic/csgrid/grid-settings.html',
                    controller: 'gridSettingsController',
                    resolve: {
                        gridOptions: function () {
                            return gridOptions;
                        }
                    }
                });
            },
        };

        var setDataFromQueryResult = function (gridOptions, queryResult) {
            if ($csfactory.isNullOrEmptyArray(queryResult.PageData)) {
                $csnotify.success("No Rows matching criteria.");
                gridOptions.PageData = [];
                gridOptions.TotalRowCount = 0;
            } else {
                gridOptions.PageData = queryResult.PageData;
                gridOptions.TotalRowCount = queryResult.TotalRowCount;
                $log.info("$csgrid : total initial rows : " + gridOptions.TotalRowCount +
                    ", displayed rows : " + queryResult.PageData.length);
            }
        };

        var resetSelectedRow = function (gridOptions) {
            gridOptions.$gridScope.toggleSelectAll(false, true);
        };
        //#endregion

        //#region stakeholder data
        var showStakeholderList = function (gridOptions) {
            if (gridOptions.Reporting.Params.Send2Hierarchy == true) {
                gridOptions.showstakeholders = true;
            } else {
                gridOptions.showstakeholders = false;
                gridOptions.SelectedStakeholderIds = [];
                return;
            }
            restapi.customGET("GetStakeholderList")
                .then(
                    function (data) {
                        gridOptions.stakeholderList = data;

                    },
                    function () {
                        $csnotify.error("Error occured while fetching saved reports.");
                    }
                );
        };
        var selectedStake = function (stake, gridOptions) {
            if (angular.isUndefined(gridOptions.SelectedStakeholderIds)) {
                return false;
            }
            return gridOptions.SelectedStakeholderIds.indexOf(stake.ExternalId) !== -1;

        };
        var searchStakeholder = function (search) {
            return function (stakeholder) {

                if (angular.isUndefined(search)) {
                    return true;
                }

                var nameMatch = true;
                if (!angular.isUndefined(search.Name) && search.Name != '') {
                    nameMatch = (stakeholder.Name.toUpperCase().indexOf(search.Name.toUpperCase()) > -1);
                }

                var emailMatch = true;
                if (!angular.isUndefined(search.EmailId) && search.EmailId != '') {
                    emailMatch = (stakeholder.EmailId.toUpperCase().indexOf(search.EmailId.toUpperCase()) > -1);
                }

                var hierarchyMatch = true;
                if (!angular.isUndefined(search.Hierarchy.Hierarchy) && search.Hierarchy.Hierarchy != '') {
                    hierarchyMatch = (stakeholder.Hierarchy.Hierarchy.toUpperCase().indexOf(search.Hierarchy.Hierarchy.toUpperCase()) > -1);
                }

                var designationMatch = true;
                if (!angular.isUndefined(search.Hierarchy.Designation) && search.Hierarchy.Designation != '') {
                    designationMatch = (stakeholder.Hierarchy.Designation.toUpperCase().indexOf(search.Hierarchy.Designation.toUpperCase()) > -1);
                }

                return nameMatch && emailMatch && hierarchyMatch && designationMatch;
            };
        };

        var selectstakeholderClose = function (gridOptions) {
            gridOptions.showstakeholders = false;
            gridOptions.totalSelectedStake = true;
        };

        var selectAllStakeholder = function (gridOptions) {
            if (angular.isUndefined(gridOptions.SelectedStakeholderIds)) {
                gridOptions.SelectedStakeholderIds = [];
            }
            if (gridOptions.selectAll == true) {
                gridOptions.SelectedStakeholderIds = [];
                _.forEach(gridOptions.stakeholderList, function (item) {
                    gridOptions.SelectedStakeholderIds.push(item.ExternalId);
                });

            } else {
                gridOptions.selectAll = false;
                gridOptions.SelectedStakeholderIds = [];
            }
        };

        var selectStakeholder = function (gridOptions, stakeholder) {
            gridOptions.selectAll = false;
            if (angular.isUndefined(gridOptions.SelectedStakeholderIds)) {
                gridOptions.SelectedStakeholderIds = [];
            }
            var selected = _.find(gridOptions.SelectedStakeholderIds, function (stakeid) {
                return stakeid == stakeholder.ExternalId;

            });

            if (angular.isUndefined(selected) || gridOptions.SelectedStakeholderIds.length == 0) {
                gridOptions.SelectedStakeholderIds.push(stakeholder.ExternalId);
            }

            if (angular.isDefined(selected)) {
                var index = gridOptions.SelectedStakeholderIds.indexOf(selected);
                gridOptions.SelectedStakeholderIds.splice(index, 1);
            }

        };
        //#endregion

        //#region download/email
        var downloadDataToExcel = function (gridOptions) {
            gridOptions.selectedOption = '';
            var dataParams = serverData.GetDataParams(gridOptions);
            $log.debug("$csgrid: downloading data from server " + dataParams);

            $csfactory.enableSpinner();
            restapi.customPOST(dataParams, "DownloadGridData")
                .then(
                    function (filename) {
                        $csfactory.downloadFile(filename);
                    },
                    function () {
                        $csnotify.error("Download failed.");
                    }
                );
        };

        var getDataOnEmail = function (gridOptions) {
            gridOptions.selectedOption = '';
            var dataParams = serverData.GetDataParams(gridOptions);
            $log.debug("$csgrid: downloading data from server " + dataParams);
            $csfactory.enableSpinner();
            restapi.customPOST(dataParams, "EmailGridData")
                .then(
                    function () {
                        $csnotify.success("Email sent.");
                    },
                    function () {
                        $csnotify.error("Could not send email.");
                    }
                );
        };
        //#endregion

        //#region reporting
        var reporting = {
            resetGridAsPerReport: function (gridInitData, gridOptions) {

                gridOptions.filtersArray = gridInitData.QueryParams.FiltersList;
                gridOptions.QueryParams.Criteria = gridInitData.QueryParams.Criteria;
                gridOptions.customSort = gridInitData.QueryParams.GridConfig.sortInfo;
                gridOptions.filterOptions = gridInitData.QueryParams.GridConfig.filterOptions;
                // make sure that this is the last line
                gridOptions.pagingOptions.currentPage = 1;
                gridOptions.sortInfo = gridInitData.QueryParams.GridConfig.sortInfo;

                angular.forEach(gridInitData.QueryParams.GridConfig.columnDefs, function (savedColumn) {
                    var gridColumn = _.findWhere(gridOptions.$gridScope.columns, { 'field': savedColumn.field });
                    if (angular.isUndefined(gridColumn)) {
                        $log.error("$csgrid : field does not exist : " + savedColumn.field);
                        return;
                    }
                    if (savedColumn.field === "✔") return;

                    if (angular.isDefined(gridColumn.colDef.index)) {
                        gridColumn.index = savedColumn.index + (gridColumn.index - gridColumn.colDef.index);
                        gridColumn.colDef.index = savedColumn.index;
                    }
                    gridColumn.colDef.displayName = savedColumn.displayName; // display
                    gridColumn.displayName = savedColumn.displayName;        // display
                    gridColumn.colDef.pinned = savedColumn.pinned;           // pinned
                    gridColumn.pinned = savedColumn.pinned;                  // pinned
                    gridColumn.pinnable = true;                              // pinned
                    gridColumn.colDef.visible = savedColumn.visible;         // visible
                    gridColumn.visible = savedColumn.visible;                // visible
                    gridColumn.colDef.width = savedColumn.width;             // width
                    gridColumn.width = savedColumn.width;                    // width
                    gridColumn.sortable = true;                              // sort
                    gridColumn.colDef.sortDirection = savedColumn.sortDirection; // sort
                    gridColumn.sortDirection = savedColumn.sortDirection;        // sort
                });
                gridOptions.$gridScope.columns = _.sortBy(gridOptions.$gridScope.columns, function (c) { return c.index; });
                gridOptions.Columns = _.sortBy(gridOptions.Columns, function (c) { return c.index; });

                setDataFromQueryResult(gridOptions, gridInitData.QueryResult); // query result

                return gridOptions;
            },

            GetReportList: function (gridOptions, screen) {
                gridOptions.Reporting = {
                    Params: { ScreenName: screen },
                    ScreenName: screen
                };
                gridOptions.Reporting.savedReportList = [];
                gridOptions.Reporting.isEditMode = false;
                var param = { 'ScreenName': screen };

                restapi.customPOST(param, "GetReportsList")
                     .then(
                         function (data) {
                             if (angular.isArray(data)) {
                                 gridOptions.Reporting.savedReportList = data;
                             } else {
                                 gridOptions.Reporting.savedReportList = [];
                             }
                         },
                         function () {
                             $csnotify.error("Error occured while fetching saved reports.");
                         }
                     );
            },
            GetReportData: function (gridOptions) {
                if ($csfactory.isNullOrEmptyString(gridOptions.Reporting.SelectedReportIndex)) {
                    gridOptions.Reporting.Params = {};
                    return;
                }
                gridOptions.Reporting.Params = _.find(gridOptions.Reporting.savedReportList,
                    { 'Id': gridOptions.Reporting.SelectedReportIndex });
                if (angular.isUndefined(gridOptions.Reporting.Params)) {
                    return;
                }

                gridOptions.showGrid = false;
                gridOptions.Reporting.isEditMode = true;
                var param = { 'reportId': gridOptions.Reporting.Params.Id };

                restapi.customPOST(param, "GetReportsContents")
                    .then(
                        function (data) {
                            if (angular.isUndefined(data.QueryParams) || angular.isUndefined(data.QueryResult)) { return; }
                            gridOptions = reporting.resetGridAsPerReport(data, gridOptions);
                            gridOptions.showGrid = true;
                        },
                        function () {
                            gridOptions.showGrid = true;
                            $csnotify.error("No such report exist.");
                        }
                    );
            },
            SaveAsReport: function (gridOptions) {
                if (!gridOptions.Reporting.isEditMode) {
                    var report = _.find(gridOptions.Reporting.savedReportList,
                        { 'ReportName': gridOptions.Reporting.Params.ReportName });
                    if (angular.isDefined(report)) {
                        $csnotify.error("Duplicate report name.");
                        return;
                    }
                }

                gridOptions.selectedOption = '';
                var dataParams = serverData.GetDataParams(gridOptions);
                $log.debug("$csgrid: saving as report" + dataParams);
                gridOptions.Reporting.Params.ScreenName = gridOptions.Reporting.ScreenName;

                gridOptions.Reporting.Params.StakeholderIds = gridOptions.SelectedStakeholderIds.toString();

                var reportParams = {
                    Params: dataParams,
                    Report: gridOptions.Reporting.Params
                };

                restapi.customPOST(reportParams, "SaveReport")
                    .then(
                        function (data) {
                            gridOptions.Reporting.Params = data;
                            $csnotify.success("Saved Report " + gridOptions.Reporting.Params.ReportName);
                            if (data.Version === 1) {
                                gridOptions.Reporting.savedReportList.push(data);
                            } else {
                                gridOptions.Reporting.Params.Version = data.Version;
                            }
                            gridOptions.Reporting.isEditMode = true;
                        },
                        function () {
                            $csnotify.error("Could not save report.");
                        }
                    );
            },
            DeleteReport: function (gridOptions) {
                if (!gridOptions.Reporting.isEditMode || $csfactory.isNullOrEmptyGuid(gridOptions.Reporting.SelectedReportIndex)) {
                    $csnotify.error("Technical Error. Cannot delete report");
                    return;
                }

                gridOptions.selectedOption = '';
                gridOptions.Reporting.Params = _.find(gridOptions.Reporting.savedReportList,
                    { 'Id': gridOptions.Reporting.SelectedReportIndex });
                if (angular.isUndefined(gridOptions.Reporting.Params)) {
                    return;
                }

                var reportParams = { 'reportId': gridOptions.Reporting.Params.Id };
                restapi.customPOST(reportParams, "DeleteReport")
                    .then(
                        function () {
                            $csnotify.success("Deleted Report.");
                            gridOptions.Reporting.savedReportList = _.rest(gridOptions.Reporting.savedReportList,
                                { 'Id': gridOptions.Reporting.SelectedReportIndex });
                            gridOptions.Reporting.Params = {};
                            gridOptions.Reporting.isEditMode = false;
                            gridOptions.Reporting.SelectedReportIndex = '';
                            resetHelper.ResetAll(gridOptions);
                        },
                        function () {
                            $csnotify.error("Could not save report.");
                        }
                    );
            },
            CloneReport: function (gridOptions) {
                gridOptions.Reporting.Params = angular.copy(gridOptions.Reporting.Params);
                gridOptions.Reporting.Params.ReportName = 'clone';
                gridOptions.Reporting.Params.Id = '';
                gridOptions.Reporting.Params.Version = '';
                gridOptions.Reporting.SelectedReportIndex = '';
                gridOptions.Reporting.isEditMode = false;
            }
        };
        //#endregion

        //#region reset
        var resetHelper = {
            resetFreeze: function (gridOptions) {
                angular.forEach(gridOptions.$gridScope.columns, function (gridColumn) {
                    if (($csfactory.isNullOrEmptyString(gridColumn.field)) || (gridColumn.field === "✔")) return;
                    if (gridColumn.pinned) {
                        gridOptions.$gridScope.togglePin(gridColumn);
                        gridColumn.colDef.pinned = false;
                        gridColumn.pinned = false;
                        gridColumn.pinnable = true;
                    }
                });
            },
            resetNameChanges: function (gridOptions) {
                angular.forEach(gridOptions.$gridScope.columns, function (gridColumn) {
                    gridColumn.colDef.displayName = gridColumn.field;
                    gridColumn.displayName = gridColumn.field;
                });
            },
            resetPosition: function (gridOptions) {
                angular.forEach(gridOptions.$gridScope.columns, function (gridColumn) {
                    if (angular.isDefined(gridColumn.colDef.index)) {
                        var diff = gridColumn.index - gridColumn.colDef.index;
                        gridColumn.index = gridColumn.originalIndex;
                        gridColumn.colDef.index = gridColumn.index - diff;
                    }
                });
                gridOptions.$gridScope.columns = _.sortBy(gridOptions.$gridScope.columns, function (c) { return c.index; });
                gridOptions.Columns = _.sortBy(gridOptions.Columns, function (c) { return c.index; });
            },
            resetSorting: function (gridOptions) {
                angular.forEach(gridOptions.$gridScope.columns, function (gridColumn) {
                    gridColumn.sortable = true;
                    gridColumn.sortDirection = '';
                    gridColumn.colDef.sortDirection = '';
                });
                gridOptions.customSort = { columns: [], fields: [], directions: [] };
                gridOptions.sortInfo = { columns: [], fields: [], directions: [] };
            },
            resetFiltering: function (gridOptions) {
                // old filter
                gridOptions.filtering = { useExternalFilter: true, filterText: "" };
                gridOptions.filterOptions = gridOptions.filtering;
                // new filter
                gridOptions.customFilter = {};
                gridOptions.filtersArray = [];
            },
            resetVisibility: function (gridOptions) {
                angular.forEach(gridOptions.$gridScope.columns, function (gridColumn) {
                    gridColumn.colDef.visible = true;
                    gridColumn.visible = true;
                });
            },
            resetPagination: function (gridOptions) {
                gridOptions.pageOptions = {
                    pageSizes: [20, 50, 100, 200, 500, 1000],
                    pageSize: 20,
                    currentPage: 1,
                    totalServerItems: 0
                };
                gridOptions.pagingOptions = gridOptions.pageOptions;
            },
            resetReport: function (gridOptions) {
                if (!gridOptions.Reporting.isEditMode) return;
                gridOptions.Reporting.Params = {};
                gridOptions.Reporting.isEditMode = false;
                gridOptions.Reporting.SelectedReportIndex = '';
            },
            resetBooleans: function (gridOptions) {
                if (angular.isUndefined(gridOptions)) return;
                if (angular.isUndefined(gridOptions.ResetSetting))
                    gridOptions.ResetSetting = {};
                var setting = gridOptions.ResetSetting;
                setting.doResetFreeze = false;
                setting.doResetRenames = false;
                setting.doResetPosition = false;
                setting.doResetSorting = false;
                setting.doResetFilter = false;
                setting.doResetVisibility = false;
                setting.doResetSorting = false;
                setting.doResetReport = false;
                gridOptions.selectedOption = '';
            },
            ResetSelected: function (gridOptions) {
                var setting = gridOptions.ResetSetting;
                if (setting.doResetFreeze === true) resetHelper.resetFreeze(gridOptions);
                if (setting.doResetRenames === true) resetHelper.resetNameChanges(gridOptions);
                if (setting.doResetPosition === true) resetHelper.resetPosition(gridOptions);
                if (setting.doResetSorting === true) resetHelper.resetSorting(gridOptions);
                if (setting.doResetFilter === true) resetHelper.resetFiltering(gridOptions);
                if (setting.doResetVisibility === true) resetHelper.resetVisibility(gridOptions);
                if (setting.doResetReport === true) resetHelper.resetReport(gridOptions);
                if (setting.doResetSorting === true || setting.doResetFilter === true) {
                    resetHelper.resetPagination(gridOptions);
                    serverData.GetGridData(gridOptions);
                }
                resetHelper.resetBooleans(gridOptions);
            },
            ResetAll: function (gridOptions) {
                resetHelper.resetFreeze(gridOptions);
                resetHelper.resetNameChanges(gridOptions);
                resetHelper.resetPosition(gridOptions);
                resetHelper.resetSorting(gridOptions);
                resetHelper.resetFiltering(gridOptions);
                resetHelper.resetVisibility(gridOptions);
                resetHelper.resetReport(gridOptions);
                resetHelper.resetPagination(gridOptions);
                serverData.GetGridData(gridOptions);
                resetHelper.resetBooleans(gridOptions);
            }
        };
        //#endregion

        return {
            InitGrid: initGrid,   // query params
            SetData: setDataFromQueryResult, // query result
            ResetSelection: resetSelectedRow,
            freezeColumn: freezeColumn,     // freeze
            showHideColumn: showHideColumn, // show-hide
            MoveColumn: changeColumnPosition, // move
            SortHelper: sortingHelper,   // sort
            FilterHelper: filteringHelper, // filter
            Download: downloadDataToExcel, // download
            ShowStakeholder: showStakeholderList,
            SearchStakeholder: searchStakeholder,
            SelectStakeholder: selectStakeholder,
            SelectAllStakeholder: selectAllStakeholder,
            SelectedStake: selectedStake,
            SelectstakeholderClose: selectstakeholderClose,
            RepotingHelper: reporting, // reports
            consts: $csConstants,
            ResetHelper: resetHelper,
            EmailNow: getDataOnEmail,
            Helper: helpers
        };
    }]);



