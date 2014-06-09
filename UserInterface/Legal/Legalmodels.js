csapp.factory("$csLegalModels", function () {

    var requisitionPreparation = function () {
        return {
            Function: { label: 'Select options', type: 'radio', options: [{ value: 'Add', display: 'Add' }, { value: 'Edit', display: 'Edit' }, { value: 'Approve', display: 'Approve' }], textField: 'display', valueField: 'value' },
            City: { label: 'City', type: 'select' },
            Products: { label: 'Products', type: 'select' },
            LoanNo: { label: 'Loan No', type: 'text' },
            LoanStatus: { label: 'Loan Status', type: 'select' },
            CaseStatus: { label: 'Case Status', type: 'select' },
        };
    };
   
    var requisitionIntiation = function () {
        return {
            Function: { label: 'Function', type: 'radio', options: [{ value: 'Initiate', display: 'Initiate' }, { value: 'Initiated', display: 'Initiated' }], textField: 'display', valueField: 'value' },
            Location: { label: "Location", type: "select", },
            Division: { label: "Division", type: "select", },
            LoanNo: { label: "Loan No", type: "text" },
            DateFrom: { label: "Loan Date From", type: "date" },
            DateTo: { label: "Loan Date To", type: "date" },
            RequsitionNo: { label: "Requsition No", type: "text" },
            RequsitionDateFrom: { label: "Requsition Date From", type: "date" },
            RequsitionDateTo: { label: "Requsition Date To", type: "date" },

        };
    };
    
    var legalCaseexecution = function () {
        return {
            Function: { label: 'Function', type: 'radio', options: [{ value: 'ReadyForWithDraw', display: 'Ready For WithDraw' }, { value: 'Withdrawn/Closed', display: 'Withdrawn/Closed' }], textField: 'display', valueField: 'value' },
            Location: { label: "Location", type: "select", },
            Division: { label: "Division", type: "select", },
            LoanNo: { label: "Loan No", type: "text" },
            PartyName: { label: "Party Name", type: "text" },
            AdvocateName: { label: "Advocate Name", type: "text" },
            LoanStatus: { label: "Loan Status", type: "select" },
            LoanCloseStatus: { label: "Loan Close Status", type: "select" },
        };
    };
    
    var followup = function () {
        return {
            Function: { label: 'Function', type: 'radio', options: [{ value: 'Followup', display: 'Followup' }, { value: 'ReadyForWithDraw', display: 'ReadyForWithDraw' },{ value: 'All', display: 'All' }], textField: 'display', valueField: 'value' },
            Location: { label: "Location", type: "select", },
            Division: { label: "Division", type: "select", },
            LoanNo: { label: "Loan No", type: "text" },
            DateTo: { label: "Loan Date To", type: "date" },
            DateFrom: { label: "Loan Date From", type: "date" },
            RequsitionNo: { label: "Requsition No", type: "text" },
            RequsitionDateFrom: { label: "Requsition Date From", type: "date" },
            RequsitionDateTo: { label: "Requsition Date To", type: "date" },
            AdvocateName: { label: "Advocate Name", type: "text" },
            AdvocateCode: { label: "Advocate Code", type: "text" },
            DateofAppointment: { label: "Date of Appointment", type: "date" },
            Dateoftermination: { label: "Date of termination", type: "date" },
            EmpCode: { label: "Employee Code", type: "text" },
            EmpName: { label: "Employee Name", type: "text" },
          
        };
    };

    var init = function () {
        var models = {};

        models.RequisitionPreparation = {
            Table: "RequisitionPreparation",
            Columns: requisitionPreparation(),
        };

        models.RequisitionIntiation = {
            Table: "RequisitionIntiation",
            Columns: requisitionIntiation(),
        };
        
        models.LegalCaseexecution = {
            Table: "LegalCaseexecution",
            Columns: legalCaseexecution(),
        };
        
        models.FollowUp = {
            Table: "FollowUp",
            Columns: followup(),
        };

        return models;
    };

    return {
        init: init
    };
});