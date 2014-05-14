
csapp.factory("$csShared", function () {
    var enums = {};
    return {
        enums: enums,
    };
});

csapp.factory("$csModels", ["$csFileUploadModels", "$csStakeholderModels", "$csAllocationModels", '$csBillingModels', '$csGenericModels',
    function ($csFileUploadModels, $csStakeholderModels, $csAllocationModels, $csBillingModels, $csGenericModels) {

        var models = {};

        var init = function () {
            models.FileUpload = $csFileUploadModels.init();
            models.Stakeholder = $csStakeholderModels.init();
            models.AllocSubpolicy = $csAllocationModels.init();
            models.Billing = $csBillingModels.init();
            models.Generic = $csGenericModels.init();
            return;
        };

        var getTable = function (tableName) {
            switch (tableName) {
                case "FileDetail":
                    return angular.copy(models.FileUpload.FileDetail);

                case "FileColumn":
                    return angular.copy(models.FileUpload.FileDetail);

                case "FileMapping":
                    return angular.copy(models.FileUpload.fileDetail);

                case "FileStatus":
                    return angular.copy(models.FileUpload.fileDetail);

                case "FileScheduler":
                    return angular.copy(models.FileUpload.fileDetail);

                case "CustomerInfo":
                    return angular.copy(models.FileUpload.fileDetail);

                case "Payment":
                    return angular.copy(models.FileUpload.fileDetail);

                case "FCondition":
                    return angular.copy(models.FileUpload.fileDetail);

                case "FilterCondition":
                    return angular.copy(models.FileUpload.fileDetail);

                default:
                    throw "Invalid Table Name : " + tableName + ".";
            }
        };

        var getColumns = function (tableName) {
            var table = getTable(tableName);
            return angular.copy(table.Columns);
        };

        var getBillingColumns = function() {
            var table = getTable("CustomerInfo");
            return angular.copy(table.Columns);
        };

        return {
            init: init,
            getTable: getTable,
            getColumns: getColumns,
            getBillingColumns: getBillingColumns
        };
    }
]);