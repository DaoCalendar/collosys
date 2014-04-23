csapp.factory("$csGenericModels", ["$csShared", function ($csShared) {
    var models = {};

    var taxList = function () {
        return {
            TaxName: { label: 'Name', type: 'text', required: true },
            TaxType: { label: 'Type', type: 'enum', valueList: $csShared.enums.TaxType, required: true },
            ApplicableTo: { label: 'Applicable To', type: 'enum', valueList: $csShared.enums.TaxApplicableTo, required: true },
            IndustryZone: { label: 'Industry Zone', type: 'text', required: true },
            ApplyOn: { label: 'Apply On', type: 'enum', valueList: $csShared.enums.TaxApplyOn, required: true },
            TotSource: { label: 'TOT Source', type: 'text', required: true },
            Description: { label: 'Description', type: 'textarea', required: true }
        };
    };

    var taxMaster = function () {
        return {
            GTaxesList: { label: 'Tax List', type: 'select', textField: 'TaxName' },
            ApplicableTo: { label: 'Role', type: 'enum', valueList: $csShared.enums.TaxApplicableTo, required: true },
            IndustryZone: { label: 'Industry Zone', type: 'text' },
            Country: { label: 'Country', type: 'text' },
            State: { label: 'State', type: 'enum', valueList:[] },
            District: { label: 'District', type: 'text' },
            Priority: {},
            // public virtual UInt64 TaxId { get; set; }
            Percentage: { label: 'Percentage', type: 'number',template:'percentage' }, //pattern: '/^$|^\d{0,2}(\.\d{1,2})? *%?$/'
            StartDate: { label: 'Start Date', type: 'date', required: true },
            EndDate: { label: 'End Date', type: 'date' }
        };
    };

    var editpincode = function () {
        return {
            Country: { label: 'Country', type: 'text', editable: false },
            Region: { label: 'Region', type: 'select',editable: false,valueList:[] },
            State: { label: 'State', type: 'select', editable: false, valueList: [] },
            Cluster: { label: 'Cluster', type: 'select', editable: false, valueList: [] },
            District: { label: 'District', type: 'select', editable: false, valueList: [] },
            City: { label: 'City', type: 'select', required: true, valueList: [] },
            CityCategory: { label: 'CityCategory', type: 'enum', valueList: $csShared.enums.CityCategory, required: true },
            Area: { label: 'Area', type: 'text', required: true },
            Pincode: { label: 'Pincode', type: 'text', editable: false, pattern: '/^[0-9]{6}$/', required:true }
        };
    };

    var init = function () {
        models.TaxList = taxList();
        models.TaxMaster = taxMaster();
        models.Pincode = editpincode();
        return models;
    };

    return {
        init: init,
        models: models
    };
}]);