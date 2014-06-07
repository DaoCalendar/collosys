
csapp.factory("$csShared", function () {
    var enums = {};
    var permissions = {};
    var menus = {};
    return {
        enums: enums,
        permissions: permissions,
        menus:menus
    };
});

csapp.factory("$csModels", ["$csFileUploadModels", "$csStakeholderModels", "$csAllocationModels", '$csBillingModels', '$csGenericModels', '$csLegalModels',
    function ($csFileUploadModels, $csStakeholderModels, $csAllocationModels, $csBillingModels, $csGenericModels, $csLegalModels) {

        var models = {};

        var init = function () {
            models.FileUpload = $csFileUploadModels.init();
            models.Stakeholder = $csStakeholderModels.init();
            models.Allocation = $csAllocationModels.init();
            models.Billing = $csBillingModels.init();
            models.Generic = $csGenericModels.init();
            models.Legal = $csLegalModels.init();
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

                case "Fcondition":
                    return angular.copy(models.FileUpload.Fcondition);

                case "FilterCondition":
                    return angular.copy(models.FileUpload.FilterCondition);

                case "Stakeholder":
                    return angular.copy(models.Stakeholder.Stakeholder);

                case "StkhHierarchy":
                    return angular.copy(models.Stakeholder.StkhHierarchy);

                case "StkhWorking":
                    return angular.copy(models.Stakeholder.StkhWorking);

                case "StkhPayment":
                    return angular.copy(models.Stakeholder.StkhPayment);

                case "StkhRegistration":
                    return angular.copy(models.Stakeholder.StkhRegistration);

                case "StakeAddress":
                    return angular.copy(models.Stakeholder.StakeAddress);

                case "AllocSubpolicy":
                    return angular.copy(models.Allocation.AllocSubpolicy);

                case "AllocPolicy":
                    return angular.copy(models.Allocation.AllocPolicy);

                case "ViewApprovePolicy":
                    return angular.copy(models.Allocation.ViewApprovePolicy);

                case "BillAdhoc":
                    return angular.copy(models.Billing.BillAdhoc);

                case "AdhocPayout":
                    return angular.copy(models.Billing.AdhocPayout);

                case "ReadyForBilling":
                    return angular.copy(models.Billing.ReadyForBilling);

                case "Summary":
                    return angular.copy(models.Billing.Summary);

                case "BillAmount":
                    return angular.copy(models.Billing.BillAmount);

                case "BillingPolicy":
                    return angular.copy(models.Billing.BillingPolicy);

                case "BillingSubpolicy":
                    return angular.copy(models.Billing.BillingSubpolicy);

                case "Formula":
                    return angular.copy(models.Billing.Formula);

                case "Matrix":
                    return angular.copy(models.Billing.Matrix);

                case "HoldingPolicy":
                    return angular.copy(models.Billing.HoldingPolicy);

                case "ActivateHoldingPolicy":
                    return angular.copy(models.Billing.ActivateHoldingPolicy);

                case "TaxList":
                    return angular.copy(models.Generic.TaxList);

                case "TaxMaster":
                    return angular.copy(models.Generic.TaxMaster);

                case "Pincode":
                    return angular.copy(models.Generic.Pincode);

                case "Custbill":
                    return angular.copy(models.Generic.Custbill);

                case "Keyvalue":
                    return angular.copy(models.Generic.Keyvalue);

                case "Permission":
                    return angular.copy(models.Generic.Permission);

                case "Product":
                    return angular.copy(models.Generic.Product);
                    
                case "RequisitionPreparation":
                    return angular.copy(models.Legal.RequisitionPreparation);
                    
                case "RequisitionIntiation":
                    return angular.copy(models.Legal.RequisitionIntiation);
                    
                case "LegalCaseexecution":
                    return angular.copy(models.Legal.LegalCaseexecution);
                    
                case "FollowUp":
                    return angular.copy(models.Legal.FollowUp);
               
                case "Grid":
                    return angular.copy(models.Generic.Grid);

                default:
                    throw "Invalid Table Name : " + tableName + ".";
            }
        };

        var getColumns = function (tableName) {
            var table = getTable(tableName);
            return table.Columns;
        };

        var getBillingColumns = function () {
            var table = getTable("CustomerInfo");
            return table.Columns;
        };

        return {
            init: init,
            getTable: getTable,
            getColumns: getColumns,
            getBillingColumns: getBillingColumns
        };
    }
]);