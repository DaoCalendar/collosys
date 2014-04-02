
csapp.factory('formulaDataLayer', ['Restangular', '$csnotify', '$csfactory',
    function (rest, $csnotify, $csfactory) {
        var dldata = {};
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
        var restApi = rest.all("PayoutSubpolicyApi");

        var getProducts = function () {
            restApi.customGET("GetProducts").then(function (data) {
                dldata.productsList = data;
            }, function (data) {
                $csnotify.error(data);
            });
        };
        var selectFormula = function (sformula) {
            dldata.formula = sformula;
            if (!angular.isUndefined(sformula.GroupBy)) {
                if (!$csfactory.isNullOrEmptyString(sformula.GroupBy)) {
                    dldata.formula.GroupBy = JSON.parse(sformula.GroupBy);
                }
            }

            restApi.customGET("GetBConditions", { parentId: sformula.Id }).then(function (data) {
                dldata.AllBConditions = data;
                dldata.formula.BConditions = _.filter(data, { ConditionType: 'Condition' });

                dldata.formula.BOutputs = _.filter(data, { ConditionType: 'Output' });

                if (dldata.formula.BOutputs.length > 0) {
                    dldata.formula.BOutputs[0].Lsqlfunction = '';
                    dldata.formula.BOutputs[0].Operator = '';
                }

                changeProductCategory();
            }, function (data) {
                $csnotify.error(data);
            });
        };
        var changeProductCategory = function () {
            if (angular.isUndefined(dldata.formula)) {
                dldata.formula = {};
            }
            if (angular.isUndefined(dldata.formula.Id)) {
                dldata.formula.BConditions = [];
                dldata.formula.BOutputs = [];
            }
            resetCondition();
            resetOutput();
            var formula = dldata.formula;
            if (!angular.isUndefined(formula.Products) && !angular.isUndefined(formula.Category)) {

                // get formulas
                restApi.customGET("GetFormulas", { product: formula.Products, category: formula.Category }).then(function (data) {
                    dldata.formulaList = _.filter(data, { PayoutSubpolicyType: 'Formula' });
                }, function (data) {
                    $csnotify.error(data);
                });


                //get column names
                restApi.customGET("GetColumns", { product: formula.Products, category: formula.Category }).then(function (data) {
                    dldata.columnDefs = data;
                    dldata.columnNames = data;
                    dldata.outColumnNames = _.filter(dldata.columnDefs, { InputType: 'number' });
                }, function (data) {
                    $csnotify.error(data);
                });

            } else {
                dldata.columnNames = [];
                dldata.formulaNames = [];
                dldata.matrixNames = [];
            }
        };
        var resetCondition = function () {
            dldata.newCondition = {};
            dldata.newCondition.Rtype = 'Value';
            if (dldata.formula.BConditions.length < 1) {
                dldata.newCondition.RelationType = '';
            } else {
                dldata.newCondition.RelationType = 'And';
            }
        };
        var resetOutput = function () {
            dldata.newOutput = {};
            dldata.newOutput.Rtype = 'Value';
            if (dldata.formula.BOutputs.length < 1) {
                dldata.newOutput.Operator = 'None';
            } else {
                dldata.newOutput.Operator = 'Plus';
            }
        };
        var getColumnValues = function (columnName) {
            restApi.customGET('GetValuesofColumn', { columnName: columnName }).then(function (data) {
                dldata.conditionValues = data;
            }, function (data) {
                $csnotify.error(data);
            });
        };
        var resetFormula = function (product) {
            dldata.formula = {};
            dldata.formula.BConditions = [];
            dldata.formula.BOutputs = [];
            dldata.deleteConditions = [];
            dldata.newCondition = {};
            dldata.newOutput = {};
            dldata.formula.Products = product;
            dldata.formula.Category = "Liner";
            dldata.formula.PayoutSubpolicyType = 'Formula';
            dldata.formula.OutputType = 'Number';

            resetCondition();
            resetOutput();
        };
        var saveFormula = function (formula) {
            // var operator = $scope.newOutput.Operator;
            formula.GroupBy = JSON.stringify(formula.GroupBy);

            // var saveBConditions = [];
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
                if (out.Operator !== "") {
                    out.Operator = operatorsEnum[out.Operator];
                }
                formula.BConditions.push(out);
            });

            //var savedata = {
            //    payoutSubpolicy: formula,
            //    conditions: saveBConditions
            //};

            if (formula.Id) {

                restApi.customPUT(formula, "Put", { id: formula.Id }).then(function (data) {
                    dldata.formulaList = _.reject(dldata.formulaList, function (form) { return form.Id == data.Id; });
                    dldata.formulaList.push(data);
                    dldata.formula = data;
                    resetFormula(data.Products);
                    $csnotify.success("Formula saved");
                }, function (data) {
                    $csnotify.error(data);
                });

            } else {
                restApi.customPOST(formula, "Post").then(function (data) {
                    dldata.formulaList = _.reject(dldata.formulaList, function (form) { return form.Id == data.Id; });
                    dldata.formulaList.push(data);
                    dldata.formula = data;
                    resetFormula(data.Products);
                    $csnotify.success("Payout Subpolicy saved");
                }, function (data) {
                    $csnotify.error(data);
                });
            }
        };
        return {
            dldata: dldata,
            getProducts: getProducts,
            selectFormula: selectFormula,
            getColumnValues: getColumnValues,
            resetCondition: resetCondition,
            resetOutput: resetOutput,
            saveFormula: saveFormula,
            resetFormula: resetFormula,
            changeProductCategory: changeProductCategory
        };
    }]);

