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
          Location: { label: "Location", type: "select", textField: "state", valueField: "row"},
          Division: { label: "Division", type: "select", textField: "District", valueField: "row"},
          LoanNo: { label: "Loan No", type: "text"},
          DateFrom: { label: "Loan Date From", type: "text"},
          DateTo: { label: "Loan Date To", type: "text"},
          RequsitionNo: { label: "Requsition No", type: "text"},
          RequsitionDateFrom: { label: "Requsition Date From", type: "text"},
          RequsitionDateTo: { label: "Requsition Date To", type: "text" },

        };
    };

   var legalCaseexecution = function() {
        return {
            withDraw: { type: 'radio' },
            Location: { label: "Location", type: "select", textField: "state", valueField: "row" },
            Division: { label: "Division", type: "select", textField: "District", valueField: "row" },
            LoanNo: { label: "Loan No", type: "text" },
            PartyName: { label: "Party Name", type: "text" },
            AdvocateName: { label: "Advocate Name", type: "text" },
            LoanStatus: { label: "Loan Status", type: "select", textField: "Name", valueField: "row" },
            LoanClosestatus: { label: "Loan close status", type: "select", textField: "Name", valueField: "row" },
        };
    };

    var followUp = function() {
        return {
            withDraw: { type: 'radio' },
            Location: { label: "Location", type: "select", textField: "state", valueField: "row" },
            Division: { label: "Division", type: "select", textField: "District", valueField: "row" },
            LoanNo: { label: "Loan No", type: "text" },
            DateFrom: { label: "Loan Date From", type: "text" },
            DateTo: { label: "Loan Date To", type: "text" },
            RequsitionNo: { label: "Requsition No", type: "text" },
            RequsitionDateFrom: { label: "Requsition Date From", type: "text" },
            RequsitionDateTo: { label: "Requsition Date To", type: "text" },
            AdvocateName: { label: "Advocate Name", type: "text" }
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


        models.LegalCaseexecution = {
            Table: "LegalCaseexecution",
            Columns: legalCaseexecution(),
        };
        models.FollowUp = {
            Table: "FollowUp",
            Columns: followUp(),
        };
       
        return models;
    };



    return {
        init: init
    };
});