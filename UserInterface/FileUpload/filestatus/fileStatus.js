
csapp.controller("fileStatusController", ["$scope", "Restangular", "$csnotify", "$timeout", "$csConstants",
    function ($scope, rest, $csnotify, $timeout, $csConstants) {
        "use strict";

        var restApi = rest.all("FileStatusApi");
        $scope.fileScheduler = {};
        $scope.fileScheduler.fromDate = moment().add('months', -1).format('L');
        $scope.fileScheduler.toDate = moment().format('L');

        //#region auto refresh
        $scope.getCurrentStatus = function () {
            $scope.isInProcessing = true;
            var fromData1 = moment($scope.fileScheduler.fromDate).format('L');
            var fromData2 = moment($scope.fileScheduler.toDate).format('L');
            restApi.customGET("GetStatusByDate", { fromDate: fromData1, toDate: fromData2 }).then(function (data) {
                $scope.isInProcessing = false;
                $scope.fileSchedulers = data;
            }, function (data) {
                $scope.isInProcessing = false;
                $csnotify.error(data.Message);
            });
        };

        $scope.toggleAutoRefresh = function () {
            if ($scope.autoRefresh === true) {
                $scope.autoRefresh = false;
                $scope.autoRefreshText = "Resume Refresh";
            } else {
                $scope.autoRefresh = true;
                $scope.autoRefreshText = "Pause Refresh";
            }
        };
        $scope.toggleAutoRefresh();



        var refreshPage = function () {
            if ($scope.isInProcessing) return;
            if ($scope.doUpdate) {
                $scope.updatingInSeconds = 30;
                $scope.doUpdate = false;
                $scope.getCurrentStatus();
            } else {
                if ($scope.autoRefresh === true)
                    $scope.updatingInSeconds = $scope.updatingInSeconds - 1;
                $scope.doUpdate = $scope.updatingInSeconds === 0;
            }
        };

        $scope.isInProcessing = false;
        $scope.doUpdate = true;
        setInterval(function () { $scope.$apply(refreshPage); }, 1000);
        //#endregion

        //#region Db Operations  / download

        $scope.fileSchedulers = [];

        $scope.retryFileUpload = function (fileScheduler, index) {
            $scope.isInProcessing = true;
            restApi.customPOST(fileScheduler, "RetryUpload")
                .then(
                    function (data) {
                        $scope.isInProcessing = false;
                        $scope.fileSchedulers[index] = data;
                        $csnotify.success("File Rescheduled for Upload");
                    }, function (data) {
                        $scope.isInProcessing = false;
                        $csnotify.error(data);
                    });
        };

        $scope.showDeleteModelPopup = function (fileScheduler, index) {
            $scope.showDeleteModel = true;
            $scope.DeleteFileScheduler = fileScheduler;
            $scope.DeleteIndex = index;
        };

        $scope.noToDelete = function () {
            $scope.showDeleteModel = false;
        };


        $scope.deleteUpload = function () {
            $scope.showDeleteModel = false;
            var fileScheduler = $scope.DeleteFileScheduler;
            var index = $scope.DeleteIndex;
            $scope.isInProcessing = true;
            restApi.customDELETE("Delete", { id: fileScheduler.Id })
            .then(function () {
                $scope.isInProcessing = false;
                $scope.fileSchedulers.splice(index, 1);
                $csnotify.success("File deleted");
            }, function (data) {
                $scope.isInProcessing = false;
                $csnotify.error(data.Message);
            });
        };

        $scope.rescheduleFile = function (fileScheduler) {
            $scope.openImidiateModel = false;
            $scope.isInProcessing = true;

            restApi.customPOST(fileScheduler, "ImmidiateFileSchedule")
                .then(function (data) {
                    $scope.isInProcessing = false;
                    $scope.fileSchedulers[$scope.ImmediatefileIndex] = data;
                    $csnotify.success("File is Rescheduled");
                }, function () {
                    $scope.isInProcessing = false;
                    fileScheduler.ImmediateReason = "";
                    $csnotify.error("File Not Rescheduled. Please try again");
                });
        };

        $scope.downloadFile = function (fileScheduler) {
            var scheduler = [];
            scheduler.push(fileScheduler);
            $scope.isInProcessing = true;

            restApi.customPOST(scheduler, "DownloadFile")
                .then(function (filename) { //success
                    var downloadpath = $csConstants.MVC_BASE_URL + "FileUploader/FileStatus/Download?fullfilename='" + filename + "'";
                    window.location = downloadpath;
                    $scope.isInProcessing = false;
                }, function (data) { //error
                    $scope.isInProcessing = false;
                    $csnotify.error(data.data);
                });
        };

        $scope.downloadInputFile = function (fileScheduler) {
            $scope.isInProcessing = true;
            var filename = fileScheduler.FileDirectory + '\\' + fileScheduler.FileName;
            var downloadpath = $csConstants.MVC_BASE_URL + "FileUploader/FileStatus/Download?fullfilename=" + filename + "";
            window.location = downloadpath;
            $scope.isInProcessing = false;

        };

        //#endregion

        //#region File Status Model

        $scope.openModelData = function (fileScheduler) {
            $scope.modalFileScheduler = fileScheduler;
            $scope.shouldBeOpen = true;
        };

        $scope.closeModel = function () {
            $scope.shouldBeOpen = false;
        };

        $scope.modelOption = {
            backdropFade: true,
            dialogFade: true,
            dialogClass: 'modal modal-huge'
        };

        $scope.CompuateTimeSpent = function (index) {
            if (index <= 0) {
                return "No Gap";
            }

            debugger;
            var prevTime = moment($scope.modalFileScheduler.FileStatuss[index - 1].EntryDateTime);
            var currTime = moment($scope.modalFileScheduler.FileStatuss[index].EntryDateTime);

            return moment.duration(currTime.diff(prevTime)).humanize();
        };

        $scope.CompuateTotalTimeSpent = function (index) {
            if (index <= 0) {
                return "No Gap";
            }

            debugger;
            var prevEntry = _.findWhere($scope.modalFileScheduler.FileStatuss, { 'UploadStatus': 'UploadStarted' });
            var prevTime = moment(prevEntry.EntryDateTime);
            var currTime = moment($scope.modalFileScheduler.FileStatuss[index].EntryDateTime);

            if (currTime.isBefore(prevTime)) {
                return "No Gap";
            }

            return moment.duration(currTime.diff(prevTime)).humanize();
        };

        //#endregion

        //#region Immediate Model

        $scope.modelImmediateOpen = function (fileScheduler, index) {
            $scope.ImmediatefileIndex = index;
            $scope.ImmediatefileScheduler = fileScheduler;
            $scope.openImidiateModel = true;
        };

        $scope.closeImidiateModel = function (fileScheduler) {
            fileScheduler.ImmediateReason = "";
            $scope.openImidiateModel = false;
        };

        $scope.imidiateModelOption = {
            backdropFade: true,
            dialogFade: true
        };

        //#endregion

    }
]);


