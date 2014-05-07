
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
        return {
            dldata: dldata,
            getFormulaList: getFormulaList,
            getColumnNames: getColumnNames,
            getBConditions: getBConditions
        };
    }]);

csapp.factory('formulaFactory', ['formulaDataLayer', function (datalayer) {
    return {
    };
}]);

csapp.controller('formulaController', ['$scope', 'formulaDataLayer', 'formulaFactory', '$csfactory', '$csBillingModels', '$csShared', '$csFileUploadModels', '$csGenericModels',
    function ($scope, datalayer, factory, $csfactory, $csBillingModels, $csShared, $csFileUploadModels, $csGenericModels) {

        $scope.initFormulaList = function (product) {
            if (angular.isUndefined(product)) {
                return;
            }
            datalayer.getFormulaList(product).then(function (data) {
                $scope.formulaList = data;
                getColumnNames(product);
            });
        };

        var initPageData = function () {
            $scope.selParams = {};
            $scope.formulaList = [];
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

        var getBConditions = function (formulaId) {
            datalayer.getBConditions(formulaId).then(function (data) {
                $scope.formula.BConditions = data;
            });
        };

        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            $scope.Formula = $csBillingModels.models.Formula;
            initPageData();
        })();


    }]);
