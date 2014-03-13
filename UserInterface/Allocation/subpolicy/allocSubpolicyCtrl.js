/// <reference path="../../../Scripts/angular.js" />


(csapp.controller("allocSubpolicyCtrl", ["$scope", "$csnotify", "$csfactory", "Restangular", '$Validations', function ($scope, $csnotify, $csfactory, rest, $validation) {
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
    $scope.SubpolicyStakeholderList = [{ display: "Handle By Telecaller", value: "HandleByTelecaller" },
        { display: "Do Not Allocate", value: "DoNotAllocate" },
        { display: "Allocate As Per Stakeholder Working", value: "AllocateAsPerPolicy" },
        { display: "Allocate to Particular Stakeholder", value: "AllocateToStkholder" }];

    $scope.allocSubpolicy.Category = "Liner";
    $scope.newCondition.Rtype = "Value";
    $scope.conditionOperators = ["EqualTo", "NotEqualTo", "LessThan", "LessThanEqualTo", "GreaterThan", "GreaterThanEqualTo"];
    $scope.dateValueEnum = ["First_Quarter", "Second_Quarter", "Third_Quarter", "Fourth_Quarter", "Start_of_Year", "Start_of_Month", "Start_of_Week", "Today", "End_of_Week", "End_of_Month", "End_of_Year", "Absolute_Date"];
    $scope.relationTypeSwitch = [{ Name: 'And', Value: 'And' }, { Name: 'Or', Value: 'Or' }];
    $scope.categorySwitch = [{ Name: 'Collection', Value: 'Liner' }, { Name: 'Recovery', Value: 'WriteOff' }];
    //$scope.allocSubpolicy.NoAllocMonth = false;
    $scope.openDateModel = false;
    $scope.modalData = {};
    $scope.isDuplicateName = false;
    $scope.policyapproved = false;

    restApi.customGET("GetProducts").then(function (data) {
        $scope.productsList = data;
    }, function (data) {
        $csnotify.error(data.data.Message);
    });


    restApi.customGET("GetReasons").then(function (data) {
        $scope.reasonsNotAllocate = data;
    }, function (data) {
        $csnotify.error(data);
    });

    $scope.selectAllocSubpolicy = function (sallocSubpolicy) {
        $scope.IsPolicyApproved = false;

        var subpolicy = angular.copy(sallocSubpolicy);
        $scope.allocSubpolicy = sallocSubpolicy;
        //$scope.allocSubpolicy.StakeholderId = sallocSubpolicy.Stakeholder.Id;

        if (!$scope.$$phase) $scope.$apply();
        restApi.customGET("GetConditions", { allocationId: sallocSubpolicy.Id }).then(function (data) {
            $scope.changeProductCategory();
            $scope.allocSubpolicy.Conditions = data;
        }, function (data) {
            $csnotify.error(data.data.Message);
        });
        restApi.customPOST(subpolicy, "GetRelations").then(function (relation) {
            $scope.curRelation = relation;
            setIsPolicyApproved($scope.curRelation);
        });
    };

    var setIsPolicyApproved = function (data) {

        if (data.Status === 'Approved') {
            $scope.IsPolicyApproved = true;
            $scope.policyapproved = true;
            //$scope.policyapproved = true;
            $csnotify.success("Policy is already Approved");
        } else {
            $scope.policyapproved = false;
        }
    };



    $scope.disableIfRelationExists = function () {
        if (angular.isDefined($scope.curRelation)) {
            if ($csfactory.isNullOrEmptyString($scope.curRelation.AllocPolicy) || $csfactory.isNullOrEmptyString($scope.curRelation.AllocPolicy))
                return true;
            else return false;
        }
    };

    $scope.checkDuplicateName = function () {
        $scope.isDuplicateName = false;
        _.forEach($scope.allocSubpolicyList, function (subpolicy) {
            if (subpolicy.Name.toUpperCase() == $scope.allocSubpolicy.Name.toUpperCase()) {
                $scope.isDuplicateName = true;
                return;
            }
        });
    };

    $scope.changeProductCategory = function () {

        if (angular.isUndefined($scope.allocSubpolicy.Id)) {
            $scope.allocSubpolicy.Conditions = [];
            $scope.allocSubpolicy.Name = '';
            $scope.isDuplicateName = false;
            $scope.allocSubpolicy.AllocateType = '';
            $scope.allocSubpolicy.NoAllocMonth = 1;
        } else {
            $scope.check($scope.allocSubpolicy.ReasonNotAllocate);
        }
        $scope.resetCondition();

        //$scope.resetAllocSubpolicy($scope.allocSubpolicy);    
        //check if product =='CC' then set category='Liner'
        //if ($scope.allocSubpolicy.Products == 'CC') {
        $scope.allocSubpolicy.Category = 'Liner';
        //}

        var allocSubpolicy = $scope.allocSubpolicy;

        if (!angular.isUndefined(allocSubpolicy.Products) && !angular.isUndefined(allocSubpolicy.Category)) {

            // get sub policy list
            restApi.customGET("GetSubPolicy", { products: allocSubpolicy.Products, category: allocSubpolicy.Category }).then(function (data) {
                $scope.allocSubpolicyList = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });

            //get column names
            restApi.customGET("GetConditionColumns", { products: allocSubpolicy.Products, category: allocSubpolicy.Category }).then(function (data) {
                $scope.columnDefs = data;
                $scope.condLcolumnNames = _.pluck(data, 'field');
            }, function (data) {
                $csnotify.error(data);
            });

            //stakeholderList
            restApi.customGET('GetStakeholders', { products: allocSubpolicy.Products }).then(function (data) {

                $scope.stakeholderList = data;
            }, function (data) {
                $csnotify.error(data);
            });
        } else {
            $scope.LcolumnNames = [];
        }
    };

    $scope.resetCondition = function () {
        $scope.newCondition = {};

    };

    $scope.$watch('allocSubpolicy.AllocateType', function () {

        if (angular.isUndefined($scope.allocSubpolicy.AllocateType))
            return;

        if ($scope.allocSubpolicy.AllocateType !== 'DoNotAllocate')
            $scope.allocSubpolicy.ReasonNotAllocate = '';

        if ($scope.allocSubpolicy.AllocateType !== 'AllocateToStkholder')
            $scope.allocSubpolicy.Stakeholder = {};
    });

    var getColumnValues = function (columnName) {
        restApi.customGET('GetValuesofColumn', { columnName: columnName }).then(function (data) {
            $scope.conditionValues = data;
        }, function (data) {
            $csnotify.error(data);
        });
    };

    $scope.changeLeftColName = function (condition) {
        $scope.selectedLeftColumn = _.find($scope.columnDefs, { field: condition.ColumnName });
        var inputType = $scope.selectedLeftColumn.InputType;
        if (inputType === "text") {
            $scope.conditionOperators = ["EqualTo", "NotEqualTo", "Contains", "StartsWith", "EndsWith"];
            condition.Operator = '';
            condition.Rtype = 'Value';
            condition.Rvalue = '';
            getColumnValues(condition.ColumnName);
            return;
        }

        if (inputType === "checkbox") {
            $scope.conditionOperators = ["EqualTo"];
            condition.Operator = "EqualTo";
            condition.Rtype = 'Value';
            condition.Rvalue = '';
            return;
        }

        if (inputType === "dropdown") {
            $scope.conditionOperators = ["EqualTo", "NotEqualTo"];
            $scope.conditionValues = $scope.selectedLeftColumn.dropDownValues;
            condition.Rtype = 'Value';
            condition.Rvalue = '';
            return;
        }

        $scope.conditionOperators = ["EqualTo", "NotEqualTo", "LessThan", "LessThanEqualTo", "GreaterThan", "GreaterThanEqualTo"];
        condition.Operator = '';
        condition.Rtype = 'Value';
        condition.Rvalue = '';
    };
    //#region Allocate Moth for died stakeholer
    $scope.check = function (val) {
        //debugger;
        var arr = [];
        arr = val.split(" ");
        _.find(arr, function (string) {
            if (string == 'dead' || string == 'died') {
                $scope.allocSubpolicy.NoAllocMonth = 0;
                $scope.readTrue = true;
            } else {
                $scope.allocSubpolicy.NoAllocMonth = 1;
                $scope.readTrue = false;
            }
        });

    };

    //#endregion

    $scope.addNewCondition = function (condition) {

        var duplicateCond = _.find($scope.allocSubpolicy.Conditions, function (cond) {
            return (cond.ColumnName == condition.ColumnName && cond.Operator == condition.Operator && cond.Value == condition.Value);
        });

        if (duplicateCond) {
            $csnotify.error("condition is duplicate");
            return;
        }

        condition.Priority = $scope.allocSubpolicy.Conditions.length;

        if (condition.dateValueEnum && condition.dateValueEnum != 'Absolute_Date') {
            condition.Value = condition.dateValueEnum;
        }

        var con = angular.copy(condition);
        $scope.allocSubpolicy.Conditions.push(con);
        $scope.conditionValueType = 'text';

        $scope.resetCondition();
    };



    $scope.deleteCondition = function (condition, index) {

        $scope.allocSubpolicy.Conditions.splice(index, 1);
        $scope.allocSubpolicy.Conditions[0].RelationType = "";

        for (var i = index; i < $scope.allocSubpolicy.Conditions.length; i++) {
            $scope.allocSubpolicy.Conditions[i].Priority = i;
        }
    };

    $scope.resetAllocSubpolicy = function (products, category) {
        $scope.policyapproved = false;
        $scope.allocSubpolicy = {};
        $scope.allocSubpolicy.Conditions = [];
        $scope.isDuplicateName = false;
        $scope.deleteConditions = [];
        $scope.newCondition = {};
        $scope.allocSubpolicy.Products = products;
        $scope.allocSubpolicy.Category = category;
        $scope.allocSubpolicy.DoAllocate = 1;
        $scope.allocSubpolicy.NoAllocMonth = 1;

        $scope.resetCondition();
    };

    $scope.saveAllocSubpolicy = function (allocSubpolicy) {
        if (allocSubpolicy.Stakeholder && allocSubpolicy.Stakeholder.Id) {
            allocSubpolicy.Stakeholder = _.find($scope.stakeholderList, { Id: allocSubpolicy.Stakeholder.Id });
        }

        if ($scope.allocSubpolicy.Products === 'CC') {
            $scope.allocSubpolicy.Category = 'Liner';
        }


        if (allocSubpolicy.Id) {

            restApi.customPUT(allocSubpolicy, "Put", { id: allocSubpolicy.Id }).then(function (data) {
                $scope.allocSubpolicyList = _.reject($scope.allocSubpolicyList, function (subpolicy) { return subpolicy.Id == data.Id; });
                $scope.allocSubpolicyList.push(data);
                //$scope.allocSubpolicy = data;
                $scope.resetAllocSubpolicy(data.Products, data.Category);
                $scope.selectAllocSubpolicy(data);
                $csnotify.success("Alloc Subpolicy saved");
            }, function (data) {
                $csnotify.error(data);
            });

        } else {
            restApi.customPOST(allocSubpolicy, "Post").then(function (data) {
                $scope.allocSubpolicyList = _.reject($scope.allocSubpolicyList, function (subpolicy) { return subpolicy.Id == data.Id; });
                $scope.allocSubpolicyList.push(data);
                //$scope.allocSubpolicy = data;
                $scope.resetAllocSubpolicy(data.Products, data.Category);
                $scope.selectAllocSubpolicy(data);
                $csnotify.success("Alloc Subpolicy saved");
            }, function (data) {
                $csnotify.error(data);
            });
        }
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

    $scope.activateSubPoicy = function (modelData) {
        //restApi.customGET("ActivateSupolicy", { startDate: modelData.startDate.toString(), endDate: modelData.endDate.toString() }).then(function (data) {
        //    $scope.allocSubpolicyList = _.reject($scope.allocSubpolicyList, function (subpolicy) { return subpolicy.Id == data.Id; });
        //    $scope.allocSubpolicyList.push(data);
        //    $scope.resetAllocSubpolicy(data.Products, data.Category);
        //    $csnotify.success("Alloc Subpolicy saved");
        //}, function (data) {
        //    $csnotify.error(data);
        //});
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

}]));