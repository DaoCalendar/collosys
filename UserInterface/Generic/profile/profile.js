
csapp.factory("profileDataLayer", ["$csnotify", 'Restangular', "$csAuthFactory", "$csfactory",
    function ($csnotify, rest, auth, $csfactory) {
        var apictrl = rest.all('ProfileApi');
        var dldata = {};

        var getUserProfile = function () {
            return apictrl.customGET('GetUser', { 'username': auth.getUsername() })
                .then(function (data) {
                    if ($csfactory.isNullOrEmptyString(data)) return;
                    $csnotify.success('Profile Loaded Successfully.');
                    dldata.profile = data;
                }, function (response) {
                    $csnotify.error(response.Message);
                });
        };

        var getManager = function () {
            if ($csfactory.isNullOrEmptyGuid(dldata.profile.ReportsTo)) {
                return;
            }

            apictrl.customGET('Get', { id: dldata.profile.ReportsTo })
                .then(function (dt) {
                    dldata.reportToName = dt.Name;
                });
        };

        var save = function () {
            return apictrl.customPOST(dldata.profile, 'Post')
                .then(function (data) {
                    $csnotify.success("Mobile Number updated successfully.");
                    dldata.profile = data;
                }, function (response) {
                    $csnotify.error(response.Message);
                });
        };


        return {
            dldata: dldata,
            Get: getUserProfile,
            GetManager: getManager,
            Save: save
        };
    }
]);

csapp.controller("profileController", ["$scope", "profileDataLayer",
    function ($scope, datalayer) {
        'use strict';
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            datalayer.Get().then(function () {
                $scope.origMobile = angular.copy($scope.dldata.profile.MobileNo);
                datalayer.GetManager();
            });
            $scope.isChangingMobile = false;
        })();

        $scope.saveMobile = function () {
            if ($scope.origMobile === $scope.dldata.profile.MobileNo) {
                return;
            }

            datalayer.Save().then(function () {
                $scope.isChangingMobile = false;
                $scope.origMobile = angular.copy($scope.dldata.profile.MobileNo);
            });
        };

        $scope.resetMobile = function () {
            $scope.isChangingMobile = false;
            $scope.dldata.profile.MobileNo = $scope.origMobile;
        };
    }
]);
//#endregion