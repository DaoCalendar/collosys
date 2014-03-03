
//Amol
csapp.controller("AddStakeHolderCtrl", ['$scope', 'Restangular', '$Validations', '$log', '$window', '$csfactory', '$csnotify',
    function ($scope, rest, $validations, $log, $window, $csfactory, $csnotify) {

        $scope.StepManager = {
            StepNames: {
                Hierarchy: 'addHierarchyForm',
                BasicInfo: 'basicInfoForm',
                Registration: 'registrationForm',
                Payment: 'paymentDetailsForm',
                Working: 'paymentWorkinForm',
                Address: 'addressDetailsForm'
            },
            SetDefaultStep: function () {
                if ($scope.isEditMode === true) {
                    $scope.currentStep = $scope.StepManager.StepNames.BasicInfo;
                } else {
                    $scope.currentStep = $scope.StepManager.StepNames.Hierarchy;
                }
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
                if (angular.isUndefined($scope.isEditMode) || $scope.isEditMode === false) {
                    $scope.HierarchySteps.push($scope.StepManager.StepNames.Hierarchy);
                }
                $scope.HierarchySteps.push($scope.StepManager.StepNames.BasicInfo);

                if (hierarchy.HasRegistration) {
                    $scope.HierarchySteps.push($scope.StepManager.StepNames.Registration);
                }
                if (hierarchy.HasPayment) {
                    $scope.HierarchySteps.push($scope.StepManager.StepNames.Payment);
                }
                if (hierarchy.HasWorking) {
                    $scope.HierarchySteps.push($scope.StepManager.StepNames.Working);
                }
                if (hierarchy.HasAddress) {
                    $scope.HierarchySteps.push($scope.StepManager.StepNames.Address);
                }
                $log.info("$stakeholder : the steps are => " + $scope.HierarchySteps);
            },
            HasNextStep: function () {
                if ($csfactory.isNullOrEmptyArray($scope.HierarchySteps)) return false;
                var index = _.indexOf($scope.HierarchySteps, $scope.currentStep);
                if (index === -1) return false;
                if ($scope.currentStep === $scope.StepManager.StepNames.Hierarchy) return true;
                if (index === $scope.HierarchySteps.length - 1) return false;
                return true;
            },
            HasPrevStep: function () {
                if ($csfactory.isNullOrEmptyArray($scope.HierarchySteps)) return false;
                var index = _.indexOf($scope.HierarchySteps, $scope.currentStep);
                if (index === -1) return false;
                if (index === 0) return false;
                return true;
            },
            ShowSaveButton: function () {
                if ($csfactory.isNullOrEmptyArray($scope.HierarchySteps)) return false;
                var index = _.indexOf($scope.HierarchySteps, $scope.currentStep);
                if (index === -1) return false;
                if ($scope.currentStep === $scope.StepManager.StepNames.Hierarchy) return false;
                if (index === $scope.HierarchySteps.length - 1) return true;
                return false;
            },
            StepForward: function () {
                if (!$scope.StepManager.HasNextStep()) $scope.currentStep = '';
                var index = _.indexOf($scope.HierarchySteps, $scope.currentStep);
                $scope.currentStep = $scope.HierarchySteps[index + 1];
            },
            StepBackward: function () {
                if (!$scope.StepManager.HasPrevStep()) $scope.currentStep = '';
                var index = _.indexOf($scope.HierarchySteps, $scope.currentStep);
                $scope.currentStep = $scope.HierarchySteps[index - 1];
            },
            IsStepInvalid: function () {
                if ($csfactory.isNullOrEmptyString($scope.currentStep)) return true;
                if (angular.isUndefined($scope.currentForm)) return true;
                return $scope.currentForm.$invalid;
                //$log.debug("$stakeholder : current step => " + $scope.currentStep +
                //    " , and currently invalid => " + $scope.StepManager.StepIsInvalid);
            },
            Cancel: function () {
                $scope.currentStep = $scope.StepManager.StepNames.Hierarchy;
                $scope.Reset();
            }
        };

        $scope.enableShowForCurrentStep = function (currentStep) {
            if (currentStep === $scope.StepManager.StepNames.Working) {
                return ($scope.Stakeholder.StkhWorkings.length === 0);
            }

            if (currentStep === $scope.StepManager.StepNames.Payment) {
                return ($scope.WizardData.PaymentList.length === 0);
            }
            //if (currentStep === $scope.StepManager.StepNames.BasicInfo) {
            //    return false;
            //}

            return false;

        };
        //$scope.enablePayment = function () {
        //    if ($scope.currentStep === $scope.StepManager.StepNames.Payment) {
        //        return ($scope.WizardData.PaymentList.length === 0) ? true : false;
        //    }
        //    return false;
        //};

        //#region init
        var restApi = rest.all('AddStakeHolder');

        $scope.init = function () {
            $scope.val = $validations;
            $scope.Stakeholder = {
                StkhPayments: [],
                GAddress: [],
                StkhRegistrations: [],
                StkhWorkings: []
            };
            $scope.WizardData = {};
            $scope.WizardData.SelHierarchy = {
                Hierarchy: '',
                Designation: ''
            };
            $scope.WizardData.LocationLevel = '';
            $scope.WizardData.PaymentList = [];
            $scope.WizardData.EmailId = '';
            // $scope.WizardData.Gender = 0;
            $scope.WizardData.Registration = {};
            $scope.ReportsToList = [];
            $scope.WizardData.LocationLevelArray = [];
            $scope.WizardData.isFromWorking = false;
            $scope.WizardData.isFromPayment = false;
            $scope.WizardData.PaymentForWork = {};

            $scope.WizardData.PayWorkModel = {
                Payment: {},
                WorkList: []
            };
            $scope.WizardData.PayWorkModelList = [];

            $scope.WizardData.finalPostModel = {
                Stakeholders: {},
                PayWork: []
            };
            //if stakeholder is in edit mode
            $scope.isEditMode = false;

            if (!$csfactory.isNullOrEmptyGuid($window.stakeholderId)) {
                $scope.isEditMode = true;
                getStakeholderForEdit($window.stakeholderId);
                $scope.StepManager.PopulateSteps($scope.WizardData.Hierarchy);
            }
            $scope.StepManager.SetDefaultStep();
            $log.info("$stakeholder : intialization done.");
        };

        $scope.resetStakeholder = function () {
            $scope.Stakeholder = {
                StkhPayments: [],
                GAddress: [],
                StkhRegistrations: [],
                StkhWorkings: []
            };
        };

        $scope.resetWizardData = function () {
            $scope.WizardData.LocationLevel = '';
            $scope.WizardData.PaymentList = [];
            $scope.WizardData.EmailId = '';
            $scope.WizardData.Gender = 0;
            $scope.WizardData.Registration = {};
            $scope.ReportsToList = [];
            $scope.WizardData.LocationLevelArray = [];
            $scope.WizardData.Address = {};
            $scope.WizardData.Pincode = '';
        };
        //#endregion

        //get stakeholder for edit
        var getStakeholderForEdit = function (stakeholderId) {
            restApi.customGET('GetStakeholderEditMode', { stakeholderId: stakeholderId }).then(function (data) {
                $log.debug('Stakeholder loaded for edit: ' + data);
                setStakeholderForEdit(data);
                $scope.StepManager.PopulateSteps(data.Hierarchy);
            }, function () {
                $csnotify.error('Error in loading stakeholder for edit');
                $log.error('Error in loading stakeholder for edit');
            });
        };

        //set staekholder for edit mode
        var setStakeholderForEdit = function (stakeholder) {
            $scope.Stakeholder = stakeholder;
            $scope.addStakeholderpanel1 = false;
            $scope.WizardData.LocationLevel = stakeholder.LocationLevel;
            $scope.WizardData.EmailId = stakeholder.EmailId.substring(1, stakeholder.EmailId.indexOf('@'));
            $scope.WizardData.Hierarchy = stakeholder.Hierarchy;
            $scope.WizardData.Registration = stakeholder.StkhRegistrations.length > 0 ? stakeholder.StkhRegistrations[0] : null;
            $scope.WizardData.PaymentList = stakeholder.StkhPayments;
            $scope.WizardData.Address = stakeholder.GAddress;

            //load reports to list
            $scope.getReportsTo(stakeholder.Hierarchy);
        };

        $scope.SaveData = function () {
            //save stakeholder
            if ($scope.WizardData.Hierarchy.Hierarchy != 'External') {
                $scope.Stakeholder.EmailId = $scope.WizardData.EmailId + '@sc.com';
            }

            // assign approver name
            $scope.Stakeholder.ApprovedBy = getApproverName($scope.Stakeholder.ReportsTo);
            $scope.Stakeholder.StkhPayments = assignPayments($scope.WizardData.PaymentList);

            if (angular.isDefined($scope.WizardData.Address) && $scope.WizardData.Hierarchy.HasAddress === true) {
                $scope.Stakeholder.GAddress.push($scope.WizardData.Address);
            }

            if (checkRegistration($scope.WizardData.Registration)) {
                $scope.Stakeholder.StkhRegistrations.push($scope.WizardData.Registration);
            }

            $scope.Stakeholder.Hierarchy = $scope.WizardData.Hierarchy;
            $scope.Stakeholder.LocationLevel = $scope.WizardData.LocationLevel;

            $scope.WizardData.finalPostModel.Stakeholders = $scope.Stakeholder;
            $scope.WizardData.finalPostModel.PayWork = $scope.WizardData.PayWorkModelList;
            $scope.WizardData.finalPostModel.Hierarchy = $scope.WizardData.Hierarchy;

            $log.info($scope.Stakeholder);
            restApi.customPOST($scope.WizardData.finalPostModel, 'SaveStakeholder').then(function () {
                $scope.StepManager.Cancel();
                $csnotify.success('Stakeholder Saved');
            }, function () {
            });
        };

        var checkRegistration = function (registration) {
            if (!($csfactory.isNullOrEmptyString(registration.TanNo))) {
                return true;
            } else if (!($csfactory.isNullOrEmptyString(registration.PanNo))) {
                return true;
            } else if (!($csfactory.isNullOrEmptyString(registration.RegistrationNo))) {
                return true;
            } else {
                return false;
            }
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
            var appname = _.find($scope.ReportsToList, { 'Id': approverId });
            if (angular.isDefined(appname)) {
                return appname.Name;
            }
            return '';
        };

        $scope.Reset = function () {
            $scope.Stakeholder.FixpayBasic = null;
            $scope.WizardData.SelHierarchy = {};
            $scope.Stakeholder = {};
            $scope.WizardData.Registration = {};
            $scope.WizardData.PaymentList = [];
            $scope.WizardData.EmailId = null;
            $scope.WizardData.Address = {};
            $scope.WizardData.Pincode = '';

        };

        $scope.getReportsTo = function (hierarchy) {

            if (angular.isUndefined(hierarchy)) {
                return;
            }
            restApi.customGET('GetReportsToInHierarchy', { reportsto: hierarchy.ReportsTo })
                   .then(function (data) { $scope.ReportsToList = data; });
        };

    }]);


