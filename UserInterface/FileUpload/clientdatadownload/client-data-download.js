
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
                        dldata.fileDetails = data.FileDetails;
                    },
                    function (response) {
                        $csnotify.error("Data Retrieve Error." + response.data.Message);
                    });
        };

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
    ["$scope", "$csfactory", "$csGrid", "Logger", "ClientDataDownloadDataLayer",
    function ($scope, $csfactory, $grid, logManager, datalayer) {

        //#region init
        //var $log = logManager.getInstance("ClientDataDownloadController");
        (function () {
            $scope.params = {};
            $scope.$csfactory = $csfactory;
            $scope.datalayer = datalayer;
            $scope.$grid = $grid;
            datalayer.Get();
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
                case "Product":
                    param.SelectedCategory = "";
                case "System":
                    param.SelectedCategory = "";
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
