csapp.factory('holdingpolicyFactory', [
    '$csfactory',
    function ($csfactory) {
        return {

        };
    }
]);

csapp.factory('holdingpolicyDatalayer',
    ['Restangular', '$csnotify', '$csfactory',
        function (rest, $csnotify, $csfactory) {
            var restApi = rest.all("PayoutPolicyApi");
            var dldata = {};

            return {
                dldata: dldata,
            };
        }]);

csapp.controller('holdingpolicyCtrl', [
    '$scope', 'holdingpolicyDatalayer', 'holdingpolicyFactory',
    function ($scope, datalayer, factory) {

        (function () {
            $scope.factory = factory;
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
        })();
    }]);