
(csapp.controller("payoutSubpolicyCtrl", ["$scope", "$csnotify", "$csfactory", "Restangular", "$Validations", function ($scope, $csnotify, $csfactory, rest, $validation) {
    "use strict";

    
    
    $scope.val = $validation;
    $scope.payoutSubpolicyList = [];
    $scope.productsList = [];
    $scope.columnNames = [];
    $scope.formulaNames = [];
    $scope.matrixNames = [];
    $scope.AllBConditions = [];
    $scope.payoutSubpolicy = {};
    $scope.payoutSubpolicy.BConditions = [];
    $scope.payoutSubpolicy.BOutputs = [];
    $scope.columnDefs = [];
    $scope.condLcolumnNames = [];
    $scope.condRcolumnNames = [];
    $scope.outColumnNames = [];
    $scope.deleteConditions = [];
    $scope.newCondition = {};
    $scope.newOutput = {};
    $scope.payoutSubpolicy.Category = "Liner";
    $scope.payoutSubpolicy.PayoutSubpolicyType = 'Subpolicy';
    $scope.newCondition.Rtype = "Value";
    $scope.outputWithFunction = false;
    $scope.openDateModel = false;
    $scope.modalData = {};
    $scope.isDuplicateName = false;
    $scope.policyapproved = false;
    $scope.conditionOperators = ["EqualTo", "NotEqualTo", "LessThan", "LessThanEqualTo", "GreaterThan", "GreaterThanEqualTo"];
    $scope.dateValueEnum = ["First_Quarter", "Second_Quarter", "Third_Quarter", "Fourth_Quarter", "Start_of_Year", "Start_of_Month", "Start_of_Week", "Today", "End_of_Week", "End_of_Month", "End_of_Year", "Absolute_Date"];
    $scope.OperatorSwitch = [{ Name: '+', Value: 'Plus' }, { Name: '-', Value: 'Minus' }, { Name: '*', Value: 'Multiply' }, { Name: '/', Value: 'Divide' }, { Name: '%', Value: 'ModuloDivide' }];
    $scope.relationTypeSwitch = [{ Name: 'And', Value: 'And' }, { Name: 'Or', Value: 'Or' }];
    $scope.categorySwitch = [{ Name: 'Collection', Value: 'Liner' }, { Name: 'Recovery', Value: 'WriteOff' }];
    $scope.PayoutSubpolicyTypeSwitch = [{ Name: 'Formula', Value: 'Formula' }, { Name: 'Subpolicy', Value: 'Subpolicy' }];
    $scope.outputTypeSwitch = [{ Name: 'Number', Value: 'Number' }, { Name: 'Boolean', Value: 'Boolean' }];

    $scope.typeSwitch = [{ Name: 'Table', Value: 'Table' }, { Name: 'Formula', Value: 'Formula' }, { Name: 'Matrix', Value: 'Matrix' }, { Name: 'Value', Value: 'Value' }];


    restApi.customGET("GetProducts").then(function (data) {
        $scope.productsList = data;
    }, function (data) {
        $csnotify.error(data);
    });



    $scope.selectPayoutSubpolicy = function (spayoutSubpolicy) {
        var subpolicy = angular.copy(spayoutSubpolicy);
        $scope.payoutSubpolicy = spayoutSubpolicy;
        if (!angular.isUndefined(spayoutSubpolicy.GroupBy)) {
            if (!$csfactory.isNullOrEmptyString(spayoutSubpolicy.GroupBy))
                $scope.payoutSubpolicy.GroupBy = JSON.parse(spayoutSubpolicy.GroupBy);
        }

        restApi.customGET("GetBConditions", { parentId: spayoutSubpolicy.Id }).then(function (data) {

            $scope.AllBConditions = data;

            $scope.payoutSubpolicy.BConditions = _.filter(data, { ConditionType: 'Condition' });

            $scope.payoutSubpolicy.BOutputs = _.filter(data, { ConditionType: 'Output' });

            if ($scope.payoutSubpolicy.BOutputs.length > 0) {
                $scope.payoutSubpolicy.BOutputs[0].Lsqlfunction = '';
                $scope.payoutSubpolicy.BOutputs[0].Operator = '';
            }

            $scope.changeProductCategory();
        }, function (data) {
            $csnotify.error(data);
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
            $csnotify.success("Policy is already Approved");
        } else {
            $scope.policyapproved = false;
        }
    };

    $scope.disableIfRelationExists = function () {
        if (angular.isDefined($scope.curRelation)) {
            if ($csfactory.isNullOrEmptyString($scope.curRelation.BillingPolicy) || $csfactory.isNullOrEmptyString($scope.curRelation.BillingSubpolicy))
                return true;
            else return false;
        }
    };

    $scope.checkDuplicateName = function () {
        $scope.isDuplicateName = false;
        _.forEach($scope.payoutSubpolicyList, function (subpolicy) {
            if (subpolicy.Name.toUpperCase() == $scope.payoutSubpolicy.Name.toUpperCase()) {
                $scope.isDuplicateName = true;
                return;
            }
        });
    };

    $scope.changeProductCategory = function () {

        if (angular.isUndefined($scope.payoutSubpolicy.Id)) {
            $scope.payoutSubpolicy.BConditions = [];
            $scope.payoutSubpolicy.BOutputs = [];
        }
        $scope.resetCondition();
        $scope.resetOutput();

        var payoutSubpolicy = $scope.payoutSubpolicy;
        if (!angular.isUndefined(payoutSubpolicy.Products) && !angular.isUndefined(payoutSubpolicy.Category)) {

            ////get column names
            //restApi.customGET("GetColumnNames", { product: payoutSubpolicy.Products, category: payoutSubpolicy.Category }).then(function (data) {
            //    $scope.columnNames = data;
            //}, function (data) {
            //    $csnotify.error(data);
            //});

            // get subpolicy
            restApi.customGET("GetPayoutSubpolicy", { product: payoutSubpolicy.Products, category: payoutSubpolicy.Category }).then(function (data) {
                $scope.payoutSubpolicyList = _.filter(data, { PayoutSubpolicyType: 'Subpolicy' });
            }, function (data) {
                $csnotify.error(data);
            });

            //get column names 
            debugger;
            restApi.customGET("GetColumns", { product: payoutSubpolicy.Products, category: payoutSubpolicy.Category }).then(function (data) {
                $scope.columnDefs = data;
                $scope.condLcolumnNames = data;
                $scope.outColumnNames = _.filter($scope.columnDefs, { InputType: 'number' });
            }, function (data) {
                $csnotify.error(data);
            });

            // get formula names
            restApi.customGET("GetFormulaNames", { product: payoutSubpolicy.Products, category: payoutSubpolicy.Category }).then(function (data) {
                $scope.formulaNames = data;
            }, function (data) {
                $csnotify.error(data);
            });

            // get formula names
            restApi.customGET("GetMatrixNames", { product: payoutSubpolicy.Products, category: payoutSubpolicy.Category }).then(function (data) {
                $scope.matrixNames = data;
            }, function (data) {
                $csnotify.error(data);
            });

        } else {
            $scope.LcolumnNames = [];
            $scope.formulaNames = [];
            $scope.matrixNames = [];
        }
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

        $scope.condRcolumnNames = _.filter($scope.columnDefs, { InputType: $scope.selectedLeftColumn.InputType });

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

    //#region "Condition operations"
    $scope.resetCondition = function () {
        $scope.newCondition = {};
        if ($scope.payoutSubpolicy.BConditions.length < 1) {
            $scope.newCondition.RelationType = '';
        } else {
            $scope.newCondition.RelationType = 'And';
        }
    };

    $scope.addNewCondition = function (condition) {
        condition.Ltype = "Column";
        condition.Lsqlfunction = "";
        condition.ConditionType = 'Condition';
        condition.ParentId = $scope.payoutSubpolicy.Id;
        condition.Priority = $scope.payoutSubpolicy.BConditions.length;

        if (condition.dateValueEnum && condition.dateValueEnum != 'Absolute_Date') {
            condition.Rvalue = condition.dateValueEnum;
        }

        var con = angular.copy(condition);
        $scope.payoutSubpolicy.BConditions.push(con);
        $scope.conditionValueType = 'text';

        $scope.resetCondition();
    };

    $scope.deleteCondition = function (condition, index) {
        if (($scope.payoutSubpolicy.BConditions.length == 1)) {
            $scope.newCondition.RelationType = '';
        }
        if (condition.Id) {
            condition.ParentId = '';
            $scope.deleteConditions.push(condition);
        }

        $scope.payoutSubpolicy.BConditions.splice(index, 1);
        if ($scope.payoutSubpolicy.BConditions.length > 0) {
            $scope.payoutSubpolicy.BConditions[0].RelationType = "";
        }


        for (var i = index; i < $scope.payoutSubpolicy.BConditions.length; i++) {
            $scope.payoutSubpolicy.BConditions[i].Priority = i;
        }
    };

    //#endregion
    
    //#region "Output Operations"
    $scope.resetOutput = function () {
        $scope.newOutput = {};
        if ($scope.payoutSubpolicy.BOutputs.length < 1) {
            $scope.newOutput.Operator = '';
        } else {
            $scope.newOutput.Operator = 'Plus';
        }
    };

    $scope.addNewOutput = function (output) {
        output.ConditionType = 'Output';
        output.ParentId = $scope.payoutSubpolicy.Id;
        output.Priority = $scope.payoutSubpolicy.BOutputs.length;
        var out = angular.copy(output);
        $scope.payoutSubpolicy.BOutputs.push(out);

        $scope.resetOutput();
    };

    $scope.deleteOutput = function (output, index) {
        if ($scope.payoutSubpolicy.BOutputs.length == 1) {
            $scope.newOutput.Operator = '';
        }
        if (output.Id) {
            output.ParentId = '';
            $scope.deleteConditions.push(output);
        }

        $scope.payoutSubpolicy.BOutputs.splice(index, 1);
        $scope.payoutSubpolicy.BOutputs[0].Operator = "";

        for (var i = index; i < $scope.payoutSubpolicy.BOutputs.length; i++) {
            $scope.payoutSubpolicy.BOutputs[i].Priority = i;
        }
    };
  
    $scope.$watch("payoutSubpolicy.BOutputs.length", function () {
        var outResult = _.find($scope.payoutSubpolicy.BOutputs, function (output) {
            return (output.Lsqlfunction != "");
        });

        $scope.outputWithFunction = (outResult) ? true : false;
    });
    //#endregion

    //#region "Reset BillingSubPolicy"
    $scope.resetPayoutSubpolicy = function () {
        $scope.payoutSubpolicy = {};
        $scope.payoutSubpolicy.BConditions = [];
        $scope.payoutSubpolicy.BOutputs = [];
        $scope.deleteConditions = [];
        $scope.newCondition = {};
        $scope.newOutput = {};
        $scope.payoutSubpolicy.Category = "Liner";
        $scope.payoutSubpolicy.PayoutSubpolicyType = 'Subpolicy';
        $scope.resetCondition();
        $scope.resetOutput();
    };
    //#endregion

    //#region "Save Billing SubPolicy"
    $scope.savePayoutSubpolicy = function (payoutSubpolicy) {
        payoutSubpolicy.GroupBy = JSON.stringify(payoutSubpolicy.GroupBy);

        _.forEach(payoutSubpolicy.BOutputs, function (out) {
            payoutSubpolicy.BConditions.push(out);
        });
        if (payoutSubpolicy.Id) {

            restApi.customPUT(payoutSubpolicy, "Put", { id: payoutSubpolicy.Id }).then(function (data) {
                $scope.payoutSubpolicyList = _.reject($scope.payoutSubpolicyList, function (subpolicy) { return subpolicy.Id == data.Id; });
                $scope.payoutSubpolicyList.push(data);
                $scope.selectPayoutSubpolicy(data);
               // $scope.resetPayoutSubpolicy();
                $csnotify.success("Payout Subpolicy saved");
            }, function (data) {
                $csnotify.error(data);
            });

        } else {
            restApi.customPOST(payoutSubpolicy, "Post").then(function (data) {
                $scope.payoutSubpolicyList = _.reject($scope.payoutSubpolicyList, function (subpolicy) { return subpolicy.Id == data.Id; });
                $scope.payoutSubpolicyList.push(data);
                $scope.selectPayoutSubpolicy(data);
                //$scope.payoutSubpolicy = data;
               // $scope.resetPayoutSubpolicy();
                $csnotify.success("Payout Subpolicy saved");
            }, function (data) {
                $csnotify.error(data);
            });
        }
    };

    //#endregion
    
    //#region "Modal PopUp Actions"
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
            data.BillingPolicy = null;
            data.BillingSubpolicy = null;
            $scope.curRelation = data;
          //  $scope.resetPayoutSubpolicy();
            $csnotify.success("Policy Activated");
        });
    };
    $scope.showStartEndModalPopup = function () {
        $scope.modalData.forActivate = true;
        $scope.modalData.startDate = null;
        $scope.modalData.endDate = null;
        $scope.openDateModel = true;
    };
    //#endregion

}]));


