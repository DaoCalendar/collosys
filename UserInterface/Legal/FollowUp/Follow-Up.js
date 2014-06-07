csapp.controller("FollowUpCtrl", ["$scope","$csModels","FollowUpCtrlDataLayer",
    function ($scope,$csModels,datalayer) {
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.legal = {};
            $scope.legalprepare = $csModels.getColumns("FollowUp");
        })();

    }]);


csapp.factory("FollowUpCtrlDataLayer", ["$csnotify", function ($csnotify) {

    var dldata = {};
    var errorDisplay = function (response) {
        $csnotify.error(response);
    };
    
    dldata.location = ['Mumbai', 'Pune', 'Kolkata', 'Bangalore', 'Hyderabad', 'Jaipur'];
    dldata.division = ['Mumbai', 'Pune', 'Kolkata', 'Bangalore', 'Hyderabad', 'Jaipur'];

    //var save = function (legal) {
    //    dldata.Requisitiondata.push(legal);
    //};


    return {
        dldata: dldata,
        //save: save,

    };
}]);