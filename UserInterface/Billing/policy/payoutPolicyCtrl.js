
csapp.factory("newpolicyDatalayer", ['Restangular', '$csnotify', function (rest, $csnotify) {

    var restApi = rest.all("PayoutPolicyApi");
    var dldata = {};
    dldata.buttonStatus = "";

    var getStakeholderOrHier = function (policyfor) {
        return restApi.customGET("GetStakeholerOrHier", { 'policyfor': policyfor })
            .then(function (data) {
                return data;
            });
    };

    var getPolicyList = function (product) {
        return restApi.customGET("GetBillingSubpolicyList", { 'product': product })
            .then(function (data) {
                if (data.IsInUseSubpolices.length === 0 && data.NotInUseSubpolices.length === 0) {
                    $csnotify.success("No subpolices defined.");
                }
                return data;
            });
    };

    var displaySubpolicyDetails = function (subpolicy) {
        return restApi.customGETLIST("GetBillingTokens", { 'id': subpolicy.Id })
            .then(function (data) {
                return data;
            });
    };

    var saveSubpolicylist = function (subpolicy) {
        return restApi.customPOST("SaveSubpolicy", subpolicy)
            .then(function (data) {
                return data;
            });
    };

    return {
        dldata: dldata,
        getStakeholderOrHier: getStakeholderOrHier,
        getPolicyList: getPolicyList,
        displaySubpolicyDetails: displaySubpolicyDetails,
        saveSubpolicylist: saveSubpolicylist
    };

}]);

csapp.factory("newpolicyFactory", function () {

});

csapp.controller("newpolicyController", ["$scope", "$csfactory", "$csModels", "$csShared", "newpolicyDatalayer", "$csnotify", "$modal", "modalService",
    function ($scope, $csfactory, $csModels, $csShared, datalayer, $csnotify, $modal, modalService) {

        (function () {
            $scope.direction = { up: true, down: true };
            $scope.BillingPolicyModel = $csModels.getColumns("BillingPolicy");
            $scope.BillingPolicyModel.Products.valueList = _.reject($csShared.enums.Products, function (item) {
                return (item === "UNKNOWN" || item === "ALL");
            });
        })();

        $scope.onParamChange = function (changed) {
            switch (changed) {
                case "product":
                    $scope.billingpolicy.PolicyType = "";
                case "policy":
                    $scope.billingpolicy.PolicyFor = "";
                    $scope.billingpolicy.PolicyForId = "";
                    $scope.dldata.buttonStatus = "";
                    break;
            }
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

        $scope.displaySubpolicyDetails = function (subpolicy, index) {
            datalayer.displaySubpolicyDetails(subpolicy).then(function (data) {
                $scope.billingpolicy.BillTokens = data;
                $scope.displaySubPolicy = {
                    conditionTokens: _.filter(data, { 'GroupType': 'Condition' }),
                    outputTokens: _.filter(data, { 'GroupType': 'Output' }),
                };
            });

            $scope.setButtonStatus(subpolicy.BillingRelations[0]);
            $scope.manageUpDownArrow(subpolicy.BillingRelations[0], index);
        };

        $scope.setButtonStatus = function (relation) {
            if (angular.isUndefined(relation)) {
                $scope.dldata.buttonStatus = 'Draft';
                return;
            }
            if (relation.Status === 'Approved') {
                $scope.dldata.buttonStatus = relation.Status;
            }
            var today = moment();
            var endDate = moment(relation.EndDate);
            var diff = endDate.diff(today, 'days');
            if ((diff < 0)) {
                $scope.dldata.buttonStatus = 'Expired';
            }
            if ((relation.Status === 'Submitted') && (diff >= 0)) {
                $scope.dldata.buttonStatus = 'UnApproved';
            }
        };

        $scope.manageUpDownArrow = function (relation, index) {
            if (angular.isUndefined(relation)) {
                $scope.direction = { up: true, down: true };
                return;
            };

            var isinlist = _.find($scope.ApproveUnapproved, function (item) {
                return (item.BillingRelations[0].Id === relation.Id);
            });

            if (angular.isUndefined(isinlist)) {
                $scope.direction = { up: true, down: true };
                return;
            }

            $scope.direction.up = index === 0;
            $scope.direction.down = $scope.ApproveUnapproved.length === (index + 1);
        };

        $scope.moveUp = function (subpolicy) {
            var index = $scope.ApproveUnapproved.indexOf(subpolicy);
            var tempPriority = $scope.ApproveUnapproved[index].BillingRelations[0].Priority;
            $scope.ApproveUnapproved[index].BillingRelations[0].Priority = $scope.ApproveUnapproved[index - 1].BillingRelations[0].Priority;
            $scope.ApproveUnapproved[index - 1].BillingRelations[0].Priority = tempPriority;
            $scope.saveSubpolicylist(subpolicy);
        };

        $scope.moveDown = function (subpolicy) {
            var index = $scope.ApproveUnapproved.indexOf(subpolicy);
            var tempPriority = $scope.ApproveUnapproved[index].BillingRelations[0].Priority;
            $scope.ApproveUnapproved[index].BillingRelations[0].Priority = $scope.ApproveUnapproved[index + 1].BillingRelations[0].Priority;
            $scope.ApproveUnapproved[index + 1].BillingRelations[0].Priority = tempPriority;
            $scope.saveSubpolicylist(subpolicy);
        };

        $scope.saveSubpolicylist = function (subpolicy) {
            datalayer.saveSubpolicylist(subpolicy).then(function () {
                $csnotify.success("SubPolices saved");

            });
        };

        $scope.approveORreject = function (activity, subpolicy) {
            var modalOptions = {
                actionButtonText: 'Yes',
                closeButtonText: 'Cancel',
                headerText: activity + ' Subpolicy',
                bodyText: 'Are you sure you want to ' + activity + ' Subpolicy : ' + subpolicy.Name + '?'
            };
            modalService.showModal({}, modalOptions).then(function () {
                datalayer.save({
                    Activity: activity,
                    Subpolicy: subpolicy,
                    Policy: $scope.billingpolicy.policy
                });
            });
        };

        $scope.openModelforSubPolicy = function (activity, subpolicy) {
            var modalInstance = $modal.open({
                templateUrl: baseUrl + 'Billing/policy/date-modal.html',
                controller: 'billingPolicymodal',
                size: 'modal-large',
                resolve: {
                    pageData: function() {
                        return {
                            Activity: activity,
                            Subpolicy: subpolicy
                        };
                    }
                }
            });

            modalInstance.result.then(function(data) {
                datalayer.save({
                    Activity: activity,
                    Subpolicy: subpolicy,
                    Policy: $scope.billingpolicy.policy,
                    StartDate: data.startDate,
                    EndDate: data.endDate
                });
            });

        };
    }]);

csapp.controller("billingPolicymodal", ['$scope', 'pageData', '$modalInstance', '$csModels',
    function ($scope, pageData, $modalInstance, datalayer, $csModels) {

        (function () {
            $scope.pageData = pageData;
            $scope.BillingPolicy = $csModels.getColumns("BillingPolicy");
        })();

        $scope.modelDateValidation = function (startDate, endDate) {
            if (angular.isUndefined(endDate) || endDate == null) {
                $scope.dldata.isModalDateValid = true;
                return;
            }
        };

        $scope.closeModal = function (params) {
            $modalInstance.close(params);
        };

        $scope.dismissModal = function () {
            $modalInstance.dismiss();
        };

    }]);