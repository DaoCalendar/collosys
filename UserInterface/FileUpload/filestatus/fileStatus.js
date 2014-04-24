
csapp.factory("fileStatusDataLayer", ["Restangular", "$csnotify", function (rest, $csnotify) {
    var restApi = rest.all("FileStatusApi");
    var dldata = { isHttpCallInProgress: false };

    var getStatusByDateRange = function (from, to) {
        var fromData1 = moment(from).format('L');
        var fromData2 = moment(to).format('L');
        dldata.isHttpCallInProgress = true;
        return restApi.customGET("GetStatusByDate", { fromDate: fromData1, toDate: fromData2 })
            .then(function (data) {
                dldata.isHttpCallInProgress = false;
                dldata.fileSchedulers = data;
            }, function (data) {
                dldata.isHttpCallInProgress = false;
                $csnotify.error(data.Message);
            });
    };

    var retryFileUpload = function (fileScheduler) {
        dldata.isHttpCallInProgress = true;
        restApi.customPOST(fileScheduler, "RetryUpload")
            .then(
                function (data) {
                    var index = _.findIndex(dldata.fileSchedulers, { 'Id': fileScheduler.Id });
                    if (index !== -1) dldata.fileSchedulers[index] = data;
                    dldata.isHttpCallInProgress = false;
                    $csnotify.success("File Rescheduled for Upload");
                }, function (data) {
                    dldata.isHttpCallInProgress = false;
                    $csnotify.error(data);
                });
    };

    var deleteUpload = function (fileScheduler) {
        dldata.isHttpCallInProgress = true;
        restApi.customDELETE("Delete", { id: fileScheduler.Id })
            .then(function () {
                dldata.isHttpCallInProgress = false;
                var index = _.findIndex(dldata.fileSchedulers, { 'Id': fileScheduler.Id });
                if (index !== -1) dldata.fileSchedulers.splice(index, 1);
                $csnotify.success("File deleted");
            }, function (data) {
                dldata.isHttpCallInProgress = false;
                $csnotify.error(data.Message);
            });
    };

    var rescheduleFile = function (fileScheduler) {
        dldata.isHttpCallInProgress = true;
        return restApi.customPOST(fileScheduler, "ImmidiateFileSchedule")
            .then(function (data) {
                dldata.isHttpCallInProgress = false;
                var index = _.findIndex(dldata.fileSchedulers, { 'Id': fileScheduler.Id });
                if (index !== -1) dldata.fileSchedulers[index] = data;
                $csnotify.success("File is Rescheduled");
            }, function () {
                dldata.isHttpCallInProgress = false;
                fileScheduler.ImmediateReason = "";
                $csnotify.error("File Not Rescheduled. Please try again");
            });
    };

    var createDownloadableExcel = function (fileScheduler) {
        var scheduler = [];
        scheduler.push(fileScheduler);
        dldata.isHttpCallInProgress = true;
        return restApi.customPOST(scheduler, "DownloadFile")
            .then(function (data) {
                dldata.isHttpCallInProgress = false;
                return data;
            }, function (data) {
                dldata.isHttpCallInProgress = false;
                $csnotify.error(data.data);
            });
    };

    return {
        dldata: dldata,
        GetStatus: getStatusByDateRange,
        Retry: retryFileUpload,
        Delete: deleteUpload,
        MakeImmediate: rescheduleFile,
        GenerateExcel: createDownloadableExcel,
    };
}]);

csapp.factory("fileStatusFactory", function () {
    var refresh = {
        suspend: false,
        pause: function () { refresh.suspend = true; },
        cont: function () { refresh.suspend = false; },
        toggle: function () { refresh.suspend = !refresh.suspend; },
        refreshText: function () { return refresh.suspend ? "Resume Refresh" : "Pause Refresh"; }
    };

    var timer = {
        timePending: 0,
        refreshInterval: 1.5 * 60,
        reset: function () { timer.timePending = timer.refreshInterval; },
        update: function () { timer.timePending--; },
        done: function () { return timer.timePending <= 0; },
        force: function () { timer.timePending = 0; }
    };

    return {
        refresh: refresh,
        timer: timer
    };
});

csapp.controller("fileImmediateController", ["$scope", "fileStatusDataLayer", "fileScheduled", "$modalInstance",
    function ($scope, datalayer, fileScheduled, $modalInstance) {
        $scope.file = angular.copy(fileScheduled);

        $scope.close = function () {
            $modalInstance.dismiss();
        };

        $scope.reschedule = function () {
            datalayer.MakeImmediate($scope.file).then(function () {
                $modalInstance.close();
            });
        };
    }
]);

