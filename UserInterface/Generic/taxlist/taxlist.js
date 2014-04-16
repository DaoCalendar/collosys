csapp.factory('taxlistDataLayer', ['$csnotify', 'Restangular',
    function ($csnotify, rest) {
        var apictrl = rest.all('ProfileApi');
        var dldata = {};
        var initTaxList = function () {
        };

        return {
            dldata: dldata,
        };
    }
]);

csapp.factory('taxListFactory', ['$csShared', function ($csShared) {
    var initEnumsList = function(taxList) {
        taxList.TaxType.valueList = $csShared.enums.TaxType;
        taxList.ApplicableTo.valueList = $csShared.enums.TaxApplicableTo;
        taxList.ApplyOn.valueList = $csShared.enums.TaxApplyOn;
    };
    return {
        initEnumsList: initEnumsList
    };
}]);

csapp.controller('taxlistCtrl', ['$scope', 'taxlistDataLayer', 'taxListFactory', '$csModels',
    function ($scope, datalayer, factory, $csModels) {
        'use strict';
        var resetTax = function() {
            $scope.tax = {};
            $scope.taxForm.$setPristine();
        };
        var initLocal = function() {
            $scope.TaxList = $csModels.models.Generic.TaxList;
            $scope.taxList = [];
            $scope.tax = {};
            $scope.indexOfSelected = -1;
            $scope.isAddMode = true;
        };
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.factory = factory;
            initLocal();
            factory.initEnumsList($scope.TaxList);
        })();

        $scope.add = function(tax) {
            //save tax then
            $scope.taxList.push(tax);
            resetTax();
        };

        $scope.edit = function(t, index) {
            $scope.tax = angular.copy(t);
            $scope.indexOfSelected = index;
            $scope.isAddMode = false;
        };

        $scope.applyedit = function (t) {
            //save t first and then
            $scope.taxList[$scope.indexOfSelected] = t;
            resetTax();
            $scope.isAddMode = true;
        };
    }
]);
