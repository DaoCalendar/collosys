﻿csapp.controller("allocSubpolicyCtrl1", ["$scope", "$csnotify", "$csfactory", "Restangular", '$Validations', function ($scope, $csnotify, $csfactory, rest, $validation) {
    "use strict";

    var restApi = rest.all("AllocationSubPolicyApi");

    $scope.val = $validation;
    $scope.allocSubpolicyList = [];
    $scope.stakeholderList = [];
    $scope.allocSubpolicy = {};
    $scope.newCondition = {};
    $scope.allocSubpolicy.Conditions = [];
    $scope.allocSubpolicy.DoAllocate = 1;
    $scope.allocSubpolicy.NoAllocMonth = 1;


    $scope.allocSubpolicy.Category = "Liner";
    $scope.newCondition.Rtype = "Value";
    $scope.conditionOperators = ["EqualTo", "NotEqualTo", "LessThan", "LessThanEqualTo", "GreaterThan", "GreaterThanEqualTo"];

    $scope.relationTypeSwitch = [{ Name: 'And', Value: 'And' }, { Name: 'Or', Value: 'Or' }];
    $scope.categorySwitch = [{ Name: 'Collection', Value: 'Liner' }, { Name: 'Recovery', Value: 'WriteOff' }];
    //$scope.allocSubpolicy.NoAllocMonth = false;
    $scope.openDateModel = false;
    $scope.modalData = {};
    $scope.isDuplicateName = false;
    $scope.policyapproved = false;



    $scope.modelDateValidation = function (startDate, endDate) {
        if (angular.isUndefined(endDate) || endDate == null) {
            $scope.isModalDateValid = true;
            return;
        }
        startDate = moment(startDate);
        endDate = moment(endDate);
        $scope.isModalDateValid = (endDate > startDate);
    };

    $scope.activateSubPoicy = function (modelData) {
        $scope.curRelation.StartDate = modelData.startDate;
        $scope.curRelation.EndDate = modelData.endDate;
        restApi.customPOST($scope.curRelation, "ActivateSubpolicy").then(function (data) {
            data.AllocPolicy = null;
            data.AllocSubpolicy = null;
            $scope.curRelation = data;
            $csnotify.success("Policy Activated");
        });
    };

    $scope.showStartEndModalPopup = function () {
        $scope.modalData.forActivate = true;
        $scope.modalData.startDate = null;
        $scope.modalData.endDate = null;
        $scope.openDateModel = true;
    };

}]);

csapp.controller('allocSubpolicyCtrl', ['$scope', 'subpolicyDataLayer', 'subpolicyFactory',
    function ($scope, datalayer, factory) {
        "use strict";


        (function () {
            $scope.factory = factory;
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.datalayer.getProducts();
            $scope.datalayer.getReasons();
            console.log($scope.dldata);
        })();

        $scope.dldata.SubpolicyStakeholderList = [{ display: "Handle By Telecaller", value: "HandleByTelecaller" },
        { display: "Do Not Allocate", value: "DoNotAllocate" },
        { display: "Allocate As Per Stakeholder Working", value: "AllocateAsPerPolicy" },
        { display: "Allocate to Particular Stakeholder", value: "AllocateToStkholder" }];

        $scope.$watch('allocSubpolicy.AllocateType', factory.watchAllocateType());

    }]);

