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
            var restApi = rest.all("ActivateHoldingApi");
            var dldata = {};

            var pageData = function (products) {
                return restApi.customGET('GetPageData', { products: products }).then(function (data) {
                    return data;
                });
            };

            var create = function (policy) {
                return restApi.post(policy).then(function (data) {
                    $csnotify.success('data saved');
                    return data;
                });
            };
            var deleteData = function (policy) {
                return restApi.remove(policy).then(function (data) {
                    return;
                });
            };
            var getList = function () {
                return restApi.customGET('GetActivatePolicies').then(function (data) {
                    return data;
                });
            };

            return {
                dldata: dldata,
                pageData: pageData,
                create: create,
                getList: getList,
                deleteData: deleteData
            };
        }]);

csapp.controller('holdingactiveCtrl', [
    '$scope', 'holdingactiveDatalayer', 'holdingactiveFactory', '$csModels',
    function ($scope, datalayer, factory, $csModels) {

        var calculateMonthList = function () {
            var i = 1;
            $scope.monthList = [];
            for (var j = i; j < 6; j++) {
                var data = {
                    valuefield: moment().add('month', j).format('YYYYMM'),
                    display: moment().add('month', j).format("MMM-YYYY")
                };
                $scope.monthList.push(data);
            }
            $scope.ActPolicy.StartMonth.valueList = $scope.monthList;
        };
        
        $scope.reset = function () {
            $scope.active = {};
            $scope.activateform.$setPristine();
        };
        
        var initlocals = function () {
            $scope.policyList = [];
            $scope.indexOfSelected = -1;
            $scope.isAddMode = true;
            $scope.search = {};
            $scope.active = {};
        };

        $scope.save = function (policy) {
            datalayer.create(policy).then(function (data) {
                $scope.policyList.push(data);
                $scope.reset();
            });
        };

        $scope.delete = function (policy, index) {
            datalayer.deleteData(policy).then(function() {
                $scope.policyList.splice(index, 1);
            });
            
        };
        
        $scope.pageData = function (product) {
            datalayer.pageData(product).then(function (data) {
                $scope.ActPolicy.Stakeholder.valueList = data.Stakeholders;
                $scope.ActPolicy.HoldingPolicy.valueList= data.HoldingPolicies;
            });
        };

        (function () {
            $scope.ActPolicy = $csModels.models.Billing.ActivateHoldingPolicy;
            calculateMonthList();
            initlocals();
            datalayer.getList().then(function (data) {
                $scope.policyList = data;
            });
        })();
    }]);