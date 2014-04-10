
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

csapp.factory("$csFileUploadModels", ["$csShared", function ($csShared) {

    var fileDetail = function () {
        return {
            Frequency: {},
            FileCount: {}
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

    var stakeholder = function () {
        return {
            Name: { label: "Name", type: 'text', pattern: '/^[a-zA-Z ]{1,100}$/',required:true, patternMessage: 'Invalid Name' },
            userId : { label: "UserId",editable:false,template:'user' ,required: "true",type:"text",pattern:'/^[0-9]{7}$/' ,patternMessage: 'Invalid ID' },
            mobile : { label: "Mobile No",type:'text',pattern:'/^[0-9]{10}$/',template:'phone', patternMessage: 'Invalid Mobile Number' },
            Email : { label: "Email",  patternMessage: 'Invalid Email' },
            Date : { type:'date' },
            manager : {},

            //PAN : { label: 'PAN',patternMessage:'accepts only xxxxxxxx' },
            //TAN : { label: 'TAN',patternMessage:'accepts only xxxxxxxx' },
            //Registration : { label: 'Registration', patternMessage: 'special characters not allowed' },
            //ServiceTaxNo : { label: 'ServiceTaxNo', patternMessage: 'special characters not allowed' },

            //line1 : { label: "Line1", required: true },
            //line2 : { label: "Line2", required: true },
            //line3 : { label: "Line3" },
            //landline : { label: "Landline",patternMessage:"Invalid Number" }
        };     
    };

    var init = function () {
        return {
            Stakeholder: stakeholder()
        };
    };

    return {
      init:init()  
    };

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