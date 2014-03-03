
//#region controller

(
csapp.controller("fileDetailsController", ['$scope', '$csfactory', '$csnotify', 'Restangular',
    '$Validations', "fileUploadModels", function ($scope, $csfactory, $csnotify, rest, $val, fileUploadModels) {

        "use strict";

        var apictrl = rest.all('FileDetailsApi');
        
        $scope.val = $val;

        $scope.dateFormats = [{ format: 'dd-mm-yyyy', group: "date" }, { format: 'yyyy-mm-dd', group: "date" }, { format: 'dddd-MMMM-yyyy', group: "date" }];
        // $scope.dateFormats = ['dd-mm-yyyy', 'yyyy-mm-dd', 'dddd-MMMM-yyyy'];
        $scope.UsedFor = ['Allocation', 'Billing'];
        $scope.FileDetailsModel = fileUploadModels.FileDetailsModel;
        $scope.Button =  {
            save: {btnType:"save"},
            cancel: {btnType:"cancel"}
        };


        //#region other Operations
        $scope.reset = function () {
            $scope.modelTitle = "Add New File Details";
            $scope.isReadOnly = false;
            $scope.editIndex = -1;
            $scope.fileDetail = {};
        };

        $scope.AddinLocal = function (temp) {
            if ($scope.editIndex === -1) {
                $scope.fileDetailsList.push(temp);
            } else {
                $scope.fileDetailsList[$scope.editIndex] = temp;
            }
        };

        $scope.openModel = function () {
            $scope.reset();
            $scope.DependsOnAlias = [];
            $scope.shouldBeOpen = true;
        };

        $scope.closeModel = function () {
            $scope.shouldBeOpen = false;
        };

        $scope.modelOption = {
            backdropFade: true,
            dialogFade: true
        };

        $scope.openModelData = function (fileDetailRow, readOnly, index) {
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

        $scope.yesToDelete = function () {
            $scope.deleteFileDetails($scope.deletedFile.Id, $scope.editIndex);
            $scope.showDeleteModel = false;
        };

        $scope.noToDelete = function () {
            $scope.deletedFile = {};
            $scope.editIndex = -1;
            $scope.showDeleteModel = false;
        };

        $scope.showDeleteModelPopup = function (file, index) {
            $scope.deletedFile = file;
            $scope.editIndex = index;
            $scope.showDeleteModel = true;
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


        $scope.StepNames = [
            'addfiledetail',
            'addfilecolumn',
            'addfilemapping'
        ];

        $scope.HasNextStep = function () {
            debugger;
            var index = $scope.StepNames.indexOf($scope.currentStep);
            $scope.currentStep = $scope.StepNames[index + 1];
        };


        $scope.HasPrevStep = function () {
            debugger;
            var index = $scope.StepNames.indexOf($scope.currentStep);
            $scope.currentStep = $scope.StepNames[index - 1];
        };


        $scope.init = function () {
            //$scope.NextFileColumn = false;
            //$scope.NextFileMapping = false;
            //$scope.PreviousFileColumn = false;
            $scope.currentStep = 'addfiledetail';
        };

        $scope.init();

        //$scope.NextFileDetails = function () {
        //    debugger;
        //    if (angular.isDefined($scope.fileDetail)) {
        //        $scope.NextFileColumn = true;
        //        $scope.NextFileMapping = false;
        //        $scope.NextFileDetails = false;
        //    }
        //    else {
        //        $scope.NextFileDetails = false;
        //    }
        //};

        //$scope.NextFileColumn = function () {
        //    debugger;
        //    if (angular.isDefined($scope.fileColumns)) {
        //        $scope.NextFileDetails = false;
        //        $scope.NextFileMapping = true;
        //    }
        //    else {
        //        $scope.NextFileColumn = false;
        //    }
        //};

        //#endregion

        //#region Db operations

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
                    $scope.AddinLocal(data);
                    $scope.reset();
                    $scope.closeModel();
                }, function (response) {
                    $csnotify.error(response);
                });
            } else {
                apictrl.customPOST(fileDetail, 'Post').then(function (data) {
                    $csnotify.success("File Detail added Successfully");
                    $scope.AddinLocal(data);
                    $scope.reset();
                    $scope.closeModel();
                }, function (response) {
                    $csnotify.error(response);
                });
            }
        };

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

    }])
);

//#endregion
csapp.factory("fileUploadModels", ["fileUploadEnums", function (enums) {
    var fileDetailsModel = {
        AliasName: { type: "enum", label: "Alias Name", required: true, values: enums.FileAliasNameEnum },
        AliasDescription: { type: "textarea", label: "Alias Description", required: true, placeholder: "Enter Alias Description" },
        FileName: { type: "text", label: "File Name", placeholder: "Enter File Name", required: true},
        FileCount: { type: "number", label: "File Count", placeholder:"Enter no of files", required: true, min: 1,pattern:"/^[0-9]+$/"},
        DependsOnAlias: { type: "enum", label: "Depends On Alias", values: enums.FileAliasNameEnum },
        FileReaderType: { type: "enum", label: "FileReader Type", required: true, values: enums.FileUploadBy },
        DateFormat: { type: "select", label: "Date Format", required: true},
        FileType: { type: "enum", label: "File Type", required: true, values: enums.FileType },
        SheetName: { type: "text", label: "Sheet Name", required: true, placeholder:"Enter Sheet Name" },
        Frequency: { type: "enum", label: "Frequency", required: true, values: enums.Frequency },
        SkipLine: { type: "number", label: "Skip Line", placeholder: "Enter skip Line", required: true, min: 0 ,pattern:"/^[0-9]+$/"},
        FileServer: { type: "text", label: "File Server", required: true },
        FileDirectory: { type: "text", label: "File Directory", required: true, maxlength: 50, placeholder:"Enter File Directory" },
        ActualTable: { type: "enum", label: "Actual Table", required: true, values: enums.ClientDataTables },
        TempTable: { type: "text", label: "Temporary Table", required: true, maxlength: 50 },
        ErrorTable: { type: "text", label: "Error Table", required: true, maxlength: 50 },
        EmailId: { type: "email", label: "EmailId", required: true, placeholder: "Enter Email ID" },
        Description: { type: "text", label: "Description", required: true, placeholder: "Enter Description" },
        UsedFor: { type: "enum", label: "Used For", required: true, values: enums.UsedFor },
        StartDate: { type: "date", label: "Start Date", required: true},
        EndDate: { type: "date", label: "End Date", required: true},
        ScbSystems: { type: "enum", label: "Scb Systems", required: true, values: enums.ScbSystems },
        Category: { type: "enum", label: "Category", required: true, values: enums.ScbSystems },
    };

    
    return {
        FileDetailsModel: fileDetailsModel,
       
    };
}]);

