
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

    var fetchMissingPincodes = function (product) {
        return pincodeapi.customGET("FetchMissingPincodes", { 'product': product })
            .then(function (data) {
                return data;
            }, function (error) {
                $csnotify.error(error.Message);
            });
    };

    var fetchMissingRcodes = function (product) {
        return pincodeapi.customGET("FetchMissingRcodes", { 'product': product })
            .then(function (data) {
                return data;
            }, function (error) {
                $csnotify.error(error.Message);
            });
    };

    var uploadPincodes = function (uploadInfo) {
        return pincodeapi.customPOST(uploadInfo, "UploadPincode")
            .then(function (data) {
                return data;
            }, function (error) {
                $csnotify.error(error.Message);
            });
    };

    var uploadRcodes = function (uploadInfo) {
        return pincodeapi.customPOST(uploadInfo, "UploadRcode")
            .then(function (data) {
                return data;
            }, function (error) {
                $csnotify.error(error.Message);
            });
    };

    return {
        dldata: dldata,
        fetchProducts: fetchProducts,
        fetchMissingPincodes: fetchMissingPincodes,
        fetchMissingRcodes: fetchMissingRcodes,
        uploadRcodes: uploadRcodes,
        uploadPincodes: uploadPincodes
    };
}]);

csapp.controller("uploadPincodeController", ["$scope", "uploadPincodeDatalayer", "$csfactory", "dataService",
    function ($scope, datalayer, $csfactory, mode) {
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
            $scope.mode = mode;
        })();

        $scope.upload = function () {
            var uploadInfo = {
                Product: $scope.selected.Product,
                FileName: $scope.selected.fileInfo.path
            };
            if (mode === "pincode") {
                datalayer.uploadPincodes(uploadInfo);
            } else {
                datalayer.uploadRcodes(uploadInfo);
            }
        };

        $scope.download = function (product) {
            if (mode === "pincode") {
                datalayer.fetchMissingPincodes(product).then(function (filename) {
                    $csfactory.downloadFile(filename);
                });
            } else {
                datalayer.fetchMissingRcodes(product).then(function (filename) {
                    $csfactory.downloadFile(filename);
                });
            }
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
