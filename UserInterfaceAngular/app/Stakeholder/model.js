csapp.factory("StakeholderModels", ["StakeholderEnums", function (enums) {

    var basicInfoModel = {
        Name: { type: "text", label: "Name",pattern:'/^[a-zA-Z ]*$/',patternMessage:"Invalid Name", required: true },
        UserId: { type: "userId", label: "User Id"},
        MobileNo: { type: "phone", label: "Mobile Number", required: true},
        
        Gender: { type: "enum", label: "Gender", required: true, value: enums.gender },
        DateOfBirth: { type: "text", label: "Date Of Birth", required: true, maxlength: 20 },
        Email:{type:"email",label:"Email",required:false},
        Date: { type: "date", template: 'future', required: true },
        ReportingManager:{type:"select"},
        
        TAN: {type:'text', label: 'TAN', maxlength: 50 },
        RegistrationNo: { type: 'text', label: 'Registration No', template: 'alphanum', maxlength: 30 },
        ServiceTax: { type: 'text', label: 'ServiceTax No', template: 'alphanum', maxlength: 30 },
        PAN: { type: 'text', label: 'PAN', pattern: "/^([A-Z]{5})([0-9]{4})([a-zA-Z]{1})$/", patternMessage: 'Accepts only AAAAA0000A format', required: false },
        
        Line1: { type: 'text', label: 'Line1', required: true },
        Line2: { type: 'text', label: 'Line2', required: true },
        Line3: { type: 'text', label: 'Line3', required: false },
        Pincode: { type: 'text', label: 'Pincode', required: true },
        LandlineNo: { type: "phone", label: "Landline Number", required: true },
       
    };

    var hierarchy = {
        Hierarchy: { type:"select", label: "Hierarchy", required: true },
        Designation: { type:"select", label: "Designation", required: true },
    };

    var workingPayment = {
        
         Working :{
             Products: { type: "enum", label: "Products", required: true, values: enums.products },
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

    return {
        BasicInfoModel: basicInfoModel,
        WorkingPayment: workingPayment,
        Hierarchy: hierarchy,
    };

}]);

csapp.factory("StakeholderEnums", function () {

    var products =
    [
        "UNKNOWN",
        "SME_BIL",
        "SME_ME",
        "SME_LAP",
        "MORT",
        "AUTO",
        "SME_LAP_OD",
        "PL",
        "CC",
        "AUTO_OD",
        "SMC"
    ];

    return {
        products:products
    };
});