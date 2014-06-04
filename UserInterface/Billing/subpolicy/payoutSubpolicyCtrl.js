
csapp.factory('payoutSubpolicyDataLayer', ['Restangular', '$csnotify', '$csfactory',
    function (rest, $csnotify, $csfactory) {
        var restApi = rest.all("PayoutSubpolicyApi");

        var getSubpolicyList = function (selectedParams) {
            return restApi.customGET("GetPayoutSubpolicy", { product: selectedParams.Products, policyType: selectedParams.PolicyType })
                .then(function (data) {
                    return data;
                }, function (data) {
                    $csnotify.error(data);
                });
        };

        var saveSubpolicy = function (subpolicy) {
            return restApi.customPOST(subpolicy, 'Post').then(function (data) {
                return data;
            });
        };

        var getMatrixList = function (product) {
            return restApi.customGET("GetMatrixList", { product: product })
               .then(function (data) {
                   return data;

               }, function (data) {
                   $csnotify.error(data);
               });
        };

        return {
            getSubpolicyList: getSubpolicyList,
            saveSubpolicy: saveSubpolicy,
            getMatrixList: getMatrixList,
        };
    }]);

csapp.controller('payoutSubpolicyCtrl', ['$scope', 'payoutSubpolicyDataLayer', '$modal', '$csModels', '$csShared', "$csnotify", "$csfactory",
    function ($scope, datalayer, $modal, $csModels, $csShared, $csnotify, $csfactory) {

        $scope.getSubpolicyList = function (selectedParams) {

            if ($csfactory.isNullOrEmptyString(selectedParams.Products)
                || $csfactory.isNullOrEmptyString(selectedParams.PolicyType))
                return;

            datalayer.getSubpolicyList(selectedParams).then(function (data) {

                if (data.length === 0) {
                    $csnotify.success("subpolicy dosen't exists");
                    return;
                }

                $scope.subpolicyList = _.filter(data, { PayoutSubpolicyType: 'Subpolicy' });
                $scope.formulaList = _.filter(data, { PayoutSubpolicyType: 'Formula' });
                //get matrixList
                datalayer.getMatrixList(selectedParams.Products).then(function (matrixData) {
                    $scope.matrixList = matrixData;
                });
            });
            $scope.subpolicy.Products = selectedParams.Products;
            $scope.subpolicy.PolicyType = selectedParams.PolicyType;

        };

        var combineTokens = function (selectedTokens) {
            return _.union(selectedTokens.conditionTokens,
                selectedTokens.ifOutputTokens,
                selectedTokens.ElseOutputTokens);
        };

        var divideTokens = function (tokensList) {
            $scope.selectedTokens.conditionTokens = _.filter(tokensList, { 'GroupType': 'Condition' });
            $scope.selectedTokens.ifOutputTokens = _.filter(tokensList, { 'GroupType': 'Output' });
            $scope.selectedTokens.ElseOutputTokens = _.filter(tokensList, { 'GroupType': 'ElseOutput' });
        };

        $scope.saveSubPolicy = function (subpolicy, selectedTokens) {
            subpolicy.PayoutSubpolicyType = 'Subpolicy';
            subpolicy.Category = 'Liner';
            subpolicy.BillTokens = combineTokens(selectedTokens);
            datalayer.saveSubpolicy(subpolicy).then(function (data) {
                $scope.subpolicyList.push(data);
                $scope.resetSubPolicy();
            });
        };

        $scope.resetSubPolicy = function (products) {
            $scope.subpolicy = {
                Products : products
            };
            $scope.selected = [];
            $scope.selectedTokens = {
                conditionTokens: [],
                ifOutputTokens: [],
                ElseOutputTokens: []
            };
            $scope.subpolicyForm.$setPristine();
        };

        $scope.selectSubpolicy = function (subpolicy) {
            $scope.showDiv = true;
            $scope.subpolicy = subpolicy;
            divideTokens(subpolicy.BillTokens);
        };

        $scope.addSubPolicy = function (selectedParams, form) {
            $scope.showDiv = true;
            $scope.resetSubPolicy(selectedParams.Products, form);
            $scope.subpolicy.PolicyType = selectedParams.PolicyType;
        };

        var init = function () {
            $scope.sele = {};
            $scope.subpolicy = {
                PayoutSubpolicyType: 'Subpolicy'
            };
            $scope.subpolicyList = [];
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
            $scope.payoutSubpolicy.PolicyTypeText = { type: "text", label: "Policy Type" };
            $scope.payoutSubpolicy.Products.valueList = _.reject($csShared.enums.Products, function (item) {
                return (item === "UNKNOWN" || item === "ALL");
            });
            init();
        })();
    }]);
