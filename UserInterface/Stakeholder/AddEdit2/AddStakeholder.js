
csapp.factory("AddEditStakeholderDatalayer", ["$csfactory", "$csnotify", "Restangular", function ($csfactory, $csnotify, rest) {

    var apistake = rest.all('StakeholderApi');

    var getPincode = function (pincode, level) {
        return apistake.customGET('GetPincodes', { pincode: pincode, level: level }).then(function (data) {
            return data;
        });
    };

    var getHierarchies = function () {
        return apistake.customGET('GetAllHierarchies').then(function (data) {
            return data;
        }, function () {
            $csnotify.error('Error loading hierarchies');
        });

    };

    var save = function (data) {
        return apistake.customPOST(data, 'SaveStake').then(function (afterPostData) {
            return afterPostData;
        });
    };

    var checkUser = function (userId) {
        return apistake.customGET('CheckUserId', { id: userId }).then(function (data) {
            return data;
        });
    };

    return {
        CheckUser: checkUser,
        GetHierarchies: getHierarchies,
        GetPincode: getPincode,
        Save: save
    };
}]);

csapp.factory("AddEditStakeholderFactory", ["$csfactory", "$location", function ($csfactory, $location) {

    var setHierarchyModel = function (hierarchy, model) {
        if (hierarchy.IsUser) {
            model.stakeholder.MobileNo.required = true;
            model.stakeholder.ExternalId.required = true;
            model.stakeholder.EmailId.required = true;
            model.stakeholder.EmailId.suffix = '@scb.com';
        } else {
            model.stakeholder.MobileNo.required = false;
            model.stakeholder.ExternalId.required = false;
            model.stakeholder.EmailId.required = false;
            model.stakeholder.EmailId.suffix = undefined;
        }

        if (hierarchy.IsEmployee)
            model.stakeholder.JoiningDate.label = "Date of Joining";
        else model.stakeholder.JoiningDate.label = "Date of Starting";

        if (hierarchy.ManageReportsTo) {
            if (hierarchy.Hierarchy != 'External') {
                model.stakeholder.ReportingManager.label = "Line Manager";

            } else if (hierarchy.Hierarchy === 'External' && !(hierarchy.Designation == 'ExternalAgency' || hierarchy.Designation == 'ManpowerAgency')) {
                model.stakeholder.ReportingManager.label = "Agency Name";

            } else if (hierarchy.Designation == 'ExternalAgency' || hierarchy.Designation == 'ManpowerAgency') {
                model.stakeholder.ReportingManager.label = "Agency Supervisor";

            }
            model.stakeholder.ReportingManager.required = true;
        } else model.label = 'Reporting Manager';


    };

    var resetVal = function (key, except) {
        var reset = true;
        _.forEach(except, function (prop) {
            if (key === prop) reset = false;
        });
        return reset;
    };

    var resetObj = function (obj, except) {

        angular.forEach(obj, function (value, key) {
            if (resetVal(key, except)) {
                switch (typeof value) {
                    case 'object':
                        (!angular.isArray(value)) ? obj[key] = {} : obj[key] = [];
                        break;
                    case 'string':
                        obj[key] = "";
                        break;
                    case 'boolean':
                        obj[key] = false;
                        break;
                    case 'number':
                        obj[key] = 0;
                        break;
                }
            };
        });
    };

    return {
        SetHierarchyModel: setHierarchyModel,
        ResetObj: resetObj

    };
}]);

