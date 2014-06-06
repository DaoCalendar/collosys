csapp.factory("$csLegalModels", function () {

    var requisitionPreparation = function () {
        return {
            Requis: { label: 'Select', type: 'radio', options: [{ value: 'Add', display: 'Add' }, { value: 'Edit', display: 'Edit' }, { value: 'Approve', display: 'Approve' }], textField: 'display', valueField: 'value' },
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