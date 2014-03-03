(
csapp.controller("logdownloadCtrl", ["$scope", "Restangular", "$csnotify", "$csConstants",
    function ($scope, rest, $csnotify, $csConstants) {
        "use strict";
        var restApi = rest.all("LogDownloadApi");
        var currentPath = {};
        
        restApi.customGET("GetDrives")
            .then(function(data) {
                $scope.drives = data;
            }, function(response) {
                $csnotify.error(response.data.Message);
            });

        $scope.getStatus = function (path) {
            restApi.customGET("Fetch", { dir: path })
                .then(function(data) {
                    $scope.logfile = data;
                    $scope.logfile.Path = stripTrailingSlash($scope.logfile.Path);
                    currentPath = $scope.logfile.Path;
                }, function(response) {
                    $csnotify.error(response.data.Message);
                    $scope.logfile.Path = currentPath;
                });            
        };
        $scope.getStatus();

        function stripTrailingSlash(str) {
            if (str.substr(-1) == '\\') {
                return str.substr(0, str.length - 1);
            }
            return str;
        }

        $scope.changeDrive = function(d) {
            $scope.getStatus(d);
        };

        $scope.changeDirectory = function (dir) {
            $scope.logfile.Path = $scope.logfile.Path + "\\" + dir.Name;
            $scope.getStatus($scope.logfile.Path);
        };
        
        $scope.getParentDir = function (path) {
            var lpath = stripTrailingSlash(path);
            restApi.customGET("FetchParent", { dir: lpath })
                .then(function (data) {
                    $scope.logfile = data;
                }, function (response) {
                    $csnotify.error(response.data.Message);
                });
        };

        $scope.downloadFile = function (logFile) {
            var downloadpath = $csConstants.MVC_BASE_URL + "Developer/LogDownload/Download?filePathNName=" + logFile.FullName + "";
            window.location = downloadpath;
        };
    }])
);
