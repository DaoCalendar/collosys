
(
csapp.controller("allocPolicyCtrl", ["$scope", "$csnotify", "Restangular", function ($scope, $csnotify, rest) {
    "use strict";

    var restApi = rest.all("AllocationPolicyApi");
    $scope.productsList = [];
    $scope.subPolicyList = {};
    $scope.allocPolicy = {};
    $scope.allocPolicy.Category = "Liner";
    $scope.modalData = {
        AllocRelation: {},
        StartDate: null,
        endDate: null,
        subPolicyIndex: -1,
        forActivate: true
    };
    $scope.openDateModel = false;
    $scope.isModalDateValid = false;

    $scope.categorySwitch = [{ Name: 'Collection', Value: 'Liner' }, { Name: 'Recovery', Value: 'WriteOff' }];
    $scope.ApproveSwitch = [{ Name: 'Approve', Value: 'Approved' }, { Name: 'Reject', Value: 'Rejected' }];
    restApi.customGET("GetProducts").then(function (data) {
        $scope.productsList = data;
    }, function (data) {
        $csnotify.error(data);
    });

    $scope.changeProductCategory = function () {


        //check if product =='CC' then set category='Liner'
        //if ($scope.allocPolicy.Products == 'CC') {
        $scope.allocPolicy.Category = 'Liner';
        //}

        var allocPolicy = $scope.allocPolicy;
        debugger;
        if (!angular.isUndefined(allocPolicy.Products) && !angular.isUndefined(allocPolicy.Category)) {

            restApi.customGET("GetAllocPolicy", { products: allocPolicy.Products, category: allocPolicy.Category }).then(function (data) {
                $scope.allocPolicy = data.AllocPolicy;
                //var savedSubpolicyNames = _.pluck(_.pluck($scope.allocPolicy.AllocRelations, 'AllocSubpolicy'), 'Name');
                $scope.subPolicyList = data.UnUsedSubpolicies;
                // _.filter(data.AllocSubpolicies, function (sdata) { return !_.contains(savedSubpolicyNames, sdata.Name); });

            }, function (data) {
                $csnotify.error(data);
            });
        }
    };

    $scope.getDisplaySubPolicy = function (subPolicy) {
        var displaySubPolicy = {};
        displaySubPolicy.Name = subPolicy.Name;
        displaySubPolicy.Condition = getOuputConditionString(subPolicy);
        displaySubPolicy.DoAllocate = subPolicy.DoAllocate;
        displaySubPolicy.Policy = (subPolicy.DoAllocate ? "Allocate As Per Policy" : subPolicy.AllocateType);
        displaySubPolicy.Reason = subPolicy.ReasonNotAllocate;
        if (displaySubPolicy.Policy == 'AllocateToStkholder') {
            displaySubPolicy.StakeholdersName = subPolicy.Stakeholder.Name;
        }

        return displaySubPolicy;
    };

    var getOuputConditionString = function (subPolicy) {
        var conditionList = _.sortBy(subPolicy.Conditions, 'Priority');

        var conditionString = "";
        for (var i = 0; i < conditionList.length; i++) {
            var condition = conditionList[i];
            conditionString = conditionString + condition.RelationType + ' ( ' + condition.ColumnName + ' ' + condition.Operator + ' ' + condition.Value + ' ) ';
        }

        return conditionString;
    };

    $scope.openModelReactivateSubPolicy = function (relation) {
        $scope.modalData.AllocRelations = { AllocSubpolicy: relation.AllocSubpolicy };
        $scope.modalData.subPolicyIndex = -1;
        $scope.modalData.startDate = null;
        $scope.modalData.endDate = null;
        $scope.modalData.forActivate = true;
        $scope.openDateModel = true;
    };

    $scope.openModelDeactivateSubPolicy = function(relation) {
        $scope.modalData.AllocRelations = { AllocSubpolicy: relation.AllocSubpolicy, OrigEntityId: relation.Id };
        $scope.modalData.AllocRelations.Status = "Submitted";
        $scope.modalData.subPolicyIndex = -1;
        $scope.modalData.startDate = relation.StartDate;
        $scope.modalData.endDate = null;
        $scope.modalData.forActivate = false;
        $scope.openDateModel = true;
    };

    $scope.openModelNewSubPolicy = function (subPolicy, index) {

        $scope.modalData.AllocRelations = { AllocSubpolicy: subPolicy };
        $scope.modalData.subPolicyIndex = index;
        $scope.modalData.startDate = null;
        $scope.modalData.endDate = null;
        $scope.modalData.forActivate = true;
        $scope.openDateModel = true;
    };

    $scope.openModel = function (allocRelations, forActivate) {
        $scope.modalData.AllocRelations = allocRelations;
        if (forActivate == false) {
            $scope.modalData.startDate = allocRelations.StartDate;
        } else {
            $scope.modalData.startDate = null;
        }
        //$scope.modalData.endDate = allocRelations.EndDate;
        $scope.modalData.forActivate = forActivate;
        $scope.openDateModel = true;
    };

    $scope.modelDateValidation = function (startDate, endDate) {
        if (angular.isUndefined(endDate) || endDate == null) {
            $scope.isModalDateValid = true;
            return;
        }
        startDate = moment(startDate);
        endDate = moment(endDate);
        $scope.isModalDateValid = (endDate > startDate);
    };

    $scope.closeModel = function () {
        $scope.modalData = {
            AllocRelations: {},
            startDate: null,
            endDate: null,
            subPolicyIndex: -1,
            forActivate: true
        };
        $scope.openDateModel = false;
    };

    $scope.activateSubPoicy = function (modalData) {

        var maxPriorityPolicy = _.max($scope.allocPolicy.AllocRelations, 'Priority');
        modalData.AllocRelations.Priority = maxPriorityPolicy.Priority + 1;
        modalData.AllocRelations.StartDate = modalData.startDate;
        modalData.AllocRelations.EndDate = modalData.endDate;
        modalData.AllocRelations.Status = "Submitted";
        $scope.allocPolicy.AllocRelations.push(JSON.parse(JSON.stringify(modalData.AllocRelations)));
        if (modalData.subPolicyIndex > -1) {
            $scope.subPolicyList.splice(modalData.subPolicyIndex, 1);
        }
        $scope.saveAllocPolicy($scope.allocPolicy);
        $scope.closeModel();
    };

    $scope.deactivateSubPoicy = function (modalData) {
        debugger;
        modalData.AllocRelations.StartDate = modalData.startDate;
        modalData.AllocRelations.EndDate = modalData.endDate;
        $scope.allocPolicy.AllocRelations.push(JSON.parse(JSON.stringify(modalData.AllocRelations)));
        $scope.saveAllocPolicy($scope.allocPolicy);
        $scope.closeModel();
    };

    $scope.filterRelation = function (todayActive, status) {
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

    $scope.setStatus = function (status) {
        if (status === 'Rejected') {
            $scope.color = { color: 'red' };
        }
        if (status === 'Approved') {
            $scope.color = { color: 'green' };
        }
        if (status === 'Submitted') {
            $scope.color = { color: 'blue' };
        }
        return status;
    };
    $scope.expiredPolicyStatus = function (policy) {
        var status = $scope.setStatus(policy);

        return status;
    };

    //$scope.futureActiveRelation = function (todayA) {
    //    return function (relation) {
    //        var today = moment();
    //        var startDate = moment(relation.StartDate);
    //        var endDate = relation.EndDate ? moment(relation.EndDate) : moment();
    //        if (todayA == true) {
    //            return (today < startDate);

    //        } else {
    //            return ((today > startDate) && (today > endDate));
    //        }
    //    };
    //};

    var relationValidToday = function (relation, todayActive) {
        var today = moment();
        var startDate = moment(relation.StartDate);
        var endDate = relation.EndDate ? moment(relation.EndDate) : moment();
        return ((today <= endDate) === todayActive);
    };

    $scope.SaveSubPolicy = function () {
        $scope.saveAllocPolicy($scope.allocPolicy);
    };




    $scope.upside = function (subPolicy, index) {
        var test = _.filter($scope.allocPolicy.AllocRelations, function (relation) { return relationValidToday(relation, true); });
        var relations = _.sortBy(test, 'Priority');
        var tempPriority = relations[index].Priority;
        relations[index].Priority = relations[index - 1].Priority;
        relations[index - 1].Priority = tempPriority;
        $scope.saveAllocPolicy($scope.allocPolicy);
    };

    $scope.downside = function (subPolicy, index) {

        var test = _.filter($scope.allocPolicy.AllocRelations, function (relation) { return relationValidToday(relation, true); });
        var relations = _.sortBy(test, 'Priority');

        var tempPriority = relations[index].Priority;
        relations[index].Priority = relations[index + 1].Priority;
        relations[index + 1].Priority = tempPriority;
        $scope.saveAllocPolicy($scope.allocPolicy);
    };

    $scope.RejectSubPolicy = function (rejectedRelation) {

        restApi.customDELETE("RejectSubpolicy", { id: rejectedRelation.Id }).then(function () {
            $scope.allocPolicy.AllocRelations.splice($scope.allocPolicy.AllocRelations.indexOf(rejectedRelation), 1);
            $scope.subPolicyList.push(rejectedRelation.AllocSubpolicy);
            $csnotify.success("Subpolicy Rejected");
        }, function (data) {
            $csnotify.error(data);
        });


    };

    $scope.saveAllocPolicy = function (allocPolicy) {

        var deletedData = '';
        if (allocPolicy.Id != "00000000-0000-0000-0000-000000000000") {

            var rejectedRelation = _.find(allocPolicy.AllocRelations, { Status: 'Rejected' });
            if (angular.isDefined(rejectedRelation)) {
                deletedData = angular.copy(rejectedRelation);
                allocPolicy.AllocRelations.splice(allocPolicy.AllocRelations.indexOf(rejectedRelation), 1);
            }
            //_.forEach(allocPolicy.AllocRelations, function (item) {
            //    if (item.Status === 'Rejected') {
            //        deletedData = angular.copy(item);
            //        allocPolicy.AllocRelations.splice(allocPolicy.AllocRelations.indexOf(item), 1);
            //        b;
            //    }
            //});

            restApi.customPUT(allocPolicy, "Put", { id: allocPolicy.Id }).then(function (data) {

                $scope.allocPolicy = data;
                if (deletedData != '') {
                    $scope.subPolicyList.push(deletedData.AllocSubpolicy);
                    deletedData = '';
                }
                $csnotify.success("Policy Saved");
            }, function (data) {
                $csnotify.error(data);
            });
        } else {
            restApi.customPOST(allocPolicy, "POST").then(function (data) {
                $scope.allocPolicy = data;
                $csnotify.success("Policy Saved");
            }, function (data) {
                $csnotify.error(data);
            });
        }
    };

    $scope.reset = function () {
        $scope.allocPolicy = {};
        $scope.allocPolicy.Category = "Liner";
        $scope.subPolicyList = {};
    };

}])
);