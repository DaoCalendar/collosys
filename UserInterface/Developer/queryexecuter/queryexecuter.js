
csapp.controller("queryExecuterController", ["$scope", "Restangular", "$csnotify", "$csfactory", function ($scope, rest, $csnotify, $csfactory) {

    var restApi = rest.all("ExecuteQueryApi");
    $scope.gridOptions = {
        data: 'tableData',
        columnDefs: 'columnDefinition'
    };
    $scope.columnDefinition = [];
    $scope.properties = [];
    $scope.queryList = [];
    $scope.executeQuery = function (data) {
        if (angular.isDefined(data)) {
            $scope.query = data;
        }
        if ($scope.query.length > 6) {
            restApi.customGET("CheckandExecute", { query: $scope.query }).then(function (queryResult) {
                debugger;
                $scope.properties = [];
                $scope.columnDefinition = [];
                $scope.tableData = queryResult;
                if (angular.isArray(queryResult)) {
                    var obj = queryResult[0];
                    //enlisting properties of an object
                    for (var key in obj) {
                        if (obj.hasOwnProperty(key) && typeof obj[key] !== 'function') {
                            $scope.properties.push(key);
                        }
                    }

                    //creating an array for columnDefs of ng-grid
                    for (var i = 0; i < $scope.properties.length; i++) {
                        var col = {};
                        col.displayName = $scope.properties[i];
                        col.width = 200;
                        $scope.columnDefinition.push(col);
                    }
                }
                $scope.queryType = $scope.query.substring(0, 3).toUpperCase();

                var dupQueryCheck = _.find($scope.queryList, function (item) {
                    if (item === $scope.query)
                        return item;
                });
                if (angular.isDefined(dupQueryCheck)) {
                    $scope.queryList.splice($scope.queryList.indexOf($scope.query), 1);
                    $scope.queryList.push($scope.query);
                    $scope.queryList.reverse();
                } else {
                    $scope.queryList.push($scope.query);
                    $scope.queryList.reverse();
                }

                $scope.view = 'viewData';

            });
        } else $csnotify.error("INVALID QUERY");


    };
    $scope.deleteQuery = function (data) {
        $scope.queryList.splice($scope.queryList.indexOf(data), 1);
    };
}])
;