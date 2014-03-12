
csapp.factory("fileDetailFactory", [function () {

    var isSheetTypeColumn = function (type) {
        return (type == 'xls' || type == 'xlsx');
    };

    var dateFormats = ['dd-mm-yyyy', 'yyyy-mm-dd', 'dddd-MMMM-yyyy'];
    var usedFor = ['Allocation', 'Billing'];

    var depends = function (file, list) {
        if (angular.isUndefined(file)) return [];
        var system = file.ScbSystems, frequency = file.Frequency;
        if (angular.isDefined(system) && angular.isDefined(frequency)) {
            return _.pluck(_.filter(list, function (fileDetail) {
                return (fileDetail.ScbSystems == system) && (fileDetail.Frequency == frequency);
            }), 'AliasName');
        }
        return [];
    };

    return {
        isSheetTypeColumn: isSheetTypeColumn,
        dateFormats: dateFormats,
        usedFor: usedFor,
        getDependsOnAlias: depends
    };
}]);

csapp.factory("fileDetailDataLayer", ["Restangular", "$csnotify", "$csfactory", function(rest, $csnotify, $csfactory) {

    var apictrl = rest.all('FileDetailsApi');
    var dldata = {};

    var errorDisplay = function(response) {
        $csnotify.error(response);
    };

    var getAllFileDetails = function() {
        apictrl.customGET('Fetch')
            .then(function(data) {
                $csnotify.success("All File Details Loaded Successfully.");
                dldata.fileDetailsList = data.FileDetails;
                dldata.enums = {
                    fileCategories: data.FileCategories,
                    fileAliasNames: data.FileAliasNames,
                    dependsOnAlias: data.FileAliasNames,
                    fileFrequencies: data.FileFrequencies,
                    fileTypes: data.FileTypes,
                    fileSystems: data.FileSystems
                };
            }, errorDisplay);
    };

    var deleteFileDetails = function(id) {
        apictrl.customDELETE('Delete', { id: id }).then(function() {
            $csnotify.success('File Detail Deleted Successfully.');
            var index = _.findIndex(dldata.fileDetailsList, { 'Id': id });
            dldata.fileDetailsList.splice(index, 1);
        }, errorDisplay);
    };

    var saveFileDetails = function(fileDetail) {
        if (angular.isUndefined(fileDetail) || $csfactory.isEmptyObject(fileDetail)) {
            throw "Invalid fileDetail object";
        }

        fileDetail.FileServer = "localhost";
        fileDetail.TempTable = "TEMP_" + fileDetail.AliasName;
        fileDetail.ErrorTable = "ERROR_" + fileDetail.AliasName;
        if (fileDetail.Id) {
            return apictrl.customPUT(fileDetail, 'Put', { id: fileDetail.Id })
                .then(function(data) {
                    var index = _.findIndex(dldata.fileDetailsList, { Id: fileDetail.Id });
                    dldata.fileDetailsList[index] = data;
                    $csnotify.success("File Detail Updated Successfully");
                }, errorDisplay);
        } else {
            return apictrl.customPOST(fileDetail, 'Post')
                .then(function() {
                    $csnotify.success("File Detail added Successfully");
                    dldata.fileDetailsList.push(data);
                }, errorDisplay);
        }
    };

    return {
        Save: saveFileDetails,
        Delete: deleteFileDetails,
        GetAll: getAllFileDetails,
        dldata: dldata
    };
}]);

csapp.controller("fileDetailsAddEditController", ["$scope", '$Validations', "$modalInstance", "fileDetails", "fileDetailDataLayer", "fileDetailFactory",
    function ($scope, $val, $modalInstance, fileDetails, datalayer, factory) {
        "use strict";

        $scope.close = function (closer) {
            $modalInstance.dismiss(closer);
        };

        $scope.add = function (fileDetail) {
            datalayer.Save(fileDetail).then(function (data) {
                $modalInstance.close(data);
            });
        };

        $scope.updateDependsOnAlias = function (details) {
            if (angular.isUndefined(details)) details = {};
            details.dependsOnAliasList = factory.getDependsOnAlias(details, datalayer.dldata.fileDetailsList);
            console.log(details.dependsOnAliasList);
        };

        (function () {
            $scope.fileDetail = fileDetails.fileDetail;
            $scope.val = $val;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            $scope.updateDependsOnAlias($scope.fileDetail);
        })();

        (function (mode) {
            switch (mode) {
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
                    throw ("Invalid display mode : " + JSON.stringify(fileDetails));
            }
        })(fileDetails.displayMode);
    }
]);

csapp.controller("fileDetailsController", ['$scope', "modalService", "$modal", "fileDetailDataLayer",
    function ($scope, modalService, $modal, datalayer) {
        "use strict";

        (function () {
            $scope.datalayer = datalayer;
            datalayer.GetAll();
        })();

        $scope.showDeleteModelPopup = function (fileDetail) {
            var modalOptions = {
                closeButtonText: 'Cancel',
                actionButtonText: 'Delete File',
                headerText: 'Delete ' + fileDetail.AliasName + '?',
                bodyText: 'Are you sure you want to delete this file?'
            };

            modalService.showModal({}, modalOptions).then(function () {
                datalayer.Delete(fileDetail.Id);
            });
        };

        $scope.showAddEditPopup = function (mode, fileDetails) {
            $modal.open({
                templateUrl: 'filedetail/file-detail-add.html',
                controller: 'fileDetailsAddEditController',
                resolve: {
                    fileDetails: function () {
                        return {
                            fileDetail: angular.copy(fileDetails),
                            displayMode: mode
                        };
                    }
                }
            });
        };
    }
]);
