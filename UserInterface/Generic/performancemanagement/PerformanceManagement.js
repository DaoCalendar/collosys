csapp.factory("PerformanceManagemnetDatalayer", ["Restangular", function (rest) {

    var dldata = {};
    var restapi = rest.all("");

    var save = function(performanceMgt) {
        return restapi.customPOST("Saveperformanace", performanceMgt);
    };

    return {
        dldata: dldata,
        Save:save
    };
}]);

csapp.controller("PerformanaceManagementCtrl", ["$scope", "PerformanceManagemnetDatalayer", "$csShared", function ($scope, datalayer, $csShared) {

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
            Param:[],
            Weightage: []
        };
    })();


    $scope.save = function (performanceParam) {
        performanceParam.Param = $scope.ParamList;
        for (var i = 0; i < performanceParam.Param.length; i++) {
            var item = {
                Param: performanceParam.Param[i],
                Weightage: performanceParam.Weightage[i]
            };
            $scope.performanceMgt.PerformanceParamses.push(item);
        }
        return datalayer.Save($scope.performanceMgt).then(function (data) {
            
        });


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
            default:
                $scope.ParamList = ["SME_ME_Param1", "SME_ME_Param2", "SME_ME_Param3", "SME_ME_Param4"];
        }

    };
}]);