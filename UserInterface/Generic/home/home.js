csapp.factory("HomeDatalayer", ['Restangular', function (rest) {
    var restApi = rest.all("HomeApi");

    var getNotification = function () {
        return restApi.customGETLIST("GetUserNotifyList");
    };

    var getDetails = function (selectedUser) {
        return restApi.customGETLIST("GetActiveNotificationForStakeholder",
            { 'stakeholderId': selectedUser.StakeholderId });
    };

    return {
        GetNotifications: getNotification,
        GetNotificationDetails: getDetails
    };
}]);

csapp.controller("HomeCtrl", ['$scope', 'HomeDatalayer', function ($scope, datalayer) {
    (function () {
        datalayer.GetNotifications().then(function (data) {
            $scope.userNotifyCount = data;
        });
    })();

    $scope.GetUserNotificationList = function () {
        datalayer.GetNotificationDetails($scope.selectedUser).then(function (data) {
            $scope.userNotifications = data;
        });
    };

}]);
