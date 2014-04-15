csapp.factory('taxlistDataLayer', ['$csnotify', 'Restangular',
    function ($csnotify, rest) {
        var apictrl = rest.all('ProfileApi');
        var dldata = {};



        return {
            dldata: dldata,
        };
    }
]);

csapp.factory('taxListFactory', ['$csShared', function ($csShared) {
    var taxTypeEnum = $csShared.enums.TaxType;
    var taxApplicableToEnum = $csShared.enums.TaxApplicableTo;

    var initEnumsList = function(taxList) {
        taxList.TaxType.valueList = taxTypeEnum;
        taxList.ApplicableTo.valueList = taxApplicableToEnum;
    };
    return {
        taxTypeEnum: taxTypeEnum,
        taxApplicableToEnum: taxApplicableToEnum,
        initEnumsList: initEnumsList
    };
}]);

csapp.controller('taxlistCtrl', ['$scope', 'taxlistDataLayer', 'taxListFactory', '$csModels',
    function ($scope, datalayer, factory, $csModels) {
        'use strict';
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.factory = factory;
            $scope.TaxList = $csModels.models.Generic.TaxList;
            factory.initEnumsList($scope.TaxList);
        })();
    }
]);
