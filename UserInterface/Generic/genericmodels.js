csapp.factory("$csGenericModels", ["$csShared", function ($csShared) {

    var taxList = function () {
        return {
            TaxName: { label: 'Name', type: 'text', required: true },
            TaxType: { label: 'Type', type: 'enum', valueList: $csShared.enums.TaxType, required: true },
            ApplicableTo: { label: 'Applicable To', type: 'enum', valueList: $csShared.enums.TaxApplicableTo, required: true },
            IndustryZone: { label: 'Industry Zone', type: 'text' },
            ApplyOn: { label: 'Apply On', type: 'enum', valueList: $csShared.enums.TaxApplyOn, required: true },
            TotSource: { label: 'TOT Source', type: 'text' },
            Description: { label: 'Description', type: 'textarea', required: true }
        };
    };

    var taxMaster = function () {
        return {
            GTaxesList: { label: 'Tax List', type: 'select', textField: 'TaxName' },
            ApplicableTo: { label: 'Role', type: 'enum', valueList: $csShared.enums.TaxApplicableTo, required: true },
            IndustryZone: { label: 'Industry Zone', type: 'text' },
            Country: { label: 'Country', type: 'text' },
            State: { label: 'State', type: 'enum', valueList: [] },
            District: { label: 'District', type: 'text' },
            Priority: { label: 'Priority', type: 'number', template: 'int' },
            Percentage: { label: 'Percentage', type: 'number', template: 'percentage' },
            StartDate: { label: 'Start Date', type: 'date', required: true },
            EndDate: { label: 'End Date', type: 'date' }
        };
    };

    var pincode = function () {
        return {
            Country: { label: 'Country', type: 'text', editable: false },
            Region: { label: 'Region', type: 'select', editable: false, required: true, valueList: [] },
            State: { label: 'State', type: 'select', editable: false, required: true, valueList: [] },
            Cluster: { label: 'Cluster', type: 'select', editable: false, required: true, valueList: [] },
            District: { label: 'District', type: 'select', editable: false, required: true, valueList: [] },
            City: { label: 'City', type: 'select', required: true, valueList: [] },
            CityCategory: { label: 'CityCategory', type: 'enum', required: true },
            Area: { label: 'Area', type: 'text', required: true },
            IsInUse: { type: "bool", required: true },
            Pincode: { label: 'Pincode', type: 'number', template: 'uint', editable: false, pattern: '/^[0-9]{6}$/', patternMessage: 'Only 6 digits required', required: true },
            selectedState:{label:'State',type:'enum'}
        };
    };

    var custbill = function () {
        return {
            AccountNo: { label: 'AccountNo', type: 'text' },
            GlobalCustId: { label: 'GlobalCustId', type: 'text' },
            Flag: { label: 'Flag', type: 'enum', valueList: $csShared.enums.FlagEnum },
            Product: { label: 'Product', type: 'enum', valueList: $csShared.enums.ProductEnum },
            IsInRecovery: { label: 'IsInRecovery', type: 'checkbox' },
            ChargeofDate: { label: 'ChargeofDate', type: 'date' },
            Cycle: { label: 'Cycle', type: 'number', template: 'uint' },
            Bucket: { label: 'Bucket', type: 'number', template: 'uint' },
            MobWriteoff: { label: 'MobWriteoff', type: 'number', template: 'uint' },
            Vintage: { label: 'Vintage', type: 'number', template: 'uint' },
            IsXHoldAccount: { label: 'IsXHoldAccount', type: 'checkbox' },
            AllocationStartDate: { label: 'AllocationStartDate', type: 'date' },
            AllocationEndDate: { label: 'AllocationEndDate', type: 'date' },
            TotalDueOnAllocation: { label: 'TotalDueOnAllocation', type: 'number', template: 'decimal' },
            TotalAmountRecovered: { label: 'TotalAmountRecovered', type: 'number', template: 'decimal' },
            ResolutionPercentage: { label: 'ResolutionPercentage', type: 'number', template: 'decimal' },
            GPincode: { label: 'GPincode', type: 'enum' },
            Stakeholders: { label: 'Stakeholders', type: 'enum' },
        };
    };

    var keyvalue = function () {
        return {
            Area: { label: 'Area', type: 'enum', valueList: $csShared.enums.Activities },
            Key: { label: 'Key', type: 'enum' },
            TextValue: { label: 'value', type: 'text', required: true, maxlength: 25 },
            NumberValue: { label: 'Value', type: 'number', template: 'decimal', required: true },
            DateValue: { label: 'Value', type: 'date', required: true },
            ValueType: { label: 'ValueType', type: 'enum' },
        };
    };

    var permission = function () {
        return {
            Activity: { label: "Activity", type: "enum", valuList: $csShared.enums.Activities },
            Permission: { type: "enum", valueList: $csShared.enums.Permissions },
            Vertical: { label: "Vertical", type: "enum", valueList: $csShared.enums.Vertical },
            EscalationDays: { type: "number", },
            Hierarchy: { label: "Hierarchy", type: "select", textField: "Hierarchy", valueField: "Hierarchy" },
            Designation: { label: "Designation", type: "select", textField: "Designation", valueField: "Id" }
        };
    };
    
    var product = function () {
        return {
            Product: { label: "Product Name", type: "text", maxlength: "40", placeholder: "Enter Product Name" },
            ProductGroup: { label: "Product Group", type: "text", maxlength: "40", placeholder: "Enter Product Group" },
            AllocationResetStrategy: { label: "Allocation Reset Strategy", type: "select", textField: "display", valueField: "value" },
            BillingResetStrategy: { label: "Billing Reset Strategy", type: "select",  textField: "display", valueField: "value" },
            HasTelecalling: { label: "HasTelecalling", type: "bool" },
            FrCutOffDaysCycle: { label: "FR Cycle Cut Off Days", type: "number", min: "0", max: "30", placeholder: "Enter FR Cycle Cut Off Days", required:true },
            FrCutOffDaysMonth: { label: "FR Month Cut Off Days", type: "number", min: "0", max: "30", placeholder: "Enter FR Month Cut Off Days", required: true },
            CycleCodes: { label:"Cycle Codes",type: "text",required:"true" },
            LinerTable: { type: "enum", valueList: $csShared.enums.ClientDataTables },
            WriteoffTable: { type: "enum", valueList: $csShared.enums.ClientDataTables },
            PaymentTable: { type: "enum", valueList: $csShared.enums.ClientDataTables },
        };
    };

    var gnotify = function() {
        return {
            NotificationType: { label: "NotificationType", type: "text" },
            EsclationDays: { label: "EsclationDays", type: "number", template: "uint", min: "1", max: "30" },
            NotifyHierarchy: { label: "NotifyHierarchy", type: "enum", valueList: $csShared.enums.NotifyHierarchy }
        };
    };

    var grid = function () {
        return {
            ReportName: { label: 'ReportName', type: 'text', maxlength: 100, minlength: 6, pattern: '/[A-Za-z0-9 ]+/', patternMessage: 'ReportName must not have special characters', required: true },
            Description: { label: 'Description', type: 'textarea', required: true, minlength: 6, maxlength: 250 },
            DoEmailReport: { label: 'Email Report', type: 'checkbox' },
            Frequency: { label: 'Email Frequency', type: 'select' },
            FrequencyParamDaily: { label: 'Which Hour of Day', type: 'select', valueField: 'key', textField: 'display' },
            FrequencyParamWeekly: { label: 'Which Day of Week', type: 'select', valueField: 'key', textField: 'display' },
            FrequencyParamMonthly: { label: 'Which Day of Month', type: 'select', valueField: 'key', textField: 'display' },
            UseFieldName4Header: { label: 'Use both field & display name as excel headers', type: 'checkbox' },
            SendOnlyIfData: { label: 'Send Only If Data', type: 'checkbox' },
            Send2Hierarchy: { label: 'Send Mail to Multiple', type: 'checkbox' },
            
            doResetFilter: { label: 'Filter', type: 'checkbox' },
            doResetRenames:{label:'Renames',type:'checkbox'},
            doResetSorting: { label: 'Sorting', type: 'checkbox' },
            doResetPosition: { label: 'Postion-Changes', type: 'checkbox' },
            doResetFreeze: { label: 'Freeze', type: 'checkbox' },
            doResetVisibility: { label: 'Column-Display', type: 'checkbox' },
        };
    };

    var init = function () {
        var models = {};

        models.TaxList = {
            Table: "TaxList",
            Columns: taxList(),
        };

        models.TaxMaster = {
            Table: "TaxMaster",
            Columns: taxMaster(),
        };

        models.Pincode = {
            Table: "Pincode",
            Columns: pincode(),
        };

        models.Custbill = {
            Table: "Custbill",
            Columns: custbill(),
        };

        models.Keyvalue = {
            Table: "Keyvalue",
            Columns: keyvalue(),
        };

        models.Permission = {
            Table: "Permission",
            Columns: permission(),
        };

        models.Product = {
            Table: "Product",
            Columns: product(),
        };

        models.Grid = {
            Table: "Grid",
            Columns: grid(),
        };
        
        models.GNotification = {
            Table: "GNotification",
            Columns: gnotify(),
        };

        return models;
    };

    return {
        init: init
    };
}]);