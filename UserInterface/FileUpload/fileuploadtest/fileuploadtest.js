csapp.controller("fileUploadTestController", [
    "$scope", "$upload", function ($scope, $upload) {

        $scope.fileUploadObj = { testString1: "Test string 1", testString2: "Test string 2" };

        $scope.onFileSelect = function ($files) {
            var cfile = $files[0];
            $scope.upload = $upload.upload({
                url: '/api/FileTransfer/Upload',
                method: "Post",
                data: { fileUploadObj: $scope.fileUploadObj },
                file: cfile,
            }).progress(function (evt) {
                console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
            }).success(function (data, status, headers, config) {
                // file is uploaded successfully
                console.log(data);
            }).error(function (data, status, headers, config) {
                // file failed to upload
                console.log(data);
            });
        };

        $scope.abortUpload = function(index) {
            $scope.upload[index].abort();
        };
    }
]);