//    public virtual ColloSysEnums.FileAliasName? DependsOnAlias { get; set; }
//public virtual DateTime? EndDate { get; set; }

csapp.factory("fileUploadEnums", function () {
    var fileAliasName = {
        CACS_ACTIVITY: "CACS_ACTIVITY",
        C_LINER_COLLAGE: "C_LINER_COLLAGE",
        C_LINER_UNBILLED: "C_LINER_UNBILLED",
        C_PAYMENT_LIT: "C_PAYMENT_LIT",
        C_PAYMENT_UIT: "C_PAYMENT_UIT",
        C_PAYMENT_VISA: "C_PAYMENT_VISA",
        C_WRITEOFF: "C_WRITEOFF",
        E_LINER_AUTO: "E_LINER_AUTO",
        E_LINER_OD_SME: "E_LINER_OD_SME",
        E_PAYMENT_LINER: "E_PAYMENT_LINER",
        E_PAYMENT_WO_AUTO: "E_PAYMENT_WO_AUTO",
        E_PAYMENT_WO_SMC: "E_PAYMENT_WO_SMC",
        E_WRITEOFF_AUTO: "E_WRITEOFF_AUTO",
        E_WRITEOFF_SMC: "E_WRITEOFF_SMC",
        R_WRITEOFF_SME: "R_WRITEOFF_SME",
        R_LINER_BFS_LOAN: "R_LINER_BFS_LOAN",
        R_LINER_MORT_LOAN: "R_LINER_MORT_LOAN",
        R_LINER_PL: "R_LINER_PL",
        R_MANUAL_REVERSAL: "R_MANUAL_REVERSAL",
        R_PAYMENT_LINER: "R_PAYMENT_LINER",
        R_PAYMENT_WO_AEB: "R_PAYMENT_WO_AEB",
        R_PAYMENT_WO_PLPC: "R_PAYMENT_WO_PLPC",
        R_WRITEOFF_PL_AEB: "R_WRITEOFF_PL_AEB",
        R_WRITEOFF_PL_SCB: "R_WRITEOFF_PL_SCB",
        R_WRITEOFF_PL_GB: "R_WRITEOFF_PL_GB",
        R_WRITEOFF_PL_LORDS: "R_WRITEOFF_PL_LORDS",
        R_WRITEOFF_AUTO_AEB: "R_WRITEOFF_AUTO_AEB",
        R_WRITEOFF_AUTO_GB: "R_WRITEOFF_AUTO_GB",
        R_WRITEOFF_AUTO_SCB: "R_WRITEOFF_AUTO_SCB"
    };

    var fileUploadBy = {
        NotSpecified: "NotSpecified",
        TextReader: "TextReader",
        FileHelper: "FileHelper",
        OleDbProvider: "OleDbProvider",
        ExcelReader: "ExcelReader",
        CsvHelper: "CsvHelper",
        NPOIXlsReader: "NPOIXlsReader",
        EpPlusXlsxReader: "EpPlusXlsxReader"
    };

    var filetype = {
        csv: "csv",
        txt: "txt",
        xls: "xls",
        xlsx: "xlsx"
    };

    var frequency = {
        Daily: "Daily",
        Monthly: "Monthly",
        Weekly: "Weekly"
    };

    var clientdataTable = {
        RLiner: "RLiner",
        RPayment: "RPayment",
        RWriteoff: "RWriteoff",
        ELiner: "ELiner",
        EPayment: "EPayment",
        EWriteoff: "EWriteoff",
        CLiner: "CLiner",
        CUnbilled: "CUnbilled",
        CPayment: "CPayment",
        CWriteoff: "CWriteoff",
        CacsActivity: "CacsActivity",
        RInfo: "RInfo",
        CInfo: "CInfo",
        EInfo: "EInfo"
    };

    var usedfor = {
        None: "None",
        Allocation: "Allocation",
        Billing: "Billing"
    };

    var scbSystem = {
        CACS: "CACS",
        CCMS: "CCMS",
        EBBS: "EBBS",
        RLS: "RLS"
    };

    var category = {
        Activity: "Activity",
        Liner: "Liner",
        Payment: "Payment",
        WriteOff: "WriteOff"
    };
    return {
        FileAliasNameEnum: fileAliasName,
        FileUploadBy: fileUploadBy,
        FileType: filetype,
        Frequency: frequency,
        ClientDataTables: clientdataTable,
        UsedFor: usedfor,
        ScbSystems: scbSystem,
        Category: category
    };
});
