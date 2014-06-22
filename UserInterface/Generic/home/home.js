csapp.factory("HomeDatalayer", ['Restangular', function (rest) {
    var restApi = rest.all("HomeApi");

    var getNotification = function () {
        return restApi.customGETLIST("GetUserNotifyList");
    };

    var getDetails = function (selectedUser) {
        return restApi.customGETLIST("GetActiveNotificationForStakeholder",
            { 'stakeholderId': selectedUser.StakeholderId });
    };

    var notifyApproval = function (notification) {
        return restApi.customPOST(notification, "NotifyApproval");
    };

    var notifyRejection = function (notification) {
        return restApi.customPOST(notification, "NotifyRejection");
    };

    var notifyDismiss = function (notification) {
        return restApi.customPOST(notification, "NotifyDissmiss");
    };

    return {
        GetNotifications: getNotification,
        GetNotificationDetails: getDetails,
        NotifyApproval: notifyApproval,
        NotifyRejection: notifyRejection,
        NotifyDismiss: notifyDismiss
    };
}]);

csapp.controller("HomeCtrl", ['$scope', 'HomeDatalayer', 'StakeholderAddNotificationFactory',
    function ($scope, datalayer, stkhAddFactory) {
        (function () {
            datalayer.GetNotifications().then(function (data) {
                $scope.userNotifyCount = data;
            });
        })();

        $scope.GetUserNotificationList = function () {
            datalayer.GetNotificationDetails($scope.selectedUser).then(function (data) {
                $scope.userNotifications = data;
                $scope.selectedUser.NotifyCount = data.length;
                angular.forEach(data, function (item) {
                    setFactory(item);
                });
            });
        };

        var reduceCount = function (notification) {
            $scope.userNotifications.splice(notification, 1);
            $scope.selectedUser.NotifyCount--;
        };

        $scope.onApprove = function (notification) {
            notification.factory.Approve(notification).then(function () {
                datalayer.NotifyApproval(notification).then(function () {
                    reduceCount(notification);
                });
            });
        };

        $scope.onReject = function (notification) {
            notification.factory.Reject(notification).then(function () {
                datalayer.NotifyRejection(notification).then(function () {
                    reduceCount(notification);
                });
            });
        };

        $scope.onDetails = function (notification) {
            notification.factory.Details(notification);
        };

        $scope.onDismiss = function (notification) {
            datalayer.NotifyDismiss(notification).then(function () {
                reduceCount(notification);
            });
        };

        var setFactory = function (notification) {
            switch (notification.NoteType) {
                case "StakeholderChange":
                    notification.factory = stkhAddFactory;
                    break;
                case "StakeholderPaymentChange":
                    break;
                case "StakeholderWorkingChange":
                    break;
                default:
                    throw "invalid notification type : " + notification.NoteType;
            }
            notification.buttons = notification.factory.Buttons();
        };

    }]);


csapp.factory("StakeholderAddNotificationFactory", ["Restangular", "$location", function (rest, $location) {

    var restApi = rest.all("ViewStakeApi");

    var buttons = function () {
        return {
            showApprove: true,
            showReject: true,
            showDetails: true
        };
    };

    var approve = function (notification) {
        var stakeholder = { Id: notification.EntityId };
        return restApi.customPOST(stakeholder, "ApproveStakeholder");
    };

    var reject = function (notification) {
        var stakeholder = { Id: notification.EntityId };
        return restApi.customPOST(stakeholder, "RejectStakeholder");
    };

    var details = function (notification) {
        $location.path('/stakeholder/add/' + notification.EntityId);
    };

    return {
        Buttons: buttons,
        Approve: approve,
        Reject: reject,
        Details: details
    };
}]);