csapp.controller("AddStakeHolderCtrl", ['$routeParams', '$scope', '$log', '$window', '$csfactory', '$csnotify', '$csConstants', "$location", "$csModels", "AddEditStakeholderDatalayer", "AddEditStakeholderFactory", "$timeout",
    function ($routeParams, $scope, $log, $window, $csfactory, $csnotify, $csConstants, $location, $csModels, datalayer, factory, $timeout) {

        (function () {

            $scope.factory = factory;
            $scope.Stakeholder = {
                GAddress: [],
                StkhRegistrations: []
            };

            $scope.stakeholderModels = {
                stakeholder: $csModels.getColumns("Stakeholder"),
                address: $csModels.getColumns("StakeAddress"),
                registration: $csModels.getColumns("StkhRegistration")
            };

            datalayer.GetHierarchies().then(function (data) {
                $scope.HierarchyList = data;
                $scope.hierarchyDisplayList = _.uniq(_.pluck($scope.HierarchyList, "Hierarchy"));
            });

        })();


        $scope.Pincodes = function (pincode, level) {
            if ($csfactory.isNullOrEmptyString(pincode)) return [];
            if (pincode.length < 3) return [];
            return datalayer.GetPincode(pincode, level).then(function (data) {
                return data;
            });

        };

        $scope.checkUser = function (userId) {
            if (angular.isDefined(userId) && userId.length === 7) {
                datalayer.CheckUser(userId).then(function (exist) {
                    $scope.userExists = exist === "true" ? true : false;
                    if ($scope.userExists) $csnotify.error('User ID exists');
                });
            }
        };

        $scope.changeInHierarchy = function (hierarchy) {
            $scope.showBasicInfo = false;
            var hierarchies = _.filter($scope.HierarchyList, function (item) {
                if (item.Hierarchy === hierarchy)
                    return item;
            });
            getHierarchyDisplayName(hierarchies);
        };

        var getHierarchyDisplayName = function (hierarchy) {
            $scope.Designation = [];
            hierarchy = _.sortBy(hierarchy, 'PositionLevel');
            if (!$csfactory.isNullOrEmptyArray(hierarchy)) {
                if ((hierarchy[0].Hierarchy !== 'External')) {
                    _.forEach(hierarchy, function (item) {
                        $scope.Designation.push(item);
                    });
                } else {
                    _.forEach(hierarchy, function (item) {
                        var reportTo = _.find($scope.HierarchyList, { 'Id': item.ReportsTo });
                        var desig = {
                            Designation: angular.copy(item.Designation) + '(' + reportTo.Designation + ')',
                            Id: item.Id
                        };
                        $scope.Designation.push(desig);
                    });
                }
            }
            return '';
        };

        $scope.assignSelectedHier = function (designation, form) {
            form.$setPristine();
            factory.ResetObj($scope.Stakeholder, ['Hierarchy', 'Designation']);

            if ($csfactory.isNullOrEmptyArray(designation)) return;
            $scope.showBasicInfo = false;
            $scope.selectedHierarchy = _.find($scope.HierarchyList, { 'Id': designation });
            factory.SetHierarchyModel($scope.selectedHierarchy, $scope.stakeholderModels);
            $scope.showBasicInfo = true;
        };

        $scope.saveData = function (data) {
            setStakeObject(data);
            datalayer.Save(data).then(function (savedStakeholder) {
                $location.path('/stakeholder/working/' + savedStakeholder.Id);
            });
        };

        var setStakeObject = function (data) {
            data.GAddress = [];
            data.Registration = [];
            data.Hierarchy = $scope.selectedHierarchy;//set hierarchy
            if (!$csfactory.isEmptyObject(data.Address)) data.GAddress.push(angular.copy(data.Address));//set GAddress if exists
            if (!$csfactory.isEmptyObject(data.Regis)) data.StkhRegistrations.push(angular.copy(data.Regis));//set StkhRegistration if exists
        };

    }]);



//#region completeoldcode

//$scope.StepManager = {
//    StepNames: {
//        // Hierarchy: 'addHierarchyForm',
//        BasicInfo: 'addHierarchyForm',
//        Working: 'paymentDetailsForm'
//    },
//    SetDefaultStep: function () {
//        if ($scope.WizardData.IsEditMode() === true) {
//            $scope.showInEditmode = true;
//            $scope.currentStep = $scope.StepManager.StepNames.BasicInfo;

//        } else {
//            $scope.showInEditmode = false;
//            //$scope.currentStep = $scope.StepManager.StepNames.Hierarchy;
//        }
//        $scope.currentStep = $scope.StepManager.StepNames.BasicInfo;
//        $scope.HierarchySteps = [];
//        $scope.HierarchySteps.push($scope.currentStep);
//        $log.info("$stakeholder :  setting default step to => " + $scope.currentStep);
//        return;
//    },
//    PopulateSteps: function (hierarchy) {
//        if (angular.isUndefined(hierarchy)) {
//            $log.error("$stakeholder : hierarchy is not defined yet!!!");
//            return;
//        }

//        $scope.HierarchySteps = [];
//        if (angular.isUndefined($scope.WizardData.IsEditMode()) || $scope.WizardData.IsEditMode() === false) {
//            $scope.showInEditmode = false;
//            //$scope.HierarchySteps.push($scope.StepManager.StepNames.Hierarchy);
//        }
//        $scope.HierarchySteps.push($scope.StepManager.StepNames.BasicInfo);

//        if (hierarchy.HasWorking) {
//            $scope.HierarchySteps.push($scope.StepManager.StepNames.Working);
//        }

//        $log.info("$stakeholder : the steps are => " + $scope.HierarchySteps);
//    },
//    HasNextStep: function () {
//        if ($csfactory.isNullOrEmptyArray($scope.HierarchySteps)) return false;
//        var index = _.indexOf($scope.HierarchySteps, $scope.currentStep);
//        if (index === -1) return false;
//        if (index === $scope.HierarchySteps.length - 1) return false;
//        if ($scope.currentStep === $scope.StepManager.StepNames.BasicInfo) return true;
//        //  if ($scope.currentStep === $scope.StepManager.StepNames.Hierarchy) return true;

