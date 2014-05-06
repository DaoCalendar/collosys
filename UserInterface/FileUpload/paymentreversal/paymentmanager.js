
csapp.factory("paymentDataLayer", ["Restangular", "$csnotify", "$csGrid",
    function (rest, $csnotify, $grid) {

        var dldata = {
            gridOptions: {}
        };
        var restApi = rest.all("PaymentReversalApi");

        var showErrorMessage = function (response) {
            $csnotify.error(response.data);
        };

        var getSystemsEnum = function () {
            restApi.customGETLIST("GetScbSystems").then(function (data) {
                dldata.scbSystems = data;
            }, showErrorMessage);
        };

        var getProducts = function () {
            restApi.customGET("GetProducts").then(function (data) {
                dldata.productsList = data;
            }, showErrorMessage);
        };

        var getGridInitInfo = function (selectedSystem) {
            return restApi.customGET("FetchPageData", { systems: selectedSystem }).then(function (data) {
                if (angular.isUndefined(data.QueryParams) || angular.isUndefined(data.QueryResult)) {
                    throw "The return type of the data passed should only be GridInitData";
                }
                dldata.gridOptions = $grid.InitGrid(data.QueryParams); // query params
                $grid.SetData(dldata.gridOptions, data.QueryResult); // query result
                $grid.RepotingHelper.GetReportList(dldata.gridOptions, data.ScreenName);
                return data;
            }, showErrorMessage);
        };

        var excludePayments = function (items, selectedSystem) {
            return restApi.customPOST(items, "ExcludePayment", { systems: selectedSystem }).then(function (data) {
                $csnotify.success("Data Excluded");
                if (data) {
                    getGridInitInfo(selectedSystem);
                    dldata.gridOptions.$gridScope.selectedItems.splice(0, dldata.gridOptions.$gridScope.selectedItems.length);;
                }
            }, showErrorMessage);
        };

        var save = function (addpayment, selectedSystem) {
            return restApi.customPOST(addpayment, 'AddPayments').then(function (data) {
                $csnotify.success("Payment added Successfully");
                getGridInitInfo(selectedSystem);
                return data;
            }, showErrorMessage);
        };

        var getAccountNo = function (accNo, product) {
            if (accNo.length < 3) {
                return [];
            }
            return restApi.customGET('GetAccountNo', { accountNo: accNo, products: product })
                .then(function (data) {
                    dldata.accNoAndCustList = data;
                    return data;
                }, showErrorMessage);
        };

        return {
            dldata: dldata,
            GetSystems: getSystemsEnum,
            GetProducts: getProducts,
            Exclude: excludePayments,
            GetAll: getGridInitInfo,
            AddPayment: save,
            GetAccounts: getAccountNo
        };
    }
]);


csapp.controller("paymentAddController", ["$scope", "paymentDataLayer", "$modalInstance", "$Validations",
    "$csFileUploadModels",
    function ($scope, datalayer, $modalInstance, $Validation, $csFileUploadModels) {

        (function () {
            $scope.val = $Validation;
            $scope.addpayment = {};
            $scope.accNoAndCustList = [];
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.addpaymentModel = $csFileUploadModels.models.ExcludeCase;
        })();

        $scope.getCustomerName = function () {
            if (angular.isUndefined($scope.addpayment.AccountNo)) {
                return;
            }
            if ($scope.addpayment.AccountNo.length >= 8) {
                _.find($scope.dldata.accNoAndCustList, function(item) {
                    if (item.AccountNo == $scope.addpayment.AccountNo) {
                        $scope.addpayment.customerName = item.CustomerName;
                    }
                });
            }            

        };

        $scope.save = function(item) {
            datalayer.AddPayment(item, $scope.selectedSystem).then(function (data) {
                $modalInstance.close(data);
            });
        };

        $scope.close = function() {
            $modalInstance.dismiss();
        };

        $scope.reset = function() {
            $scope.addpayment = {};
        };
    }
]);

csapp.controller("paymentManagerController", [
    "$scope", "$csGrid", "$Validations", "paymentDataLayer", "$modal", "$csfactory", "$csFileUploadModels",
    function ($scope, $grid, $validation, datalayer, $modal, $csfactory, $csFileUploadModels) {
        "use strict";

        (function () {
            $scope.val = $validation;
            datalayer.GetProducts();
            datalayer.GetSystems();
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.addpaymentModel = $csFileUploadModels.models.ExcludeCase;
        })();

        $scope.excludePayment = function () {
            var items = $scope.gridOptions.$gridScope.selectedItems;
            datalayer.Exclude(items, $scope.selectedSystem);
        };

        $scope.getData = function (system) {
            if ($csfactory.isNullOrEmptyString(system)) return;
            datalayer.GetAll(system).then(function () {
                $scope.gridOptions = datalayer.dldata.gridOptions;
            });;
        };

        $scope.OpenAddPaymentModal = function() {
            $modal.open({
                templateUrl: baseUrl + 'FileUpload/paymentreversal/payment-add.html',
                controller: 'paymentAddController'
            });
        };
    }
]);