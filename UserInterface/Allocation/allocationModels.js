csapp.factory("$csAllocationModels", ["$csShared", function ($csShared) {

    var models = {};
    var allocSubpolicy = function () {
        return {
            Stakeholder: { label: "Select Stakeholder", type: "" },//TOBE Disscuss list type
            Name: { label: "Subpolicy Name", type: "text", pattern: "/^\w*$/", maxlength: 20, required: true },
            AllocateType: { label: "Policy Allocate Type", type: "enum", valueList: $csShared.enums.AllocationType, required: true },
            ReasonNotAllocate: { label: "Select Reason", type: "enum", valueList: "" },//TObe disscuss list
            NoAllocMonth: { label: "Allocate Months", type: "number", template: 'int', min: 0, required: true },
            Products: { label: "Product", type: "enum", valueList: $csShared.enums.Products, required: true },
            Product: { label: "Product", type: "text", required: true, editable: false },
            Category: { label: "Category", type: "enum", valueList: $csShared.enums.Category },
            CheckboxConditionOperators: { type: "enum", valueList: $csShared.enums.CheckboxConditionOperators, required: true },
            DropdownConditionOperators: { type: "enum", valueList: $csShared.enums.DropdownConditionOperators, required: true },
            ConditionOperators: { type: "enum", valueList: $csShared.enums.ConditionOperators, required: true },
            TextConditionOperators: { type: "enum", valueList: $csShared.enums.TextConditionOperators, required: true },
            DateValueEnum: { type: "enum", valueList: $csShared.enums.DateValueEnum, required: true },
        };
    };

    var allocPolicy = function () {
        return {
            Name: { label: "Policy Name", type: "text", pattern: "/^\w*$/", maxlength: 20, required: true },
            Products: { label: "Product", type: "enum", valueList: $csShared.enums.Products, required: true },
            Category: { label: "Category", type: "enum", valueList: $csShared.enums.Category },
            Status: { label: "Status", type: "enum", valueList: $csShared.enums.ApproveStatus },
            Description: { label: "Description", type: "text", required: true },
            ApprovedBy: { label: "ApprovedBy", type: "text", required: true },
            ApprovedOn: { label: "ApprovedOn", type: "date", required: true },

        };
    };

    var allocRelation = function () {
        return {
            AllocPolicy: { label: 'AllocPolicy', type: 'text' },
            AllocSubpolicy: { label: 'AllocSubpolicy', type: 'text' },
            Priority:{label:'Priority',type:'number',template:'uint'},
            StartDate: { label: 'Start Date', type: 'date', required: true },
            EndDate: { label: 'End Date', type: 'date' },
            Status: { label: "Status", type: "enum", valueList: $csShared.enums.ApproveStatus },
            Description: { label: "Description", type: "text", required: true },
            ApprovedBy: { label: "ApprovedBy", type: "text", required: true },
            ApprovedOn: { label: "ApprovedOn", type: "date", required: true },

        };
    };

    var allocCondition = function() {
        return {
            AllocSubpolicy: { label: 'AllocSubpolicy', type: 'text' },
            Priority: { label: 'Priority', type: 'number', template: 'uint' },
            ColumnName: { label: 'ColumnName', type: 'text' },
            Operator: { label: 'Operator', type: 'text', valueList: $csShared.enums.Operators },
            Value: { label: 'Value', type: 'text' },
            RelationType: { label: 'RelationType', type: 'text' },
            AllocStatus: { label: "AllocStatus", type: "enum", valueList: $csShared.enums.AllocStatus },
            NoAllocResons: { label: "NoAllocResons", type: "enum", valueList: $csShared.enums.NoAllocResons },

        };
    };

    var allocations = function() {
        return {
            AllocPolicy: { label: 'AllocPolicy', type: 'text' },
            AllocSubpolicy: { label: 'AllocSubpolicy', type: 'text' },
            Stakeholder: { label: "Select Stakeholder", type: "" },//TOBE Disscuss list type
            Info: { label: 'Info',type:'text' },
            StartDate: { label: 'Start Date', type: 'date', required: true },
            EndDate: { label: 'End Date', type: 'date' },
            WithTelecalling: { label: 'WithTelecalling', type: 'checkbox' },
            IsAllocated: { label: 'IsAllocated', type: 'checkbox' },
            Bucket: { label: 'Bucket', type: 'number', template: 'Int32' },
            AmountDue: { label: 'AmountDue', type: 'number', template: 'decimal' },
            ChangeReason: { label: 'ChangeReason', type: 'text'},
            Status: { label: "Status", type: "enum", valueList: $csShared.enums.ApproveStatus },
            Description: { label: "Description", type: "text", required: true },
            ApprovedBy: { label: "ApprovedBy", type: "text", required: true },
            ApprovedOn: { label: "ApprovedOn", type: "date", required: true },
        };
    };

    var init = function () {
        models.AllocSubpolicy = allocSubpolicy();
        models.AllocPolicy = allocPolicy();
        models.AllocRelation = allocRelation();
        models.AllocCondition = allocCondition();
        models.Allocations = allocations();
        return models;
    };
    return {
        init: init,
        models: models
    };
}]);