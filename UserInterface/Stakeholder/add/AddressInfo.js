(
csapp.controller('addressCntrl', ['$scope', '$http', 'Restangular', '$csfactory', '$csnotify', '$log',
    function ($scope, $http, rest, $csfactory, $csnotify, $log) {
        'use strict';

        var apistake = rest.all('StakeholderApi');
        var init = function () {
            $scope.addData = {
                Pincode: '',
                Hierarchy: $scope.$parent.WizardData.GetHierarchy()
            };
            //if (($scope.addData.Hierarchy.Designation != null) && ($scope.addData.Hierarchy.HasAddress == true)) {
            //     if (angular.isDefined($scope.$parent.WizardData.FinalPostModel.Address)) {
            //    $scope.addData.Pincode = $scope.$parent.WizardData.FinalPostModel.Address.Pincode;
            //}
            //}
            if (angular.isDefined($scope.$parent.WizardData.FinalPostModel.Address)) {
                $scope.addData.Pincode = $scope.$parent.WizardData.FinalPostModel.Address.Pincode;
            }

            if ($scope.$parent.WizardData.IsEditMode() === true) {
                $scope.addData.Pincode = $scope.WizardData.GetPincode();
            }
        };
        $scope.Pincodes = function (pincode, level) {
            if ($csfactory.isNullOrEmptyString(pincode)) {
                return [];
            }
            // harish - add number check
            if (pincode.length < 3) {
                return [];
            }
            return apistake.customGET('GetPincodes', { pincode: pincode, level: level }).then(function (data) {
                return $scope.PincodeList = data;
            });
        };

        $scope.setPincodeOnArea = function (areaname) {
            var data = _.find($scope.PincodeList, { 'Area': areaname });
            if (!angular.isUndefined(data)) {
                $scope.$parent.WizardData.FinalPostModel.Address.Pincode = data.Pincode;
                $scope.$parent.WizardData.FinalPostModel.Address.Country = 'India';
                $scope.$parent.WizardData.FinalPostModel.Address.StateCity = data.State + '/' + data.City;
            } else {
                $scope.$parent.WizardData.FinalPostModel.Address.Pincode = undefined;
            }

        };
        init();
    }])
);