//        return true;
//    },
//    HasPrevStep: function () {
//        if ($csfactory.isNullOrEmptyArray($scope.HierarchySteps)) return false;
//        if (($scope.WizardData.IsEditMode() === true) && ($scope.currentStep === $scope.StepManager.StepNames.BasicInfo)) {
//            $scope.ShowPrevBtn = false;
//        } else {
//            $scope.ShowPrevBtn = true;
//        }
//        var index = _.indexOf($scope.HierarchySteps, $scope.currentStep);
//        if (index === -1) return false;
//        if (index === 0) return false;
//        return true;
//    },
//    ShowSaveButton: function () {
//        // 
//        if ($csfactory.isNullOrEmptyArray($scope.HierarchySteps)) return false;
//        var index = _.indexOf($scope.HierarchySteps, $scope.currentStep);
//        if (index === -1) return false;
//        //if ($scope.currentStep === $scope.StepManager.StepNames.BasicInfo) return false;
//        if (index === $scope.HierarchySteps.length - 1) return true;
//        return false;
//    },
//    StepForward: function () {
//        if (($scope.currentStep === $scope.StepManager.StepNames.BasicInfo) && ($scope.indexData.Hierarchy.HasAddress === true)) {
//            if (angular.isUndefined($scope.WizardData.FinalPostModel.Address.Pincode)) {
//                $csnotify.error("Pincode dosen't exist");
//                return;
//            }
//        }
//        if (($scope.currentStep === $scope.StepManager.StepNames.BasicInfo) && ($scope.indexData.Hierarchy.IsUser === true)) {
//            if ($scope.WizardData.userExists === true) {
//                $csnotify.error("UserId Already Exist");
//                return;
//            }
//        }

//        if (!$scope.StepManager.HasNextStep()) $scope.currentStep = '';
//        var index = _.indexOf($scope.HierarchySteps, $scope.currentStep);
//        $scope.currentStep = $scope.HierarchySteps[index + 1];
//    },
//    StepBackward: function () {
//        if (!$scope.StepManager.HasPrevStep()) $scope.currentStep = '';
//        var index = _.indexOf($scope.HierarchySteps, $scope.currentStep);

//        $scope.WizardData.prevStatus = true;
//        $scope.currentStep = $scope.HierarchySteps[index - 1];

//        if (($scope.WizardData.IsEditMode() === true) && ($scope.currentStep === $scope.StepManager.StepNames.BasicInfo)) {
//            $scope.ShowPrevBtn = false;
//        } else {
//            $scope.ShowPrevBtn = true;
//        }
//    },
//    IsStepInvalid: function () {
//        if ($csfactory.isNullOrEmptyString($scope.currentStep)) return true;
//        if (angular.isUndefined($scope.currentForm)) return true;
//        return $scope.currentForm.$invalid;

//    },
//    Cancel: function () {
//        if ($scope.WizardData.IsEditMode() === true) {
//            goToViewPage();
//        }

//        goToViewPage();
//        $scope.Reset();
//    }
//};

//$scope.WizardData = {

//    Payment: {},

//    BillingPolicy: {
//        LinerPolicies: [],
//        WriteOffPolicies: [],

//    },
//    FinalPostModel: {
//        Stakeholder: {
//            StkhPayments: [],
//            StkhRegistrations: [],
//            StkhWorkings: [],
//            GAddress: []
//        },
//        Hierarchy: {},
//        LocationLevel: '',
//        EmailId: '',
//        Registration: {},
//        Address: null,
//        ReportsToList: [],
//        ReportsTo: {},
//        IsEditMode: false,
//        PayWorkModel: {
//            Payment: {
//                Products: '',
//                CollectionBillingPolicy: '',
//                RecoveryBillingPolicy: '',
//                BillingPolicyId: ''
//            },
//            WorkList: []
//        },
//        PayWorkModelList: [],
//        Pincode: '',
//        SelHierarchy: {
//            Hierarchy: '',
//            Designation: '',
//        },
//        LocationLevelArray: []
//    },
//    //get functions
//    GetHierarchy: function () {
//        return $scope.WizardData.FinalPostModel.Hierarchy;

//    },
//    GetLocationLevel: function () {
//        return $scope.WizardData.FinalPostModel.LocationLevel;
//    },
//    GetLocationLevelArray: function () {
//        return $scope.WizardData.FinalPostModel.LocationLevelArray;
//    },
//    GetStakeholder: function () {
//        return $scope.WizardData.FinalPostModel.Stakeholder;
//    },
//    GetStakeholderRegistration: function () {
//        if ($scope.WizardData.FinalPostModel.isEditMode) {
//            return $scope.WizardData.FinalPostModel.Stakeholder.StkhRegistrations[0];
//        }
//        return {};
//    },
//    GetStakeholderAddress: function () {
//        if ($scope.WizardData.FinalPostModel.isEditMode) {
//            return $scope.WizardData.FinalPostModel.Stakeholder.GAddress[0];
//        }
//        return {};
//    },
//    GetReporteeList: function () {
//        if (angular.isUndefined($scope.WizardData.FinalPostModel.ReportsToList)) {
//            return [];
//        }
//        if ($scope.WizardData.FinalPostModel.ReportsToList.length === 0) {
//            return [];
//        }
//        return $scope.WizardData.FinalPostModel.ReportsToList;
//    },
//    GetReportsToStakeholder: function () {
//        if ($scope.WizardData.FinalPostModel.isEditMode) {
//            return $scope.WizardData.FinalPostModel.ReportsTo;
//        }
//        return {};
//    },
//    GetPayWorkModel: function () {
//        if (angular.isUndefined($scope.WizardData.FinalPostModel.PayWorkModel)) {
//            $scope.WizardData.FinalPostModel.PayWorkModel = {};
//            $scope.WizardData.FinalPostModel.PayWorkModel.Payment = {};
//            $scope.WizardData.FinalPostModel.PayWorkModel.WorkList = [];
//        }
//        return $scope.WizardData.FinalPostModel.PayWorkModel;
//    },
//    GetPayWorkModelList: function () {
//        if ($scope.WizardData.FinalPostModel.IsEditMode) {
//            return $scope.WizardData.FinalPostModel.PayWorkModelList;
//        }
//        return null;
//    },
//    GetApproverName: function (approverId) {
//        var appname = _.find($scope.WizardData.FinalPostModel.ReportsToList, { 'Id': approverId });
//        if (angular.isDefined(appname)) {
//            return appname.Name;
//        }
//        return '';
//    },
//    GetPincode: function () {
//        return $scope.WizardData.FinalPostModel.Pincode;
//    },

