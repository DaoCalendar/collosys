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
            var api = rest.all('HoldingPolicyApi');
            var dldata = {};

            var create = function (policy) {
                return api.post(policy).then(function (data) {
                    $csnotify.success('data saved');
                    return data;
                });
            };

            var save = function (policy) {
                return policy.put().then(function (data) {
                    $csnotify.success('data saved');
                    return data;
                });
            };
            var getList = function () {
                return api.getList().then(function (data) {
                    return data;
                });
            };
            return {
                dldata: dldata,
                save: save,
                create: create,
                getList: getList
            };
        }]);

csapp.controller('holdingpolicyCtrl', [
    '$scope', 'holdingpolicyDatalayer', 'holdingpolicyFactory','$csModels',
    function ($scope, datalayer, factory, $csModels) {

       
        
        $scope.reset = function () {
            $scope.policy = {};
            $scope.holdingpolicyform.$setPristine();
        };
        var initLocal = function () {
            $scope.PolicyList = $csModels.models.Billing.HoldingPolicy;
            $scope.policyList = [];
            $scope.policy = {};
            $scope.indexOfSelected = -1;
            $scope.isAddMode = true;
            $scope.search = {};
        };
        
        $scope.add = function (policy) {
            //save tax then
            datalayer.create(policy).then(function (data) {
                $scope.policyList.push(data);
                $scope.reset();
            });
        };

        $scope.edit = function (policy, index) {
            $scope.policy = angular.copy(policy);
            $scope.indexOfSelected = index;
            $scope.isAddMode = false;
        };

        $scope.applyedit = function (t) {
            
            datalayer.create(t).then(function (data) {
                $scope.policyList[$scope.indexOfSelected] = data;
                $scope.reset();
                $scope.isAddMode = true;
            });
        };

        (function () {
            $scope.factory = factory;
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.HoldingPolicy = $csModels.models.Billing.HoldingPolicy;
            initLocal();
            
            datalayer.getList().then(function (data) {
                $scope.policyList = data;
            });
        })();
    }]);