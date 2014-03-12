
csapp.factory("fileSchedulerDataLayer", ["Restangular", "$csnotify",
    function (restangular, $csnotify) {
        var restApi = restangular.all('FileSchedulerApi');
        var dldata = {};

        var getFileDetails = function () {
            restApi.customGETLIST("GetFileDetails").then(function (data) {
                dldata.fileDetails = data;
            }, function () {
                $csnotify.error("Not able to retrieve basic data. Please contact AlgoSys support team.");
            });
        };

        //var getFileStatus = function () {
        //    var today = $filter('date')($scope.SelectedDate, 'yyyy-MM-dd');
        //    $http({
        //        url: $csConstants.MVC_BASE_URL + "FileUploader/FileScheduler/GetFileStatus",
        //        method: "GET",
        //        params: { isystem: $scope.SelectedSystem, icategory: $scope.SelectedCategory, idate: today }
        //    }).success(function (data) {
        //        $scope.fileScheduleDetails = data;
        //        hasAnyUnscheduledFiles();
        //    }).error(function () {
        //        $csnotify.error("Not able to retrieve data. Please contact AlgoSys support team.");
        //    });
        //};

        return {
            dldata: dldata,
            GetAll: getFileDetails
            //,
            //GetStatus: getFileStatus
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
            //$scope.fileDetails = [];
            //$scope.isPageValid = false;
            $scope.ResetPage();
            $scope.datalayer = datalayer;
            datalayer.GetAll();
        })();

        $scope.changeSelectedFrequency = function () {
            var list = _.where(datalayer.dldata.fileDetails, { ScbSystems: $scope.SelectedSystem, Category: $scope.SelectedCategory });
            $scope.SelectedFrequency = list[0].Frequency;
            $scope.IsImmediate = "false";
        };

    }
]);

//// get file details
//var hasAnyUnscheduledFiles = function () {
//    var list = _.findWhere($scope.fileScheduleDetails, { IsScheduled: false });
//    if (list) {
//        $scope.hasUnscheduledFiles = true;
//    } else {
//        $scope.hasUnscheduledFiles = false;
//    }

//    var list2 = _.findWhere($scope.fileScheduleDetails, { IsScheduled: true });
//    if (list2) {
//        $scope.hasScheduledFiles = true;
//    } else {
//        $scope.hasScheduledFiles = false;
//    }
//};
////#endregion

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

////#region validate page
//$scope.validatePage = function () {
//    var isPageValid = isReasonValid();
//    //isPageValid = isPageValid && hasAnyFileScheduled();
//    return !isPageValid;
//};

//var hasAnyFileScheduled = function () {
//    if ($csfactory.isNullOrEmptyArray($scope.selectedFiles.file)) {
//        return false;
//    }

//    for (var i = 0; i < $scope.selectedFiles.file.length; i++) {
//        if ($scope.selectedFiles.file[i] != null) {
//            return true;
//        }
//    }

//    return false;
//};

//// submit the page
//var isReasonValid = function () {
//    // if not scheduled, keep it disabled
//    if (!$scope.IsImmediate) {
//        return false;
//    }

//    // if nightly, no reason needed, enable
//    if ($scope.IsImmediate === 'false') {
//        return true;
//    }

//    // if not nightly='immediate', reason must be provided
//    if (!$scope.immedateReason) {
//        return false;
//    }

//    //reason must be min 10 chars
//    if ($scope.immedateReason.length < 5) {
//        return false;
//    }

//    return true;
//};
////#endregion