csapp.controller('payoutSubpolicyCtrl', ['$scope', 'payoutSubpolicyDataLayer', 'payoutSubpolicyFactory', '$modal',
    function ($scope, datalayer, factory,$modal) {
        (function () {
            $scope.factory = factory;
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
        })();
}]);

csapp.factory('payoutSubpolicyDataLayer', ['Restangular', '$csnotify',
    function (rest, csnotify) {
        var restApi = rest.all("PayoutSubpolicyApi");
        var dldata = {};

        return {
            dldata:dldata
        };
    }]);

csapp.factory('payoutSubpolicyFactory', ['payoutSubpolicyDataLayer', '$csfactory',
    function (datalayer, $csfactory) {
    
}]);

//#region RowData

//var saveBConditions = [];
//_.forEach(payoutSubpolicy.BConditions, function(con) {
//    saveBConditions.push(con);
//});

//_.forEach(payoutSubpolicy.BOutputs, function(out) {
//    saveBConditions.push(out);
//});

//_.forEach($scope.deleteConditions, function(dcond) {
//    saveBConditions.push(dcond);
//});

//var savedata = {
//    payoutSubpolicy: payoutSubpolicy,
//    conditions: saveBConditions
//};


//$scope.payoutSubpolicyList = _.reject($scope.payoutSubpolicyList, function (subpolicy) { return subpolicy.Id == data.Id; });
//$scope.payoutSubpolicyList.push(data);
//$scope.payoutSubpolicy = data;
//#endregion