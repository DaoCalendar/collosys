csapp.controller('EditStake', ['$scope', '$http', 'Restangular', '$Validations', '$log', '$timeout',
    '$csfactory', '$csnotify', 'csConstants',
    function ($scope, $http, rest, $validations, $log, $timeout, $csfactory, $csnotify, csConstants) {
        var restApi = rest.all('StakeholderEdit');


        var getStakeholderForEdit = function () {
             restApi.customGET('GetStakeholderEditMode').then(function (data) {
                $log.debug('Stakeholder loaded for edit: ' + data);
                debugger;
                $scope.Hierarchy = data.Hierarchy;
                $scope.Stakeholder = data;
            }, function (data) {
                $csnotify.error('Error in loading stakeholder for edit');
                $log.error('Error in loading stakeholder for edit');
            });
        };

        var init = function () {
             getStakeholderForEdit();
            //Panel 
            $scope.basicInfopanel1 = true;
            $scope.registrationpanel2 = false;
            $scope.paymentDetailspanel3 = false;
            $scope.paymentWorkingpanel4 = false;
            $scope.addressDetails5 = false;
            
        };
        $scope.Next = function (val) {
            switch (val) {
                //case 1:
                //    $scope.addStakeholderpanel1 = true;
                //    $scope.basicInfopanel1 = false;
                //    $scope.registrationpanel2 = false;
                //    $scope.paymentDetailspanel3 = false;

                //    break;

                case 2:
                    $scope.basicInfopanel1 = true;
                    $scope.registrationpanel2 = false;
                    $scope.paymentDetailspanel3 = false;
                    $scope.paymentWorkingpanel4 = false;
                    $scope.addressDetails5 = false;
                    break;

                case 3:
                    $scope.basicInfopanel1 = false;
                    $scope.registrationpanel2 = true;
                    $scope.paymentDetailspanel3 = false;
                    $scope.paymentWorkingpanel4 = false;
                    $scope.addressDetails5 = false;
                    break;
                case 4:
                    $scope.basicInfopanel1 = false;
                    $scope.registrationpanel2 = false;
                    $scope.paymentDetailspanel3 = true;
                    $scope.paymentWorkingpanel4 = false;
                    $scope.addressDetails5 = false;
                    break;
                case 5:
                    $scope.basicInfopanel1 = false;
                    $scope.registrationpanel2 = false;
                    $scope.paymentDetailspanel3 = false;
                    $scope.paymentWorkingpanel4 = true;
                    $scope.addressDetails5 = false;
                    break;
                case 6:
                    $scope.basicInfopanel1 = false;
                    $scope.registrationpanel2 = false;
                    $scope.paymentDetailspanel3 = false;
                    $scope.paymentWorkingpanel4 = false;
                    $scope.addressDetails5 = true;
                default:
            }

        };

        $scope.Prev = function (val) {
            switch (val) {

                //case 1:
                //    $scope.addStakeholderpanel1 = true;
                //    $scope.basicInfopanel1 = false;
                //    $scope.registrationpanel2 = false;
                //    $scope.paymentDetailspanel3 = false;

                //    break;

                case 2:
                    $scope.addStakeholderpanel1 = true;
                    $scope.basicInfopanel1 = false;
                    $scope.registrationpanel2 = false;
                    $scope.paymentDetailspanel3 = false;
                    $scope.paymentWorkingpanel4 = false;
                    $scope.addressDetails5 = false;
                    break;

                case 3:
                    $scope.addStakeholderpanel1 = false;
                    $scope.basicInfopanel1 = true;
                    $scope.registrationpanel2 = false;
                    $scope.paymentDetailspanel3 = false;
                    $scope.paymentWorkingpanel4 = false;
                    $scope.addressDetails5 = false;
                    break;
                case 4:
                    $scope.addStakeholderpanel1 = false;
                    $scope.basicInfopanel1 = false;
                    $scope.registrationpanel2 = true;
                    $scope.paymentDetailspanel3 = false;
                    $scope.paymentWorkingpanel4 = false;
                    $scope.addressDetails5 = false;
                    break;

                case 5:
                    $scope.addStakeholderpanel1 = false;
                    $scope.basicInfopanel1 = false;
                    $scope.registrationpanel2 = false;
                    $scope.paymentDetailspanel3 = true;
                    $scope.paymentWorkingpanel4 = false;
                    $scope.addressDetails5 = false;
                    break;
                default:
            }
        };

        $scope.Cancel = function () {
            init();
        };
        init();
    }]);

/*
 $scope.steps = ['basic', 'payment', 'registration','working'];
    $scope.step = 0;
    $scope.wizard = { tacos: 2 };

    $scope.isFirstStep = function () {
        return $scope.step === 0;
    };

    $scope.isLastStep = function () {
        return $scope.step === ($scope.steps.length - 1);
    };

    $scope.isCurrentStep = function (step) {
        return $scope.step === step;
    };

    $scope.setCurrentStep = function (step) {
        $scope.step = step;
    };

    $scope.getCurrentStep = function () {
        return $scope.steps[$scope.step];
    };

    $scope.getNextLabel = function () {
        return ($scope.isLastStep()) ? 'Submit' : 'Next';
    };

    $scope.handlePrevious = function () {
        $scope.step -= ($scope.isFirstStep()) ? 0 : 1;
    };

    $scope.handleNext = function (dismiss) {
        if ($scope.isLastStep()) {
            //save stakeholder
        } else {
            $scope.step += 1;
        }
    };
*/