csapp.controller("fileStatusDetailsController", ["$scope", "fileStatusDataLayer", "fileScheduled", "$modalInstance",
    function ($scope, datalayer, fileScheduled, $modalInstance) {
        $scope.file = fileScheduled;

        $scope.close = function () {
            $modalInstance.dismiss();
        };

        $scope.CompuateTimeSpent = function (index) {
            if (index <= 0) return "No Gap";
            var prevTime = moment($scope.file.FileStatuss[index - 1].EntryDateTime);
            var currTime = moment($scope.file.FileStatuss[index].EntryDateTime);
            return moment.duration(currTime.diff(prevTime)).humanize();
        };

        $scope.CompuateTotalTimeSpent = function (index) {
            if (index <= 0) return "No Gap";
            var prevEntry = _.findWhere($scope.file.FileStatuss, { 'UploadStatus': 'UploadStarted' });
            var prevTime = moment(prevEntry.EntryDateTime);
            var currTime = moment($scope.file.FileStatuss[index].EntryDateTime);
            if (currTime.isBefore(prevTime)) return "No Gap";
            return moment.duration(currTime.diff(prevTime)).humanize();
        };
    }
]);

csapp.controller("fileStatusController", ["$scope", "$interval", "$csfactory", "fileStatusDataLayer", "fileStatusFactory", "modalService", "$modal",
    function ($scope, $interval, $csfactory, datalayer, factory, modalServer, $modal) {
        "use strict";

        //#region auto refresh
        (function () {
            $scope.fileScheduler = {};
            $scope.fileScheduler.fromDate = moment().add('months', -1).format('L');
            $scope.fileScheduler.toDate = moment().format('L');
            $scope.datalayer = datalayer;
            $scope.factory = factory;
        })();

        $scope.$watch(function () {
            return factory.timer.timePending;
        }, function () {
            if (!factory.timer.done()) return;
            datalayer.GetStatus($scope.fileScheduler.fromDate, $scope.fileScheduler.toDate)
                .then(function () { factory.timer.reset(); }, function () { factory.timer.reset(); });
        });

        var updateTimer = function () {
            if (datalayer.dldata.isHttpCallInProgress) return;
            if (factory.refresh.suspend) return;
            factory.timer.update();
        };

        var interval = $interval(updateTimer, 1000);
        $scope.$on('$destroy', function () {
            $interval.cancel(interval);
        });
        //#endregion

        //#region modal delete/reschedule/status
        $scope.showDeleteModal = function (fileScheduler) {
            var modalOptions = {
                closeButtonText: 'Cancel',
                actionButtonText: 'Delete File',
                headerText: "Delete '" + fileScheduler.FileName + "' ?",
                bodyText: 'Are you sure you want to delete this scheduled file?'
            };

            factory.refresh.pause();
            modalServer.showModal({}, modalOptions).then(function () {
                datalayer.Delete(fileScheduler);
                factory.refresh.cont();
            }, function () { factory.refresh.cont(); });
        };

        $scope.showImmediateModal = function (file) {
            factory.refresh.pause();
            var inst1 = $modal.open({
                templateUrl: baseUrl + 'FileUpload/filestatus/file-status-immediate.html',
                controller: 'fileImmediateController',
                resolve: { fileScheduled: function () { return file; } }
            });

            inst1.result.then(function () { factory.refresh.cont(); },
                function () { factory.refresh.cont(); });
        };

        $scope.showStatusModal = function (file) {
            factory.refresh.pause();
            var inst2 = $modal.open({
                templateUrl: baseUrl + 'FileUpload/filestatus/file-status-detail.html',
                controller: 'fileStatusDetailsController',
                resolve: { fileScheduled: function () { return file; } }
            });

            inst2.result.then(function () { factory.refresh.cont(); },
                function () { factory.refresh.cont(); });
        };
        //#endregion

        //#region download
        $scope.retryUpload = function (filescheduler) {
            datalayer.Retry(filescheduler);
        };

        $scope.downloadOutput = function (filescheduler) {
            $csfactory.enableSpinner();
            datalayer.GenerateExcel(filescheduler)
                .then(function (data) {
                    $csfactory.downloadFile(data);
                });
        };

        $scope.downloadInput = function (fileScheduler) {
            var filename = fileScheduler.FileDirectory + '\\' + fileScheduler.FileName;
            $csfactory.downloadFile(filename);
        };
        //#endregion
    }
]);
