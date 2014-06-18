
csapp.factory("StakeWorkingDatalayer", ["$csnotify", "Restangular", function ($csnotify, rest) {

    var restApi = rest.all('WorkingApi');

    var getReportsTo = function (stake) {
        return restApi.customGET('GetWorkingReportsTo', { 'id': stake.Hierarchy.Id, 'level': stake.Hierarchy.ReportingLevel })
            .then(function (data) {
                return data;
            });
    };

    var getWorkingData = function (stakeId) {
        return restApi.customGET('GetStakeWorkingData', { stakeholderId: stakeId })
            .then(function (data) {
                return data;
            });
    };

    //TODO: get fix pay components
    var getPaymentDetails = function () {
        return restApi.customGET('GetStakePaymentData')
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
        restApi.customPOST(paymentData, "SavePayment");
    };

    var saveWorking = function (workData) {
        return restApi.customPOST(workData, "SaveWorking").then(function () {
            $csnotify.success("Working Saved");
        });
    };

    return {
        GetStakeholder: getWorkingData,
        GetPaymentDetails: getPaymentDetails,
        GetReportsTo: getReportsTo,
        GetPincodeData: getPincodeData,
        GetGPincodeData: getGPincodeData,
        SavePayment: savePayment,
        SaveWorking: saveWorking,
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

    // ReSharper disable DuplicatingLocalDeclaration
    var getQueryFor = function (locLevel, displayMngr) {
        // ReSharper restore DuplicatingLocalDeclaration
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
                displayManager.showArea = true;
                return displayManager;
            default:
                throw "invalid location level";
        }

    };

    var getWorkingDetailsList = function (workingModel, locLevel, workingDetailsList) {
        var multiSelectList = angular.copy(workingModel.SelectedPincodeData[locLevel]);
        _.forEach(multiSelectList, function (location) {
            workingModel.SelectedPincodeData[locLevel] = location;
            workingDetailsList.push(angular.copy(workingModel.SelectedPincodeData));
        });
        return workingDetailsList;
    };

    var setWorkList = function (stakeholder, worklist, working) {
        _.forEach(worklist, function (workdata) {
            workdata.Stakeholder = stakeholder;
            workdata.StartDate = stakeholder.JoiningDate;
            workdata.Products = working.Products;
            workdata.ReportsTo = working.ReportsTo;
            workdata.LocationLevel = stakeholder.Hierarchy.LocationLevel;
        });

    };

    return {
        GetFixedPayObj: getFixedPayObj,
        GetQueryFor: getQueryFor,
        GetDisplayManager: getDisplayManager,
        GetWorkingDetailsList: getWorkingDetailsList,
        SetWorkList: setWorkList
    };
}]);

