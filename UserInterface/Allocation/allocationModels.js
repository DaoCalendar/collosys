csapp.factory("$csAllocationModels", ["$csShared", function ($csShared) {

    var models = {};
    var allocSubpolicy = function () {
        return {
            Stakeholder: { label: "Select Stakeholder", type: "" },//TOBE Disscuss list type
            Name: { label: "Name", type: "text", maxlength: 20, required: true },
            AllocateType: {
                label: "Allocate Type", type: "select", valueField:"value", textField:"display",
                valueList: [{ display: "Handle By Telecaller", value: "HandleByTelecaller" },
        { display: "Do Not Allocate", value: "DoNotAllocate" },
        { display: "Allocate As Per Stakeholder Working", value: "AllocateAsPerPolicy" },
        { display: "Allocate to Particular Stakeholder", value: "AllocateToStkholder" }], required: true},
            ReasonNotAllocate: { label: "Select Reason", type: "enum", valueList: [] },
            NoAllocMonth: { label: "Allocate Months", type: "number", template: 'int', min: 0, required: true },
            Products: { label: "Product", type: "enum", valueList: $csShared.enums.Products, required: true },
            ProductName: { label: "Product", type: "text", required: true, editable: false },
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

        };
    };

    var init = function () {
        models.AllocSubpolicy = allocSubpolicy();
        models.AllocPolicy = allocPolicy();
        return models;
    };
    return {
        init: init,
        models: models
    };
}]);