//    SetHierarchy: function (hierarchy) {
//        if ($csfactory.isEmptyObject(hierarchy)) {
//            return;
//        }
//        $scope.WizardData.FinalPostModel.Hierarchy = hierarchy;
//        setLocalHierarchy();
//    },
//    SetStakeholder: function (stakeholder) {
//        if ($csfactory.isEmptyObject(stakeholder)) {
//            return;
//        }
//        $scope.WizardData.FinalPostModel.Stakeholder = stakeholder;
//    },
//    SetRegistration: function (registration) {
//        if ($csfactory.isEmptyObject(registration)) {
//            return;
//        }
//        $scope.WizardData.FinalPostModel.Registration = registration;
//    },
//    SetAddress: function (address) {
//        if ($csfactory.isEmptyObject(address)) {
//            return;
//        }
//        $scope.WizardData.FinalPostModel.Address = address;
//    },
//    SetEmailId: function (val) {
//        if ($csfactory.isNullOrEmptyString(val)) {
//            return;
//        }
//        $scope.WizardData.FinalPostModel.EmailId = val;
//    },
//    SetReportsToStakeholder: function (stakeholder) {
//        if ($csfactory.isEmptyObject(stakeholder)) {
//            return;
//        }
//        $scope.WizardData.FinalPostModel.ReportsTo = stakeholder;
//    },
//    SetPayWorkModel: function (payworkmodel) {
//        if ($csfactory.isEmptyObject(payworkmodel)) {
//            return;
//        }
//        $scope.WizardData.FinalPostModel.PayWorkModel = payworkmodel;
//    },
//    SetPayWorkModelList: function (payworkmodelList) {
//        if ($csfactory.isEmptyObject(payworkmodelList)) {
//            return;
//        }
//        $scope.WizardData.FinalPostModel.PayWorkModelList = payworkmodelList;
//    },
//    SetLocationLevelArray: function (val) {
//        if (val.length === 0) {
//            return;
//        }
//        $scope.WizardData.FinalPostModel.LocationLevelArray = val;
//    },
//    SetLocationLevel: function (val) {
//        if ($csfactory.isNullOrEmptyString(val)) {
//            return;
//        }
//        $scope.WizardData.FinalPostModel.LocationLevel = val;
//    },
//    SetIsEditMode: function (val) {
//        if (val === true || val === false) {
//            $scope.WizardData.FinalPostModel.IsEditMode = val;
//        }
//    },

//    AddPayWorkModel: function (payworkmodel) {
//        if ($csfactory.isEmptyObject(payworkmodel)) {
//            return;
//        }


//        var dup = _.filter($scope.WizardData.FinalPostModel.PayWorkModelList, function (item) {
//            if (angular.toJson(item.Payment) === angular.toJson(payworkmodel.Payment))
//                if (angular.toJson(item.WorkList) === angular.toJson(payworkmodel.WorkList)) {
//                    return item;
//                }
//        });
//        if ($csfactory.isNullOrEmptyArray(dup))
//            $scope.WizardData.FinalPostModel.PayWorkModelList.push(angular.copy(payworkmodel));
//        else $csnotify.error("Already Added");
//    },

//    AddWorkingInPaywork: function (working) {
//        if ($csfactory.isEmptyObject(working)) {
//            return;
//        }
//        $scope.WizardData.FinalPostModel.PayWorkModel.WorkList.push(angular.copy(working));
//    },
//    RemoveWorkingFromPaywork: function (index) {
//        $scope.WizardData.FinalPostModel.PayWorkModel.WorkList.splice(index, 1);
//    },
//    RemovePayWorkModel: function (index) {
//        if (index === -1) {
//            return;
//        }
//        $scope.WizardData.FinalPostModel.PayWorkModelList.splice(index, 1);
//    },
//    IsEditMode: function () {
//        return $scope.WizardData.FinalPostModel.IsEditMode;
//    },
//    ResetPayWorkModel: function () {
//        //$scope.WizardData.FinalPostModel.PayWorkModel.Payment = {};
//        $scope.WizardData.FinalPostModel.PayWorkModel.WorkList = [];
//    },
//    ResetPayWorkModelList: function () {
//        $scope.WizardData.FinalPostModel.PayWorkModelList = [];
//    },
//    ResetStakeholderData: function () {
//        $scope.WizardData.FinalPostModel.Stakeholder = {};
//        $scope.WizardData.FinalPostModel.EmailId = null;
//        //$scope.WizardData.FinalPostModel.Stakeholder.Gender = 0;
//        $scope.WizardData.FinalPostModel.Registration = {};
//        $scope.WizardData.FinalPostModel.Address = {};
//        $scope.WizardData.FinalPostModel.Pincode = '';
//    },
//    Reset: function () {

