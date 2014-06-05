
csapp.factory("newpolicyDatalayer", ['Restangular', '$csnotify', function (rest, $csnotify) {

    var restApi = rest.all("BillingPolicyApi");
    var dldata = {};

    var getStakeholderOrHier = function (policyfor) {
        return restApi.customGET("GetStakeholerOrHier", { 'policyfor': policyfor }).then(function (data) {
            return data;
        });
    };

    var getPolicyList = function (product) {
        return restApi.customGET("GetBillingSubpolicyList", { 'product': product }).then(function (data) {
            if (data.IsInUseSubpolices.length === 0 && data.NotInUseSubpolices.length === 0) {
                $csnotify.success("SubPolices are not available");
            }
            return data;
        });
    };

    var displaySubpolicyDetails = function (subpolicy) {
        return restApi.customGETLIST("GetBillingTokens", { 'id': subpolicy.Id }).then(function (data) {
            return data;
        });
    };

    var saveSubpolicylist = function(subpolicy) {
        return restApi.customPOST("SaveSubpolicy",subpolicy).then(function(data) {
            return data;
        });
    };

    return {
        dldata: dldata,
        getStakeholderOrHier: getStakeholderOrHier,
        getPolicyList: getPolicyList,
        displaySubpolicyDetails: displaySubpolicyDetails,
        saveSubpolicylist:saveSubpolicylist
    };

}]);

