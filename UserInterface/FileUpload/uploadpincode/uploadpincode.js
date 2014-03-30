
csapp.directive("csFileUpload", ["Restangular", "Logger", "$csfactory", "$upload",
    function (rest, logManager, $csfactory, $upload) {
        var $log = logManager.getInstance("csFileUploadDirective");
        var getFileInputTemplate = function () {
            return '<div ng-form="myform">' +
                        '<div data-ng-show="fileInfo.isUploading">' +
                            '<progressbar class="progress-striped active" value="fileInfo.uploadPercent" ' +
                                'type="success"></progressbar>' +
                        '</div>' +
                        '<div data-ng-hide="fileInfo.isUploading">' +
                            '<input name="myfield" ng-model="ngModel" type="file" ' +
                                'ng-file-select="copyToServer($files)" />' +
                        '</div>' +
                        '<div data-ng-show="myform.myfield.$invalid"> ' +
                            '<span data-ng-show="myform.myfield.$error.nonEmpty">Please provide non-empty files</span>' +
                            '<span data-ng-show="myform.myfield.$error.extension">Please select {{val.extension}} file.</span>' +
                            '<span data-ng-show="myform.myfield.$error.pattern">Pattern {{val.pattern}} mismatch.</span>' +
                        '</div>' +
                    '</div>';
        };

        var isFileValid = function (file, validations, ctrl) {
            ctrl.$setValidity("pattern", true);
            ctrl.$setValidity("extension", true);
            ctrl.$setValidity("nonEmpty", true);

            if (!$csfactory.isNullOrEmptyString(validations.pattern)) {
                if (!file.name.match(validations.pattern)) {
                    ctrl.$setValidity("pattern", false);
                    $log.error("pattern mismatch");
                    return false;
                }
            }

            if (file.size === 0) {
                ctrl.$setValidity("nonEmpty", false);
                $log.error("zero size file");
                return false;
            }

            if (!$csfactory.isNullOrEmptyString(validations.extension)) {
                var extension = substring(file.name.lastIndexOf('.') + 1);
                if (extension !== validations.extension) {
                    ctrl.$setValidity("extension", false);
                    $log.error("extension mismatch");
                    return false;
                }
            }

            $log.info("file valid");
            return true;
        };

        var setParams = function (cfile) {
            var file = {};
            file.name = cfile.name;
            file.size = cfile.size;
            return file;
        };

        var saveFileOnServer = function (cfile, scope) {
            $log.info("copying file to server");
            scope.fileInfo.isUploading = true;
            $upload.upload({
                url: '/api/FileSchedulerApi/SaveFileOnServer',
                method: "Post",
                file: cfile
            }).progress(function (evt) {
                scope.fileInfo.uploadPercent = parseInt(100.0 * evt.loaded / evt.total);
            }).success(function (data) {
                scope.fileInfo.path = data.path;
                scope.fileInfo.name = data.name;
                scope.fileInfo.isUploading = false;
                scope.fileInfo.success = true;
                $log.info("copying done");
                scope.onSave({ 'fileInfo': scope.fileInfo });
            }).error(function () {
                scope.fileInfo.isUploading = false;
                scope.fileInfo.success = false;
                $log.info("copying failed");
                scope.onSave({ 'fileInfo': scope.fileInfo });
            });
        };

        var linkFunction = function (scope, element, attr, ctrl) {
            scope.copyToServer = function ($files) {
                var cfile = $files[0];
                ctrl.$setViewValue(cfile.name);
                scope.fileInfo = setParams(cfile);
                if (isFileValid(cfile, scope.validations, ctrl)) {
                    saveFileOnServer(cfile, scope);
                }
            };
        };

        return {
            scope: { onSave: '&', ngModel: '=', fileInfo: '=', validations: '=' },
            restrict: 'E',
            template: getFileInputTemplate,
            link: linkFunction,
            require: 'ngModel'
        };
    }]);


csapp.factory("uploadPincodeDatalayer", ["Restangular", "$csnotify", function (rest, $csnotify) {
    var pincodeapi = rest.all("PincodeUploadApi");
    var dldata = {};

    var fetchProducts = function () {
        pincodeapi.customGETLIST("FetchProducts")
            .then(function (data) {
                dldata.ProductList = data;
            }, function (data) {
                $csnotify.error(data);
            });
    };

    return {
        dldata: dldata,
        fetchProducts: fetchProducts
    };
}]);

csapp.controller("uploadPincodeController", ["$scope", "uploadPincodeDatalayer",
    function ($scope, datalayer) {

        (function () {
            datalayer.fetchProducts();
            $scope.dldata = datalayer.dldata;
            $scope.fileValidations = {
                extensions: ".xls"
            };
        })();

        $scope.fileCopied = function (fileInfo) {
            console.log(fileInfo);
        };

    }
]);


////#region download

//$scope.DownloadPincodes = function (product) {
//    $scope.showProgressBar = true;
//    $scope.stopwatch.start();
//    $scope.showProgressBarMessage = "Downloading Missing Pincodes";

//    pincodeapi.customGETLIST("FetchCustomerMissingPincodes", { 'product': product })
//        .then(function (data) {
//            $scope.showProgressBar = false;
//            $scope.stopwatch.reset();
//            var downloadpath = $csConstants.MVC_BASE_URL + "OtherUploads/PincodeUpload/Download?fullfilename='" + data + "'";
//            window.location = downloadpath;
//        }, function (data) {
//            $scope.showProgressBar = false;
//            $scope.stopwatch.reset();
//            $csnotify.error(data.data);
//        });
//};

////#endregion

////#region upload

//$scope.DuringUpload = function (data, completed) {
//    if (completed) {
//        $scope.showProgressBar = false;
//        $scope.stopwatch.reset();
//        $csnotify.success("Updated Customer Pincode Information.");
//    } else if ($scope.showProgressBar === false) {
//        $scope.showProgressBarMessage = "Uploading Pincodes";
//        $scope.showProgressBar = true;
//        $scope.stopwatch.start();
//    }
//};

////#endregion
