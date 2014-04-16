csapp.factory('taxmasterDataLayer', ['$csnotify', 'Restangular',
    function ($csnotify, rest) {
        var api = rest.all('TaxMasterApi');
        var dldata = {};

        var stateList = function () {
            return api.customGET('StateList').then(function (data) {
                return data;
            });
        };
        return {
            dldata: dldata,
            stateList: stateList
        };
    }
]);

csapp.factory('taxmasterFactory', ['$csShared', function ($csShared) {

}]);

csapp.controller('taxmasterCtrl', ['$scope', 'taxmasterDataLayer', 'taxmasterFactory', '$csModels',
    function ($scope, datalayer, factory, $csModels) {
        'use strict';

        var resetTax = function () {
            $scope.tax = {
                Country: 'India',
                District: 'ALL',
                IndustryZone: 'ALL'
            };
            $scope.taxForm.$setPristine();
        };
        var initLocal = function () {
            $scope.TaxMaster = $csModels.models.Generic.TaxMaster;
            $scope.taxMasterList = [];
            $scope.tax = {
                Country: 'India',
                District: 'ALL',
                IndustryZone: 'ALL'
            };
            $scope.indexOfSelected = -1;
            $scope.isAddMode = true;
        };

        var initListFromDb = function () {
            datalayer.stateList().then(function (data) {
                $scope.TaxMaster.State.valueList = data;
            });
        };
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.factory = factory;
            initLocal();
            initListFromDb();
        })();

        $scope.add = function (tax) {
            //save tax then
            $scope.taxMasterList.push(tax);
            resetTax();
        };

        $scope.edit = function (t, index) {
            $scope.tax = angular.copy(t);
            $scope.indexOfSelected = index;
            $scope.isAddMode = false;
        };

        $scope.applyedit = function (t) {
            //save t first and then
            $scope.taxMasterList[$scope.indexOfSelected] = t;
            resetTax();
            $scope.isAddMode = true;
        };
    }
]);
