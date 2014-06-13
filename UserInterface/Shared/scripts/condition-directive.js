//#region Output directive
csapp.directive("csOutput", function () {
    var linkfunction = function (scope, ele, attr, ctrl) {
        scope.csForm = ctrl[0];
        console.log(scope.csForm);
    };
    return {
        restrict: 'E',
        controller: 'outputCtrl',
        templateUrl: baseUrl + 'Shared/templates/output-directive.html',
        require: ['^csForm'],
        scope: {
            tableName: '@', //replace with product - models.js getBillingColumns
            formulaList: '=',
            matrixList: '=',
            groupId: '@',
            groupType: '@',
            tokensList: '=',
            debug: '@', //remove
            edit: '@' //remove
        },
        link: linkfunction
    };
});

csapp.controller('outputCtrl',
    ['$scope', '$csModels', 'operatorsFactory', 'tokenValidations', 'queryGenHelpers', 'tokenHelpers',
    function ($scope, $csmodels, operatorFactory, validations, helpers, tokenHelpers) {

        //#region init
        var tokenHelper = tokenHelpers.tokenHelper;

        var initLocals = function () {
            $scope.modal = $csmodels.getTable($scope.tableName);
            initToken();
        };

        var initToken = function () {
            console.log($scope);
            $scope.tokens = {
                AllTokensList: [],
                //selected: [],
                nextTokens: [],
                lastToken: {},
                collections: {
                    formulaListC: [],
                    tableColumns: [],
                    matrixListC: []
                },
                filterString: ''
            };
            //$scope.SavedTokensList = [];  DO NOT RESET
            //$scope.tokensList = $scope.tokens.selected;
        };

        var initFirstTokens = function () {
            tokenHelper.initListOutput($scope.tokens, $scope.modal,
                $scope.tableName, $scope.formulaList, $scope.matrixList);

            $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function (row) {
                return ((row.Type == 'Formula' || row.Type == 'Table' || row.Type == 'Sql' || row.Type == 'Matrix') && row.DataType == 'number');
            });
            return;
        };


        (function () {
            initLocals();
            initFirstTokens();
        })();

        $scope.$watch('formulaList', initFirstTokens);

        $scope.$watch('matrixList', initFirstTokens);

        //#endregion

        //#region 

        $scope.addToken = function (item) { //, model, label
            tokenHelper.addTokenToTokenList($scope.tokens, $scope.tokensList, item, $scope.groupId, $scope.groupType);
            setNextToken(item);
        };

        $scope.addValue = function (value) {
            if (validations.validateValue($scope.tokens, value)) {
                var seleVal = tokenHelper.setAddValue($scope.tokens, $scope.tokensList, value, $scope.groupId, $scope.groupType);
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
            $scope.tokens.collections.matrixListC = [];
            $scope.tokens.collections.tableColumns = [];
        };

        var setNextToken = function (token) {
            if (token.Type == 'Operator' || token.Type == 'Sql') {
                $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function (row) {
                    return ((row.Type == 'Formula' || row.Type == 'Table') &&
                    (row.datatype === token.datatype));
                });
                if (token.datatype == 'number') {
                    $scope.tokens.nextTokens = _.union($scope.tokens.nextTokens, _.filter($scope.tokens.tokensList, function (row) {
                        return (row.Type == 'Sql');
                    }));
                }
            } else if (token.Type == 'Formula' || token.Type == 'Table' || token.Type == 'Matrix') {
                $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function (row) {
                    return ((row.Type === 'Operator') && (row.datatype === token.datatype));
                });
            } else if (token.Type === 'Value') {
                $scope.tokens.nextTokens = _.filter($scope.tokens.tokensList, function (row) {
                    return ((row.Type === 'Operator') && (row.datatype === $scope.tokens.secondLastToken.datatype));
                });
            }

            if (token.Type == 'Sql') {
                $scope.tokens.nextTokens = _.filter($scope.tokens.nextTokens, function (row) {
                    return (row.Type !== token.Type);
                });
            }
        };

        $scope.setValidation = function () {
            if (angular.isUndefined($scope.tokensList)) {
                return '';
            }
            if ($scope.tokens.lastToken.Type == 'Operator' || $scope.tokensList.length < 2) {
                return 'alert-danger';
            }
            return 'alert-info';
        };

        $scope.reset = function () {
            initToken();
            initFirstTokens();
        };
    }]);


