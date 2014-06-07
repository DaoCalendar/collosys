
csapp.factory("newpolicyDatalayer", ['Restangular', '$csnotify', function (rest, $csnotify) {

    var restApi = rest.all("PayoutPolicyApi");
    var dldata = {};
    dldata.buttonStatus = "";

    var getStakeholderOrHier = function (policydto) {
        return restApi.customGET("GetStakeholerOrHier", { 'policy': policydto })
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

csapp.controller("newpolicyController", ["$scope", "$csfactory", "$csModels", "$csShared", "newpolicyDatalayer", "$csnotify", "$modal", "modalService",
    function ($scope, $csfactory, $csModels, $csShared, datalayer, $csnotify, $modal, modalService) {

        (function () {
            $scope.direction = { up: true, down: true };
            $scope.billingpolicy = {};
            $scope.buttonStatus = "";
            $scope.BillingPolicyModel = $csModels.getColumns("BillingPolicy");
            $scope.BillingPolicyModel.startDateText = { label: "Start date", type: "text" };
            $scope.BillingPolicyModel.endDateText = { label: "End date", type: "text" };
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

                $scope.config.lhsValueList = data.NotInUseSubpolices;
                $scope.config.rhsValueList = data.IsInUseSubpolices;
                $scope.config.lhsHeading = "Draft/Expired";
                $scope.config.rhsHeading = "Approved/Unapproved";
                $scope.config.lhsTextField = "Name";
                $scope.config.rhsTextField = "Name";
                $scope.config.showRightLeftButtons = false;
            });
        };

        $scope.displaySubpolicyDetails = function () {
            datalayer.displaySubpolicyDetails($scope.selected.selectedItem).then(function (data) {
                $scope.billingpolicy.BillTokens = data;
                $scope.displaySubPolicy = {
                    conditionTokens: _.filter(data, { 'GroupType': 'Condition' }),
                    outputTokens: _.filter(data, { 'GroupType': 'Output' }),
                };
            });

            $scope.setButtonStatus();
        };

        $scope.setButtonStatus = function () {
            var relation = $scope.selected.selectedItem.BillingRelations[0];
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
            if ((diff < 0)) {
                $scope.buttonStatus = 'Expired';
            }
            if ((relation.Status === 'Submitted') && (diff >= 0)) {
                $scope.buttonStatus = 'UnApproved';
            }
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
                bodyText: 'Are you sure you want to ' + activity + ' Subpolicy : ' + subpolicy + '?'
            };
            modalService.showModal({}, modalOptions).then(function () {
                $scope.BillingSubpolicyForsave = {
                    Activity: activity,
                    Subpolicy: subpolicy,
                    Policy: $scope.billingpolicy.policy
                };
                datalayer.saveSubpolicylist($scope.BillingSubpolicyForsave).then(function () {
                    $csnotify.success("Subpolicy saved");
                });
            });
        };

        $scope.openModelforSubPolicy = function (activity, selected) {
            var modalInstance = $modal.open({
                templateUrl: baseUrl + 'Billing/policy/date-modal.html',
                controller: 'billingPolicymodal',
                size: 'modal-large',
                resolve: {
                    pageData: function () {
                        return {
                            Activity: activity,
                            Subpolicy: selected.selectedItem
                        };
                    }
                }
            });

            modalInstance.result.then(function (data) {
                $scope.BillingSubpolicyForsave = {
                    Activity: activity,
                    Subpolicy: subpolicy,
                    Policy: $scope.billingpolicy.policy,
                    StartDate: data.startDate,
                    EndDate: data.endDate
                };
                datalayer.saveSubpolicylist($scope.BillingSubpolicyForsave).then(function () {
                    $csnotify.success("Subpolicy saved");
                });
            });

        };
    }]);

csapp.controller("billingPolicymodal", ['$scope', 'pageData', '$modalInstance', '$csModels', '$csnotify',
    function ($scope, pageData, $modalInstance, $csModels, $csnotify) {

        (function () {
            $scope.pageData = pageData;
            $scope.BillingPolicy = $csModels.getColumns("BillingPolicy");
        })();

        $scope.modelDateValidation = function (startDate, endDate) {
            if (angular.isUndefined(endDate) || endDate == null) {
                $scope.isModalDateValid = true;
                return;
            }
            startDate = moment(startDate);
            endDate = moment(endDate);
            $scope.isModalDateValid = (endDate > startDate);
            if ($scope.isModalDateValid === false) {
                $csnotify.success("EndDate should be Greater Than StartDate");
            }
        };

        $scope.closeModal = function (params) {
            $modalInstance.close(params);
        };

        $scope.dismissModal = function () {
            $modalInstance.dismiss();
        };

    }]);