//    },
//};

//$scope.addAnotherWorking = function () {

//    if ($scope.indexData.Hierarchy.HasPayment && !$scope.indexData.Hierarchy.HasWorking) {
//        var dummuWorking = {
//            Country: "INDIA", Products: "ALL", Region: "ALL", State: "ALL", Cluster: "ALL", District: "ALL", City: "ALL",
//            Area: "ALL", LocationLevel: "Country"
//        };
//        $scope.WizardData.FinalPostModel.PayWorkModel.WorkList.push(dummuWorking);
//        $scope.WizardData.FinalPostModel.PayWorkModel.Payment.Products = 'ALL';
//    }

//    $scope.WizardData.AddPayWorkModel($scope.WizardData.FinalPostModel.PayWorkModel);
//    $scope.WizardData.ResetPayWorkModel();
//};

//$scope.setPayWorkIndex = function (index) {
//    $scope.showIndex = $scope.showIndex === index ? false : index;
//};

//$scope.deletePaymentWorking = function (index, data) {
//    $scope.showTable = false;
//    if ($scope.WizardData.IsEditMode() === true) {
//        if ($scope.WizardData.FinalPostModel.Stakeholder.Status !== 'Approved') {
//            $scope.WizardData.RemovePayWorkModel(index);
//            return;
//        }
//        var worklist = data.WorkList;
//        data.Payment.Status = 'Submitted';
//        data.Payment.RowStatus = 'Delete';
//        _.forEach(worklist, function (item) {
//            item.Status = 'Submitted';
//            item.RowStatus = 'Delete';
//        });
//    } else {
//        $scope.WizardData.RemovePayWorkModel(index);
//    }
//    $scope.showTable = true;
//};

//$scope.displayPaymentWorkingData = function (hierarchy, count) {
//    var result = (hierarchy.HasWorking || hierarchy.HasPayment) && hierarchy.HasPayment && count > 0;
//    return result;
//};

//$scope.enableShowForCurrentStep = function (currentStep, invalid) {
//    if (currentStep === $scope.StepManager.StepNames.BasicInfo) {
//        return invalid;
//    }
//    if (currentStep === $scope.StepManager.StepNames.Working) {
//        if ($scope.WizardData.IsEditMode()) {
//            if ($scope.WizardData.FinalPostModel.Hierarchy.HasPayment && $scope.WizardData.FinalPostModel.Hierarchy.HasWorking) {
//                return ($scope.WizardData.FinalPostModel.PayWorkModelList.length == 0);
//            } else {
//                return ($scope.WizardData.FinalPostModel.PayWorkModel.WorkList.length === 0);
//            }
//        }
//        if ($scope.WizardData.FinalPostModel.Hierarchy.HasWorking && !$scope.WizardData.FinalPostModel.Hierarchy.HasPayment) {
//            return ($scope.WizardData.FinalPostModel.PayWorkModel.WorkList.length == 0);
//        }
//        if ($scope.WizardData.FinalPostModel.Hierarchy.HasPayment && $scope.WizardData.FinalPostModel.Hierarchy.HasWorking) {
//            return ($scope.WizardData.FinalPostModel.PayWorkModelList.length == 0);
//        }
//    }

//    if (currentStep === $scope.StepManager.StepNames.BasicInfo) {
//        return invalid;
//    }
//    return false;
//};

//$scope.enableAddToList = function (invalid) {
//    if ($scope.WizardData.FinalPostModel.PayWorkModel.WorkList.length > 0 && invalid == false) {
//        return false;
//    }
//    else if ($scope.indexData.Hierarchy.HasPayment && !$scope.indexData.Hierarchy.HasWorking) {
//        return invalid;
//    }
//    else {
//        return true;
//    }
//};

//var goToViewPage = function () {
//    //var downloadpath = $csConstants.MVC_BASE_URL + "Stakeholder2/StakeholerView/ViewStakeholder";
//    $location.path('/stakeholder/view');

//    //$log.info(downloadpath);

//};

////#region init
//var restApi = rest.all('AddStakeHolderApi');

//$scope.init = function () {
//    $scope.ShowPrevBtn = true;
//    $scope.WizardData.prevStatus = false;
//    $scope.showInEditmode = false;
//    $scope.showBasicInfo = false;
//    $scope.showWorking = false;
//    $scope.stakeholderModels = $csModels.getColumns("Stakeholder");

//    // $scope.showHierarchyDesignation = false;
//    $scope.indexData = {
//        Hierarchy: $scope.WizardData.GetHierarchy(),
//        ShowHierarchyDesignation: true
//    };
//    if (!$csfactory.isNullOrEmptyGuid($routeParams.data)) {
//        $scope.WizardData.FinalPostModel.IsEditMode = true;
//        getStakeholderForEdit($routeParams.data);
//    } else {
//        $scope.StepManager.SetDefaultStep();
//    }

//    $log.info("$stakeholder : intialization done.");
//};

