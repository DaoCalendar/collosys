csapp.factory("$csLegalModels", function () {

    var requisitionPreparation = function () {
        return {
            Prepare: { label: 'Select options', type: 'radio', options: [{ value: 'Add', display: 'Add' }, { value: 'Edit', display: 'Edit' }, { value: 'Approve', display: 'Approve' }], textField: 'display', valueField: 'value' },
            City: { label: 'City', type: 'select' },
            Products: { label: 'Products', type: 'select' },
            LoanNo: { label: 'Loan No', type: 'text' },
            LoanStatus: { label: 'Loan Status', type: 'select' },
            CaseStatus: { label: 'Case Status', type: 'select' },
        };
    };
   
    var requisitionIntiation = function () {
        return {
            Function: { label: 'Function', type: 'radio', options: [{ value: 'Initiate', display: 'Initiate' }, { value: 'Initiated', display: 'Initiated' }], textField: 'display', valueField: 'value' },
            Location: { label: "Location", type: "select", },
            Division: { label: "Division", type: "select", },
            LoanNo: { label: "Loan No", type: "text" },
            DateFrom: { label: "Loan Date To From", type: "date" },
            DateTo: { label: "Loan Date To", type: "date" },
            RequsitionNo: { label: "Requsition No", type: "text" },
            RequsitionDateFrom: { label: "Requsition Date From", type: "date" },
            RequsitionDateTo: { label: "Requsition Date To", type: "date" },

        };
    };

    var init = function () {
        var models = {};

        models.RequisitionPreparation = {
            Table: "RequisitionPreparation",
            Columns: requisitionPreparation(),
        };

        models.RequsitionIntiation = {
            Table: "RequsitionIntiation",
            Columns: requisitionIntiation(),
        };
        return models;
    };

    return {
        init: init
    };
});