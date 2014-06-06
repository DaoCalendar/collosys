csapp.factory("$csLegalModels", function () {

    var requisitionPreparation = function () {
        return {
            Prepare: { label: 'Select options', type: 'radio', options: [{ value: 'Add', display: 'Add' }, { value: 'Edit', display: 'Edit' }, { value: 'Approve', display: 'Approve' }], textField: 'display', valueField: 'value' },
            City: { label: 'City', type: 'select' },
            Products:{label: 'Products', type: 'select'},
            LoanNo: { label: 'Loan No', type: 'text' },
            LoanStatus: { label: 'Loan Status', type: 'select' },
            CaseStatus: { label: 'Case Status', type: 'select' },
        };
    };


    var init = function () {
        var models = {};

        models.RequisitionPreparation = {
            Table: "RequisitionPreparation",
            Columns: requisitionPreparation(),
        };
        return models;
    };



    return {
        init: init
    };
});