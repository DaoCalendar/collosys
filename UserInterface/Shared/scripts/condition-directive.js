csapp.directive("csCondition", function () {
    return {
        restrict: 'E',
        controller: 'conditionCtrl',
        templateUrl: baseUrl + 'Shared/templates/condition-directive.html',
        scope: {
            type: '@',
            tableName: '@',
            selected: '=',
            formulaList: '='
        }
    };

});

csapp.controller('conditionCtrl', ['$scope', '$csModels', 'operatorsFactory', 'tokenValidations',
    function ($scope, $csmodels, operatorFactory, validations) {

        //#region reset
        var resetTokenlist = function () {
            $scope.tokens.tokensList = [];
            $scope.tokens.nextTokens = [];
        };

        var resetCollections = function () {
            $scope.collections = {
                tableColumns: [],
                formulaListC: []
            };
        };

        var clearFilterString = function () {
            $scope.filter.filterString = '';
        };
        //#endregion

        //#region init
        var getEnumValues = function (value) {
            var values = [];
            angular.forEach(value.valueList, function (valueInner, key) {
                values.push({
                    'type': 'enum',
                    'text': valueInner,
                    'value': valueInner,
                    'datatype': 'enum',
                    'valuelist': []
                });
            });
            return values;
        };

        var getOperatorList = function () {
            if ($scope.type === 'Condition') {
                $scope.tokens.tokensList = _.union($scope.tokens.tokensList,
                operatorFactory.Operators.numberOperators(),
                operatorFactory.Operators.relationals(),
                operatorFactory.Operators.stringOperators(),
                operatorFactory.Operators.sqlOperators(),
                operatorFactory.Operators.dateOperators(),
                operatorFactory.Operators.conditionals());

            } else if ($scope.type === 'Output') {

                $scope.tokens.tokensList = _.union($scope.tokens.tokensList,
               operatorFactory.Operators.numberOperators(),
               operatorFactory.Operators.sqlOperators());
            }
        };

        var createTableList = function () {

            angular.forEach($scope.modal, function (value, key) {
                $scope.collections.tableColumns.push({
                    'type': 'Table',
                    'text': 'Col:' + key,
                    'value': $scope.tableName + '.' + key,
                    'datatype': value.type,
                    'valuelist': value.type === 'enum' ? getEnumValues(value) : []
                });
            });
            $scope.tokens.tokensList = _.union($scope.tokens.tokensList, $scope.collections.tableColumns);
        };

        var createFormulaList = function () {
            angular.forEach($scope.formulaList, function (value, key) {
                $scope.collections.formulaListC.push({
                    'type': 'Formula',
                    'text': 'For:' + value.Name + '()',
                    'value': value.Id,
                    'datatype': value.OutputType.toLowerCase(),
                    'valuelist': []
                });
            });
            $scope.tokens.tokensList = _.union($scope.tokens.tokensList, $scope.collections.formulaListC);
        };

        var initialiseList = function () {
            resetTokenlist();
            resetCollections();
            createTableList();
            getOperatorList();
            createFormulaList();
        };

        var initLocals = function () {
            $scope.modal = $csmodels.tables[$scope.tableName];
            $scope.tokens = {
                tokensList: [],
                selected: [],
                lastToken: {},
                nextTokens: []
            };
            $scope.collections = {
                formulaListC: [],
                tableColumns: []
            };
        };
        var initFirstTokens = function () {
            initialiseList();
            $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function (row) {
                return (row.type == 'Formula' || row.type == 'Table');
            });
            return;
        };

        (function () {
            initLocals();
            initFirstTokens();
            //initialiseList();
        })();

        $scope.$watch('formulaList', initFirstTokens);
        //#endregion

        //#region 
        //call to list initialisers

        //set token for last and second last
        var setToken = function (token) {
            $scope.tokens.secondLastToken = angular.isUndefined($scope.tokens.secondLastToken) ?
                angular.copy(token) : angular.isUndefined($scope.tokens.lastToken) ?
                    angular.copy(token) : angular.copy($scope.tokens.lastToken);
            $scope.tokens.lastToken = token;
        };

        //add token selected on page to token list
        $scope.addToken = function (item, model, label) {
            $scope.tokens.selected.push(item);
            setToken(item);
            setNextToken(item);
            clearFilterString();
        };

        //adds value to token list
        $scope.addValue = function (value) {
            var seleVal = {
                'type': 'value',
                'text': value,
                'value': value,
                'datatype': 'string',
                'valuelist': []
            };
            if (validations.validateValue($scope.tokens, value)) {
                $scope.tokens.selected.push(seleVal);
                setToken(seleVal);
                setNextToken(seleVal);
                clearFilterString();
            } else {
                $scope.tokens.error = 'Please select field or operator first';
                clearFilterString();
            }

        };

        //initialise variables
        var setNextToken = function (token) {
            if (token.type == 'Operator' || token.type=='Sql') {
                $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function (row) {
                    return ((row.type == 'Formula' || row.type == 'Table') &&
                    (row.datatype === token.datatype));
                });
                if (token.datatype == 'number') {
                    $scope.tokens.nextTokens = _.union($scope.tokens.nextTokens, _.filter($scope.tokens.tokensList, function (row) {
                        return (row.type == 'Sql');
                    }));
                }
            } else if(token.type=='Formula' || token.type=='Table') {
                $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function (row) {
                    return ((row.type === 'Operator') && (row.datatype === token.datatype));
                });
            } else if (token.type === 'value') {
                $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function (row) {
                    return ((row.type === 'Operator') && (row.datatype === $scope.tokens.secondLastToken.datatype));
                });
            }
            
            if (token.type == 'Sql') {
                $scope.tokens.nextTokens = _.filter($scope.tokens.nextTokens, function(row) {
                    return (row.type !== token.type);
                });
            }
        };

        $scope.reset = function () {
            clearFilterString();
            initLocals();
            initFirstTokens();
        };

    }]);

