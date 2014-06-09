csapp.controller("RequisitionCtrl", ["$scope", "$csModels", "RequisitionDataLayer",
    function ($scope, $csModels, datalayer) {
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.legal = {};
            $scope.legalprepare = $csModels.getColumns("RequisitionPreparation");
        })();

        $scope.Clear = function (legal) {
            return datalayer.clear(legal);
        };
        
        $scope.save = function (legal) {
            return datalayer.save(legal);
        };

    }]);

csapp.factory("RequisitionDataLayer", ["$csnotify", function ($csnotify) {

    var dldata = {};
    //var errorDisplay = function (response) {
    //    $csnotify.error(response);
    //};
    
    dldata.Products = ['AUTO', 'SMC', 'SME-BIL', 'MORT'];
    console.log(dldata.Products);
    dldata.City = ['Mumbai', 'Pune', 'Kolkata', 'Bangalore', 'Hyderabad', 'Jaipur'];
    dldata.loanstatus = ['Disbursed', 'Do/Sanction LetterMade', 'FCI Allocated', 'FCI Despatched', 'FCI Notrequired', 'Received','In-Principally Sanctioned'];
    dldata.casestatus = ['Cancelled', 'Closed', 'Disposed', 'Expired', 'Legal', 'Live', 'Released','Repossessed'];

    var save = function() {
        datalayer.save();
    };

    var clear = function(legal) {
        legal.City = '';
        legal.Products = '';
        legal.LoanNo = '';
        legal.LoanStatus = '';
        legal.CaseStatus = '';
    };


    return {
        dldata: dldata,
        clear: clear,
        save: save,
        //reset: resetdata
    };
}]);
