
csapp.controller("fileDetailsController", ['$scope', '$csfactory', '$csnotify', 'Restangular',
    '$Validations', function($scope, $csfactory, $csnotify, rest, $val) {

        "use strict";

        var apictrl = rest.all('FileDetailsApi');

        $scope.val = $val;

        $scope.dateFormats = [{ format: 'dd-mm-yyyy', group: "date" }, { format: 'yyyy-mm-dd', group: "date" }, { format: 'dddd-MMMM-yyyy', group: "date" }];
        // $scope.dateFormats = ['dd-mm-yyyy', 'yyyy-mm-dd', 'dddd-MMMM-yyyy'];
        $scope.UsedFor = ['Allocation', 'Billing'];
        $scope.Button = {
            save: { btnType: "save" },
            cancel: { btnType: "cancel" }
        };


        //#region other Operations
        $scope.reset = function() {
            $scope.modelTitle = "Add New File Details";
            $scope.isReadOnly = false;
            $scope.editIndex = -1;
            $scope.fileDetail = {};
        };

        $scope.AddinLocal = function(temp) {
            if ($scope.editIndex === -1) {
                $scope.fileDetailsList.push(temp);
            } else {
                $scope.fileDetailsList[$scope.editIndex] = temp;
            }
        };

        $scope.openModel = function() {
            $scope.reset();
            $scope.DependsOnAlias = [];
            $scope.shouldBeOpen = true;
        };

        $scope.closeModel = function() {
            $scope.shouldBeOpen = false;
        };

        $scope.modelOption = {
            backdropFade: true,
            dialogFade: true
        };

        $scope.openModelData = function(fileDetailRow, readOnly, index) {
            if (fileDetailRow.Id && readOnly) {
                $scope.modelTitle = "View File Details";
            }
            if (!fileDetailRow.Id) {
                $scope.modelTitle = "Add New File Details";
            }

            if (fileDetailRow.Id && !readOnly) {
                $scope.modelTitle = "Update File Details";
            }
            $scope.isReadOnly = readOnly;
            $scope.fileDetail = angular.copy(fileDetailRow);
            $scope.shouldBeOpen = true;
            $scope.editIndex = index;
        };

        $scope.yesToDelete = function() {
            $scope.deleteFileDetails($scope.deletedFile.Id, $scope.editIndex);
            $scope.showDeleteModel = false;
        };

        $scope.noToDelete = function() {
            $scope.deletedFile = {};
            $scope.editIndex = -1;
            $scope.showDeleteModel = false;
        };

        $scope.showDeleteModelPopup = function(file, index) {
            $scope.deletedFile = file;
            $scope.editIndex = index;
            $scope.showDeleteModel = true;
        };

        $scope.isSheetTypeColumn = function(type) {
            if (type == 'xls' || type == 'xlsx') {
                return true;
            } else {
                return false;
            }
        };

        $scope.Depends = function() {
            if (angular.isDefined($scope.fileDetail.ScbSystems) && angular.isDefined($scope.fileDetail.Frequency)) {
                $scope.DependsOnAlias = _.pluck(_.filter($scope.fileDetailsList, function(fileDetail) {
                    return (fileDetail.ScbSystems == $scope.fileDetail.ScbSystems)
                        && (fileDetail.Frequency == $scope.fileDetail.Frequency);
                }), 'AliasName');
            }
        };


        $scope.StepNames = [
            'addfiledetail',
            'addfilecolumn',
            'addfilemapping'
        ];

        $scope.HasNextStep = function() {
            debugger;
            var index = $scope.StepNames.indexOf($scope.currentStep);
            $scope.currentStep = $scope.StepNames[index + 1];
        };


        $scope.HasPrevStep = function() {
            debugger;
            var index = $scope.StepNames.indexOf($scope.currentStep);
            $scope.currentStep = $scope.StepNames[index - 1];
        };


        $scope.init = function() {
            //$scope.NextFileColumn = false;
            //$scope.NextFileMapping = false;
            //$scope.PreviousFileColumn = false;
            $scope.currentStep = 'addfiledetail';
        };

        $scope.init();

        //#endregion

        //#region Db operations

        //for getting all file details
        apictrl.customGET('Fetch').then(function(data) {
            $csnotify.success("All File Details Loaded Successfully.");
            $scope.fileDetailsList = data.FileDetails;
            $scope.fileCategories = data.FileCategories;
            $scope.fileAliasNames = data.FileAliasNames;
            //$scope.DependsOnAlias = data.FileAliasNames;
            $scope.fileFrequencies = data.FileFrequencies;
            $scope.fileTypes = data.FileTypes;
            $scope.fileSystems = data.FileSystems;
        }, function(response) {
            $csnotify.error(response);
        });


        //for save file details
        $scope.saveFileDetails = function(fileDetail) {
            if (angular.isUndefined(fileDetail) || $csfactory.isEmptyObject(fileDetail)) {
                return;
            }
            fileDetail.FileServer = "localhost";
            $scope.fileDetail.TempTable = "TEMP_" + $scope.fileDetail.AliasName;
            $scope.fileDetail.ErrorTable = "ERROR_" + $scope.fileDetail.AliasName;
            if (fileDetail.Id) {
                apictrl.customPUT(fileDetail, 'Put', { id: fileDetail.Id }).then(function(data) {
                    $csnotify.success("File Detail Updated Successfully");
                    $scope.AddinLocal(data);
                    $scope.reset();
                    $scope.closeModel();
                }, function(response) {
                    $csnotify.error(response);
                });
            } else {
                apictrl.customPOST(fileDetail, 'Post').then(function(data) {
                    $csnotify.success("File Detail added Successfully");
                    $scope.AddinLocal(data);
                    $scope.reset();
                    $scope.closeModel();
                }, function(response) {
                    $csnotify.error(response);
                });
            }
        };

        //for delete file details
        $scope.deleteFileDetails = function(id, index) {
            apictrl.customDELETE('Delete', { id: id }).then(function() {
                $csnotify.success('File Detail Deleted Successfully.');
                $scope.fileDetailsList.splice(index, 1);
            }, function(response) {
                $csnotify.error(response);
            });
        };

        //#endregion

    }]);
