
csapp.factory('holdingactiveDatalayer',
    ['Restangular', '$csnotify',
        function (rest, $csnotify) {
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
                return restApi.customDELETE('Delete', { id: policy.Id }).then(function (data) {
                    $csnotify.success('data deleted successfully');
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
    '$scope', 'holdingactiveDatalayer', '$location',
    function ($scope, datalayer, $location) {

        var initlocals = function () {
            $scope.policyList = [];
            $scope.indexOfSelected = -1;
            $scope.search = {};
            $scope.active = {};
        };

        $scope.delete = function (policy, index) {
            datalayer.deleteData(policy).then(function () {
                $scope.policyList.splice(index, 1);
            });

        };

        (function () {
            initlocals();
            datalayer.getList().then(function (data) {
                $scope.policyList = data;
            });
        })();

        $scope.showAddEditPopup = function (mode) {
                $location.path("/billing/holdingactive/addedit/" + mode);
        };
    }]);

csapp.controller('holdingactiveAddEditCtrl', [
    '$scope', 'holdingactiveDatalayer','$csModels','$location','$routeParams',
    function ($scope, datalayer, $csModels, $location, $routeParams) {

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
                $location.path("/billing/holdingactive");
            });
        };

        $scope.pageData = function (product) {
            datalayer.pageData(product).then(function (data) {
                $scope.ActPolicy.Stakeholder.valueList = data.Stakeholders;
                $scope.ActPolicy.HoldingPolicy.valueList = data.HoldingPolicies;
            });
        };
        
        $scope.close = function () {
            $location.path("/billing/holdingactive");
        };

        (function () {
            $scope.active = {};
            $scope.ActPolicy = $csModels.getColumns("ActivateHoldingPolicy");
            calculateMonthList();
            initlocals();
            //datalayer.getList().then(function (data) {
            //    $scope.policyList = data;
            //});
        })();
        
        (function (mode) {
            switch (mode) {
                case "add":
                    $scope.modelTitle = "Add Holding Policy";
                    break;
                default:
                    throw ("Invalid display mode : " + JSON.stringify(policy));
            }
            $scope.mode = mode;
        })($routeParams.mode);
    }]);