
(
csapp.controller("fileColumnController",
    ['$scope', "$csnotify", '$location', 'Restangular',
        function ($scope, $csnotify, $location, restApi) {
            'use strict';

            $scope.dateFormats = ['dd-mm-yyyy', 'yyyy-mm-dd', 'mm-dd-yyyy'];

            //#region Other functions

            //#region ExcelManager Popup

            $scope.openExcelManager = function () {
                $scope.shouldbeOpenExcelManager = true;
            };

            $scope.closeExcelManager = function () {
                $scope.shouldbeOpenExcelManager = false;
            };

            //#endregion

            //#region grid popup

            $scope.openModel = function () {
                $scope.reset();
                $scope.modelHeader = "Add New File Column";
                $scope.shouldBeOpen = true;

            };

            $scope.closeModel = function () {
                $scope.shouldBeOpen = false;
            };

            $scope.openModelData = function (fileColumnRow, readOnly, index) {
                debugger;
                if (index || index == 0) {
                    $scope.modelHeader = "Update File Column";
                } else {
                    $scope.modelHeader = "View File Column";
                }

                $scope.isReadOnly = readOnly;
                $scope.fileColumn = angular.copy(fileColumnRow);
                $scope.shouldBeOpen = true;
                $scope.editIndex = index;
            };

            $scope.yesToDelete = function () {
                $scope.deleteFileColumn($scope.deletedColumn.Id, $scope.editIndex);
                $scope.showDeleteModel = false;
            };

            $scope.noToDelete = function () {
                $scope.deletedColumn = {};
                $scope.editIndex = -1;
                $scope.showDeleteModel = false;
            };

            $scope.showDeleteModelPopup = function (column, index) {
                $scope.deletedColumn = column;
                $scope.editIndex = index;
                $scope.showDeleteModel = true;
            };

            $scope.reset = function () {
                $scope.isReadOnly = false;
                $scope.editIndex = -1;
                $scope.fileColumn = {
                    FileColumnName: "",
                    Position: $scope.fileDetail.FileColumns.length + 1,
                    DateFormat: "",
                    Length: "",
                    ColumnDataType: "",
                    TempColumnName: "",
                    Description: "",
                    FileDetail: ""
                };
            };

            $scope.AddinLocal = function (data) {
                debugger;
                for (var i = 0; i < data.length; i++) {
                    if ($scope.editIndex === -1) {
                        $scope.fileDetail.FileColumns.push(data[i]);
                    }
                    else {
                        $scope.fileDetail.FileColumns = _.reject($scope.fileDetail.FileColumns, function (filecolumn) { return filecolumn.FileColumnName == data[i].FileColumnName; });
                        $scope.fileDetail.FileColumns.push(data[i]);
                        //$scope.fileDetail.FileColumns[$scope.editIndex] = data[i];
                    }
                }
            };

            $scope.modelOption = {
                backdropFade: true,
                dialogFade: true
            };

            //#endregion

            $scope.getColumnType = function (col) {
                var column = col.toUpperCase();
                if (column.indexOf("ACCOUNT") > -1) {
                    return "Number";
                }
                else if (column.indexOf("NO") > -1) {
                    return "Number";
                }
                else if (column.indexOf("AMOUNT") > -1) {
                    return "Amount";
                }
                else if (column.indexOf("DATE") > -1) {
                    return "Date";
                } else {
                    return "String";
                }
            };

            //for getting TempColumnName
            $scope.getTempColumnName = function (str) {
                var tempColName = str.replace(/\s+/g, '').replace("(", '').replace(")", '').replace(".", '').replace("@", '').replace(/["'/*]/g, '').replace("-", '_');

                var num1 = tempColName.match(/\d+\_?\d*/g);

                if (num1 !== null) {
                    tempColName = tempColName.replace(/\d+\_?\d*/g, '') + num1;///\d+\_?\d*/g
                }

                return tempColName;
            };

            //for convert data to columns
            $scope.convertData = function () {
                var listOfColumns = $scope.excelData.split('\t');

                var maxPosition = $scope.fileDetail.FileColumns.length > 0 ? _.max(_.pluck($scope.fileDetail.FileColumns, "Position")) : 0;
                for (var i = 0; i < listOfColumns.length; i++) {

                    var tempcolumn = $scope.getTempColumnName(listOfColumns[i]);
                    var type = $scope.getColumnType(tempcolumn);
                    var tempdate = new Date();
                    var position = maxPosition + (i + 1);

                    var existTempColumn = _.find($scope.fileDetail.FileColumns, { TempColumnName: tempcolumn });

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


                    //$scope.fileColumns.push(fileColumn);

                    $scope.fileDetail.FileColumns.push(fileColumn);
                }

                $scope.closeExcelManager();
                $scope.excelData = "";
            };

            //for starting enddate date picker
            $scope.getEndDate = function (startdate) {
                var start = new Date(startdate.getTime() + (24 * 60 * 60 * 1000));
                return start;
            };

            // for checking start date and end date
            $scope.checkEndDate = function (startDate, endDate) {
                if (startDate == endDate) {
                    return true;
                } else {
                    return false;
                }
            };

            //for checking column is date type or not
            $scope.isDateTypeColumn = function (type) {
                if (type == 'Date') {
                    return true;
                } else {
                    return false;
                }
            };

            //#endregion 

            //#region DB Operations

            var apictrl = restApi.all('FileColumnApi');
            //for selected file columns
            $scope.getFileColumns = function (aliasName) {
                debugger;
                if (aliasName) {

                    apictrl.customGET("GetFileColumns", { aliasName: aliasName }).then(function (data) {

                        if (data) {
                            debugger;
                            $scope.fileDetail = data;
                            $scope.fileDetail.FileColumns = _.sortBy($scope.fileDetail.FileColumns, 'Position');
                            $csnotify.success("All File Columns Loaded Successfully");
                        }

                    }, function (response) {
                        $csnotify.error(response.status);
                    });
                }
            };

            apictrl.customGET('Fetch').then(function (data) {

                if (data) {
                    $scope.fileDetails = data.FileNames;
                    $scope.dataTypes = data.ColumnDataTypes;
                    $csnotify.success("All Files Loaded Successfully");
                }
            }, function (response) {
                $csnotify.error(response.Message);
            });

            //for save file column
            $scope.saveFileColumn = function (fileColumn, fileDetail) {
                debugger;

                fileColumn.FileDetail = fileDetail;
                if (fileColumn && fileDetail) {
                    if (fileColumn.Id) {
                        debugger;
                        var saveFileColumnList = [];
                        var list = _.sortBy($scope.fileDetail.FileColumns, 'Position');

                        var oldFileDetail = _.find(list, { Position: fileColumn.Position });
                        var newFileDetail = _.find(list, { Id: fileColumn.Id });

                        if (oldFileDetail && oldFileDetail.Id != newFileDetail.Id)
                            oldFileDetail.Position = newFileDetail.Position;

                        saveFileColumnList.push(fileColumn);
                        //saveFileColumnList.push(oldFileDetail);


                        apictrl.customPUT(saveFileColumnList, 'PutFileColumn', { id: fileColumn.Id }).then(function (data) {
                            debugger;
                            $scope.AddinLocal(data);
                            $scope.reset();
                            $scope.closeModel();
                            $csnotify.success("File Column Updated Successfully");
                        }, function (response) {
                            $csnotify.error("Error:-" + response.Message);
                        });

                    } else {
                        apictrl.customPOST(fileColumn, 'Post').then(function (data) {
                            $scope.AddinLocal(data);
                            $scope.reset();
                            $scope.closeModel();
                            $csnotify.success("File Column Added Successfully");
                        }, function (response) {
                            $csnotify.error("Error:-" + response.Message);
                        });
                    }
                }
            };

            //for save all file columns
            $scope.SaveAllColumns = function (fileDetail) {
                debugger;
                if (fileDetail) {
                    apictrl.customPOST(fileDetail, 'SaveAllColumns').then(function (data) {
                        $scope.fileDetail = data;
                        $csnotify.success('All File Columns Save Successfully');
                    }, function (response) {
                        $csnotify.error("Error:-" + response.Message);
                    });
                }
            };

            //for delete the columns
            $scope.deleteFileColumn = function (id, index) {
                debugger;
                var pos = $scope.fileDetail.FileColumns[index].Position;
                if (id) {
                    apictrl.customDELETE('Delete', { id: id }).then(function () {
                        debugger;

                        $scope.fileDetail.FileColumns.splice(index, 1);

                        $csnotify.success('File Column Deleted Successfully');

                        var list = _.sortBy($scope.fileDetail.FileColumns, 'Position');
                        for (var j = index; j <= list.length ; j++) {
                            list[j].Position = j + 1;
                        }

                    }, function (response) {
                        $csnotify.error('Error:-' + response.Message);
                    });
                }

                else {
                    $scope.fileDetail.FileColumns.splice(index, 1);
                }
                for (var i = index + 1; i < $scope.fileDetail.FileColumns.length; i++) {
                    $scope.fileDetail.FileColumns[i].Position = pos;
                    pos++;
                }
            };

            $scope.MoveUpward = function (column, index) {
                debugger;
                var list = _.sortBy($scope.fileDetail.FileColumns, 'Position');
                if (index != 0) {
                    list[index].Position = index;
                    list[index - 1].Position = index + 1;
                }
            };

            $scope.MoveDownward = function (column, index) {
                debugger;
                var list = _.sortBy($scope.fileDetail.FileColumns, 'Position');
                if (index != $scope.fileDetail.FileColumns.length - 1) {
                    list[index].Position = index + 2;
                    list[index + 1].Position = index + 1;
                }
            };
            //#endregion

        }])
);