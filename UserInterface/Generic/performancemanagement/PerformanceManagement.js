csapp.factory("PerformanceManagemnetDatalayer", ["Restangular", function (rest) {

    var restapi = rest.all("PerformanceManagementApi");

    var save = function (performanceMgt) {
        return restapi.customPOST(performanceMgt, "Saveperformanace");
    };

    return {
        Save: save
    };
}]);

csapp.controller("PerformanaceManagementCtrl", ["$scope", "PerformanceManagemnetDatalayer", "$csShared", "$csnotify", function ($scope, datalayer, $csShared, $csnotify) {

    (function () {
        $scope.PerformanceModel = {
            Product: { type: "enum", label: "Product" },
            Weightage: { type: "number", template: "percentage" },
        };
        $scope.PerformanceModel.Product.valueList = $csShared.enums.Products;
        $scope.ParamList = [];
        $scope.performanceMgt = {
            PerformanceParamses: [],
            Products: ""
        };
        $scope.PerformanceParam = {
            Param: [],
            Weightage: []
        };
    })();


    $scope.save = function (performanceParam) {
        var sum = 0;
        _.forEach(performanceParam.Weightage, function (row) {
            sum = sum + row;
        });
        if (sum === 100) {
            performanceParam.Param = $scope.ParamList;
            for (var i = 0; i < performanceParam.Param.length; i++) {
                var item = {
                    Param: performanceParam.Param[i],
                    Weightage: performanceParam.Weightage[i]
                };
                $scope.performanceMgt.PerformanceParamses.push(item);
            }
            return datalayer.Save($scope.performanceMgt).then(function (data) {
                $scope.performanceMgt = {};
                $csnotify.success("Performance Saved.....!!");
            });

        } else {
            $csnotify.success("Total weightage should be exact 100%");
        }
    };

    $scope.cancel = function() {
        $scope.performanceMgt = {};
        $scope.PerformanceParam = {
            Param: [],
            Weightage: []
        };
    };
    $scope.setParams = function (product) {
        switch (product) {
            case 'AUTO':
                $scope.ParamList = $csShared.enums.Param_Auto;
                break;
            case 'MORT':
                $scope.ParamList = $csShared.enums.Param_MORT;
                break;
            case 'AUTO_OD':
                $scope.ParamList = $csShared.enums.Param_Auto_OD;
                break;
            case 'CC':
                $scope.ParamList = $csShared.enums.Param_CC;
                break;
            case 'PL':
                $scope.ParamList = $csShared.enums.Param_PL;
                break;
            case 'SMC':
                $scope.ParamList = $csShared.enums.Param_SMC;
                break;
            case 'SME_BIL':
                $scope.ParamList = $csShared.enums.Param_SME_BIL;
                break;
            case 'SME_LAP':
                $scope.ParamList = $csShared.enums.Param_SME_LAP;
                break;
            case 'SME_LAP_OD':
                $scope.ParamList = $csShared.enums.Param_SME_LAP_OD;
                break;
            case 'SME_ME':
                $scope.ParamList = $csShared.enums.Param_SME_ME;
                break;
           default:
                $scope.ParamList = [];
        }

    };
}]);