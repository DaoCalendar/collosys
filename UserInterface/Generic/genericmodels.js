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
<<<<<<< HEAD
            Percentage: { label: 'Percentage', type: 'decimal', max: 99.99, min: 1 }, //pattern: '/^$|^\d{0,2}(\.\d{1,2})? *%?$/'
=======
            Percentage: { label: 'Percentage', type: 'text',template:'percentage' }, //pattern: '/^$|^\d{0,2}(\.\d{1,2})? *%?$/'
>>>>>>> f1f2f65aab60f2ca71c8565c982eb47656bdd060
            StartDate: { label: 'Start Date', type: 'date', required: true },
            EndDate: { label: 'End Date', type: 'date' }
        };
    };

    var editpincode = function () {
        return {
            Country: { label: 'Country', type: 'text', editable: false, },
            Region: { label: 'Region', type: 'select',valueList:[] },
            State: { label: 'State', type: 'select', editable: true, valueList: [] },
            Cluster: { label: 'Cluster', type: 'select', editable: true, valueList: [] },
            District: { label: 'District', type: 'select', editable: true, valueList: [] },
            City: { label: 'City', type: 'select', required: true, valueList: [] },
            CityCategory: { label: 'CityCategory', type: 'enum', valueList: $csShared.enums.CityCategory, required: true },
            Area: { label: 'Area', type: 'text', required: true },
            Pincode: { label: 'Pincode', type: 'int', editable: false, pattern: '/^[0-9]{6}$/' }
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