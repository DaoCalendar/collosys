csapp.factory("StakeWorkingDatalayer", ["$csnotify", "Restangular", function ($csnotify, rest) {

    var restApi = rest.all('WorkingApi');

    var getReportsTo = function (stake, product) {
        return restApi.customGET('GetWorkingReportsTo', {
            'id': stake.Hierarchy.Id,
            'level': stake.Hierarchy.WorkingReportsLevel,
            'product': product
        });
    };

    var getStakeholder = function (stakeId) {
        return restApi.customGET('GetStakeholder', { stakeholderId: stakeId })
            .then(function (data) {
                return data;
            });
    };

    var getPincodeData = function (workingModel) {
        return restApi.customPOST(workingModel, 'GetPincodeData');
    };

    var getGPincodeData = function (workingModel) {
        return restApi.customPOST(workingModel, 'GetGPincodeData');
    };

    var savePayment = function (paymentData) {
        return restApi.customPOST(paymentData, "SavePayment").then(function (data) {
            $csnotify.success("Payment Saved");
            return data;
        });
    };

    var getSalaryDetails = function (paymentIds) {
        return restApi.customPOST(paymentIds, "GetSalaryDetails");
    };

    var saveWorking = function (workData) {
        return restApi.customPOST(workData, "SaveWorking").then(function (data) {
            $csnotify.success("Working Saved");
            return data;
        });
    };

    var deleteWorkingList = function (list) {
        return restApi.customPOST(list, "DeleteWorking");
    };

    var approveWorkings = function (list) {
        return restApi.customPOST(list, 'ApproveWorkingList');
    };

    var approvePayment = function (stakeObj) {
        return restApi.customPOST(stakeObj, 'ApprovePayment');
    };

    var rejectWorkings = function (list) {
        return restApi.customPOST(list, 'RejectWorkingList');
    };

    var rejectPayment = function (stakeObj) {
        return restApi.customPOST(stakeObj, 'RejectPayment');
    };

    return {
        GetStakeholder: getStakeholder,
        GetReportsTo: getReportsTo,
        GetPincodeData: getPincodeData,
        GetGPincodeData: getGPincodeData,
        SavePayment: savePayment,
        SaveWorking: saveWorking,
        GetSalaryDetails: getSalaryDetails,
        DeleteWorkingList: deleteWorkingList,
        ApproveWorkings: approveWorkings,
        ApprovePayment: approvePayment,
        RejectWorkings: rejectWorkings,
        RejectPayment: rejectPayment
    };
}]);

