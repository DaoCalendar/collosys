/// <reference path="../../../Scripts/angular.js" />
/// <reference path="~/Scripts/lodash.js" />

(csapp.controller("matrixCtrl", ["$scope", "$csfactory", "$csnotify", "$Validations", "Restangular", function ($scope, $csfactory, $csnotify, $validation, rest) {
    "use strict";

    var restApi = rest.all("MatrixApi");
    $scope.matrixList = [];
    $scope.productsList = [];
    $scope.columnNames = [];
    $scope.formulaNames = [];
    $scope.matrix = {};
    $scope.matrix.BMatricesValues = [];
    $scope.isMatrixCreated = false;
    $scope.matrix.Category = "Liner";
    $scope.matrix.Row1DType = "Table";
    $scope.matrix.Column2DType = "Table";
    $scope.matrix.Row3DType = "Table";
    $scope.matrix.Column4DType = "Table";
    $scope.categorySwitch = [{ Name: 'Collection', Value: 'Liner' }, { Name: 'Recovery', Value: 'WriteOff' }];
    $scope.typeSwitch = [{ Name: 'Table', Value: 'Table' }, { Name: 'Formula', Value: 'Formula' }];
    $scope.val = $validation;
    var columnDef = [];
    $scope.isDuplicate = false;
    $scope.rowType;
    $scope.columnType;
    restApi.customGET("GetProducts").then(function (data) {
        $scope.productsList = data;
    }, function (data) {
        $csnotify.error(data);
    });

    $scope.selectMatrix = function (smatrix) {
        $scope.isMatrixCreated = false;
        $scope.matrix = {};
        $scope.matrix.BMatricesValues = [];
        restApi.customGET("GetMatrixValues", { matrixId: smatrix.Id }).then(function (data) {
            $scope.isMatrixCreated = true;
            $scope.matrix = angular.copy(smatrix);
            $scope.matrix.BMatricesValues = data;
        }, function (data) {
            $csnotify.error(data);
        });
    };


    $scope.buttonDisable = function (dimension) {
        switch (dimension) {
            case "2":
                if (!$csfactory.isNullOrEmptyString($scope.matrix.Row1DTypeName))
                    if ($scope.matrix.Row1DTypeName === $scope.matrix.Column2DTypeName)
                        return true;
                break;
        }
    };

    $scope.changeProductCategory = function () {
        $scope.isDuplicate = false;
        var matrix = $scope.matrix;
        if ($scope.isMatrixCreated == true)
            return;

        if (!angular.isUndefined(matrix.Products) && !angular.isUndefined(matrix.Category)) {

            // get matrix
            restApi.customGET("GetMatrix", { product: matrix.Products, category: matrix.Category }).then(function (data) {
                $scope.matrixList = data;
            }, function (data) {
                $csnotify.error(data);
            });

            //get column names
            restApi.customGET("GetColumnNames", { product: matrix.Products, category: matrix.Category }).then(function (data) {
                columnDef = data;
                _.forEach(data, function (item) {
                    $scope.columnNames.push(item.field);
                });
               // $scope.columnNames = data;
            }, function (data) {
                $csnotify.error(data);
            });

            // get formula names
            restApi.customGET("GetFormulaNames", { product: matrix.Products, category: matrix.Category }).then(function (data) {
                $scope.formulaNames = data;
            }, function (data) {
                $csnotify.error(data);
            });

        } // else  {

        //    //$scope.columnNames = [];
        //    //$scope.formulaNames = [];
        //}
    };

    $scope.changeDimension = function (matrix) {
        matrix.Row1DCount = 1;
        matrix.Row1DType = "Table";
        matrix.Row1DTypeName = "";
        matrix.Column2DCount = 1;
        matrix.Column2DType = "Table";
        matrix.Column2DTypeName = "";
    };

    $scope.createMatrix = function (matrix) {
        $scope.isMatrixCreated = false;
        matrix.BMatricesValues = [];
        for (var i = 0; i < matrix.Row1DCount + 1; i++) {
            if (matrix.Dimension == 1 && i == 0) {
                continue;
            }
            for (var j = 0; j < matrix.Column2DCount + 1; j++) {
                if (i == 0 && j == 0) {
                    continue;
                }
                var matrixValue = {
                    RowNo1D: i,
                    ColumnNo2D: j,
                    RowNo3D: 0,
                    ColumnNo4D: 0,
                    Value: '',   //i + j
                    RowOperator: 'None',
                    ColumnOperator:'None'
                };
                matrix.BMatricesValues.push(angular.copy(matrixValue));
            }
        }
        $scope.isMatrixCreated = true;
    };

   

    $scope.checkDupName = function (name) {
        $scope.isDuplicate = false;
        if (!$csfactory.isNullOrEmptyString(name)) {
            _.find($scope.matrixList, function(item) {
                if (item.Name.toUpperCase() === name.toUpperCase()) {
                    $scope.isDuplicate = true;
                    return;
                } 
            });
        }
    };

    $scope.highlightSelectedMatrix = function (smatrix) {
        if (smatrix.Name.toUpperCase() === $scope.matrix.Name.toUpperCase())
            return { backgroundColor: 'rgba(195, 201, 204, 0.24)' };
    };

    $scope.getMatrixValue = function (row, col) {
        var matVal = _.find($scope.matrix.BMatricesValues, function (matrixVal) {
            return (matrixVal.RowNo1D == row && matrixVal.ColumnNo2D == col);
        });
        return matVal;
    };

    $scope.saveMatrix = function (matrix) {
        _.forEach(matrix.BMatricesValues, function (item) {
            item.ColumnOperator = operatorNameFromValue(item.ColumnOperator);
            item.RowOperator = operatorNameFromValue(item.RowOperator);
        });
        matrix.Dimension = parseInt(matrix.Dimension);
        restApi.customPOST(matrix, "Post")
            .then(function (data) {
                afterSavedMatrix(data);
            }, function (data) {
                $csnotify.error(data);
            });
    };

    $scope.highlightColumn = function (n) {
        if (n === 0) {
            return ({ backgroundColor: 'rgba(128, 128, 128, 0.29)' });
        }
    };

    var afterSavedMatrix = function (data) {
        debugger;
        $scope.matrixList = _.reject($scope.matrixList, function (mat) { return mat.Id == data.Id; });
        $scope.matrixList.push(data);
        $scope.matrix = angular.copy(data);
        $csnotify.success('Matrix Saved');
    };

    $scope.opertorValue = function (index, totalRows, selOperator) {
        if (index === totalRows) {
            return operatorsEnum[getOppositeOperator(selOperator)];
        }
        return operatorsEnum[selOperator];
    };

    $scope.setInputTypeForColumn = function (columnTypeValue) {
        var inputTypeItem = _.find(columnDef, function (item) {
            if (item.displayName === columnTypeValue)
                return item;
        });
        $scope.columnType = inputTypeItem.InputType === 'number' ? inputTypeItem.InputType : 'text';
    };
    
    $scope.setInputTypeForRow = function (rowTypeValue) {
        var inputTypeItem = _.find(columnDef, function (item) {
            if (item.displayName === rowTypeValue)
                return item;
        });
        $scope.rowType = inputTypeItem.InputType === 'number' ? inputTypeItem.InputType : 'text';
    };

    var getOppositeOperator = function (selOperator) {
        if (selOperator === 'GreaterThan') {
            return 'LessThanEqualTo';
        }
        if (selOperator === 'LessThan') {
            return 'GreaterThanEqualTo';
        }
        if (selOperator === 'EqualTo') {
            return 'EqualTo';
        }

    };

    var operatorNameFromValue = function (selValue) {
        return operatorsEnumReverse[selValue];
    };

    $scope.getInputType = function (rowIndex, columnIndex) {
        if (rowIndex === 0 && columnIndex > 0) {
            return $scope.columnType;
        }
        if (rowIndex > 0 && columnIndex === 0) {
            return $scope.rowType;
        }
        return 'text';
    };
    
    $scope.getInputPattern = function (rowIndex, columnIndex) {
        if (rowIndex === 0 && columnIndex > 0) {
            return "/^[a-zA-Z0-9]{1,100}$/";
        }
        if (rowIndex > 0 && columnIndex === 0) {
            return $scope.rowType;
        }
        return '/^[0-9]{1,7}(\.[0-9]+)?$/';
    };

    $scope.isDisabled = function(rowIndex, columnIndex, totalRows, totalColumns,matrixValue) {
        if (rowIndex === totalRows && columnIndex === 0 && matrixValue.RowOperator!=='=') {
            return true;
        }
        if (columnIndex === totalColumns && rowIndex === 0 && matrixValue.ColumnOperator !== '=') {
            return true;
        }
        return false;
    };

    $scope.setNextValue = function (rowIndex, columnIndex, totalRows, totalColumns,value) {
        var matrixValue;
        if (rowIndex === totalRows-1 && columnIndex === 0)  {
            matrixValue = $scope.getMatrixValue(rowIndex + 1, columnIndex );
            matrixValue.Value = value;
        }
        if (columnIndex === totalColumns-1 && rowIndex === 0) {
            matrixValue = $scope.getMatrixValue(rowIndex, columnIndex + 1);
            matrixValue.Value = value;
        }
    };
    var operatorsEnum = {
        GreaterThan: '>',
        LessThan: '<',
        GreaterThanEqualTo: '>=',
        LessThanEqualTo: '<=',
        EqualTo: '='
    };

    var operatorsEnumReverse = {
        '>': 'GreaterThan',
        '<': 'LessThan',
        '>=':'GreaterThanEqualTo',
        '<=': 'LessThanEqualTo',
        '=': 'EqualTo'
    };
}]));