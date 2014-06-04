
csapp.controller('policymodal', ['$scope', 'modaldata', '$modalInstance', 'payoutPolicyFactory', 'payoutPolicyDataLayer', '$csModels',
    function ($scope, modaldata, $modalInstance, factory, datalayer, $csModels) {
        $scope.modelData = modaldata;
        $scope.dldata = datalayer.dldata;
        $scope.BillingPolicy = $csModels.getColumns("BillingPolicy");
       // $scope.dldata.isModalDateValid = false;

        $scope.activateSubPoicy = function (modalData) {
            var maxPriorityPolicy = _.max($scope.dldata.payoutPolicy.BillingRelations, 'Priority');
            modalData.BillingRelations.Priority = maxPriorityPolicy.Priority + 1;
            modalData.BillingRelations.StartDate = modalData.startDate;
            modalData.BillingRelations.EndDate = modalData.endDate;
            modalData.BillingRelations.Status = "Submitted";

            if (modalData.subPolicyIndex > -1) {
                $scope.dldata.payoutPolicy.BillingRelations.push(JSON.parse(JSON.stringify(modalData.BillingRelations)));
                $scope.dldata.subPolicyList.splice(modalData.subPolicyIndex, 1);
            }
            //datalayer.savePayoutPolicy($scope.dldata.payoutPolicy).then
            //(function () {
            //    $modalInstance.close();
            //});

        };

        $scope.deactivateSubPoicy = function (modalData) {
            modalData.payoutRelation.StartDate = modalData.startDate;
            modalData.payoutRelation.EndDate = modalData.endDate;
            $scope.dldata.payoutPolicy.BillingRelations.push(JSON.parse(JSON.stringify(modalData.payoutRelation)));
            datalayer.savePayoutPolicy($scope.dldata.payoutPolicy).then(function () {
                $modalInstance.close();
            });
        };

        $scope.modelDateValidation = function (startDate, endDate) {
            if (angular.isUndefined(endDate) || endDate == null) {
                $scope.dldata.isModalDateValid = true;
                return;
            }

            startDate = moment(startDate);
            endDate = moment(endDate);
            $scope.dldata.isModalDateValid = (endDate >= startDate);
        };

        $scope.closeModal = function () {
            $modalInstance.dismiss();
        };
    }]);

