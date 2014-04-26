//Datalayer
csapp.factory("filterConditionDatalayer", ["Restangular", "$csnotify", function (rest, $csnotify) {

    var restApi = rest.all("FilterConditionApi");
    var dldata = {};

    var errorDisplay = function (response) {
        $csnotify.error(response);
    };

    var getFileDetails = function () {
        return restApi.customGET("GetFiledetails", function (data) {
            return data;

        }, errorDisplay);
    };

    var getColumnValues = function (filedeatil) {
        return restApi.customGET("GetFileColumnData", { fileDetailId: filedeatil.Id }).then(function (data) {
            if (data.length === 0) {
                $csnotify.success("Data not available");
            }
            return data;
        });
    };

    var saveAliseCondition = function (filterCondition) {
        var obj = JSON.parse(filterCondition.FileDetail);
        filterCondition.FileDetail = obj;
        console.log(filterCondition);
        return  restApi.customPOST(filterCondition, "Post").then(function (data) {
            $csnotify.success("AliseCondition saved");
            return data;
        });
    };

    return {
        dldata: dldata,
        getFileDetails: getFileDetails,
        saveAliseCondition: saveAliseCondition,
        getColumnValues: getColumnValues
    };

}]);

//factory
csapp.factory("filterconditionFactory", ["filterConditionDatalayer", "$csnotify", function (datalayer, $csnotify) {
    var dldata = datalayer.dldata;

    return {

    };
}]);

//Controller
csapp.controller("filterConditionController", ["$scope", "filterConditionDatalayer", "filterconditionFactory", "$csFileUploadModels", "$csnotify","$csShared",
    function ($scope, datalayer, factory, $csFileUploadModels, $csnotify,$csShared) {

        $scope.fetchFileDetails = function () {
            datalayer.getFileDetails().then(function (data) {
                $scope.fileDetailsList = data;
            });
        };

        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.factory = factory;
            $scope.FilterCondition = $csFileUploadModels.models.FilterCondition;
            $scope.showDiv = false;
            $scope.fileDetailsList = [];
            $scope.filterCondition = { ConditionList: [] };
            $scope.AliseConditionList = [];
            $scope.fetchFileDetails();

        })();

        $scope.addAliseCondition = function () {
            $scope.showDiv = true;
        };

        $scope.addCondition = function (condition) {
            var obj = JSON.parse(condition.ColumnName);
            condition.ColumnName = obj.TempColumnName;
            var duplicateCond = _.find($scope.filterCondition.ConditionList, function (cond) {
                return (cond.ColumnName == condition.ColumnName && cond.Operator == condition.Operator && cond.Value == condition.Value);
            });

            if (duplicateCond) {
                $csnotify.error("condition is duplicate");
                return;
            }
            $scope.filterCondition.ConditionList.push(condition);
            $scope.reset();

        };

        $scope.deleteCondition = function (condition, index) {
            $scope.filterCondition.ConditionList.splice(index, 1);
            $scope.filterCondition.ConditionList[0].RelationType = "";

        };
        $scope.save = function (filterCondition) {
           datalayer.saveAliseCondition(filterCondition).then(function(data) {
               $scope.AliseConditionList = data;
           });
        };

        $scope.getColumnValues = function (fileData) {
            var filedetail = JSON.parse(fileData);
            datalayer.getColumnValues(filedetail).then(function (data) {
                $scope.ColumnNameList = data;
            });
        };

        $scope.manageOperatorField = function (condition) {
            var obj = JSON.parse(condition.ColumnName);
            console.log(obj);
            $scope.InputType = obj.ColumnDataType;
            if ($scope.InputType === "String") {
                condition.Operator = "";
                condition.Value = "";
                $scope.FilterCondition.Operator.valueList = $csShared.enums.TextConditionOperators;
                return;
            }
            condition.Operator = "";
            condition.Value = "";
            $scope.FilterCondition.Operator.valueList = $csShared.enums.ConditionOperators;

        };

        $scope.reset = function () {
            $scope.dldata.newCondition = {};
        };

    }]);