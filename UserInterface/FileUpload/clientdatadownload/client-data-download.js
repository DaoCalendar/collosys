
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

csapp.controller("ClientDataDownloadController", ["$scope", "$csfactory", "$csGrid", "ClientDataDownloadDataLayer", "$modal",
    function ($scope, $csfactory, $grid, datalayer, $modal) {

        //#region init
        (function () {
            $scope.params = {};
            $scope.$csfactory = $csfactory;
            $scope.datalayer = datalayer;
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
            $scope.gridOptions = {};
            $scope.$grid = $grid;

            datalayer.GetData(downloadparams).then(function () {
                $scope.gridOptions = datalayer.dldata.gridOptions;
            });
        };
        //#endregion
    }
]);


////#region harish - testing - button
//$scope.fetchdatawithdetauls = function () {
//    $scope.params = {
//        SelectedCategory: "Payment",
//        SelectedDate: "2014-03-15",
//        SelectedProduct: "",
//        SelectedSystem: "RLS",
//        ShowDataBy: "System"
//    };
//    $scope.getPagedDataAsync($scope.params);
//};
//$scope.fetchdatawithdetauls();
////#endregion