//#region Condition directive
csapp.directive("csCondition", function () {
    return {
        restrict: 'E',
        controller: 'conditionCtrl',
        require: ['^csForm'],
        templateUrl: baseUrl + 'Shared/templates/condition-directive.html',
        scope: {
            tableName: '@',
            formulaList: '=',
            groupId: '@',
            groupType: '@',
            tokensList: '=',
            debug: '@',
            edit: '@'
        }
    };

});

csapp.controller('conditionCtrl',
    ['$scope', '$csModels', 'operatorsFactory', 'tokenValidations',
        'queryGenHelpers', 'tokenHelpers', 'OperatorsGroup',
    function ($scope, $csmodels, operatorFactory, validations,
        helpers, tokenHelpers, operatorsGroup) {

        //#region init
        var tokenHelper = tokenHelpers.tokenHelper;

        var initLocals = function () {
            $scope.modal = $csmodels.getTable($scope.tableName);
            initToken();
        };

        var initToken = function () {
            $scope.tokens = {
                tokensList: [],
                nextTokens: [],
                lastToken: {},
                collections: {
                    formulaListC: [],
                    tableColumns: []
                },
                filterString: ''
            };

            $scope.tokensList = [];

            $scope.tokensSupport = {
                isValid: false,
                hasConditional: false,
                DataType: undefined,
                bracketCounter: 0,
                valueList:[]
            };
        };

        var initFirstTokens = function () {
            tokenHelper.initListsConditions($scope.tokens, $scope.modal,
                $scope.tableName, $scope.formulaList);

            $scope.tokens.nextTokens = operatorsGroup.group1($scope.tokens.tokensList);
            return;
        };

        (function () {
            initLocals();
            initFirstTokens();
        })();

        $scope.$watch('formulaList', initFirstTokens);

        //#endregion

        //#region 
        var manageBracketCounter = function (token) {
            if (token.Type == 'Operator' && token.Value == 'OpenBracket')
                $scope.tokensSupport.bracketCounter += 1;
            if (token.Type == 'Operator' && token.Value == 'CloseBracket')
                $scope.tokensSupport.bracketCounter -= 1;
        };

        $scope.addToken = function (item, model, label) {
            tokenHelper.addTokenToTokenList($scope.tokens, $scope.tokensList, item, $scope.groupId, $scope.groupType);
            manageBracketCounter(item);
            setNextToken(item);
        };

        $scope.addValue = function (value) {
            if (validations.validateValue($scope.tokens, value)) {
                var seleVal = tokenHelper.setAddValue($scope.tokens, $scope.tokensList, value, $scope.groupId, $scope.groupType);
                seleVal.DataType = $scope.tokens.secondLastToken.DataType;
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

        var isValidState = function (currentToken) {
            if ($scope.tokensSupport.bracketCounter !== 0) {
                $scope.tokensSupport.isValid = false;
                $scope.addForm.$setValidity('valid', $scope.tokensSupport.isValid);
                return;
            }
            if ($scope.tokensSupport.hasConditional === false) {
                $scope.tokensSupport.isValid = false;
                $scope.addForm.$setValidity('valid', $scope.tokensSupport.isValid);
                return;
            }
            if (currentToken.Type === "Operator") {
                if (currentToken.Value == 'CloseBracket' && $scope.tokensSupport.bracketCounter == 0) {
                    $scope.tokensSupport.isValid = true;
                    $scope.addForm.$setValidity('valid', $scope.tokensSupport.isValid);
                    return;
                }
                $scope.tokensSupport.isValid = false;
                $scope.addForm.$setValidity('valid', $scope.tokensSupport.isValid);
                return;
            }
            $scope.tokensSupport.isValid = true;
            $scope.addForm.$setValidity('valid', $scope.tokensSupport.isValid);
        };

        var getNextTokens = function (dataType, currentGroupType, currentToken) {

            switch (currentGroupType) {
                case 'group1': //group1:Formula/Column/Matrix/Brackets

                    if (dataType == undefined) {
                        return operatorsGroup.group1($scope.tokens.tokensList, dataType);
                    }
                    switch (dataType.toUpperCase()) {
                        case 'NUMBER':
                            var tokens = _.union(operatorsGroup.group2(),
                            operatorsGroup.group3(),
                            operatorsGroup.group4(),
                            operatorsGroup.addCloseBracket());
                            if ($scope.tokensSupport.hasConditional === true)
                                tokens = _.union(tokens, operatorsGroup.group5());
                            return tokens;
                        case 'BOOLEAN':
                            $scope.tokensSupport.hasConditional = true;
                            return _.union(operatorsGroup.group4(),
                            operatorsGroup.group5(),
                            operatorsGroup.addCloseBracket());
                        case 'ENUM':
                            var tokens2 = _.union(operatorsGroup.group4(),
                            operatorsGroup.addCloseBracket());
                            if ($scope.tokensSupport.hasConditional === true)
                                tokens2 = _.union(tokens2, operatorsGroup.group5(),
                                    $scope.tokensSupport.valueList);
                            return tokens2;//TODO: attach enum value list
                        default:
                            return operatorsGroup.group1($scope.tokens.tokensList, dataType);
                    }
                case 'group2': //group2:Plus,Minus,Brackets
                    if (dataType.toUpperCase() == 'NUMBER')
                        return operatorsGroup.group1($scope.tokens.tokensList, dataType);
                case 'group3': //group3:gt,lt,etc
                    $scope.tokensSupport.hasConditional = true;
                    return operatorsGroup.group1($scope.tokens.tokensList, dataType);
                case 'group4': //group4:equal,noteaual
                    switch (dataType.toUpperCase()) {
                        case 'NUMBER':
                            $scope.tokensSupport.hasConditional = true;
                            return _.union(
                                operatorsGroup.group1($scope.tokens.tokensList, dataType)
                            );
                        case 'BOOLEAN':
                            $scope.tokensSupport.hasConditional = true;
                            return _.union(
                                operatorsGroup.group1($scope.tokens.tokensList, dataType),
                                operatorsGroup.boolTokens()
                            );
                        case 'ENUM':
                            $scope.tokensSupport.hasConditional = true;
                            return _.union(
                                operatorsGroup.group1($scope.tokens.tokensList, dataType),
                                $scope.tokensSupport.valueList
                            );
                        default:
                            return operatorsGroup.group1($scope.tokens.tokensList, dataType);
                    }
                case 'group5': //group5:And/Or
                    $scope.tokensSupport.DataType = undefined;
                    $scope.tokensSupport.isValid = false;
                    $scope.tokensSupport.hasConditional = false;
                    return operatorsGroup.group1($scope.tokens.tokensList);
            }
            return [];
        };

        var setNextToken = function (token) {
            var currentGroupType = operatorsGroup.getGroupType(token);
            if (token.Type == 'Formula' || token.Type == 'Table' || token.Type == 'Matrix') {
                $scope.tokensSupport.DataType = token.DataType;
                $scope.tokensSupport.valueList = token.valuelist;
            }
            $scope.tokens.nextTokens = getNextTokens($scope.tokensSupport.DataType, currentGroupType, token);
            isValidState(token);
        };

        $scope.reset = function () {
            initToken();
            initFirstTokens();
        };

    }]);
//#endregion

csapp.factory('OperatorsGroup', ['operatorsFactory', function (operatorsFactory) {

    //group1:Formula/Column/Matrix/Brackets
    //group2:Plus,Minus,Brackets
    //group3:gt,lt,etc
    //group4:equal,noteaual
    //group5:And/Or

    var group1 = function (tokensList, dataType) {
        if (angular.isUndefined(dataType))
            dataType = 'All';
        return _.filter(tokensList, function (token) {
            return ((token.Type == 'Formula' || token.Type == 'Table' || token.Type == 'Matrix') &&
                (token.DataType.toUpperCase() == dataType.toUpperCase() || dataType == 'All'))
                || (token.Type == 'Operator' && token.Value == 'OpenBracket');
        });
    };

    var group2 = function () {
        return operatorsFactory.Operators.numberOperators();
    };

    var group3 = function () {
        return operatorsFactory.Operators.conditionals();
    };

    var group4 = function () {
        return operatorsFactory.Operators.equality();
    };

    var group5 = function () {
        return operatorsFactory.Operators.relationals();
    };

    var getGroupType = function (token) {
        switch (token.Type) {
            case 'Formula':
            case 'Table':
            case 'Matrix':
            case 'Value':
                return 'group1';
            case 'Operator':
                if (token.DataType == 'bracket') {
                    return 'group1';
                }
                if (token.DataType == 'number')
                    return 'group2';
                if (token.DataType == 'conditional')
                    return 'group3';
                if (token.DataType == 'relational')
                    return 'group5';
                if (token.DataType == 'equality' || token.DataType == 'enumvals')
                    return 'group4';
                break;
            default:
                throw "invalid token type" + token.Type;
        }
    };

    var boolTokens = function () {
        return [
            {
                'Type': 'Operator',
                'Text': 'True',
                'Value': 'True',
                'DataType': 'enumvals',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'False',
                'Value': 'False',
                'DataType': 'enumvals',
                'valuelist': []
            }
        ];
    };

    var addCloseBracket = function () {
        return _.filter(operatorsFactory.Operators.bracketOperators(), function (token) {
            return (token.Value == 'CloseBracket');
        });
    };

    return {
        group1: group1,
        group2: group2,
        group3: group3,
        group4: group4,
        group5: group5,
        getGroupType: getGroupType,
        boolTokens: boolTokens,
        addCloseBracket: addCloseBracket
    };
}]);
//#region Single If else directive
csapp.directive('csIfElse', function () {
    return {
        restrict: 'E',
        controller: 'ifElseCtrl',
        templateUrl: baseUrl + 'Shared/templates/if-else-directive.html',
        scope: {
            tableName: '@',
            selected: '=',
            formulaList: '=',
            groupId: '@',
            tokensList: '=',
            debug: '@',
            edit: '@'
        }
    };
});

csapp.controller('ifElseCtrl', function () { });
//#endregion

csapp.directive('csMultiIfElse', function () {
    return {
        restrict: 'E',
        controller: 'multiIfElseCtrl',
        templateUrl: baseUrl + 'Shared/templates/multi-if-else-directive.html',
        scope: {
            tableName: '@',
            selected: '=',
            formulaList: '=',
            matrixList: '=',
            tokensList: '=',
            debug: '@',
            edit: '@'
        }
    };
});

csapp.controller('multiIfElseCtrl', ['$scope', '$csModels', 'operatorsFactory', 'tokenValidations', 'queryGenHelpers', 'tokenHelpers',
    function ($scope, $csmodels, operatorFactory, validations, helpers, tokenHelpers) {
        var tokenHelper = tokenHelpers.tokenHelper;

        $scope.addIf = function (index) {
            $scope.noOfIf.push(index + 1);
            $scope.tokensList.push(tokenHelper.getEmptyGroupToken(index));
        };

        $scope.deleteIf = function (index) {
            $scope.noOfIf.splice(index, 1);
            $scope.tokensList.splice(index, 1);
        };

        var init = function () {
            $scope.noOfIf = new Array($scope.tokensList.length);
        };

        $scope.$watch('selected', init);
        (function () {
            init();
        })();
    }]);


//#region factory
csapp.factory('tokenValidations', ['$csfactory', function ($csfactory) {

    var validateOperator = function (tokens, newToken) {

        if (newToken.datatype === 'sql' &&
            (angular.isDefined(tokens.lastToken.Type) &&
                tokens.lastToken.Type !== 'Operator'))
            return false;

        if (newToken.datatype === 'sql' &&
            (angular.isDefined(tokens.lastToken.Type)
                && tokens.lastToken.Type === 'Operator'
                && tokens.lastToken.datatype !== 'sql')) {
            return true;
        }

        if (angular.isDefined(tokens.lastToken.Type) && tokens.lastToken.Type !== 'Operator')
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
        if (angular.isUndefined(tokens.lastToken.Type)) {
            return true;
        }
        if (tokens.lastToken.Type == 'Operator') {
            return true;
        }
        return false;
    };

    var validateTable = function (tokens, newToken) {
        return validateFormula(tokens, newToken);
    };

    var validate = function (tokens, newToken) {
        var result = false;
        switch (newToken.Type) {
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
        if (angular.isDefined(tokens.lastToken.Type) &&
            tokens.lastToken.Type === 'Operator') {
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
                'Type': 'Operator',
                'Text': 'Opr:+',
                'Value': 'Plus',
                'DataType': 'number',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'Opr:-',
                'Value': 'Minus',
                'DataType': 'number',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'Opr:/',
                'Value': 'Divide',
                'DataType': 'number',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'Opr:*',
                'Value': 'Multiply',
                'DataType': 'number',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'Opr:%',
                'Value': 'ModuloDivide',
                'DataType': 'number',
                'valuelist': []
            }
        ];
    };

    operators.conditionals = function () {
        return [
            //{
            //    'Type': 'Operator',
            //    'Text': 'Opr:=',
            //    'Value': 'EqualTo',
            //    'DataType': 'conditional',
            //    'valuelist': []
            //},
            //{
            //    'Type': 'Operator',
            //    'Text': 'Opr:!=',
            //    'Value': 'NotEqualTo',
            //    'DataType': 'conditional',
            //    'valuelist': []
            //},
            {
                'Type': 'Operator',
                'Text': 'Opr:<',
                'Value': 'LessThan',
                'DataType': 'conditional',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'Opr:<=',
                'Value': 'LessThanEqualTo',
                'DataType': 'conditional',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'Opr:>',
                'Value': 'GreaterThan',
                'DataType': 'conditional',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'Opr:>=',
                'Value': 'GreaterThanEqualTo',
                'DataType': 'conditional',
                'valuelist': []
            }
        ];
    };

    operators.relationals = function () {
        return [{
            'Type':
                'Operator',
            'Text':
                'Opr:And',
            'Value':
                'AND',
            'DataType':
                'relational',
            'valuelist':
                []
        },
            {
                'Type':
                    'Operator',
                'Text':
                    'Opr:Or',
                'Value':
                    'OR',
                'DataType':
                    'relational',
                'valuelist':
                    []
            }];
    };

    operators.stringOperators = function () {
        return [
             //{
             //    'Type': 'Operator',
             //    'Text': 'Opr:=',
             //    'Value': 'EqualTo',
             //    'DataType': 'Text',
             //    'valuelist': []
             //},
             {
                 'Type': 'Operator',
                 'Text': 'Opr:!=',
                 'Value': 'NotEqualTo',
                 'DataType': 'Text',
                 'valuelist': []
             },
            {
                'Type': 'Operator',
                'Text': 'Opr:Contains',
                'Value': 'Contains',
                'DataType': 'Text',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'Opr:DoNotContains',
                'Value': 'DoNotContains',
                'DataType': 'Text',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'Opr:StartsWith',
                'Value': 'StartsWith',
                'DataType': 'Text',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'Opr:EndsWith',
                'Value': 'EndsWith',
                'DataType': 'Text',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'Opr:IsInList',
                'Value': 'IsInList',
                'DataType': 'Text',
                'valuelist': []
            }
        ];
    };

    operators.sqlOperators = function () {
        return [
            {
                'Type': 'Sql',
                'Text': 'Avg Of',
                'Value': 'AVG',
                'DataType': 'number',
                'valuelist': []
            },
            {
                'Type': 'Sql',
                'Text': 'Count Of',
                'Value': 'COUNT',
                'DataType': 'number',
                'valuelist': []
            },
            {
                'Type': 'Sql',
                'Text': 'Sum Of',
                'Value': 'SUM',
                'DataType': 'number',
                'valuelist': []
            }
        ];
    };

    operators.dateOperators = function () {
        return [
             {
                 'Type': 'Operator',
                 'Text': 'Date:Today',
                 'Value': 'Today',
                 'DataType': 'Date',
                 'valuelist': []
             },
            {
                'Type': 'Operator',
                'Text': 'Date:Yesterday',
                'Value': 'Yesterday',
                'DataType': 'Date',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'Date:Tommorow',
                'Value': 'Tommorow',
                'DataType': 'Date',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'Date:MonthStart',
                'Value': 'MonthStart',
                'DataType': 'Date',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'Date:MonthEnd',
                'Value': 'MonthEnd',
                'DataType': 'Date',
                'valuelist': []
            }
        ];
    };

    operators.bracketOperators = function () {
        return [{
            'Type':
                'Operator',
            'Text':
                '(',
            'Value':
                'OpenBracket',
            'DataType':
                'bracket',
            'valuelist':
                []
        },
            {
                'Type':
                    'Operator',
                'Text':
                    ')',
                'Value':
                    'CloseBracket',
                'DataType':
                    'bracket',
                'valuelist':
                    []
            }];
    };

    operators.equality = function () {
        return [
            {
                'Type': 'Operator',
                'Text': 'Opr:=',
                'Value': 'EqualTo',
                'DataType': 'equality',
                'valuelist': []
            },
            {
                'Type': 'Operator',
                'Text': 'Opr:!=',
                'Value': 'NotEqualTo',
                'DataType': 'equality',
                'valuelist': []
            }
        ];
    };

    return {
        Operators: operators
    };
});

csapp.factory('queryGenHelpers', function () {

    var getEnumValues = function (value) {
        var values = [];
        angular.forEach(value.valueList, function (valueInner) { //, key
            values.push({
                'Type': 'Table',
                'Text': valueInner,
                'Value': valueInner,
                'DataType': 'text',
                'valuelist': []
            });
        });
        return values;
    };

    var createTableList = function (modal, tableName) {
        var list = [];
        angular.forEach(modal.Columns, function (value, key) {
            list.push({
                'Type': 'Table',
                'Text': 'Col:' + key,
                'Value': tableName + '.' + key,
                'DataType': value.type,
                'valuelist': value.type === 'enum' ? getEnumValues(value) : []
            });
        });
        return angular.copy(list);
    };

    var createFormulaList = function (formulaList) {
        var list = [];
        angular.forEach(formulaList, function (value) { //, key
            list.push({
                'Type': 'Formula',
                'Text': 'For:' + value.Name + '()',
                'Value': value.Id,
                'DataType': value.OutputType.toLowerCase(),
                'valuelist': []
            });
        });
        return angular.copy(list);
    };

    var createMatrixList = function (matrixList) {
        var list = [];
        angular.forEach(matrixList, function (value) { //, key
            list.push({
                'Type': 'Matrix',
                'Text': 'Mat:' + value.Name + '()',
                'Value': value.Id,
                'DataType': 'number',
                'valuelist': []
            });
        });
        return angular.copy(list);
    };

    var convertValueIntoObject = function (value) {
        return angular.copy({
            'Type': 'Value',
            'Text': value,
            'Value': value,
            'DataType': 'Value',
            'valuelist': []
        });
    };

    return {
        getTableList: createTableList,
        getFormulaList: createFormulaList,
        getMatrixList: createMatrixList,
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

        token.createMatrixList = function (tokens, matrixList) {
            tokens.collections.matrixListC = helpers.getMatrixList(matrixList);
            tokens.tokensList = _.union(tokens.tokensList, tokens.collections.matrixListC);
        };

        token.loadAllOperators = function (tokens) {
            tokens.tokensList = _.union(tokens.tokensList,
                operatorFactory.Operators.numberOperators(),
                operatorFactory.Operators.relationals(),
                operatorFactory.Operators.stringOperators(),
                operatorFactory.Operators.sqlOperators(),
                operatorFactory.Operators.dateOperators(),
                operatorFactory.Operators.conditionals(),
                operatorFactory.Operators.equality(),
                operatorFactory.Operators.bracketOperators());
        };

        token.loadOutputOperators = function (tokens) {
            tokens.tokensList = _.union(tokens.tokensList,
                operatorFactory.Operators.numberOperators(),
                operatorFactory.Operators.sqlOperators(),
                operatorFactory.Operators.bracketOperators());
        };

        token.initListOutput = function (tokens, modal, tableName, formulaList, matrixList) {
            token.resetTokenList(tokens);
            token.resetCollections(tokens);
            token.createTableList(tokens, modal, tableName);
            token.loadOutputOperators(tokens);
            token.createFormulaList(tokens, formulaList);
            token.createMatrixList(tokens, matrixList);
        };

        token.initListsConditions = function (tokens, modal, tableName, formulaList) {
            token.resetTokenList(tokens);
            token.resetCollections(tokens);
            token.createTableList(tokens, modal, tableName);
            token.loadAllOperators(tokens);
            token.createFormulaList(tokens, formulaList);
        };

        token.setAddValue = function (tokens, tokensList, value, groupId, groupType) {
            var seleVal = helpers.convertValue(value);
            seleVal.GroupId = groupId;
            seleVal.GroupType = groupType;
            seleVal.priority = tokensList.length;
            tokensList.push(seleVal);
            token.setLastandSecondToken(tokens, seleVal);
            token.clearFilterString(tokens);
            return seleVal;
        };

        token.addTokenToTokenList = function (tokens, tokensList, tokenVal, groupId, groupType) {
            tokenVal.GroupId = groupId;
            tokenVal.GroupType = groupType;
            tokenVal.priority = tokensList.length;
            tokensList.push(angular.copy(tokenVal));
            token.setLastandSecondToken(tokens, tokenVal);
            token.clearFilterString(tokens);
        };

        token.getEmptyGroupToken = function (index) {
            return angular.copy({ 'id': index, 'Condition': [], 'IfOutput': [], 'ElseOutput': [] });
        };

        return {
            tokenHelper: token,
        };
    }]);

//#endregion

//#region filters
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
        if (input.search('Mat:') > -1) {
            return input.replace("Mat:", "");
        }

        return input;
    };
});
//#endregion