csapp.factory('payoutPolicyFactory', [
    '$csfactory', 'payoutPolicyDataLayer',
    function ($csfactory, datalayer) {
        var dldata = datalayer.dldata;

        var getDisplaySubPolicy = function (subPolicy) {
            var displaySubPolicy = {};
            displaySubPolicy.Name = subPolicy.Name;
            displaySubPolicy.conditionTokens = _.filter(subPolicy.BillTokens, { 'GroupType': 'Condition' });
            displaySubPolicy.ifOutputTokens = _.filter(subPolicy.BillTokens, { 'GroupType': 'Output' });
            displaySubPolicy.ElseOutputTokens = _.filter(subPolicy.BillTokens, { 'GroupType': 'ElseOutput' });
            //displaySubPolicy.Condition = getOuputConditionString(subPolicy, 'Condition');
            //displaySubPolicy.Output = getOuputConditionString(subPolicy, 'Output');
            return displaySubPolicy;
        };

        var getOuputConditionString = function (subPolicy, conditionType) {
            var conditionList = _.sortBy(_.filter(subPolicy.BConditions, function (bcond) { return (bcond.ConditionType == conditionType); }), 'Priority');

            var conditionString = "";
            for (var i = 0; i < conditionList.length; i++) {
                if (conditionType == 'Condition') {
                    var condition = conditionList[i];
                    conditionString = conditionString + condition.RelationType + ' ( ' + condition.LtypeName + ' ' + condition.Operator + ' ' + condition.RtypeName +
                                                                                            (condition.Rvalue) + ' ) ';
                } else {
                    var output = conditionList[i];
                    if (output.RtypeName) {
                        conditionString = conditionString +
                            (output.Operator == 'None' ? '' : output.Operator) +
                            (output.Lsqlfunction == 'None' ? '' : output.Lsqlfunction) +
                            '( ' + output.RtypeName + ' )';
                    } else {
                        conditionString =
                            conditionString +
                                (output.Operator == 'None' ? '' : output.Operator) +
                               (output.Lsqlfunction == 'None' ? '' : output.Lsqlfunction) +
                                '( ' + output.Rvalue + ' )';
                    }

                }
            }

            return conditionString;
        };

        var filterRelation = function (todayActive, status) {
            return function (relation) {
                var today = moment();
                // var startDate = moment(relation.StartDate);
                var endDate = relation.EndDate ? moment(relation.EndDate) : moment();
                var diff = endDate.diff(today, 'days');
                var dateFilter = ((diff >= 0) === todayActive);
                var statusfilter = true;
                if (status != '') {
                    statusfilter = relation.Status == status;
                }
                return (dateFilter && statusfilter);
            };
        };

        var setStatus = function (status) {
            if (status === 'Rejected') {
                dldata.color = { color: 'red' };
            }
            if (status === 'Approved') {
                dldata.color = { color: 'green' };
            }
            if (status === 'Submitted') {
                dldata.color = { color: 'blue' };
            }
            return status;
        };

        var relationValidToday = function (relation, todayActive) {
            var today = moment();
            // var startDate = moment(relation.StartDate);
            var endDate = relation.EndDate ? moment(relation.EndDate) : moment();
            return ((today <= endDate) === todayActive);
        };

        var upside = function (subPolicy, index) {
            var test = _.filter(dldata.payoutPolicy.BillingRelations, function (relation) { return relationValidToday(relation, true); });
            var relations = _.sortBy(test, 'Priority');

            var tempPriority = relations[index].Priority;
            relations[index].Priority = relations[index - 1].Priority;
            relations[index - 1].Priority = tempPriority;
            datalayer.savePayoutPolicy(dldata.payoutPolicy);
        };

        var downside = function (subPolicy, index) {
            var test = _.filter(dldata.payoutPolicy.BillingRelations, function (relation) { return relationValidToday(relation, true); });
            var relations = _.sortBy(test, 'Priority');

            var tempPriority = relations[index].Priority;
            relations[index].Priority = relations[index + 1].Priority;
            relations[index + 1].Priority = tempPriority;
            datalayer.savePayoutPolicy(dldata.payoutPolicy);
        };

        var reset = function () {
            dldata.payoutPolicy = {};
            dldata.payoutPolicy.Category = "Liner";
        };
        return {
            getDisplaySubPolicy: getDisplaySubPolicy,
            filterRelation: filterRelation,
            setStatus: setStatus,
            relationValidToday: relationValidToday,
            upside: upside,
            downside: downside,
            reset: reset,
        };
    }
]);

