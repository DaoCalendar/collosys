
csapp.controller("paymentReversalCtrl",
    ["$scope", "Restangular", "$csnotify", "$csGrid", "$Validations", function ($scope, rest, $csnotify, $grid, $validation) {
        "use strict";

        var restApi = rest.all("PaymentReversalApi");

        $scope.selectedSystem = '';
        $scope.val = $validation;
        $scope.addpayment = {};
        $scope.accNoAndCustList = [];

        var showErrorMessage = function (response) {
            $csnotify.error(response.data);
        };

        restApi.customGETLIST("GetScbSystems").then(function (data) {
            $scope.scbSystems = data;
        }, showErrorMessage);


        $scope.fetchData = function () {
            $scope.$grid = $grid;
            $scope.gridOptions = {};

            restApi.customGET("FetchPageData", { systems: $scope.selectedSystem }).then(function (data) {
                debugger;
                if (angular.isUndefined(data.QueryParams) || angular.isUndefined(data.QueryResult)) { return; }
                $scope.gridOptions = $grid.InitGrid(data.QueryParams); // query params
                $grid.SetData($scope.gridOptions, data.QueryResult); // query result
                $grid.RepotingHelper.GetReportList($scope.gridOptions, data.ScreenName);
            }, function (response) {
                $csnotify.error(response.data);
            });
        };


        $scope.excludeselectedPayment = function () {
            var items = $scope.gridOptions.$gridScope.selectedItems;

            restApi.customPOST(items, "ExcludePayment", { systems: $scope.selectedSystem }).then(function (data) {
                if (data) {
                    $csnotify.success("Data Excluded");
                    $scope.fetchData();
                    debugger;
                    $scope.gridOptions.$gridScope.selectedItems.splice(0, $scope.gridOptions.$gridScope.selectedItems.length);;
                }
            }, showErrorMessage);
        };

        $scope.OpenPaymentManager = function () {
            $scope.OpenAddPaymentManager = true;
        };


        $scope.CloseAddPaymentManager = function () {
            $scope.Reset();
         $scope.OpenAddPaymentManager = false;
        };

        restApi.customGET("GetProducts").then(function (data) {
            $scope.productsList = data;
        }, function (response) {
            $csnotify.error(response);
        });

        $scope.Reset = function () {
            $scope.addpayment = {};
        };

        $scope.accountNo = function (accNo, product) {
          if (accNo.length < 3) {
                return [];
            }
          return restApi.customGET('GetAccountNo', { accountNo: accNo, products: product }).then(function (data) {
              $scope.accNoAndCustList = data;
              return data;
            });
        };

        $scope.customerName = function () {
            if (angular.isUndefined($scope.addpayment.AccountNo)) {
                return;
            }
            _.find($scope.accNoAndCustList,function (item) {
                if (item.AccountNo == $scope.addpayment.AccountNo) {
                    debugger;
                    $scope.addpayment.customerName = item.CustomerName;
                }
            });

        };

        $scope.Save = function (addpayment) {
            restApi.customPOST(addpayment, 'AddPayments').then(function () {
                $csnotify.success("Payment added Successfully");
                $scope.CloseAddPaymentManager();
                $scope.fetchData();
            }, function () {
                $csnotify.error();
            });
        };

    }]);