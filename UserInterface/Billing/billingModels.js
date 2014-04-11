csapp.factory("$csBillingModels", ["$csShared", function ($csShared) {
    var models = {};

    var billAdhoc = function () {
        return {
            Stakeholder: { label: 'Hierarchy', type: 'text' },
            Products: { label: "Product", type: "enum", valueList: $csShared.enums.Products, required: true, },
            TotalAmount: { label: 'Total Amount', type: 'text', pattern: '/^[0-9]+$/', patternMessage: 'Please insert valid amount', required: true },
            IsRecurring: { label: 'IsRecurring', type: 'checkbox', },
            IsCredit: { label: 'Transaction Type', type: 'select', required: true },
            ReasonCode: { label: 'Reason', type: 'select', required: true, valueField: 'display', textField: 'display' },
            StartMonth: { label: 'Start Month', type: 'select' },
            Tenure: { label: 'Tenure', type: 'text', pattern: '/^[0-9]+$/', patternMessage: 'Tenure must be in 0-9' },
            Description: { label: 'Description', type: 'text', required: true }
        };
    };

    var init = function () {
        models.BillAdhoc = billAdhoc();
        return models;
    };

    return {
        init: init,
        models: models
    };
}]);