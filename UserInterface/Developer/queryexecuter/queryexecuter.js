
csapp.factory("queryExecuterDataLayer", [
    "Restangular", function (rest) {

        var restApi = rest.all("ExecuteQueryApi");

        var executeQuery = function (query) {
            return restApi.customPOST({ 'Query' : query }, "CheckandExecute");
        };

        return {
            executeQuery: executeQuery
        };
    }
]);

csapp.factory("queryExecuterFactory", ["queryExecuterDataLayer", "$csfactory",
    function (datalayer, $csfactory) {

    var addQueryToList = function (query, list) {
        if ($csfactory.isNullOrEmptyString(query))
            return list;
        query = query.toLowerCase().trim();
        if ($csfactory.isNullOrEmptyArray(list)) {
            list = [];
            list.push(query);
            return list;
        }

        var result = _.find(list, function (item) {
            return item == query;
        });

        if (angular.isUndefined(result)) {
            list.push(query);
        }

        return list;
    };

    var getQueryType = function (query) {
        return query.trim().substring(0, 3).toUpperCase();
    };

    var getProperties = function (queryResult) {
        if ($csfactory.isNullOrEmptyArray(queryResult) || !angular.isArray(queryResult)) {
            return [];
        }

        var obj = queryResult[0];
        var properties = [];
        for (var key in obj) {
            if (obj.hasOwnProperty(key) && typeof obj[key] !== 'function') {
                properties.push(key);
            }
        }

        return properties;
    };

    var getGridColumns = function (properties) {
        var columnDefinition = [];
        for (var i = 0; i < properties.length; i++) {
            var col = {};
            col.displayName = properties[i];
            col.width = 200;
            columnDefinition.push(col);
            return columnDefinition;
        }
        return columnDefinition;
    };

    return {
        addQueryToList: addQueryToList,
        getQueryType: getQueryType,
        getProperties: getProperties,
        getGridColumns: getGridColumns
    };
}]);

csapp.controller("queryExecuterController", ["$scope", "queryExecuterDataLayer", "queryExecuterFactory",
    function ($scope, datalayer, factory) {

        $scope.gridOptions = {
            data: 'tableData',
            columnDefs: 'columnDefinition'
        };

        $scope.executeQuery = function (query) {
            if (query.length < 5) return;
            $scope.tableData = [];

            datalayer.executeQuery(query).then(function (queryResult) {
                $scope.view = 'viewData';
                $scope.queryList = factory.addQueryToList(query, $scope.queryList);
                $scope.tableData = queryResult;
                $scope.queryType = factory.getQueryType(query);
                $scope.properties = factory.getProperties(queryResult);
                $scope.columnDefinition = factory.getGridColumns($scope.properties);
            }, function (error) {
                $scope.view = 'viewError';
                $scope.queryError = error;
            });
        };

        $scope.deleteQuery = function (data) {
            $scope.queryList.splice($scope.queryList.indexOf(data), 1);
        };
    }
]);