csapp.controller("RequisitionCtrl", ["$scope", "$csModels",
    function ($scope,$csModels) {
        (function () {
            $scope.legal = {};
            $scope.legalpre = $csModels.getColumns("RequisitionPreparation");
        })();
    
    }]);