
csapp.directive("csFileUpload", ["Restangular", "Logger", "$csfactory", "$upload",
    function (rest, logManager, $csfactory, $upload) {
        //var $log = logManager.getInstance("csFileUploadDirective");
        var getFileInputTemplate = function () {
            return '<div ng-form="" name="myform">' +
                        '<div data-ng-show="fileInfo.isUploading">' +
                            '<progressbar class="progress-striped active" value="fileInfo.uploadPercent" ' +
                                'type="success"></progressbar>' +
                            '<div class="text-error">Copying file to server!!!</div>' +
                        '</div>' +
                        '<div data-ng-hide="fileInfo.isUploading">' +
                            '<input name="myfield" ng-model="ngModel" type="file" ' +
                                'ng-file-select="copyToServer($files)" ng-required="validations.required" />' +
                        '</div>' +
                        '<div data-ng-show="valerror.$invalid">' +
                            '<div class="text-error" data-ng-show="valerror.$error.nonempty">Please provide non-empty files</div>' +
                            '<div class="text-error" data-ng-show="valerror.$error.extension">Please select {{validations.extension}} file.</div>' +
                            '<div class="text-error" data-ng-show="valerror.$error.pattern">Pattern {{validations.pattern}} mismatch.</div>' +
                            '<div class="text-error" data-ng-show="valerror.$error.required">Please select a file.</div>' +
                        '</div>' +
                    '</div>';
        };

        var setParams = function (cfile, file) {
            file.name = cfile.name;
            file.size = cfile.size;
        };

        var saveFileOnServer = function (scope, ngModel) {
            scope.fileInfo.isUploading = true;
            scope.fileInfo.copied = false;
            ngModel.$setValidity("noncopying", false);

            $upload.upload({
                url: '/api/FileIoApi/SaveFile',
                method: "Post",
                file: scope.cfile
            }).progress(function (evt) {
                scope.fileInfo.uploadPercent = parseInt(100.0 * evt.loaded / evt.total);
            }).success(function (data) {
                scope.fileInfo.path = data.FullPath;
                scope.fileInfo.isUploading = false;
                scope.fileInfo.copied = true;
                if (angular.isFunction(scope.onSave)) {
                    scope.onSave({ 'fileInfo': scope.fileInfo });
                }
                ngModel.$setValidity("noncopying", true);
            }).error(function () {
                scope.fileInfo.isUploading = false;
                scope.onSave({ 'fileInfo': scope.fileInfo });
                ngModel.$setValidity("noncopying", true);
            });
        };

        var linkFunction = function (scope, element, attr, ngModel) {
            ngModel.$render = function (filename) {
                ngModel.$setViewValue(filename);
            };

            if (angular.isUndefined(scope.fileInfo)) {
                throw "please provide file info.";
            }

            scope.valerror = {
                $invalid: false,
                $error: {},
                add: function (prop) {
                    scope.valerror.$invalid = true;
                    scope.valerror.$error[prop] = true;
                },
                reset: function() {
                    scope.valerror.$invalid = false;
                    scope.valerror.$error = {};
                }
            };
            scope.isFileValid = function () {
                scope.valerror.reset();
                ngModel.$setValidity("pattern", true);
                ngModel.$setValidity("extension", true);
                ngModel.$setValidity("nonEmpty", true);
                ngModel.$setValidity("required", true);
                if (angular.isUndefined(scope.validations)) {
                    return true;
                }

                if (scope.validations.required === true) {
                    if ($csfactory.isNullOrEmptyString(scope.fileInfo.name)) {
                        ngModel.$setValidity("required", false);
                        scope.valerror.add("required");
                        return false;
                    }
                }

                if (!$csfactory.isNullOrEmptyString(scope.validations.pattern)) {
                    if (!scope.fileInfo.name.match(scope.validations.pattern)) {
                        ngModel.$setValidity("pattern", false);
                        scope.valerror.add("pattern");
                        return false;
                    }
                }

                if (scope.fileInfo.size === 0) {
                    ngModel.$setValidity("nonempty", false);
                    scope.valerror.add("nonempty");
                    return false;
                }

                if (!$csfactory.isNullOrEmptyString(scope.validations.extension)) {
                    var extension = scope.fileInfo.name.substring(scope.fileInfo.name.lastIndexOf('.') + 1);
                    if (extension !== scope.validations.extension) {
                        ngModel.$setValidity("extension", false);
                        scope.valerror.add("extension");
                        return false;
                    }
                }

                return true;
            };

            scope.isFileValid();

            scope.copyToServer = function ($files) {
                scope.cfile = $files[0];
                setParams(scope.cfile, scope.fileInfo);
                ngModel.$render(scope.fileInfo.name);
                if (scope.isFileValid()) {
                    saveFileOnServer(scope, ngModel);
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
                extension: "xlsx",
                required: true
            };
            $scope.selected = {
                fileInfo: {}
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
