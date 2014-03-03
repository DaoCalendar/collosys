(
csapp.controller("PincodeUpload", ["$scope", "Restangular", "$csnotify", "$csConstants", "csStopWatch",
    function ($scope, $restangular, $csnotify, $csConstants, stopWatch) {

        var pincodeapi = $restangular.all("PincodeUploadApi");

        //#region init

        var initializer = function () {
            //$scope.selectedProduct = {};
            $scope.showProgressBar = false;
            $scope.stopwatch = stopWatch;

            pincodeapi.customGETLIST("FetchProducts")
                .then(function (data) {
                    $scope.ProductList = data;
                }, function (data) {
                    $csnotify.error(data);
                });
        };
        initializer();

        //#endregion

        //#region download

        $scope.DownloadPincodes = function (product) {
            $scope.showProgressBar = true;
            $scope.stopwatch.start();
            $scope.showProgressBarMessage = "Downloading Missing Pincodes";

            pincodeapi.customGETLIST("FetchCustomerMissingPincodes", { 'product': product })
                .then(function (data) {
                    $scope.showProgressBar = false;
                    $scope.stopwatch.reset();
                    var downloadpath = $csConstants.MVC_BASE_URL + "OtherUploads/PincodeUpload/Download?fullfilename='" + data + "'";
                    window.location = downloadpath;
                }, function (data) {
                    $scope.showProgressBar = false;
                    $scope.stopwatch.reset();
                    $csnotify.error(data.data);
                });
        };

        //#endregion

        //#region upload

        $scope.DuringUpload = function (data, completed) {
            if (completed) {
                $scope.showProgressBar = false;
                $scope.stopwatch.reset();
                $csnotify.success("Updated Customer Pincode Information.");
            } else if ($scope.showProgressBar === false) {
                $scope.showProgressBarMessage = "Uploading Pincodes";
                $scope.showProgressBar = true;
                $scope.stopwatch.start();
            }
        };

        //#endregion
    }
])
);
