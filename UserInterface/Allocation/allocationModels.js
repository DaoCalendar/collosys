csapp.factory("$csAllocationModels", ["$csShared", function ($csShared) {

    var allocSubpolicy = function () {
        return {
            Stakeholder: { label: "Stakeholder", type: "select", textField: "Name", valueField: "Id" },//TOBE Disscuss list type
            Name: { label: "Name", type: "text", maxlength: 20, required: true },
            AllocateType: {
                label: "Allocate Type", type: "select", valueField: "value", textField: "display",
                valueList: [{ display: "Handle By Telecaller", value: "HandleByTelecaller" },
                            { display: "Do Not Allocate", value: "DoNotAllocate" },
                            { display: "Allocate As Per Stakeholder Working", value: "AllocateAsPerPolicy" },
                            { display: "Allocate to Particular Stakeholder", value: "AllocateToStkholder" }],
                required: true
            },
            ReasonNotAllocate: { label: "Select Reason", type: "enum", valueList: [] },
            NoAllocMonth: { label: "Allocate Months", type: "number", template: 'int', min: 0, required: true },
            Products: { label: "Product", type: "enum", valueList:[], required: true },
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
            stakeholder: {label:"Stakeholder",type:"text"},
            reason: {label:"Reason",type:"text"},
            Product: { label: "Product", type: "enum", valueList:[] },
            startdate: { label: "StartDate:", type: 'date' },
            enddate: { label: "EndDate:", type: 'date' },
            StartDateText: { label: "Start Date:", type: 'date' },
            EndDateText: { label: "End Date:", type: 'date' },
            condition: { label: "Condition", type: "textarea", editable: false }
        };
    };

    var viewApprovePolicy = function () {
        return {
            Product: { label: "Product", type: "enum", valueList: [] },
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
                label: "Parameter", type: "select", valueField: "value", textField: "display",
                valueList: [{ value: "Donotallocate", display: "Do not allocate" },
                    { value: "AllocateToStakeholder", display: "Allocate to stakeholder" }]
            },
            Stakeholder: { label: "Stakeholder", type: "select", valueField: "row", textField: "Name", valueList: [] }

        };
    };

    var init = function () {
        var models = {};

        models.AllocSubpolicy = {
            Table: "AllocSubpolicy",
            Columns: allocSubpolicy()
        };

        models.AllocPolicy = {
            Table: "AllocPolicy",
            Columns: allocPolicy()
        };

        models.ViewApprovePolicy = {
            Table: "ViewApprovePolicy",
            Columns: viewApprovePolicy()
        };

        return models;
    };
    return {
        init: init
    };
}]);