csapp.factory('tokenValidations', ['$csfactory', function ($csfactory) {

    var validateOperator = function (tokens, newToken) {

        if (newToken.datatype === 'sql' &&
            (angular.isDefined(tokens.lastToken.type) &&
                tokens.lastToken.type !== 'Operator'))
            return false;

        if (newToken.datatype === 'sql' &&
            (angular.isDefined(tokens.lastToken.type)
                && tokens.lastToken.type === 'Operator'
                && tokens.lastToken.datatype !== 'sql')) {
            return true;
        }

        if (angular.isDefined(tokens.lastToken.type) && tokens.lastToken.type !== 'Operator')
            return true;
        if (newToken.datatype === 'sql') {
            if (tokens.lastToken.datatype !== 'sql') {
                return true;
            } else {
                return false;
            }
        }

        return false;
    };

    var validateFormula = function (tokens, newToken) {
        if (angular.isUndefined(tokens.lastToken.type)) {
            return true;
        }
        if (tokens.lastToken.type == 'Operator') {
            return true;
        }
        return false;
    };

    var validateTable = function (tokens, newToken) {
        return validateFormula(tokens, newToken);
    };

    var validate = function (tokens, newToken) {
        var result = false;
        switch (newToken.type) {
            case 'Operator':
                result = validateOperator(tokens, newToken);
                break;
            case 'Formula':
                result = validateFormula(tokens, newToken);
                break;
            case 'Table':
                result = validateTable(tokens, newToken);
                break;
            default:
        }

        return result;
    };

    var validateValue = function (tokens, value) {
        if (angular.isDefined(tokens.lastToken.type) &&
            tokens.lastToken.type === 'Operator') {
            return true;
        }
        return false;
    };

    return {
        validate: validate,
        validateValue: validateValue
    };
}]);

