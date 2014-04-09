
csapp.factory("$csShared",  ["$csnotify", function ($csnotify) {
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

csapp.factory("$csFileUploadModels", ["$csShared", function ($csShared) {

    var fileDetail = function () {
        return {
            Frequency: {},
            FileCount : {}
        };
    };

    var fileColumn = function () {
        return {

        };
    };

    var init = function () {
        return {
            FileDetail: fileDetail(),
            FileColumn: fileColumn()
        };
    };

    return {
        init: init
    };
}]);

csapp.factory("$csStakeholderModels", ["$csShared", function () {
}]);

csapp.factory("$csModels", ["$csFileUploadModels", "$csStakeholderModels",
    function ($csFileUploadModels, $csStakeholderModels) {
        var models = {};
        var init = function () {
            models.FileUpload = $csFileUploadModels.init();
            models.Stakeholder = $csStakeholderModels.init();
            return;
        };
        return {
            init: init,
            FileUpload: models.FileUpload,
            Stakeholder: models.Stakeholder
        };
    }
]);