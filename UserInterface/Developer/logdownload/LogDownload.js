
csapp.factory("driveExplorerDataLayer", [
    "Restangular", "$csnotify", "driveExplorerFactory", function (rest, $csnotify, factory) {

        var restApi = rest.all("LogDownloadApi");
        var dldata = {};

        var getDrives = function () {
            restApi.customGET("GetDrives")
                        .then(function (data) {
                            dldata.drives = data;
                        }, function (response) {
                            $csnotify.error(response.data.Message);
                        });
        };

        var getStatus = function (path) {
            restApi.customGET("Fetch", { dir: path })
                .then(function (data) {
                    dldata.logfile = data;
                    dldata.logfile.Path = factory.stripTrailingSlash(dldata.logfile.Path);
                    dldata.currentPath = dldata.logfile.Path;
                }, function (response) {
                    $csnotify.error(response.data.Message);
                    dldata.logfile.Path = dldata.currentPath;
                });
        };

        var getParentDir = function (path) {
            var lpath = factory.stripTrailingSlash(path);
            restApi.customGET("FetchParent", { dir: lpath })
                .then(function (data) {
                    dldata.logfile = data;
                }, function (response) {
                    $csnotify.error(response.data.Message);
                });
        };

        return {
            dldata: dldata,
            getDrives: getDrives,
            getStatus: getStatus,
            getParentDir: getParentDir
        };
    }
]);

csapp.factory("driveExplorerFactory", function () {

    function stripTrailingSlash(str) {
        if (str.substr(-1) == '\\') {
            return str.substr(0, str.length - 1);
        }
        return str;
    };

    return {
        stripTrailingSlash: stripTrailingSlash
    };
});

csapp.controller("driveExplorerController", [
    "$scope", "$csConstants", "$csfactory", "driveExplorerDataLayer",
    function ($scope, $csConstants, $csfactory, datalayer) {
        "use strict";
        (function () {
            $scope.currentPath = {};
            datalayer.getDrives();
            datalayer.getStatus();
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
        })();

        $scope.changeDrive = function (d) {
            datalayer.getStatus(d);
        };

        $scope.changeDirectory = function (dir) {
            $scope.dldata.logfile.Path = $scope.dldata.logfile.Path + "\\" + dir.Name;
            datalayer.getStatus($scope.dldata.logfile.Path);
        };

        $scope.downloadFile = function (logFile) {
            $csfactory.downloadFile(logFile.FullName);
        };
    }
]);
