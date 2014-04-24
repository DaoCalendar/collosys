csapp.factory('holdingactiveFactory', [
    '$csfactory',
    function ($csfactory) {
        return {
        
        };
    }
]);

csapp.factory('holdingactiveDatalayer',
    ['Restangular', '$csnotify', '$csfactory',
        function (rest, $csnotify, $csfactory) {
    var restApi = rest.all("PayoutPolicyApi");
    var dldata = {};

    return {
        dldata: dldata,
    };
}]);

csapp.controller('holdingactiveCtrl', [
    '$scope', 'holdingactiveDatalayer', 'holdingactiveFactory',
    function ($scope, datalayer, factory) {

        (function () {
            $scope.factory = factory;
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
        })();
    }]);