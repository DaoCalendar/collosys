
csapp.controller("payoutSubpolicyCtrl1", ["$scope", "$csnotify", "$csfactory", "Restangular", "$Validations", function ($scope, $csnotify, $csfactory, rest, $validation) {
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

   

}]);

csapp.factory('payoutSubpolicyDataLayer', ['Restangular', '$csnotify', '$csfactory',
    function (rest, $csnotify, $csfactory) {
        var restApi = rest.all("PayoutSubpolicyApi");
        var dldata = {};

        dldata.conditionOperators = ["EqualTo", "NotEqualTo", "LessThan", "LessThanEqualTo", "GreaterThan", "GreaterThanEqualTo"];
        dldata.dateValueEnum = ["First_Quarter", "Second_Quarter", "Third_Quarter", "Fourth_Quarter", "Start_of_Year", "Start_of_Month", "Start_of_Week", "Today", "End_of_Week", "End_of_Month", "End_of_Year", "Absolute_Date"];
        dldata.OperatorSwitch = [{ Name: '+', Value: 'Plus' }, { Name: '-', Value: 'Minus' }, { Name: '*', Value: 'Multiply' }, { Name: '/', Value: 'Divide' }, { Name: '%', Value: 'ModuloDivide' }];
        dldata.relationTypeSwitch = [{ Name: 'And', Value: 'And' }, { Name: 'Or', Value: 'Or' }];
        dldata.categorySwitch = [{ Name: 'Collection', Value: 'Liner' }, { Name: 'Recovery', Value: 'WriteOff' }];
        dldata.PayoutSubpolicyTypeSwitch = [{ Name: 'Formula', Value: 'Formula' }, { Name: 'Subpolicy', Value: 'Subpolicy' }];
        dldata.outputTypeSwitch = [{ Name: 'Number', Value: 'Number' }, { Name: 'Boolean', Value: 'Boolean' }];
        dldata.typeSwitch = [{ Name: 'Table', Value: 'Table' }, { Name: 'Formula', Value: 'Formula' }, { Name: 'Matrix', Value: 'Matrix' }, { Name: 'Value', Value: 'Value' }];


        var getProducts = function () {
            restApi.customGET("GetProducts").then(function (data) {
                dldata.productsList = data;
            }, function (data) {
                $csnotify.error(data);
            });
        };

        var selectPayoutSubpolicy = function (spayoutSubpolicy) {
            var subpolicy = angular.copy(spayoutSubpolicy);
            dldata.payoutSubpolicy = spayoutSubpolicy;
            if (!angular.isUndefined(spayoutSubpolicy.GroupBy)) {
                if (!$csfactory.isNullOrEmptyString(spayoutSubpolicy.GroupBy))
                    dldata.payoutSubpolicy.GroupBy = JSON.parse(spayoutSubpolicy.GroupBy);
            }

            restApi.customGET("GetBConditions", { parentId: spayoutSubpolicy.Id }).then(function (data) {

                dldata.AllBConditions = data;

                dldata.payoutSubpolicy.BConditions = _.filter(data, { ConditionType: 'Condition' });

                dldata.payoutSubpolicy.BOutputs = _.filter(data, { ConditionType: 'Output' });

                if (dldata.payoutSubpolicy.BOutputs.length > 0) {
                    dldata.payoutSubpolicy.BOutputs[0].Lsqlfunction = '';
                    dldata.payoutSubpolicy.BOutputs[0].Operator = '';
                }

                changeProductCategory();
            }, function (data) {
                $csnotify.error(data);
            });

            restApi.customPOST(subpolicy, "GetRelations").then(function (relation) {
                dldata.curRelation = relation;
                setIsPolicyApproved(dldata.curRelation);
            });
        };

        var changeProductCategory = function () {

            if (angular.isUndefined(dldata.payoutSubpolicy))
                return;

            if (angular.isUndefined(dldata.payoutSubpolicy.Id)) {
                dldata.payoutSubpolicy.BConditions = [];
                dldata.payoutSubpolicy.BOutputs = [];
            }
            resetCondition();
            resetOutput();

            var payoutSubpolicy = dldata.payoutSubpolicy;
            if (!angular.isUndefined(payoutSubpolicy.Products) && !angular.isUndefined(payoutSubpolicy.Category)) {

                // get subpolicy
                restApi.customGET("GetPayoutSubpolicy", { product: payoutSubpolicy.Products, category: payoutSubpolicy.Category }).then(function (data) {
                    dldata.payoutSubpolicyList = _.filter(data, { PayoutSubpolicyType: 'Subpolicy' });
                }, function (data) {
                    $csnotify.error(data);
                });

                //get column names 

                restApi.customGET("GetColumns", { product: payoutSubpolicy.Products, category: payoutSubpolicy.Category }).then(function (data) {
                    dldata.columnDefs = data;
                    dldata.condLcolumnNames = data;
                    dldata.outColumnNames = _.filter(dldata.columnDefs, { InputType: 'number' });
                }, function (data) {
                    $csnotify.error(data);
                });

                // get formula names
                restApi.customGET("GetFormulaNames", { product: payoutSubpolicy.Products, category: payoutSubpolicy.Category }).then(function (data) {
                    dldata.formulaNames = data;
                }, function (data) {
                    $csnotify.error(data);
                });

                // get formula names
                restApi.customGET("GetMatrixNames", { product: payoutSubpolicy.Products, category: payoutSubpolicy.Category }).then(function (data) {
                    dldata.matrixNames = data;
                }, function (data) {
                    $csnotify.error(data);
                });

            } else {
                dldata.LcolumnNames = [];
                dldata.formulaNames = [];
                dldata.matrixNames = [];
            }
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

        var resetCondition = function () {
            dldata.newCondition = {};
            if (dldata.payoutSubpolicy.BConditions.length < 1) {
                dldata.newCondition.RelationType = '';
            } else {
                dldata.newCondition.RelationType = 'And';
            }
        };

        var resetOutput = function () {
            dldata.newOutput = {};
            if (dldata.payoutSubpolicy.BOutputs.length < 1) {
                dldata.newOutput.Operator = '';
            } else {
                dldata.newOutput.Operator = 'Plus';
            }
        };

        var savePayoutSubpolicy = function (payoutSubpolicy) {
            payoutSubpolicy.GroupBy = JSON.stringify(payoutSubpolicy.GroupBy);

            _.forEach(payoutSubpolicy.BOutputs, function (out) {
                payoutSubpolicy.BConditions.push(out);
            });
            if (payoutSubpolicy.Id) {

                restApi.customPUT(payoutSubpolicy, "Put", { id: payoutSubpolicy.Id }).then(function (data) {
                    dldata.payoutSubpolicyList = _.reject(dldata.payoutSubpolicyList, function (subpolicy) { return subpolicy.Id == data.Id; });
                    dldata.payoutSubpolicyList.push(data);
                    dldata.selectPayoutSubpolicy(data);
                    // $scope.resetPayoutSubpolicy();
                    $csnotify.success("Payout Subpolicy saved");
                }, function (data) {
                    $csnotify.error(data);
                });

            } else {
                restApi.customPOST(payoutSubpolicy, "Post").then(function (data) {
                    dldata.payoutSubpolicyList = _.reject(dldata.payoutSubpolicyList, function (subpolicy) { return subpolicy.Id == data.Id; });
                    dldata.payoutSubpolicyList.push(data);
                    datalayer.selectPayoutSubpolicy(data);
                    $csnotify.success("Payout Subpolicy saved");
                }, function (data) {
                    $csnotify.error(data);
                });
            }
        };

        var getColumnValues = function (columnName) {
            restApi.customGET('GetValuesofColumn', { columnName: columnName }).then(function (data) {
                dldata.conditionValues = data;
            }, function (data) {
                $csnotify.error(data);
            });
        };

        var activateSubPoicy = function (modelData) {
            dldata.curRelation.StartDate = modelData.startDate;
            dldata.curRelation.EndDate = modelData.endDate;
           return  restApi.customPOST(dldata.curRelation, "ActivateSubpolicy").then(function (data) {
                data.BillingPolicy = null;
                data.BillingSubpolicy = null;
                dldata.curRelation = data;
                $csnotify.success("Policy Activated");
            });
        };

        return {
            dldata: dldata,
            getProducts: getProducts,
            selectPayoutSubpolicy: selectPayoutSubpolicy,
            changeProductCategory: changeProductCategory,
            setIsPolicyApproved: setIsPolicyApproved,
            resetCondition: resetCondition,
            resetOutput: resetOutput,
            savePayoutSubpolicy: savePayoutSubpolicy,
            getColumnValues: getColumnValues,
            activateSubPoicy: activateSubPoicy
        };
    }]);

csapp.factory('payoutSubpolicyFactory', ['payoutSubpolicyDataLayer', '$csfactory',
    function (datalayer, $csfactory) {

        var dldata = datalayer.dldata;

        var disableIfRelationExists = function () {
            if (angular.isDefined(dldata.curRelation)) {
                if ($csfactory.isNullOrEmptyString(dldata.curRelation.BillingPolicy) || $csfactory.isNullOrEmptyString(dldata.curRelation.BillingSubpolicy))
                    return true;
                else return false;
            }
            return false;
        };

        var checkDuplicateName = function () {
            dldata.isDuplicateName = false;
            _.forEach(dldata.payoutSubpolicyList, function (subpolicy) {
                if (subpolicy.Name.toUpperCase() == dldata.payoutSubpolicy.Name.toUpperCase()) {
                    dldata.isDuplicateName = true;
                    return;
                }
            });
        };

        var addNewCondition = function (condition) {
            condition.Ltype = "Column";
            condition.Lsqlfunction = "";
            condition.ConditionType = 'Condition';
            condition.ParentId = dldata.payoutSubpolicy.Id;
            condition.Priority = dldata.payoutSubpolicy.BConditions.length;

            if (condition.dateValueEnum && condition.dateValueEnum != 'Absolute_Date') {
                condition.Rvalue = condition.dateValueEnum;
            }

            var con = angular.copy(condition);
            dldata.payoutSubpolicy.BConditions.push(con);
            dldata.conditionValueType = 'text';

            datalayer.resetCondition();
        };

        var deleteCondition = function (condition, index) {
            if ((dldata.payoutSubpolicy.BConditions.length == 1)) {
                dldata.newCondition.RelationType = '';
            }
            if (condition.Id) {
                condition.ParentId = '';
                dldata.deleteConditions.push(condition);
            }

            dldata.payoutSubpolicy.BConditions.splice(index, 1);
            if (dldata.payoutSubpolicy.BConditions.length > 0) {
                dldata.payoutSubpolicy.BConditions[0].RelationType = "";
            }


            for (var i = index; i < dldata.payoutSubpolicy.BConditions.length; i++) {
                dldata.payoutSubpolicy.BConditions[i].Priority = i;
            }
        };

        var addNewOutput = function (output) {
            output.ConditionType = 'Output';
            output.ParentId = dldata.payoutSubpolicy.Id;
            output.Priority = dldata.payoutSubpolicy.BOutputs.length;
            var out = angular.copy(output);
            dldata.payoutSubpolicy.BOutputs.push(out);

            datalayer.resetOutput();
        };

        var deleteOutput = function (output, index) {
            if (dldata.payoutSubpolicy.BOutputs.length == 1) {
                dldata.newOutput.Operator = '';
            }
            if (output.Id) {
                output.ParentId = '';
                dldata.deleteConditions.push(output);
            }

            dldata.payoutSubpolicy.BOutputs.splice(index, 1);
            dldata.payoutSubpolicy.BOutputs[0].Operator = "";

            for (var i = index; i < dldata.payoutSubpolicy.BOutputs.length; i++) {
                dldata.payoutSubpolicy.BOutputs[i].Priority = i;
            }
        };

        var resetPayoutSubpolicy = function () {
            dldata.payoutSubpolicy = {};
            dldata.payoutSubpolicy.BConditions = [];
            dldata.payoutSubpolicy.BOutputs = [];
            dldata.deleteConditions = [];
            dldata.newCondition = {};
            dldata.newOutput = {};
            dldata.payoutSubpolicy.Category = "Liner";
            dldata.payoutSubpolicy.PayoutSubpolicyType = 'Subpolicy';
            datalayer.resetCondition();
            datalayer.resetOutput();
        };

        var changeLeftTypeName = function (condition) {
            condition.RtypeName = '';
            dldata.selectedLeftColumn = _.find(dldata.columnDefs, { field: condition.LtypeName });

            dldata.condRcolumnNames = _.filter(dldata.columnDefs, { InputType: dldata.selectedLeftColumn.InputType });

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

        var watchPayoutSubpolicy = function () {
            if (angular.isUndefined(dldata.payoutSubpolicy))
                return false;
            var outResult = _.find(dldata.payoutSubpolicy.BOutputs, function (output) {
                return (output.Lsqlfunction != "");
            });

            dldata.outputWithFunction = (outResult) ? true : false;
        };

        var modelDateValidation = function (startDate, endDate) {
            if (angular.isUndefined(endDate) || endDate == null) {
                dldata.isModalDateValid = true;
                return;
            }
            startDate = moment(startDate);
            endDate = moment(endDate);
            dldata.isModalDateValid = (endDate > startDate);
        };

        return {
            disableIfRelationExists: disableIfRelationExists,
            checkDuplicateName: checkDuplicateName,
            addNewCondition: addNewCondition,
            deleteCondition: deleteCondition,
            addNewOutput: addNewOutput,
            deleteOutput: deleteOutput,
            resetPayoutSubpolicy: resetPayoutSubpolicy,
            changeLeftTypeName: changeLeftTypeName,
            watchPayoutSubpolicy: watchPayoutSubpolicy,
            modelDateValidation: modelDateValidation
        };
    }]);

csapp.controller('payoutSubpolicyCtrl', ['$scope', 'payoutSubpolicyDataLayer', 'payoutSubpolicyFactory', '$modal',
    function ($scope, datalayer, factory, $modal) {
        (function () {
            $scope.factory = factory;
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.dldata.payoutSubpolicy = {};
            $scope.dldata.payoutSubpolicy.Category = "Liner";
            $scope.dldata.payoutSubpolicy.PayoutSubpolicyType = 'Subpolicy';
            $scope.dldata.newCondition = {};
            $scope.dldata.newCondition.Rtype = "Value";

            $scope.datalayer.getProducts();
        })();

        $scope.openmodal = function () {
            $scope.modalData = {};
            $scope.modalData.forActivate = true;
            $scope.modalData.startDate = null;
            $scope.modalData.endDate = null;
            $modal.open({
                templateUrl: '/Billing/subpolicy/subpolicy-date-model.html',
                controller: 'subpolicydatemodel',
                resolve: {
                    modalData: function () {
                        return $scope.modalData;
                    }
                }
            });
        };

        $scope.$watch("payoutSubpolicy.BOutputs.length", factory.watchPayoutSubpolicy);

    }]);

csapp.controller('subpolicydatemodel', [
    '$scope', 'payoutSubpolicyDataLayer', 'payoutSubpolicyFactory', 'modalData', '$modalInstance',
    function($scope, datalayer, factory, modaldata, $modalInstance) {
        $scope.datalayer = datalayer;
        $scope.dldata = datalayer.dldata;
        $scope.factory = factory;

        $scope.modalData = modaldata;

        $scope.closeModel = function() {
            $modalInstance.dismiss();
        };

        $scope.activateSubPolicy = function(modalData) {
            $scope.datalayer.activateSubPoicy(modalData).then(function () {
                $modalInstance.close();
            });
        };
    }

]);
