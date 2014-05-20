﻿
csapp.factory('payoutSubpolicyDataLayer', ['Restangular', '$csnotify', '$csfactory',
    function (rest, $csnotify, $csfactory) {
        var restApi = rest.all("PayoutSubpolicyApi");

        var getSubpolicyList = function (product) {
            return restApi.customGET("GetPayoutSubpolicy", { product: product })
                .then(function (data) {
                    return data;

                }, function (data) {
                    $csnotify.error(data);
                });
        };

        var getFormulaList = function (product) {
            return restApi.customGET('GetFormulas', { product: product, category: 'Liner' }).then(function (data) {
                return _.filter(data, { PayoutSubpolicyType: 'Formula' });
            }, function (data) {
                $csnotify.error(data);
            });
        };

        var saveSubpolicy = function (subpolicy) {
            return restApi.customPOST(subpolicy, 'Post').then(function () {
                return;
            });
        };

        var getMatrixList = function(product) {
            return restApi.customGET("GetMatrixList", { product: product })
               .then(function (data) {
                   return data;

               }, function (data) {
                   $csnotify.error(data);
               });
        };

        return {
            getSubpolicyList: getSubpolicyList,
            getFormulaList: getFormulaList,
            saveSubpolicy: saveSubpolicy,
            getMatrixList: getMatrixList
        };
    }]);

csapp.controller('payoutSubpolicyCtrl', ['$scope', 'payoutSubpolicyDataLayer', '$modal', '$csModels', '$csShared',
    function ($scope, datalayer,$modal, $csModels, $csShared) {

        $scope.getSubpolicyList = function (product) {
            datalayer.getSubpolicyList(product).then(function (data) {
                $scope.subpolicyList = _.filter(data, { PayoutSubpolicyType: 'Subpolicy' });
                $scope.formulaList = _.filter(data, { PayoutSubpolicyType: 'Formula' });
                
                //get matrixList
                datalayer.getMatrixList(product).then(function (matrixData) {
                    $scope.matrixList = matrixData;
                });
            });
            $scope.subpolicy.Products = product;
        };

        var combineTokens = function (selectedTokens) {
            return _.union(selectedTokens.conditionTokens,
                selectedTokens.ifOutputTokens,
                selectedTokens.ElseOutputTokens);
        };

        var divideTokens = function (tokensList) {
            $scope.selectedTokens.conditionTokens = _.filter(tokensList, { 'GroupId': '0.Condition' });
            $scope.selectedTokens.ifOutputTokens = _.filter(tokensList, { 'GroupId': '1.Output' });
            $scope.selectedTokens.ElseOutputTokens = _.filter(tokensList, { 'GroupId': '2.Output' });
        };

        $scope.saveSubPolicy = function (subpolicy, selectedTokens) {
            subpolicy.BillTokens = combineTokens(selectedTokens);
            datalayer.saveSubpolicy(subpolicy).then(function (data) {

            });
        };

        $scope.selectSubpolicy = function (subpolicy) {
            $scope.subpolicy = subpolicy;
            divideTokens(subpolicy.BillTokens);
        };

        var init = function () {
            $scope.subpolicy = {
                PayoutSubpolicyType: 'Subpolicy'
            };
            $scope.formulaList = [];
            $scope.matrixList = [];
            $scope.selectedTokens = {
                conditionTokens: [],
                ifOutputTokens: [],
                ElseOutputTokens: []
            };
        };

        (function () {
            $scope.CustBillViewModel = $csModels.getColumns("CustomerInfo");
            $scope.GPincode = $csModels.getColumns("Pincode");
            $scope.payoutSubpolicy = $csModels.getColumns("BillingSubpolicy");
            init();
        })();
    }]);
