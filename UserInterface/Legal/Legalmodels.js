csapp.factory("$csLegalModels", function () {

    var requisitionPreparation = function () {
        return {
            Requis: { label: 'Select', type: 'radio', options: [{ value: 'Add', display: 'Add' }, { value: 'Edit', display: 'Edit' }, { value: 'Approve', display: 'Approve' }], textField: 'display', valueField: 'value' },
        };
    };

    //$scope.location = [
    //    { state: "Maharashtra", District: "Osmanabad" },
    //    { state: "Maharashtra", District: "Nagpur" },
    //    { state: "Maharashtra", District: "Latur" },
    //    { state: "Maharashtra", District: "Beed" },
    //    { state: "Gujrat", District: "Ahemadabad" },
    //    { state: "Kolkatta", District: "Magma Carta" },
    //    { state: "Karnatka", District: "Banglore" }
    //];
    $scope.requisitionIntiation = function () {
        return {
           Function: { label: 'Function', type: 'radio', options: [{ value: 'Intiate', display: 'Intiate' }, { value: 'Intiated', display: 'Intiated' }], textField: 'display', valueField: 'value' },
          Location: { label: "Location", type: "select", textField: "state", valueField: "District", required: "true", valueList: $scope.location },
          Division: { label: "Division", type: "select", textField: "District", valueField: "District", required: "true", valueList: $scope.location },
          LoanNo: { label: "Loan No", type: "text", required: true },
          DateFrom: { label: "Loan Date From", type: "text", required: true },
          DateTo: { label: "Loan Date To", type: "text", required: true },
          RequsitionNo: { label: "Requsition No", type: "text", required: true },
          RequsitionDateFrom: { label: "Requsition Date From", type: "text", required: true },
          RequsitionDateTo: { label: "Requsition Date To", type: "text", required: true },

        };
    };


    var init = function () {
        var models = {};

        models.RequisitionPreparation = {
            Table: "RequisitionPreparation",
            Columns: requisitionPreparation(),
        };

        models.RequsitionIntiation = {
            Table: "RequsitionIntiation",
            Columns: requisitionIntiation(),
        };
        return models;
    };



    return {
        init: init
    };
});