csapp.factory('payoutPolicyDataLayer', ['Restangular', '$csnotify', '$csfactory', function (rest, $csnotify, $csfactory) {
    var restApi = rest.all("PayoutPolicyApi");
    var dldata = {};
    dldata.subpolicylist = [];
    dldata.ExpiredAndSubpolicy = [];
    dldata.ApproveUnapproved = [];
    dldata.buttonStatus = "";
    dldata.Payoutsubpolicy = {
        Name: "",
        type: "",
        Priority: "",
        BillingRelations: {},
        subpolicy: {}
    };
    dldata.listOne = [];
    dldata.listTwo = [];
    dldata.listThree = [];
    dldata.listFour = [];
    dldata.categorySwitch = [{ Name: 'Collection', Value: 'Liner' }, { Name: 'Recovery', Value: 'WriteOff' }];

    var reset = function () {
        dldata.payoutPolicy = {};
        dldata.subPolicyList = [];
    };

    var changeProductCategory = function () {
        if (angular.isUndefined(dldata.payoutPolicy) || angular.isUndefined(dldata.payoutPolicy.Products) || angular.isUndefined(dldata.payoutPolicy.PolicyType))
            return;
        var payoutPolicy = dldata.payoutPolicy;
        var policyType = dldata.payoutPolicy.PolicyType;
        payoutPolicy.Category = 'Liner';
        if (!angular.isUndefined(payoutPolicy.Products) && !angular.isUndefined(payoutPolicy.Category)) {
            restApi.customGET("GetPayoutPolicy", { products: payoutPolicy.Products, category: payoutPolicy.Category }).then(function (data) {
                dldata.payoutPolicy = data.PayoutPolicy;
                dldata.payoutPolicy.PolicyType = policyType;
                dldata.payoutPolicy.PolicyFor = payoutPolicy.PolicyFor;
                dldata.payoutPolicy.PolicyForId = payoutPolicy.PolicyForId;
                dldata.subPolicyList = data.UnUsedSubpolicies;
                dldata.listOne = _.filter(dldata.payoutPolicy.BillingRelations, function (item) {
                    return filterRelation(item, false, '');
                });
                _.forEach(dldata.listOne, function (row) {
                    dldata.Payoutsubpolicy = {
                        Name: row.BillingSubpolicy.Name,
                        type: row.Status,
                        Priority: row.Priority,
                        BillingRelations: row,
                        subpolicy: row.BillingSubpolicy
                    };
                    dldata.listTwo.push(dldata.Payoutsubpolicy);
                });
                _.forEach(data.UnUsedSubpolicies, function (row) {
                    dldata.Payoutsubpolicy = {
                        Name: row.Name,
                        type: "",
                        BillingRelations: {},
                        subpolicy: row
                    };
                    dldata.listThree.push(dldata.Payoutsubpolicy);
                });
                dldata.ExpiredAndSubpolicy = _.union(dldata.listTwo, dldata.listThree);
                dldata.listOne = [];
                dldata.listTwo = [];
                dldata.listThree = [];
                dldata.listOne = _.filter(dldata.payoutPolicy.BillingRelations, function (item) {
                    return filterRelation(item, true, 'Submitted');
                });
                _.forEach(dldata.listOne, function (row) {
                    dldata.Payoutsubpolicy = {
                        Name: row.BillingSubpolicy.Name,
                        type: row.Status,
                        Priority: row.Priority,
                        BillingRelations: row,
                        subpolicy: row.BillingSubpolicy
                    };
                    dldata.listTwo.push(dldata.Payoutsubpolicy);
                });
                dldata.listThree = _.filter(dldata.payoutPolicy.BillingRelations, function (item) {
                    return filterRelation(item, true, 'Approved');
                });
                _.forEach(dldata.listThree, function (row) {
                    dldata.Payoutsubpolicy = {
                        Name: row.BillingSubpolicy.Name,
                        type: row.Status,
                        Priority: row.Priority,
                        BillingRelations: row,
                        subpolicy: row.BillingSubpolicy
                    };
                    dldata.listFour.push(dldata.Payoutsubpolicy);
                });
                dldata.ApproveUnapproved = _.union(dldata.listTwo, dldata.listFour);

            }, function (data) {
                $csnotify.error(data);
            });
        }
    };

    var filterRelation = function (relation, todayActive, status) {
        var today = moment();
        var endDate = relation.EndDate ? moment(relation.EndDate) : moment();
        var diff = endDate.diff(today, 'days');
        var dateFilter = ((diff >= 0) === todayActive);
        var statusfilter = true;
        if (status != '') {
            statusfilter = relation.Status == status;
        }
        return (dateFilter && statusfilter);

    };

    var resetList = function () {
        dldata.subpolicylist = [];
        dldata.ExpiredAndSubpolicy = [];
        dldata.ApproveUnapproved = [];
        dldata.buttonStatus = "";
        dldata.listOne = [];
        dldata.listTwo = [];
        dldata.listThree = [];
        dldata.listFour = [];
    };

    var getStakeHier = function () {

        return restApi.customGET("GetStakeHier").then(function (data) {
            dldata.hierarchy = data;
            return data;
        });
    };

    var rejectSubPolicy = function (rejectedRelation) {
        return restApi.customGET("RejectRelation", { relationId: rejectedRelation.Id }).then(function () {
            dldata.payoutPolicy.BillingRelations.splice(dldata.payoutPolicy.BillingRelations.indexOf(rejectedRelation), 1);
            dldata.subPolicyList.push(rejectedRelation.BillingSubpolicy);
            $csnotify.success("Subpolicy Rejected");
            return;
        }, function (data) {
            $csnotify.error(data);
        });
    };

    var approveRelation = function (relation) {
        return restApi.customGET('ApproveRelation', { relationId: relation.Id }).then(function () {
            $csnotify.success('Subpolicy Approved');
            return;
        });
    };

    var savePayoutPolicy = function (payoutPolicy) {
        var detelatedData = '';
        if (!$csfactory.isNullOrEmptyGuid(payoutPolicy.Id)) {
            var rejectedRelation = _.find(payoutPolicy.BillingRelations, { Status: 'Rejected' });
            if (angular.isDefined(rejectedRelation)) {
                detelatedData = angular.copy(rejectedRelation);
                payoutPolicy.BillingRelations.splice(payoutPolicy.BillingRelations.indexOf(rejectedRelation), 1);
            }
            return restApi.customPUT(payoutPolicy, "Put", { id: payoutPolicy.Id }).then(function (data) {
                dldata.payoutPolicy = data;
                if (detelatedData != '') {
                    dldata.subPolicyList.push(detelatedData.BillingSubpolicy);
                    detelatedData = '';
                }
                $csnotify.success("Policy Saved");
            }, function (data) {
                $csnotify.error(data);
            });
        } else {
            return restApi.customPOST(payoutPolicy, "POST").then(function (data) {
                dldata.payoutPolicy = data;
                $csnotify.success("Policy Saved");
            }, function (data) {
                $csnotify.error(data);
            });
        }
    };

    return {
        dldata: dldata,
        changeProductCategory: changeProductCategory,
        RejectSubPolicy: rejectSubPolicy,
        savePayoutPolicy: savePayoutPolicy,
        approveRelation: approveRelation,
        reset: reset,
        resetList: resetList,
        GetStakeHier: getStakeHier
    };
}]);

