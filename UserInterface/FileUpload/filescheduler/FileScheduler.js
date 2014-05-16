
csapp.factory("fileSchedulerDataLayer", ["Restangular", "$csnotify",
    function (restangular, $csnotify) {
        var restApi = restangular.all('FileSchedulerApi');
        var dldata = {};

        var getFileDetails = function () {
            restApi.customGETLIST("GetFileDetails").then(function (data) {
                dldata.fileDetails = data;
                dldata.fileDetailsScbSystems = _.uniq(_.pluck(data, 'ScbSystems'));
                dldata.fileDetailsCategory = _.uniq(_.pluck(data, 'Category'));
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

        var scheduleFiles = function () {
            var scheduledFiles = dldata.fileScheduleDetails;
            return restApi.customPOST(scheduledFiles, "Upload")
                .then(function (data) {
                    dldata.fileScheduleDetails = data;
                }, function () {
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

csapp.factory("fileSchedulerFactory", function () {

    var validateFile = function (file) {
        return true;
    };

    return {
        isFileValid: validateFile
    };
});


csapp.controller("fileSchedulerController", ["$scope", "$filter", "$csfactory", "$csnotify", "fileSchedulerDataLayer", "$upload", "fileSchedulerFactory", "$csModels",
    function ($scope, $filter, $csfactory, $csnotify, datalayer, $upload, factory, $csModels) {
        "use strict";

        //#region helpers
        $scope.ResetPage = function () {
            $scope.Selected.Date = null;
            $scope.fileScheduleDetails = {};
            $scope.IsImmediate = false;
            $scope.immedateReason = '';
            $scope.hasUnscheduledFiles = false;
            $scope.selectedFiles = { file: [] };
        };

        (function () {
            $scope.Selected = { Date: null };
            $scope.isPageValid = false;
            $scope.ResetPage();
            $scope.datalayer = datalayer;
            datalayer.GetAll();
            $scope.fileSchedulerfield = $csModels.getColumns("FileScheduler");
            $scope.IsImmediate = "false";
        })();

        $scope.changeSelectedFrequency = function () {
            var list = _.where(datalayer.dldata.fileDetails, { ScbSystems: $scope.Selected.System, Category: $scope.Selected.Category });
            $scope.Selected.Frequency = list[0].Frequency;
        };

        $scope.getFileDetails = function () {
            var selecteddate = moment($scope.Selected.Date).format('YYYY-MM-DD');
            datalayer.GetStatus($scope.Selected.System,
                    $scope.Selected.Category,
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
        $scope.upload = [];
        $scope.onFileSelect = function (file, $files, $index) {
            var cfile = $files[0];
            if (!factory.isFileValid(cfile)) {
                return;
            }
            file.IsUploading = true;
            $scope.uploadCount++;
            file.UploadPercent = 10;
            $scope.upload[$index] = $upload.upload({
                url: '/api/FileSchedulerApi/SaveFileOnServer',
                method: "Post",
                file: cfile,
                data: { schedulerInfo: file },
            }).progress(function (evt) {
                file.UploadPercent = parseInt(100.0 * evt.loaded / evt.total);
            }).success(function (data) {
                file.UploadPath = data.UploadPath;
                file.FileName = data.FileName;
                file.FileSize = data.FileSize;
                file.IsUploading = false;
                $scope.uploadCount--;
            }).error(function () {
                file.IsUploading = false;
                $scope.uploadCount--;
            });
        };

        $scope.scheduleFiles = function () {
            datalayer.Schedule().then(function () {
                $scope.upload = [];
                hasAnyUnscheduledFiles();
            });
        };
        //#endregion

        //#region validate page
        $scope.validatePage = function () {
            var isPageValid = isReasonValid();
            isPageValid = isPageValid && ($scope.uploadCount === 0); // no files being copied
            isPageValid = isPageValid && ($scope.upload.length > 0); // atleast one file scheduled
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

