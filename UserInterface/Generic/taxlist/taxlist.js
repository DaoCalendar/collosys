csapp.factory('taxlistDataLayer', ['$csnotify', 'Restangular',
    function ($csnotify, rest) {
        var apictrl = rest.all('ProfileApi');
        var dldata = {};



        return {
            dldata: dldata,
        };
    }
]);

csapp.controller('taxlistCtrl', ['$scope', 'taxlistDataLayer',
    function ($scope, datalayer) {
        'use strict';
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;

        })();
    }
]);
