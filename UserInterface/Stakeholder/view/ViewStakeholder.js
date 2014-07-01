
csapp.factory('ViewStakeholderDatalayer', ["Restangular", function (rest) {

    var restApi = rest.all('ViewStakeApi');

    var getFilteredList = function (filterParam) {
        return restApi.customPOST(filterParam, "GetFilteredList");
    };

    var searchStakeholder = function (param) {
        return restApi.customGET('GetStakeById', { 'param': param });
    };

    return {
        GetFilteredList: getFilteredList,
        SearchStakeholder: searchStakeholder
    };

}]);

csapp.controller('viewStake', [
    '$scope', '$log', 'ViewStakeholderDatalayer', '$location', "$timeout", "$csnotify", 'ngTableParams', "$filter",
    function ($scope, $log, datalayer, $location, $timeout, $csnotify, ngTableParams, $filter) {

        var getPagedData = function () {
            var params = {
                page: $scope.tableParams.page(),
                size: $scope.tableParams.count(),
                name: $scope.filter.name,
                filter: $scope.filter.search
            };

            return datalayer.GetFilteredList(params).then(function (data) {
                $scope.stakeholders = data.Data;
                if (data.length === 0) {
                    $csnotify.success("stakeholders not found");
                };
                return data;
            });

        };

        $scope.tableParams = new ngTableParams({
            page: 1,
            count: 10,
            sorting: {
                name: 'asc'
            }
        }, {
            total: function () { return getData().length; },
            getData: function ($defer, params) {
                getPagedData().then(function (data) {
                    params.total(data.Count);
                    var orderedData = params.sorting() ?
                                   $filter('orderBy')(data.Data, params.orderBy()) :
                                   data.Data;
                    $defer.resolve(orderedData);
                });
            },
            $scope: { $data: {} }
        });

        (function () {
            $scope.fields = {
                filters: { type: 'enum', label: 'View' },
                Search: { placeholder: "enter ID/Name to edit", type: 'text' }
            };
            $scope.filter = {
                name: "Approved"
            };

            $scope.filterList = ['All', 'Approved', 'Unapproved'];
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

        $scope.searchStake = function (param) {
            if (param.length < 3) { return; }
            $scope.tableParams.reload();
        };

        $scope.getStakeholders = function () {
            $scope.tableParams.reload();
        };
    }
]);