////#region show/hide forms
//$scope.showWorkingScreen = function () {
//    $scope.showBasicInfo = false;
//    $scope.showWorking = true;
//};

//$scope.showBasicInfoScreen = function () {

//    $scope.showBasicInfo = true;
//    $scope.showWorking = false;

//};

//$scope.enableSave = function (indexData) {

//    if ($scope.WizardData.IsEditMode()) {
//        if ($scope.WizardData.FinalPostModel.Hierarchy.HasPayment && $scope.WizardData.FinalPostModel.Hierarchy.HasWorking) {
//            return ($scope.WizardData.FinalPostModel.PayWorkModelList.length == 0);
//        } else if ($scope.WizardData.FinalPostModel.Hierarchy.HasPayment && !$scope.WizardData.FinalPostModel.Hierarchy.HasWorking) {
//            return ($scope.WizardData.FinalPostModel.PayWorkModelList.length == 0);
//        }
//    }

//    if (indexData.Hierarchy.HasWorking && !indexData.Hierarchy.HasPayment)
//        return ($scope.WizardData.FinalPostModel.PayWorkModel.WorkList.length == 0);

//    if (indexData.Hierarchy.HasWorking && indexData.Hierarchy.HasPayment)
//        return ($scope.WizardData.FinalPostModel.PayWorkModelList.length == 0);

//    if (!indexData.Hierarchy.HasWorking && indexData.Hierarchy.HasPayment) {
//        return ($scope.WizardData.FinalPostModel.PayWorkModelList.length == 0);
//    }


//};
////#endregion

//var setLocalHierarchy = function () {
//    $scope.indexData.Hierarchy = $scope.WizardData.GetHierarchy();
//};

//$scope.resetWizardData = function () {
//    $scope.WizardData.ResetStakeholderData();
//    $scope.WizardData.ResetPayWorkModel();
//    $scope.WizardData.ResetPayWorkModelList();
//};

//$scope.resetPaymentInPaywork = function () {
//    $scope.WizardData.FinalPostModel.PayWorkModel.Payment = {};
//};
////#endregion

//var getStakeholderForEdit = function (stakeholderId) {
//    restApi.customGET('GetStakeholderEditMode', { stakeholderId: stakeholderId }).then(function (data) {
//        $log.debug('Stakeholder loaded for edit: ' + data);
//        $scope.WizardData.FinalPostModel = data;
//        $scope.WizardData.SetIsEditMode(true);
//        $scope.indexData.Hierarchy = $scope.WizardData.GetHierarchy();
//        $scope.StepManager.SetDefaultStep();
//        $scope.StepManager.PopulateSteps($scope.indexData.Hierarchy);
//        if ($scope.indexData.Hierarchy.IsUser) {
//            var index = $scope.WizardData.FinalPostModel.Stakeholder.EmailId.indexOf('@');
//            $scope.WizardData.FinalPostModel.EmailId = angular.copy($scope.WizardData.FinalPostModel.Stakeholder.EmailId.substring(0, index));
//        }
//        setBasicInfoModel($scope.indexData.Hierarchy);

//    }, function () {
//        $csnotify.error('Error in loading stakeholder for edit');
//        $log.error('Error in loading stakeholder for edit');
//    });
//};

//$scope.SaveData = function () {

//    if (($scope.indexData.Hierarchy.IsUser === true)) {
//        if ($scope.WizardData.userExists === true) {
//            $csnotify.error("UserId Already Exist");
//            return;
//        }
//    }

//    var hierarchy = $scope.WizardData.GetHierarchy();
//    var stakeholder = $scope.WizardData.GetStakeholder();
//    var finalPostModel = $scope.WizardData.FinalPostModel;


//    //save stakeholder
//    //if (hierarchy.Hierarchy !== 'External') {
//    //stakeholder.EmailId = finalPostModel.EmailId;//+ '@sc.com';
//    //}

//    // assign approver name
//    //stakeholder.ApprovedBy = getApproverName(stakeholder.ReportingManager);

//    if (angular.isDefined(finalPostModel.Address) && hierarchy.HasAddress === true) {
//        stakeholder.GAddress = [];
//        stakeholder.GAddress.push(finalPostModel.Address);
//    }

//    if (checkRegistration(finalPostModel.Registration)) {
//        stakeholder.StkhRegistrations = [];
//        stakeholder.StkhRegistrations.push(finalPostModel.Registration);
//    }

//    stakeholder.Hierarchy = finalPostModel.Hierarchy;
//    stakeholder.LocationLevel = finalPostModel.LocationLevel;

//    if (hierarchy.HasWorking && !hierarchy.HasPayment) {
//        finalPostModel.PayWorkModelList = [];
//        finalPostModel.PayWorkModelList.push(finalPostModel.PayWorkModel);
//    }
//    if (hierarchy.HasWorking && hierarchy.HasPayment) {
//        if (finalPostModel.PayWorkModelList.length === 0 || $scope.WizardData.IsEditMode()) {
//            if (finalPostModel.PayWorkModel.WorkList.length > 0) {

//                finalPostModel.PayWorkModelList.push(finalPostModel.PayWorkModel);
//            }
//        }
//    }

