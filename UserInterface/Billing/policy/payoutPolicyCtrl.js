csapp.controller("payoutPolicyCtrl1", ["$scope", "$csnotify", "Restangular", function ($scope, $csnotify, rest) {
    "use strict";

    $scope.productsList = [];
    $scope.payoutPolicy = {};
    $scope.payoutPolicy.Category = "Liner";
    $scope.modalData = {
        BillingRelation: {},
        StartDate: null,
        endDate: null,
        subPolicyIndex: -1,
        forActivate: true
    };
    $scope.openDateModel = false;
    $scope.isModalDateValid = false;

    $scope.openModelNewSubPolicy = function (subPolicy, index) {
        $scope.modalData.BillingRelations = { BillingSubpolicy: subPolicy };
        $scope.modalData.subPolicyIndex = index;
        $scope.modalData.startDate = null;
        $scope.modalData.endDate = null;
        $scope.modalData.forActivate = true;
        $scope.openDateModel = true;
    };

    $scope.openModel = function (billingRelations, forActivate) {
        $scope.modalData.BillingRelations = billingRelations;
        $scope.modalData.startDate = billingRelations.StartDate;
        $scope.modalData.endDate = billingRelations.EndDate;
        $scope.modalData.forActivate = forActivate;
        $scope.openDateModel = true;
    };

    $scope.closeModel = function () {
        $scope.modalData = {
            BillingRelations: {},
            StartDate: null,
            endDate: null,
            subPolicyIndex: -1,
            forActivate: true
        };
        $scope.openDateModel = false;
    };

    $scope.modelDateValidation = function (startDate, endDate) {
        if (angular.isUndefined(endDate) || endDate == null) {
            $scope.isModalDateValid = true;
            return;
        }

        startDate = moment(startDate);
        endDate = moment(endDate);
        $scope.isModalDateValid = (endDate >= startDate);
    };

    $scope.activateSubPoicy = function (modalData) {
        var maxPriorityPolicy = _.max($scope.payoutPolicy.BillingRelations, 'Priority');
        modalData.BillingRelations.Priority = maxPriorityPolicy.Priority + 1;
        modalData.BillingRelations.StartDate = modalData.startDate;
        modalData.BillingRelations.EndDate = modalData.endDate;
        modalData.BillingRelations.Status = "Submitted";

        if (modalData.subPolicyIndex > -1) {
            $scope.payoutPolicy.BillingRelations.push(JSON.parse(JSON.stringify(modalData.BillingRelations)));
            $scope.subPolicyList.splice(modalData.subPolicyIndex, 1);
        }
        $scope.savePayoutPolicy($scope.payoutPolicy);
        $scope.closeModel();
    };

    $scope.deactivateSubPoicy = function (modalData) {
        modalData.payoutRelation.StartDate = modalData.startDate;
        modalData.payoutRelation.EndDate = modalData.endDate;
        $scope.payoutPolicy.BillingRelations.push(JSON.parse(JSON.stringify(modalData.payoutRelation)));
        $scope.savePayoutPolicy($scope.payoutPolicy);
        $scope.closeModel();
    };

    $scope.openModelDeactivateSubPolicy = function (relation) {
        $scope.modalData.payoutRelation = { BillingSubpolicy: relation.BillingSubpolicy, OrigEntityId: relation.Id };
        $scope.modalData.payoutRelation.Status = "Submitted";
        $scope.modalData.subPolicyIndex = -1;
        $scope.modalData.startDate = relation.StartDate;
        $scope.modalData.endDate = null;
        $scope.modalData.forActivate = false;
        $scope.openDateModel = true;
    };

    $scope.SaveSubPolicy = function () {
        $scope.savePayoutPolicy($scope.payoutPolicy);
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
            datalayer.savePayoutPolicy($scope.payoutPolicy);
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
            reset: reset
        };
    }
]);

csapp.factory('payoutPolicyDataLayer', ['Restangular', '$csnotify', '$csfactory', function(rest, $csnotify, $csfactory) {
    var restApi = rest.all("PayoutPolicyApi");
    var dldata = {};

    dldata.categorySwitch = [{ Name: 'Collection', Value: 'Liner' }, { Name: 'Recovery', Value: 'WriteOff' }];

    var getProducts = function () {
        restApi.customGET("GetProducts").then(function (data) {
            dldata.productsList = data;
        }, function (data) {
            $csnotify.error(data);
        });
    };

    var changeProductCategory = function () {
        var payoutPolicy = dldata.payoutPolicy;
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
        restApi.customDELETE("RejectSubpolicy", { id: rejectedRelation.Id }).then(function () {
            dldata.payoutPolicy.BillingRelations.splice(dldata.payoutPolicy.BillingRelations.indexOf(rejectedRelation), 1);
            dldata.subPolicyList.push(rejectedRelation.BillingSubpolicy);
            $csnotify.success("Subpolicy Rejected");
        }, function (data) {
            $csnotify.error(data);
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
            restApi.customPUT(payoutPolicy, "Put", { id: payoutPolicy.Id }).then(function (data) {
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
            restApi.customPOST(payoutPolicy, "POST").then(function (data) {
                dldata.payoutPolicy = data;
                $csnotify.success("Policy Saved");
            }, function (data) {
                $csnotify.error(data);
            });
        }
    };

    return {
        dldata: dldata,
        getProducts: getProducts,
        changeProductCategory: changeProductCategory,
        RejectSubPolicy: rejectSubPolicy,
        savePayoutPolicy: savePayoutPolicy
    };
}]);

csapp.controller('payoutPolicyCtrl', [
    '$scope', 'payoutPolicyDataLayer','payoutPolicyFactory',
    function ($scope, datalayer, factory) {

        (function () {
            $scope.factory = factory;
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            datalayer.getProducts();
        })();

    }]);
//, 'payoutPolicyDataLayer', 'payoutPolicyFactory', '$modal',
//, datalayer, factory, $modal