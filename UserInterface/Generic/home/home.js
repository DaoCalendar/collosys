csapp.controller("HomeCtrl", ['$scope', '$csnotify','$csfactory',
    function ($scope, $csnotify,$csfactory) {

        var restApi = rest.all("HomeApi");
        
        (function () {
            restApi.customGET("GetData",{'currentUser':$csfactory.getCurrentUserName()}).then(function (data) {
                $scope.datalist = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });

        })();


        $scope.showPendingOptions = function () {
            if (angular.isUndefined($scope.datalist) || $scope.datalist.stakeholder != 0 || $scope.datalist.payment != 0 || $scope.datalist.working != 0 || $scope.datalist.allocation != 0 || $scope.datalist.allocationpolicy != 0)
                return true;
            else return false;
        };

    }

]);
