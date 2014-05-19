
csapp.factory('payoutSubpolicyDataLayer', ['Restangular', '$csnotify', '$csfactory',
    function (rest, $csnotify, $csfactory) {
        var restApi = rest.all("PayoutSubpolicyApi");

        var getSubpolicyList = function(product) {
            return restApi.customGET("GetPayoutSubpolicy", { product: product })
                .then(function(data) {
                    return data;
                    
                }, function (data) {
                    $csnotify.error(data);
                });
        };
        
        var getFormulaList = function (product) {
            return restApi.customGET('GetFormulas', { product: product, category: 'Liner' }).then(function (data) {
                return _.filter(data, { PayoutSubpolicyType: 'Formula' });
            }, function (data) {
                $csnotify.error(data);
            });
        };

        var saveSubpolicy = function(subpolicy) {
            return restApi.customPOST(subpolicy, 'Post').then(function () {
                return;
            });
        };
        
        return {
            getSubpolicyList: getSubpolicyList,
            getFormulaList: getFormulaList,
            saveSubpolicy: saveSubpolicy
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

        var addNewCondition = function (condition, form) {
            condition.Ltype = "Column";
            condition.Lsqlfunction = "";
            condition.ConditionType = 'Condition';
            condition.ParentId = dldata.payoutSubpolicy.Id;
            condition.Priority = dldata.payoutSubpolicy.BConditions.length;

            if (condition.dateValueEnum && condition.dateValueEnum != 'Absolute_Date') {
                condition.Rvalue = condition.dateValueEnum;
            }

            condition.Rvalue = JSON.stringify(condition.Rvalue);

            var con = angular.copy(condition);
            dldata.payoutSubpolicy.BConditions.push(con);
            dldata.conditionValueType = 'text';

            datalayer.resetCondition();
            form.$setPristine();
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

        var addNewOutput = function (output, form) {
            output.ConditionType = 'Output';
            output.ParentId = dldata.payoutSubpolicy.Id;
            output.Priority = dldata.payoutSubpolicy.BOutputs.length;
            var out = angular.copy(output);
            dldata.payoutSubpolicy.BOutputs.push(out);

            datalayer.resetOutput();
            form.$setPristine();
        };

        var deleteOutput = function (output, index) {
            dldata.deleteConditions = [];
            if (dldata.payoutSubpolicy.BOutputs.length == 1) {
                dldata.newOutput.Operator = '';
            }
            if (output.Id) {
                output.ParentId = '';
                dldata.deleteConditions.push(output);
            }

            dldata.payoutSubpolicy.BOutputs.splice(index, 1);
            if (angular.isDefined(dldata.payoutSubpolicy.BOutputs[0])) {
                dldata.payoutSubpolicy.BOutputs[0].Operator = "";
            }

            for (var i = index; i < dldata.payoutSubpolicy.BOutputs.length; i++) {
                dldata.payoutSubpolicy.BOutputs[i].Priority = i;
            }
        };

        var resetPayoutSubpolicy = function (product) {
            dldata.payoutSubpolicy = {};
            dldata.payoutSubpolicy.BConditions = [];
            dldata.payoutSubpolicy.BOutputs = [];
            dldata.deleteConditions = [];
            dldata.newCondition = {};
            dldata.policyapproved = false;
            dldata.newOutput = {};
            dldata.payoutSubpolicy.Products = product;
            dldata.payoutSubpolicy.Category = "Liner";
            dldata.payoutSubpolicy.PayoutSubpolicyType = 'Subpolicy';
            datalayer.resetCondition();
            datalayer.resetOutput();
        };



        var watchPayoutSubpolicy = function () {
            if (angular.isUndefined(dldata.payoutSubpolicy))
                return false;
            var outResult = _.find(dldata.payoutSubpolicy.BOutputs, function (output) {
                return (output.Lsqlfunction != "");
            });

            dldata.outputWithFunction = (outResult) ? true : false;
            return dldata.outputWithFunction;
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
            watchPayoutSubpolicy: watchPayoutSubpolicy,
            modelDateValidation: modelDateValidation
        };
    }]);

csapp.controller('payoutSubpolicyCtrl', ['$scope', 'payoutSubpolicyDataLayer', 'payoutSubpolicyFactory', '$modal', '$csModels', '$csShared',
    function ($scope, datalayer, factory, $modal, $csModels, $csShared) {

        $scope.getSubpolicyList = function(product) {
            datalayer.getSubpolicyList(product).then(function(data) {
                $scope.subpolicyList = _.filter(data, { PayoutSubpolicyType: 'Subpolicy' });
                $scope.formulaList = _.filter(data, { PayoutSubpolicyType: 'Formula' });
            });
            $scope.subpolicy.Products = product;
        };
        
        var combineTokens = function (selectedTokens) {
            return _.union(selectedTokens.conditionTokens,
                selectedTokens.ifOutputTokens,
                selectedTokens.ElseOutputTokens);
        };

        var divideTokens = function (tokensList) {
            $scope.selectedTokens.conditionTokens = _.filter(tokensList, { 'GroupId': '0.Condition' });
            $scope.selectedTokens.ifOutputTokens = _.filter(tokensList, { 'GroupId': '1.Output' });
            $scope.selectedTokens.ElseOutputTokens = _.filter(tokensList, { 'GroupId': '2.Output' });
        };

        $scope.saveSubPolicy = function(subpolicy,selectedTokens) {
            subpolicy.BillTokens = combineTokens(selectedTokens);
            datalayer.saveSubpolicy(subpolicy).then(function (data) {

            });
        };

        $scope.selectSubpolicy = function(subpolicy) {
            $scope.subpolicy = subpolicy;
            divideTokens(subpolicy.BillTokens);
        };
        
        var init = function() {
            $scope.subpolicy = {
                PayoutSubpolicyType: 'Subpolicy'
            };
            $scope.formulaList = [];
            $scope.selectedTokens = {
                conditionTokens: [],
                ifOutputTokens: [],
                ElseOutputTokens: []
            };
        };
        
        (function () {
            $scope.CustBillViewModel = $csModels.getColumns("CustomerInfo");
            $scope.GPincode = $csModels.getColumns("Pincode");
            $scope.payoutSubpolicy = $csModels.getColumns("BillingSubpolicy");
            init();
        })();

        $scope.openmodal = function () {
            $scope.modalData = {};
            $scope.modalData.forActivate = true;
            $scope.modalData.startDate = null;
            $scope.modalData.endDate = null;
            $modal.open({
                templateUrl: baseUrl + 'Billing/subpolicy/subpolicy-date-model.html',
                controller: 'subpolicydatemodel',
                resolve: {
                    modalData: function () {
                        return $scope.modalData;
                    }
                }
            });
        };

    }]);

csapp.controller('subpolicydatemodel', [
    '$scope', 'payoutSubpolicyDataLayer', 'payoutSubpolicyFactory', 'modalData', '$modalInstance',
    function ($scope, datalayer, factory, modaldata, $modalInstance) {
        $scope.datalayer = datalayer;
        $scope.dldata = datalayer.dldata;
        $scope.factory = factory;

        $scope.modalData = modaldata;

        $scope.closeModel = function () {
            $modalInstance.dismiss();
        };

        $scope.activateSubPolicy = function (modalData) {
            $scope.datalayer.activateSubPoicy(modalData).then(function () {
                $modalInstance.close();
            });
        };
    }

]);