csapp.controller("StakeWorkingCntrl", ["$scope", "$routeParams", "StakeWorkingDatalayer", "$csModels", "StakeWorkingFactory", "$csfactory",
    function ($scope, $routeParams, datalayer, $csModels, factory, $csfactory) {

        (function () {
            datalayer.GetStakeholder($routeParams.stakeId).then(function (data) {
                data.Hierarchy.LocationLevelArray = JSON.parse(data.Hierarchy.LocationLevel);
                data.Hierarchy.LocationLevel = data.Hierarchy.LocationLevelArray[0];
                $scope.selectedHierarchy = data.Hierarchy;
                $scope.displayManager = factory.GetDisplayManager($scope.selectedHierarchy.LocationLevel);

                $scope.WorkingModel = {
                    SelectedPincodeData: {},
                    QueryFor: "",
                    MultiSelectValues: [],
                    DisplayManager: $scope.displayManager,
                    Buckets: []
                };

                $scope.currStakeholder = data;
            }).then(function () {
                //TODO: has fixed pay individual
                datalayer.GetPaymentDetails().then(function (data) {
                    $scope.FixedPay = factory.GetFixedPayObj(data);
                });
            });
            $scope.paymentModel = $csModels.getColumns("StkhPayment");
            $scope.workingModel = $csModels.getColumns("StkhWorking");
            $scope.Payment = {};
            $scope.Working = {};
            $scope.workingDetailsList = [];
        })();

        //TODO: move to factory
        $scope.TotalPayment = function (basic, hra, other) {
            $scope.SalDetails = {};

            if (angular.isUndefined(basic)) {
                basic = 0;
            }
            if (angular.isUndefined(hra)) {
                hra = 0;
            }
            if (angular.isUndefined(other)) {
                other = 0;
            }
            if (basic !== 0) {
                if ($scope.selectedHierarchy.HasFixedIndividual || true) {
                    $scope.SalDetails.pf = Number((basic) * ($scope.FixedPay.EmployeePF) / 100);
                    $scope.SalDetails.pf = Number($scope.SalDetails.pf.toFixed(2));

                    $scope.SalDetails.employerPf = Number((basic) * ($scope.FixedPay.EmployerPF) / 100);
                    $scope.SalDetails.employerPf = Number($scope.SalDetails.employerPf.toFixed(2));

                    $scope.SalDetails.totalNotEsic = Number((basic) + (hra) + (other));

                    $scope.SalDetails.esicEmployee = Number(($scope.SalDetails.totalNotEsic * ($scope.FixedPay.EmployeeESIC)) / 100);
                    $scope.SalDetails.esicEmployee = Number($scope.SalDetails.esicEmployee.toFixed(2));

                    $scope.SalDetails.esicEmployer = Number($scope.SalDetails.totalNotEsic * Number($scope.FixedPay.EmployerESIC) / 100);
                    $scope.SalDetails.esicEmployer = Number($scope.SalDetails.esicEmployer.toFixed(2));

                    $scope.SalDetails.ServiceCharge = $scope.FixedPay.ServiceCharge !== 0 ? Number((basic * $scope.FixedPay.ServiceCharge)) / 100 : 0;
                    $scope.SalDetails.ServiceCharge = Number($scope.SalDetails.ServiceCharge.toFixed(2));

                    $scope.SalDetails.serviceTax = Number(basic * ($scope.FixedPay.ServiceTax) / 100);
                    $scope.SalDetails.serviceTax = Number($scope.SalDetails.serviceTax.toFixed(2));

                    $scope.SalDetails.total =
                        $scope.SalDetails.pf * 2 +
                        $scope.SalDetails.totalNotEsic +
                        $scope.SalDetails.esicEmployee +
                        $scope.SalDetails.esicEmployer +
                        $scope.SalDetails.ServiceCharge +
                        $scope.SalDetails.serviceTax;
                    $scope.SalDetails.total = Number($scope.SalDetails.total.toFixed(2));
                } else {
                    $scope.SalDetails.total = basic;
                }
            }
            return 0;
        };

        $scope.getReportsTo = function (product) {
            datalayer.GetReportsTo($scope.currStakeholder).then(function (reportsToList) {
                $scope.reportsToList = reportsToList;
            });
        };

        $scope.savePayment = function (paymentData) {
            paymentData.Stakeholder = $scope.currStakeholder;
            datalayer.SavePayment(paymentData);
        };

        $scope.getPincodeData = function (workingModel, selected) {
            workingModel.QueryFor = $csfactory.isNullOrEmptyString(selected) ?
                factory.GetQueryFor($scope.selectedHierarchy.LocationLevel) : factory.GetQueryFor(selected, $scope.displayManager);
            if ($csfactory.isNullOrEmptyString(workingModel.QueryFor)) return;
            datalayer.GetPincodeData(workingModel).then(function (data) {
                $scope.WorkingModel = data;
            });
        };

        $scope.showDropdown = function (locLevel) {
            if ($csfactory.isEmptyObject($scope.selectedHierarchy)) return false;
            return $scope.selectedHierarchy.LocationLevel === locLevel;
        };

        $scope.addWorking = function (workingModel, locLevel) {
            $scope.workingDetailsList = factory.GetWorkingDetailsList(workingModel, locLevel, $scope.workingDetailsList);
            workingModel.SelectedPincodeData[locLevel] = [];
        };

        $scope.save = function (workList) {
            factory.SetWorkList($scope.currStakeholder, workList, $scope.Working);
            datalayer.SaveWorking(workList).then(function () {
                $scope.workingDetailsList = [];
            });
        };

    }]);