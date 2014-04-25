
csapp.factory("$csShared", ["$csnotify", function ($csnotify) {
    var enums = {};
    var getEnum = function (name) {
        _.forEach(enums, function (obj) {
            if (obj.Name === name) {
                return obj.Value;
            }
        });

    };
    return {
        enums: enums,
        getEnum: getEnum

    };
}]);

csapp.factory("$csModels", ["$csFileUploadModels", "$csStakeholderModels", "$csAllocationModels", '$csBillingModels','$csGenericModels',
    function ($csFileUploadModels, $csStakeholderModels, $csAllocationModels, $csBillingModels,$csGenericModels) {

        var models = {};

        var init = function () {
            models.FileUpload = $csFileUploadModels.init();
            models.Stakeholder = $csStakeholderModels.init;
            models.AllocSubpolicy = $csAllocationModels.init();
            models.Billing = $csBillingModels.init();
            models.Generic = $csGenericModels.init();
            return;
        };

        return {
            init: init,
            models: models
        };
    }
]);