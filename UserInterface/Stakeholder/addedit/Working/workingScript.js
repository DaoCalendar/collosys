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

    var approveWorkings = function (stakeObj) {
        return restApi.customPOST(stakeObj, 'ApproveWorking');
    };

    var approvePayment = function (stakeObj) {
        return restApi.customPOST(stakeObj, 'ApprovePayment');
    };

    var rejectWorkings = function (stakeObj) {
        return restApi.customPOST(stakeObj, 'RejectWorking').then(function (data) {
            $csnotify.success('Workings Rejected');
            return data;
        });
    };

    var rejectPayment = function (stakeObj) {
        return restApi.customPOST(stakeObj, 'RejectWorking').then(function (data) {
            $csnotify.success('Payment Rejected');
            return data;
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
        DeleteWorkingList: deleteWorkingList,
        ApproveWorkings: approveWorkings,
        ApprovePayment: approvePayment,
        RejectWorkings: rejectWorkings,
        RejectPayment: rejectPayment
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

    var getApprovalStatus = function (status) {
        if (status === 'Approved' || status === 'Changed') return 'Changed';
        else {
            return 'Submitted';
        }
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
    var getLength = function (workingModel, locLevel) {
        var listName = getListName(locLevel);
        if (angular.isUndefined(listName)) return 0;
        return angular.isUndefined(workingModel[listName]) ? 0 : workingModel[listName].length;
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
            if (working.Status === 'Approved' || working.Status === 'Changed') {
                working.EndDate = endDate;
            }
        });
    };

    var setWorkList = function (stakeholder, worklist, working) {
        stakeholder.StkhWorkings = [];
        stakeholder.StkhPayment = [];
        _.forEach(worklist, function (workdata) {
            workdata.Stakeholder = stakeholder;
            workdata.StartDate = stakeholder.JoiningDate;
            workdata.Buckets = JSON.stringify(workdata.Buckets);
            workdata.Status = getApprovalStatus(workdata.Status);
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
        if ($csfactory.isNullOrEmptyString(endDate))
            return true;
        return moment().isBefore(moment(endDate));
    };

    //dunno how it works, copy pasted for getting the amount with precision of 2 
    var round = function (value, exp) {
        if (typeof exp === 'undefined' || +exp === 0)
            return Math.round(value);

        value = +value;
        exp = +exp;

        if (isNaN(value) || !(typeof exp === 'number' && exp % 1 === 0))
            return NaN;

        // Shift
        value = value.toString().split('e');
        value = Math.round(+(value[0] + 'e' + (value[1] ? (+value[1] + exp) : exp)));

        // Shift back
        value = value.toString().split('e');
        return +(value[0] + 'e' + (value[1] ? (+value[1] - exp) : -exp));
    };

    var computeSalary = function (basic, gross, salObj) {
        salObj.FixpayBasic = angular.isUndefined(basic) ? 0 : basic;
        salObj.FixpayGross = angular.isUndefined(gross) ? 0 : gross;

        salObj.EmployeePf = round(salObj.FixpayBasic * (salObj.EmployeePfPct / 100), 2);

        salObj.EmployerPf = round(salObj.FixpayBasic * (salObj.EmployerPfPct / 100), 2);

        salObj.EmployeeEsic = round(salObj.FixpayGross * (salObj.EmployeeEsicPct / 100), 2);

        salObj.EmployerEsic = round(salObj.FixpayGross * (salObj.EmployerEsicPct / 100), 2);

        var midTotal = round(salObj.FixpayGross + salObj.EmployerEsic + salObj.EmployerPf, 2);

        salObj.ServiceCharge = round(midTotal * (salObj.EmployerEsicPct / 100), 2);

        salObj.ServiceTax = round((midTotal + salObj.ServiceCharge) * (salObj.ServiceTaxPct / 100), 2);

        salObj.FixpayTotal = round(midTotal + salObj.ServiceTax + salObj.ServiceCharge, 2);

        return salObj;
    };

    //TODO: move this logic to server sides
    var filterWorkingList = function (workList) {
        var filteredList = [];
        _.forEach(workList, function (work) {
            if (work.Status === 'Changed') {
                if (checkEndDate(work.EndDate)) {
                    filteredList.push(work);
                }
            }
            else if (work.Status !== 'Rejected') {
                filteredList.push(work);
            }
        });
        return filteredList;
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
        GetApprovalStatus: getApprovalStatus,
        ComputeSalary: computeSalary,
        ParseBuckets: parseBuckets,
        GetDisplayManager: getDisplayManager,
        GetWorkingDetailsList: getWorkingDetailsList,
        SetProduct: setProduct,
        SetEndDate: setEndDate,
        CheckEndDate: checkEndDate,
        FilterWorkingList: filterWorkingList,
        SetReportsToName: setReportsToName,
        SetWorkList: setWorkList,
        Splice: safeSplice
    };
}]);

csapp.controller("StakeWorkingCntrl", ["$scope", "$routeParams", "StakeWorkingDatalayer", "$csModels", "StakeWorkingFactory", "$csfactory", "$location", "$timeout", "$csnotify",
    function ($scope, $routeParams, datalayer, $csModels, factory, $csfactory, $location, $timeout, $csnotify) {

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
            $scope.workingDetailsList = factory.FilterWorkingList(data.Stakeholder.StkhWorkings);
            factory.ParseBuckets($scope.workingDetailsList);
        };

        var getStakeholderData = function (stakeId) {
            datalayer.GetStakeholder(stakeId).then(function (data) {
                setData(data);
                if (angular.isDefined($routeParams.editStakeId)) {
                    $scope.formMode = 'view';
                    $scope.paymentMode = 'view';
                } else {
                    $scope.formMode = 'add';
                    $scope.paymentMode = 'add';
                }
            });
        };

        var getStakeData = function () {
            angular.isDefined($routeParams.editStakeId) ? getStakeholderData($routeParams.editStakeId)
               : getStakeholderData($routeParams.stakeId);
        };

        (function () {
            $scope.$parent.WorkingModel = {
                SelectedPincodeData: {},
                QueryFor: "",
                MultiSelectValues: [],
                DisplayManager: $scope.displayManager,
                Buckets: []
            };
            $scope.showPayment = true;
            $scope.bucketList = [1, 2, 3, 4, 5, 6];
            getStakeData();
            $scope.paymentModel = $csModels.getColumns("StkhPayment");
            $scope.workingModel = $csModels.getColumns("StkhWorking");
            $scope.workingDetailsList = $csfactory.isNullOrEmptyArray($scope.workingDetailsList) ? [] : $scope.workingDetailsList;
            $scope.deleteWorkingList = [];
        })();

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

        $scope.computeSalary = function (basic, gross) {
            $scope.SalDetails = factory.ComputeSalary(basic, gross, $scope.SalDetails);
        };

        $scope.getReportsTo = function (product) {
            datalayer.GetReportsTo($scope.currStakeholder, product).then(function (reportsToList) {
                $scope.reportsToList = reportsToList;
                autoSelect($scope.reportsToList);
            });
        };
        var autoSelect = function (reportsToList) {
            if (reportsToList.length === 1) {
                $scope.$parent.WorkingModel.SelectedPincodeData.ReportsTo = reportsToList[0].Id;
                $scope.getPincodeData($scope.$parent.WorkingModel);
            }
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

        $scope.save = function (workList) {
            factory.SetWorkList($scope.currStakeholder, workList, $scope.Working);
            return datalayer.SaveWorking(workList).then(function (data) {
                $scope.workingDetailsList = data.WorkList;
                if (!$scope.selectedHierarchy.HasPayment) {
                    $scope.gotoView();
                } else {
                    factory.ParseBuckets($scope.workingDetailsList);
                    factory.SetReportsToName($scope.workingDetailsList, data.ReportsToList);
                }
                return data;
            });
        };

        $scope.addWorkingToDelete = function (data) {
            var selected = _.find($scope.deleteWorkingList, function (working) { return working === data; });
            if ($csfactory.isEmptyObject(selected)) {
                $scope.deleteWorkingList.push(data);
            } else {
                factory.Splice($scope.deleteWorkingList, data);
            }
        };

        $scope.setApprovalStatus = function (id, status, param) {
            var stakeObj = {
                Id: id,
            };
            switch (status) {
                case 'approve':
                    switch (param) {
                        case 'working':
                            return datalayer.ApproveWorkings(stakeObj).then(function (data) {
                                return postApproval(data, param);
                            });
                        case 'payment':
                            return datalayer.ApprovePayment(stakeObj).then(function (data) {
                                return postApproval(data, param);
                            });
                    }
                case 'reject':
                    switch (param) {
                        case 'working':
                            return datalayer.RejectWorkings(stakeObj).then(function (data) {
                                return postApproval(data, param);
                            });
                        case 'payment':
                            return datalayer.RejectPayment(stakeObj).then(function (data) {
                                return postApproval(data, param);
                            });
                    }

                default:
                    throw "invalid approval status";
            }

        };
        var postApproval = function (data, param) {
            switch (param) {
                case 'working':
                    $scope.workingDetailsList = factory.FilterWorkingList(data.WorkList);
                    factory.SetReportsToName($scope.workingDetailsList, data.ReportsToList);
                    $csnotify.success("Workings Approved");
                    return $scope.workingDetailsList;
                case 'payment':
                    $scope.Payment = data;
                    $csnotify.success("Payment Approved");
                    $scope.gotoView();
                    return $scope.Payment;
                default:
                    throw "invalid param " + param;
            }
        };

        //TODO: why seperate logic for splicing it when it can maintained as is
        //TODO: handle delete of unsaved working
        $scope.deleteSelectedWorking = function (endDate) {
            factory.SetEndDate($scope.deleteWorkingList, endDate);
            factory.SetWorkList($scope.currStakeholder, $scope.deleteWorkingList);
            datalayer.DeleteWorkingList($scope.deleteWorkingList).then(function () { getStakeData(); });
            $scope.deleteWorkingList = [];
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

        $scope.getEndDate = function (data) {
            if (data.Status === 'Changed') {
                if ($csfactory.isNullOrEmptyString(data.EndDate)) return "";
                else {
                    return 'End Date: ' + moment(data.EndDate).format('YYYY-MM-DD');
                }
            }
        };

        $scope.disableDeleteBtn = function (deleteList, endDate) {
            if ($scope.showEndDate(deleteList)) {
                return $csfactory.isNullOrEmptyString(endDate);
            } else {
                return false;
            }
        };

        $scope.disableAddBtn = function (workModel, form) {
            if (form.$invalid) return true;
            if (angular.isUndefined(workModel)) return true;
            if (angular.isUndefined($scope.selectedHierarchy)) return true;
            if ($scope.selectedHierarchy.HasBuckets)//isDefined check required because the multiselect component keeps the model undefined untill selected
                return !(angular.isDefined(workModel.SelectedPincodeData.Buckets) && workModel.SelectedPincodeData.Buckets.length > 0);
            if ($scope.selectedHierarchy.LocationLevel === 'Country') return false;
            return !(angular.isDefined(workModel.SelectedPincodeData[$scope.selectedHierarchy.LocationLevel]) && workModel.SelectedPincodeData[$scope.selectedHierarchy.LocationLevel].length > 0);
        };

        $scope.showEndDate = function (deleteList) {
            var showEndDt = false;
            _.forEach(deleteList, function (working) {
                if (working.Status === 'Approved' || working.Status === 'Changed')
                    showEndDt = true;
            });
            return showEndDt;
        };

        $scope.showApproveButtons = function (workList) {
            var showApproveBtn = false;
            _.forEach(workList, function (work) {
                if (work.Status == 'Submitted') {
                    showApproveBtn = true;
                }
            });

            return showApproveBtn;
        };

        $scope.showDropdown = function (locLevel) {
            if ($csfactory.isEmptyObject($scope.selectedHierarchy)) return false;
            return $scope.selectedHierarchy.LocationLevel === locLevel;
        };

        $scope.showSaveButton = function () {
            if (angular.isUndefined($scope.workingDetailsList)) return false;
            return $scope.workingDetailsList.length == 0;
        };

        $scope.showPaymentApproval = function () {
            return $scope.Payment.ApprovalStatus === 'Submitted';
        };
    }
]);



//var getStakeholderForEdit = function (stakeId) {
//    datalayer.GetStakeholder(stakeId).then(function (data) {
//        setData(data);


//    });
//};



//delete workings

//_.forEach(remainingWorking, function(workingToBeDeleted) {
//    if (workingToBeDeleted.Status === 'Approved' || workingToBeDeleted.Status === 'Changed') {
//        if (!factory.CheckEndDate(workingToBeDeleted.EndDate))
//            factory.Splice($scope.workingDetailsList, workingToBeDeleted, $scope.selectedHierarchy.LocationLevel);
//    } else {
//        factory.Splice($scope.workingDetailsList, workingToBeDeleted, $scope.selectedHierarchy.LocationLevel);
//    }
//});



//$scope.workingChecked = function (data2) {
//    return $scope.deleteWorkingList.indexOf(data2) !== -1;
//};

//$scope.assignEndDate = function (data3, endDate) {
//    data3.EndDate = endDate;
//};

//$scope.getReportsToName = function (reportsToId) {
//    if ($csfactory.isNullOrEmptyArray($scope.reportsToList)) return "";
//    var data = _.find($scope.reportsToList, { 'Id': reportsToId });
//    return data.Name;
//};