csapp.factory("PerformanceManagemnetDatalayer", ["Restangular", function (rest) {

    var restapi = rest.all("PerformanceManagementApi");

    var save = function (performanceMgt) {
        return restapi.customPOST(performanceMgt, "Saveperformanace");
    };

    var fetchParams = function (product) {
        return restapi.customGET("FetchParams", { 'Products': product });
    };

    return {
        Save: save,
        fetchParams: fetchParams
    };
}]);

csapp.controller("PerformanaceParameterCtrl", ["$scope", "PerformanceManagemnetDatalayer", "$csShared", "$csnotify", function ($scope, datalayer, $csShared, $csnotify) {

    (function () {
        $scope.PerformanceModel = {
            Product: { type: "enum", label: "Product" },
            Weightage: { type: "number", template: "percentage" },
            Param: { type: "text" },
            Total: { type: "number" }
        };
        $scope.PerformanceModel.Product.valueList = $csShared.enums.Products;
        $scope.performanceMgt = [];
        $scope.PerformParam = {
            total: 0,
            Products: ""
        };
    })();

    $scope.totalWeightage = function (performanceParam) {
        $scope.PerformParam.total = 0;
        _.forEach(performanceParam, function (row) {
            $scope.PerformParam.total = $scope.PerformParam.total + row.Weightage;
        });
    };
    $scope.save = function (performanceParam) {
        if ($scope.PerformParam.total === 100) {
            return datalayer.Save(performanceParam).then(function () {
                $scope.performanceMgt = [];
                $scope.PerformParam = {};
                $csnotify.success("Performance Saved.....!!");
            });
        } else {
            $csnotify.error("Total weightage should be exact 100%");
        }
    };

    $scope.fetchParams = function (product) {
        datalayer.fetchParams(product).then(function (data) {
            $scope.performanceMgt = data;
            $scope.totalWeightage($scope.performanceMgt);
        });
    };
    $scope.cancel = function () {
        $scope.performanceMgt = [];
        $scope.PerformParam = {};
    };

}]);