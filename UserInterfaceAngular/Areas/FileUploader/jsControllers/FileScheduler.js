/* globals mvcBaseUrl */
var controller = (csapp.controller("fileUploadController",
    ["$scope", "$http", "$filter", "$csfactory", "$csnotify", "csStopWatch", "Restangular", "$csConstants",
        function ($scope, $http, $filter, $csfactory, $csnotify, stopWatch, restangular, $csConstants) {

            "use strict";
            var restApi = restangular.all('FileDetailApi');

            //#region init page
            $scope.ResetPage = function () {
                $scope.SelectedDate = null;
                $scope.fileScheduleDetails = {};
                $scope.IsImmediate = false;
                $scope.immedateReason = '';
                $scope.hasUnscheduledFiles = false;
                $scope.selectedFiles = { file: [] };
            };

            // init page
            var init = function () {
                $scope.fileDetails = [];
                $scope.isPageValid = false;
                $scope.showProgressBar = false;
                $scope.stopwatch = stopWatch;
                $scope.ResetPage();

                restApi.customGETLIST("Get").then(function (data) {
                    $scope.fileDetails = data;
                }, function () {
                    $csnotify.error("Not able to retrieve basic data. Please contact AlgoSys support team.");
                });
            };
            init();

            //#endregion

            //#region get status
            $scope.changeSelectedFrequency = function () {
                var list = _.where($scope.fileDetails, { ScbSystems: $scope.SelectedSystem, Category: $scope.SelectedCategory });
                $scope.SelectedFrequency = list[0].Frequency;
                $scope.IsImmediate = "false";
            };

            // get file details
            $scope.getFileDetails = function () {
                var today = $filter('date')($scope.SelectedDate, 'yyyy-MM-dd');
                $http({
                    url: $csConstants.MVC_BASE_URL + "FileUploader/FileScheduler/GetFileStatus",
                    method: "GET",
                    params: { isystem: $scope.SelectedSystem, icategory: $scope.SelectedCategory, idate: today }
                }).success(function (data) {
                    $scope.fileScheduleDetails = data;
                    hasAnyUnscheduledFiles();
                }).error(function () {
                    $csnotify.error("Not able to retrieve data. Please contact AlgoSys support team.");
                });

            };

            var hasAnyUnscheduledFiles = function () {
                var list = _.findWhere($scope.fileScheduleDetails, { IsScheduled: false });
                if (list) {
                    $scope.hasUnscheduledFiles = true;
                } else {
                    $scope.hasUnscheduledFiles = false;
                }

                var list2 = _.findWhere($scope.fileScheduleDetails, { IsScheduled: true });
                if (list2) {
                    $scope.hasScheduledFiles = true;
                } else {
                    $scope.hasScheduledFiles = false;
                }
            };
            //#endregion

            //#region schedule
            $scope.postSchedule = function (data, completed) {
                if (completed) {
                    $scope.showProgressBar = false;
                    $scope.stopwatch.reset();
                    $scope.fileScheduleDetails = {};
                    try {
                        $scope.fileScheduleDetails = JSON.parse(data);
                        hasAnyUnscheduledFiles();
                    } catch (e) {
                        $scope.$log.error(data);
                        $csnotify.error("Could not schedule files.");
                    }
                } else if ($scope.showProgressBar === false) {
                    $scope.showProgressBar = true;
                    $scope.stopwatch.start();
                }
            };
            //#endregion

            //#region validate page
            $scope.validatePage = function () {
                var isPageValid = isReasonValid();
                //isPageValid = isPageValid && hasAnyFileScheduled();
                return !isPageValid;
            };

            var hasAnyFileScheduled = function () {
                if ($csfactory.isNullOrEmptyArray($scope.selectedFiles.file)) {
                    return false;
                }

                for (var i = 0; i < $scope.selectedFiles.file.length; i++) {
                    if ($scope.selectedFiles.file[i] != null) {
                        return true;
                    }
                }

                return false;
            };

            // submit the page
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

        }]));


