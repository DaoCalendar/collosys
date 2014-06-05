
csapp.factory("newpolicyDatalayer", ['Restangular', '$csnotify', function (rest, $csnotify) {

    var restApi = rest.all("BillingPolicyApi");
    var dldata = {};

    var getStakeholderOrHier = function (policyfor) {

        return restApi.customGET("GetStakeholerOrHier", { 'policyfor': policyfor }).then(function (data) {
            return data;
        });
    };

    var getPolicyList = function(product) {
        return restApi.customGET("GetBillingSubpolicyList", { 'product': product }).then(function (data) {
            if (data.IsInUseSubpolices.length === 0 && data.NotInUseSubpolices.length === 0) {
                $csnotify.success("SubPolices are not available");
            }
            return data;
        });
    };

    return {
        dldata: dldata,
        getStakeholderOrHier: getStakeholderOrHier,
        getPolicyList: getPolicyList
    };

}]);

csapp.controller("newpolicyController", ["$scope", "$csModels", "$csShared", "newpolicyDatalayer", "$csnotify", function ($scope, $csModels, $csShared, datalayer, $csnotify) {

    (function () {
        $scope.datalayer = datalayer;
        $scope.dldata = datalayer.dldata;
        $scope.policyForList = [];
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
      
        datalayer.getStakeholderOrHier(policyfor).then(function (data) {
            if (policyfor === 'Stakeholder') {
                $scope.policyForList = data;
            } else {
                $scope.policyForList = [];
                _.forEach(data, function (item) {
                    var obj = {
                        Hierarchy: item.Hierarchy + '(' + item.Designation + ')',
                        row: item
                    };
                    $scope.policyForList.push(obj);
                });
            }
        });
    };

    $scope.getSubpolicyList = function (product) {

        datalayer.getPolicyList(product).then(function (data) {
          
            $scope.ExpiredAndSubpolicy = data.NotInUseSubpolices;
            $scope.ApproveUnapproved = data.IsInUseSubpolices;
        });
    };

    $scope.displaySubpolicyDetails = function (policy) {

    };
    $scope.moveUp = function () {

    };

    $scope.moveDown = function () {

    };
  
}]);