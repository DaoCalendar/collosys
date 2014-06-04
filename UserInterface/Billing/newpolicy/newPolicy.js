
csapp.factory("newpolicyDatalayer", ['Restangular', function (rest) {

    var restApi = rest.all("NewpolicyApi");
    var dldata = {};

    var getStakeholderOrHier = function (policyfor) {

        return restApi.customGET("GetStakeholerOrHier", { 'policyfor': policyfor }).then(function (data) {
            return data;
        });
    };

    var getPolicyList = function() {
        return restApi.customGET("GetBillingPolicyList").then(function(data) {
            return data;
        });
    };

    return {
        dldata: dldata,
        getStakeholderOrHier: getStakeholderOrHier,
        getPolicyList: getPolicyList
    };

}]);

csapp.controller("newpolicyController", ["$scope", "$csModels", "$csShared", "newpolicyDatalayer", function ($scope, $csModels, $csShared, datalayer) {

    (function () {
        $scope.datalayer = datalayer;
        $scope.dldata = datalayer.dldata;
        $scope.BillingPolicyModel = $csModels.getColumns("BillingPolicy");
        $scope.BillingPolicyModel.Products.valueList = _.reject($csShared.enums.Products, function (item) {
            return (item === "UNKNOWN" || item === "ALL");
        });
    })();

    $scope.onProductChange = function () {
    };

    $scope.onPolicyTypeChange = function () {
    };

    $scope.getStakeholderOrHierarchy = function (policyfor) {
        if (policyfor === 'Product') {
            return;
        }
        if (policyfor === "Stakeholder") {
            $scope.BillingPolicyModel.PolicyForGuid.label = "Stakeholder";
        } else {
            $scope.BillingPolicyModel.PolicyForGuid.label = "Hierarchy";
        }

        datalayer.getStakeholderOrHier(policyfor).then(function (data) {
            $scope.policyForList = data;
        });
    };

    $scope.getSubpolicyList = function () {

        datalayer.getPolicyList().then(function (data) {
            
        });
    };

    $scope.displaySubpolicyDetails = function (policy) {

    };
    $scope.moveUp = function () {

    };

    $scope.moveDown = function () {

    };
  
}]);