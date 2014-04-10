
csapp.factory("$csShared", ["$csnotify", function ($csnotify) {
    var enums = {};
    var getEnum = function (name) {
        _.forEach(enums, function (obj) {
            if (obj.Name === name) {
                return obj.Value;
            }
        });

    };
    return {
        enums: enums,
        getEnum: getEnum

    };
}]);

csapp.factory("$csFileUploadModels", ["$csShared", function ($csShared) {
    var models = {};

    var fileDetail = function () {
        return {
            AliasName: { label: "Alias Name", type: "enum", required: true, valueList: $csShared.enums.FileAliasName, placeholder: "Select File Alias Name" },
            AliasDescription: { label: "Alias Description", type: "text", placeholder: "Enter Alias Description", required: true },
            FileName: { label: "File Name", type: "text", placeholder: "Enter File Name",required:true },
            FileCount: { label: "File Count", type: "int", pattern: "/^[0-9]+$/", min: 0, max: 100, required: true, placeholder: "Enter no of files" },
            DependsOnAlias: { label: "SCB Systems", type: "enum", valueList: $csShared.enums.FileAliasName, required: true, placeholder: "Select System" },
            FileReaderType: { type: "enum", valueList: $csShared.enums.FileUploadBy },
            DateFormat: { label: "Date Format", type: "select", valueList: $csShared.enums.DateFormats, placeholder: "Select Date Format" },
            FileType: { label: "File Type", type: "enum", valueList: $csShared.enums.FileType, required: true, placeholder: "Select File Type" },
            SheetName: { label: "Sheet Name", type: "text", placeholder: "Enter Sheet Name" },
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
            DateFormat: { label: "Date Format", type: "enum", valueList: $csShared.enums.dateFormats, placeholder: "Select Date Format" },
            StartDate: { label: "Start Date", type: "date", required: true },
            EndDate: { label: "End Date", type: "date" }//data-date-start-date have function.
        };
    };

    var fileMapping = function () {
        return {
            ActualTable: { label: "Actual Table", type: "text" },
            FileDetail: { label: "File Name" },// tobe disscuss Enum query 
            ActualColumn: { label: "Actual Column", type: "text" },
            Position: { label: "Position", type: "text", required: true },
            OutputPosition: { label: "Output Position", type: "text", required: true },
            OutputColumnName: { label: "Output ColumnName", type: "text", required: true },
            ValueType: { label: "Value Type", type: "enum", value: $csShared.enums.FileMappingValueType, required: true },
            TempTable: { label: "Temp Table", type: "text" },
            TempColumn: { label: "Temp Column" },//similar to FileDeatils
            DefaultValue: { label: "Default Value", type: "text", required: true },
            StartDate: { label: "Start Date", type: "date", required: true },
            EndDate: { label: "End Date", type: "date", required: true },
        };
    };

    var init = function () {
        models.FileDetail = fileDetail();
        models.FileColumn = fileColumn();
        models.FileMapping = fileMapping();
        return models;
    };

    return {
        init: init,
        models: models
    };
}]);

csapp.factory("$csStakeholderModels", ["$csShared", function () {

    var stakeholder = function () {
        return {
            Name: { label: "Name", type: 'text', pattern: '/^[a-zA-Z ]{1,100}$/', patternMessage: 'Invalid Name' },
            userId: { label: "UserId", editable: false, template: 'user', required: "true", type: "text", pattern: '/^[0-9]{7}$/', patternMessage: 'Invalid ID' },
            mobile: { label: "Mobile No", type: 'text', pattern: '/^[0-9]{10}$/', template: 'phone', patternMessage: 'Invalid Mobile Number' },
            Email: { label: "Email", patternMessage: 'Invalid Email' },
            Date: {},
            manager: {},
            Name: { label: "Name", type: 'text', pattern: '/^[a-zA-Z ]{1,100}$/', required: true, patternMessage: 'Invalid Name' },
            userId: { label: "UserId", editable: false, template: 'user', required: "true", type: "text", pattern: '/^[0-9]{7}$/', patternMessage: 'Invalid ID' },
            mobile: { label: "Mobile No", type: 'text', pattern: '/^[0-9]{10}$/', template: 'phone', patternMessage: 'Invalid Mobile Number' },
            Email: { label: "Email", patternMessage: 'Invalid Email' },
            Date: { type: 'date' },
            manager: {},
            //PAN : { label: 'PAN',patternMessage:'accepts only xxxxxxxx' },
            //TAN : { label: 'TAN',patternMessage:'accepts only xxxxxxxx' },
            //Registration : { label: 'Registration', patternMessage: 'special characters not allowed' },
            //ServiceTaxNo : { label: 'ServiceTaxNo', patternMessage: 'special characters not allowed' },

            //line1 : { label: "Line1", required: true },
            //line2 : { label: "Line2", required: true },
            //line3 : { label: "Line3" },
            //landline : { label: "Landline",patternMessage:"Invalid Number" }
        };
    };

    var init = function () {
        return {
            Stakeholder: stakeholder()
        };
    };

    return {
        init: init
    };

}]);

csapp.factory("$csModels", ["$csFileUploadModels", "$csStakeholderModels",
    function ($csFileUploadModels, $csStakeholderModels) {

        var models = {};

        var init = function () {
            models.FileUpload = $csFileUploadModels.init();
            models.Stakeholder = $csStakeholderModels.init();
            return;
        };

        return {
            init: init,
            models: models
        };
    }
]);