csapp.factory("StakeholderModels", ["StakeholderEnums", function (enums) {

    var basicInfoModel = {
        Hierarchy: { type: "text", label: "Hierarchy", required: true },
        Designation:{type:"text",label:"Designation",required:true},
        Name: { type: "text", label: "Name", required: true },
        UserId: { type: "number", label: "User Id", required: true, minlength: 7, maxlength: 7 },
        MobileNo: { type: "number", label: "Mobile Number", required: true, minlength: 10, maxlength: 10 },
        Gender: { type: "enum", label: "Gender", required: true, value: enums.gender },
        DateOfBirth: { type: "text", label: "Date Of Birth", required: true, maxlength: 20 },
        DateOfJoining: { type: "text", label: "Date Of Joining", required: true, maxlength: 20 }
    };

    var workingPayment = {
        
         Working :{
            Products: { type: "enum", label: "Products", required: true, value: enums.products },
            //product manager (to be discussed)
            //pincode display manager (to be discussed)
         },
         
         Payment: {
             VariablePay: {
                 CollectionPolicy: { type: "text", label: "Collection Policy", required: false },
                 RecoveryPolicy: { type: "text", label: "Recovery Policy", required: false }
             },
             FixedPay: {
                 Bacic: { type: "number", label: "Basic" },
                 Mobile: { type: "number", label: "Mobile" },
                 Travel: { type: "number", label: "Travel" },
                 HRA: { type: "number", label: "HRA" },
                 ServiceCharge: { type: "number", label: "Service Charge" },
                 StartDate: { type: "text", label: "Start Date" },
                 EndDate: { type: "text", label: "End Date" }
             },
         },
    };

}]);

csapp.factory("StakeholderEnums", function () {

    var gender = {
        Male: "Male",
        Female:"Female"
    };

    var products =
    {
        UNKNOWN: "UNKNOWN",
        SME_BIL: "SME_BIL",
        SME_ME: "SME_ME",
        SME_LAP: "SME_LAP",
        MORT: "MORT",
        AUTO: "AUTO",
        SME_LAP_OD: "SME_LAP_OD",
        PL: "PL",
        CC: "CC",
        AUTO_OD: "AUTO_OD",
        SMC: "SMC"
    };
});