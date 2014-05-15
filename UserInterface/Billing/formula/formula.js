
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
            dldata.selectedFormula = sformula;
            if (!angular.isUndefined(sformula.GroupBy)) {
                if (!$csfactory.isNullOrEmptyString(sformula.GroupBy)) {
                    dldata.formula.GroupBy = JSON.parse(sformula.GroupBy);
                }
            }

            restApi.customGET("GetBConditions", { parentId: sformula.Id }).then(function (data) {
                dldata.AllBConditions = data;
                dldata.formula.BConditions = _.filter(data, { ConditionType: 'Condition' });

                dldata.formula.BOutputs = _.filter(data, { ConditionType: 'Output' });

                var bOutputsIfelse = _.filter(data, { ConditionType: 'OutputIf' });
                _.forEach(bOutputsIfelse, function (ifelseOut) {
                    dldata.formula.BOutputs.push(ifelseOut);
                });

                dldata.formula.BOutputs2 = _.filter(data, { ConditionType: 'OutputElse' });

                if (dldata.formula.BOutputs.length > 0) {
                    _.forEach(dldata.formula.BOutputs, function(bOut) {
                        if (bOut.Lsqlfunction === 'None') {
                            bOut.Lsqlfunction = '';
                        }
                        if (bOut.Operator === 'None') {
                            bOut.Operator = '';
                        }
                    });
                    //dldata.formula.BOutputs[0].Lsqlfunction = '';
                    //dldata.formula.BOutputs[0].Operator = '';
                }

                if (dldata.formula.BOutputs2.length > 0) {
                    _.forEach(dldata.formula.BOutputs2, function (bOut) {
                        if (bOut.Lsqlfunction === 'None') {
                            bOut.Lsqlfunction = '';
                        }
                        if (bOut.Operator === 'None') {
                            bOut.Operator = '';
                        }
                    });
                    //dldata.formula.BOutputs2[0].Lsqlfunction = '';
                    //dldata.formula.BOutputs2[0].Operator = '';
                }
                _.forEach(dldata.formula.BOutputs, function (output) {
                    checkString(output);
                });

                _.forEach(dldata.formula.BOutputs2, function (output) {
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
                dldata.formula.BOutputs2 = [];
            }
            resetCondition();
            resetOutput();
            resetOutput2();

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
        var resetOutput2 = function () {
            dldata.newOutput2 = {};
            dldata.newOutput2.Rtype = 'Value';
            if (dldata.formula.BOutputs2.length < 1) {
                dldata.newOutput2.Operator = 'None';
            } else {
                dldata.newOutput2.Operator = 'Plus';
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
            dldata.formula.BOutputs2 = [];
            dldata.deleteConditions = [];
            dldata.newCondition = {};
            dldata.newOutput = {};
            dldata.newOutput2 = {};
            dldata.selectedFormula = {};
            dldata.formula.Products = product;
            dldata.formula.Category = "Liner";
            dldata.formula.PayoutSubpolicyType = 'Formula';
            dldata.formula.OutputType = 'Number';

            resetCondition();
            resetOutput();
            resetOutput2();
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

            _.forEach(formula.BOutputs2, function (out) {
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
            resetOutput2: resetOutput2,
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
        dldata.outputTypeSwitch = [{ Name: 'Number', Value: 'Number' }, { Name: 'Boolean', Value: 'Boolean' }, { Name: 'IfElse', Value: 'IfElse' }];
        dldata.typeSwitch = [{ Name: 'Value', Value: 'Value' }, { Name: 'Table', Value: 'Table' }, { Name: 'Formula', Value: 'Formula' }];
    };


    var changeOutputType = function () {
        if (angular.isDefined(dldata.formula)) {
            dldata.formula.BConditions = [];
            dldata.formula.BOutputs = [];
            dldata.formula.BOutputs2 = [];
            datalayer.resetCondition();
            datalayer.resetOutput();
            datalayer.resetOutput2();
        }

    };
    var addNewCondition = function (condition, newConditionForm) {
        //condition.Ltype = "Column";
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
        newConditionForm.$setPristine();
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

    var addNewOutput = function (output, newOutputForm) {
        datalayer.checkString(output);
        output.ConditionType = 'Output';
        output.ParentId = dldata.formula.Id;
        output.Priority = dldata.formula.BOutputs.length;

        if (dldata.formula.OutputType === 'IfElse') {
            output.ConditionType = 'OutputIf';
        }
        var out = angular.copy(output);
        dldata.formula.BOutputs.push(out);

        datalayer.resetOutput();
        newOutputForm.$setPristine();
    };

    var addNewOutput2 = function (output, newOutputForm2) {
        datalayer.checkString(output);
        output.ConditionType = 'OutputElse';
        output.ParentId = dldata.formula.Id;
        output.Priority = dldata.formula.BOutputs2.length;
        var out = angular.copy(output);
        dldata.formula.BOutputs2.push(out);

        datalayer.resetOutput2();
        newOutputForm2.$setPristine();
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

    var deleteOutput2 = function (output, index) {
        if ((dldata.formula.BOutputs2.length == 1)) {
            dldata.newOutput2.Operator = '';
        }

        if (output.Id) {
            output.ParentId = '';
            dldata.deleteConditions.push(output);
        }

        dldata.formula.BOutputs2.splice(index, 1);
        dldata.formula.BOutputs2[0].Operator = "";

        for (var i = index; i < dldata.formula.BOutputs2.length; i++) {
            dldata.formula.BOutputs2[i].Priority = i;
        }
    };

    return {
        initEnums: initEnums,
        changeOutputType: changeOutputType,
        addNewCondition: addNewCondition,
        deleteCondition: deleteCondition,
        addNewOutput: addNewOutput,
        addNewOutput2: addNewOutput2,
        deleteOutput: deleteOutput,
        deleteOutput2: deleteOutput2
    };
}]);

csapp.controller('formulaController', ['$scope', 'formulaDataLayer', 'formulaFactory', '$csfactory', '$csModels', '$csShared',
    function ($scope, datalayer, factory, $csfactory, $csModels, $csShared) {
        (function () {

            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            $scope.factory.initEnums();
            $scope.formula = $csModels.getColumns("Formula");
            $scope.CustBillViewModel = $csModels.getColumns("CustomerInfo");
            $scope.GPincode = $csModels.getColumns("Pincode");
            $scope.dldata.formula = {};
            $scope.dldata.formula.Category = 'Liner';
            $scope.showDiv = false;
            $scope.fieldname = {};
            $scope.showField = true;
            $scope.showField2 = false;
            $scope.datalayer.getProducts();
            $scope.listofFormula = [];

            $scope.$watch("dldata.formula.BOutputs.length", function () {
                if (angular.isUndefined($scope.dldata.formula)) {
                    return;
                }
                var outResult = _.find($scope.dldata.formula.BOutputs, function (output) {
                    return (output.Lsqlfunction && output.Lsqlfunction != "");
                });

                $scope.dldata.outputWithFunction = (outResult) ? true : false;
            });

            $scope.$watch("dldata.formula.BOutputs2.length", function () {
                if (angular.isUndefined($scope.dldata.formula)) {
                    return;
                }
                var outResult = _.find($scope.dldata.formula.BOutputs2, function (output) {
                    return (output.Lsqlfunction && output.Lsqlfunction != "");
                });

                $scope.dldata.outputWithFunction = (outResult) ? true : false;
            });
        })();

        $scope.addformula = function (product) {
            $scope.showDiv = true;
            $scope.datalayer.resetFormula(product);
            $scope.listofFormula = [];
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

        $scope.change = function (condition) {
            var con = condition.toString();
            var field = con.split(".");
            if (field[0] === "CustBillViewModel") {
                field[0] = "Customer";
                var fieldName = (field[0] + "." + field[1]);
                return fieldName;
            } else {
                return condition;
            }

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
                //datalayer.getColumnValues(condition.LtypeName).then(function (data) {
                //    $scope.fieldname.valueList = data;
                //});
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

        $scope.setFormulaType = function (condition) {
            $scope.showField = $scope.showField === true ? false : true;
            $scope.showField2 = !$scope.showField2;
            condition.Operator = '';
            condition.Rtype = 'Value';
            condition.Rvalue = '';
        };

        $scope.setFormulaList = function (rtype) {
            if (rtype !== 'Formula') {
                return;
            }
            $scope.listofFormula = angular.copy($scope.dldata.formulaList);
            if (angular.isDefined($scope.dldata.selectedFormula)) {
                var index = $csfactory.findIndex($scope.dldata.formulaList, "Id", $scope.dldata.selectedFormula.Id);
                if (index !== -1)
                    $scope.listofFormula.splice(index, 1);
            }

        };

        $scope.getFormulaName = function (id) {
            if ($csfactory.isNullOrEmptyGuid(id)) {
                return '';
            }
            
            var name = _.find($scope.listofFormula, { 'Id': id });
            return name.Name;
        };

    }]);

//csapp.controller('formulaDetailsCtrl', ['$scope', 'formulaController', 'formulaDataLayer',
//    function ($scope, formulaCtrl, datalayer) {

//    }]);

//csapp.controller('formulaConditionCtrl', ['$scope', 'formulaController', 'formulaDataLayer',
//    function ($scope, formulaCtrl, datalayer) {

//    }]);

//csapp.controller('formulaOutputCtrl', ['$scope', 'formulaController', 'formulaDataLayer',
//    function ($scope, formulaCtrl, datalayer) {

//    }]);

//csapp.controller('formulaDetailsCtrl', ['$scope', 'formulaController', 'formulaDataLayer',
//    function ($scope, formulaCtrl, datalayer) {

//    }]);