
csapp.factory('ViewStakeholderDatalayer', ["Restangular", function (rest) {

    var restApi = rest.all('ViewStakeApi');

    var getStakeholder = function () {
        return restApi.customGET('GetAllStakeHolders');
    };

    return {
        GetStakeholder: getStakeholder
    };

}]);

csapp.controller('viewStake', [
    '$scope', '$log', 'ViewStakeholderDatalayer', '$location',
    function ($scope, $log, datalayer, $location) {

        var getAllStakeholders = function () {
            return datalayer.GetStakeholder().then(function (data) {
                $scope.stakeholders = data;
            });
        };

        (function () {
            getAllStakeholders();
        })();

        $scope.switchPage = function (data, page) {
            switch (page.toUpperCase()) {
                case 'BASIC':
                    $location.path('/stakeholder/add/' + data.Id);
                    break;
                case 'PAYMENT':
                case 'WORKING':
                    $location.path('/stakeholder/working/edit/' + data.Id);
                    break;
                default:
                    throw "invalid page name " + page;
            }
        };
    }
]);
