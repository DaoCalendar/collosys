csapp.factory("$csBillingModels", ["$csShared", function ($csShared) {

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

    var adhocpayout = function () {
        return {
            selectedProduct: { label: "Product", type: "enum", valueList: $csShared.enums.ProductEnum, required: true },
            TotalAmount: { label: 'Total Amount', type: 'text', pattern: '/^[0-9]+$/', patternMessage: 'Please insert valid amount', required: true },
            IsRecurring: { label: 'IsRecurring', type: 'checkbox' },
            IsPretax: { label: ' IsPretax', type: 'select' },
            IsCredit: { label: 'Transaction Type', valueField: 'value', textField: 'display', type: 'select', required: true, valueList: [] },
            ReasonCode: { label: 'Reason', type: 'select', required: true, valueField: 'display', textField: 'display' },
            StartMonth: { label: 'Start Month', type: 'date', template: 'MonthPicker', required: true, valueField: 'Key', textField: 'Value' },
            Tenure: { label: 'Tenure', type: 'number', template: 'uint', pattern: '/^[0-9]+$/', patternMessage: 'Tenure must be in 0-9' },
            Description: { label: 'Description', type: 'textarea', required: true },
            Stakeholder: { label: "Stakeholder Name", type: "select", valueField: "Id", textField: "Name" }
        };
    };

    var billingPolicy = function () {
        return {
            Name: { label: 'Name', type: 'text' },
            Products: { label: 'Product', type: 'enum', valueList: $csShared.enums.Products },
            Category: { label: 'Category', type: 'enum', valueList: $csShared.enums.Category },
            Output: { label: "Output", type: "textarea" },
            Condition: { label: "Condition", type: "textarea" },

        };
    };

    var summary = function () {
        return {
            Product: { label: "Product", type: "enum", valueList: $csShared.enums.ProductEnum, required: true },
            Month: { label: 'Month', type: 'date', template: 'MonthPicker', required: true, valueField: 'Key', textField: 'Value' },
            FixedAmount: { label: 'FixedAmount', type: 'number', template: 'decimal', editable: false },
            StartDate: { label: 'StartDate', type: 'text', editable: false },
            EndDate: { label: 'EndDate', type: 'text', editable: false },
            VariableAmount: { label: 'Variable Amount', type: 'number', template: 'decimal', editable: false },
            DeductionIncentive: { label: "Deduction/Incentive", type: "text" },
            TaxAmount: { label: 'TaxAmount', type: 'number', template: 'decimal', editable: false },
            HoldAmount: { label: 'HoldAmount', type: 'number', template: 'decimal', editable: false },
            HoldRepayment: { label: 'HoldRepayment', type: 'number', template: 'decimal', editable: false },
            Stakeholder: { label: "Stakeholder", type: "select", valueField: "Id", textField: "Name" },
            // TotalAmount: { label: 'Total Amount', type: 'number', template: 'decimal', pattern: '/^[0-9]+$/', patternMessage: 'Please insert valid amount', required: true },
            TotalAmount: { label: 'Total Amount', type: 'number', template: 'decimal', required: true },
            IsCredit: { label: 'Transaction Type', valueField: 'value', textField: 'display', type: 'select', required: true, valueList: [] },
            IsPretax: { label: ' IsPretax', type: 'select' },
            ReasonCode: { label: 'Reason', type: 'select', required: true, valueField: 'display', textField: 'display' },
            Description: { label: 'Description', type: 'textarea', required: true },
        };
    };

    var readyforbilling = function () {
        return {
            Products: { label: "Product", type: "enum", valueList: $csShared.enums.ProductEnum, required: true },
            BillMonth: { label: "Month", type: "date", template: "MonthPicker" }
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
            Productname: { type: "text", label: "Product", required: true },
            Products: { label: "Product", required: true, type: "enum", valueList: $csShared.enums.Products },
            Category: { label: "Category", required: true, type: "enum", valueList: $csShared.enums.Category },
            LtypeName: { type: "select", textField: "displayName", valueField: "field" },
            typeSelect: { type: "select", textField: "displayName", valueField: "field" },
            typeEnum: { type: "enum", valueList: [] },
            PayoutSubpolicyType: { label: "PayoutSubPolicy", required: true, type: "enum", valueList: $csShared.enums.PayoutSubpolicyType },
            OutputType: { label: "Output", required: true, type: "enum", valueList: $csShared.enums.OutputType },
            GroupBy: { label: "Group By", type: "select", required: true, textField: "displayName", valueField: "field" },//TOBE Disscuss
            Description: { label: "Description", type: "textarea" },
            OperatorType: { type: "enum", valueList: $csShared.enums.OperatorType },
            RelationType: { type: "enum", valueList: $csShared.enums.RelationType, },
            TypeSwitch: { type: "enum", valueList: $csShared.enums.TypeSwitch, required: true },
            DateValueEnum: { type: "enum", valueList: $csShared.enums.DateValueEnum, required: true },
            LsqlFunctionType: { type: "enum", valueList: $csShared.enums.LsqlFunctionType },
            ConditionOperators: { type: "enum", valueList: $csShared.enums.ConditionOperators, required: true },
            PolicyType: { type: "enum", label: "Policy Type", valueList: $csShared.enums.PolicyType, required: true },
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
            ValueType: { type: "enum", valueList: ['Formula', 'Value', 'Table'] },
            ProcessingFee: { label: 'Processing Fee', type: 'select' },
            PayoutCapping: { label: 'Payout Capping', type: 'select' },
            OutputType: { label: 'Output Type', type: 'select', valueList: ['Number', 'Boolean', 'IfElse', 'MultiIfElse'], required: true }
        };
    };

    var matrix = function () {
        return {
            Name: { label: "Name", type: "text", required: true },
            Product: { type: "enum", label: "Product", valueList: $csShared.enums.Products },
            Description: { label: "Description", type: "textarea" },
            Dimension: { label: "Dimension", type: "select", valueField: "value", textField: "text", valueList: [{ value: "1", text: "1D" }, { value: "2", text: "2D" }] },
            RowDCount: { type: "number", min: 1, max: 10 },
            RowDTypeName: { type: "select", valueList: [] },
            Operator: { type: "select", valueField: "value", textField: "text", valueList: [{ value: "EqualTo", text: "EqualTo" }, { value: "GreaterThan", text: "Greater Than" }, { value: "LessThan", text: "Less Than" }] },
            MatrixPerType: { label: "Matrix Per Type", type: "enum", valueList: ["Table", "Formula"] },
            MatrixPerTypeName: { type: "enum", valueList: [], label: "Matrix Per Type Name" }
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
            Value: { label: 'Value', type: 'number', template: 'decimal', required: true },
            ValuePercent: { label: 'Value', type: 'number', template: 'percentage', required: true },
            TransactionType: { label: 'Transaction Type', type: 'radio', options: [{ value: 'Fixed', key: 'Fixed' }, { value: 'Recurring', key: 'Recurring' }], valueField: 'value', textField: 'key', required: true },
            Tenure: { label: 'Tenure', type: 'number', template: 'int', max: 24, min: 0 },
        };
    };

    var activateHolding = function () {
        return {
            HoldingPolicy: { label: 'Holding Policy', type: 'select', textField: 'Name', required: true },
            Stakeholder: { label: 'Stakeholder', type: 'select', textField: 'Name', required: true },
            Products: { label: "Product", type: "enum", valueList: $csShared.enums.ProductEnum, required: true, },
            StartMonth: { label: 'Start Month', type: 'select', required: true, valueField: 'valuefield', textField: 'display' }
        };
    };

    var init = function () {
        var models = {};

        models.BillAdhoc = {
            Table: "BillAdhoc",
            Columns: billAdhoc()
        };

        models.AdhocPayout = {
            Table: "AdhocPayout",
            Columns: adhocpayout()
        };

        models.ReadyForBilling = {
            Table: "ReadyForBilling",
            Columns: readyforbilling()
        };

        models.Summary = {
            Table: "Summary",
            Columns: summary()
        };

        models.BillAmount = {
            Table: "BillAmount",
            Columns: billAmount()
        };

        models.BillingPolicy = {
            Table: "BillingPolicy",
            Columns: billingPolicy()
        };

        models.BillingSubpolicy = {
            Table: "BillingSubpolicy",
            Columns: billingSubpolicy()
        };

        models.Formula = {
            Table: "Formula",
            Columns: formula()
        };

        models.Matrix = {
            Table: "Matrix",
            Columns: matrix()
        };

        models.HoldingPolicy = {
            Table: "HoldingPolicy",
            Columns: holdingPolicy()
        };

        models.ActivateHoldingPolicy = {
            Table: "ActivateHoldingPolicy",
            Columns: activateHolding()
        };

        return models;
    };

    return {
        init: init
    };
}]);