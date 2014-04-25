
csapp.factory('formulaDataLayer', ['Restangular', '$csnotify', '$csfactory',
    function (rest, $csnotify, $csfactory) {

        var dldata = {};
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
                _.forEach(dldata.formula.BOutputs, function (output) {
                    checkString(output);
                });
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
            return restApi.customGET('GetValuesofColumn', { columnName: columnName }).then(function (data) {
                return data;
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
        var checkString = function (output) {
            if (output.Operator === 'None') {
                output.Operator = "";
            }
            if (output.Lsqlfunction === 'None') {
                output.Lsqlfunction = "";
            }

        };

        var saveFormula = function (formula) {

            formula.GroupBy = JSON.stringify(formula.GroupBy);

            _.forEach(formula.BOutputs, function (out) {
                formula.BConditions.push(out);
            });

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
            changeProductCategory: changeProductCategory,
            checkString: checkString
        };
    }]);

csapp.factory('formulaFactory', ['formulaDataLayer', function (datalayer) {
    var dldata = datalayer.dldata;

    var initEnums = function () {
        dldata.categorySwitch = [{ Name: 'Collection', Value: 'Liner' }, { Name: 'Recovery', Value: 'WriteOff' }];
        dldata.PayoutSubpolicyTypeSwitch = [{ Name: 'Formula', Value: 'Formula' }, { Name: 'Subpolicy', Value: 'Subpolicy' }];
        dldata.outputTypeSwitch = [{ Name: 'Number', Value: 'Number' }, { Name: 'Boolean', Value: 'Boolean' }];
        dldata.typeSwitch = [{ Name: 'Value', Value: 'Value' }, { Name: 'Table', Value: 'Table' }];
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
        condition.Rvalue = JSON.stringify(condition.Rvalue);
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
        datalayer.checkString(output);
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

    return {
        initEnums: initEnums,
        changeOutputType: changeOutputType,
        addNewCondition: addNewCondition,
        deleteCondition: deleteCondition,
        addNewOutput: addNewOutput,
        deleteOutput: deleteOutput
    };
}]);

csapp.controller('formulaController', ['$scope', 'formulaDataLayer', 'formulaFactory', '$csfactory', '$csBillingModels', '$csShared', '$csFileUploadModels', '$csGenericModels',
    function ($scope, datalayer, factory, $csfactory, $csBillingModels, $csShared, $csFileUploadModels, $csGenericModels) {
        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            $scope.factory.initEnums();
            $scope.formula = $csBillingModels.models.Formula;
            $scope.CustBillViewModel = $csFileUploadModels.models.CustomerInfo;
            $scope.GPincode = $csGenericModels.models.Pincode;
            $scope.dldata.formula = {};
            $scope.dldata.formula.Category = 'Liner';
            $scope.showDiv = false;
            $scope.fieldname = '';
            $scope.showField = true;
            $scope.showField2 = false;
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

        $scope.addformula = function (product) {
            $scope.showDiv = true;
            $scope.datalayer.resetFormula(product);
        };

        $scope.changeProductCategory = function (product) {
            $scope.datalayer.changeProductCategory();
            $scope.addformula(product);
            $scope.showDiv = false;
        };

        $scope.selectFormula = function (formula) {
            $scope.datalayer.selectFormula(formula);
            $scope.showDiv = true;
        };

        $scope.changeLeftTypeName = function (condition) {

            $scope.showField = $scope.showField === true ? false : true;
            $scope.showField2 = !$scope.showField2;
            var fieldVal = condition.LtypeName.split(".");

            $scope.fieldname = $scope[fieldVal[0]][fieldVal[1]];
            condition.RtypeName = '';
            $scope.dldata.selectedLeftColumn = _.find($scope.dldata.columnDefs, { field: condition.LtypeName });

            $scope.dldata.RcolumnNames = _.filter($scope.dldata.columnDefs, { InputType: $scope.dldata.selectedLeftColumn.InputType });

            var inputType = $scope.dldata.selectedLeftColumn.InputType;
            if (inputType === "text") {
                $scope.formula.ConditionOperators.valueList = $csShared.enums.TextConditionOperators;
                condition.Operator = '';
                condition.Rtype = 'Value';
                condition.Rvalue = '';
                datalayer.getColumnValues(condition.LtypeName).then(function (data) {
                    $scope.fieldname.valueList = data;
                });
                return;
            }

            if (inputType === "checkbox") {
                $scope.formula.ConditionOperators.valueList = $csShared.enums.CheckboxConditionOperators;
                condition.Operator = "Equal";
                condition.Rtype = 'Value';
                condition.Rvalue = '';
                return;
            }

            if (inputType === "dropdown") {
                $scope.formula.ConditionOperators.valueList = $csShared.enums.DropdownConditionOperators;
                condition.Rtype = 'Value';
                condition.Rvalue = '';
                return;
            }

            $scope.formula.ConditionOperators.valueList = $csShared.enums.ConditionOperators;
            condition.Operator = '';
            condition.Rtype = 'Value';
            condition.Rvalue = '';
        };

        $scope.manageField = function (condition) {
            condition.Rvalue = '';
            $scope.showField = $scope.showField === true ? false : true;
            $scope.showField2 = !$scope.showField2;
            $scope.dldata.selectedLeftColumn = _.find($scope.dldata.columnDefs, { field: condition.LtypeName });
            var inputType = $scope.dldata.selectedLeftColumn.InputType;
            if (inputType !== 'text') {
                return;
            }
            if (condition.Operator === 'EndsWith' || condition.Operator === 'StartsWith' ||
                condition.Operator === 'Contains' || condition.Operator === 'DoNotContains') {
                $scope.fieldname.type = "text";
                $scope.fieldname.required = true;
                return;
            }
            if (condition.Operator === "IsInList") {
                $scope.fieldname.multiple = "multiple";
            }
            $scope.fieldname.type = "enum";
        };

    }]);

//#region "Row data"
//if (out.Operator !== "") {
//    out.Operator = operatorsEnum[out.Operator];
//}
//var savedata = {
//    payoutSubpolicy: formula,
//    conditions: saveBConditions
//};
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

//#endregion