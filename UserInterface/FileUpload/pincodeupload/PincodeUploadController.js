
csapp.controller("PincodeUploadController1", ["$scope", "Restangular", "$csnotify", "$csConstants", "csStopWatch",
    function ($scope, rest, $csnotify, $csConstants, stopWatch) {


        //#region init
        var pincodeapi = rest.all("PincodeUploadApi");

        var initializer = function () {
            //$scope.selectedProduct = {};
            $scope.showProgressBar = false;
            $scope.stopwatch = stopWatch;
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
]);

csapp.factory("pincodeUploadDatalayer", ["Restangular", "$csnotify", "$csConstants", function (rest, $csnotify, $csConstants) {

    var dldata = {};
    var pincodeapi = rest.all("PincodeUploadApi");

    return {
        dldata:dldata  
    };
}]);

csapp.factory("pincodeUploadFactory", ["$scope","pincodeUploadDatalayer", function ($scope,datalayer) {
    $scope.dldata = datalayer.dldata;
}]);

csapp.controller("PincodeUploadController", ["$scope", "pincodeUploadDatalayer", "pincodeUploadFactory"], function ($scope, datalayer, factory) {

    (function () {
        $scope.dldata = datalayer.dldata;
        $scope.factory = factory;
    })();
});