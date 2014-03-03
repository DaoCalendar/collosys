
//#region controller

(
csapp.controller("AddfileDetailsController", ['$scope', '$csfactory', '$csnotify', 'Restangular',
    '$Validations', function ($scope, $csfactory, $csnotify, rest, $val) {

        "use strict";

        var apictrl = rest.all('AddFilesDetails');

        $scope.val = $val;
        $scope.dateFormats = ['dd-mm-yyyy', 'yyyy-mm-dd', 'MM-yyyy-dd', 'MM-dd-yyyy'];
        $scope.UsedFor = ['Allocation', 'Billing'];
        $scope.fileDetail = {};
        $scope.fileDetail.FileColumns = [];
        $scope.fileDetails = [];
        $scope.fileMappings = [];
        $scope.valueTypes = [];
        $scope.actualTable = '';
        $scope.fileMapping = {};
        $scope.TempColumnName = [];
        $scope.isModalDateValid = true;
        $scope.isEnddateValid = true;
        // $scope.DateInvalid = false;

        //file details
        $scope.modelDateValidation = function (startDate, endDate) {
            debugger;
            if (angular.isUndefined(endDate) || endDate == null || endDate == '') {
                $scope.isModalDateValid = true;
                return;
            } else {
                if (startDate >= endDate) {
                    $scope.isModalDateValid = false;
                    $csnotify.error("Invalid EndDate");
                } else {
                    $scope.isModalDateValid = true;
                }
            }


        };

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

        $scope.init = function () {
            $scope.currentStep = 'addfiledetail';
            $scope.ShowPrevBtn = true;
        };

        $scope.init();

        $scope.StepNames = [
           'addfiledetail',
           'addfilecolumn',
           'addfilemapping'
        ];

        $scope.HasNextStep = function (currentStep) {
            debugger;

            var index = $scope.StepNames.indexOf($scope.currentStep);
            $scope.currentStep = $scope.StepNames[index + 1];
            $scope.sActualTable = '';
            if (currentStep === 'addfiledetail') {
                $scope.fileDetail.TempTable = 'Temp_' + $scope.fileDetail.AliasName;
            }

        };


        $scope.HasPrevStep = function () {
            if ($scope.currentStep == 'addfilecolumn') {
                $scope.isModalDateValid = true;
            }
            debugger;
            var index = $scope.StepNames.indexOf($scope.currentStep);
            $scope.currentStep = $scope.StepNames[index - 1];

        };

        $scope.ResetStep = function (currentStep) {
            debugger;
            if (currentStep === 'addfiledetail') {
                $scope.fileDetail = {};
            } else if (currentStep === 'addfilecolumn') {
                $scope.fileDetail.FileColumns = [];
            } else {
                $scope.fileMappings = [];
            }

        };


        //for getting all file details
        apictrl.customGET('Fetch').then(function (data) {
            $csnotify.success("All File Details Loaded Successfully.");
            $scope.fileDetailsList = data.FileDetails;
            $scope.fileCategories = data.FileCategories;
            $scope.fileAliasNames = data.FileAliasNames;
            //$scope.DependsOnAlias = data.FileAliasNames;
            $scope.fileFrequencies = data.FileFrequencies;
            $scope.fileTypes = data.FileTypes;
            $scope.fileSystems = data.FileSystems;
        }, function (response) {
            $csnotify.error(response);
        });


        //for save file details
        //$scope.saveFileDetails = function (fileDetail) {
        //    debugger;
        //    fileDetail.FileServer = "localhost";
        //    $scope.fileDetail.TempTable = "TEMP_" + $scope.fileDetail.AliasName;
        //    $scope.fileDetail.ErrorTable = "ERROR_" + $scope.fileDetail.AliasName;
        //    if (fileDetail.Id) {
        //        apictrl.customPUT(fileDetail, 'Put', { id: fileDetail.Id }).then(function (data) {
        //            $csnotify.success("File Detail Updated Successfully");
        //            $scope.AddinLocal(data);
        //            $scope.reset();
        //            $scope.closeModel();
        //        }, function (response) {
        //            $csnotify.error(response);
        //        });
        //    } else {
        //        apictrl.customPOST(fileDetail, 'Post').then(function (data) {
        //            $csnotify.success("File Detail added Successfully");
        //            $scope.AddinLocal(data);
        //            $scope.reset();
        //            $scope.closeModel();
        //        }, function (response) {
        //            $csnotify.error(response);
        //        });
        //    }
        //};
        //#endregion for file Details

        //file columns
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
            var tempColName = str.replace(/\s+/g, '').replace("(", '').replace(")", '').replace(".", '').replace("@", '').replace(/["'/*]/g, '').replace("-", '_').replace(",", '');

            var num1 = tempColName.match(/\d+\_?\d*/g);

            if (num1 !== null) {
                tempColName = tempColName.replace(/\d+\_?\d*/g, '') + num1;///\d+\_?\d*/g
            }

            return tempColName;
        };

        //for convert data to columns
        $scope.convertData = function () {
            debugger;
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
                    Length: 255,
                    ColumnDataType: type,
                    TempColumnName: tempcolumn,
                    DateFormat: "",
                    Description: "",
                    FileDetail: "",
                    StartDate: moment().utc().format('YYYY-MM-DD')
                };

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
        //$scope.checkEndDate = function (startDate, endDate) {
        //    debugger;
        //    if (startDate >= endDate) {
        //        return true;
        //    } else {
        //        return false;
        //    }
        //};


        //for checking column is date type or not
        $scope.isDateTypeColumn = function (type) {
            if (type == 'Date') {
                return true;
            } else {
                return false;
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

        $scope.dateValidation = function (startDate, endDate) {
            debugger;
            if (angular.isUndefined(endDate) || endDate == null || endDate == '') {
                $scope.isModalDateValid = true;
                return;
            } else {
                if (startDate >= endDate) {
                    $scope.isModalDateValid = false;
                    $csnotify.error("Invalid EndDate ");
                } else {
                    $scope.isModalDateValid = true;
                }

            }

        };


        //for selected file columns

        $scope.openExcelManager = function () {
            $scope.shouldbeOpenExcelManager = true;
        };

        $scope.closeExcelManager = function () {
            $scope.shouldbeOpenExcelManager = false;
        };

        $scope.outputCol = function (fileMapping) {
            debugger;
            var temp = _.find($scope.fileDetail.FileColumns, { 'TempColumnName': fileMapping.TempColumn });
            fileMapping.OutputColumnName = temp.FileColumnName;
            fileMapping.Position = temp.Position;
        };


        // file Mappings
        apictrl.customGET('Fetchit').then(function (data) {

            if (data) {
                $scope.fileDetails = data.FileNames;
                $scope.dataTypes = data.ColumnDataTypes;
                $csnotify.success("All Files Loaded Successfully");
            }
        }, function (response) {
            $csnotify.error(response.Message);
        });

        apictrl.customGET("GetValueTypes").then(function (data) {
            debugger;
            $scope.valueTypes = data;
        }, function (data) {
            $csnotify.error(data);
        });


        apictrl.customGET("GetFileTypes").then(function (data) {
            debugger;
            $scope.fileclassTypes = data;
        }, function (data) {
            $csnotify.error(data);
        });

        $scope.getFileMappings = function (actualTable, temptable) {

            apictrl.customGET("GetFileMappings", { 'actualTableName': actualTable, 'tempTableName': temptable }).then(function (data) {
                debugger;
                $scope.fileMappings = data;

                if ($scope.fileMappings.length < 1)
                    return;

                $scope.tempTable = $scope.fileMappings[0].TempTable;

                for (var i = 0; i < $scope.fileMappings.length; i++) {
                    var fileMapping = $scope.fileMappings[i];
                    var colName = fileMapping.ActualColumn.substring(0, 3);

                    var fileColumn = _.find($scope.fileDetail.FileColumns, function (column) {
                        return (column.TempColumnName.substring(0, 3).toUpperCase() === colName.toUpperCase());
                    });

                    if (angular.isDefined(fileMapping) == angular.isDefined(fileColumn)) {
                        fileMapping.TempColumn = fileColumn.TempColumnName;
                        fileMapping.Position = fileColumn.Position;
                        fileMapping.OutputPosition = 0;
                        fileMapping.OutputColumnName = fileColumn.FileColumnName;
                        fileMapping.ValueType = "ExcelValue";
                    } else {
                        fileMapping.TempColumn = '';
                        fileMapping.Position = 0;
                        fileMapping.OutputPosition = 0;
                        fileMapping.OutputColumnName = '';
                        fileMapping.ValueType = "DefaultValue";
                    }
                }
            }, function (data) {
                $csnotify.error(data);
            });
        };


        $scope.MoveUpwardMapping = function (column, index) {
            debugger;
            var list = _.sortBy($scope.fileMappings, 'OutputPosition');
            if (index != 0) {
                list[index].OutputPosition = index;
                list[index - 1].OutputPosition = index + 1;
            }
        };

        $scope.MoveDownwardMapping = function (column, index) {
            debugger;
            var list = _.sortBy($scope.fileMappings, 'OutputPosition');
            if (index != $scope.fileMappings.length - 1) {
                var store = list[index].OutputPosition;
                list[index].OutputPosition = list[index + 1].OutputPosition;
                list[index + 1].OutputPosition = store;
            }
        };

    }])
);



// unused for file Mapping
//$scope.$watch('fileMapping.ValueType', function () {
//    debugger;
//    if ($scope.fileMapping.ValueType == 'ExcelValue' || $scope.fileMapping.ValueType == 'MappedValue') {
//        $scope.fileMapping.DefaultValue = '';
//    } else {
//        $scope.fileMapping.TempColumn = '';
//        $scope.fileMapping.Position = 0;
//    }
//});

//$scope.changeFileColumn = function (tempColumnName) {
//    debugger;
//    $scope.fileMapping.Position = _.find($scope.fileColumns, { 'TempColumnName': tempColumnName }).Position;
//    $scope.fileMapping.OutputPosition = _.find($scope.fileColumns, { 'TempColumnName': tempColumnName }).Position;
//    $scope.fileMapping.OutputColumnName = _.find($scope.fileColumns, { 'TempColumnName': tempColumnName }).FileColumnName;
//};


//$scope.addinLocal = function (fileMapping) {
//    $scope.fileMappings = _.reject($scope.fileMappings, function (mapping) { return mapping.Id == fileMapping.Id; });
//    $scope.fileMappings.push(fileMapping);
//};