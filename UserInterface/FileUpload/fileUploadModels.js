csapp.factory("$csFileUploadModels", ["$csShared", function ($csShared) {
    var models = {};

    var fileDetail = function () {
        return {
            AliasName: { label: "Alias Name", type: "enum", required: true, valueList: $csShared.enums.FileAliasName, placeholder: "Select File Alias Name" },
            AliasDescription: { label: "Alias Description", type: "text", placeholder: "Enter Alias Description", required: true },
            FileName: { label: "File Name", type: "text", placeholder: "Enter File Name", required: true },
            FileCount: { label: "File Count", type: "uint", min: 1, max: 100, required: true, placeholder: "Enter no of files" },
            DependsOnAlias: { label: "DependsOnAlias", type: "enum", valueList: $csShared.enums.FileAliasName, required: true, placeholder: "Select System" },
            FileReaderType: { type: "enum", valueList: $csShared.enums.FileUploadBy },
            DateFormat: { label: "Date Format", type: "enum", valueList: $csShared.enums.DateFormat, placeholder: "Select Date Format", required: true },
            FileType: { label: "File Type", type: "enum", valueList: $csShared.enums.FileType, required: true, placeholder: "Select File Type" },
            SheetName: { label: "Sheet Name", type: "text", placeholder: "Enter Sheet Name", required: true },
            Frequency: { label: "Frequency", type: "enum", valueList: $csShared.enums.FileFrequency, required: true, placeholder: "Select File Frequency" },
            SkipLine: { label: "Skip Line", type: "int", pattern: "/^[0-9]+$/", min: 0, required: true, placeholder: "Enter Skip Lines" },
            FileDirectory: { label: "FileDirectory", type: "text", placeholder: "Enter File Directory Name" },
            ActualTable: { type: "enum", valueList: $csShared.enums.ClientDataTables },
            EmailId: { label: "Email Id", type: "email", required: true, placeholder: "Enter Email Id" },
            Description: { label: "Description", type: "text", required: true, placeholder: "Enter Description" },
            UsedFor: { label: "UsedFor", type: "enum", valueList: $csShared.enums.UsedFor, required: true, placeholder: "Enter Used For" },
            StartDate: { label: "Start Date", type: "date", required: true },
            EndDate: { label: "End Date", type: "date", required: true },
            ScbSystems: { label: "SCB Systems", type: "enum", valueList: $csShared.enums.ScbSystems, required: true, placeholder: "Select System" },
            Category: { label: "Category", type: "enum", valueList: $csShared.enums.Category, required: true, placeholder: "Select Category" },
        };
    };

    var fileColumn = function () {

        return {
            Position: { label: "Position", type: "int", required: true, placeholder: "Enter the Column Position", min: 0 },
            FileColumnName: { label: "Excel Column Name", required: true, type: "text", editable: false, placeholder: "Enter File Column Name" },
            Description: { label: "Description", type: "text", placeholder: "Enter Description" },
            Length: { label: "Length", type: "int", min: 0, required: true, placeholder: "Enter the Column Length" },
            ColumnDataType: { label: "Column Data Type", type: "enum", valueList: $csShared.enums.FileDataType, required: true, placeholder: "Select Column Type" },
            TempColumnName: { label: "DB Column Name", type: "text", required: true, pattern: "/^\w*$/", patternMessage: "Invalid Column Name", placeholder: "Enter Temporary Column Name" },
            DateFormat: { label: "Date Format", type: "enum", valueList: $csShared.enums.DateFormat, placeholder: "Select Date Format" },
            StartDate: { label: "Start Date", type: "date", required: true },
            EndDate: { label: "End Date", type: "date" }//data-date-start-date have function.
        };
    };
    var fileMapping = function () {
        return {
            ActualTable: { label: "Actual Table", type: "text", editable: false},
            FileDetail: { label: "File Name", type:"enum"},// tobe disscuss Enum query 
            ActualColumn: { label: "Actual Column", type: "text"},
            Position: { label: "Position", type: "text",required:true},
            OutputPosition: { label: "Output Position", type: "text",required:true},
            OutputColumnName: { label: "Output ColumnName", type: "text",required:true},
            ValueType: { label: "Value Type", type: "enum", valueList: $csShared.enums.FileMappingValueType, required: true },
            TempTable: { label: "Temp Table", type: "text" },
            TempColumn: { label: "Temp Column",type:"text",required:true },//similar to FileDetails
            DefaultValue: { label: "Default Value", type: "text",required:true},
            StartDate: { label: "Start Date", type: "date",required:true},
            EndDate: { label: "End Date", type: "date",required:true},
        };
    };

    var init = function () {
        models.FileDetail = fileDetail();
        models.FileColumn = fileColumn();
        models.FileMapping = fileMapping();
        models.CustomerInfo = customerInfo();
        return models;
    };

    var customerInfo = function () {

        return {
            Flag: { type: "enum", valueList: $csShared.enums.DelqFlag},
            AccountNo: { type: "enum", valueList: []},
            GlobalCustId: { type: "enum", valueList:[] },
            CustomerName: { type: "enum", valueList: [] },
            Pincode: { type: "uint"},
            Product: { type: "enum", valueList: $csShared.enums.Products},
            CustStatus: { type: "text" },
            AllocStartDate: { type: "date" },
            IsInRecovery: {type:"enum",valueList:['Yes','No']},//to be disscuss for checkbox
            IsReferred: { type: "enum", valueList: ['Yes', 'No'] },
            IsXHoldAccount: { type: "enum", valueList: ['Yes', 'No']},
            AllocEndDate: { type: "date"},
            ChargeofDate: { type: "date"},
            AllocStatus: { type: "enum", valueList: $csShared.enums.AllocStatus },
            TotalDue: { type: "decimal" },
            NoAllocResons: { type: "enum", valueList: $csShared.enums.NoAllocResons },
            Cycle: { type: "number"},
            Bucket: { type: "uint"}
        };
    };

    return {
        init: init,
        models: models
    };
}]);
