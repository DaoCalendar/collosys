/*global csapp*/

(
csapp.controller("keyValueCtrl", ["$scope", "$csnotify", "Restangular", function ($scope, $csnotify, rest) {
    "use strict";
    var keyValueApi = rest.all('KeyValueApi');
    var init = function () {
        $scope.GKeyValues = [];
        $scope.Keys = [];
        $scope.Values = [];
        $scope.gKeyValue = {};
        $scope.shouldBeOpen = false;
        $scope.showAddButton = false;
        $scope.Areas = [];
        $scope.valueTypes = ['Text', 'TextList', 'Number', 'NumberList', 'Date', 'DateList'];

        keyValueApi.customGETLIST("GetAreas").then(function (data) {
            $scope.Areas = data;
        }, function (data) {
            $csnotify.error(data);
        });
    };

    init();

    $scope.changeArea = function (selectedArea) {
        if (angular.isUndefined(selectedArea) || selectedArea == "") {
            return;
        }
        keyValueApi.customGETLIST("GetKeyValues", { area: selectedArea }).then(function (data) {
            $scope.GKeyValues = data;
            $scope.Keys = _.unique(_.pluck($scope.GKeyValues, 'Key'));
            $scope.Values = [];
            $scope.gKeyValue.Key = '';
            $scope.gKeyValue.ValueType = '';
        }, function (data) {
            $csnotify.error(data);
        });
    };


    $scope.changeKey = function (selectedKey) {
        if (angular.isUndefined(selectedKey) || selectedKey == "") {
            return;
        }
        $scope.Values = _.filter($scope.GKeyValues, function (kv) { return kv.Key == selectedKey; });
        $scope.gKeyValue.ValueType = $scope.Values[0].ValueType;

        if (_.indexOf(['TextList', 'NumberList', 'DateList'], $scope.gKeyValue.ValueType) > -1) {
            $scope.showAddButton = true;
        }
        else {
            $scope.showAddButton = false;
        }
    };

    $scope.openModel = function (keyValue) {
        $scope.shouldBeOpen = true;
        $scope.gKeyValue = angular.copy(keyValue);
        $scope.Values = [];
        $scope.gKeyValue.Key = '';
        $scope.gKeyValue.ValueType = '';
    };

    $scope.closeModel = function () {
        $scope.shouldBeOpen = false;
      // $scope.gKeyValue.ValueType = '';
    };

    $scope.saveKeyValue = function (gKeyValue) {
        if (gKeyValue.Id) {
            keyValueApi.customPUT(gKeyValue, "Put", { id: gKeyValue.Id }).then(function (data) {
                $scope.GKeyValues = _.reject($scope.GKeyValues, function (kv) { return kv.Id == data.Id; });
                $scope.GKeyValues.push(data);
                afterSave(data);
            }, function (data) {
                $csnotify.error(data);
            });
        } else {
            keyValueApi.customPOST(gKeyValue, "Post").then(function (data) {
                $scope.GKeyValues.push(data);
                afterSave(data);
            }, function (data) {
                $csnotify.error(data);
            });
        }
    };

    var afterSave = function (data) {
        $scope.Keys = _.unique(_.pluck($scope.GKeyValues, 'Key'));
        $scope.Values = _.filter($scope.GKeyValues, function (kv) { return kv.Key == data.Key; });
        $scope.gKeyValue = { Area: data.Area, Key: data.Key, ValueType: data.ValueType };

        if (_.indexOf(['TextList', 'NumberList', 'DateList'], $scope.gKeyValue.ValueType) > -1) {
            $scope.showAddButton = true;
        }
        else {
            $scope.showAddButton = false;
        }
    };
}])
);
