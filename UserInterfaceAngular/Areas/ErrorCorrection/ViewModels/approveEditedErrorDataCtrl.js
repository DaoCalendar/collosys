
(
csapp.controller("approveErrorDataCtrl", ["$scope", "$csnotify", 'Restangular', function ($scope, $csnotify, rest) {
    "use strict";

    $scope.gridReady = false;
    $scope.selectedRowIndex = 0;
    $scope.selectedRow = '';
    $scope.showModel = false;
    $scope.fileDetail = '';

    $scope.fileDetails = [];
    $scope.columns = [];

    //rest api
    var apireport = rest.all('ApproveEditedErrorDataApi');

    apireport.customGET('GetFileDetails').then(function (data) {
        $scope.fileDetails = data;
    }, function (data) {
        $csnotify.error(data);
    });

    $scope.showGridData = function (fileDetail) {
        $scope.gridReady = false;

        apireport.customGET('GetNgGridOptions', { file_detail_id: fileDetail.Id }).then(function (data) {
            $scope.setNgGridOptions(data);
            $scope.gridReady = true;
        }, function (data) {
            $csnotify.error(data);
        });
    };

    $scope.setNgGridOptions = function (data) {
        $scope.serverNgGridOptions = data;

        $scope.columns = [];

        $scope.columns.push({
            displayName: 'Action',
            cellTemplate: //'<div class="btn-toolbar" style="width: 70px">' +
                '<i class="btn icon-file-alt" data-ng-click="openModel(row.rowIndex)"></i>',// +
            //'</div>',
            width: 70
        });

        for (var i = 0; i < $scope.serverNgGridOptions.columnDefs.length; i++) {
            $scope.columns.push({
                field: $scope.serverNgGridOptions.columnDefs[i].field,
                displayName: $scope.serverNgGridOptions.columnDefs[i].displayName,
                width: $scope.serverNgGridOptions.columnDefs[i].width
            });
        }

        $scope.gridOptions = {
            data: 'serverNgGridOptions.data',
            columnDefs: $scope.columns,
            multiSelect: $scope.serverNgGridOptions.multiSelect,
            enableColumnResize: $scope.serverNgGridOptions.enableColumnResize,
            showSelectionCheckbox: $scope.serverNgGridOptions.showSelectionCheckbox,
            selectedItems: []
        };
    };


    $scope.ApproveRejectSelectedRows = function (approveRows, approved) {

        var saveData = {
            approved: approved,
            fileAliasName: $scope.fileDetail.AliasName,
            tableName: $scope.fileDetail.ErrorTable,
            data: approveRows
        };

        apireport.costomPOST(saveData, 'PostRows').then(function () {
            $scope.showGridData($scope.fileDetail);
        }, function (data) {
            $csnotify.error(data);
        });
    };


    $scope.approveRejectErrorData = function (row, approved, index, next) {

        var saveData = {
            approved: approved,
            fileAliasName: $scope.fileDetail.AliasName,
            tableName: $scope.fileDetail.ErrorTable,
            data: row
        };

        var saveSuccess = function () {
            $scope.serverNgGridOptions.data.splice(index, 1);
            $scope.closeModel();

            if (next) {
                $scope.openModel(index);
            }
        };

        apireport.costomPOST(saveData, 'PostRow').then(saveSuccess, function (data) {
            $csnotify.error(data);
        });
    };


    //#region

    $scope.changeSelectedRow = function (index) {
        $scope.selectedRowIndex = index;
        $scope.selectedRow = $scope.serverNgGridOptions.data[index];
    };

    $scope.openModel = function (index) {
        $scope.changeSelectedRow(index);
        $scope.showModel = true;
    };

    $scope.closeModel = function () {
        $scope.errorMessage = '';
        $scope.showModel = false;
    };

    //#endregion

}])
);


//var urlApproveError = "api/ApproveEditedErrorDataApi/";

//$http({
//    url: urlApproveError + "GetFileDetails",
//    method: "GET"
//}).success(function (data) {
//    $scope.fileDetails = data;
//}).error(function () {
//    toastr.error("file Details is not comming from database. Please try again", "Error");
//});



//$http({
//    url: urlApproveError + "GetNgGridOptions",
//    method: "GET",
//    params: { file_detail_id: fileDetail.Id }
//}).success(function (data) {
//    $scope.setNgGridOptions(data);
//    $scope.gridReady = true;
//}).error(function () {
//    toastr.error("Ng Grid Options is not comming from database. Please try again", "Error");
//});


//$http({
//    url: urlApproveError + "PostRows",
//    method: "POST",
//    data: saveData
//}).success(function () {
//    $scope.showGridData($scope.fileDetail);
//    toastr.success("Data Approved", "Success");
//}).error(function () {
//    toastr.error("Data Not Save. Please try again", "Error");
//});

//$http({
//    url: urlApproveError + "PostRow",
//    method: "POST",
//    data: saveData
//}).success(function () {
//    $scope.serverNgGridOptions.data.splice(index, 1);
//    $scope.closeModel();
//    toastr.success("Data Approved", "Success");

//    if (next) {
//        $scope.openModel(index);
//    }
//}).error(function () {
//    toastr.error("Data Not Save. Please try again", "Error");
//});