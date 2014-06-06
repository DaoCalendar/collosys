
csapp.factory('formulaDataLayer', ['Restangular', '$csnotify', 
    function (rest, $csnotify) {

        var restApi = rest.all("PayoutSubpolicyApi");

        var getFormulaList = function (product) {
            return restApi.customGET('GetFormulas', { product: product, category: 'Liner' }).then(function (data) {
                return _.filter(data, { PayoutSubpolicyType: 'Formula' });
            }, function (data) {
                $csnotify.error(data);
            });
        };

        var getColumnNames = function (product) {
            //get column names
            return restApi.customGET("GetColumns", { product: product, category: 'Liner' }).then(function (data) {
                return data;
            }, function (data) {
                $csnotify.error(data);
            });
        };

        var saveFormula = function (formula) {
            return restApi.customPOST(formula, 'Post').then(function (data) {
                return data;
            });
        };

        return {
            getFormulaList: getFormulaList,
            getColumnNames: getColumnNames,
            saveFormula: saveFormula,
        };
    }]);

csapp.controller('formulaController', ['$scope', 'formulaDataLayer', 'formulaFactory', '$csfactory', '$csModels', 'tokenHelpers',
    function ($scope, datalayer, factory, $csfactory, $csModels, tokenHelpers) {

        $scope.initFormulaList = function (product) {
            if (angular.isUndefined(product)) {
                return;
            }
            datalayer.getFormulaList(product).then(function (data) {
                $scope.formulaList = data;
                $scope.formula.Products = product;
                getColumnNames(product);
            });
        };

        var initPageData = function () {
            $scope.selParams = {};
            $scope.formulaList = [];
            $scope.formula = {};
            $scope.columns = {
                columnNames: [],
                formulaNames: [],
                matrixNames: []
            };
            $scope.boolOpr = {
                showDetails: false
            };
            $scope.selectedTokens = {
                conditionTokens: [],
                ifOutputTokens: [],
                ElseOutputTokens: []
            };
            $scope.groupTokens = [];
            $scope.groupTokens.push(tokenHelpers.tokenHelper.getEmptyGroupToken(0));
        };

        var getColumnNames = function (product) {
            if (angular.isUndefined(product)) {
                return;
            }
            datalayer.getColumnNames(product).then(function (data) {
                $scope.columns.columnDefs = data;
                $scope.columns.columnNames = data;
                $scope.columns.outColumnNames = _.filter($scope.columns.columnDefs, { InputType: 'number' });
            });

        };

        var combineTokens = function (groupTokens) {
            var list = [];
            _.forEach(groupTokens, function (item) {
                _.forEach(item.Condition, function(condi) {
                    list.push(condi);
                });
                _.forEach(item.IfOutput, function (condi) {
                    list.push(condi);
                });
                _.forEach(item.ElseOutput, function (condi) {
                    list.push(condi);
                });
            });
            return list;
        };

        var divideTokens = function (tokensList) {
            $scope.groupTokens = [];
            var maxGroupId = _.max(_.uniq(_.pluck(_.filter(tokensList, { 'GroupType': 'Condition' }), 'GroupId')));
            if (maxGroupId != 0 || maxGroupId > 0) {
                maxGroupId = _.max(_.uniq(_.pluck(_.filter(tokensList, { 'GroupType': 'Output' }), 'GroupId')));
            }
            for (var i = 0; i <= maxGroupId; i++) {
                var groupToken = tokenHelpers.tokenHelper.getEmptyGroupToken(i);
                groupToken.Condition = _.sortBy(_.filter(tokensList, { 'GroupType': 'Condition', 'GroupId': i }), 'Priority');
                groupToken.IfOutput = _.sortBy(_.filter(tokensList, { 'GroupType': 'Output', 'GroupId': i }), 'Priority');
                groupToken.ElseOutput = _.sortBy(_.filter(tokensList, { 'GroupType': 'ElseOutput' }), 'Priority');
                $scope.groupTokens.push(groupToken);
            }
        };

        $scope.selectFormula = function (selectedformula) {
            $scope.boolOpr.showDetails = true;
            $scope.showDiv = true;
            $scope.formula = angular.copy(selectedformula);
            divideTokens(selectedformula.BillTokens);
        };

        $scope.addformula = function (product, form) {
            $scope.showDiv = true;
            $scope.formula = {};
            $scope.selected = [];
            $scope.formula.Products = product;
            form.$setPristine();
        };

        $scope.reset = function (product,form) {
            $scope.formula = {};
            $scope.selected = [];
            $scope.formula.Products = product;
            if (angular.isDefined(form)) {
                form.$setPristine();
            }
        };

        $scope.saveFormula = function (formula, groupTokens) {
            formula.BillTokens = combineTokens(groupTokens);
            datalayer.saveFormula(formula).then(function (data) {
                $scope.formulaList.push(data);
                $scope.reset();
            });
        };

        (function () {
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            $scope.Formula = $csModels.getColumns("Formula");
            initPageData();
        })();
    }]);
