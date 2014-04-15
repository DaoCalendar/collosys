csapp.factory("$csBillingModels", ["$csShared", function ($csShared) {
    var models = {};

    var billAdhoc = function () {
        return {
            Stakeholder: { label: 'Stakeholder', type: 'text', required: true },
            Products: { label: "Product", type: "enum", valueList: $csShared.enums.ProductEnum, required: true, },
            TotalAmount: { label: 'Total Amount', type: 'text', pattern: '/^[0-9]+$/', patternMessage: 'Please insert valid amount', required: true },
            IsRecurring: { label: 'IsRecurring', type: 'checkbox', },
            IsCredit: { label: 'Transaction Type', type: 'select', required: true },
            ReasonCode: { label: 'Reason', type: 'select', required: true, valueField: 'display', textField: 'display' },
            StartMonth: { label: 'Start Month', type: 'select', required: true, valueField: 'Key', textField: 'Value' },
            Tenure: { label: 'Tenure', type: 'text', pattern: '/^[0-9]+$/', patternMessage: 'Tenure must be in 0-9' },
            Description: { label: 'Description', type: 'text', required: true }
        };
    };

    var billAmount = function () {
        return {
            Stakeholder: { label: 'Stakeholder', type: 'text', required: true },
            PayStatus: { label: 'Payment Status', type: 'enum', valueList: $csShared.enums.BillPaymentStatus },
            PayStatusDate: { label: 'Payment Status Date', type: 'date' },
            Month: { label: 'Month', type: 'date', template: 'MonthPicker', required: true },
            Products: { label: "Product", type: "enum", valueList: $csShared.enums.ProductEnum, required: true, }
        };
    };

    var billingSubpolicy = function () {
        return {
            Name: { label: "Name", type: "text", required: true },
            Products: { label: "Product", required: true, type: "enum", valueList: $csShared.enums.Products },
            Category: { label: "Category", required: true, type: "enum", valueList: $csShared.enums.Category },
            PayoutSubpolicyType: { label: "PayoutSubPolicy", required: true, type: "enum", valueList: $csShared.enums.PayoutSubpolicyType },
            OutputType: { label: "Output", required: true, type: "enum", valueList: $csShared.enums.OutputType },
            GroupBy: { label: "Group By", type: "select", required: true },//TOBE Disscuss
            Description: { label: "Description", type: "textarea" }
        };
    };

    var init = function () {
        models.BillAdhoc = billAdhoc();
        models.BillAmount = billAmount();
        models.BillingSubpolicy = billingSubpolicy();
        return models;
    };

    return {
        init: init,
        models: models
    };
}]);