
csapp.factory("profileDataLayer", ["$csnotify", 'Restangular', "$csAuthFactory",
    function ($csnotify, rest, auth) {
        var apictrl = rest.all('ProfileApi');
        var dldata = {};

        var getUserProfile = function () {
            apictrl.customGET('GetUser', { 'username': auth.getUsername() })
                .then(function (data) {
                    if (data === "null") return;
                    $csnotify.success('Profile Loaded Successfully.');
                    dldata.profile = data;
                }, function (response) {
                    $csnotify.error(response.Message);
                });
        };

        var getManager = function () {
            apictrl.customGET('Get', { id: dldata.profile.ReportsTo })
                .then(function (dt) {
                    dldata.reportToName = dt.Name;
                });
        };

        var save = function (profile) {
            apictrl.customPUT(profile, 'Put', { id: profile.Id })
                .then(function (data) {
                    $csnotify.success("Mobile Number updated successfully.");
                    dldata.profile = data;
                }, function (response) {
                    $csnotify.error(response.Message);
                });
        };


        return {
            Get: getUserProfile,
            GetManager: getManager,
            Save: save
        };
    }
]);

csapp.controller("ProfileController", ["$scope", "profileDataLayer",
    function ($scope, datalayer) {
        'use strict';
        (function() {
            datalayer.Get().then(function() {
                datalayer.GetManager();
            });
        })();


    }
]);
//#endregion