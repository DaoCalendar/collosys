
csapp.factory('ViewStakeholderDatalayer', ["Restangular", function (rest) {

    var restApi = rest.all('ViewStakeApi');

    var getFilteredList = function (filterParam) {
        return restApi.customGET("GetFilteredList", { "filterParam": filterParam });
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
                page: $scope.tableParams.page,
                size: $scope.tableParams.count,
                name: $scope.filter.name,
                filter: $scope.filter.search
            };

            return datalayer.GetFilteredList("All").then(function (data) {
                $scope.stakeholders = data;
                if (data.length === 0) {
                    $csnotify.success("stakeholders not found");
                };
                return data;
            });

        };

        $scope.tableParams = new ngTableParams({
            page: 1,            // show first page
            count: 10,          // count per page
            sorting: {
                name: 'asc'     // initial sorting
            }
        }, {
            total: function () { return getData().length; }, // length of data
            getData: function ($defer, params) {
                getPagedData().then(function (data) {
                    params.total(200);
                    var orderedData = params.sorting() ?
                                   $filter('orderBy')(data, params.orderBy()) :
                                   data;
                    $defer.resolve(orderedData.slice((params.page() - 1) * params.count(),
                        params.page() * params.count()));
                });
            },
            $scope: { $data: {} }
        });

        (function () {
            $scope.fields = {
                filters: { type: 'enum', label: 'View' },
                Search: { placeholder: "enter ID/Name to edit", type: 'text' }
            };
            $scope.filter = {};
            $scope.filterList = ['All', 'Approved', 'Unapproved', "Search"];
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
            $scope.filter.name = "Search";
            $timeout(function () {
                datalayer.SearchStakeholder(param).then(function (data) {
                    $scope.stakeholders = data;
                });
            }, 400);
        };

        $scope.getStakeholders = function (filterParam) {
            $scope.tableParams.reload();
            //if (filterParam === 'Search') return;
            //datalayer.GetFilteredList(filterParam).then(function (data) {
            //    $scope.stakeholders = data;
            //    if (data.length === 0) {
            //        $csnotify.success("stakeholders not found");
            //    } else {
            //        $scope.tableParams.reload();
            //    }
            //});
        };
    }
]);
