
csapp.factory('formulaDataLayer', ['Restangular', '$csnotify', '$csfactory',
    function (rest, $csnotify, $csfactory) {

        var dldata = {};
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

        var getBConditions = function (formulaId) {
            if ($csfactory.isNullOrEmptyGuid(formulaId)) {
                return [];
            }
            return restApi.customGET("GetBConditions", { parentId: formulaId }).then(function (data) {
                return data;
            });
        };

        var saveFormula = function (formula) {
            return restApi.customPOST(formula, 'Post').then(function () {
                return;
            });
        };
        
        return {
            dldata: dldata,
            getFormulaList: getFormulaList,
            getColumnNames: getColumnNames,
            getBConditions: getBConditions,
            saveFormula: saveFormula
        };
    }]);

csapp.controller('formulaController', ['$scope', 'formulaDataLayer', 'formulaFactory', '$csfactory', '$csModels',
    function ($scope, datalayer, factory, $csfactory, $csModels) {

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
        
        var combineTokens = function (selectedTokens) {
            return _.union(selectedTokens.conditionTokens,
                selectedTokens.ifOutputTokens,
                selectedTokens.ElseOutputTokens);
        };

        var divideTokens = function(tokensList) {
            $scope.selectedTokens.conditionTokens = _.filter(tokensList, { 'GroupId': '0.Condition' });
            $scope.selectedTokens.ifOutputTokens = _.filter(tokensList, { 'GroupId': '1.Output' });
            $scope.selectedTokens.ElseOutputTokens = _.filter(tokensList, { 'GroupId': '2.Output' });
        };

        $scope.selectFormula = function (selectedformula) {
            $scope.boolOpr.showDetails = true;
            $scope.formula = selectedformula;
            divideTokens(selectedformula.BillTokens);
        };
        
        

        $scope.saveFormula = function(formula, selectedTokens) {
            formula.BillTokens = combineTokens(selectedTokens);
            datalayer.saveFormula(formula).then(function(data) {

            });
        };
        
        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            $scope.Formula = $csModels.getColumns("Formula");
            $scope.selectedTokens = {
                conditionTokens: [],
                ifOutputTokens: [],
                ElseOutputTokens: []
            };
            initPageData();
        })();


    }]);