//    //if (hierarchy.HasPayment && !hierarchy.HasWorking) {

//    //    var dummyWorking = {
//    //        Country: 'INDIA',
//    //    };
//    //    $scope.WizardData.FinalPostModel.PayWorkModel.WorkList.push(dummyWorking);
//    //    $scope.WizardData.FinalPostModel.PayWorkModel.Payment = $scope.WizardData.Payment;
//    //    $scope.WizardData.FinalPostModel.PayWorkModelList.push($scope.WizardData.FinalPostModel.PayWorkModel);
//    //}

//    if (hierarchy.HasWorking && hierarchy.HasPayment) {
//        var product; //= finalPostModel.PayWorkModelList[0].WorkList[0].Products;
//        var policyCollection; //= finalPostModel.PayWorkModelList[0].Payment.CollectionBillingPolicy;
//        var policyRecovery;
//        _.forEach(finalPostModel.PayWorkModelList, function (item) {
//            //product = item.WorkList[0].Products;
//            //policyCollection = item.Payment.CollectionBillingPolicy;
//            //policyRecovery = item.Payment.RecoveryBillingPolicy;
//            item.Payment.Products = item.WorkList[0].Products;
//            item.CollectionBillingPolicyId = item.Payment.CollectionBillingPolicy;
//            item.RecoveryBillingPolicyId = item.Payment.RecoveryBillingPolicy;
//        });
//        //finalPostModel.PayWorkModel.BillingPolicy = policy;
//        //finalPostModel.PayWorkModel.Payment.Products = product;

//    }
//    stakeholder.CreatedBy = $csfactory.getCurrentUserName();
//    finalPostModel.Stakeholder = stakeholder;

//    $log.info($scope.Stakeholder);
//    restApi.customPOST(finalPostModel, 'SaveStakeholder').then(function () {
//        $log.info("finalPostModel", finalPostModel);
//        $scope.StepManager.Cancel();
//        $scope.resetWizardData();
//        $scope.resetPaymentInPaywork();
//        $csnotify.success('Stakeholder Saved');
//    }, function () {
//        $csnotify.error('Stakeholder not Saved');
//    });
//    $scope.init();
//};

//var checkRegistration = function (registration) {
//    if (!$csfactory.isNullOrEmptyString(registration)) {

//        if (!($csfactory.isNullOrEmptyString(registration.TanNo))) {
//            return true;
//        } else if (!($csfactory.isNullOrEmptyString(registration.PanNo))) {
//            return true;
//        } else if (!($csfactory.isNullOrEmptyString(registration.RegistrationNo))) {
//            return true;
//        } else {
//            return false;
//        }
//    }
//    return false;
//};

//var assignPayments = function (paymentList) {
//    $log.debug('Stakeholder : payment list count = ' + paymentList.length);
//    var list = [];
//    _.forEach(paymentList, function (item) {
//        list.push(item.Payment);
//    });
//    return list;
//};

//var getApproverName = function (approverId) {
//    var appId = _.find($scope.WizardData.FinalPostModel.ReportsToList, { 'Id': approverId });
//    if (angular.isDefined(appId)) {
//        return appId.ExternalId;
//    }
//    return '';
//};

//$scope.Reset = function () {
//    if ($scope.currentStep === $scope.StepManager.StepNames.BasicInfo) {
//        $scope.WizardData.FinalPostModel.SelHierarchy = {};
//        $scope.WizardData.FinalPostModel.Stakeholder = {};
//        $scope.WizardData.FinalPostModel.EmailId = null;
//        //$scope.WizardData.FinalPostModel.Stakeholder.Gender = 0;
//        $scope.WizardData.FinalPostModel.Registration = {};
//        $scope.WizardData.FinalPostModel.Address = {};
//        $scope.WizardData.FinalPostModel.Pincode = '';
//    }

//    //if ($scope.currentStep === $scope.StepManager.StepNames.Hierarchy) {
//    //    $scope.WizardData.FinalPostModel.SelHierarchy = {};
//    //}

//    if ($scope.currentStep === $scope.StepManager.StepNames.Working) {
//        $scope.WizardData.LocationLevel = '';
//        $scope.Stakeholder.Payment = {};

//    }
//};

//var setBasicInfoModel = function (hierarchy) {
//    $scope.WizardData.showBasicInfo = false;

//    if (hierarchy.IsUser) {
//        $scope.stakeholderModels.mobile.required = true;
//        $scope.stakeholderModels.userId.required = true;

//        $scope.stakeholderModels.email.required = true;
//        $scope.stakeholderModels.email.suffix = '@scb.com';
//    } else {
//        $scope.stakeholderModels.mobile.required = false;
//        $scope.stakeholderModels.userId.required = false;

//        $scope.stakeholderModels.email.required = false;
//        $scope.stakeholderModels.email.suffix = undefined;
//    }


//    if (hierarchy.IsEmployee)
//        $scope.stakeholderModels.date.label = "Date of Joining";
//    else $scope.stakeholderModels.date.label = "Date of Starting";


//    if (hierarchy.ManageReportsTo) {
//        if (hierarchy.Hierarchy != 'External') {
//            $scope.stakeholderModels.manager.label = "Line Manager";

