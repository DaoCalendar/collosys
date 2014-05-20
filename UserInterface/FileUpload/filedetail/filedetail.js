
csapp.factory("fileDetailFactory", [function () {

    var isSheetTypeColumn = function (type) {
        var abc = (type === 'xls' || type === 'xlsx');
        return abc;
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

csapp.factory("fileDetailDataLayer", ["Restangular", "$csnotify", "$csfactory", function (rest, $csnotify, $csfactory) {

    var apictrl = rest.all('FileDetailsApi');
    var dldata = {};

    var errorDisplay = function (response) {
        $csnotify.error(response);
    };

    var getAllFileDetails = function () {
        apictrl.customGET('Fetch')
            .then(function (data) {
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

    var deleteFileDetails = function (id) {
        apictrl.customDELETE('Delete', { id: id }).then(function () {
            $csnotify.success('File Detail Deleted Successfully.');
            var index = _.findIndex(dldata.fileDetailsList, { 'Id': id });
            dldata.fileDetailsList.splice(index, 1);
        }, errorDisplay);
    };

    var saveFileDetails = function (fileDetail) {
        if (angular.isUndefined(fileDetail) || $csfactory.isEmptyObject(fileDetail)) {
            throw "Invalid fileDetail object";
        }

        fileDetail.FileServer = "localhost";
        fileDetail.TempTable = "TEMP_" + fileDetail.AliasName;
        fileDetail.ErrorTable = "ERROR_" + fileDetail.AliasName;

        if (fileDetail.Id) {
            return apictrl.customPUT(fileDetail, 'Put', { id: fileDetail.Id })
                .then(function (data) {
                    var index = _.findIndex(dldata.fileDetailsList, { Id: fileDetail.Id });
                    dldata.fileDetailsList[index] = data;
                    $csnotify.success("File Detail Updated Successfully");
                    return;
                }, errorDisplay);
        } else {
            return apictrl.customPOST(fileDetail, 'Post')
                .then(function (data) {
                    $csnotify.success("File Detail added Successfully");
                    dldata.fileDetailsList.push(data);
                    return data;
                    //$modalInstance.close();
                }, errorDisplay);
        }
    };

    var getFileDetails = function (detailsid) {
        return apictrl.customGET('Get', { id: detailsid })
            .then(function(data) {
                return data; 
            },
            errorDisplay);
    };

    return {
        Save: saveFileDetails,
        Delete: deleteFileDetails,
        GetAll: getAllFileDetails,
        dldata: dldata,
        Get: getFileDetails
        //reset: resetdata
    };
}]);

csapp.controller("fileDetailsAddEditController", ["$scope", "$routeParams",
    "fileDetailDataLayer", "fileDetailFactory", "Logger", "$csModels", "$location",
    function ($scope, $routeParams, datalayer, factory, logManager, $csModels, $location) {
        "use strict";

        var $log = logManager.getInstance("fileDetailsAddEditController");


        $scope.close = function () {
            $location.path("/fileupload/filedetail");
        };

        $scope.reset = function () {
            $scope.fileDetail = {};
        };

        $scope.add = function (fileDetail) {
            datalayer.Save(fileDetail).then(function () {
                $location.path("/fileupload/filedetail");
            });
        };

        $scope.updateDependsOnAlias = function (details) {
            if (angular.isUndefined(details)) details = {};
            details.dependsOnAliasList = factory.getDependsOnAlias(details, datalayer.dldata.fileDetailsList);
        };

        (function () {
            $scope.fileDetailModel = $csModels.getColumns("FileDetail");

            datalayer.Get($routeParams.id).then(function(data) {
                $scope.fileDetail = data;
            });
            $scope.isReadOnly = $routeParams.mode == 'view';

            if (angular.isUndefined($scope.fileDetail))
                $scope.fileDetail = {};
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            $scope.updateDependsOnAlias($scope.fileDetail);
        })();

        (function (mode) {
            switch (mode) {
                case "add":
                    $scope.modelTitle = "Add New File Details";
                    break;
                case "edit":
                    $scope.modelTitle = "Update File Details";
                    break;
                case "view":
                    $scope.modelTitle = "View File Details";
                    break;
                default:
                    throw ("Invalid display mode : " + JSON.stringify(fileDetails));
            }
            $scope.mode = mode;
        })($routeParams.mode);
    }
]);

csapp.controller("fileDetailsController", ['$scope', "modalService", "$location", "fileDetailDataLayer",
    function ($scope, modalService, $location, datalayer) {
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
            if (mode === "edit" || mode === "view") {
                $location.path("/fileupload/filedetail/addedit/" + mode + "/" + fileDetails.Id);
            } else {
                $location.path("/fileupload/filedetail/addedit/" + mode + "/" );
            }
        };
    }
]);