csapp.controller('payoutPolicyCtrl', [
    '$scope', 'payoutPolicyDataLayer', 'payoutPolicyFactory', '$modal', '$csModels', '$csfactory', '$csShared',
    function ($scope, datalayer, factory, $modal, $csModels, $csfactory, $csShared) {

        var findIndex = function (list, value) {
            var index = -1;
            for (var i = 0; i < list.length; i++) {
                if (list[i].Id == value) {
                    index = i;
                }
            }
            return index;
        };

        $scope.getApplyToData = function (applyTo) {
            switch (applyTo) {
                case "Stakeholder":
                    $scope.applyOnArray = _.uniq(_.pluck($scope.stakeHierarchy, 'Designation'));
                    break;
                case "Hierarchy":
                    $scope.applyOnArray = _.uniq(_.pluck($scope.stakeHierarchy, 'Hierarchy'));
                    break;
                case "Product":
                    $scope.applyOnArray = [$scope.dldata.payoutPolicy.Products];
                    break;
            }
            if (applyTo === 'Product') {
                $scope.getSubpolicy();
            }
        };

        $scope.setButtonStatus = function (policy) {
            // $scope.buttonStatus = status;
            if (policy.type === "") {
                $scope.dldata.buttonStatus = 'Draft';
            }
            if (policy.type === 'Approved') {
                $scope.dldata.buttonStatus = policy.type;
            }
            var today = moment();
            var endDate = moment(policy.BillingRelations.EndDate);
            var diff = endDate.diff(today, 'days');
            if ((policy.type === 'Submitted') && (diff < 0)) {
                $scope.dldata.buttonStatus = 'Expired';
            }
            if ((policy.type === 'Submitted') && (diff >= 0)) {
                $scope.dldata.buttonStatus = 'UnApproved';
            }
        };

      $scope.setDisplaySubpolicy = function (subpolicy, relation) {
            if (angular.isUndefined(relation) || $csfactory.isEmptyObject(relation)) {
                $scope.billingRelation = subpolicy;
            } else {
                $scope.billingRelation = relation;
            }

            $scope.disSubPolicy = factory.getDisplaySubPolicy(subpolicy);
            var isinlist = _.find($scope.dldata.ApproveUnapproved, function (item) {
                if (item.BillingRelations.Id === relation.Id) {
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
                var maxindex = ($scope.dldata.ApproveUnapproved.length) - 1;
                if (maxindex === index) {
                    $scope.direction.down = true;
                }
            }
        };

        (function () {
            $scope.factory = factory;
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.BillingPolicy = $csModels.getColumns("BillingPolicy");
            $scope.BillingPolicy.PolicyType = { label: "Policy Type", type: "enum", valueList: ["Payout", "Capping", "PF"] };
            $scope.BillingPolicy.ApplyTo = { label: "Apply To", type: "enum", valueList: ["Stakeholder", "Hierarchy", "Product"] };
            $scope.BillingPolicy.ApplyOn = { label: "Apply On", type: "enum" };
            $scope.BillingPolicy.ApplyOnText = { label: "Apply On", type: "text" };
            $scope.BillingPolicy.startdate = { label: "StartDate:", type: 'date' };
            $scope.BillingPolicy.enddate = { label: "EndDate:", type: 'date' };
            datalayer.reset();
            $scope.modalData = {
                BillingRelation: {},
                StartDate: null,
                endDate: null,
                subPolicyIndex: -1,
                forActivate: true
            };
            $scope.BillingPolicy.Products.valueList = _.reject($csShared.enums.Products, function (item) {
                return (item === "UNKNOWN" || item === "ALL");
            });
            $scope.direction = {
                up: true,
                down: true
            };
            datalayer.resetList();
            datalayer.GetStakeHier().then(function (data) {
                $scope.stakeHierarchy = data;
            });
        })();
        $scope.onProductChange = function() {
            datalayer.resetList();
           // $scope.buttonStatus = "";
            $scope.dldata.payoutPolicy.PolicyType = "";
            $scope.dldata.payoutPolicy.PolicyFor = "";
            $scope.dldata.payoutPolicy.PolicyForId = "";
        };
        $scope.onPolicyTypeChange = function() {
            datalayer.resetList();
            //$scope.buttonStatus = "";
            $scope.dldata.payoutPolicy.PolicyFor = "";
            $scope.dldata.payoutPolicy.PolicyForId = "";
        };
        $scope.moveUp = function(policy) {
            var test = [];
            _.forEach($scope.dldata.ApproveUnapproved, function (item) {
                _.forEach($scope.dldata.payoutPolicy.BillingRelations, function (rel) {
                    if (angular.isDefined(rel)) {
                        if (item.allocRelation.Id === rel.Id) {
                            $scope.dldata.payoutPolicy.BillingRelations.splice($scope.dldata.payoutPolicy.BillingRelations.indexOf(rel), 1);
                            test.push(rel);
                        }
                    }
                });
            });

            var relations = _.sortBy(test, 'Priority');
            console.log(relations);
            var index = relations.indexOf(policy.BillingRelations);
            var tempPriority = relations[index].Priority;
            relations[index].Priority = relations[index - 1].Priority;
            relations[index - 1].Priority = tempPriority;
            _.forEach(relations, function (item) {
                $scope.dldata.payoutPolicy.BillingRelations.push(item);
            });
            datalayer.savePayoutPolicy($scope.dldata.payoutPolicy).then(function () {
                $scope.getSubpolicy();
                $scope.direction = {
                    up: true,
                    down: true
                };
            });
        };
        $scope.moveDown = function(policy) {
            var test = [];
            _.forEach($scope.dldata.ApproveUnapproved, function (item) {
                _.forEach($scope.dldata.payoutPolicy.BillingRelations, function (rel) {
                    if (angular.isDefined(rel)) {
                        if (item.BillingRelations.Id === rel.Id) {
                            $scope.dldata.payoutPolicy.BillingRelations.splice($scope.dldata.payoutPolicy.BillingRelations.indexOf(rel), 1);
                            test.push(rel);
                        }
                    }
                });
            });

            var relations = _.sortBy(test, 'Priority');
            console.log(relations);
            var index = relations.indexOf(policy.BillingRelations);
            var tempPriority = relations[index].Priority;
            relations[index].Priority = relations[index + 1].Priority;
            relations[index + 1].Priority = tempPriority;
            _.forEach(relations, function (item) {
                $scope.dldata.payoutPolicy.BillingRelations.push(item);
            });
            datalayer.savePayoutPolicy($scope.dldata.BillingRelations).then(function () {
                $scope.getSubpolicy();
                $scope.direction = {
                    up: true,
                    down: true
                };
            });
        };

        var openmodal = function (modaldata) {
            $modal.open({
                templateUrl: baseUrl + 'Billing/policy/date-modal.html',
                controller: 'policymodal',
                size: 'lg',
                resolve: {
                    modaldata: function () {
                        return modaldata;
                    }
                }
            });
        };

        $scope.getSubpolicy = function () {
            console.log("function called");
            datalayer.resetList();
            datalayer.changeProductCategory();
        };

        $scope.openModelNewSubPolicy = function (subPolicy) {
            $scope.dldata.buttonStatus = null;
            $scope.modalData.BillingRelations = { BillingSubpolicy: subPolicy };
            var indexl = findIndex($scope.dldata.subPolicyList, subPolicy.Id);
            $scope.modalData.subPolicyIndex = indexl;
            $scope.modalData.startDate = null;
            $scope.modalData.endDate = null;
            $scope.modalData.forActivate = true;
            openmodal($scope.modalData);
        };

        $scope.openModelDeactivateSubPolicy = function (relation) {
            $scope.dldata.buttonStatus = null;
            $scope.modalData.payoutRelation = { BillingSubpolicy: relation.BillingSubpolicy, OrigEntityId: relation.Id };
            $scope.modalData.payoutRelation.Status = "Submitted";
            $scope.modalData.subPolicyIndex = -1;
            $scope.modalData.startDate = relation.StartDate;
            $scope.modalData.endDate = null;
            $scope.modalData.forActivate = false;
            openmodal($scope.modalData);
        };

        $scope.openModelReactivateSubPolicy = function (relation) {
            $scope.dldata.buttonStatus = null;
            $scope.modalData.payoutRelation = { BillingSubpolicy: relation.BillingSubpolicy, OrigEntityId: relation.Id };
            $scope.modalData.payoutRelation.Status = "Submitted";
            $scope.modalData.subPolicyIndex = -1;
            $scope.modalData.startDate = null;
            $scope.modalData.endDate = null;
            $scope.modalData.forActivate = true;
            openmodal($scope.modalData);
        };

        $scope.openModel = function (billingRelations, forActivate) {
            $scope.modalData.BillingRelations = billingRelations;
            $scope.modalData.startDate = billingRelations.StartDate;
            $scope.modalData.endDate = billingRelations.EndDate;
            $scope.modalData.forActivate = forActivate;
            openmodal($scope.modalData);
        };

        $scope.approve = function (relation) {
            datalayer.approveRelation(relation).then(function () {
                relation.Status = 'Approved';
                $scope.dldata.buttonStatus = null;
            });
        };

        $scope.reject = function (relation) {
            datalayer.RejectSubPolicy(relation).then(function () {
                $scope.dldata.buttonStatus = null;
                relation.Status = 'Rejected';
            });
        };

    }]);
