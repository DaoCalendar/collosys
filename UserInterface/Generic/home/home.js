csapp.controller("HomeCtrl", ['$scope', '$csnotify', '$csfactory', 'Restangular',
    function ($scope, $csnotify, $csfactory, rest) {

        var restApi = rest.all("HomeApi");

        $scope.changeFunc = function (data) {

            $scope.changed = data;
        };

        (function () {
            $scope.Field = {};
            $scope.number = { label: 'Age', required: true, type: 'uint'};
            $scope.text = { label: 'User', required: true, template: 'user', type: 'text', minlength: 5 };
            $scope.phone = { label: 'phone', required: true, template: 'phone', type: 'text', minlength: 5 };

            restApi.customGET("GetData", { 'currentUser': $csfactory.getCurrentUserName() }).then(function (data) {
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
