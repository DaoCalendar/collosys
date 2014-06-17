csapp.factory('holdingpolicyDatalayer', ['Restangular', '$csnotify',
        function (rest, $csnotify) {
            var api = rest.all('HoldingPolicyApi');
            var dldata = {};

            var save = function (policy) {
                return api.post(policy).then(function (data) {
                    $csnotify.success('data saved');
                    return data;
                });
            };
            var getList = function () {
                return api.getList().then(function (data) {
                    return data;
                });
            };
            var getHoldingPolicy = function (detailsid) {
                return api.customGET('Get', { id: detailsid })
                    .then(function (data) {
                        return data;
                    }, function () {
                        $csnotify.error('error in saving hierarchy');
                    });
            };

            return {
                dldata: dldata,
                save: save,
                //create: create,
                getList: getList,
                Get: getHoldingPolicy,
            };
        }]);

csapp.controller('holdingpolicyCtrl', [
    '$scope', 'holdingpolicyDatalayer', '$csModels', '$location',
    function ($scope, datalayer, $csModels, $location) {

        var initLocal = function () {
            $scope.policyList = [];
            $scope.policy = {};
            $scope.indexOfSelected = -1;
            $scope.search = {};
        };

        (function () {
            initLocal();
            datalayer.getList().then(function (data) {
                $scope.policyList = data;
            });
        })();

        $scope.showAddEditPopup = function (mode, policy) {
            if (mode === "edit" || mode === "view") {
                $location.path("/billing/holdingpolicy/addedit/" + mode + "/" + policy.Id);
            } else {
                $location.path("/billing/holdingpolicy/addedit/" + mode);
            }
        };
    }]);


csapp.controller('holdingpolicyAddEditCtrl', [
    '$scope', 'holdingpolicyDatalayer', '$csModels', '$csnotify', '$routeParams', '$location',
    function ($scope, datalayer, $csModels, $csnotify, $routeParams, $location) {


        $scope.reset = function () {
            $scope.policy = {};
            $scope.holdingpolicyform.$setPristine();
        };

        var initLocal = function () {
            $scope.PolicyList = $csModels.getColumns("HoldingPolicy");
            $scope.policyList = [];
            $scope.policy = {};
            $scope.indexOfSelected = -1;
        };

        $scope.applyedit = function (t) {
           return datalayer.save(t).then(function (data) {
                $scope.policyList[$scope.indexOfSelected] = data;
                $scope.reset();
                $location.path("/billing/holdingpolicy");
            });
        };

        $scope.dateValiadation = function (startDate, endDate) {
            if (!(endDate === null) && startDate >= endDate) {
                $csnotify.success("EndDate should be greater than StartDate");
                $scope.policy.EndDate = null;
            }
        };

        $scope.close = function () {
            $location.path("/billing/holdingpolicy");
        };

        (function () {

            if (angular.isDefined($routeParams.id)) {
                datalayer.Get($routeParams.id).then(function (data) {
                    $scope.policy = data;
                });
            } else {
                $scope.policy = {};
            }
            $scope.policy = {};
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.HoldingPolicy = $csModels.getColumns("HoldingPolicy");
            initLocal();

            datalayer.getList().then(function (data) {
                $scope.policyList = data;
            });
        })();

        (function (mode) {
            switch (mode) {
                case "add":
                    $scope.modelTitle = "Add Holding Policy";
                    break;
                case "edit":
                    $scope.modelTitle = "Update Holding Policy";
                    break;
                case "view":
                    $scope.modelTitle = "View Holding Policy";
                    break;
                default:
                    throw ("Invalid display mode : " + JSON.stringify(policy));
            }
            $scope.mode = mode;
        })($routeParams.mode);

    }]);



//$scope.add = function (policy) {
//    //save tax then
//    datalayer.create(policy).then(function (data) {
//        $scope.policyList.push(data);
//        $scope.reset();
//    });
//};


//$scope.edit = function (policy, index) {
//    $scope.policy = angular.copy(policy);
//    $scope.indexOfSelected = index;
//    $scope.isAddMode = false;
//};

//var save = function (policy) {
//    return policy.put().then(function (data) {
//        $csnotify.success('data saved');
//        return data;
//    });
//};

//in main ctrl

//$scope.datalayer = datalayer;
//$scope.dldata = datalayer.dldata;
//$scope.HoldingPolicy = $csModels.getColumns("HoldingPolicy");