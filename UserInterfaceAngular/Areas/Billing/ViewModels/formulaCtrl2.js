/// <reference path="../../../Scripts/angular.js" />

csapp.controller("formulaCtrl", ["$scope", "$csnotify", "Restangular", function ($scope, $csnotify, rest) {
    "use strict";

    var restApi = rest.all("formulaApi");
    $scope.productsList = [];
    $scope.formulaList = [];
    $scope.formula = {};
    $scope.formula.BConditions = [];
    $scope.formula.Category = "Liner";
    $scope.deleteConditions = [];
    $scope.conditionColumns = [];
    $scope.conditionOperators = [];
    $scope.newCondition = {};
    $scope.categorySwitch = [{ Name: 'Liner', Value: 'Liner' }, { Name: 'Writeoff', Value: 'Writeoff' }];
    $scope.relationTypeSwitch = [{ Name: 'And', Value: 'And' }, { Name: 'Or', Value: 'Or' }];
    //$scope.outputs = [];

    restApi.customGET("GetProducts").then(function (data) {
        $scope.productsList = data;
    }, function (data) {
        $csnotify.error(data);
    });

    restApi.customGET("Get").then(function (data) {
        $scope.formulaList = data;
    }, function (data) {
        $csnotify.error(data);
    });

    $scope.changeProductCategory = function () {
        var formula = $scope.formula;
        if (!angular.isUndefined(formula.Products) && !angular.isUndefined(formula.Category)) {
            restApi.customGET("GetColumnNames", { product: formula.Products, category: formula.Category }).then(function (data) {
                $scope.conditionColumns = data;
            }, function (data) {
                $csnotify.error(data);
            });
        } else {
            $scope.conditionColumns = [];
        }
    };

    $scope.leftColumnChange = function (selectedColumn, newCondition) {
        newCondition.Ltype = "Column";
        newCondition.Lsqlfunction = "";
        newCondition.LtypeName = selectedColumn.field;

        if (selectedColumn.InputType === "text") {
            $scope.conditionOperators = ["Equal", "Not Equal", "Contains", "Start With", "End With"];
            newCondition.Operator = '';
            newCondition.Rvalue = '';
            return;
        }

        if (selectedColumn.InputType === "checkbox") {
            $scope.conditionOperators = ["Equal"];
            newCondition.Operator = "Equal";
            newCondition.Rvalue = '';
            return;
        }

        if (selectedColumn.InputType === "dropdown") {
            $scope.conditionOperators = ["Equal", "Not Equal"];
            $scope.conditionValues = selectedColumn.dropDownValues;
            newCondition.Rvalue = '';
            return;
        }

        $scope.conditionOperators = ["=", "!=", "<", "<=", ">", ">="];
        newCondition.Operator = '';
        newCondition.Rvalue = '';
    };

    $scope.addNewCondition = function (condition) {
        //condition.FormulaId = $scope.formula;
        condition.Priority = $scope.formula.BConditions.length;
        var con = angular.copy(condition);
        $scope.formula.BConditions.push(con);
        $scope.conditionValueType = 'text';

        $scope.resetCondition();
    };

    $scope.deleteCondition = function (condition, index) {
        if (condition.Id) {
            $scope.deleteConditions.push(angular.copy(condition));
        }

        $scope.formula.BConditions.splice(index, 1);
        $scope.formula.BConditions[0].RelationType = "";

        for (var i = index; i < $scope.formula.BConditions.length; i++) {
            $scope.formula.BConditions[i].Priority = i;
        }
    };

    $scope.saveFormula = function (formula) {
        var savedata = {
            formula: formula,
            deleteConditions: $scope.deleteConditions
        };

        if (formula.Id) {

            restApi.customPUT(formula, "Put", { id: formula.Id }).then(function (data) {
                $scope.formulaList = _.reject($scope.formulaList, function (form) { return form.Id == data.Id; });
                $scope.formulaList.push(data);
                $scope.formula = data;
                $csnotify.success("Formula saved");
            }, function (data) {
                $csnotify.error(data);
            });

        } else {
            restApi.customPOST(formula, "POST").then(function (data) {
                $scope.formulaList = _.reject($scope.formulaList, function (form) { return form.Id == data.Id; });
                $scope.formulaList.push(data);
                $scope.formula = data;
                $csnotify.success("Formula saved");
            }, function (data) {
                $csnotify.error(data);
            });
        }
    };


    $scope.selectFormula = function (sformula) {
        debugger;
        $scope.formula = sformula;
        $scope.resetCondition();
    };

    $scope.resetCondition = function () {
        $scope.newCondition = {};
        if ($scope.formula.BConditions.length < 1) {
            $scope.newCondition.RelationType = '';
        } else {
            $scope.newCondition.RelationType = 'And';
        }

        selectedLeftColumn = {};
        selectedRightColumn = {};
    };

}]);