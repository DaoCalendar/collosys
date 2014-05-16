
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

        var saveTokens = function (tokensList) {
            return restApi.customPOST(tokensList, 'SaveTokens').then(function () {
                return;
            });
        };
        return {
            dldata: dldata,
            getFormulaList: getFormulaList,
            getColumnNames: getColumnNames,
            getBConditions: getBConditions,
            saveTokens: saveTokens
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

        $scope.selectFormula = function (selectedformula) {
            $scope.boolOpr.showDetails = true;
            $scope.formula = selectedformula;
        };
        $scope.saveTokens = function(selectedTokens) {
            var tokensList = _.union(selectedTokens.conditionTokens,
                selectedTokens.ifOutputTokens,
                selectedTokens.ElseOutputTokens);
            datalayer.saveTokens(tokensList);
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
