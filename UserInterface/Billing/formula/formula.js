csapp.controller("formulaController", [
    "$scope", "$csnotify", '$csfactory', "Restangular",
    function ($scope, $csnotify, $csfactory, rest) {
        "use strict";

        var restApi = rest.all("PayoutSubpolicyApi");
        $scope.formulaList = [];
        $scope.productsList = [];
        $scope.columnNames = [];
        $scope.formulaNames = [];
        $scope.columnDefs = [];
        $scope.matrixNames = [];
        $scope.AllBConditions = [];
        $scope.formula = {};


        $scope.formula.BConditions = [];
        $scope.formula.BOutputs = [];
        $scope.deleteConditions = [];
        $scope.newCondition = {};
        $scope.newOutput = {};
        //$scope.formula.BMatricesValues = [];
        //$scope.isPayoutSubpolicyCreated = false;
        $scope.formula.Category = "Liner";
        $scope.formula.PayoutSubpolicyType = 'Formula';
        $scope.formula.OutputType = 'Boolean';
        $scope.newCondition.Rtype = 'Value';
        $scope.newOutput.Rtype = 'Value';
        $scope.outputWithFunction = false;
        //$scope.formula.Row1DType = "Table";
        //$scope.formula.Column2DType = "Table";
        //$scope.formula.Row3DType = "Table";
        //$scope.formula.Column4DType = "Table";
        $scope.conditionOperators = ["EqualTo", "NotEqualTo", "LessThan", "LessThanEqualTo", "GreaterThan", "GreaterThanEqualTo"];
        $scope.dateValueEnum = ["First_Quarter", "Second_Quarter", "Third_Quarter", "Fourth_Quarter", "Start_of_Year", "Start_of_Month", "Start_of_Week", "Today", "End_of_Week", "End_of_Month", "End_of_Year", "Absolute_Date"];
        $scope.OperatorSwitch = [{ Name: '+', Value: 'Plus' }, { Name: '-', Value: 'Minus' }, { Name: '*', Value: 'Multiply' }, { Name: '/', Value: 'Divide' }, { Name: '%', Value: 'ModuloDivide' }];
        $scope.relationTypeSwitch = [{ Name: 'And', Value: 'And' }, { Name: 'Or', Value: 'Or' }];
        $scope.categorySwitch = [{ Name: 'Collection', Value: 'Liner' }, { Name: 'Recovery', Value: 'WriteOff' }];
        $scope.PayoutSubpolicyTypeSwitch = [{ Name: 'Formula', Value: 'Formula' }, { Name: 'Subpolicy', Value: 'Subpolicy' }];
        $scope.outputTypeSwitch = [{ Name: 'Number', Value: 'Number' }, { Name: 'Boolean', Value: 'Boolean' }];
        $scope.typeSwitch = [{ Name: 'Value', Value: 'Value' }, { Name: 'Table', Value: 'Table' }];


        restApi.customGET("GetProducts").then(function (data) {
            $scope.productsList = data;
        }, function (data) {
            $csnotify.error(data);
        });

        $scope.selectFormula = function (sformula) {
            $scope.formula = sformula;
            if (!angular.isUndefined(sformula.GroupBy)) {
                if (!$csfactory.isNullOrEmptyString(sformula.GroupBy)) {
                    $scope.formula.GroupBy = JSON.parse(sformula.GroupBy);
                }

            }

            restApi.customGET("GetBConditions", { parentId: sformula.Id }).then(function (data) {
                $scope.AllBConditions = data;
                $scope.formula.BConditions = _.filter(data, { ConditionType: 'Condition' });

                $scope.formula.BOutputs = _.filter(data, { ConditionType: 'Output' });

                if ($scope.formula.BOutputs.length > 0) {
                    $scope.formula.BOutputs[0].Lsqlfunction = '';
                    $scope.formula.BOutputs[0].Operator = '';
                }

                $scope.changeProductCategory();
            }, function (data) {
                $csnotify.error(data);
            });
        };

        $scope.changeProductCategory = function () {
            if (angular.isUndefined($scope.formula.Id)) {
                $scope.formula.BConditions = [];
                $scope.formula.BOutputs = [];
            }
            $scope.resetCondition();
            $scope.resetOutput();
            var formula = $scope.formula;
            if (!angular.isUndefined(formula.Products) && !angular.isUndefined(formula.Category)) {

                // get formulas
                restApi.customGET("GetFormulas", { product: formula.Products, category: formula.Category }).then(function (data) {
                    $scope.formulaList = _.filter(data, { PayoutSubpolicyType: 'Formula' });
                }, function (data) {
                    $csnotify.error(data);
                });


                //get column names
                restApi.customGET("GetColumns", { product: formula.Products, category: formula.Category }).then(function (data) {
                    $scope.columnDefs = data;
                    $scope.columnNames = data;
                    $scope.outColumnNames = _.filter($scope.columnDefs, { InputType: 'number' });
                }, function (data) {
                    $csnotify.error(data);
                });

            } else {
                $scope.columnNames = [];
                $scope.formulaNames = [];
                $scope.matrixNames = [];
            }
        };


        $scope.changeOutputType = function () {
            $scope.formula.BConditions = [];
            $scope.formula.BOutputs = [];
            $scope.resetCondition();
            $scope.resetOutput();
        };

        var getColumnValues = function (columnName) {
            restApi.customGET('GetValuesofColumn', { columnName: columnName }).then(function (data) {
                $scope.conditionValues = data;
            }, function (data) {
                $csnotify.error(data);
            });
        };

        $scope.changeLeftTypeName = function (condition) {
            condition.RtypeName = '';
            $scope.selectedLeftColumn = _.find($scope.columnDefs, { field: condition.LtypeName });

            $scope.RcolumnNames = _.filter($scope.columnDefs, { InputType: $scope.selectedLeftColumn.InputType });

            var inputType = $scope.selectedLeftColumn.InputType;
            if (inputType === "text") {
                $scope.conditionOperators = ["EqualTo", "NotEqualTo", "Contains", "StartsWith", "EndsWith"];
                condition.Operator = '';
                condition.Rtype = 'Value';
                condition.Rvalue = '';
                getColumnValues(condition.LtypeName);
                return;
            }

            if (inputType === "checkbox") {
                $scope.conditionOperators = ["EqualTo"];
                condition.Operator = "Equal";
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

            $scope.conditionOperators = ["EqualTo", "NotEqualTo", "LessThan", "LessThanEqualTo", "GreaterThan", "GreaterThanEqualTo"];;
            condition.Operator = '';
            condition.Rtype = 'Value';
            condition.Rvalue = '';
        };


        $scope.resetCondition = function () {
            $scope.newCondition = {};
            $scope.newCondition.Rtype = 'Value';
            if ($scope.formula.BConditions.length < 1) {
                $scope.newCondition.RelationType = '';
            } else {
                $scope.newCondition.RelationType = 'And';
            }
        };

        $scope.addNewCondition = function (condition) {


            condition.Ltype = "Column";
            condition.Lsqlfunction = "";
            condition.ConditionType = 'Condition';
            condition.ParentId = $scope.formula.Id;
            condition.Priority = $scope.formula.BConditions.length;
            if (condition.dateValueEnum && condition.dateValueEnum != 'Absolute_Date') {
                condition.Rvalue = condition.dateValueEnum;
            }


            var con = angular.copy(condition);
            $scope.formula.BConditions.push(con);
            $scope.conditionValueType = 'text';

            $scope.resetCondition();
        };

        $scope.deleteCondition = function (condition, index) {
            if (($scope.formula.BConditions.length == 1)) {
                $scope.newCondition.RelationType = '';
            }
            if (condition.Id) {
                condition.ParentId = '';
                $scope.deleteConditions.push(condition);
            }

            $scope.formula.BConditions.splice(index, 1);
            $scope.formula.BConditions[0].RelationType = "";

            for (var i = index; i < $scope.formula.BConditions.length; i++) {
                $scope.formula.BConditions[i].Priority = i;
            }
        };

        $scope.resetOutput = function () {
            $scope.newOutput = {};
            $scope.newOutput.Rtype = 'Value';
            if ($scope.formula.BOutputs.length < 1) {
                $scope.newOutput.Operator = 'None';
            } else {
                $scope.newOutput.Operator = 'Plus';
            }
        };

        $scope.addNewOutput = function (output) {
            output.ConditionType = 'Output';
            output.ParentId = $scope.formula.Id;
            output.Priority = $scope.formula.BOutputs.length;
            var out = angular.copy(output);
            $scope.formula.BOutputs.push(out);

            $scope.resetOutput();
        };

        $scope.deleteOutput = function (output, index) {
            if (($scope.formula.BOutputs.length == 1)) {
                $scope.newOutput.Operator = '';
            }

            if (output.Id) {
                output.ParentId = '';
                $scope.deleteConditions.push(output);
            }

            $scope.formula.BOutputs.splice(index, 1);
            $scope.formula.BOutputs[0].Operator = "";

            for (var i = index; i < $scope.formula.BOutputs.length; i++) {
                $scope.formula.BOutputs[i].Priority = i;
            }
        };

        $scope.$watch("formula.BOutputs.length", function () {
            var outResult = _.find($scope.formula.BOutputs, function (output) {
                return (output.Lsqlfunction && output.Lsqlfunction != "");
            });

            $scope.outputWithFunction = (outResult) ? true : false;
        });

        $scope.resetFormula = function (product) {
            $scope.formula = {};

            $scope.formula.BConditions = [];
            $scope.formula.BOutputs = [];
            $scope.deleteConditions = [];
            $scope.newCondition = {};
            $scope.newOutput = {};
            $scope.formula.Products = product;
            $scope.formula.Category = "Liner";
            $scope.formula.PayoutSubpolicyType = 'Formula';
            $scope.formula.OutputType = 'Number';

            $scope.resetCondition();
            $scope.resetOutput();
        };

        $scope.chnageDataFormat = function (date) {

        };

        $scope.saveFormula = function (formula) {
            var operator = $scope.newOutput.Operator;
            formula.GroupBy = JSON.stringify(formula.GroupBy);

            var saveBConditions = [];
            //_.forEach(formula.BConditions, function (con) {
            //    saveBConditions.push(con);
            //});

            //_.forEach(formula.BOutputs, function (out) {
            //    saveBConditions.push(out);
            //});

            //_.forEach($scope.deleteConditions, function (dcond) {
            //    saveBConditions.push(dcond);
            //});

            _.forEach(formula.BOutputs, function (out) {
                out.Operator = operatorsEnum[out.Operator];
                formula.BConditions.push(out);
            });

            //var savedata = {
            //    payoutSubpolicy: formula,
            //    conditions: saveBConditions
            //};

            if (formula.Id) {

                restApi.customPUT(formula, "Put", { id: formula.Id }).then(function (data) {
                    $scope.formulaList = _.reject($scope.formulaList, function (form) { return form.Id == data.Id; });
                    $scope.formulaList.push(data);
                    $scope.formula = data;
                    $scope.resetFormula(data.Products);
                    $csnotify.success("Formula saved");
                }, function (data) {
                    $csnotify.error(data);
                });

            } else {
                restApi.customPOST(formula, "Post").then(function (data) {
                    $scope.formulaList = _.reject($scope.formulaList, function (form) { return form.Id == data.Id; });
                    $scope.formulaList.push(data);
                    $scope.formula = data;
                    $scope.resetFormula(data.Products);
                    $csnotify.success("Payout Subpolicy saved");
                }, function (data) {
                    $csnotify.error(data);
                });
            }
        };

        var operatorsEnum = {
            '>': 'GreaterThan',
            '<': 'LessThan',
            '>=': 'GreaterThanEqualTo',
            '<=': 'LessThanEqualTo',
            '=': 'EqualTo',
            '+': 'Plus',
            '-': 'Minus',
            '*': 'Multiply',
            '/': 'Divide'
        };

        var operatorsEnumReverse = {
            'GreaterThan': '>',
            'LessThan': '<',
            'GreaterThanEqualTo': '>=',
            'LessThanEqualTo': '<=',
            'EqualTo': '=',
            'Plus': '+',
            'Minus': '-',
            'Multiply': '*',
            'Divide': '/'
        };

        $scope.checkString = function (inputString) {
            if (inputString === 'None') {
                return '';
            }
            return inputString;
        };

        $scope.convertOperatorToReverse = function (operator) {
            if (operator === undefined || operator === '')
                return operator;
            return operatorsEnumReverse[operator];
        };

    }
]);

csapp.factory('formulaDataLayer', ['Restangular', '$csnotify',
    function(rest, $csnotify) {
        var dldata = {};

        return {
            dldata:dldata
        };
    }]);

csapp.factory('formulaFactory', ['formulaDataLayer', function (datalayer) { }]);

csapp.controller('formulaController', ['$scope', 'formulaDataLayer', 'formulaFactory',
    function ($scope, datalayer, factory) {
        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
        })();
    }]);