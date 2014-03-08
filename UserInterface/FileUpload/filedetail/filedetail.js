
csapp.controller("fileDetailsAddEditController", ["$scope", "Restangular", '$Validations', "$csfactory", "$csnotify", "$modalInstance", "fileDetails", "Logger",
    function ($scope, rest, $val, $csfactory, $csnotify, $modalInstance, fileDetails, logManager) {
        
        "use strict";
        
        var $log = logManager.getInstance("fileDetailsAddEditController");
        switch (fileDetails.displayMode) {
            case "add":
                $scope.modelTitle = "Add New File Details";
                $scope.isReadOnly = false;
                break;
            case "edit":
                $scope.modelTitle = "Add New File Details";
                $scope.isReadOnly = false;
                break;
            case "view":
                $scope.modelTitle = "";
                $scope.isReadOnly = true;
                break;
            default:
                $log.error("Invalid display mode : " + JSON.stringify(fileDetails));
        }

        $scope.fileCategories = fileDetails.enums.fileCategories;
        $scope.fileAliasNames = fileDetails.enums.fileAliasNames;
        //$scope.DependsOnAlias = fileDetails.enums.fileAliasNames;
        $scope.fileFrequencies = fileDetails.enums.fileFrequencies;
        $scope.fileTypes = fileDetails.enums.fileTypes;
        $scope.fileSystems = fileDetails.enums.fileSystems;
        
        $scope.isSheetTypeColumn = function (type) {
            if (type == 'xls' || type == 'xlsx') {
                return true;
            } else {
                return false;
            }
        };

        $scope.Depends = function () {
            if (angular.isDefined($scope.fileDetail.ScbSystems) && angular.isDefined($scope.fileDetail.Frequency)) {
                $scope.DependsOnAlias = _.pluck(_.filter($scope.fileDetailsList, function (fileDetail) {
                    return (fileDetail.ScbSystems == $scope.fileDetail.ScbSystems)
                        && (fileDetail.Frequency == $scope.fileDetail.Frequency);
                }), 'AliasName');
            }
        };

        var apictrl = rest.all('FileDetailsApi');
        
        $scope.fileDetail = fileDetails.fileDetail;
        
        $scope.closeModel = function (closer) {
            $modalInstance.dismiss(closer);
        };
        
        $scope.val = $val;

        //$scope.dateFormats = [{ format: 'dd-mm-yyyy', group: "date" }, { format: 'yyyy-mm-dd', group: "date" }, { format: 'dddd-MMMM-yyyy', group: "date" }];
        $scope.dateFormats = ['dd-mm-yyyy', 'yyyy-mm-dd', 'dddd-MMMM-yyyy'];
        $scope.UsedFor = ['Allocation', 'Billing'];
        $scope.Button = {
            save: { btnType: "save" },
            cancel: { btnType: "cancel" }
        };

        //for save file details
        $scope.saveFileDetails = function (fileDetail) {
            if (angular.isUndefined(fileDetail) || $csfactory.isEmptyObject(fileDetail)) {
                return;
            }
            fileDetail.FileServer = "localhost";
            $scope.fileDetail.TempTable = "TEMP_" + $scope.fileDetail.AliasName;
            $scope.fileDetail.ErrorTable = "ERROR_" + $scope.fileDetail.AliasName;
            if (fileDetail.Id) {
                apictrl.customPUT(fileDetail, 'Put', { id: fileDetail.Id }).then(function (data) {
                    $csnotify.success("File Detail Updated Successfully");
                    $modalInstance.close(data);
                }, function (response) {
                    $csnotify.error(response);
                });
            } else {
                apictrl.customPOST(fileDetail, 'Post').then(function (data) {
                    $csnotify.success("File Detail added Successfully");
                    $modalInstance.close(data);
                }, function (response) {
                    $csnotify.error(response);
                });
            }
        };
    }
]);

csapp.controller("fileDetailsController", ['$scope', '$csfactory', '$csnotify', 'Restangular', "modalService", "$modal",
    function ($scope, $csfactory, $csnotify, rest, modalService, $modal) {
        "use strict";

        var apictrl = rest.all('FileDetailsApi');
        
        //#region other Operations
        
        $scope.showDeleteModelPopup = function (fileDetail, index) {
            var modalOptions = {
                closeButtonText: 'Cancel',
                actionButtonText: 'Delete Customer',
                headerText: 'Delete ' + fileDetail.AliasName + '?',
                bodyText: 'Are you sure you want to delete this file?'
            };

            modalService.showModal({}, modalOptions).then(function () {
                $scope.deleteFileDetails(fileDetail.Id, index);
            });

        };

        $scope.showAddEditPopup = function (mode, fileDetails) {
            var modalInstance = $modal.open({
                templateUrl: 'filedetail/filedetailadd.html',
                controller: 'fileDetailsAddEditController',
                resolve: {
                    fileDetails: function() {
                        return {
                            fileDetail: angular.copy(fileDetails),
                            displayMode: mode,
                            enums: $scope.enums
                        };
                    }
                }
            });

            modalInstance.result.then(function(data) {
                if (mode === 'add') {
                    $scope.fileDetailsList.push(data);
                } else if (mode === 'edit') {
                    var index = _.findIndex($scope.fileDetailsList, { 'Id': fileDetails.Id });
                    $scope.fileDetailsList[index] = data;
                }
            });
        };

        //#endregion

        //#region Db operations

        //for getting all file details
        apictrl.customGET('Fetch').then(function (data) {
            $csnotify.success("All File Details Loaded Successfully.");
            $scope.fileDetailsList = data.FileDetails;
            $scope.enums = {
                fileCategories: data.FileCategories,
                fileAliasNames: data.FileAliasNames,
                dependsOnAlias: data.FileAliasNames,
                fileFrequencies: data.FileFrequencies,
                fileTypes: data.FileTypes,
                fileSystems: data.FileSystems
            };
        }, function (response) {
            $csnotify.error(response);
        });

        //for delete file details
        $scope.deleteFileDetails = function (id, index) {
            apictrl.customDELETE('Delete', { id: id }).then(function () {
                $csnotify.success('File Detail Deleted Successfully.');
                $scope.fileDetailsList.splice(index, 1);
            }, function (response) {
                $csnotify.error(response);
            });
        };

        //#endregion

    }]);
