csapp.factory("$csGenericModels", ["$csShared", function ($csShared) {
    var models = {};

    var taxList = function () {
        return {
            TaxName: { label: 'Name', type: 'text', required: true },
            TaxType: { label: 'Type', type: 'select', required: true },
            ApplicableTo: { label: 'Applicable To', type: 'select', required: true },
            IndustryZone: { label: 'Industry Zone', type: 'text', required:true },
            ApplyOn: { label: 'Apply On', type: 'select', required: true },
            TotSource: { label: 'TOT Source', type: 'text', required: true},
            Description: { label: 'Description', type: 'textarea', required: true }
        };
    };

    var taxMaster = function () {
        return {
            
        };
    };

    var init = function () {
        models.TaxList = taxList();
        models.TaxMaster = taxMaster();
        return models;
    };

    return {
        init: init,
        models: models
    };
}]);