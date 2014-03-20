/*global csapp*/


csapp.factory("Datalayer", ['$csnotify', 'Restangular', function ($csnotify, rest) {

    var restApi = rest.all('KeyValueApi');

    var dldata = {};

    var getAreas = function () {
        restApi.customGETLIST("GetAreas").then(function (data) {
            dldata.Areas = data;
        }, function (data) {
            $csnotify.error(data);
        });
    };

    var getKeyValues = function (selectedArea) {
        restApi.customGETLIST("GetKeyValues", { area: selectedArea }).then(function (data) {
            dldata.GKeyValues = data;
            dldata.gKeyValue = {};
            dldata.Keys = _.unique(_.pluck(dldata.GKeyValues, 'Key'));
            dldata.Values = [];
            dldata.gKeyValue.Key = '';
            dldata.gKeyValue.ValueType = '';
        }, function (data) {
            $csnotify.error(data);
        });
    };

    var saveKeyValue = function (gKeyValue) {
        if (gKeyValue.Id) {
            return restApi.customPUT(gKeyValue, "Put", { id: gKeyValue.Id }).then(function (data) {
                dldata.GKeyValues = _.reject(dldata.GKeyValues, function (kv) { return kv.Id == data.Id; });
                dldata.GKeyValues.push(data);
                afterSave(data);
            }, function (data) {
                $csnotify.error(data);
            });
        } else {
            return restApi.customPOST(gKeyValue, "Post").then(function (data) {
                dldata.GKeyValues.push(data);
                afterSave(data);
            }, function (data) {
                $csnotify.error(data);
            });
        }
    };

    var afterSave = function (data) {
        console.log('after save');
        dldata.Keys = _.unique(_.pluck(dldata.GKeyValues, 'Key'));
        dldata.Values = _.filter(dldata.GKeyValues, function (kv) { return kv.Key == data.Key; });
        dldata.gKeyValue = { Area: data.Area, Key: data.Key, ValueType: data.ValueType };
    };

    return {
        dldata: dldata,
        GetAreas: getAreas,
        GetKeyValues: getKeyValues,
        Save: saveKeyValue
    };
}]);

csapp.factory("KeyValueFactory", ['$csfactory', function ($csfactory) {

    var setShowAddButton = function (buttonVal, dldata) {
        return (_.indexOf(['TextList', 'NumberList', 'DateList'], dldata.gKeyValue.ValueType) > -1);
    };

    var changeKey = function (selectedKey, dldata) {
        if (angular.isUndefined(selectedKey) || selectedKey == "") {
            return;
        }
        dldata.Values = _.filter(dldata.GKeyValues, function (kv) { return kv.Key == selectedKey; });
        dldata.gKeyValue.ValueType = dldata.Values[0].ValueType;
    };

    return {
        setShowAddButton: setShowAddButton,
        changeKey: changeKey
    };
}]);

csapp.controller("keyValueCtrl", ["$scope", "$csnotify", "Restangular", "Datalayer", "KeyValueFactory", function ($scope, $csnotify, rest, datalayer, factory) {
    "use strict";
    (function () {
        $scope.showAddButton = false;
        $scope.valueTypes = ['Text', 'TextList', 'Number', 'NumberList', 'Date', 'DateList'];
        $scope.datalayer = datalayer;
        $scope.dldata = datalayer.dldata;
        datalayer.GetAreas();
    })();

    $scope.changeKey = function (selectedKey) {
        factory.changeKey(selectedKey, $scope.dldata);
        $scope.showAddButton = factory.setShowAddButton($scope.showAddButton, $scope.dldata);
    };

    $scope.saveKeyValue = function (gKeyValue) {
        datalayer.Save(gKeyValue).then(function () {
            factory.setShowAddButton($scope.showAddButton, $scope.dldata);
        });

    };

}]);


$scope.openModel = function (keyValue) {
    $scope.shouldBeOpen = true;
    $scope.dldata.gKeyValue = angular.copy(keyValue);
    $scope.dldata.Values = [];
    $scope.dldata.gKeyValue.Key = '';
    $scope.dldata.gKeyValue.ValueType = '';
};

$scope.closeModel = function () {
    $scope.shouldBeOpen = false;
    // $scope.gKeyValue.ValueType = '';
};




//keyValueApi.customGETLIST("GetAreas").then(function (data) {
//    $scope.Areas = data;
//}, function (data) {
//    $csnotify.error(data);
//});

//    keyValueApi.customGETLIST("GetKeyValues", { area: selectedArea }).then(function(data) {
//        $scope.GKeyValues = data;
//        $scope.Keys = _.unique(_.pluck($scope.GKeyValues, 'Key'));
//        $scope.Values = [];
//        $scope.gKeyValue.Key = '';
//        $scope.gKeyValue.ValueType = '';
//    }, function(data) {
//        $csnotify.error(data);
//    });

//if (gKeyValue.Id) {
//    keyValueApi.customPUT(gKeyValue, "Put", { id: gKeyValue.Id }).then(function(data) {
//        $scope.dldata.GKeyValues = _.reject($scope.dldata.GKeyValues, function(kv) { return kv.Id == data.Id; });
//        $scope.dldata.GKeyValues.push(data);
//        afterSave(data);
//    }, function(data) {
//        $csnotify.error(data);
//    });
//} else {
//    keyValueApi.customPOST(gKeyValue, "Post").then(function(data) {
//        $scope.dldata.GKeyValues.push(data);
//        afterSave(data);
//    }, function(data) {
//        $csnotify.error(data);
//    });
//}

//var afterSave = function(data) {
//    $scope.dldata.Keys = _.unique(_.pluck($scope.GKeyValues, 'Key'));
//    $scope.dldata.Values = _.filter($scope.dldata.GKeyValues, function(kv) { return kv.Key == data.Key; });
//    $scope.dldata.gKeyValue = { Area: data.Area, Key: data.Key, ValueType: data.ValueType };

//    if (_.indexOf(['TextList', 'NumberList', 'DateList'], $scope.dldata.gKeyValue.ValueType) > -1) {
//        $scope.showAddButton = true;
//    } else {
//        $scope.showAddButton = false;
//    }
//};


//if (_.indexOf(['TextList', 'NumberList', 'DateList'], $scope.dldata.gKeyValue.ValueType) > -1) {
//    $scope.showAddButton = true;
//} else {
//    $scope.showAddButton = false;
//}

// if (angular.isUndefined(selectedKey) || selectedKey == "") {
//     return;
// }
// $scope.dldata.Values = _.filter($scope.dldata.GKeyValues, function (kv) { return kv.Key == selectedKey; });
//$scope.dldata.gKeyValue.ValueType = $scope.dldata.Values[0].ValueType;

//$scope.changeArea = function (selectedArea) {
//    if (angular.isUndefined(selectedArea) || selectedArea == "") {
//        return;
//    }
//    datalayer.GetKeyValues(selectedArea);
//};