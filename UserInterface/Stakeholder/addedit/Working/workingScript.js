
csapp.factory("StakeWorkingDatalayer", ["$csnotify", "Restangular", function ($csnotify, rest) {

    var restApi = rest.all('WorkingApi');

    var getReportsTo = function (stake, product) {
        return restApi.customGET('GetWorkingReportsTo', {
            'id': stake.Hierarchy.Id,
            'level': stake.Hierarchy.ReportingLevel,
            'product': product
        }).then(function (data) {
            return data;
        });
    };

    var getStakeholder = function (stakeId) {
        return restApi.customGET('GetStakeholder', { stakeholderId: stakeId })
            .then(function (data) {
                return data;
            });
    };

    var getPincodeData = function (workingModel) {
        return restApi.customPOST(workingModel, 'GetPincodeData')
          .then(function (data) {
              return data;
          });
    };

    var getGPincodeData = function (workingModel) {
        return restApi.customPOST(workingModel, 'GetGPincodeData')
            .then(function (data) {
                return data;
            });
    };

    var savePayment = function (paymentData) {
        return restApi.customPOST(paymentData, "SavePayment").then(function (data) {
            $csnotify.success("Payment Saved");
            return data;
        });
    };

    var getSalaryDetails = function (payment) {
        return restApi.customPOST(payment, "GetSalaryDetails").then(function (sal) {
            return sal;
        });
    };

    var saveWorking = function (workData) {
        return restApi.customPOST(workData, "SaveWorking").then(function () {
            $csnotify.success("Working Saved");
        });
    };

    var deleteWorkingList = function (list) {
        return restApi.customPOST(list, "DeleteWorking").then(function (remainingWorkings) {
            return remainingWorkings;
        });
    };

    return {
        GetStakeholder: getStakeholder,
        GetReportsTo: getReportsTo,
        GetPincodeData: getPincodeData,
        GetGPincodeData: getGPincodeData,
        SavePayment: savePayment,
        SaveWorking: saveWorking,
        GetSalaryDetails: getSalaryDetails,
        DeleteWorkingList: deleteWorkingList
    };
}]);

csapp.factory("StakeWorkingFactory", ["$csfactory", function ($csfactory) {

    var displayManager = {
        showCountry: true,
        showRegion: false,
        showState: false,
        showCluster: false,
        showDistrict: false,
        showCity: false,
        showArea: false,
    };

    var getFixedPayObj = function (data) {
        var fixedPayObj = {};
        _.forEach(data, function (item) {
            fixedPayObj[item.ParamName] = parseFloat(item.Value);
        });
        return fixedPayObj;
    };

    var resetDisplayManager = function () {
        return displayManager = {
            showCountry: true,
            showRegion: false,
            showState: false,
            showCluster: false,
            showDistrict: false,
            showCity: false,
            showArea: false,
        };
    };

    var getQueryFor = function (locLevel, displayMngr) {
        return $csfactory.isEmptyObject(displayMngr) ? getInitialQueryFor(locLevel) : getNextQueryFor(locLevel, displayMngr);
    };

    var getInitialQueryFor = function (locLevel) {
        switch (locLevel.toUpperCase()) {
            case "COUNTRY":
                return "State";
            case "REGION":
                return "Region";
            case "STATE":
                return "State";
            case "CLUSTER":
                return "State";
            case "DISTRICT":
                return "State";
            case "CITY":
                return "State";
            case "AREA":
                return "State";
            default:
                throw "invalid location level";
        }
    };

    var getNextQueryFor = function (selected, displayMngr) {
        switch (selected.toUpperCase()) {
            case "COUNTRY":
                if (displayMngr.showRegion === true) return "Region";
            case "REGION":
                if (displayMngr.showCluster === true) return "Cluster";
            case "STATE":
                if (displayMngr.showCluster === true) return "Cluster";
            case "CLUSTER":
                if (displayMngr.showDistrict === true) return "District";
            case "DISTRICT":
                if (displayMngr.showCity === true) return "City";
            case "CITY":
                if (displayMngr.showArea === true) return "Area";
            case "AREA":
                return "";

            default:
                throw "invalid location level";
        }
    };

    var getDisplayManager = function (locLevel) {
        resetDisplayManager();
        switch (locLevel.toUpperCase()) {
            case "COUNTRY":
                return displayManager;
            case "REGION":
                displayManager.showRegion = true;
                return displayManager;
            case "STATE":
                displayManager.showState = true;
                return displayManager;
            case "CLUSTER":
                displayManager.showState = true;
                displayManager.showCluster = true;
                return displayManager;
            case "DISTRICT":
                displayManager.showState = true;
                displayManager.showDistrict = true;
                return displayManager;
            case "CITY":
                displayManager.showState = true;
                displayManager.showCity = true;
                return displayManager;
            case "AREA":
                displayManager.showState = true;
                displayManager.showCity = true;
                displayManager.showArea = true;
                return displayManager;
            default:
                throw "invalid location level";
        }
    };

    var getWorkingDetailsList = function (workingModel, locLevel, workingDetailsList) {
        if (locLevel === 'Country') {
            workingModel.SelectedPincodeData[locLevel] = [];
            workingModel.SelectedPincodeData[locLevel].push('India');
        }

        var multiSelectList = angular.copy(workingModel.SelectedPincodeData[locLevel]);
        _.forEach(multiSelectList, function (location) {
            workingModel.SelectedPincodeData.Status = 'Submitted';
            workingModel.SelectedPincodeData[locLevel] = location;
            workingDetailsList.push(angular.copy(workingModel.SelectedPincodeData));
        });
        return workingDetailsList;
    };

    var getReportsToName = function (id, list) {
        var reportsTo = _.find(list, { Id: id });
        return reportsTo.Name;
    };

    var setReportsToName = function (workList, reportsToList) {
        _.forEach(workList, function (item) {
            item.ReportsToName = getReportsToName(item.ReportsTo, reportsToList);
        });
    };

    var setWorkList = function (stakeholder, worklist, working) {
        stakeholder.StkhWorkings = [];
        stakeholder.StkhPayment = [];
        _.forEach(worklist, function (workdata) {
            workdata.Stakeholder = stakeholder;
            workdata.StartDate = stakeholder.JoiningDate;
            workdata.LocationLevel = stakeholder.Hierarchy.LocationLevel;
        });
    };

    var setProduct = function (obj, data) {
        obj.SelectedPincodeData.Products = data.Products;
        obj.SelectedPincodeData.ReportsTo = data.ReportsTo;
    };

    //TODO: move to $csfactory, if it is generic
    var safeSplice = function (list, row, property) {
        var indx = list.indexOf(row);
        if (indx !== -1) {
            list.splice(indx, 1);
            return;
        }

        if ($csfactory.isNullOrEmptyString(property)) {
            return;
        }

        var pluckedList = [];
        _.forEach(list, function (item) {
            pluckedList.push(item[property]);
        });

        var index = pluckedList.indexOf(row[property]);
        if (index !== -1) list.splice(index, 1);
    };

    return {
        GetFixedPayObj: getFixedPayObj,
        GetQueryFor: getQueryFor,
        GetDisplayManager: getDisplayManager,
        GetWorkingDetailsList: getWorkingDetailsList,
        SetProduct: setProduct,
        SetReportsToName: setReportsToName,
        SetWorkList: setWorkList,
        Splice: safeSplice
    };
}]);

