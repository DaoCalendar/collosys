(
csapp.controller("RcodeUpload", ["$scope", "Restangular", "$csnotify", "$csConstants", "csStopWatch",
    function ($scope, $restangular, $csnotify, $csConstants, stopWatch) {

        var rcodeapi = $restangular.all("RcodeUploadApi");

        //#region init

        var initializer = function () {
            //$scope.selectedProduct = {};
            $scope.showProgressBar = false;
            $scope.stopwatch = stopWatch;

            rcodeapi.customGETLIST("FetchProducts")
                .then(function (data) {
                    $scope.ProductList = data;
                }, function (data) {
                    $csnotify.error(data);
                });
        };
        initializer();

        //#endregion

        //#region download

        $scope.DownloadRcodes = function (product) {
            $scope.showProgressBar = true;
            $scope.stopwatch.start();
            $scope.showProgressBarMessage = "Downloading Missing Rcodes";

            rcodeapi.customGETLIST("FetchCustomerMissingRcodes", { 'product': product })
                .then(function (data) {
                    $scope.showProgressBar = false;
                    $scope.stopwatch.reset();
                    var downloadpath = $csConstants.MVC_BASE_URL + "OtherUploads/RcodeUpload/Download?fullfilename='" + data + "'";
                    window.location = downloadpath;
                }, function (data) {
                    $scope.showProgressBar = false;
                    $scope.stopwatch.reset();
                    $csnotify.error(data);
                });
        };

        //#endregion

        //#region upload

        $scope.DuringUpload = function (data, completed) {
            if (completed) {
                $scope.showProgressBar = false;
                $scope.stopwatch.reset();
                $csnotify.success("Updated Customer Rcode Information.");
            } else if ($scope.showProgressBar === false) {
                $scope.showProgressBarMessage = "Uploading Rcodes";
                $scope.showProgressBar = true;
                $scope.stopwatch.start();
            }
        };

        //#endregion
    }
])
);