csapp.factory("StakeWorkingFactory", ["$csfactory", function ($csfactory) {

    //#region display manager

    //TODO: separate factory for display manager

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

    var getListName = function (locLevel) {
        switch (locLevel.toUpperCase()) {
            case "COUNTRY":
                return "ListOfCountries";
            case "REGION":
                return "ListOfRegions";
            case "STATE":
                return "ListOfStates";
            case "CLUSTER":
                return "ListOfClusters";
            case "DISTRICT":
                return "ListOfDistricts";
            case "CITY":
                return "ListOfCities";
            case "AREA":
                return "ListOfAreas";
            default:
                throw "invalid location level";
        }
    };

    //#endregion

    //#region working

    var getWorkingDetailsList = function (workingModel, locLevel, workingDetailsList) {
        if (locLevel === 'Country') {
            workingModel.SelectedPincodeData[locLevel] = [];
            workingModel.SelectedPincodeData[locLevel].push('India');
        }
        checkSelectAll(workingModel, locLevel);
        var multiSelectList = angular.copy(workingModel.SelectedPincodeData[locLevel]);
        _.forEach(multiSelectList, function (location) {
            workingModel.SelectedPincodeData[locLevel] = location;
            workingDetailsList.push(angular.copy(workingModel.SelectedPincodeData));
        });
        return workingDetailsList;
    };

    var checkSelectAll = function (workingModel, locLevel) {
        if (workingModel.SelectedPincodeData[locLevel].length === getLength(workingModel, locLevel)) {
            workingModel.SelectedPincodeData[locLevel] = [];
            workingModel.SelectedPincodeData[locLevel].push('ALL');
        }
    };

    var setProduct = function (obj, data) {
        obj.SelectedPincodeData.Products = data.Products;
        obj.SelectedPincodeData.ReportsTo = data.ReportsTo;
    };

    //TODO: change the name, get length of what??
    var getLength = function (workingModel, locLevel) {
        var listName = getListName(locLevel);
        if (angular.isUndefined(listName)) return 0;
        return angular.isUndefined(workingModel[listName]) ? 0 : workingModel[listName].length;
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

    var setEndDate = function (list, endDate) {
        _.forEach(list, function (working) {
            if (working.ApprovalStatus === 'Approved' || working.ApprovalStatus === 'Changed') {
                working.EndDate = endDate;
            }
        });
    };

    var setWorkList = function (stakeholder, worklist) {
        stakeholder.StkhWorkings = [];
        stakeholder.StkhPayment = [];
        _.forEach(worklist, function (workdata) {
            workdata.Stakeholder = stakeholder;
            workdata.StartDate = stakeholder.JoiningDate;
            //workdata.Buckets = JSON.stringify(workdata.Buckets);
            workdata.LocationLevel = stakeholder.Hierarchy.LocationLevel;
        });
    };

    var parseBuckets = function (workList) {
        _.forEach(workList, function (work) {
            if (!$csfactory.isNullOrEmptyString(work.Buckets))
                work.Buckets = JSON.parse(work.Buckets);
        });
    };

    var checkEndDate = function (endDate) {
        if ($csfactory.isNullOrEmptyString(endDate)) return true;
        return moment().isBefore(moment(endDate));
    };

    //#endregion

    //#region csfactory

    //TODO: move to $csfactory
    var round = function (value, exp) {
        if (typeof exp === 'undefined' || +exp === 0)
            return Math.round(value);

        value = +value;
        exp = +exp;

        if (isNaN(value) || !(exp % 1 === 0))
            return NaN;

        // Shift
        value = value.toString().split('e');
        value = Math.round(+(value[0] + 'e' + (value[1] ? (+value[1] + exp) : exp)));

        // Shift back
        value = value.toString().split('e');
        return +(value[0] + 'e' + (value[1] ? (+value[1] - exp) : -exp));
    };

    //TODO: move to $csfactory
    var safeSplice = function (list, row, properties) {
        var indx = list.indexOf(row);
        if (indx !== -1) {
            list.splice(indx, 1);
            return;
        }

        if ($csfactory.isNullOrEmptyString(properties)) {
            return;
        }

        var record = _.find(list, comparisons(row, properties));
        var index = list.indexOf(record);
        if (index !== -1) list.splice(index, 1);
    };
    var comparisons = function (row, properties) {
        var comparison = {};
        _.forEach(properties, function (property) {
            comparison[property] = row[property];
        });
        return comparison;
    };

    //#endregion

    //#region payment

    var computeSalary = function (basic, gross, salObj) {
        salObj.FixpayBasic = angular.isUndefined(basic) ? 0 : basic;
        salObj.FixpayGross = angular.isUndefined(gross) ? 0 : gross;

        salObj.EmployeePf = round(salObj.FixpayBasic * (salObj.EmployeePfPct / 100), 2);

        salObj.EmployerPf = round(salObj.FixpayBasic * (salObj.EmployerPfPct / 100), 2);

        salObj.EmployeeEsic = round(salObj.FixpayGross * (salObj.EmployeeEsicPct / 100), 2);

        salObj.EmployerEsic = round(salObj.FixpayGross * (salObj.EmployerEsicPct / 100), 2);

        var midTotal = round(salObj.FixpayGross + salObj.EmployerEsic + salObj.EmployerPf, 2);

        if (salObj.ReporteeCount > 100) salObj.ServiceChargePct = 7;
        else if (salObj.ReporteeCount > 50) salObj.ServiceChargePct = 8;
        else salObj.ServiceChargePct = 9;

        salObj.ServiceCharge = round(midTotal * (salObj.ServiceChargePct / 100), 2);

        salObj.ServiceTax = round((midTotal + salObj.ServiceCharge) * (salObj.ServiceTaxPct / 100), 2);

        salObj.FixpayTotal = round(midTotal + salObj.ServiceTax + salObj.ServiceCharge, 2);

        return salObj;
    };

    //#endregion

    return {
        GetFixedPayObj: getFixedPayObj,
        GetQueryFor: getQueryFor,
        ComputeSalary: computeSalary,
        ParseBuckets: parseBuckets,
        GetDisplayManager: getDisplayManager,
        GetWorkingDetailsList: getWorkingDetailsList,
        SetProduct: setProduct,
        SetEndDate: setEndDate,
        CheckEndDate: checkEndDate,
        SetReportsToName: setReportsToName,
        SetWorkList: setWorkList,
        Splice: safeSplice
    };
}]);

csapp.controller("StakeWorkingCntrl", ["$scope", "$routeParams", "StakeWorkingDatalayer", "$csModels", "StakeWorkingFactory", "$csfactory", "$location", "$timeout", "$csnotify",
    function ($scope, $routeParams, datalayer, $csModels, factory, $csfactory, $location, $timeout, $csnotify) {

        //#region init

        $scope.gotoView = function () {
            $location.path('/stakeholder/view');
        };

        var getPaymentData = function (hierarchy) {
            if (hierarchy.HasFixedIndividual) {
                $scope.getSalaryDetails();
            }
        };

        var setData = function (data) {
            data.Stakeholder.Hierarchy.LocationLevelArray = JSON.parse(data.Stakeholder.Hierarchy.LocationLevel);
            data.Stakeholder.Hierarchy.LocationLevel = data.Stakeholder.Hierarchy.LocationLevelArray[0];
            $scope.selectedHierarchy = data.Stakeholder.Hierarchy;
            $scope.displayManager = factory.GetDisplayManager($scope.selectedHierarchy.LocationLevel);
            $scope.currStakeholder = data.Stakeholder;
            factory.SetReportsToName(data.Stakeholder.StkhWorkings, data.ReportsToStakes);
            $scope.Payment = data.Stakeholder.StkhPayments.length === 0 ? {} : data.Stakeholder.StkhPayments[0];
            getPaymentData($scope.selectedHierarchy);
            $scope.workingDetailsList = data.Stakeholder.StkhWorkings;
            if ($scope.selectedHierarchy.HasBuckets)
                factory.ParseBuckets($scope.workingDetailsList);
            return data;
        };

        var getStakeholderData = function (stakeId) {
            return datalayer.GetStakeholder(stakeId).then(function (data) {
                setData(data);
                if (angular.isDefined($routeParams.editStakeId)) {
                    $scope.formMode = 'view';
                    $scope.paymentMode = 'view';
                } else {
                    $scope.formMode = 'add';
                    $scope.paymentMode = 'add';
                }
                return data;
            });
        };

        var getStakeData = function () {
            return angular.isDefined($routeParams.editStakeId)
                ? getStakeholderData($routeParams.editStakeId)
              : getStakeholderData($routeParams.stakeId);
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
            $scope.bucketList = ["1", "2", "3", " 4", "5", "6"];
            getStakeData();
            $scope.paymentModel = $csModels.getColumns("StkhPayment");
            $scope.workingModel = $csModels.getColumns("StkhWorking");
            $scope.reporteeCount = { label: "Reportee Count", type: "number" };
            $scope.workingDetailsList = $csfactory.isNullOrEmptyArray($scope.workingDetailsList) ? [] : $scope.workingDetailsList;
            $scope.selectedWorkingList = [];
        })();

        //#endregion

        //#region api calls
        $scope.getSalaryDetails = function () {
            var paymentIds = {
                ReportingId: $scope.currStakeholder.ReportingManager,
                PaymentId: $scope.Payment.Id
            };
            datalayer.GetSalaryDetails(paymentIds).then(function (sal) {
                $scope.SalDetails = $csfactory.isEmptyObject($scope.Payment) ? sal
                    : factory.ComputeSalary($scope.Payment.FixpayBasic, $scope.FixpayGross, sal);

                console.log("salary details: ", $scope.SalDetails);
            });
        };

        $scope.getReportsTo = function (product) {
            datalayer.GetReportsTo($scope.currStakeholder, product).then(function (reportsToList) {
                $scope.reportsToList = reportsToList;
                autoSelect($scope.reportsToList);
            });
        };

        //TODO: rename save working
        $scope.save = function (workList) {
            factory.SetWorkList($scope.currStakeholder, workList);
            return datalayer.SaveWorking(workList).then(function (data) {
                $scope.workingDetailsList = data.WorkList;
                if (!$scope.selectedHierarchy.HasPayment) {
                    $scope.gotoView();
                } else {
                    if ($scope.selectedHierarchy.HasBuckets)
                        factory.ParseBuckets($scope.workingDetailsList);
                    factory.SetReportsToName($scope.workingDetailsList, data.ReportsToList);
                }
                return data;
            });
        };

        //#endregion

        //#region working
        //TODO:move to working factory
        var autoSelect = function (reportsToList) {
            if (reportsToList.length === 1) {
                $scope.WorkingModel.SelectedPincodeData.ReportsTo = reportsToList[0].Id;
                $scope.getPincodeData($scope.WorkingModel);
            }
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
            clearArray(workingModel, $scope.selectedHierarchy.LocationLevel);
            datalayer.GetPincodeData(workingModel).then(function (data) {
                $scope.WorkingModel = data;
                factory.SetProduct($scope.WorkingModel, temp);
            });
        };

        //TODO: rename, move it to display manager model factory
        var clearArray = function (workingModel, locLevel) {
            if (workingModel.QueryFor !== locLevel) {
                workingModel.SelectedPincodeData[locLevel] = [];
                if ($scope.selectedHierarchy.HasBuckets) {
                    if (angular.isDefined(workingModel.SelectedPincodeData.Buckets)) {
                        workingModel.SelectedPincodeData.Buckets = [];
                    }
                }
            }
        };

        $scope.addWorking = function (workingModel, locLevel) {
            $scope.workingDetailsList = factory.GetWorkingDetailsList(workingModel, locLevel, $scope.workingDetailsList);
            factory.SetReportsToName($scope.workingDetailsList, $scope.reportsToList);
            workingModel.SelectedPincodeData[locLevel] = [];
            workingModel.SelectedPincodeData.Buckets = [];
        };

        $scope.addWorkingToList = function (data) {
            var selected = _.find($scope.selectedWorkingList, function (working) { return working === data; });
            if ($csfactory.isEmptyObject(selected)) {
                $scope.selectedWorkingList.push(data);
            } else {
                factory.Splice($scope.selectedWorkingList, data);
            }
        };

        $scope.deleteSelectedWorking = function (endDate) {
            var remainingData = angular.copy(deleteData());
            factory.SetEndDate($scope.selectedWorkingList, endDate);
            factory.SetWorkList($scope.currStakeholder, $scope.selectedWorkingList);
            datalayer.DeleteWorkingList($scope.selectedWorkingList).then(function () {
                getStakeData().then(function (data) { addUnsavedDataToList(data.Stakeholder, remainingData); });
            });
            $scope.selectedWorkingList = [];
        };

        var deleteData = function () {
            _.forEach($scope.selectedWorkingList, function (workingToBeDeleted) {
                if ($csfactory.isNullOrEmptyGuid(workingToBeDeleted.Id)) {
                    factory.Splice($scope.workingDetailsList, workingToBeDeleted, ["ReportsTo", "Products", "EndDate", $scope.selectedHierarchy.LocationLevel]);
                }
            });

            _.forEach(angular.copy($scope.workingDetailsList), function (work) {
                if (!$csfactory.isNullOrEmptyGuid(work.Id)) {
                    factory.Splice($scope.workingDetailsList, work, ["ReportsTo", "Products", "EndDate", $scope.selectedHierarchy.LocationLevel]);
                }
            });

            return $scope.workingDetailsList;
        };

        var addUnsavedDataToList = function (stakedata, unsavedList) {
            _.forEach(unsavedList, function (unsavedData) {
                stakedata.StkhWorkings.push(unsavedData);
            });
            $scope.workingDetailsList = stakedata.StkhWorkings;
        };

        $scope.getEndDate = function (data) {
            if (data.ApprovalStatus === 'Changed') {
                if ($csfactory.isNullOrEmptyString(data.EndDate)) return undefined;
                else {
                    return 'End Date: ' + moment(data.EndDate).format('YYYY-MM-DD');
                }
            }
            return undefined;
        };

        $scope.disableDeleteBtn = function (deleteList, endDate) {
            if ($scope.showEndDate(deleteList)) {
                return $csfactory.isNullOrEmptyString(endDate);
            } else {
                return false;
            }
        };

        $scope.showEndDate = function (deleteList) {
            var showEndDt = false;
            _.forEach(deleteList, function (working) {
                if (working.ApprovalStatus === 'Approved')
                    showEndDt = true;
            });
            return showEndDt;
        };

        $scope.disableDeleteBtn = function (deleteList, endDate) {
            if ($scope.showEndDate(deleteList)) {
                return $csfactory.isNullOrEmptyString(endDate);
            } else {
                return false;
            }
        };

        $scope.showEndDate = function (deleteList) {
            var showEndDt = false;
            _.forEach(deleteList, function (working) {
                if (working.ApprovalStatus === 'Approved')
                    showEndDt = true;
            });
            return showEndDt;
        };

        $scope.showApproveButtons = function (workList) {
            var showApproveBtn = false;
            _.forEach(workList, function (work) {
                if (work.ApprovalStatus == 'Submitted') {
                    showApproveBtn = true;
                }
            });

            return showApproveBtn;
        };

        //#endregion

        //#region payment
        //TODO: move to payment factory

        $scope.computeSalary = function (basic, gross) {
            $scope.SalDetails = factory.ComputeSalary(basic, gross, $scope.SalDetails);

        };

        $scope.savePayment = function (paymentData) {
            paymentData.Stakeholder = $scope.currStakeholder;
            paymentData.Stakeholder.StkhPayments = [];
            paymentData.Stakeholder.StkhWorkings = [];
            paymentData.ApprovalStatus = factory.GetApprovalStatus(paymentData.ApprovalStatus);
            datalayer.SavePayment(paymentData).then(function (data) {
                $scope.Payment = data;
                $scope.gotoView();
            });
        };

        //TODO: separate into working payment part-done

        $scope.approveWorkings = function (workList) {
            factory.SetWorkList($scope.currStakeholder, workList);
            datalayer.ApproveWorkings(workList).then(function (data) {
                postWorkingApproval(data);
                $csnotify.success("Workings Approved");
            });
        };
        $scope.rejectWorking = function (worklist) {
            factory.SetWorkList($scope.currStakeholder, worklist);
            datalayer.RejectWorkings(worklist).then(function (data) {
                postWorkingApproval(data);
                $csnotify.success("Workings Rejected");
            });
        };
        var postWorkingApproval = function (data) {
            $scope.workingDetailsList = data.WorkList;
            factory.SetReportsToName($scope.workingDetailsList, data.ReportsToList);
            $scope.selectedWorkingList = [];
            return $scope.workingDetailsList;
        };


        $scope.approvePayment = function (id) {
            var stakeObj = {
                Id: id,
            };
            return datalayer.ApprovePayment(stakeObj).then(function (data) {
                postApprovalPayment(data);
            });
        };
        $scope.RejectPayment = function (id) {
            var stakeObj = {
                Id: id,
            };
            return datalayer.RejectPayment(stakeObj).then(function (data) {
                return postApprovalPayment(data);
            });
        };
        var postApprovalPayment = function (data) {
            $scope.Payment = data;
            $csnotify.success("Payment Approved");
            $scope.gotoView();
            return $scope.Payment;
        };

        //$scope.setApprovalStatus = function (param1, status, param) {

        //    switch (status) {
        //        case 'approve':
        //            switch (param) {
        //                case 'working':
        //                    return approveWorking(param1, param);

        //                case 'payment':
        //                    return datalayer.ApprovePayment(stakeObj).then(function (data) {
        //                        return postApproval(data, param);
        //                    });
        //            }
        //        case 'reject':
        //            switch (param) {
        //                case 'working':
        //                    return rejectWorking(param1, param);

        //                case 'payment':
        //                    return datalayer.RejectPayment(stakeObj).then(function (data) {
        //                        return postApproval(data, param);
        //                    });
        //            }

        //        default:
        //            throw "invalid approval status";
        //    }

        //};

        //TODO: separate into working payment part
        //var postApproval = function (data, param) {
        //    switch (param) {
        //        case 'working':

        //        case 'payment':

        //        default:
        //            throw "invalid param " + param;
        //    }
        //};

        //TODO: separate into working payment part
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

        $scope.showPaymentApproval = function () {
            return $scope.Payment.ApprovalStatus === 'Submitted';
        };

        //TODO: rename
        $scope.showDropdown = function (locLevel) {
            if ($csfactory.isEmptyObject($scope.selectedHierarchy)) return false;
            return $scope.selectedHierarchy.LocationLevel === locLevel;
        };
        //#endregion
    }
]);
