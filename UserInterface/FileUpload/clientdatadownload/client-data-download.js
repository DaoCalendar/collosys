
csapp.factory("ClientDataDownloadDataLayer", [
    "Restangular", "$csnotify", "$csGrid", function (rest, $csnotify, $grid) {
        var restapi = rest.all("ClientDataDownloadApi");
        var dldata = {};

        var getProductCategory = function () {
            restapi.customGET("FetchProductCategory")
                .then(
                    function (data) {
                        dldata.ProductList = data.Products;
                        dldata.CategoryList = data.Category;
                        dldata.fileDetails = _.uniq(_.pluck(data.FileDetails, 'Category'));

                    },
                    function (response) {
                        $csnotify.error("Data Retrieve Error." + response.data.Message);
                    });
        };

        dldata.Downloadby = [{ display: 'Product', value: 'Product' }, { display: 'System', value: 'System' }];


        var getPagedDataAsync = function (downloadParams) {
            dldata.gridOptions = {};

            return restapi.customPOST(downloadParams, "FetchPageData")
                .then(
                    function (data) {
                        if (angular.isUndefined(data.QueryParams) || angular.isUndefined(data.QueryResult)) { return; }
                        dldata.gridOptions = $grid.InitGrid(data.QueryParams, dldata.gridOptions); // query params
                        $grid.SetData(dldata.gridOptions, data.QueryResult); // query result
                        $grid.RepotingHelper.GetReportList(dldata.gridOptions, data.ScreenName);
                    },
                    function (response) {
                        $csnotify.error("Failed to fetch the data." + response.data.Message);
                    }
                );
        };

        return {
            dldata: dldata,
            Get: getProductCategory,
            GetData: getPagedDataAsync
        };
    }
]);

csapp.controller("ClientDataDownloadController",
    ["$scope", "$csfactory", "$csGrid", "Logger", "ClientDataDownloadDataLayer", "$csShared",
    function ($scope, $csfactory, $grid, logManager, datalayer, $csShared) {

        //#region init
        //var $log = logManager.getInstance("ClientDataDownloadController");
        (function () {
            $scope.params = {};
            $scope.$csfactory = $csfactory;
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.$grid = $grid;
            datalayer.Get();
            $scope.paramsfield = {
                ShowDataBy: { label: 'Download By', type: 'select', textField: 'display', valueField: 'value' },
                SelectedProduct: { label: 'Product', type: "enum", valueList: $csShared.enums.ProductEnum },
                SelectedSystem: { label: 'System', type: 'enum', valueList: $csShared.enums.ScbSystems },
                SelectedCategory: { label: 'Category', type: 'select' },
                SelectedDate: { label: 'Date', type: 'date' }
            };
        })();
        //#endregion

        //#region helpers
        $scope.ResetControls = function (control, param) {
            if ($csfactory.isNullOrEmptyString(control)) {
                return;
            }
            switch (control) {
                case "ShowDataBy":
                    param.SelectedProduct = "";
                    param.SelectedSystem = "";
                    param.SelectedCategory = "";
                case "Product":
                    param.SelectedCategory = "";
                    param.SelectedDate = null;
                case "System":
                    param.SelectedCategory = "";
                    param.SelectedDate = null;
                case "Category":
                    param.SelectedDate = null;
                    if (angular.isDefined($scope.gridOptions)) {
                        $scope.gridOptions.showGrid = false;
                    }
                    break;
                default:
                    break;
            }
        };
        $scope.ResetControls("ShowDataBy", $scope.params);

        $scope.AreParamsInvalid = function (param) {
            if ($csfactory.isNullOrEmptyString(param.ShowDataBy)) {
                return true;
            } else {
                if ((param.ShowDataBy === 'Product') && ($csfactory.isNullOrEmptyString(param.SelectedProduct))) {
                    return true;
                } else if ((param.ShowDataBy === 'System') && ($csfactory.isNullOrEmptyString(param.SelectedSystem))) {
                    return true;
                } else if ((param.ShowDataBy !== 'System') && (param.ShowDataBy !== 'Product')) {
                    return true;
                }
            }

            if ($csfactory.isNullOrEmptyString(param.SelectedCategory) ||
                $csfactory.isNullOrEmptyString(param.SelectedDate) ||
                (angular.isUndefined($scope.gridOptions) || $csfactory.isNullOrEmptyArray($scope.gridOptions.PageData))) {
                return true;
            }

            return false;
        };

        $scope.getPagedDataAsync = function (downloadparams) {
            if ($scope.gettingPageData === true) return;
            $scope.gettingPageData = true;
            $csfactory.enableSpinner();

            datalayer.GetData(downloadparams).then(function () {
                $scope.gridOptions = datalayer.dldata.gridOptions;
            }).finally(function () {
                $scope.gettingPageData = false;
            });
        };

        $scope.fetchdatawithdetauls = function () {
            $scope.params = {
                SelectedCategory: "Liner",
                SelectedDate: "2014-04-02",
                SelectedProduct: "",
                SelectedSystem: "EBBS",
                ShowDataBy: "System"
            };
            $scope.getPagedDataAsync($scope.params);
        };
        //$scope.fetchdatawithdetauls();

        //#endregion
    }
    ]);
