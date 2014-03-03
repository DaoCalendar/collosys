csapp.controller("fileuploadcontroller", ['$scope', '$csnotify',"fileUploadModels", function ($scope, $csnotify,fileUploadModels) {
   
    $scope.FileDetailsModel = fileUploadModels.FileDetailsModel;
    $scope.show=false,
    $scope.button = {
        save: { btnType: "save" }
    };
    $scope.saveFileDetails = function (file) {
     console.log(file);
    };

}]);

csapp.factory("fileUploadModels", ["fileUploadEnums", function (enums) {
  var fileDetailsModel = {
        AliasName: {type:"enum", label: "Alias Name", required: true, values: enums.FileAliasNameEnum },
        AliasDescription: { type: "textarea", label: "Alias Description", required: true,placeholder:"Enter Alias Description" },
        FileName: { type: "text", label: "File Name", placeholder:"Enter File Name", required: true, maxlength: 100 },
        FileCount: { type: "number", label: "File Count",placeholder:"Enter no.of File", required: true, min: 1, max: 20 },
        DependsOnAlias: { type: "enum", label: "Depends On Alias", values: enums.FileAliasNameEnum },//clear doubt
        FileReaderType: { type: "enum", label: "FileReader Type", required: true, values: enums.FileUploadBy },
        DateFormat: { type: "text", label: "Date Format", required: true, maxlength: 20 },
        FileType: { type: "enum", label: "File Type", required: true, values: enums.FileType },
        SheetName: { type: "text", label: "Sheet Name", required: true, maxlength: 20 },
        Frequency: { type: "enum", label: "Frequency", required: true, values: enums.Frequency },
        SkipLine: { type: "number", label: "Skip Line",placeholder:"Enter skip Line", required: true, min: 0 },
        FileServer: { type: "text", label: "File Server", required: true },
        FileDirectory: { type: "text", label: "File Directory", required: true, maxlength: 50 },
        ActualTable: { type: "enum", label: "Actual Table", required: true, values: enums.ClientDataTables },
        TempTable: { type: "text", label: "Temporary Table", required: true, maxlength: 50 },
        ErrorTable: { type: "text", label: "Error Table", required: true, maxlength: 50 },
        EmailId: { type: "email", label: "EmailId", required: true, placeholder: "Enter EmailID" },
        Description: { type: "text", label: "Description", required: true, placeholder: "Enter Description" },
        UsedFor: { type: "enum", label: "Used For", required: true, values: enums.UsedFor },
        StartDate: { type: "date", label: "Start Date", required: true, minViewMode: 0 },
        EndDate: { type: "date", label: "End Date", required: true, minViewMode: 0 },
        ScbSystems: { type: "enum", label: "Scb Systems", required: true, values: enums.ScbSystems },
        Category: { type: "enum", label: "Category", required: true, values: enums.ScbSystems },
    };


    return {
        FileDetailsModel: fileDetailsModel
    };
}]);

//    public virtual ColloSysEnums.FileAliasName? DependsOnAlias { get; set; }
//public virtual DateTime? EndDate { get; set; }

csapp.factory("fileUploadEnums", function () {
    var fileAliasName = {
        CACS_ACTIVITY: "CACS_ACTIVITY",
        C_LINER_COLLAGE: "C_LINER_COLLAGE",
        C_LINER_UNBILLED: "C_LINER_UNBILLED",
        C_PAYMENT_LIT:"C_PAYMENT_LIT",
        C_PAYMENT_UIT:"C_PAYMENT_UIT",
        C_PAYMENT_VISA:"C_PAYMENT_VISA",
        C_WRITEOFF:"C_WRITEOFF",
        E_LINER_AUTO:"E_LINER_AUTO",
        E_LINER_OD_SME:"E_LINER_OD_SME",
        E_PAYMENT_LINER:"E_PAYMENT_LINER",
        E_PAYMENT_WO_AUTO:"E_PAYMENT_WO_AUTO",
        E_PAYMENT_WO_SMC:"E_PAYMENT_WO_SMC",
        E_WRITEOFF_AUTO:"E_WRITEOFF_AUTO",
        E_WRITEOFF_SMC:"E_WRITEOFF_SMC",
        R_WRITEOFF_SME:"R_WRITEOFF_SME",
        R_LINER_BFS_LOAN:"R_LINER_BFS_LOAN",
        R_LINER_MORT_LOAN:"R_LINER_MORT_LOAN",
        R_LINER_PL:"R_LINER_PL",
        R_MANUAL_REVERSAL:"R_MANUAL_REVERSAL",
        R_PAYMENT_LINER:"R_PAYMENT_LINER",
        R_PAYMENT_WO_AEB:"R_PAYMENT_WO_AEB",
        R_PAYMENT_WO_PLPC:"R_PAYMENT_WO_PLPC",
        R_WRITEOFF_PL_AEB:"R_WRITEOFF_PL_AEB",
        R_WRITEOFF_PL_SCB:"R_WRITEOFF_PL_SCB",
        R_WRITEOFF_PL_GB:"R_WRITEOFF_PL_GB",
        R_WRITEOFF_PL_LORDS:"R_WRITEOFF_PL_LORDS",
        R_WRITEOFF_AUTO_AEB:"R_WRITEOFF_AUTO_AEB",
        R_WRITEOFF_AUTO_GB:"R_WRITEOFF_AUTO_GB",
        R_WRITEOFF_AUTO_SCB: "R_WRITEOFF_AUTO_SCB"
    };

    var fileUploadBy = {
        NotSpecified:"NotSpecified",
        TextReader:"TextReader",
        FileHelper:"FileHelper",
        OleDbProvider:"OleDbProvider",
        ExcelReader:"ExcelReader",
        CsvHelper:"CsvHelper",
        NPOIXlsReader:"NPOIXlsReader",
        EpPlusXlsxReader: "EpPlusXlsxReader"
    };

    var filetype = {
        csv:"csv",
        txt:"txt",
        xls:"xls",
        xlsx:"xlsx"
    };

    var frequency = {
        Daily:"Daily",
        Monthly:"Monthly",
        Weekly: "Weekly"
    };

    var clientdataTable = {
        RLiner:"RLiner",
        RPayment:"RPayment",
        RWriteoff:"RWriteoff",
        ELiner:"ELiner",
        EPayment:"EPayment",
        EWriteoff:"EWriteoff",
        CLiner:"CLiner",
        CUnbilled:"CUnbilled",
        CPayment:"CPayment",
        CWriteoff:"CWriteoff",
        CacsActivity:"CacsActivity",
        RInfo:"RInfo",
        CInfo:"CInfo",
        EInfo: "EInfo"
    };

    var usedfor = {
        None:"None",
        Allocation:"Allocation",
        Billing:"Billing"
    };

    var scbSystem = {
        CACS:"CACS",
        CCMS:"CCMS",
        EBBS:"EBBS",
        RLS: "RLS"
    };

    var category = {
        Activity:"Activity",
        Liner:"Liner",
        Payment:"Payment",
        WriteOff: "WriteOff"
    };
    return {        
        FileAliasNameEnum: fileAliasName,
        FileUploadBy:fileUploadBy,
        FileType: filetype,
        Frequency: frequency,
        ClientDataTables:clientdataTable,
        UsedFor:usedfor,
        ScbSystems: scbSystem,
        Category: category
    };
});



  

