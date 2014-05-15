csapp.directive("csOutput", function () {
    return {
        restrict: 'E',
        controller: 'outputCtrl',
        templateUrl: baseUrl + 'Shared/templates/output-directive.html',
        scope: {
            tableName: '@',
            selected: '=',
            formulaList: '='
        }
    };

});

csapp.controller('outputCtrl', ['$scope', '$csModels', 'operatorsFactory', 'tokenValidations', 'queryGenHelpers', 'tokenHelpers',
    function ($scope, $csmodels, operatorFactory, validations, helpers, tokenHelpers) {

        //#region init
        var tokenHelper = tokenHelpers.tokenHelper;

        var initLocals = function () {
            $scope.modal = $csmodels.getTable($scope.tableName);
            initToken();
        };

        var initToken = function () {
            $scope.tokens = {
                tokensList: [],
                selected: [],
                nextTokens: [],
                lastToken: {},
                collections: {
                    formulaListC: [],
                    tableColumns: []
                },
                filterString: ''
            };
        };

        var initFirstTokens = function () {
            tokenHelper.initListOutput($scope.tokens, $scope.modal,
                $scope.tableName, $scope.formulaList);

            $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function (row) {
                return ((row.type == 'Formula' || row.type == 'Table') && row.datatype == 'number');
            });
            return;
        };

        (function () {
            initLocals();
            initFirstTokens();
        })();

        $scope.$watch('formulaList', initFirstTokens);

        //#endregion

        //#region 

        $scope.addToken = function (item, model, label) {
            tokenHelper.addTokenToTokenList($scope.tokens, item);
            setNextToken(item);
        };

        $scope.addValue = function (value) {
            if (validations.validateValue($scope.tokens, value)) {
                var seleVal = tokenHelper.setAddValue($scope.tokens, value);
                setNextToken(seleVal);
            } else {
                $scope.tokens.error = 'Please select field or operator first';
                tokenHelper.clearFilterString($scope.tokens);
            }
        };

        $scope.tokens.resetClearList = function () {
            $scope.tokens.tokensList = [];
            $scope.tokens.nextTokens = [];
            $scope.tokens.collections.formulaListC = [];
            $scope.tokens.collections.tableColumns = [];
        };

        $scope.tokens.resetAll = function () {
            initToken();
        };

        var setNextToken = function (token) {
            if (token.type == 'Operator' || token.type == 'Sql') {
                $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function (row) {
                    return ((row.type == 'Formula' || row.type == 'Table') &&
                    (row.datatype === token.datatype));
                });
                if (token.datatype == 'number') {
                    $scope.tokens.nextTokens = _.union($scope.tokens.nextTokens, _.filter($scope.tokens.tokensList, function (row) {
                        return (row.type == 'Sql');
                    }));
                }
            } else if (token.type == 'Formula' || token.type == 'Table') {
                $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function (row) {
                    return ((row.type === 'Operator') && (row.datatype === token.datatype));
                });
            } else if (token.type === 'value') {
                $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function (row) {
                    return ((row.type === 'Operator') && (row.datatype === $scope.tokens.secondLastToken.datatype));
                });
            }

            if (token.type == 'Sql') {
                $scope.tokens.nextTokens = _.filter($scope.tokens.nextTokens, function (row) {
                    return (row.type !== token.type);
                });
            }
        };

        $scope.reset = function () {
            $scope.tokens.resetAll();
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

csapp.factory('queryGenHelpers', function () {

    var createTableList = function (modal, tableName) {
        var list = [];
        angular.forEach(modal.Columns, function (value, key) {
            list.push({
                'type': 'Table',
                'text': 'Col:' + key,
                'value': tableName + '.' + key,
                'datatype': value.type,
                'valuelist': []
            });
        });
        return angular.copy(list);
    };

    var createFormulaList = function (formulaList) {
        var list = [];
        angular.forEach(formulaList, function (value, key) {
            list.push({
                'type': 'Formula',
                'text': 'For:' + value.Name + '()',
                'value': value.Id,
                'datatype': value.OutputType.toLowerCase(),
                'valuelist': []
            });
        });
        return angular.copy(list);
    };

    var convertValueIntoObject = function (value) {
        return angular.copy({
            'type': 'value',
            'text': value,
            'value': value,
            'datatype': 'value',
            'valuelist': []
        });
    };

    return {
        getTableList: createTableList,
        getFormulaList: createFormulaList,
        convertValue: convertValueIntoObject
    };
});

csapp.factory('tokenHelpers', ['queryGenHelpers', 'operatorsFactory',
    function (helpers, operatorFactory) {

        var token = {};

        token.resetTokenList = function (tokens) {
            tokens.tokensList = [];
            tokens.nextTokens = [];
        };

        token.resetCollections = function (tokens) {
            tokens.collections.formulaListC = [];
            tokens.collections.tableColumns = [];
        };

        token.clearFilterString = function (tokens) {
            tokens.filterString = '';
        };

        token.setLastandSecondToken = function (tokens, tokenVal) {
            tokens.secondLastToken = angular.isUndefined(tokens.secondLastToken) ?
                   angular.copy(tokenVal) : angular.isUndefined(tokens.lastToken) ?
                       angular.copy(tokenVal) : angular.copy(tokens.lastToken);
            tokens.lastToken = tokenVal;
        };

        token.createTableList = function (tokens, modal, tableName) {
            tokens.collections.tableColumns = helpers.getTableList(modal, tableName);
            tokens.tokensList = _.union(tokens.tokensList, tokens.collections.tableColumns);
        };

        token.createFormulaList = function (tokens, formulaList) {
            tokens.collections.formulaListC = helpers.getFormulaList(formulaList);
            tokens.tokensList = _.union(tokens.tokensList, tokens.collections.formulaListC);
        };

        token.loadAllOperators = function (tokens) {
            tokens.tokensList = _.union(tokens.tokensList,
                operatorFactory.Operators.numberOperators(),
                operatorFactory.Operators.relationals(),
                operatorFactory.Operators.stringOperators(),
                operatorFactory.Operators.sqlOperators(),
                operatorFactory.Operators.dateOperators(),
                operatorFactory.Operators.conditionals());

        };

        token.loadOutputOperators = function (tokens) {
            tokens.tokensList = _.union(tokens.tokensList,
           operatorFactory.Operators.numberOperators(),
           operatorFactory.Operators.sqlOperators());
        };

        token.initListOutput = function (tokens, modal, tableName, formulaList) {
            token.resetTokenList(tokens);
            token.resetCollections(tokens);
            token.createTableList(tokens, modal, tableName);
            token.loadOutputOperators(tokens);
            token.createFormulaList(tokens, formulaList);
        };

        token.initListsConditions = function (tokens, modal, tableName, formulaList) {
            token.resetTokenList(tokens);
            token.resetCollections(tokens);
            token.createTableList(tokens, modal, tableName);
            token.loadAllOperators(tokens);
            token.createFormulaList(tokens, formulaList);
        };

        token.setAddValue = function (tokens, value) {
            var seleVal = helpers.convertValue(value);
            tokens.selected.push(seleVal);
            token.setLastandSecondToken(tokens, seleVal);
            token.clearFilterString(tokens);
            return seleVal;
        };

        token.addTokenToTokenList = function (tokens, tokenVal) {
            tokens.selected.push(tokenVal);
            token.setLastandSecondToken(tokens, tokenVal);
            token.clearFilterString(tokens);
        };

        return {
            tokenHelper: token,
        };
    }]);

//#region Condition directive
csapp.directive("csCondition", function () {
    return {
        restrict: 'E',
        controller: 'conditionCtrl',
        templateUrl: baseUrl + 'Shared/templates/condition-directive.html',
        scope: {
            tableName: '@',
            selected: '=',
            formulaList: '='
        }
    };

});

csapp.controller('conditionCtrl', ['$scope', '$csModels', 'operatorsFactory', 'tokenValidations', 'queryGenHelpers', 'tokenHelpers',
    function ($scope, $csmodels, operatorFactory, validations, helpers, tokenHelpers) {

        //#region init
        var tokenHelper = tokenHelpers.tokenHelper;

        var initLocals = function () {
            $scope.modal = $csmodels.getTable($scope.tableName);
            initToken();
        };

        var initToken = function () {
            $scope.tokens = {
                tokensList: [],
                selected: [],
                nextTokens: [],
                lastToken: {},
                collections: {
                    formulaListC: [],
                    tableColumns: []
                },
                filterString: '',
                hasConditional:false
            };
        };

        var initFirstTokens = function () {
            tokenHelper.initListsConditions($scope.tokens, $scope.modal,
                $scope.tableName, $scope.formulaList);

            $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function (row) {
                return (row.type == 'Formula' || row.type == 'Table');
            });
            return;
        };

        (function () {
            initLocals();
            initFirstTokens();
        })();

        $scope.$watch('formulaList', initFirstTokens);

        //#endregion

        //#region 

        $scope.addToken = function (item, model, label) {
            tokenHelper.addTokenToTokenList($scope.tokens, item);
            if (item.datatype == 'conditional') {
                $scope.tokens.hasConditional = true;
            }
            if (item.datatype == 'relational') {
                $scope.tokens.hasConditional = false;
            }
            setNextToken(item);
        };

        $scope.addValue = function (value) {
            if (validations.validateValue($scope.tokens, value)) {
                var seleVal = tokenHelper.setAddValue($scope.tokens, value);
                seleVal.datatype = $scope.tokens.secondLastToken.datatype;
                setNextToken(seleVal);
            } else {
                $scope.tokens.error = 'Please select field or operator first';
                tokenHelper.clearFilterString($scope.tokens);
            }
        };

        $scope.tokens.resetClearList = function () {
            $scope.tokens.tokensList = [];
            $scope.tokens.nextTokens = [];
            $scope.tokens.collections.formulaListC = [];
            $scope.tokens.collections.tableColumns = [];
        };

        $scope.tokens.resetAll = function () {
            initToken();
        };

        var setNextToken = function (token) {
            if (token.type == 'Operator') {
                $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function(row) {
                    return ((row.type == 'Formula' || row.type == 'Table') &&
                    (row.datatype == $scope.tokens.secondLastToken.datatype));
                });
            } else { //(token.type == 'Formula' || token.type == 'Table' || token.type=='value')
                if ($scope.tokens.hasConditional) {
                    $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function (row) {
                        return ((row.type == 'Operator') && (row.datatype == 'relational' ||
                            $scope.tokens.lastToken.datatype == row.datatype));
                    });
                } else {
                    $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function (row) {
                        return ((row.type == 'Operator') && (row.datatype == 'conditional' ||
                            $scope.tokens.lastToken.datatype == row.datatype));
                    });
                }
            }
        };

        $scope.setValidation = function() {
            if ($scope.tokens.hasConditional && $scope.tokens.lastToken.type !== 'Operator') {
                return 'alert-info';
            }
            return 'alert-danger';
        };
        
        $scope.reset = function () {
            $scope.tokens.resetAll();
            initFirstTokens();
        };

    }]);

//#endregion


//$scope.tokens.tokensList = [];
//$scope.tokens.selected = [];
//$scope.tokens.lastToken = {};
//$scope.tokens.nextTokens = [];
//$scope.tokens.collections = {};
//$scope.tokens.collections.formulaListC = [];
//$scope.tokens.collections.tableColumns = [];
//$scope.tokens.filterString = '';