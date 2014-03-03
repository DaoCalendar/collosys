(
csapp.controller("ClientDataDownloadCtrl", ["$scope", "Restangular", "$csnotify", "$csConstants", "$csfactory", "$csGrid",
    function ($scope, $restangular, $csnotify, $csConstants, $csfactory, $grid) {

        var restapi = $restangular.all("ClientDataDownloadApi");

        //#region init
        var init = function () {
            $scope.params = {};
            $scope.$csfactory = $csfactory;
            getProductCategory();
        };

        var getProductCategory = function () {
            restapi.customGETLIST("FetchProductCategory")
                .then(
                    function (data) {
                        $scope.ProductList = data.Products;
                        $scope.CategoryList = data.Category;
                        $scope.fileDetails = data.FileDetails;
                        //$csnotify.success("Data Retrieve Success");
                    },
                    function (response) {
                        $csnotify.error("Data Retrieve Error." + response.data.Message);
                    });
        };

        init();
        //#endregion

        //#region reset
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
        //#endregion

        //#region download
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
        //#endregion

        //#region nggrid
        $scope.getPagedDataAsync = function (downloadParams) {

            $scope.$grid = $grid;
            $scope.gridOptions = {};

            restapi.customPOST(downloadParams, "FetchPageData")
                .then(
                    function (data) {
                        if (angular.isUndefined(data.QueryParams) || angular.isUndefined(data.QueryResult)) { return; }
                        $scope.gridOptions = $grid.InitGrid(data.QueryParams, $scope.gridOptions); // query params
                        $grid.SetData($scope.gridOptions, data.QueryResult); // query result
                        $grid.RepotingHelper.GetReportList($scope.gridOptions, data.ScreenName);
                    },
                    function (response) {
                        $csnotify.error("Failed to fetch the data." + response.data.Message);
                    }
                );
        };

        //#endregion

        //#region harish - testing - button
        $scope.fetchdatawithdetauls = function () {
            $scope.params = {
                SelectedCategory: "Payment",
                SelectedDate: "2014-01-01T00:00:00.000Z",
                SelectedProduct: "",
                SelectedSystem: "RLS",
                ShowDataBy: "System"
            };
            $scope.getPagedDataAsync($scope.params);
        };
        //$scope.fetchdatawithdetauls();
        //#endregion
    }
])
);
