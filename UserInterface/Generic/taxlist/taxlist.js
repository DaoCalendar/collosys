csapp.factory('taxlistDataLayer', ['$csnotify', 'Restangular','$csfactory',
    function ($csnotify, rest,$csfactory) {
        var api = rest.all('TaxListApi');
        var dldata = {};
        
        var initTaxList = function () {
        };

        var create = function (tax) {
            return api.post(tax).then(function (data) {
                $csnotify.success('data saved');
                return;
            });
        };

        var save = function(tax) {
            return tax.put().then(function (data) {
                $csnotify.success('data saved');
                return;
            });
        };
        var getList = function() {
            return api.getList().then(function (data) {
                return data;
            });
        };
        return {
            dldata: dldata,
            save: save,
            create:create,
            getList: getList
        };
    }
]);

csapp.controller('taxlistCtrl', ['$scope', 'taxlistDataLayer', '$csModels',
    function ($scope, datalayer, $csModels) {
        'use strict';
        $scope.resetTax = function() {
            $scope.tax = {};
            $scope.taxForm.$setPristine();
        };
        var initLocal = function() {
            $scope.TaxList = $csModels.models.Generic.TaxList;
            $scope.taxList = [];
            $scope.tax = {};
            $scope.indexOfSelected = -1;
            $scope.isAddMode = true;
            $scope.search = {};
        };
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            initLocal();
            datalayer.getList().then(function (data) {
                $scope.taxList = data;
            });
        })();

        $scope.add = function(tax) {
            //save tax then
            datalayer.create(tax).then(function() {
                $scope.taxList.push(tax);
                $scope.resetTax();
            });
        };

        $scope.edit = function(t, index) {
            $scope.tax = angular.copy(t);
            $scope.indexOfSelected = index;
            $scope.isAddMode = false;
        };

        $scope.applyedit = function (t) {
            datalayer.create(t).then(function () {
                $scope.taxList[$scope.indexOfSelected] = t;
                $scope.resetTax();
                $scope.isAddMode = true;
            });
        };
    }
]);
