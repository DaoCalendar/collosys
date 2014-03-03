(
csapp.controller('changeLeave', ['$scope', '$http', 'Restangular', '$csfactory', '$csnotify', '$Validations', '$log',
    function ($scope, $http, rest, $csfactory, $csnotify, $val, $log) {

        //init rest api
        var restApi = rest.all('ManageStakeholderApi');

        var init = function () {
            $scope.Stakeholder = {};

            $scope.StakeCounts = {
                Working: 0,
                Payment: 0
            };

            $scope.StakeholderList = [];
        };

        $scope.Stakeholders = function (name) {
            if ($csfactory.isNullOrEmptyString(name)) {
                return [];
            }

            $scope.hideSpinner();
            return restApi.customGET('GetStakeholders', { name: name }).then(function (data) {
                return $scope.StakeholderList = data;
            });
        };

        $scope.setStakeholder = function (name) {
            $scope.Stakeholder = _.find($scope.StakeholderList, { 'Name': name });
            $scope.StakeCounts.Working = $scope.Stakeholder.StkhWorkings.length;
            $scope.StakeCounts.Payment = $scope.Stakeholder.StkhPayments.length;

        };

        //call init
        init();
    }])
);