csapp.factory('operatorsFactory', function () {
    var operators = {};

    operators.numberOperators = function () {
        return [
            {
                'type': 'Operator',
                'text': 'Opr:+',
                'value': 'Plus',
                'datatype': 'number',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Opr:-',
                'value': 'Minus',
                'datatype': 'number',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Opr:/',
                'value': 'Divide',
                'datatype': 'number',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Opr:*',
                'value': 'Multiply',
                'datatype': 'number',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Opr:%',
                'value': 'ModuloDivide',
                'datatype': 'number',
                'valuelist': []
            }
        ];
    };

    operators.conditionals = function () {
        return [
            {
                'type': 'Operator',
                'text': 'Opr:=',
                'value': 'EqualTo',
                'datatype': 'conditional',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Opr:!=',
                'value': 'NotEqualTo',
                'datatype': 'conditional',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Opr:<',
                'value': 'LessThan',
                'datatype': 'conditional',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Opr:<=',
                'value': 'LessThanEqualTo',
                'datatype': 'conditional',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Opr:>',
                'value': 'GreaterThan',
                'datatype': 'conditional',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Opr:>=',
                'value': 'GreaterThanEqualTo',
                'datatype': 'conditional',
                'valuelist': []
            }
        ];
    };

    operators.relationals = function () {
        return [{
            'type':
                'Operator',
            'text':
                'Opr:And',
            'value':
                'AND',
            'datatype':
                'relational',
            'valuelist':
                []
        },
            {
                'type':
                    'Operator',
                'text':
                    'Opr:Or',
                'value':
                    'OR',
                'datatype':
                    'relational',
                'valuelist':
                    []
            }];
    };

    operators.stringOperators = function () {
        return [
             {
                 'type': 'Operator',
                 'text': 'Opr:=',
                 'value': 'EqualTo',
                 'datatype': 'text',
                 'valuelist': []
             },
             {
                 'type': 'Operator',
                 'text': 'Opr:!=',
                 'value': 'NotEqualTo',
                 'datatype': 'text',
                 'valuelist': []
             },
            {
                'type': 'Operator',
                'text': 'Opr:Contains',
                'value': 'Contains',
                'datatype': 'text',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Opr:DoNotContains',
                'value': 'DoNotContains',
                'datatype': 'text',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Opr:StartsWith',
                'value': 'StartsWith',
                'datatype': 'text',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Opr:EndsWith',
                'value': 'EndsWith',
                'datatype': 'text',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Opr:IsInList',
                'value': 'IsInList',
                'datatype': 'text',
                'valuelist': []
            }
        ];
    };

    operators.sqlOperators = function () {
        return [
            {
                'type': 'Sql',
                'text': 'Avg Of',
                'value': 'AVG',
                'datatype': 'number',
                'valuelist': []
            },
            {
                'type': 'Sql',
                'text': 'Count Of',
                'value': 'COUNT',
                'datatype': 'number',
                'valuelist': []
            },
            {
                'type': 'Sql',
                'text': 'Sum Of',
                'value': 'SUM',
                'datatype': 'number',
                'valuelist': []
            }
        ];
    };

    operators.dateOperators = function () {
        return [
             {
                 'type': 'Operator',
                 'text': 'Date:Today',
                 'value': 'Today',
                 'datatype': 'Date',
                 'valuelist': []
             },
            {
                'type': 'Operator',
                'text': 'Date:Yesterday',
                'value': 'Yesterday',
                'datatype': 'Date',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Date:Tommorow',
                'value': 'Tommorow',
                'datatype': 'Date',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Date:MonthStart',
                'value': 'MonthStart',
                'datatype': 'Date',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Date:MonthEnd',
                'value': 'MonthEnd',
                'datatype': 'Date',
                'valuelist': []
            }
        ];
    };

    return {
        Operators: operators
    };
});

csapp.filter('changetext', function () {
    return function (input) {
        if (input.search('Opr:') > -1) {
            return input.replace("Opr:", "");
        }
        if (input.search('For:') > -1) {
            return input.replace("For:", "");
        }
        if (input.search('Col:') > -1) {
            return input.replace("Col:", "");
        }
        return input;
    };
});

//#region Condition directive
csapp.directive("csCondition", function () {
    return {
        restrict: 'E',
        controller: 'conditionCtrl',
        templateUrl: baseUrl + 'Shared/templates/condition-directive.html',
        scope: {
            type: '@',
            tableName: '@',
            selected: '=',
            formulaList: '='
        }
    };

});

//#endregion

//if (validations.validate($scope.tokens, item)) {
//    $scope.tokens.selected.push(item);
//    setToken(item);
//    clearFilterString();
//    $scope.tokens.error = null;
//} else {
//    $scope.tokens.error = 'Please insert valid token';
//    clearFilterString();
//}
