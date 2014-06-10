csapp.factory("$csLegalModels", function () {

    var requisitionPreparation = function () {
        return {
            Function: { label: 'Select options', type: 'radio', options: [{ value: 'Add', display: 'Add' }, { value: 'Edit', display: 'Edit' }, { value: 'Approve', display: 'Approve' }], textField: 'display', valueField: 'value' },
            City: { label: 'City', type: 'select' },
            Products: { label: 'Products', type: 'select' },
            LoanNo: { label: 'Loan No', type: 'text' },
            LoanStatus: { label: 'Loan Status', type: 'select' },
            CaseStatus: { label: 'Case Status', type: 'select' },
            Nameofhirer: { label: 'Name of hirer', type: 'text' },
            Addressasperaggrement: { label: 'Address as per aggrement', type: 'text' },
            Presentaddress: { label: 'Present Resedential address', type: 'text' },
            Statementofaccount: { label: 'Statement of account', type: 'text' },
            WithParty: { label: 'Current Vehicle Status', type: 'radio', options: [{ value: 'Plying', display: 'Plying' }, { value: 'Accidental', display: 'Accidental' }, { value: 'WithPolice', display: 'WithPolice' }, { value: 'OtherParty', display: 'OtherParty' }], textField: 'value', valueField: 'display' },
            Dateofrepossession: { label: 'Date of repossession', type: 'date'},
            Dateofsalenotice: { label: 'Date of sale notice', type: 'date', },
            DateofSale: { label: 'DateofSale', type: 'date', },
            Saleconsideration: { label: 'Sale consideration', type: 'text', },
            Soldto: { label: 'Name of party to whom vehicle sold', type: 'text', },
            AddressSoldto: { label: 'Address of party to whom vehicle sold', type: 'text', },
            CasePending: { label: 'Case Pending in Police-Station', type: 'radio', options: [{ value: 'true', display: 'Yes' }, { value: 'false', display: 'No' }], textField: 'display', valueField: 'value' },
            ReasonBranchIncharge: { label: 'Reason by branch incharge as to why he proposes', type: 'textarea'},
            AccusedName: { label: 'Name of the Accused or signatory of cheques', type: 'textarea'},
            EntityType: { label: 'EntityType', type: 'radio', options: [{ value: 'Individual', display: 'Individual' }, { value: 'Coorporate', display: 'Coorporate' }, { value: 'Partnership', display: 'Partnership' }, { value: 'Others', display: 'Others' }], textField: 'display', valueField: 'value' },
            FileNo: { label: 'Enter File No (of legal registration)', type: 'number',template:'int'},
         
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
            DateFrom: { label: "Loan Date From", type: "date" },
            DateTo: { label: "Loan Date To", type: "date" },
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