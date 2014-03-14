 

//#region controller
(
csapp.controller("ProfileController", ["$scope", "$csnotify", 'Restangular', function ($scope, $csnotify, rest) {

    'use strict';

    var apictrl = rest.all('ProfileApi');

    $scope.isReadOnly = true;

    $scope.opts = {
        backdropFade: true,
        dialogFade: true

    };

    $scope.open = function () {
        $scope.shouldBeOpen = true;
    };

    $scope.close = function () {
        $scope.shouldBeOpen = false;
    };

    $scope.isProfileExist = function (profile) {

        if (profile) {
            return true;
        }
        return false;
    };

    $scope.saveOrUpdateProfile = function (profile) {
        if (profile.Id) {
            apictrl.customPUT(profile, 'Put', { id: profile.Id }).then(function (data) {
                $csnotify.success("Mobile Number updated successfully.");
                $scope.close();
            }, function (response) {
                $csnotify.error(response);
            });
        }
        //profileFactory.saveProfile(profile).then(function(data) {
        //      $csnotify.success("Mobile Number updated successfully.");
        //      $scope.close();
        //  });
    };

    apictrl.customGET('Get').then(function (data) {

        if (data !== "null") {
            $csnotify.success('Profile Loaded Successfully.');
            $scope.profile = data;

            apictrl.customGET('Get', { id: $scope.profile.ReportsTo }).then(function (dt) {
                //var reportstoName = JSON.parse(dt);
                $scope.reportToName = dt.Name;
            });
        }
    }, function (response) {
        $csnotify.error(response);
    });

}])
);
//#endregion