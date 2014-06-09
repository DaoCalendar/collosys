
csapp.factory("newpolicyDatalayer", ['Restangular', '$csnotify', function (rest, $csnotify) {

    var restApi = rest.all("PayoutPolicyApi");
    var dldata = {};

    var getStakeholderOrHier = function (policy) {
        return restApi.customPOST(policy, "GetStakeholerOrHier")
            .then(function (data) {
                return data;
            });
    };

    var getPolicyList = function (policy) {
        return restApi.customPOST(policy, "GetBillingSubpolicyList")
            .then(function (data) {
                policy.PolicyId = data.PolicyId;
                if (data.IsInUseSubpolices.length === 0 && data.NotInUseSubpolices.length === 0) {
                    $csnotify.success("No subpolices defined.");
                    policy.IsInUseSubpolices = [];
                    policy.NotInUseSubpolices = [];
                    return policy;
                } else {
                    policy.IsInUseSubpolices = data.IsInUseSubpolices;
                    policy.NotInUseSubpolices = data.NotInUseSubpolices;
                }
                return policy;
            });
    };

    var displaySubpolicyDetails = function (subpolicyId) {
        return restApi.customGETLIST("GetBillingTokens", { 'id': subpolicyId })
            .then(function (data) {
                return data;
            });
    };

    var saveSubpolicylist = function (subpolicy) {
        return restApi.customPOST(subpolicy, "SaveSubpolicy")
            .then(function (data) {
                return data;
            });
    };

    return {
        dldata: dldata,
        getStakeholderOrHier: getStakeholderOrHier,
        getPolicyList: getPolicyList,
        displaySubpolicyDetails: displaySubpolicyDetails,
        Save: saveSubpolicylist
    };

}]);