csapp.factory('subpolicyDataLayer', ['Restangular', '$csnotify',
    function (rest, $csnotify) {

        var dldata = {};
        dldata.dateValueEnum = ["First_Quarter", "Second_Quarter", "Third_Quarter", "Fourth_Quarter", "Start_of_Year", "Start_of_Month", "Start_of_Week", "Today", "End_of_Week", "End_of_Month", "End_of_Year", "Absolute_Date"];
        var restApi = rest.all("AllocationSubPolicyApi");

        var getProducts = function () {
            restApi.customGET("GetProducts").then(function (data) {
                dldata.productsList = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });
        };

        var getReasons = function () {
            restApi.customGET("GetReasons").then(function (data) {
                dldata.reasonsNotAllocate = data;
            }, function (data) {
                $csnotify.error(data);
            });
        };

        var selectAllocSubpolicy = function (sallocSubpolicy) {
            dldata.IsPolicyApproved = false;

            var subpolicy = angular.copy(sallocSubpolicy);
            dldata.allocSubpolicy = sallocSubpolicy;

            restApi.customGET("GetConditions", { allocationId: sallocSubpolicy.Id }).then(function (data) {
                changeProductCategory();
                dldata.allocSubpolicy.Conditions = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });
            restApi.customPOST(subpolicy, "GetRelations").then(function (relation) {
                dldata.curRelation = relation;
                setIsPolicyApproved(dldata.curRelation);
            });
        };

        var setIsPolicyApproved = function (data) {

            if (data.Status === 'Approved') {
                dldata.IsPolicyApproved = true;
                dldata.policyapproved = true;
                $csnotify.success("Policy is already Approved");
            } else {
                dldata.policyapproved = false;
            }
        };

        var changeProductCategory = function () {

            if (angular.isUndefined(dldata.allocSubpolicy.Id)) {
                dldata.allocSubpolicy.Conditions = [];
                dldata.allocSubpolicy.Name = '';
                dldata.isDuplicateName = false;
                dldata.allocSubpolicy.AllocateType = '';
                dldata.allocSubpolicy.NoAllocMonth = 1;
            } else {
                check($scope.allocSubpolicy.ReasonNotAllocate);
            }
            resetCondition();

            dldata.allocSubpolicy.Category = 'Liner';

            var allocSubpolicy = dldata.allocSubpolicy;

            if (!angular.isUndefined(allocSubpolicy.Products) && !angular.isUndefined(allocSubpolicy.Category)) {

                // get sub policy list
                restApi.customGET("GetSubPolicy", { products: allocSubpolicy.Products, category: allocSubpolicy.Category }).then(function (data) {
                    dldata.allocSubpolicyList = data;
                }, function (data) {
                    $csnotify.error(data.data.Message);
                });

                //get column names
                restApi.customGET("GetConditionColumns", { products: allocSubpolicy.Products, category: allocSubpolicy.Category }).then(function (data) {
                    dldata.columnDefs = data;
                    dldata.condLcolumnNames = _.pluck(data, 'field');
                }, function (data) {
                    $csnotify.error(data);
                });

                //stakeholderList
                restApi.customGET('GetStakeholders', { products: allocSubpolicy.Products }).then(function (data) {

                    dldata.stakeholderList = data;
                }, function (data) {
                    $csnotify.error(data);
                });
            } else {
                dldata.LcolumnNames = [];
            }
        };

        var check = function (val) {
            var arr = [];
            arr = val.split(" ");
            _.find(arr, function (string) {
                if (string == 'dead' || string == 'died') {
                    dldata.allocSubpolicy.NoAllocMonth = 0;
                    dldata.readTrue = true;
                } else {
                    dldata.allocSubpolicy.NoAllocMonth = 1;
                    dldata.readTrue = false;
                }
            });
        };

        var resetCondition = function () {
            dldata.newCondition = {};
        };

        var getColumnValues = function (columnName) {
            restApi.customGET('GetValuesofColumn', { columnName: columnName }).then(function (data) {
                dldata.conditionValues = data;
            }, function (data) {
                $csnotify.error(data);
            });
        };

        var saveAllocSubpolicy = function (allocSubpolicy) {
            if (allocSubpolicy.Stakeholder && allocSubpolicy.Stakeholder.Id) {
                allocSubpolicy.Stakeholder = _.find($scope.stakeholderList, { Id: allocSubpolicy.Stakeholder.Id });
            }

            if (dldata.allocSubpolicy.Products === 'CC') {
                dldata.allocSubpolicy.Category = 'Liner';
            }


            if (allocSubpolicy.Id) {

                restApi.customPUT(allocSubpolicy, "Put", { id: allocSubpolicy.Id }).then(function (data) {
                    dldata.allocSubpolicyList = _.reject(dldata.allocSubpolicyList, function (subpolicy) { return subpolicy.Id == data.Id; });
                    dldata.allocSubpolicyList.push(data);
                    dldata.resetAllocSubpolicy(data.Products, data.Category);
                    dldata.selectAllocSubpolicy(data);
                    $csnotify.success("Alloc Subpolicy saved");
                }, function (data) {
                    $csnotify.error(data);
                });

            } else {
                restApi.customPOST(allocSubpolicy, "Post").then(function (data) {
                    dldata.allocSubpolicyList = _.reject(dldata.allocSubpolicyList, function (subpolicy) { return subpolicy.Id == data.Id; });
                    dldata.allocSubpolicyList.push(data);
                    dldata.resetAllocSubpolicy(data.Products, data.Category);
                    dldata.selectAllocSubpolicy(data);
                    $csnotify.success("Alloc Subpolicy saved");
                }, function (data) {
                    $csnotify.error(data);
                });
            }
        };

        return {
            dldata: dldata,
            getProducts: getProducts,
            getReasons: getReasons,
            selectAllocSubpolicy: selectAllocSubpolicy,
            getColumnValues: getColumnValues,
            saveAllocSubpolicy: saveAllocSubpolicy,
            changeProductCategory: changeProductCategory
        };

    }]);


