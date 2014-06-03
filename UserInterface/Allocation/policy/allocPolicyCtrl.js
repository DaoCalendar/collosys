
csapp.controller('datemodelctrl', ['$scope', 'modelData', '$modalInstance', 'allocPolicyDataLayer', "allocPolicyFactory", "$csModels", "$csnotify",
    function ($scope, modeldata, $modalInstance, datalayer, factory, $csModels, $csnotify) {
        $scope.modalData = modeldata;
        $scope.allocmodalData = $csModels.getColumns("AllocPolicy");

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
                datalayer.resetList();
                datalayer.changeProductCategory();
                $scope.closeModel();
            });
        };

        $scope.deactivateSubPoicy = function (modalData) {
            modalData.AllocRelations.StartDate = modalData.startDate;
            modalData.AllocRelations.EndDate = modalData.endDate;
            datalayer.dldata.allocPolicy.AllocRelations.push(JSON.parse(JSON.stringify(modalData.AllocRelations)));
            datalayer.saveAllocPolicy(datalayer.dldata.allocPolicy).then(function () {
                datalayer.resetList();
                datalayer.changeProductCategory();
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

csapp.controller('allocPolicyCtrl', ['$scope', 'allocPolicyDataLayer', 'allocPolicyFactory', '$modal', '$csModels', '$csShared',
    function ($scope, datalayer, factory, $modal, $csModels, $csShared) {
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

        $scope.setButtonStatus = function (policy) {
            if (policy.type === "") {
                $scope.dldata.buttonStatus = 'Draft';
            }
            if (policy.type === 'Approved') {
                $scope.dldata.buttonStatus = policy.type;
            }
            var today = moment();
            var endDate = moment(policy.allocRelation.EndDate);
            var diff = endDate.diff(today, 'days');
            if ((policy.type === 'Submitted') && (diff < 0)) {
                $scope.dldata.buttonStatus = 'Expired';
            }
            if ((policy.type === 'Submitted') && (diff >= 0)) {
                $scope.dldata.buttonStatus = 'UnApproved';
            }
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
            $scope.dldata.buttonStatus = "";
            $scope.modalData.AllocRelations = { AllocSubpolicy: relation.AllocSubpolicy };
            $scope.modalData.subPolicyIndex = -1;
            $scope.modalData.startDate = null;
            $scope.modalData.endDate = null;
            $scope.modalData.forActivate = true;
            openModal($scope.modalData);
        };

        $scope.openModelDeactivateSubPolicy = function (relation) {
            $scope.dldata.buttonStatus = "";
            $scope.modalData.AllocRelations = { AllocSubpolicy: relation.AllocSubpolicy, OrigEntityId: relation.Id };
            $scope.modalData.AllocRelations.Status = "Submitted";
            $scope.modalData.subPolicyIndex = -1;
            $scope.modalData.startDate = relation.StartDate;
            $scope.modalData.endDate = null;
            $scope.modalData.forActivate = false;
            openModal($scope.modalData);
        };

        $scope.openModelNewSubPolicy = function (policy, index) {
            $scope.dldata.buttonStatus = "";
            $scope.modalData.AllocRelations = { AllocSubpolicy: policy.subpolicy };
            var indexl = findIndex($scope.dldata.subPolicyList, policy.subpolicy.Id);
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

        $scope.setDisplaySubpolicy = function (subpolicy, relation, index) {
            if (angular.isUndefined(relation)) {
                $scope.allocRelation = subpolicy;
            } else {
                $scope.allocRelation = relation;
            }

            $scope.disSubPolicy = factory.getDisplaySubPolicy(subpolicy);

            var isinlist = _.find($scope.dldata.ApproveUnapp, function (item) {
                if (item.allocRelation.Id === relation.Id) {
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
                var maxindex = ($scope.dldata.ApproveUnapp.length) - 1;
                if (maxindex === index) {
                    $scope.direction.down = true;
                }
            }
        };

        $scope.approve = function (policy) {
            datalayer.approveRelation(policy.allocRelation).then(function () {
                policy.Status = 'Approved';
                datalayer.resetList();
                $scope.changeProductCategory();
            });
        };

        $scope.reject = function (policy) {
            datalayer.RejectSubPolicy(policy.allocRelation).then(function () {
                policy.Status = 'Rejected';
                datalayer.resetList();
                $scope.changeProductCategory();
            });
        };

        (function () {
            $scope.factory = factory;
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.datalayer.reset();
            $scope.datalayer.resetList();
            $scope.modalData = {
                AllocRelation: {},
                StartDate: null,
                endDate: null,
                subPolicyIndex: -1,
                forActivate: true
            };
            $scope.direction = {
                up: true,
                down: true
            };
            $scope.dldata.buttonStatus = "";
            $scope.allocpolicy = $csModels.getColumns("AllocPolicy");
            $scope.allocpolicy.Product.valueList = _.reject($csShared.enums.Products, function (item) {
                return (item === "UNKNOWN" || item === "ALL");
            });
        })();

        $scope.changeProductCategory = function () {
            datalayer.resetList();
            datalayer.changeProductCategory();
        };

        $scope.moveUp = function (policy) {
            var test = [];
            _.forEach($scope.dldata.ApproveUnapp, function (item) {
                _.forEach($scope.dldata.allocPolicy.AllocRelations, function (rel) {
                    if (angular.isDefined(rel)) {
                        if (item.allocRelation.Id === rel.Id) {
                            $scope.dldata.allocPolicy.AllocRelations.splice($scope.dldata.allocPolicy.AllocRelations.indexOf(rel), 1);
                            test.push(rel);
                        }
                    }
                });
            });

            var relations = _.sortBy(test, 'Priority');
            console.log(relations);
            var index = relations.indexOf(policy.allocRelation);
            var tempPriority = relations[index].Priority;
            relations[index].Priority = relations[index - 1].Priority;
            relations[index - 1].Priority = tempPriority;
            _.forEach(relations, function (item) {
                $scope.dldata.allocPolicy.AllocRelations.push(item);
            });
            datalayer.saveAllocPolicy($scope.dldata.allocPolicy).then(function () {
                $scope.changeProductCategory();
                $scope.direction = {
                    up: true,
                    down: true
                };
            });
        };
        $scope.moveDown = function (policy) {
            var test = [];
            _.forEach($scope.dldata.ApproveUnapp, function (item) {
                _.forEach($scope.dldata.allocPolicy.AllocRelations, function (rel) {
                    if (angular.isDefined(rel)) {
                        if (item.allocRelation.Id === rel.Id) {
                            $scope.dldata.allocPolicy.AllocRelations.splice($scope.dldata.allocPolicy.AllocRelations.indexOf(rel), 1);
                            test.push(rel);
                        }
                    }
                });
            });

            var relations = _.sortBy(test, 'Priority');
            console.log(relations);
            var index = relations.indexOf(policy.allocRelation);
            var tempPriority = relations[index].Priority;
            relations[index].Priority = relations[index + 1].Priority;
            relations[index + 1].Priority = tempPriority;
            _.forEach(relations, function (item) {
                $scope.dldata.allocPolicy.AllocRelations.push(item);
            });
            datalayer.saveAllocPolicy($scope.dldata.allocPolicy).then(function () {
                $scope.changeProductCategory();
                $scope.direction = {
                    up: true,
                    down: true
                };
            });
        };
    }]);

csapp.factory('allocPolicyDataLayer', ['Restangular', '$csnotify', '$csfactory',
    function (rest, $csnotify, $csfactory) {

        var dldata = {};
        var api = rest.all('AllocationPolicyApi');

        dldata.ApproveUnapp = [];
        dldata.draftAndExpired = [];
        dldata.subpolicyList = [];
        dldata.subpolicyObj = {
            Name: "",
            type: "",
            allocRelation: {},
            subpolicy: {}
        };
        dldata.buttonStatus = "";
        dldata.listOne = [];
        dldata.listTwo = [];
        dldata.listThree = [];
        dldata.listFour = [];
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

                return api.customGET("GetAllocPolicy", { products: allocPolicy.Products, category: allocPolicy.Category }).then(function (data) {
                    if (data.AllocPolicy.AllocRelations.length === 0 && data.UnUsedSubpolicies.length === 0) {
                        $csnotify.success("Policy not Available ");
                    }
                    dldata.buttonStatus = "";
                    dldata.allocPolicy = data.AllocPolicy;
                    dldata.subPolicyList = data.UnUsedSubpolicies;
                    dldata.listOne = _.filter(data.AllocPolicy.AllocRelations, function (row) {
                        return frelation(row, false, '');
                    });
                    //  "Expired/Draft"
                    _.forEach(dldata.listOne, function (row) {
                        dldata.subpolicyObj = {
                            Name: row.AllocSubpolicy.Name,
                            type: row.Status,
                            allocRelation: row,
                            subpolicy: row.AllocSubpolicy
                        };
                        dldata.listTwo.push(dldata.subpolicyObj);
                    });
                    _.forEach(data.UnUsedSubpolicies, function (row) {
                        dldata.subpolicyObj = {
                            Name: row.Name,
                            type: '',
                            allocRelation: {},
                            subpolicy: row
                        };
                        dldata.subpolicyList.push(dldata.subpolicyObj);
                    });
                    dldata.draftAndExpired = _.union(dldata.listTwo, dldata.subpolicyList);
                    dldata.listTwo = [];
                    dldata.listOne = [];
                    // Approved/Unapproved
                    dldata.listOne = _.filter(data.AllocPolicy.AllocRelations, function (row) {
                        return frelation(row, true, 'Approved');
                    });
                    _.forEach(dldata.listOne, function (row) {
                        dldata.subpolicyObj = {
                            Name: row.AllocSubpolicy.Name,
                            type: row.Status,
                            Priority: row.Priority,
                            allocRelation: row,
                            subpolicy: row.AllocSubpolicy
                        };
                        dldata.listTwo.push(dldata.subpolicyObj);
                    });
                    dldata.listThree = _.filter(data.AllocPolicy.AllocRelations, function (row) {
                        return frelation(row, true, 'Submitted');
                    });
                    _.forEach(dldata.listThree, function (row) {
                        dldata.subpolicyObj = {
                            Name: row.AllocSubpolicy.Name,
                            type: row.Status,
                            Priority: row.Priority,
                            allocRelation: row,
                            subpolicy: row.AllocSubpolicy
                        };
                        dldata.listFour.push(dldata.subpolicyObj);
                    });
                    dldata.ApproveUnapp = _.union(dldata.listTwo, dldata.listFour);
                });
            }
        };

        var frelation = function (relation, todayActive, status) {
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
        var resetList = function () {
            dldata.ApproveUnapp = [];
            dldata.draftAndExpired = [];
            dldata.subpolicyList = [];
            dldata.listOne = [];
            dldata.listTwo = [];
            dldata.listThree = [];
            dldata.listFour = [];
        };

        return {
            dldata: dldata,
            getProducts: getProducts,
            changeProductCategory: changeProductCategory,
            RejectSubPolicy: rejectSubPolicy,
            saveAllocPolicy: saveAllocPolicy,
            reset: reset,
            approveRelation: approveRelation,
            resetList: resetList,
            frelation: frelation
        };
    }]);

csapp.factory('allocPolicyFactory', ['allocPolicyDataLayer', function (datalayer) {


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

    //var filterRelation = function (todayActive, status) {
    //    return function (relation) {
    //        var today = moment();
    //        var endDate = relation.EndDate ? moment(relation.EndDate) : moment();
    //        var diff = endDate.diff(today, 'days');
    //        var dateFilter = ((diff >= 0) === todayActive);
    //        var statusfilter = true;
    //        if (status != '') {
    //            statusfilter = relation.Status == status;
    //        }
    //        return (dateFilter && statusfilter);
    //    };
    //};

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

    //var upside = function (subPolicy, index) {
    //    var test = _.filter(datalayer.dldata.allocPolicy.AllocRelations, function (subpolicy) {
    //        return (subpolicy.Status == 'Approved');
    //    });
    //    var relations = _.sortBy(test, 'Priority');
    //    var tempPriority = relations[index].Priority;
    //    relations[index].Priority = relations[index - 1].Priority;
    //    relations[index - 1].Priority = tempPriority;
    //    datalayer.saveAllocPolicy(datalayer.dldata.allocPolicy);
    //};

    //var downside = function (subPolicy, index) {
    //    var test = _.filter(datalayer.dldata.allocPolicy.AllocRelations, function (subpolicy) {
    //        return (subpolicy.Status == 'Approved');
    //    });
    //    var relations = _.sortBy(test, 'Priority');

    //    var tempPriority = relations[index].Priority;
    //    relations[index].Priority = relations[index + 1].Priority;
    //    relations[index + 1].Priority = tempPriority;
    //    datalayer.saveAllocPolicy(datalayer.dldata.allocPolicy);
    //};



    return {
        getDisplaySubPolicy: getDisplaySubPolicy,
        //filterRelation: filterRelation,
        expiredPolicyStatus: expiredPolicyStatus,
        relationValidToday: relationValidToday,
        //upside: upside,
        //downside: downside
    };
}]);

//$scope.frelation = function (relation, todayActive, status) {
//    var today = moment();
//    // var startDate = moment(relation.StartDate);
//    var endDate = relation.EndDate ? moment(relation.EndDate) : moment();
//    var diff = endDate.diff(today, 'days');
//    var dateFilter = ((diff >= 0) === todayActive);
//    var statusfilter = true;
//    if (status != '') {
//        statusfilter = relation.Status == status;
//    }
//    return (dateFilter && statusfilter);
//};
//var setStatus = function (status) {
//    if (status === 'Rejected') {
//        datalayer.dldata.color = { color: 'red' };
//    }
//    if (status === 'Approved') {
//        datalayer.dldata.color = { color: 'green' };
//    }
//    if (status === 'Submitted') {
//        datalayer.dldata.color = { color: 'blue' };
//    }
//    return status;
//};