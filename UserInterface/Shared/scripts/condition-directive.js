
csapp.directive("csCondition", function () {
    var linkFunction = function (scope, element, attrs, conditionCtrl) {
    };

    return {
        restrict: 'E',
        controller: 'conditionCtrl',
        templateUrl: baseUrl + 'Shared/templates/condition-directive.html',
        link: linkFunction,
        scope: {
            type: '@',
            tableName: '@',
            selected: '=',
            formulaList: '='
        }
    };

}).controller('conditionCtrl', ['$scope', '$csModels', function ($scope, $csmodels) {
    var getOperatorList = function () {
        var operators = [
            { 'type': '' }
        ];
    };
    var createList = function () {
        angular.forEach($scope.modal, function (value, key) {
            $scope.tableColumns.push({ 'type': 'Table', 'text': key, 'value': $scope.tableName + '.' + key });
        });
    };

    var cretaFormulaList = function () {
        angular.forEach($scope.formulaList, function (value, key) {
            $scope.formulaListC.push({ 'type': 'Formula', 'text': 'Formula: ' + value.Name, 'value': value.Id });
        });

        $scope.tokensList = _.union($scope.formulaListC, $scope.tableColumns);
    };

    var cleatFilterString = function() {
        $scope.filter.filterString = '';
    };

    $scope.selectFilterString = function (item, model, label) {
        $scope.message = '';
        $scope.expression += ' ' + item.text;
        $scope.lastTokenType = item.type;
        cleatFilterString();
    };

    $scope.changeMessage = function (entered, label) {
        if (entered.length > 0)
            $scope.message = 'Enter to Select';
    };

    $scope.addFilter = function () {
        $scope.expression += ' ' + $scope.filter.filterString;
        cleatFilterString();
    };

    var initLocals = function () {
        $scope.tokensList = [];
        $scope.tableColumns = [];
        $scope.formulaListC = [];
        $scope.operatorList = [];
        $scope.message = '';
        $scope.expression = '';
        $scope.lastTokenType = '';
        $scope.modal = $csmodels.tables[$scope.tableName];
    };

    $scope.$watch('formulaList', cretaFormulaList);

    (function () {
        initLocals();
        createList();
    })();
}]);


