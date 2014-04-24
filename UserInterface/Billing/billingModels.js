csapp.factory("$csBillingModels", ["$csShared", function ($csShared) {
    var models = {};

    var billAdhoc = function () {
        return {
            Stakeholder: { label: 'Stakeholder', type: 'text', required: true },
            Products: { label: "Product", type: "enum", valueList: $csShared.enums.ProductEnum, required: true, },
            TotalAmount: { label: 'Total Amount', type: 'text', pattern: '/^[0-9]+$/', patternMessage: 'Please insert valid amount', required: true },
            IsRecurring: { label: 'IsRecurring', type: 'checkbox', },
            IsPretax: { label: ' IsPretax', type: 'select' },
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
            Description: { label: "Description", type: "textarea" },
            OperatorType: { type: "enum", valueList: $csShared.enums.OperatorType },
            RelationType: { type: "enum", valueList: $csShared.enums.RelationType, },
            TypeSwitch: { type: "enum", valueList: $csShared.enums.TypeSwitch, required: true },
            DateValueEnum: { type: "enum", valueList: $csShared.enums.DateValueEnum, required: true },
            LsqlFunctionType: { type: "enum", valueList: $csShared.enums.LsqlFunctionType },
            ConditionOperators: { type: "enum", valueList: $csShared.enums.ConditionOperators, required: true },
        };
    };

    var formula = function () {
        return {
            Name: { label: "Name", type: "text", required: true },
            Products: { label: "Product", type: "enum", valueList: $csShared.enums.Products },
            Description: { label: "Description", type: "textarea" },
            GroupBy: { label: "Group By", type: "select", required: true },
            ConditionOperators: { type: "enum", valueList: $csShared.enums.ConditionOperators, required: true },
            OperatorType: { type: "enum", valueList: $csShared.enums.OperatorType },
            DateValueEnum: { type: "enum", valueList: $csShared.enums.DateValueEnum, required: true },
            RelationType: { type: "enum", valueList: $csShared.enums.RelationType, },
            LsqlFunctionType: { type: "enum", valueList: $csShared.enums.LsqlFunctionType },

        };
    };

    var matrix = function () {
        return {
            Name: { label: "Name", type: "text", required: true },
            Dimension: { label: "Dimension", type: "select", },// to be disscuss
            Row1DCount: { label: "Rows 1D", type: "uint", min: 1, max: 10 },
            Row1DType: { type: "enum", valueList: $csShared.enums.PayoutLRType },
            Row1DTypeName: { type: "text" },
            Column2DCount: { label: "Columns 2D", type: "uint", min: 1, max: 10 },
            Column2DType: { type: "enum", valueList: $csShared.enums.PayoutLRType },
            Column2DTypeName: { type: "text" },
            Row3DCount: { label: "Rows 3D", type: "uint", min: 1, max: 10 },
            Row3DType: { type: "enum", valueList: $csShared.enums.PayoutLRType },
            Row3DTypeName: { type: "text" },
            Column4DCount: { label: "Columns 4D", type: "uint", min: 1, max: 10 },
            Column4DType: { type: "enum", valueList: $csShared.enums.PayoutLRType },
            Column4DTypeName: { type: "text" },
            RowsOperator: { type: "enum", valueList: $csShared.enums.Operators },
            ColumnsOperator: { type: "enum", valueList: $csShared.enums.Operators },
            MatrixPerType: { type: "enum", valueList: $csShared.enums.PayoutLRType },
        };
    };

    var holdingPolicy = function () {
        return {
            Name: { label: 'Name', type: 'text', required: true },
            Description: { label: 'Description', type: 'text', required: true },
            StartDate: { label: 'Start Date', type: 'date', required: true },
            EndDate: { label: 'End Date', type: 'date' },
            ApplyOn: { label: 'Apply On', type: 'enum', valueList: $csShared.enums.ApplyOn, required: true },
            Products: { label: "Product", type: "enum", valueList: $csShared.enums.ProductEnum, required: true, },
            Rule: { label: "Rule", type: "enum", valueList: $csShared.enums.RuleForHolding, required: true, },
            Value: { label: 'Value', type: 'number', required: true },
            ValuePercent: { label: 'Value', type: 'number', template:'percentage', required: true },
            TransactionType: { label: 'Transaction Type', type: 'radio', options: [{ value: 'Fixed', key: 'Fixed' }, { value: 'Recurring', key: 'Recurring' }], valueField: 'value', textField: 'key', required: true },
            StartMonth: { label: 'Start Month', type: 'select', required: true, valueField: 'valuefield', textField: 'display' },
            Tenure: { label: 'Tenure', type: 'number', max: 24, min: 1 },
        };
    };

    var activateHolding = function () {
        return {
            HoldingPolicy: { label: 'Stakeholder', type: 'text', required: true },
            Stakeholder: { label: 'Stakeholder', type: 'text', required: true },
            Products: { label: "Product", type: "enum", valueList: $csShared.enums.ProductEnum, required: true, },
        };
    };

    var init = function () {
        models.BillAdhoc = billAdhoc();
        models.BillAmount = billAmount();
        models.BillingSubpolicy = billingSubpolicy();
        models.Formula = formula();
        models.Matrix = matrix();
        models.HoldingPolicy = holdingPolicy();
        models.ActivateHoldingPolicy = activateHolding();
        return models;
    };

    return {
        init: init,
        models: models
    };
}]);