csapp.factory('subpolicyFactory', ['subpolicyDataLayer', '$csfactory', '$csnotify',
    function (datalayer, $csfactory, $csnotify) {
        var dldata = datalayer.dldata;

        var disableIfRelationExists = function () {
            if (angular.isDefined(dldata.curRelation)) {
                if ($csfactory.isNullOrEmptyString(dldata.curRelation.AllocPolicy) || $csfactory.isNullOrEmptyString(dldata.curRelation.AllocPolicy))
                    return true;
                else return false;
            }
            return false;
        };

        var checkDuplicateName = function () {
            dldata.isDuplicateName = false;
            _.forEach(dldata.allocSubpolicyList, function (subpolicy) {
                if (subpolicy.Name.toUpperCase() == dldata.allocSubpolicy.Name.toUpperCase()) {
                    dldata.isDuplicateName = true;
                    return;
                }
            });
        };

        var watchAllocateType = function () {

            if (angular.isUndefined(dldata))
                return;
            if (angular.isUndefined(dldata.allocSubpolicy))
                return;
            if (angular.isUndefined(dldata.allocSubpolicy.AllocateType))
                return;

            if (dldata.allocSubpolicy.AllocateType !== 'DoNotAllocate')
                dldata.allocSubpolicy.ReasonNotAllocate = '';

            if (dldata.allocSubpolicy.AllocateType !== 'AllocateToStkholder')
                dldata.allocSubpolicy.Stakeholder = {};
        };

        var changeLeftColName = function (condition) {
            dldata.selectedLeftColumn = _.find(dldata.columnDefs, { field: condition.ColumnName });
            var inputType = dldata.selectedLeftColumn.InputType;
            if (inputType === "text") {
                dldata.conditionOperators = ["EqualTo", "NotEqualTo", "Contains", "StartsWith", "EndsWith"];
                condition.Operator = '';
                condition.Rtype = 'Value';
                condition.Rvalue = '';
                datalayer.getColumnValues(condition.ColumnName);
                return;
            }

            if (inputType === "checkbox") {
                dldata.conditionOperators = ["EqualTo"];
                condition.Operator = "EqualTo";
                condition.Rtype = 'Value';
                condition.Rvalue = '';
                return;
            }

            if (inputType === "dropdown") {
                dldata.conditionOperators = ["EqualTo", "NotEqualTo"];
                dldata.conditionValues = dldata.selectedLeftColumn.dropDownValues;
                condition.Rtype = 'Value';
                condition.Rvalue = '';
                return;
            }

            dldata.conditionOperators = ["EqualTo", "NotEqualTo", "LessThan", "LessThanEqualTo", "GreaterThan", "GreaterThanEqualTo"];
            condition.Operator = '';
            condition.Rtype = 'Value';
            condition.Rvalue = '';
        };

        var addNewCondition = function (condition) {

            var duplicateCond = _.find(dldata.allocSubpolicy.Conditions, function (cond) {
                return (cond.ColumnName == condition.ColumnName && cond.Operator == condition.Operator && cond.Value == condition.Value);
            });

            if (duplicateCond) {
                $csnotify.error("condition is duplicate");
                return;
            }

            condition.Priority = dldata.allocSubpolicy.Conditions.length;

            if (condition.dateValueEnum && condition.dateValueEnum != 'Absolute_Date') {
                condition.Value = condition.dateValueEnum;
            }

            var con = angular.copy(condition);
            dldata.allocSubpolicy.Conditions.push(con);
            dldata.conditionValueType = 'text';
            resetCondition();
        };

        var deleteCondition = function (condition, index) {
            dldata.allocSubpolicy.Conditions[0].RelationType = "";
            dldata.allocSubpolicy.Conditions.splice(index, 1);
            for (var i = index; i < dldata.allocSubpolicy.Conditions.length; i++) {
                dldata.allocSubpolicy.Conditions[i].Priority = i;
            }
        };

        var resetAllocSubpolicy = function (products, category) {
            dldata.policyapproved = false;
            dldata.allocSubpolicy = {};
            dldata.allocSubpolicy.Conditions = [];
            dldata.isDuplicateName = false;
            dldata.deleteConditions = [];
            dldata.newCondition = {};
            dldata.allocSubpolicy.Products = products;
            dldata.allocSubpolicy.Category = category;
            dldata.allocSubpolicy.DoAllocate = 1;
            dldata.allocSubpolicy.NoAllocMonth = 1;
            resetCondition();
        };

        var resetCondition = function () {
            dldata.newCondition = {};
        };

        return {
            disableIfRelationExists: disableIfRelationExists,
            checkDuplicateName: checkDuplicateName,
            watchAllocateType: watchAllocateType,
            changeLeftColName: changeLeftColName,
            addNewCondition: addNewCondition,
            deleteCondition: deleteCondition,
            resetAllocSubpolicy: resetAllocSubpolicy
        };

    }]);