csapp.controller("StakeWorkingCntrl", ["$scope", "$routeParams", "StakeWorkingDatalayer", "$csModels", "StakeWorkingFactory", "$csfactory", "$location", "$timeout",
    function ($scope, $routeParams, datalayer, $csModels, factory, $csfactory, $location, $timeout) {

        var setData = function (data) {
            data.Stakeholder.Hierarchy.LocationLevelArray = JSON.parse(data.Stakeholder.Hierarchy.LocationLevel);
            data.Stakeholder.Hierarchy.LocationLevel = data.Stakeholder.Hierarchy.LocationLevelArray[0];
            $scope.selectedHierarchy = data.Stakeholder.Hierarchy;
            $scope.displayManager = factory.GetDisplayManager($scope.selectedHierarchy.LocationLevel);
            $scope.currStakeholder = data.Stakeholder;
            factory.SetReportsToName(data.Stakeholder.StkhWorkings, data.ReportsToStakes);
            $scope.Payment = data.Stakeholder.StkhPayments.length === 0 ? {} : data.StkhPayments.StkhPayments[0];
            $scope.workingDetailsList = data.Stakeholder.StkhWorkings;
        };

        var getStakeholderData = function (stakeId) {
            datalayer.GetStakeholder(stakeId).then(function (data) {
                setData(data);
                $scope.formMode = 'add';
                $scope.paymentMode = 'add';
            });
        };

        var getStakeholderForEdit = function (stakeId) {
            datalayer.GetStakeholder(stakeId).then(function (data) {
                setData(data);
               
                $scope.formMode = 'view';
                $scope.paymentMode = 'view';
            });
        };

        (function () {
            $scope.WorkingModel = {
                SelectedPincodeData: {},
                QueryFor: "",
                MultiSelectValues: [],
                DisplayManager: $scope.displayManager,
                Buckets: []
            };
            $scope.showPayment = true;

            $scope.bucketList = ["1", "2", "3", "4", "5", "6"];

            //TODO: move this to a function & call that function from here
            $routeParams.editStakeId
                ? getStakeholderForEdit($routeParams.editStakeId)
                : getStakeholderData($routeParams.stakeId);

            $scope.paymentModel = $csModels.getColumns("StkhPayment");
            $scope.workingModel = $csModels.getColumns("StkhWorking");
            $scope.workingDetailsList = $csfactory.isNullOrEmptyArray($scope.workingDetailsList) ? [] : $scope.workingDetailsList;
            $scope.deleteWorkingList = [];
        })();

        $scope.showSaveButton = function () {
            if (angular.isUndefined($scope.workingDetailsList)) return false;
            return $scope.workingDetailsList.length == 0;
        };

        $scope.gotoView = function () {
            $location.path('/stakeholder/view');
        };

        $scope.changeMode = function (param) {
            switch (param) {
                case 'working':
                    $scope.formMode = 'edit';
                    break;
                case 'payment':
                    $scope.showPayment = false;
                    $scope.paymentMode = 'add';
                    $timeout(function () { $scope.showPayment = true; }, 100);
                    break;
                default:
                    throw "invalid param " + param;
            }

        };

        $scope.getSalaryDetails = function (payment) {
            $scope.SalDetails = {};
            $scope.SalDetails.FixpayBasic = angular.copy(payment.FixpayBasic);
            $scope.SalDetails.FixpayTotal = angular.copy(payment.FixpayTotal);
            datalayer.GetSalaryDetails($scope.SalDetails).then(function (sal) {
                $scope.SalDetails = sal;
            });
        };

        $scope.getReportsTo = function (product) {
            datalayer.GetReportsTo($scope.currStakeholder, product).then(function (reportsToList) {
                $scope.reportsToList = reportsToList;
                if ($scope.reportsToList.length === 1) $scope.workingModel.ReportsTo = $scope.reportsToList[0];
            });
        };

        $scope.savePayment = function (paymentData, form) {
            paymentData.Stakeholder = $scope.currStakeholder;
            paymentData.Stakeholder.StkhPayments = [];
            paymentData.Stakeholder.StkhWorkings = [];
            datalayer.SavePayment(paymentData).then(function (data) {
                $scope.Payment = data;
                $scope.gotoView();
            });
        };

        $scope.getPincodeData = function (workingModel, selected) {

            var temp = {};
            temp.Products = angular.copy(workingModel.SelectedPincodeData.Products);
            temp.ReportsTo = angular.copy(workingModel.SelectedPincodeData.ReportsTo);
            workingModel.DisplayManager = $scope.displayManager;

            workingModel.QueryFor = $csfactory.isNullOrEmptyString(selected) ?
                factory.GetQueryFor($scope.selectedHierarchy.LocationLevel) : factory.GetQueryFor(selected, $scope.displayManager);
            if ($csfactory.isNullOrEmptyString(workingModel.QueryFor))
                return;
            datalayer.GetPincodeData(workingModel).then(function (data) {
                $scope.WorkingModel = data;
                factory.SetProduct($scope.WorkingModel, temp);
            });
        };

        $scope.showDropdown = function (locLevel) {
            if ($csfactory.isEmptyObject($scope.selectedHierarchy)) return false;
            return $scope.selectedHierarchy.LocationLevel === locLevel;
        };

        $scope.addWorking = function (workingModel, locLevel) {
            $scope.workingDetailsList = factory.GetWorkingDetailsList(workingModel, locLevel, $scope.workingDetailsList);
            factory.SetReportsToName($scope.workingDetailsList, $scope.reportsToList);
            workingModel.SelectedPincodeData[locLevel] = [];
        };

        $scope.save = function (workList) {
            factory.SetWorkList($scope.currStakeholder, workList, $scope.Working);
            datalayer.SaveWorking(workList).then(function () {
                $scope.workingDetailsList = [];
                if (!$scope.selectedHierarchy.HasPayment) $scope.gotoView();
            });
        };

        $scope.addWorkingToDelete = function (data) {
            var selected = _.find($scope.deleteWorkingList, function (working) { return working === data; });
            if ($csfactory.isEmptyObject(selected)) {
                $scope.deleteWorkingList.push(data);
            } else {
                factory.Splice($scope.deleteWorkingList, data);
            };
        };

        $scope.workingChecked = function (data2) {
            return $scope.deleteWorkingList.indexOf(data2) !== -1;
        };

        $scope.assignEndDate = function (data3, endDate) {
            data3.EndDate = endDate;
        };

        //TODO: why seperate logic for splicing it when it can maintained as is
        $scope.deleteSelectedWorking = function () {
            datalayer.DeleteWorkingList($scope.deleteWorkingList).then(function (remainingWorking) {
                _.forEach(remainingWorking, function (workingToBeDeleted) {
                    factory.Splice($scope.workingDetailsList, workingToBeDeleted, $scope.selectedHierarchy.LocationLevel);
                });
            });
            $scope.deleteWorkingList = [];
        };

        $scope.getReportsToName = function (reportsToId) {
            if ($csfactory.isNullOrEmptyArray($scope.reportsToList)) return "";
            var data = _.find($scope.reportsToList, { 'Id': reportsToId });
            return data.Name;
        };
    }
]);
