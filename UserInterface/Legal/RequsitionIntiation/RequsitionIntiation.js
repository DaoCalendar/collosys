csapp.controller("RequsitionIntiationCtrl", ["$scope","$csModels","RequisitionDataLayer",
    function ($scope, $csModels,datalayer) {
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.legal = {};
            $scope.legalprepare = $csModels.getColumns("RequsitionIntiation");
            $scope.dldata.Requisitiondata = [];
        })();
        
        $scope.save = function (legal) {
            return datalayer.save(legal);
        };
    }]);


csapp.factory("RequisitionDataLayer", ["$csnotify", function ($csnotify) {

    var dldata = {};
    var errorDisplay = function (response) {
        $csnotify.error(response);
    };

    //dldata.function = ['Initiate', 'Initiated'];
    dldata.location = ['Mumbai', 'Pune', 'Kolkata', 'Bangalore', 'Hyderabad', 'Jaipur'];
    dldata.division = ['Mumbai', 'Pune', 'Kolkata', 'Bangalore', 'Hyderabad', 'Jaipur'];

    var save = function(legal) {
        dldata.Requisitiondata.push(legal);
        return;
    };


    return {
        dldata: dldata,
        save: save,
       
    };
}]);


