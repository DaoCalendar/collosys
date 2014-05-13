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
            },
        ];
    };

    operators.sqlOperators = function () {
        return [
            {
                'type': 'Operator',
                'text': 'Avg Of',
                'value': 'AVG',
                'datatype': 'sql',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Count Of',
                'value': 'COUNT',
                'datatype': 'sql',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Sum Of',
                'value': 'SUM',
                'datatype': 'sql',
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
                 'datatype': 'date',
                 'valuelist': []
             },
            {
                'type': 'Operator',
                'text': 'Date:Yesterday',
                'value': 'Yesterday',
                'datatype': 'date',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Date:Tommorow',
                'value': 'Tommorow',
                'datatype': 'date',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Date:MonthStart',
                'value': 'MonthStart',
                'datatype': 'date',
                'valuelist': []
            },
            {
                'type': 'Operator',
                'text': 'Date:MonthEnd',
                'value': 'MonthEnd',
                'datatype': 'date',
                'valuelist': []
            },
        ];
    };

    return {
        Operators: operators
    };
});

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

})

    .controller('conditionCtrl', ['$scope', '$csModels', 'operatorsFactory',
    function ($scope, $csmodels, operatorFactory) {

        //get enum values from $csmodels
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

        //get all operators list from factory, add tokens list
        var getOperatorList = function () {
            $scope.tokens.tokensList = _.union($scope.tokens.tokensList,
            operatorFactory.Operators.numberOperators(),
            operatorFactory.Operators.relationals(),
            operatorFactory.Operators.stringOperators(),
            operatorFactory.Operators.sqlOperators(),
            operatorFactory.Operators.dateOperators(),
            operatorFactory.Operators.conditionals());

            if ($scope.type === 'Output') {
                $scope.tokens.tokensList = _.remove($scope.tokens.tokensList, function (operator) {
                    return (operator.datatype === 'number'
                        || operator.datatype === 'sql');
                    //return (operator.datatype !== 'conditional' &&
                    //operator.datatype !== 'relational' &&
                    //operator.datatype !== 'text');
                });
            }
        };

        //add tables columns to token list
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

        // formula list and add to token list
        var createFormulaList = function () {
            angular.forEach($scope.formulaList, function (value, key) {
                $scope.collections.formulaListC.push({
                    'type': 'Formula',
                    'text': 'For:' + value.Name + '()',
                    'value': value.Id,
                    'datatype': value.OutputType,
                    'valuelist': []
                });
            });
            $scope.tokens.tokensList = _.union($scope.tokens.tokensList, $scope.collections.formulaListC);
        };

        var resetTokenlist = function () {
            $scope.tokens.tokensList = [];
        };
        //call to list initialisers
        var initialiseList = function () {
            resetTokenlist();
            createTableList();
            getOperatorList();
            createFormulaList();
        };

        //clear the filter string of text box
        var cleatFilterString = function () {
            $scope.filter.filterString = '';
        };

        //manage list of tokens basis of last added token
        var managelist = function () {
            initialiseList();
            switch ($scope.tokens.lastToken.datatype) {
                case 'date':
                    $scope.tokens.tokensList = _.filter($scope.tokens.tokensList,
                    {
                        'datatype': 'conditional',
                        'type': 'Operator'
                    });
                    break;
                case 'number':
                    $scope.tokens.tokensList = _.filter($scope.tokens.tokensList,
                        {
                            'datatype': 'conditional' && 'number',
                            'type': 'Operator' && 'Formula' && 'Table'
                        });
                default:
                    initialiseList();
            }
        };

        var managelistOutput = function () {
            if ($scope.type !== 'Output') {
                return;
            }

        };
        //set token for last and second last
        var setToken = function(token) {
            $scope.tokens.secondLastToken = angular.isUndefined($scope.tokens.secondLastToken) ?
               angular.copy(token) : angular.isUndefined($scope.tokens.lastToken) ?
                    angular.copy(token) : angular.copy($scope.tokens.lastToken);
            $scope.tokens.lastToken = token;
        };
        //add token selected on page to token list
        $scope.addToken = function (item, model, label) {
            $scope.tokens.selected.push(item);
            setToken(item);
            //managelist();
            cleatFilterString();
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
            $scope.tokens.selected.push(seleVal);
            setToken(seleVal);
            cleatFilterString();
        };


        //initialise variables
        var initLocals = function () {
            $scope.modal = $csmodels.tables[$scope.tableName];
            $scope.tokens = {
                tokensList: [],
                selected: [],
                lastToken: {}
            };
            $scope.collections = {
                formulaListC: [],
                tableColumns: []
            };
        };

        $scope.reset = function () {
            cleatFilterString();
            initLocals();
            initialiseList();
        };

        //initialise all list when product is selected in page
        $scope.$watch('formulaList', initialiseList);

        (function () {
            initLocals();
        })();
    }]);

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
