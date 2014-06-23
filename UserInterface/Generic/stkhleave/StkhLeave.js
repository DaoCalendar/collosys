
csapp.factory("StakhLeaveDatalayer", ['Restangular', function (rest) {

    var restapi = rest.all("StkhLeaveApi");

    var getLeavesData = function () {
        return restapi.customGETLIST("GetLeavesHistory");
    };

    var save = function (stakeleave) {
        //var params = {
        //    FromDate: stakeleave.FromDate,
        //    ToDate: stakeleave.ToDate,
        //    DelegateTo: stakeleave.DelegatedTo.Id
        //};
        return restapi.customPOST(stakeleave, "SaveLeave2");
    };

    var getDeligatedTo = function () {
        return restapi.customGET("GetDeligateToStkh");
    };

    var cancelLeave = function (leave) {
        return restapi.customPOST(leave, "Deleteleave");
    };

    return {
        GetLeavesData: getLeavesData,
        Save: save,
        GetDeligatedTo: getDeligatedTo,
        CancelLeave: cancelLeave,
    };
}]);

csapp.controller("StkhLeaveManagementCtrl", ["$scope", "StakhLeaveDatalayer", "$csModels", "$csnotify", function ($scope, datalayer, $csModels, $csnotify) {

    var getLeavesData = function () {
        datalayer.GetLeavesData().then(function (data) {
            $scope.stkhleaveList = data;
        });
    };

    (function () {
        $scope.dldata = datalayer.dldata;
        $scope.StkhLeave = {};
        $scope.showAddButton = true;
        getLeavesData();
        $scope.stkhleavemodel = $csModels.getColumns("StkhLeave");
    })();


    $scope.save = function (stakeleave) {
        return datalayer.Save(stakeleave).then(function (data) {
            $scope.stkhleaveList.push(data);
            $scope.StkhLeave = {};
            $scope.showAddButton = true;
        });
    };

    $scope.cancelLeaveStatus = function (date) {
        var today = moment().utc().format("YYYY-MM-DD");
      return (moment(date.FromDate).isAfter(today));
    };

    $scope.endLeaveStatus = function (leave) {
        var today = moment().utc().format("YYYY-MM-DD");
        return (moment(leave.ToDate).isAfter(today)) && (moment(leave.FromDate).isBefore(today));
    };

    $scope.isDateValid = function (stakeleave) {
        if (stakeleave.FromDate >= stakeleave.ToDate) {
            stakeleave.ToDate = "";
            $csnotify.success("Leave starting day should be earlier than ending day!");
        }
    };

    $scope.leave = function () {
        $scope.showAddButton = false;
        return datalayer.GetDeligatedTo().then(function (data) {
            $scope.stkhleavemodel.DelegatedTo.valueList = data;
        });
    };

    $scope.cancelLeave = function (leave) {
        datalayer.CancelLeave(leave).then(function (data) {
            var index = $scope.stkhleaveList.indexOf(leave);
            $scope.stkhleaveList.splice(index, 1);
            $csnotify.success("Leave Canceled");
        });
    };

    $scope.endLeave = function (stakeleave) {
        stakeleave.ToDate = moment().utc().format("YYYY-MM-DD");
        return datalayer.Save(stakeleave).then(function (data) {
            $scope.stkhleaveList.splice(stakeleave);
            $scope.stkhleaveList.push(data);
        });
    };

    $scope.cancel = function () {
        $scope.showAddButton = true;
    };

}]);