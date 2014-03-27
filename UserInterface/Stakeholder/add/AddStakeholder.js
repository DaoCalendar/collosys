(
csapp.controller("AddStakeHolderCtrl", ['$routeParams', '$scope', 'Restangular', '$Validations', '$log', '$window', '$csfactory', '$csnotify', '$csConstants', "$location",
function ($routeParams, $scope, rest, $validations, $log, $window, $csfactory, $csnotify, $csConstants, $location) {

    $scope.StepManager = {
        StepNames: {
            // Hierarchy: 'addHierarchyForm',
            BasicInfo: 'addHierarchyForm',
            Working: 'paymentDetailsForm'
        },
        SetDefaultStep: function () {
            if ($scope.WizardData.IsEditMode() === true) {
                $scope.showInEditmode = true;
                $scope.currentStep = $scope.StepManager.StepNames.BasicInfo;

            } else {
                $scope.showInEditmode = false;
                //$scope.currentStep = $scope.StepManager.StepNames.Hierarchy;
            }
            $scope.currentStep = $scope.StepManager.StepNames.BasicInfo;
            $scope.HierarchySteps = [];
            $scope.HierarchySteps.push($scope.currentStep);
            $log.info("$stakeholder :  setting default step to => " + $scope.currentStep);
            return;
        },
        PopulateSteps: function (hierarchy) {
            if (angular.isUndefined(hierarchy)) {
                $log.error("$stakeholder : hierarchy is not defined yet!!!");
                return;
            }

            $scope.HierarchySteps = [];
            if (angular.isUndefined($scope.WizardData.IsEditMode()) || $scope.WizardData.IsEditMode() === false) {
                $scope.showInEditmode = false;
                //$scope.HierarchySteps.push($scope.StepManager.StepNames.Hierarchy);
            }
            $scope.HierarchySteps.push($scope.StepManager.StepNames.BasicInfo);

            if (hierarchy.HasWorking) {
                $scope.HierarchySteps.push($scope.StepManager.StepNames.Working);
            }

            $log.info("$stakeholder : the steps are => " + $scope.HierarchySteps);
        },
        HasNextStep: function () {
            if ($csfactory.isNullOrEmptyArray($scope.HierarchySteps)) return false;
            var index = _.indexOf($scope.HierarchySteps, $scope.currentStep);
            if (index === -1) return false;
            if (index === $scope.HierarchySteps.length - 1) return false;
            if ($scope.currentStep === $scope.StepManager.StepNames.BasicInfo) return true;
            //  if ($scope.currentStep === $scope.StepManager.StepNames.Hierarchy) return true;

            return true;
        },
        HasPrevStep: function () {
            if ($csfactory.isNullOrEmptyArray($scope.HierarchySteps)) return false;
            if (($scope.WizardData.IsEditMode() === true) && ($scope.currentStep === $scope.StepManager.StepNames.BasicInfo)) {
                $scope.ShowPrevBtn = false;
            } else {
                $scope.ShowPrevBtn = true;
            }
            var index = _.indexOf($scope.HierarchySteps, $scope.currentStep);
            if (index === -1) return false;
            if (index === 0) return false;
            return true;
        },
        ShowSaveButton: function () {
            // debugger;
            if ($csfactory.isNullOrEmptyArray($scope.HierarchySteps)) return false;
            var index = _.indexOf($scope.HierarchySteps, $scope.currentStep);
            if (index === -1) return false;
            //if ($scope.currentStep === $scope.StepManager.StepNames.BasicInfo) return false;
            if (index === $scope.HierarchySteps.length - 1) return true;
            return false;
        },
        StepForward: function () {
            if (($scope.currentStep === $scope.StepManager.StepNames.BasicInfo) && ($scope.indexData.Hierarchy.HasAddress === true)) {
                if (angular.isUndefined($scope.WizardData.FinalPostModel.Address.Pincode)) {
                    $csnotify.error("Pincode dosen't exist");
                    return;
                }
            }
            if (($scope.currentStep === $scope.StepManager.StepNames.BasicInfo) && ($scope.indexData.Hierarchy.IsUser === true)) {
                if ($scope.WizardData.userExists === true) {
                    $csnotify.error("UserId Already Exist");
                    return;
                }
            }

            if (!$scope.StepManager.HasNextStep()) $scope.currentStep = '';
            var index = _.indexOf($scope.HierarchySteps, $scope.currentStep);
            $scope.currentStep = $scope.HierarchySteps[index + 1];
        },
        StepBackward: function () {
            if (!$scope.StepManager.HasPrevStep()) $scope.currentStep = '';
            var index = _.indexOf($scope.HierarchySteps, $scope.currentStep);

            $scope.WizardData.prevStatus = true;
            $scope.currentStep = $scope.HierarchySteps[index - 1];

            if (($scope.WizardData.IsEditMode() === true) && ($scope.currentStep === $scope.StepManager.StepNames.BasicInfo)) {
                $scope.ShowPrevBtn = false;
            } else {
                $scope.ShowPrevBtn = true;
            }
        },
        IsStepInvalid: function () {
            if ($csfactory.isNullOrEmptyString($scope.currentStep)) return true;
            if (angular.isUndefined($scope.currentForm)) return true;
            return $scope.currentForm.$invalid;

        },
        Cancel: function () {
            if ($scope.WizardData.IsEditMode() === true) {
                goToViewPage();
            }

            goToViewPage();
            $scope.Reset();
        }
    };

    $scope.WizardData = {

        Payment: {},

        BillingPolicy: {
            LinerPolicies: [],
            WriteOffPolicies: [],

        },
        FinalPostModel: {
            Stakeholder: {
                StkhPayments: [],
                StkhRegistrations: [],
                StkhWorkings: [],
                GAddress: []
            },
            Hierarchy: {},
            LocationLevel: '',
            EmailId: '',
            Registration: {},
            Address: null,
            ReportsToList: [],
            ReportsTo: {},
            IsEditMode: false,
            PayWorkModel: {
                Payment: {
                    Products: '',
                    CollectionBillingPolicy: '',
                    RecoveryBillingPolicy: '',
                    BillingPolicyId: ''
                },
                WorkList: []
            },
            PayWorkModelList: [],
            Pincode: '',
            SelHierarchy: {
                Hierarchy: '',
                Designation: '',
            },
            LocationLevelArray: []
        },
        //get functions
        GetHierarchy: function () {
            return $scope.WizardData.FinalPostModel.Hierarchy;

        },
        GetLocationLevel: function () {
            return $scope.WizardData.FinalPostModel.LocationLevel;
        },
        GetLocationLevelArray: function () {
            return $scope.WizardData.FinalPostModel.LocationLevelArray;
        },
        GetStakeholder: function () {
            return $scope.WizardData.FinalPostModel.Stakeholder;
        },
        GetStakeholderRegistration: function () {
            if ($scope.WizardData.FinalPostModel.isEditMode) {
                return $scope.WizardData.FinalPostModel.Stakeholder.StkhRegistrations[0];
            }
            return {};
        },
        GetStakeholderAddress: function () {
            if ($scope.WizardData.FinalPostModel.isEditMode) {
                return $scope.WizardData.FinalPostModel.Stakeholder.GAddress[0];
            }
            return {};
        },
        GetReporteeList: function () {
            if (angular.isUndefined($scope.WizardData.FinalPostModel.ReportsToList)) {
                return [];
            }
            if ($scope.WizardData.FinalPostModel.ReportsToList.length === 0) {
                return [];
            }
            return $scope.WizardData.FinalPostModel.ReportsToList;
        },
        GetReportsToStakeholder: function () {
            if ($scope.WizardData.FinalPostModel.isEditMode) {
                return $scope.WizardData.FinalPostModel.ReportsTo;
            }
            return {};
        },
        GetPayWorkModel: function () {
            if (angular.isUndefined($scope.WizardData.FinalPostModel.PayWorkModel)) {
                $scope.WizardData.FinalPostModel.PayWorkModel = {};
                $scope.WizardData.FinalPostModel.PayWorkModel.Payment = {};
                $scope.WizardData.FinalPostModel.PayWorkModel.WorkList = [];
            }
            return $scope.WizardData.FinalPostModel.PayWorkModel;
        },
        GetPayWorkModelList: function () {
            if ($scope.WizardData.FinalPostModel.IsEditMode) {
                return $scope.WizardData.FinalPostModel.PayWorkModelList;
            }
            return null;
        },
        GetApproverName: function (approverId) {
            var appname = _.find($scope.WizardData.FinalPostModel.ReportsToList, { 'Id': approverId });
            if (angular.isDefined(appname)) {
                return appname.Name;
            }
            return '';
        },
        GetPincode: function () {
            return $scope.WizardData.FinalPostModel.Pincode;
        },

        SetHierarchy: function (hierarchy) {
            if ($csfactory.isEmptyObject(hierarchy)) {
                return;
            }
            $scope.WizardData.FinalPostModel.Hierarchy = hierarchy;
            setLocalHierarchy();
        },
        SetStakeholder: function (stakeholder) {
            if ($csfactory.isEmptyObject(stakeholder)) {
                return;
            }
            $scope.WizardData.FinalPostModel.Stakeholder = stakeholder;
        },
        SetRegistration: function (registration) {
            if ($csfactory.isEmptyObject(registration)) {
                return;
            }
            $scope.WizardData.FinalPostModel.Registration = registration;
        },
        SetAddress: function (address) {
            if ($csfactory.isEmptyObject(address)) {
                return;
            }
            $scope.WizardData.FinalPostModel.Address = address;
        },
        SetEmailId: function (val) {
            if ($csfactory.isNullOrEmptyString(val)) {
                return;
            }
            $scope.WizardData.FinalPostModel.EmailId = val;
        },
        SetReportsToStakeholder: function (stakeholder) {
            if ($csfactory.isEmptyObject(stakeholder)) {
                return;
            }
            $scope.WizardData.FinalPostModel.ReportsTo = stakeholder;
        },
        SetPayWorkModel: function (payworkmodel) {
            if ($csfactory.isEmptyObject(payworkmodel)) {
                return;
            }
            $scope.WizardData.FinalPostModel.PayWorkModel = payworkmodel;
        },
        SetPayWorkModelList: function (payworkmodelList) {
            if ($csfactory.isEmptyObject(payworkmodelList)) {
                return;
            }
            $scope.WizardData.FinalPostModel.PayWorkModelList = payworkmodelList;
        },
        SetLocationLevelArray: function (val) {
            if (val.length === 0) {
                return;
            }
            $scope.WizardData.FinalPostModel.LocationLevelArray = val;
        },
        SetLocationLevel: function (val) {
            if ($csfactory.isNullOrEmptyString(val)) {
                return;
            }
            $scope.WizardData.FinalPostModel.LocationLevel = val;
        },
        SetIsEditMode: function (val) {
            if (val === true || val === false) {
                $scope.WizardData.FinalPostModel.IsEditMode = val;
            }
        },

        AddPayWorkModel: function (payworkmodel) {
            if ($csfactory.isEmptyObject(payworkmodel)) {
                return;
            }


            var dup = _.filter($scope.WizardData.FinalPostModel.PayWorkModelList, function (item) {
                if (angular.toJson(item.Payment) === angular.toJson(payworkmodel.Payment))
                    if (angular.toJson(item.WorkList) === angular.toJson(payworkmodel.WorkList)) {
                        return item;
                    }
            });
            if ($csfactory.isNullOrEmptyArray(dup))
                $scope.WizardData.FinalPostModel.PayWorkModelList.push(angular.copy(payworkmodel));
            else $csnotify.error("Already Added");
        },

        AddWorkingInPaywork: function (working) {
            if ($csfactory.isEmptyObject(working)) {
                return;
            }
            $scope.WizardData.FinalPostModel.PayWorkModel.WorkList.push(angular.copy(working));
        },
        RemoveWorkingFromPaywork: function (index) {
            $scope.WizardData.FinalPostModel.PayWorkModel.WorkList.splice(index, 1);
        },
        RemovePayWorkModel: function (index) {
            if (index === -1) {
                return;
            }
            $scope.WizardData.FinalPostModel.PayWorkModelList.splice(index, 1);
        },
        IsEditMode: function () {
            return $scope.WizardData.FinalPostModel.IsEditMode;
        },
        ResetPayWorkModel: function () {
            $scope.WizardData.FinalPostModel.PayWorkModel.Payment = {};
            $scope.WizardData.FinalPostModel.PayWorkModel.WorkList = [];
        },
        ResetPayWorkModelList: function () {
            $scope.WizardData.FinalPostModel.PayWorkModelList = [];
        },
        ResetStakeholderData: function () {
            $scope.WizardData.FinalPostModel.Stakeholder = {};
            $scope.WizardData.FinalPostModel.EmailId = null;
            //$scope.WizardData.FinalPostModel.Stakeholder.Gender = 0;
            $scope.WizardData.FinalPostModel.Registration = {};
            $scope.WizardData.FinalPostModel.Address = {};
            $scope.WizardData.FinalPostModel.Pincode = '';
        },
        Reset: function () {

        },
    };

    $scope.addAnotherWorking = function () {
        $scope.WizardData.AddPayWorkModel($scope.WizardData.FinalPostModel.PayWorkModel);
        $scope.WizardData.ResetPayWorkModel();
    };
    $scope.deletePaymentWorking = function (index, data) {
        if ($scope.WizardData.IsEditMode() === true) {
            if ($scope.WizardData.FinalPostModel.Stakeholder.Status !== 'Approved') {
                $scope.WizardData.RemovePayWorkModel(index);
                return;
            }
            var worklist = data.WorkList;
            data.Payment.Status = 'Submitted';
            data.Payment.RowStatus = 'Delete';
            _.forEach(worklist, function (item) {
                item.Status = 'Submitted';
                item.RowStatus = 'Delete';
            });
        } else {
            $scope.WizardData.RemovePayWorkModel(index);
        }

    };

    $scope.displayPaymentWorkingData = function (hierarchy, count) {
        var result = hierarchy.HasWorking && hierarchy.HasPayment && count > 0;
        return result;
    };

    $scope.enableShowForCurrentStep = function (currentStep, invalid) {
        if (currentStep === $scope.StepManager.StepNames.BasicInfo) {
            return invalid;
        }
        if (currentStep === $scope.StepManager.StepNames.Working) {
            if ($scope.WizardData.IsEditMode()) {
                if ($scope.WizardData.FinalPostModel.Hierarchy.HasPayment && $scope.WizardData.FinalPostModel.Hierarchy.HasWorking) {
                    return ($scope.WizardData.FinalPostModel.PayWorkModelList.length == 0);
                } else {
                    return ($scope.WizardData.FinalPostModel.PayWorkModel.WorkList.length === 0);
                }
            }
            if ($scope.WizardData.FinalPostModel.Hierarchy.HasWorking && !$scope.WizardData.FinalPostModel.Hierarchy.HasPayment) {
                return ($scope.WizardData.FinalPostModel.PayWorkModel.WorkList.length == 0);
            }
            if ($scope.WizardData.FinalPostModel.Hierarchy.HasPayment && $scope.WizardData.FinalPostModel.Hierarchy.HasWorking) {
                return ($scope.WizardData.FinalPostModel.PayWorkModelList.length == 0);
            }
        }

        if (currentStep === $scope.StepManager.StepNames.BasicInfo) {
            return invalid;
        }
        return false;
    };
    $scope.enableAddToList = function (invalid) {
        if ($scope.WizardData.FinalPostModel.PayWorkModel.WorkList.length > 0 && invalid == false) {
            return false;
        }
        else if ($scope.indexData.Hierarchy.HasPayment && !$scope.indexData.Hierarchy.HasWorking) {
            return false;
        }
        else {
            return true;
        }
    };

    var goToViewPage = function () {
        //var downloadpath = $csConstants.MVC_BASE_URL + "Stakeholder2/StakeholerView/ViewStakeholder";
        $location.path('/stakeholder/view');
        //$log.info(downloadpath);

    };

    //#region init
    var restApi = rest.all('AddStakeHolderApi');

    $scope.init = function () {
        $scope.ShowPrevBtn = true;
        $scope.WizardData.prevStatus = false;
        $scope.showInEditmode = false;
        $scope.showBasicInfo = true;
        $scope.showWorking = false;

        // $scope.showHierarchyDesignation = false;
        $scope.val = $validations;
        $scope.indexData = {
            Hierarchy: $scope.WizardData.GetHierarchy(),
            ShowHierarchyDesignation: true
        };
        if (!$csfactory.isNullOrEmptyGuid($routeParams.data)) {
            getStakeholderForEdit($routeParams.data);
        } else {
            $scope.StepManager.SetDefaultStep();
        }

        $log.info("$stakeholder : intialization done.");
    };

    //#region show/hide forms
    $scope.showWorkingScreen = function () {
        $scope.showBasicInfo = false;
        $scope.showWorking = true;
    };

    $scope.showBasicInfoScreen = function () {

        $scope.showBasicInfo = true;
        $scope.showWorking = false;

    };

    $scope.enableSave = function (indexData) {

        if ($scope.WizardData.IsEditMode()) {
            if ($scope.WizardData.FinalPostModel.Hierarchy.HasPayment && $scope.WizardData.FinalPostModel.Hierarchy.HasWorking) {
                return ($scope.WizardData.FinalPostModel.PayWorkModelList.length == 0);
            } else {
                return ($scope.WizardData.FinalPostModel.PayWorkModel.WorkList.length === 0);
            }
        }

        if (indexData.Hierarchy.HasWorking && !indexData.Hierarchy.HasPayment)
            return ($scope.WizardData.FinalPostModel.PayWorkModel.WorkList.length == 0);

        if (indexData.Hierarchy.HasWorking && indexData.Hierarchy.HasPayment)
            return ($scope.WizardData.FinalPostModel.PayWorkModelList.length == 0);

        if (!indexData.Hierarchy.HasWorking && indexData.Hierarchy.HasPayment) {
            return true;
        }


    };
    //#endregion

    var setLocalHierarchy = function () {
        $scope.indexData.Hierarchy = $scope.WizardData.GetHierarchy();
    };

    $scope.resetWizardData = function () {
        $scope.WizardData.ResetStakeholderData();
        $scope.WizardData.ResetPayWorkModel();
        $scope.WizardData.ResetPayWorkModelList();
    };
    $scope.resetPaymentInPaywork = function () {
        $scope.WizardData.FinalPostModel.PayWorkModel.Payment = {};
    };
    //#endregion
    //get stakeholder for edit
    var getStakeholderForEdit = function (stakeholderId) {
        restApi.customGET('GetStakeholderEditMode', { stakeholderId: stakeholderId }).then(function (data) {
            $log.debug('Stakeholder loaded for edit: ' + data);
            $scope.WizardData.FinalPostModel = data;
            $scope.WizardData.SetIsEditMode(true);
            $scope.indexData.Hierarchy = $scope.WizardData.GetHierarchy();
            $scope.StepManager.SetDefaultStep();
            $scope.StepManager.PopulateSteps($scope.indexData.Hierarchy);
            if ($scope.indexData.Hierarchy.IsUser) {
                var index = $scope.WizardData.FinalPostModel.Stakeholder.EmailId.indexOf('@');
                $scope.WizardData.FinalPostModel.EmailId = angular.copy($scope.WizardData.FinalPostModel.Stakeholder.EmailId.substring(0, index));
            }

        }, function () {
            $csnotify.error('Error in loading stakeholder for edit');
            $log.error('Error in loading stakeholder for edit');
        });
    };

    $scope.SaveData = function () {

        if (($scope.indexData.Hierarchy.IsUser === true)) {
            if ($scope.WizardData.userExists === true) {
                $csnotify.error("UserId Already Exist");
                return;
            }
        }



        var hierarchy = $scope.WizardData.GetHierarchy();
        var stakeholder = $scope.WizardData.GetStakeholder();
        var finalPostModel = $scope.WizardData.FinalPostModel;


        //save stakeholder
        if (hierarchy.Hierarchy !== 'External') {
            stakeholder.EmailId = finalPostModel.EmailId + '@sc.com';
        }

        // assign approver name
        //stakeholder.ApprovedBy = getApproverName(stakeholder.ReportingManager);

        if (angular.isDefined(finalPostModel.Address) && hierarchy.HasAddress === true) {
            stakeholder.GAddress = [];
            stakeholder.GAddress.push(finalPostModel.Address);
        }

        if (checkRegistration(finalPostModel.Registration)) {
            stakeholder.StkhRegistrations = [];
            stakeholder.StkhRegistrations.push(finalPostModel.Registration);
        }

        stakeholder.Hierarchy = finalPostModel.Hierarchy;
        stakeholder.LocationLevel = finalPostModel.LocationLevel;

        if (hierarchy.HasWorking && !hierarchy.HasPayment) {
            finalPostModel.PayWorkModelList = [];
            finalPostModel.PayWorkModelList.push(finalPostModel.PayWorkModel);
        }
        if (hierarchy.HasWorking && hierarchy.HasPayment) {
            if (finalPostModel.PayWorkModelList.length === 0 || $scope.WizardData.IsEditMode()) {
                if (finalPostModel.PayWorkModel.WorkList.length > 0) {

                    finalPostModel.PayWorkModelList.push(finalPostModel.PayWorkModel);
                }
            }
        }

        if (hierarchy.HasPayment && !hierarchy.HasWorking) {
            
            var dummyWorking = {
                Country: 'INDIA',
            };
            $scope.WizardData.FinalPostModel.PayWorkModel.WorkList.push(dummyWorking);
            $scope.WizardData.FinalPostModel.PayWorkModel.Payment = $scope.WizardData.Payment;
            $scope.WizardData.FinalPostModel.PayWorkModelList.push($scope.WizardData.FinalPostModel.PayWorkModel);
        }

        if (hierarchy.HasWorking && hierarchy.HasPayment) {
            var product; //= finalPostModel.PayWorkModelList[0].WorkList[0].Products;
            var policyCollection; //= finalPostModel.PayWorkModelList[0].Payment.CollectionBillingPolicy;
            var policyRecovery;
            _.forEach(finalPostModel.PayWorkModelList, function (item) {
                //product = item.WorkList[0].Products;
                //policyCollection = item.Payment.CollectionBillingPolicy;
                //policyRecovery = item.Payment.RecoveryBillingPolicy;
                item.Payment.Products = item.WorkList[0].Products;
                item.CollectionBillingPolicyId = item.Payment.CollectionBillingPolicy;
                item.RecoveryBillingPolicyId = item.Payment.RecoveryBillingPolicy;
            });
            //finalPostModel.PayWorkModel.BillingPolicy = policy;
            //finalPostModel.PayWorkModel.Payment.Products = product;

        }
        stakeholder.CreatedBy = $csfactory.getCurrentUserName();
        finalPostModel.Stakeholder = stakeholder;

        $log.info($scope.Stakeholder);
        restApi.customPOST(finalPostModel, 'SaveStakeholder').then(function () {
            $log.info("finalPostModel", finalPostModel);
            $scope.StepManager.Cancel();
            $scope.resetWizardData();
            $scope.resetPaymentInPaywork();
            $csnotify.success('Stakeholder Saved');
        }, function () {
        });
        $scope.init();
    };

    var checkRegistration = function (registration) {
        if (!$csfactory.isNullOrEmptyString(registration)) {

            if (!($csfactory.isNullOrEmptyString(registration.TanNo))) {
                return true;
            } else if (!($csfactory.isNullOrEmptyString(registration.PanNo))) {
                return true;
            } else if (!($csfactory.isNullOrEmptyString(registration.RegistrationNo))) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    };
    var assignPayments = function (paymentList) {
        $log.debug('Stakeholder : payment list count = ' + paymentList.length);
        var list = [];
        _.forEach(paymentList, function (item) {
            list.push(item.Payment);
        });
        return list;
    };

    var getApproverName = function (approverId) {
        var appId = _.find($scope.WizardData.FinalPostModel.ReportsToList, { 'Id': approverId });
        if (angular.isDefined(appId)) {
            return appId.ExternalId;
        }
        return '';
    };

    $scope.Reset = function () {
        if ($scope.currentStep === $scope.StepManager.StepNames.BasicInfo) {
            $scope.WizardData.FinalPostModel.SelHierarchy = {};
            $scope.WizardData.FinalPostModel.Stakeholder = {};
            $scope.WizardData.FinalPostModel.EmailId = null;
            //$scope.WizardData.FinalPostModel.Stakeholder.Gender = 0;
            $scope.WizardData.FinalPostModel.Registration = {};
            $scope.WizardData.FinalPostModel.Address = {};
            $scope.WizardData.FinalPostModel.Pincode = '';
        }

        //if ($scope.currentStep === $scope.StepManager.StepNames.Hierarchy) {
        //    $scope.WizardData.FinalPostModel.SelHierarchy = {};
        //}

        if ($scope.currentStep === $scope.StepManager.StepNames.Working) {
            $scope.WizardData.LocationLevel = '';
            $scope.Stakeholder.Payment = {};

        }
    };

    $scope.getReportsTo = function (hierarchy) {
        debugger;
        if (angular.isUndefined(hierarchy)) {
            return;
        }
        restApi.customPOST(hierarchy, 'GetReportsToInHierarchy')
               .then(function (data) {
                   $scope.WizardData.FinalPostModel.ReportsToList = data;
                   if (!$scope.$$phase) {
                       $scope.$apply();
                       //  $log.info("$apply called");
                   }
                   // $log.info("$apply called 2nd");
                   //$log.info($scope.WizardData.FinalPostModel.ReportsToList);

                   //$scope.ReportsToList = data;
               });
    };

}])
);



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
//    // debugger;
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
