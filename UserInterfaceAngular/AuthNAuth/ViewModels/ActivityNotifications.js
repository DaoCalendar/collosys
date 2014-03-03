
csapp.controller("NotificationsController", ["$scope", "$http", function ($scope, $http) {
    'use strict';

    var urlNotifications = "api/NotificationApi/";


    $http({
        url: urlNotifications + "GetActivities",
        method: "GET"
    }).success(function (data) {
       $scope.notifications = data;
    }).error(function () {
    });
}]);

csapp.controller("signoutController", ["$scope", "$location", function ($scope, $location) {
    'use strict';
   
    //console.log("$location.protocol()=" + $location.protocol());
    //console.log("$location.host()=" + $location.host());
    //console.log("$location.port()=" + $location.port());
    //console.log("$location.path()=" + $location.path());
    //console.log("$location.search()=" + $location.search());
    //console.log("$location.hash()=" + $location.hash());
    //console.log("$location.url()=" + $location.url());
    //console.log("$location.absUrl()=" + $location.absUrl());

} ]);