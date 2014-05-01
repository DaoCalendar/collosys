csapp.controller("allocPolicyCtrl1", ["$scope", "$csnotify", "Restangular", function ($scope, $csnotify, rest) {
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

    $scope.SaveSubPolicy = function () {
        $scope.saveAllocPolicy($scope.allocPolicy);
    };
}]);

csapp.controller('datemodelctrl', ['$scope', 'modelData', '$modalInstance', 'allocPolicyDataLayer', "allocPolicyFactory",
    function ($scope, modeldata, $modalInstance, datalayer, factory) {
        $scope.modalData = modeldata;

        $scope.modelDateValidation = function (startDate, endDate) {
            if (angular.isUndefined(endDate) || endDate == null) {
                $scope.isModalDateValid = true;
                return;
            }
            startDate = moment(startDate);
            endDate = moment(endDate);
            $scope.isModalDateValid = (endDate > startDate);

        };


        $scope.activateSubPoicy = function (modalData) {
            var maxPriorityPolicy = _.max(datalayer.dldata.allocPolicy.AllocRelations, 'Priority');
            modalData.AllocRelations.Priority = maxPriorityPolicy.Priority + 1;
            modalData.AllocRelations.StartDate = modalData.startDate;
            modalData.AllocRelations.EndDate = modalData.endDate;
            modalData.AllocRelations.Status = "Submitted";
            datalayer.dldata.allocPolicy.AllocRelations.push(JSON.parse(JSON.stringify(modalData.AllocRelations)));
            if (modalData.subPolicyIndex > -1) {
                datalayer.dldata.subPolicyList.splice(modalData.subPolicyIndex, 1);
            }
            datalayer.saveAllocPolicy(datalayer.dldata.allocPolicy).then(function () {
                $scope.closeModel();

            });
        };

        $scope.deactivateSubPoicy = function (modalData) {
            modalData.AllocRelations.StartDate = modalData.startDate;
            modalData.AllocRelations.EndDate = modalData.endDate;
            datalayer.dldata.allocPolicy.AllocRelations.push(JSON.parse(JSON.stringify(modalData.AllocRelations)));
            datalayer.saveAllocPolicy(datalayer.dldata.allocPolicy).then(function () {
                $scope.closeModel();
            });
        };


        $scope.closeModel = function () {
            $modalInstance.close();
            $scope.modalData = {
                AllocRelations: {},
                startDate: null,
                endDate: null,
                subPolicyIndex: -1,
                forActivate: true
            };
        };

    }]);

csapp.controller('allocPolicyCtrl', ['$scope', 'allocPolicyDataLayer', 'allocPolicyFactory', '$modal',
    function ($scope, datalayer, factory, $modal) {
        "use strict";

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

        var openModal = function (modalData) {
            $modal.open({
                templateUrl: baseUrl + 'Allocation/policy/policy-modal.html',
                controller: 'datemodelctrl',
                resolve: {
                    modelData: function () {
                        return modalData;
                    }
                }
            });
        };

        $scope.openModelReactivateSubPolicy = function (relation) {
            $scope.buttonStatus = null;
            $scope.modalData.AllocRelations = { AllocSubpolicy: relation.AllocSubpolicy };
            $scope.modalData.subPolicyIndex = -1;
            $scope.modalData.startDate = null;
            $scope.modalData.endDate = null;
            $scope.modalData.forActivate = true;
            openModal($scope.modalData);
        };

        $scope.openModelDeactivateSubPolicy = function (relation) {
            $scope.buttonStatus = null;
            $scope.modalData.AllocRelations = { AllocSubpolicy: relation.AllocSubpolicy, OrigEntityId: relation.Id };
            $scope.modalData.AllocRelations.Status = "Submitted";
            $scope.modalData.subPolicyIndex = -1;
            $scope.modalData.startDate = relation.StartDate;
            $scope.modalData.endDate = null;
            $scope.modalData.forActivate = false;
            openModal($scope.modalData);
        };

        $scope.openModelNewSubPolicy = function (subPolicy, index) {
            $scope.buttonStatus = null;
            $scope.modalData.AllocRelations = { AllocSubpolicy: subPolicy };
            var indexl = findIndex($scope.dldata.subPolicyList, subPolicy.Id);
            $scope.modalData.subPolicyIndex = indexl;
            $scope.modalData.startDate = null;
            $scope.modalData.endDate = null;
            $scope.modalData.forActivate = true;
            openModal($scope.modalData);
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
            openModal($scope.modalData);
        };

        $scope.setDisplaySubpolicy = function (subpolicy, relation) {
            if (angular.isUndefined(relation)) {
                $scope.allocRelation = subpolicy;
            } else {
                $scope.allocRelation = relation;
            }

            $scope.disSubPolicy = factory.getDisplaySubPolicy(subpolicy);
        };

        $scope.approve = function (allocRelation) {
            datalayer.approveRelation(allocRelation).then(function() {
                $scope.buttonStatus = null;
                allocRelation.Status = 'Approved';
            });
        };

        $scope.reject = function (allocRelation) {
            
            datalayer.RejectSubPolicy(allocRelation).then(function() {
                $scope.buttonStatus = null;
                allocRelation.Status = 'Rejected';
            });
        };

        (function () {
            $scope.factory = factory;
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.datalayer.reset();
            $scope.modalData = {
                AllocRelation: {},
                StartDate: null,
                endDate: null,
                subPolicyIndex: -1,
                forActivate: true
            };
            $scope.datalayer.getProducts();
        })();
    }]);