//        } else if (hierarchy.Hierarchy === 'External' && !(hierarchy.Designation == 'ExternalAgency' || hierarchy.Designation == 'ManpowerAgency')) {
//            $scope.stakeholderModels.manager.label = "Agency Name";

//        } else if (hierarchy.Designation == 'ExternalAgency' || hierarchy.Designation == 'ManpowerAgency') {
//            $scope.stakeholderModels.manager.label = "Agency Supervisor";

//        }
//        $scope.stakeholderModels.manager.required = true;
//        $scope.stakeholderModels.manager.valueList = $scope.WizardData.FinalPostModel.ReportsToList;

//    }

//    $scope.WizardData.showBasicInfo = true;
//};

//$scope.getReportsTo = function (hierarchy) {

//    if (angular.isUndefined(hierarchy)) {
//        return;
//    }
//    restApi.customPOST(hierarchy, 'GetReportsToInHierarchy')
//           .then(function (data) {
//               $scope.WizardData.FinalPostModel.ReportsToList = data;
//               setBasicInfoModel(hierarchy);

//               //if (!$scope.$$phase) {
//               //    $scope.$apply();
//               //  $log.info("$apply called");
//               //}
//               // $log.info("$apply called 2nd");
//               //$log.info($scope.WizardData.FinalPostModel.ReportsToList);

//               //$scope.ReportsToList = data;
//           });
//};

//#endregion











//$scope.Stakeholder = {
//    StkhPayments: [],
//    GAddress: [],
//    StkhRegistrations: [],
//    StkhWorkings: []
//};

//$scope.WizardData = {};
//$scope.WizardData.SelHierarchy = {
//    Hierarchy: '',
//    Designation: '',
//    //Id:''
//};
//$scope.WizardData.LocationLevel = '';
//$scope.WizardData.PaymentList = [];
//$scope.WizardData.EmailId = '';
//// $scope.WizardData.Gender = 0;
//$scope.WizardData.Registration = {};
//$scope.ReportsToList = [];
//$scope.WizardData.LocationLevelArray = [];
//$scope.WizardData.isFromWorking = false;
//$scope.WizardData.isFromPayment = false;
//$scope.WizardData.PaymentForWork = {};

//$scope.WizardData.PayWorkModel = {
//    Payment: {},
//    WorkList: []
//};
//$scope.WizardData.PayWorkModelList = [];
//$scope.WizardData.finalPostModel = {
//    Stakeholders: {},
//    PayWork: []
//};

//set staekholder for edit mode
//var setStakeholderForEdit = function (stakeholder) {
//    // 
//    $scope.Stakeholder = stakeholder;
//    $scope.addStakeholderpanel1 = false;
//    $scope.WizardData.LocationLevel = stakeholder.LocationLevel;
//    $scope.WizardData.EmailId = stakeholder.EmailId.substring(1, stakeholder.EmailId.indexOf('@'));
//    $scope.WizardData.Hierarchy = stakeholder.Hierarchy;
//    $scope.WizardData.Registration = stakeholder.StkhRegistrations.length > 0 ? stakeholder.StkhRegistrations[0] : null;
//    $scope.WizardData.PaymentList = stakeholder.StkhPayments;
//    $scope.WizardData.Address = stakeholder.GAddress[0];

//    setPaymentAndWorking(stakeholder);
//    //load reports to list
//    $scope.getReportsTo(stakeholder.Hierarchy);
//};

//var setPaymentAndWorking = function (stakeholder) {
//    //set payment and working list
//    _.forEach($scope.Stakeholder.StkhPayments, function (item) {
//        $scope.WizardData.PayWorkModel.Payment = item;
//        $scope.WizardData.PayWorkModel.WorkList = _.find($scope.Stakeholder.StkhWorkings, { 'StkhPaymentId': item.Id });
//        $scope.WizardData.PayWorkModelList.push($scope.WizardData.PayWorkModel.Payment);
//        $scope.WizardData.PayWorkModel.Payment = {};
//    });
//};
//Registration: 'registrationForm',
//Payment: 'paymentDetailsForm',
//,
//Address: 'addressDetailsForm'

//if (hierarchy.HasRegistration) {
//    $scope.HierarchySteps.push($scope.StepManager.StepNames.Registration);
//}
//if (hierarchy.HasPayment) {
//    $scope.HierarchySteps.push($scope.StepManager.StepNames.Payment);
//}
//if (hierarchy.HasAddress) {
//    $scope.HierarchySteps.push($scope.StepManager.StepNames.Address);
//}
//$log.debug("$stakeholder : current step => " + $scope.currentStep +
//    " , and currently invalid => " + $scope.StepManager.StepIsInvalid);
// $scope.currentStep = $scope.StepManager.StepNames.Hierarchy;

//$scope.WizardData.finalPostModel.Stakeholders = $scope.Stakeholder;
//$scope.WizardData.finalPostModel.PayWork = $scope.WizardData.PayWorkModelList;
//$scope.WizardData.finalPostModel.Hierarchy = $scope.WizardData.Hierarchy;
//$scope.WizardData.finalPostModel.IsEditMode = $scope.isEditMode;
