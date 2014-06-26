csapp.factory("PerformanceManagemnetDatalayer", ["Restangular", function (rest) {

    var restapi = rest.all("PerformanceManagementApi");

    var save = function (performanceMgt) {
        return restapi.customPOST(performanceMgt, "Saveperformanace");
    };

    return {
        Save: save
    };
}]);

csapp.controller("PerformanaceParameterCtrl", ["$scope", "PerformanceManagemnetDatalayer", "$csShared", "$csnotify", function ($scope, datalayer, $csShared, $csnotify) {

    (function () {
        $scope.PerformanceModel = {
            Product: { type: "enum", label: "Product" },
        };
        $scope.PerformanceModel.Product.valueList = $csShared.enums.Products;
        $scope.ParamList = $csShared.enums.PerformanceParam;
        $scope.performanceMgt = [];
        $scope.PerformanceParam = {
            Param: [],
            Weightage: [],
            Products: ""
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
                    Weightage: performanceParam.Weightage[i],
                    Products: performanceParam.Products
                };
                $scope.performanceMgt.push(item);
            }
            return datalayer.Save($scope.performanceMgt).then(function (data) {
                $scope.performanceMgt = [];
                $scope.PerformanceParam = {
                    Param: [],
                    Weightage: [],
                    Products: ""
                };
                $csnotify.success("Performance Saved.....!!");
            });

        } else {
            $csnotify.error("Total weightage should be exact 100%");
        }
    };

    $scope.cancel = function () {
        $scope.performanceMgt = [];
        $scope.PerformanceParam = {
            Param: [],
            Weightage: [],
            Products: ""
        };
    };
 
}]);