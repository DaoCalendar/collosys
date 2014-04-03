
    //#region "Controller"
csapp.controller('readyForBillingController', ["$scope", "$csnotify", "$csfactory", "$csGrid", "$Validations", "readyForBillingDataLayer", function ($scope, $csnotify, $csfactory, $grid, $validation, factoryForBilling) {

    (function () {
        $scope.dldata = factoryForBilling.dldata;
        $scope.dldata.billing = {};
            $scope.dldata.pageData = [];
        $scope.dldata.oldBillStatus = 'null';
        factoryForBilling.GetProducts();
    })();

        //#region "Fetch Grid Data"
        $scope.showGrid = function(product, billmonth) {
            $scope.$grid = $grid;
            $scope.gridOptions = {};
            //$scope.pageData = [];
            var billingMonth = moment(billmonth).format('YYYYMM');
            if ($csfactory.isNullOrEmptyString(product) || $csfactory.isNullOrEmptyString(billmonth)) {
                return;
            }

            factoryForBilling.GetBillingData(product, billingMonth);


        };
        //#endregion

        //#region "Fetch Whole Products"
        $scope.fetchProduct = function() {
            factoryForBilling.GetProducts();
        };
        //#endregion

        //#region "Init"
        //var init = function() {
        //    $scope.ProductList = [];
        //    $scope.pageData = [];
        //    $scope.billing = {};
            
        //    $scope.oldBillStatus = 'null';
        //};
        //init();
        ////#endregion

        //#region "Save Data"
        $scope.saveData = function(billing) {
            $scope.dldata.billing.Status = 'Pending';
            $scope.dldata.billing.BillCycle = 0;
            billing.BillMonth = moment(billing.BillMonth).format('YYYYMM');

            factoryForBilling.SaveBillingData(billing);
        };

        //#endregion
    }]);
//#endregion


    //#region "Factory"
    csapp.factory('readyForBillingDataLayer', ["Restangular", "$csfactory", "$csnotify", function(rest, $csfactory, $csnotify) {
        var dldata = {};

        var restapi = rest.all('ReadyForBillingApi');

        var getbillingData = function(product, billingMonth) {
            return restapi.customGET("FetchPageData", { 'Products': product, 'month': billingMonth }).then(
                function (data) {
                    dldata.pageData = data;
                    //if (angular.isUndefined(data.QueryParams) || angular.isUndefined(data.QueryResult)) { return; }
                    //$scope.gridOptions = $grid.InitGrid(data.QueryParams, $scope.gridOptions); // query params
                    //$grid.SetData($scope.gridOptions, data.QueryResult); // query result
                    //$grid.RepotingHelper.GetReportList($scope.gridOptions, data.ScreenName);
                    dldata.oldBillStatus = 'null';
                    getbillStatus(product, billingMonth);
                },
                function (response) {
                    $csnotify.error("Failed to fetch the data." + response.data.Message);
                }
            );
        };

        var getbillStatus = function(product, billingMonth) {
            return restapi.customGET("GetBillStatus", { 'Products': product, 'month': billingMonth }).then(
                        function (data) {
                            dldata.oldBillStatus = data;
                        },
                        function (response) {
                            $csnotify.error("Failed to fetch the data." + response.data.Message);
                        }
                    );
        };

        var getproducts = function() {
            return restapi.customGET('GetProductList').then(function (data) {
                dldata.ProductList = data;
            });
        };

        var saveBillingData = function(billing) {
            return restapi.customPOST(billing, "SaveBillingdata").then(function() {
                $csnotify.success("Data Saved Successfully..!!");
                dldata.billing = {};
            }, function() {
                $csnotify.error("something going wrong");
            });
        };
        return {
            GetBillingData: getbillingData,
            GetBillStatus: getbillStatus,
            GetProducts: getproducts,
            SaveBillingData: saveBillingData,
            dldata: dldata
        };

    }]);
//#endregion
