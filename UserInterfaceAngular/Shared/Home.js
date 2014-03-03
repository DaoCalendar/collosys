
(csapp.controller("datacontroller", ["$scope", "$csnotify", "$csfactory", "Restangular", function ($scope, $csnotify, $csfactory, rest) {
    "use strict";

    var restApi = rest.all("HomeApi");


    restApi.customGET("GetData").then(function (data) {
        $scope.datalist = data;
    }, function (data) {
        $csnotify.error(data.data.Message);
    });


    $scope.showPendingOptions = function () {
        if (angular.isUndefined($scope.datalist) || $scope.datalist.stakeholder != 0 || $scope.datalist.payment != 0 || $scope.datalist.working != 0 || $scope.datalist.allocation != 0 || $scope.datalist.allocationpolicy != 0)
            return true;
        else return false;
    };

    $scope.genderList = [{ name: "Male", value: "yes" }, { name: "Female", value: "no" }];
    $scope.gender = "yes";
    $scope.cities = [{ name: "Aurangabad", group: "Maharashtra" }, { name: "Pune", group: "Maharashtra" }, { name: "Nagpur", group: "Maharashtra" }, { name: "Junaghar", group: "Gujrat" }, { name: "Ahmdabad", group: "Gujrat" }, { name: "Surat", group: "Gujrat" }];
   
    $scope.fields = {
        show: false,
        //TEXT options:label, autofocus,  placeholder, required, readonly, minlength, maxlength, length
        //templates: alphanum, alphabates, numeric, phone, pan
        text: { required: true, template:"alphabates"},
        
        //EMAIL options:label, placeholder, pattern, minlength, maxlength, readonly, required
        emailid: { required: true },
        
        //NUMBER options: label, autofocus,  placeholder, required, readonly, minlength, maxlength, length, min, max
        //templates: positive, uint, int, ulong, long, decimal
        age: { required: true, template: "positive" },
        
        //DATE options:label, placeholder, required, readonly, endDate, startDate, minViewMode, daysOfWeekDisabled
        //templates: MonthPicker, YearPicker, future, past
        date: { required: true, template: "MonthPicker,future" },
        
        //CHECKBOX options:label, required
        checkbox: { label: "Married" },
        
        //SELECT options:label, required 
        selectbox: { required: true  },
        
        //RADIO options: { label: "Gender", required: true, textField: "text2", options: [{ text2: "Male", value: "yes" }, { text2: "Female", value: "no" }] };
        gender: { label: "Gender", required: true, textField: "name", options: $scope.genderList },
        
        btnSave: { btnType: "save" },
        btnReset: { btnType: "reset" },
        btnCancel: { btnType: "cancel" },
        btnOk:{btnType:"ok"}
    };
   
    $scope.show = function () {
        $csnotify.success("yeah it's working.....!!");
    };
}]));
