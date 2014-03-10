csapp.factory("StakeholderModels", ["StakeholderEnums", function (enums) {

    var basicInfoModel = {
        Name: { type: "text", label: "Name",pattern:'/^[a-zA-Z ]*$/',patternMessage:"Invalid Name", required: true },
        UserId: { type: "userId", label: "User Id"},
        MobileNo: { type: "phone", label: "Mobile Number", required: true,pattern:'/^[0-9]{8,10}$/',patternMessage:"invalid number"},
        Gender: { type: "enum", label: "Gender", required: true, value: enums.gender },
        DateOfBirth: { type: "text", label: "Date Of Birth", required: true, maxlength: 20 },
        Email:{label:"Email",required:true},
        Date: { template: 'future', required: true },
        TAN: { label: 'TAN', maxlength: 50 },
        RegistrationNo: { label: 'Registration No', template: 'alphanum', maxlength: 30 },
        ServiceTax: { label: 'ServiceTax No', template: 'alphanum', maxlength: 30 },
        Line1: { label: 'Line11' ,required:true},
        Line2: { label: 'Line21' ,required:true},
        Line3: { label: 'Line31' ,required:false},
        PAN: { label: 'PAN', pattern: "/^([A-Z]{5})([0-9]{4})([a-zA-Z]{1})$/", patternMessage: 'Accepts only AAAAA0000A format' ,required:false}
    };

    var hierarchy = {
        Hierarchy: {  label: "Hierarchy", required: true },
        Designation: {  label: "Designation", required: true },
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

    return {
        BasicInfoModel: basicInfoModel,
        WorkingPayment: workingPayment,
        Hierarchy:hierarchy
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

    return {
        gender: gender,
        products:products
    };
});