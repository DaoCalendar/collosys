
csapp.factory("fileColumnFactory", function () {

    var getColumnType = function (col) {
        var column = col.toUpperCase();
        if (column.indexOf("ACCOUNT") > -1) {
            return "Number";
        } else if (column.indexOf("NO") > -1) {
            return "Number";
        } else if (column.indexOf("AMOUNT") > -1) {
            return "Amount";
        } else if (column.indexOf("DATE") > -1) {
            return "Date";
        } else {
            return "String";
        }
    };

    var getTempColumnName = function (str) {
        var tempColName = str.replace(/\s+/g, '').replace("(", '')
            .replace(")", '').replace(".", '').replace("@", '')
            .replace(/["'/*]/g, '').replace("-", '_');

        var num1 = tempColName.match(/\d+\_?\d*/g);

        if (num1 !== null) {
            tempColName = tempColName.replace(/\d+\_?\d*/g, '') + num1; ///\d+\_?\d*/g
        }

        return tempColName;
    };

    var dateFormats = ['dd-mm-yyyy', 'yyyy-mm-dd', 'mm-dd-yyyy'];

    var getEndDate = function (startdate) {
        return new Date(startdate.getTime() + (24 * 60 * 60 * 1000));
    };

    var checkEndDate = function (startDate, endDate) {
        return startDate === endDate;
    };

    var isDateTypeColumn = function (type) {
        return (type === 'Date');
    };

    var moveUpward = function (index, columns) {
        var list = _.sortBy(columns, 'Position');
        if (index != 0) {
            list[index].Position = index;
            list[index - 1].Position = index + 1;
        }
    };

    var moveDownward = function (index, columns) {
        var list = _.sortBy(columns, 'Position');
        if (index != list.length - 1) {
            list[index].Position = index + 2;
            list[index + 1].Position = index + 1;
        }
    };

    var convertData = function (columns, fileDetail) {
        var listOfColumns = columns.split('\t');

        var maxPosition = fileDetail.FileColumns.length > 0 ? _.max(_.pluck(fileDetail.FileColumns, "Position")) : 0;
        for (var i = 0; i < listOfColumns.length; i++) {

            var tempcolumn = getTempColumnName(listOfColumns[i]);
            var type = getColumnType(tempcolumn);
            var tempdate = new Date();
            var position = maxPosition + (i + 1);

            var existTempColumn = _.find(fileDetail.FileColumns, { TempColumnName: tempcolumn });

            if (existTempColumn) {
                tempcolumn = tempcolumn + '_' + position;
            }

            var fileColumn = {
                FileColumnName: listOfColumns[i],
                Position: position,
                Length: 0,
                ColumnDataType: type,
                TempColumnName: tempcolumn,
                DateFormat: "",
                Description: "",
                FileDetail: "",
                StartDate: new Date(tempdate.getFullYear(), tempdate.getMonth(), tempdate.getDate())
            };

            fileDetail.FileColumns.push(fileColumn);
        }
    };

    return {
        getColumnType: getColumnType,
        getTempColumnName: getTempColumnName,
        dateFormats: dateFormats,
        getEndDate: getEndDate,
        checkEndDate: checkEndDate,
        isDateTypeColumn: isDateTypeColumn,
        moveUpward: moveUpward,
        moveDownward: moveDownward,
        convertData: convertData
    };
});

csapp.factory("fileColumnDataLayer", ["Restangular", "$csnotify", function (restApi, $csnotify) {

    var dldata = {};
    var apictrl = restApi.all('FileColumnApi');

    var getFileColumns = function (aliasName) {
        if (!aliasName) { return; }
        dldata.fileDetail = {};

        apictrl.customGET("GetFileColumns", { aliasName: aliasName }).then(function (data) {
            if (!data) { return; }
            dldata.fileDetail = data;
            dldata.fileDetail.FileColumns = _.sortBy(dldata.fileDetail.FileColumns, 'Position');
            $csnotify.success("All File Columns Loaded Successfully");
        }, errorDisplay);
    };

    var getAliasList = function () {
        apictrl.customGET('Fetch').then(function (data) {
            if (!data) { return; }
            dldata.aliasList = data.FileNames;
            dldata.dataTypes = data.ColumnDataTypes;
        }, errorDisplay);
    };

    var saveAllColumns = function () {
        if (!dldata.fileDetail) { return; }
        apictrl.customPOST(dldata.fileDetail, 'SaveAllColumns')
            .then(function (data) {
                dldata.fileDetail = data;
                $csnotify.success('All File Columns Save Successfully');
            }, errorDisplay);
    };

    var deleteFileColumn = function (id) {
        var index = _.findIndex(dldata.fileDetail.FileColumns, { Id: id });
        var pos = dldata.fileDetail.FileColumns[index].Position;

        if (id) {
            apictrl.customDELETE('Delete', { id: id }).then(function () {

                dldata.fileDetail.FileColumns.splice(index, 1);
                $csnotify.success('File Column Deleted Successfully');
                var list = _.sortBy(dldata.fileDetail.FileColumns, 'Position');
                for (var j = index; j < list.length; j++) {
                    list[j].Position = j + 1;
                }

            }, errorDisplay);
        } else {
            dldata.fileDetail.FileColumns.splice(index, 1);
        }

        for (var i = index + 1; i < dldata.fileDetail.FileColumns.length; i++) {
            dldata.fileDetail.FileColumns[i].Position = pos;
            pos++;
        }
    };

    var saveFileColumn = function (fileColumn, fileDetail) {
         
        if (!(fileColumn && fileDetail)) { return; }

        fileColumn.FileDetail = fileDetail;
        if (fileColumn.Id) {
            var saveFileColumnList = [];
            var list = _.sortBy(fileDetail.FileColumns, 'Position');

            var oldFileDetail = _.find(list, { Position: fileColumn.Position });
            var newFileDetail = _.find(list, { Id: fileColumn.Id });

            if (oldFileDetail && oldFileDetail.Id != newFileDetail.Id)
                oldFileDetail.Position = newFileDetail.Position;

            saveFileColumnList.push(fileColumn);

            apictrl.customPUT(saveFileColumnList, 'PutFileColumn', { id: fileColumn.Id })
                .then(function (data) {
                    dldata.fileDetail.FileColumns.push(data);
                    $csnotify.success("File Column Updated Successfully");
                }, errorDisplay);
        } else {
            apictrl.customPOST(fileColumn, 'Post')
                .then(function (data) {
                    dldata.fileDetail.FileColumns.push(data);
                    $csnotify.success("File Column Added Successfully");
                }, errorDisplay);
        }
    };

    var errorDisplay = function (response) {
        $csnotify.error("Error : " + response.Message);
    };

    return {
        dldata: dldata,
        GetAll: getFileColumns,
        Save: saveFileColumn,
        SaveMulti: saveAllColumns,
        Delete: deleteFileColumn,
        GetAliases: getAliasList
    };
}]);

csapp.controller("fileColumnMultiAddModalController", ["$scope", "$modalInstance", "fileColumnFactory", "fileColumnDataLayer",
    function ($scope, $modalInstance, factory, datalayer) {
        "use strict";

        (function () {
            $scope.datalayer = datalayer;
            $scope.factory = factory;
        })();

        $scope.add = function (columns) {
            factory.convertData(columns, datalayer.dldata.fileDetail);
            $modalInstance.close();
        };

        $scope.close = function () {
            $modalInstance.dismiss('close');
        };
    }
]);

csapp.controller("fileColumnAddEditController", ["$scope", "fileColumnDataLayer", "fileColumnFactory", "$modalInstance",
    function ($scope, datalayer, factory, $modalInstance) {
        $scope.modelHeader = "Add New File Column";

        (function () {
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            $scope.dldata = datalayer.dldata;
           
        })();

        $scope.close = function () {
            $modalInstance.dismiss();
        };

        $scope.add = function (filecolumn) {
            datalayer.Save(filecolumn, datalayer.dldata.fileDetail);
            $modalInstance.close();
        };
    }
]);

csapp.controller("fileColumnController", ['$scope', "$csnotify", "$csfactory", "modalService", "$modal", "fileColumnDataLayer", "fileColumnFactory",
    function ($scope, $csnotify, $csfactory, modalService, $modal, datalayer, factory) {
        'use strict';

        (function () {
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            $scope.datalayer.GetAliases();
            $scope.dldata = datalayer.dldata;
        })();

        $scope.showDeleteModal = function (fileColumn, index) {
            var modalOptions = {
                closeButtonText: 'Cancel',
                actionButtonText: 'Delete Column',
                headerText: "Delete '" + fileColumn.FileColumnName + "' ?",
                bodyText: 'Are you sure you want to delete this column?'
            };

            modalService.showModal({}, modalOptions).then(function () {
                datalayer.Delete(fileColumn.Id, index);
            });
        };

        $scope.showAddMultiColumnModal = function () {
            $modal.open({
                templateUrl: '/FileUpload/filecolumn/file-column-multi.html',
                controller: 'fileColumnMultiAddModalController',
            });
        };

        $scope.showAddSingleColumnModal = function () {
            $modal.open({
                templateUrl: '/FileUpload/filecolumn/file-column-add.html',
                controller: 'fileColumnAddEditController',
            });
        };
    }
]);