csapp.controller("newpolicyController", ["$scope", "$csfactory", "$csModels", "$csShared", "newpolicyDatalayer", "$csnotify", "$modal", "modalService", "Logger",
    function ($scope, $csfactory, $csModels, $csShared, datalayer, $csnotify, $modal, modalService, logManager) {

        (function () {
            $scope.$log = logManager.getInstance("PolicyController");
            $scope.billingpolicy = {};
            $scope.BillingPolicyModel = $csModels.getColumns("BillingPolicy");
            $scope.BillingPolicyModel.Products.valueList = _.reject($csShared.enums.Products, function (item) {
                return (item.toUpperCase() === "UNKNOWN" || item.toUpperCase() === "ALL");
            });
            $scope.displaySubPolicy = {
                conditionTokens: [],
                outputTokens: []
            };
            $scope.config = {};
            $scope.selected = {};
        })();

        $scope.onParamChange = function (changed) {
            switch (changed) {
                case "product":
                    $scope.billingpolicy.PolicyType = undefined;
                case "policy":
                    $scope.billingpolicy.PolicyFor = undefined;
                    $scope.billingpolicy.PolicyForId = undefined;
                    $scope.buttonStatus = undefined;
                    break;
                default:
                    break;
            }
        };

        $scope.getStakeholderOrHierarchy = function (policyDto) {
            if (policyDto.PolicyFor === 'Product') {
                $scope.getSubpolicyList(policyDto);
                return;
            }
            datalayer.getStakeholderOrHier(policyDto).then(function (data) {
                $scope.policyForList = data;
            });
        };

        $scope.getSubpolicyList = function (policyDto) {
            datalayer.getPolicyList(policyDto).then(function () {
                $scope.config.lhsValueList = policyDto.NotInUseSubpolices;
                $scope.config.rhsValueList = policyDto.IsInUseSubpolices;
                $scope.config.lhsHeading = "Draft/Expired";
                $scope.config.rhsHeading = "Approved/Unapproved";
                $scope.config.lhsTextField = "Name";
                $scope.config.rhsTextField = "Name";
                $scope.config.showRightLeftButtons = false;
            });
        };

        $scope.displaySubpolicyDetails = function () {
            datalayer.displaySubpolicyDetails($scope.selected.selectedItem.SubpolicyId).then(function (data) {
                $scope.selected.selectedItem.BillTokens = data;
                $scope.displaySubPolicy = {
                    conditionTokens: _.filter(data, { 'GroupType': 'Condition' }),
                    outputTokens: _.filter(data, { 'GroupType': 'Output' }),
                };
            });
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

        $scope.approveORreject = function (activity) {
            var modalOptions = {
                actionButtonText: activity,
                closeButtonText: 'Cancel',
                headerText: activity + ' Subpolicy',
                bodyText: 'Are you sure you want to ' + activity + ' Subpolicy : '
                    + $scope.selected.selectedItem.Name + '?'
            };
            modalService.showModal({}, modalOptions).then(function () {
                $scope.selected.selectedItem.Activity = activity;
                datalayer.Save($scope.selected.selectedItem).then(function (data) {
                    manageSubpolicyActivity(activity, data);
                });
            });
        };

        var isExpireRequestPending = function () {
            var subpolicyId = $scope.selected.selectedItem.SubpolicyId;
            var items = _.find($scope.config.rhsValueList, function (item) {
                return item.ApproveStatus === "Submitted" && item.SubpolicyId === subpolicyId;
            });

            if (!angular.isUndefined(items)) {
                $csnotify.error("Expire request already pending");
                return true;
            }
            return false;
        };

        var manageSubpolicyActivity = function (activity, data) {
            switch (activity) {
                case "Reactivate":
                case "Activate":
                    var policy = $scope.config.lhsValueList[$scope.selected.selectedItemIndex];
                    $scope.config.lhsValueList.splice(policy, 1);
                    $scope.config.rhsValueList.push(data);
                    $scope.selected.selectedItem = data;
                    $scope.selected.selectedItemIndex = $scope.config.rhsValueList.length - 1;
                    $scope.selected.selectedSide = "rhs";
                    break;
                case "Expire":
                    $scope.config.rhsValueList.push(data);
                    $scope.config.rhsValueList = _.sortBy($scope.config.rhsValueList, 'Priority');
                    $scope.selected.selectedItemIndex = _.indexOf($scope.config.rhsValueList, data);
                    $scope.selected.selectedItem = $scope.config.rhsValueList[$scope.selected.selectedItemIndex];
                    break;
                case "Approve":
                    $scope.config.rhsValueList.splice($scope.selected.selectedItem, 1);
                    var item = _.find($scope.config.rhsValueList, function (input) {
                        return input.SubpolicyId === $scope.selected.selectedItem.SubpolicyId
                            && input.RelationId !== $scope.selected.selectedItem.RelationId;
                    });
                    if (!angular.isUndefined(item)) {
                        $scope.config.rhsValueList.splice(item, 1);
                    }
                    $scope.config.rhsValueList.push(data);
                    $scope.config.rhsValueList = _.sortBy($scope.config.rhsValueList, 'Priority');
                    $scope.selected.selectedItemIndex = _.indexOf($scope.config.rhsValueList, data);
                    $scope.selected.selectedItem = data;
                    break;
                case "Reject":
                    var index = $scope.config.rhsValueList.indexOf($scope.selected.selectedItem);
                    $scope.config.rhsValueList.splice(index,1);
                    $scope.selected.selectedItem = undefined;
                    $scope.selected.selectedItemIndex = -1;
                    $scope.selected.selectedSide = undefined;
                    break;
                default:
                    $log.error("invalid activity : " + activity);
            }
        };

        $scope.openModelforSubPolicy = function (activity) {

            if (activity === "Expire" && isExpireRequestPending()) return;

            var modalInstance = $modal.open({
                templateUrl: baseUrl + 'Billing/policy/date-modal.html',
                controller: 'billingPolicymodal',
                size: 'modal-large',
                resolve: {
                    pageData: function () {
                        return {
                            Activity: activity,
                            Subpolicy: $scope.selected.selectedItem
                        };
                    }
                }
            });

            modalInstance.result.then(function (data) {
                $scope.selected.selectedItem.StartDate = data.StartDate;
                $scope.selected.selectedItem.EndDate = data.EndDate;

                if (activity === "Activate" || activity === "Reactivate") {
                    if ($scope.config.rhsValueList.length === 0) {
                        $scope.selected.selectedItem.Priority = 0;
                    } else {
                        $scope.selected.selectedItem.Priority = ($scope.config.rhsValueList[$scope.config.rhsValueList.length - 1].Priority) + 1;
                    }
                }

                $scope.selected.selectedItem.Activity = activity;
                datalayer.Save($scope.selected.selectedItem).then(function (data2) {
                    manageSubpolicyActivity(activity, data2);
                });
            });
        };
    }]);

csapp.controller("billingPolicymodal", ['$scope', 'pageData', '$modalInstance',
    function ($scope, pageData, $modalInstance) {

        (function () {
            $scope.pageData = pageData;
            $scope.model = {
                StartDate: { label: "Start Date", type: "date", required: true },
                EndDate: { label: "End Date", type: "date", required: pageData.Activity === "Expire", startDate: "tomorrow" }
            };
            if (pageData.Subpolicy.SubpolicyType === "Draft") {
                pageData.Subpolicy.StartDate = moment().add('d', 1).format("YYYY-MM-DD");
            }
        })();

        $scope.modelDateValidation = function () {
            if (angular.isUndefined(pageData.Subpolicy.EndDate) || pageData.Subpolicy.EndDate == null) {
                return;
            }
            var startDate = moment(pageData.Subpolicy.StartDate);
            var endDate = moment(pageData.Subpolicy.EndDate);
            if (endDate.isBefore(startDate)) {
                $scope.dateRangeForm.$setValidity("daterange", false);
            }
        };

        $scope.closeModal = function () {
            $modalInstance.close(pageData.Subpolicy);
        };

        $scope.dismissModal = function () {
            $modalInstance.dismiss();
        };
    }]);