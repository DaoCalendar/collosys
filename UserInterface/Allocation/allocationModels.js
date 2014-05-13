csapp.factory("$csAllocationModels", ["$csShared", function ($csShared) {

    var models = {};
    var allocSubpolicy = function () {
        return {
            Stakeholder: { label: "Select Stakeholder", type: "select", textField: "Name", valueField: "Id" },//TOBE Disscuss list type
            Name: { label: "Name", type: "text", maxlength: 20, required: true },
            AllocateType: {
                label: "Allocate Type", type: "select", valueField: "value", textField: "display",
                valueList: [{ display: "Handle By Telecaller", value: "HandleByTelecaller" },
        { display: "Do Not Allocate", value: "DoNotAllocate" },
        { display: "Allocate As Per Stakeholder Working", value: "AllocateAsPerPolicy" },
        { display: "Allocate to Particular Stakeholder", value: "AllocateToStkholder" }], required: true
            },
            ReasonNotAllocate: { label: "Select Reason", type: "enum", valueList: [] },
            NoAllocMonth: { label: "Allocate Months", type: "number", template: 'int', min: 0, required: true },
            Products: { label: "Product", type: "enum", valueList: $csShared.enums.Products, required: true },
            ProductName: { label: "Product", type: "text", required: true, editable: false },
            ColumnName: { type: "enum", valueList: [] },
            Description: { label: "Description", type: "textarea" },
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
            Product: { label: "Product", type: "enum", valueList: $csShared.enums.Products },
            startdate: { label: "StartDate:", type: 'date' },
            enddate: { label: "EndDate:", type: 'date' },
        };
    };

    var viewApprovePolicy = function () {
        return {
            Product: { label: "Product", type: "enum", valueList:[] },
            Todate: { label: "To Date", type: "date" },
            Fromdate: { label: "From Date", type: "date" },
            AllocStatus: {
                label: "Allocation Status", type: "select", valueField: "value",
                textField: "display",
                valueList: [{ value: "None", display: "All" },
                    { value: "DoNotAllocate", display: "Do not allocate" },
                    { value: "AllocateToTelecalling", display: "Handle by telecaller" },
                    { value: "AllocateToStakeholder", display: "Allocated to particular stakeholder" },
                    { value: "AsPerWorking", display: "Allocated as per policy" },
                    { value: "AllocationError", display: "Error in allocation" },
                    { value: "Submitted", display: "Approve-changed allocation" }]
            },
            Allocatetype: {
                label: "Select Parameter", type: "select", valueField: "value", textField: "display",
                valueList: [{ value: "AllocateToTelecalling", display: "Handle by telecaller" },
                    { value: "Donotallocate", display: "Do not allocate" },
                    { value: "AllocateToStakeholder", display: "Allocate to stakeholder" }]
            },
            Stakeholder: {label:"Select Stakeholder",type:"select",valueField:"row",textField:"Name",valueList:[]}

        };
    };

    var init = function () {
        models.AllocSubpolicy = allocSubpolicy();
        models.AllocPolicy = allocPolicy();
        models.ViewApprovePolicy = viewApprovePolicy();
        return models;
    };
    return {
        init: init,
        models: models
    };
}]);