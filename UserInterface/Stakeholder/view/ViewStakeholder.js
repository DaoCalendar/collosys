
csapp.factory('ViewStakeholderDatalayer', ["Restangular", function (rest) {

    var restApi = rest.all('ViewStakeApi');

    var getStakeholder = function () {
        return restApi.customGET('GetAllStakeHolders');
    };

    var getFilteredList = function (filterParam) {
        return restApi.customGET("GetFilteredList", { "filterParam": filterParam });
    };

    return {
        GetStakeholder: getStakeholder,
        GetFilteredList: getFilteredList
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
            $scope.fields = {
                filters: { type: 'enum', label: 'Filters' }
            };
            $scope.filterList = ['Approved', 'Unapproved'];
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

        $scope.getStakeholders = function (filterParam) {
            datalayer.GetFilteredList(filterParam).then(function (data) {
                $scope.stakeholders = data;
            });
        };
    }
]);
