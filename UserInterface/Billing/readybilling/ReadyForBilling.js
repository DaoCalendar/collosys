(
    //#region "Controller"
csapp.controller('readyForBillingController', ["$scope", "$csnotify", "$csfactory", "$csGrid", "$Validations", "factoryForBilling", function ($scope, $csnotify, $csfactory, $grid, $validation, factoryForBilling) {

    //#region "Fetch Grid Data"
    $scope.showGrid = function (product, billmonth) {
        $scope.$grid = $grid;
        $scope.gridOptions = {};
        $scope.pageData = [];
        var billingMonth = moment(billmonth).format('YYYYMM');
        debugger;
        if ($csfactory.isNullOrEmptyString(product) || $csfactory.isNullOrEmptyString(billmonth)) {
            return;
        }

        factoryForBilling.GetBillingData(product, billingMonth).then(
                function (data) {
                    $scope.pageData = data;
                    //if (angular.isUndefined(data.QueryParams) || angular.isUndefined(data.QueryResult)) { return; }
                    //$scope.gridOptions = $grid.InitGrid(data.QueryParams, $scope.gridOptions); // query params
                    //$grid.SetData($scope.gridOptions, data.QueryResult); // query result
                    //$grid.RepotingHelper.GetReportList($scope.gridOptions, data.ScreenName);
                    $scope.oldBillStatus = 'null';
                    factoryForBilling.GetBillStatus(product, billingMonth).then(
                        function(data) {
                            $scope.oldBillStatus = data;
                        },
                        function(response) {
                            $csnotify.error("Failed to fetch the data." + response.data.Message);
                        }
                    );
                },
                function (response) {
                    $csnotify.error("Failed to fetch the data." + response.data.Message);
                }
            );



    };
    //#endregion

    //#region "Fetch Whole Products"
    $scope.fetchProduct = function () {
        factoryForBilling.GetProducts().then(function (data) {
            $scope.ProductList = data;
        });
    };
    //#endregion

    //#region "Init"
    var init = function () {
        $scope.ProductList = [];
        $scope.pageData = [];
        $scope.billing = {};
        $scope.fetchProduct();
        $scope.oldBillStatus = 'null';
    };
    init();
    //#endregion

    //#region "Save Data"
    $scope.saveData = function (billing) {
        $scope.billing.Status = 'Pending';
        $scope.billing.BillCycle = 0;
        billing.BillMonth = moment(billing.BillMonth).format('YYYYMM');

        factoryForBilling.SaveBillingData(billing).then(function () {
            $csnotify.success("Data Saved Successfully..!!");
            $scope.billing = {};
        }, function () {
            $csnotify.error("something going wrong");
        });
    };

    //#endregion
}])
//#endregion
);
(
    //#region "Factory"
csapp.factory('factoryForBilling', ["Restangular", "$csfactory", "$csnotify", function (rest, $csfactory, $csnotify) {
    var restapi = rest.all('ReadyForBillingApi');

    var getbillingData = function (product, billingMonth) {
        return restapi.customGET("FetchPageData", { 'Products': product, 'month': billingMonth });
    };

    var getbillStatus = function (product, billingMonth) {
        return restapi.customGET("GetBillStatus", { 'Products': product, 'month': billingMonth });
    };

    var getproducts = function () {
        return restapi.customGET('GetProductList');
    };

    var saveBillingData = function (billing) {
        return restapi.customPOST(billing, "SaveBillingdata");
    };
    return {
        GetBillingData: getbillingData,
        GetBillStatus: getbillStatus,
        GetProducts: getproducts,
        SaveBillingData: saveBillingData
    };

}])
//#endregion
);