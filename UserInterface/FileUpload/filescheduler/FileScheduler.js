
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

        return {
            dldata: dldata,
            GetAll: getFileDetails,
            GetStatus: getFileStatus
        };
    }
]);


csapp.controller("fileSchedulerController", ["$scope", "$filter", "$csfactory", "$csnotify", "fileSchedulerDataLayer",
    function ($scope, $filter, $csfactory, $csnotify, datalayer) {
        "use strict";

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
                .then(function() {
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

        //#region validate page
        $scope.validatePage = function () {
            var isPageValid = isReasonValid();
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

////#region schedule
//$scope.postSchedule = function (data, completed) {
//    if (completed) {
//        $scope.showProgressBar = false;
//        $scope.stopwatch.reset();
//        $scope.fileScheduleDetails = {};
//        try {
//            $scope.fileScheduleDetails = JSON.parse(data);
//            hasAnyUnscheduledFiles();
//        } catch (e) {
//            $scope.$log.error(data);
//            $csnotify.error("Could not schedule files.");
//        }
//    } else if ($scope.showProgressBar === false) {
//        $scope.showProgressBar = true;
//        $scope.stopwatch.start();
//    }
//};
////#endregion



