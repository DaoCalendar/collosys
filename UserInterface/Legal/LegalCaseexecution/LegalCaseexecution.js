csapp.controller("LegalCaseexecutionCtrl", ["$scope","$csModels","LegalCaseexecutionDataLayer",
    function ($scope,$csModels,datalayer) {
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.legal = {};
            $scope.legalprepare = $csModels.getColumns("LegalCaseexecution");
            $scope.dldata.Requisitiondata = [];
        })();

        $scope.save = function (legal) {
            legal.RequsitionNo = 1231;
            legal.LoanCaseStatus = 'Pending';
            legal.Withdrawndate = moment().format('L');
            legal.Settlementamount = 12345;
            legal.Finalclosedate = moment().add('months', +1).format('L');
            return datalayer.save(legal);
        };

    }]);

csapp.factory("LegalCaseexecutionDataLayer", ["$csnotify", function ($csnotify) {

    var dldata = {};
    var errorDisplay = function (response) {
        $csnotify.error(response);
    };

    //dldata.function = ['Initiate', 'Initiated'];
    dldata.location = ['Mumbai', 'Pune', 'Kolkata', 'Bangalore', 'Hyderabad', 'Jaipur'];
    dldata.division = ['Mumbai', 'Pune', 'Kolkata', 'Bangalore', 'Hyderabad', 'Jaipur'];
    dldata.loanstatus = ['Disbursed', 'Do/Sanction LetterMade', 'FCI Allocated', 'FCI Despatched', 'FCI Notrequired', 'Received', 'In-Principally Sanctioned'];
    dldata.legalstatus = ["Closed", "Unclosed"];
    var save = function (legal) {
        dldata.Requisitiondata.push(legal);
    };


    return {
        dldata: dldata,
        save: save,

    };
}]);