csapp.factory('formulaFactory', ['formulaDataLayer', function (datalayer) {
    var dldata = datalayer.dldata;

    var initEnums = function () {
        dldata.conditionOperators = ["EqualTo", "NotEqualTo", "LessThan", "LessThanEqualTo", "GreaterThan", "GreaterThanEqualTo"];
        dldata.dateValueEnum = ["First_Quarter", "Second_Quarter", "Third_Quarter", "Fourth_Quarter", "Start_of_Year", "Start_of_Month", "Start_of_Week", "Today", "End_of_Week", "End_of_Month", "End_of_Year", "Absolute_Date"];
        dldata.OperatorSwitch = [{ Name: '+', Value: 'Plus' }, { Name: '-', Value: 'Minus' }, { Name: '*', Value: 'Multiply' }, { Name: '/', Value: 'Divide' }, { Name: '%', Value: 'ModuloDivide' }];
        dldata.relationTypeSwitch = [{ Name: 'And', Value: 'And' }, { Name: 'Or', Value: 'Or' }];
        dldata.categorySwitch = [{ Name: 'Collection', Value: 'Liner' }, { Name: 'Recovery', Value: 'WriteOff' }];
        dldata.PayoutSubpolicyTypeSwitch = [{ Name: 'Formula', Value: 'Formula' }, { Name: 'Subpolicy', Value: 'Subpolicy' }];
        dldata.outputTypeSwitch = [{ Name: 'Number', Value: 'Number' }, { Name: 'Boolean', Value: 'Boolean' }];
        dldata.typeSwitch = [{ Name: 'Value', Value: 'Value' }, { Name: 'Table', Value: 'Table' }];
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

    var changeLeftTypeName = function (condition) {
        condition.RtypeName = '';
        dldata.selectedLeftColumn = _.find(dldata.columnDefs, { field: condition.LtypeName });

        dldata.RcolumnNames = _.filter(dldata.columnDefs, { InputType: dldata.selectedLeftColumn.InputType });

        var inputType = dldata.selectedLeftColumn.InputType;
        if (inputType === "text") {
            dldata.conditionOperators = ["EqualTo", "NotEqualTo", "Contains", "StartsWith", "EndsWith"];
            condition.Operator = '';
            condition.Rtype = 'Value';
            condition.Rvalue = '';
            datalayer.getColumnValues(condition.LtypeName);
            return;
        }

        if (inputType === "checkbox") {
            dldata.conditionOperators = ["EqualTo"];
            condition.Operator = "Equal";
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

        dldata.conditionOperators = ["EqualTo", "NotEqualTo", "LessThan", "LessThanEqualTo", "GreaterThan", "GreaterThanEqualTo"];;
        condition.Operator = '';
        condition.Rtype = 'Value';
        condition.Rvalue = '';
    };
    var changeOutputType = function () {
        if (angular.isDefined(dldata.formula)) {
            dldata.formula.BConditions = [];
            dldata.formula.BOutputs = [];
            datalayer.resetCondition();
            datalayer.resetOutput();
        }

    };
    var addNewCondition = function (condition) {
        condition.Ltype = "Column";
        condition.Lsqlfunction = "";
        condition.ConditionType = 'Condition';
        condition.ParentId = dldata.formula.Id;
        condition.Priority = dldata.formula.BConditions.length;
        if (condition.dateValueEnum && condition.dateValueEnum != 'Absolute_Date') {
            condition.Rvalue = condition.dateValueEnum;
        }
        var con = angular.copy(condition);
        dldata.formula.BConditions.push(con);
        dldata.conditionValueType = 'text';

        datalayer.resetCondition();
    };
    var deleteCondition = function (condition, index) {
        if ((dldata.formula.BConditions.length == 1)) {
            dldata.newCondition.RelationType = '';
        }
        if (condition.Id) {
            condition.ParentId = '';
            dldata.deleteConditions.push(condition);
        }

        dldata.formula.BConditions.splice(index, 1);
        dldata.formula.BConditions[0].RelationType = "";

        for (var i = index; i < dldata.formula.BConditions.length; i++) {
            dldata.formula.BConditions[i].Priority = i;
        }
    };
    var addNewOutput = function (output) {
        checkString(output);
        output.Operator = convertOperatorToReverse(output.Operator);
        output.ConditionType = 'Output';
        output.ParentId = dldata.formula.Id;
        output.Priority = dldata.formula.BOutputs.length;
        var out = angular.copy(output);
        dldata.formula.BOutputs.push(out);

        datalayer.resetOutput();
    };
    var deleteOutput = function (output, index) {
        if ((dldata.formula.BOutputs.length == 1)) {
            dldata.newOutput.Operator = '';
        }

        if (output.Id) {
            output.ParentId = '';
            dldata.deleteConditions.push(output);
        }

        dldata.formula.BOutputs.splice(index, 1);
        dldata.formula.BOutputs[0].Operator = "";

        for (var i = index; i < dldata.formula.BOutputs.length; i++) {
            dldata.formula.BOutputs[i].Priority = i;
        }
    };
    var checkString = function (output) {
        if (output.Operator === 'None') {
            output.Operator = "";
        }
        if (output.Lsqlfunction === 'None') {
            output.Lsqlfunction = "";
        }

    };

    var convertOperatorToReverse = function (operator) {
        if (operator === undefined || operator === '') {
            return "";
        }

        return operatorsEnumReverse[operator];
    };

    return {
        initEnums: initEnums,
        changeLeftTypeName: changeLeftTypeName,
        changeOutputType: changeOutputType,
        addNewCondition: addNewCondition,
        deleteCondition: deleteCondition,
        addNewOutput: addNewOutput,
        deleteOutput: deleteOutput,
        convertOperatorToReverse: convertOperatorToReverse,
        checkString: checkString
    };
}]);

csapp.controller('formulaController', ['$scope', 'formulaDataLayer', 'formulaFactory', '$csfactory',
    function ($scope, datalayer, factory, $csfactory) {
        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            $scope.factory.initEnums();

            $scope.datalayer.getProducts();
            $scope.$watch("dldata.formula.BOutputs.length", function () {
                if (angular.isUndefined($scope.dldata.formula)) {
                    return;
                }
                var outResult = _.find($scope.dldata.formula.BOutputs, function (output) {
                    return (output.Lsqlfunction && output.Lsqlfunction != "");
                });

                $scope.dldata.outputWithFunction = (outResult) ? true : false;
            });
        })();
    }]);