csapp.factory('allocPolicyDataLayer', ['Restangular', '$csnotify', '$csfactory',
    function (rest, $csnotify, $csfactory) {

        var dldata = {};

        var api = rest.all('AllocationPolicyApi');

        var getProducts = function () {
            dldata.productsList = [];
            api.customGET("GetProducts").then(function (data) {
                dldata.productsList = data;
            }, function (data) {
                $csnotify.error(data);
            });
        };

        var changeProductCategory = function () {
            dldata.allocPolicy.Category = 'Liner';

            var allocPolicy = dldata.allocPolicy;

            if (!angular.isUndefined(allocPolicy.Products) && !angular.isUndefined(allocPolicy.Category)) {

                api.customGET("GetAllocPolicy", { products: allocPolicy.Products, category: allocPolicy.Category }).then(function (data) {
                    dldata.allocPolicy = data.AllocPolicy;
                    dldata.subPolicyList = data.UnUsedSubpolicies;
                }, function (data) {
                    $csnotify.error(data);
                });
            }
        };

        var rejectSubPolicy = function (rejectedRelation) {
            return api.customDELETE("RejectSubpolicy", { id: rejectedRelation.Id }).then(function () {
                dldata.allocPolicy.AllocRelations.splice(dldata.allocPolicy.AllocRelations.indexOf(rejectedRelation), 1);
                dldata.subPolicyList.push(rejectedRelation.AllocSubpolicy);
                $csnotify.success("Subpolicy Rejected");
                return;
            }, function (data) {
                $csnotify.error(data);
            });
        };
        var approveRelation = function (relation) {
            var origId = relation.OrigEntityId;
            var origRelation = _.find(dldata.allocPolicy.AllocRelations, { Id: origId });
            return api.customGET('ApproveRelation', { relationId: relation.Id }).then(function (data) {
               // dldata.allocPolicy.AllocRelations.splice(dldata.allocPolicy.AllocRelations.indexOf(origRelation), 1);
                $csnotify.success('Subpolicy Approved');
                return;
            });
        };
        var saveAllocPolicy = function (allocPolicy) {
            var deletedData = '';
            if (!$csfactory.isNullOrEmptyGuid(allocPolicy.Id)) {

                var rejectedRelation = _.find(allocPolicy.AllocRelations, { Status: 'Rejected' });
                if (angular.isDefined(rejectedRelation)) {
                    deletedData = angular.copy(rejectedRelation);
                    allocPolicy.AllocRelations.splice(allocPolicy.AllocRelations.indexOf(rejectedRelation), 1);
                }
                return api.customPUT(allocPolicy, "Put", { id: allocPolicy.Id }).then(function (data) {
                    dldata.allocPolicy = data;
                    if (deletedData != '') {
                        dldata.subPolicyList.push(deletedData.AllocSubpolicy);
                        deletedData = '';
                    }
                    $csnotify.success("Policy Saved");
                }, function (data) {
                    $csnotify.error(data);
                });
            } else {
                return api.customPOST(allocPolicy, "POST").then(function (data) {
                    dldata.allocPolicy = data;
                    $csnotify.success("Policy Saved");
                }, function (data) {
                    $csnotify.error(data);
                });
            }
        };

        var reset = function () {
            dldata.allocPolicy = {};
            dldata.allocPolicy.Category = "Liner";
            dldata.subPolicyList = {};
        };

        return {
            dldata: dldata,
            getProducts: getProducts,
            changeProductCategory: changeProductCategory,
            RejectSubPolicy: rejectSubPolicy,
            saveAllocPolicy: saveAllocPolicy,
            reset: reset,
            approveRelation: approveRelation
        };
    }]);



csapp.factory('allocPolicyFactory', ['allocPolicyDataLayer', function (datalayer) {

    var setStatus = function (status) {
        if (status === 'Rejected') {
            datalayer.dldata.color = { color: 'red' };
        }
        if (status === 'Approved') {
            datalayer.dldata.color = { color: 'green' };
        }
        if (status === 'Submitted') {
            datalayer.dldata.color = { color: 'blue' };
        }
        return status;
    };

    var getDisplaySubPolicy = function (subPolicy) {
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

    var expiredPolicyStatus = function (policy) {
        var status = setStatus(policy);
        return status;
    };

    var relationValidToday = function (relation, todayActive) {
        var today = moment();
        var startDate = moment(relation.StartDate);
        var endDate = relation.EndDate ? moment(relation.EndDate) : moment();
        return ((today <= endDate) === todayActive);
    };

    var upside = function (subPolicy, index) {
        var test = _.filter(datalayer.dldata.allocPolicy.AllocRelations, function (subpolicy) {
            return (subpolicy.Status == 'Approved');
        });
        var relations = _.sortBy(test, 'Priority');
        var tempPriority = relations[index].Priority;
        relations[index].Priority = relations[index - 1].Priority;
        relations[index - 1].Priority = tempPriority;
        datalayer.saveAllocPolicy(datalayer.dldata.allocPolicy);
    };

    var downside = function (subPolicy, index) {
        var test = _.filter(datalayer.dldata.allocPolicy.AllocRelations, function (subpolicy) {
            return (subpolicy.Status == 'Approved');
        });
        var relations = _.sortBy(test, 'Priority');

        var tempPriority = relations[index].Priority;
        relations[index].Priority = relations[index + 1].Priority;
        relations[index + 1].Priority = tempPriority;
        datalayer.saveAllocPolicy(datalayer.dldata.allocPolicy);
    };



    return {
        setStatus: setStatus,
        getDisplaySubPolicy: getDisplaySubPolicy,
        filterRelation: filterRelation,
        expiredPolicyStatus: expiredPolicyStatus,
        relationValidToday: relationValidToday,
        upside: upside,
        downside: downside
    };
}]);