csapp.controller("newpolicyController", ["$scope", "$csModels", "$csShared", "newpolicyDatalayer", "$csnotify", "$modal", function ($scope, $csModels, $csShared, datalayer, $csnotify, $modal) {

    (function () {
        $scope.datalayer = datalayer;
        $scope.dldata = datalayer.dldata;
        $scope.policyForList = [];
        $scope.billingRelation = {};
        $scope.buttonStatus = "";
        $scope.displaySubPolicy = {
            conditionTokens: [],
            ifOutputTokens: [],
            ElseOutputTokens: []
        };
        $scope.billingpolicy = {
            Products: "",
            PolicyFor: "",
            PolicyForId: "",
            BillingRelations: {},
            BillTokens: [],
        };
        $scope.direction = {
            up: true,
            down:true
        };
        $scope.pageData = {
            Subpolicy: {},
            StartDate: "",
            EndDate: "",
            ForActive: "",
            subPolicyIndex:-1
        };
        $scope.BillingPolicyModel = $csModels.getColumns("BillingPolicy");
        $scope.BillingPolicyModel.Products.valueList = _.reject($csShared.enums.Products, function (item) {
            return (item === "UNKNOWN" || item === "ALL");
        });
    })();

    $scope.onProductChange = function () {
        $scope.billingpolicy.PolicyFor = "";
        $scope.billingpolicy.PolicyType = "";
        $scope.billingpolicy.PolicyForId = "";
        $scope.buttonStatus = "";
    };

    $scope.onPolicyTypeChange = function () {
        $scope.billingpolicy.PolicyFor = "";
        $scope.billingpolicy.PolicyForId = "";
        $scope.buttonStatus = "";
    };

    $scope.getStakeholderOrHierarchy = function (policyfor) {
        if (policyfor === 'Product') {
            $scope.getSubpolicyList($scope.billingpolicy.Products);
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

    $scope.displaySubpolicyDetails = function (subpolicy,index) {
        $scope.setButtonStatus(subpolicy.BillingRelations[0]);

        $scope.manageUpDownArrow(subpolicy.BillingRelations[0],index);

        datalayer.displaySubpolicyDetails(subpolicy).then(function (data) {
            $scope.billingpolicy.BillTokens = data;
            $scope.displaySubPolicy = {
                conditionTokens: _.filter(data, { 'GroupType': 'Condition' }),
                ifOutputTokens: _.filter(data, { 'GroupType': 'Output' }),
                ElseOutputTokens: _.filter(data, { 'GroupType': 'ElseOutput' })
            };
        });
    };

    $scope.setButtonStatus = function (relation) {
        if (angular.isUndefined(relation)) {
            $scope.buttonStatus = 'Draft';
            return;
        }
        if (relation.Status === 'Approved') {
            $scope.buttonStatus = relation.Status;
        }
        var today = moment();
        var endDate = moment(relation.EndDate);
        var diff = endDate.diff(today, 'days');
        if ((relation.Status === 'Submitted') && (diff < 0)) {
            $scope.buttonStatus = 'Expired';
        }
        if ((relation.Status === 'Submitted') && (diff >= 0)) {
            $scope.buttonStatus = 'UnApproved';
        }
    };

    $scope.manageUpDownArrow = function (relation, index) {
        if (angular.isUndefined(relation)) {
            $scope.direction = {
                up: true,
                down: true
            };
            return;
        }
        var isinlist = _.find($scope.ApproveUnapproved, function (item) {
            if (item.BillingRelations[0].Id === relation.Id) {
                $scope.direction = {
                    up: false,
                    down: false
                };
                return item;
            }
        });
        if (angular.isUndefined(isinlist)) {
            $scope.direction = {
                up: true,
                down: true
            };
        } else {
            if (index === 0) {
                $scope.direction.up = true;
            }
            var maxindex = ($scope.ApproveUnapproved.length) - 1;
            if (maxindex === index) {
                $scope.direction.down = true;
            }
        }
    };

    $scope.moveUp = function (subpolicy) {
        var index = $scope.ApproveUnapproved.indexOf(subpolicy);
        var tempPriority = $scope.ApproveUnapproved[index].BillingRelations[0].Priority;
        $scope.ApproveUnapproved[index].BillingRelations[0].Priority = $scope.ApproveUnapproved[index - 1].BillingRelations[0].Priority;
        $scope.ApproveUnapproved[index - 1].BillingRelations[0].Priority = tempPriority;
      //  $scope.saveSubpolicylist(subpolicy);
    };

    $scope.moveDown = function (subpolicy) {
        var index = $scope.ApproveUnapproved.indexOf(subpolicy);
        var tempPriority = $scope.ApproveUnapproved[index].BillingRelations[0].Priority;
        $scope.ApproveUnapproved[index].BillingRelations[0].Priority = $scope.ApproveUnapproved[index + 1].BillingRelations[0].Priority;
        $scope.ApproveUnapproved[index + 1].BillingRelations[0].Priority = tempPriority;
       // $scope.saveSubpolicylist(subpolicy);
    };

    $scope.saveSubpolicylist = function(subpolicy) {
        datalayer.saveSubpolicylist(subpolicy).then(function() {
            $csnotify.success("SubPolices saved");

        });
    };

    $scope.approve = function (relation) {
       
    };
    $scope.reject = function(relation) {
        
    };

    $scope.openModelforDraftSubPolicy = function(subpolicy) {
        $scope.buttonStatus = null;
        $scope.pageData.Subpolicy = subpolicy;
        var indexl = $scope.ExpiredAndSubpolicy.indexOf(subpolicy);
        $scope.pageData.subPolicyIndex = indexl;
        $scope.pageData.StartDate = null;
        $scope.pageData.EndDate = null;
        $scope.pageData.ForActive = true;
        openmodal($scope.pageData);
    };
    var openmodal = function (pageData) {
        $modal.open({
            templateUrl: baseUrl + 'Billing/newpolicy/date-modalPopUp.html',
            controller: 'billingPolicymodal',
            size: 'lg',
            resolve: {
                pageData: function () {
                    return pageData;
                }
            }
        });
    };
}]);

csapp.controller("billingPolicymodal", ['$scope', 'pageData', '$modalInstance', 'newpolicyDatalayer', '$csModels',
    function ($scope, pagedata, $modalInstance, datalayer, $csModels) {

        $scope.pageData = pagedata;
        $scope.dldata = datalayer.dldata;
        $scope.BillingPolicy = $csModels.getColumns("BillingPolicy");
        $scope.dldata.isModalDateValid = false;

    $scope.closeModal = function () {
        $modalInstance.dismiss();
    };
}]);