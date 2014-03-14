
csapp.factory("fileSchedulerDataLayer", ["Restangular", "$csnotify", "$filter",
    function (restangular, $csnotify, $filter) {
        var restApi = restangular.all('FileSchedulerApi');
        var dldata = {};

        var getFileDetails = function () {
            restApi.customGETLIST("GetFileDetails").then(function (data) {
                dldata.fileDetails = data;
            }, function () {
                $csnotify.error("Not able to retrieve basic data. Please contact AlgoSys support team.");
            });
        };

        var getFileStatus = function (system, category, today) {
            return restApi.customGET("GetFileStatus", {
                isystem: system,
                icategory: category,
                idate: today
            }).then(function (data) {
                dldata.fileScheduleDetails = data;
            }, function () {
                $csnotify.error("Not able to retrieve basic data. Please contact AlgoSys support team.");
            });
        };

        var scheduleFiles = function() {
            return restApi.customGET("Upload", { 'scheduledFiles': dldata.fileScheduleDetails })
                .then(function(data) {
                    dldata.fileScheduleDetails = data;
                }, function() {
                    $csnotify.error("Could not schedule files.");
                });
        };

        return {
            dldata: dldata,
            GetAll: getFileDetails,
            GetStatus: getFileStatus,
            Schedule: scheduleFiles
        };
    }
]);


csapp.controller("fileSchedulerController", ["$scope", "$filter", "$csfactory", "$csnotify", "fileSchedulerDataLayer", "$upload",
    function ($scope, $filter, $csfactory, $csnotify, datalayer, $upload) {
        "use strict";

        //#region helpers
        $scope.ResetPage = function () {
            $scope.SelectedDate = null;
            $scope.fileScheduleDetails = {};
            $scope.IsImmediate = false;
            $scope.immedateReason = '';
            $scope.hasUnscheduledFiles = false;
            $scope.selectedFiles = { file: [] };
        };

        (function () {
            $scope.isPageValid = false;
            $scope.ResetPage();
            $scope.datalayer = datalayer;
            datalayer.GetAll();
        })();

        $scope.changeSelectedFrequency = function () {
            var list = _.where(datalayer.dldata.fileDetails, { ScbSystems: $scope.SelectedSystem, Category: $scope.SelectedCategory });
            $scope.SelectedFrequency = list[0].Frequency;
            $scope.IsImmediate = "false";
        };

        $scope.getFileDetails = function () {
            var selecteddate = moment($scope.SelectedDate).format('YYYY-MM-DD');
            datalayer.GetStatus($scope.SelectedSystem,
                    $scope.SelectedCategory,
                    selecteddate)
                .then(function () {
                    hasAnyUnscheduledFiles();
                });
        };

        var hasAnyUnscheduledFiles = function () {
            var list = _.findWhere(datalayer.dldata.fileScheduleDetails.ScheduleInfo, { IsScheduled: false });
            if (list) {
                $scope.hasUnscheduledFiles = true;
            } else {
                $scope.hasUnscheduledFiles = false;
            }

            var list2 = _.findWhere(datalayer.dldata.fileScheduleDetails.ScheduleInfo, { IsScheduled: true });
            if (list2) {
                $scope.hasScheduledFiles = true;
            } else {
                $scope.hasScheduledFiles = false;
            }
        };
        //#endregion

        //#region upload
        $scope.uploadCount = 0;
        $scope.onFileSelect = function (file, $files, $index) {
            var cfile = $files[0];
            file.IsUploading = true;
            $scope.uploadCount++;
            $scope.upload[$index] = $upload.upload({
                url: '/api/FileTransfer/SaveFileOnServer',
                method: "Post",
                file: cfile,
            }).progress(function (evt) {
                file.UploadPercent = parseInt(100.0 * evt.loaded / evt.total);
            }).success(function (data) {
                file = data;
                file.IsUploading = false;
                $scope.uploadCount--;
            }).error(function () {
                file.IsUploading = false;
                $scope.uploadCount--;
            });
        };

        $scope.scheduleFiles = function() {
            datalayer.Schedule.then(function () {
                $scope.upload = [];
                hasAnyUnscheduledFiles();
            });
        };
        //#endregion

        //#region validate page
        $scope.validatePage = function () {
            var isPageValid = isReasonValid();
            isPageValid = isPageValid && ($scope.uploadCount === 0);
            return !isPageValid;
        };

        var isReasonValid = function () {
            // if not scheduled, keep it disabled
            if (!$scope.IsImmediate) {
                return false;
            }

            // if nightly, no reason needed, enable
            if ($scope.IsImmediate === 'false') {
                return true;
            }

            // if not nightly='immediate', reason must be provided
            if (!$scope.immedateReason) {
                return false;
            }

            //reason must be min 10 chars
            if ($scope.immedateReason.length < 5) {
                return false;
            }

            return true;
        };
        //#endregion
    }
]);

