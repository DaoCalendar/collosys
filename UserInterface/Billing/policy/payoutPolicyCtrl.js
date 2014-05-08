
csapp.controller('policymodal', ['$scope', 'modaldata', '$modalInstance', 'payoutPolicyFactory', 'payoutPolicyDataLayer', '$csBillingModels',
    function ($scope, modaldata, $modalInstance, factory, datalayer, $csBillingModels) {
        $scope.modelData = modaldata;
        $scope.dldata = datalayer.dldata;


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
            datalayer.savePayoutPolicy($scope.dldata.payoutPolicy).then
            (function () {
                $modalInstance.close();
            });

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
            displaySubPolicy.Condition = getOuputConditionString(subPolicy, 'Condition');
            displaySubPolicy.Output = getOuputConditionString(subPolicy, 'Output');
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

    dldata.categorySwitch = [{ Name: 'Collection', Value: 'Liner' }, { Name: 'Recovery', Value: 'WriteOff' }];

    var reset = function () {
        dldata.payoutPolicy = {};
        dldata.subPolicyList = [];
    };

    var changeProductCategory = function () {
        if (angular.isUndefined(dldata.payoutPolicy))
            return;
        var payoutPolicy = dldata.payoutPolicy;
        payoutPolicy.Category = 'Liner';
        if (!angular.isUndefined(payoutPolicy.Products) && !angular.isUndefined(payoutPolicy.Category)) {
            restApi.customGET("GetPayoutPolicy", { products: payoutPolicy.Products, category: payoutPolicy.Category }).then(function (data) {
                dldata.payoutPolicy = data.PayoutPolicy;
                dldata.subPolicyList = data.UnUsedSubpolicies;
            }, function (data) {
                $csnotify.error(data);
            });
        }
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
        var orgId = relation.OrigEntityId;
        var rejectedRelation = _.find(dldata.payoutPolicy.BillingRelations, { Id: orgId });
        return restApi.customGET('ApproveRelation', { relationId: relation.Id }).then(function () {
            //dldata.payoutPolicy.BillingRelations.splice(dldata.payoutPolicy.BillingRelations.indexOf(rejectedRelation), 1);
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
        reset: reset
    };
}]);

csapp.controller('payoutPolicyCtrl', [
    '$scope', 'payoutPolicyDataLayer', 'payoutPolicyFactory', '$modal', '$csBillingModels',
    function ($scope, datalayer, factory, $modal, $csBillingModels) {

        var findIndex = function (list, value) {
            var index = -1;
            for (var i = 0; i < list.length; i++) {
                if (list[i].Id == value) {
                    index = i;
                }
            }
            return index;
        };

        $scope.setButtonStatus = function (status) {
            $scope.buttonStatus = status;
        };

        $scope.setDisplaySubpolicy = function (subpolicy, relation) {
            if (angular.isUndefined(relation)) {
                $scope.billingRelation = subpolicy;
            } else {
                $scope.billingRelation = relation;
            }

            $scope.disSubPolicy = factory.getDisplaySubPolicy(subpolicy);
        };

        (function () {
            $scope.factory = factory;
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.BillingPolicy = $csBillingModels.models.BillingPolicy;
            datalayer.reset();
            $scope.modalData = {
                BillingRelation: {},
                StartDate: null,
                endDate: null,
                subPolicyIndex: -1,
                forActivate: true
            };
        })();

        var openmodal = function (modaldata) {
            $modal.open({
                templateUrl: baseUrl + 'Billing/policy/date-modal.html',
                controller: 'policymodal',
                resolve: {
                    modaldata: function () {
                        return modaldata;
                    }
                }
            });
        };

        $scope.openModelNewSubPolicy = function (subPolicy, index) {
            $scope.buttonStatus = null;
            $scope.modalData.BillingRelations = { BillingSubpolicy: subPolicy };
            var indexl = findIndex($scope.dldata.subPolicyList, subPolicy.Id);
            $scope.modalData.subPolicyIndex = indexl;
            $scope.modalData.startDate = null;
            $scope.modalData.endDate = null;
            $scope.modalData.forActivate = true;
            openmodal($scope.modalData);
        };

        $scope.openModelDeactivateSubPolicy = function (relation) {
            $scope.buttonStatus = null;
            $scope.modalData.payoutRelation = { BillingSubpolicy: relation.BillingSubpolicy, OrigEntityId: relation.Id };
            $scope.modalData.payoutRelation.Status = "Submitted";
            $scope.modalData.subPolicyIndex = -1;
            $scope.modalData.startDate = relation.StartDate;
            $scope.modalData.endDate = null;
            $scope.modalData.forActivate = false;
            openmodal($scope.modalData);
        };

        $scope.openModelReactivateSubPolicy = function (relation) {
            $scope.buttonStatus = null;
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
                $scope.buttonStatus = null;
            });
        };

        $scope.reject = function (relation) {
            datalayer.RejectSubPolicy(relation).then(function () {
                $scope.buttonStatus = null;
                relation.Status = 'Rejected';
            });
        };

    }]);
