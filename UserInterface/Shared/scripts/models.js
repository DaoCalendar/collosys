
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
                    return angular.copy(models.FileUpload.FileColumn);

                case "FileMapping":
                    return angular.copy(models.FileUpload.FileMapping);

                case "FileStatus":
                    return angular.copy(models.FileUpload.FileStatus);

                case "FileScheduler":
                    return angular.copy(models.FileUpload.FileScheduler);

                case "CustomerInfo":
                    return angular.copy(models.FileUpload.CustomerInfo);

                case "Payment":
                    return angular.copy(models.FileUpload.Payment);

                case "FCondition":
                    return angular.copy(models.FileUpload.FCondition);

                case "FilterCondition":
                    return angular.copy(models.FileUpload.FilterCondition);

                case "Stakeholder":
                    return angular.copy(models.FileUpload.Stakeholder);

                case "StkhHierarchy":
                    return angular.copy(models.FileUpload.StkhHierarchy);

                case "StkhWorking":
                    return angular.copy(models.FileUpload.StkhWorking);

                case "StkhPayment":
                    return angular.copy(models.FileUpload.StkhPayment);

                case "StkhRegistration":
                    return angular.copy(models.FileUpload.StkhRegistration);

                case "StakeAddress":
                    return angular.copy(models.FileUpload.StakeAddress);

                case "AllocSubpolicy":
                    return angular.copy(models.FileUpload.AllocSubpolicy);

                case "AllocPolicy":
                    return angular.copy(models.FileUpload.AllocPolicy);

                case "ViewApprovePolicy":
                    return angular.copy(models.FileUpload.ViewApprovePolicy);

                case "BillAdhoc":
                    return angular.copy(models.FileUpload.BillAdhoc);

                case "AdhocPayout":
                    return angular.copy(models.FileUpload.AdhocPayout);

                case "ReadyForBilling":
                    return angular.copy(models.FileUpload.ReadyForBilling);

                case "Summary":
                    return angular.copy(models.FileUpload.Summary);

                case "BillAmount":
                    return angular.copy(models.FileUpload.BillAmount);

                case "BillingPolicy":
                    return angular.copy(models.FileUpload.BillingPolicy);

                case "BillingSubpolicy":
                    return angular.copy(models.FileUpload.BillingSubpolicy);

                case "Formula":
                    return angular.copy(models.FileUpload.Formula);

                case "Matrix":
                    return angular.copy(models.FileUpload.Matrix);

                case "HoldingPolicy":
                    return angular.copy(models.FileUpload.HoldingPolicy);

                case "ActivateHoldingPolicy":
                    return angular.copy(models.FileUpload.ActivateHoldingPolicy);

                case "TaxList":
                    return angular.copy(models.FileUpload.TaxList);

                case "TaxMaster":
                    return angular.copy(models.FileUpload.TaxMaster);

                case "Pincode":
                    return angular.copy(models.FileUpload.Pincode);

                case "Custbill":
                    return angular.copy(models.FileUpload.Custbill);

                case "Keyvalue":
                    return angular.copy(models.FileUpload.Keyvalue);

                case "Permission":
                    return angular.copy(models.FileUpload.Permission);

                case "Product":
                    return angular.copy(models.FileUpload.Product);

                case "Grid":
                    return angular.copy(models.FileUpload.Grid);

                default:
                    throw "Invalid Table Name : " + tableName + ".";
            }
        };

        var getColumns = function (tableName) {
            var table = getTable(tableName);
            return angular.copy(table.Columns);
        };

        var getBillingColumns = function () {
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