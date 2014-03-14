/*global csapp*/

(
csapp.controller("pincodeCtrl", ["$scope", '$http', "$csnotify", "Restangular", "$Validations", function ($scope, $http, $csnotify, rest, $validation) {
    "use strict";
    var pincodeApi = rest.all('PincodeApi');
    var init = function () {
        $scope.val = $validation;
        $scope.GPincodes = [];
        $scope.GPincodedata = {};
        $scope.GPincodedata = [];
        $scope.stateClusters = [];
        $scope.eGPincode = {};
        $scope.showTextBox = false;
        $scope.shouldBeOpen = false;
        $scope.alreadyExit = false;
        $scope.citydata = false;
        $scope.ClusterList = [];
        $scope.StateList = [];
        $scope.DistrictList = [];
        $scope.RegionList = [];
        $scope.CityList = [];
        $scope.PincodeUintList = [];
        $scope.CityCategoryList = [];
        cityCategoty();


    };

    var cityCategoty = function () {
        pincodeApi.customGET('GetCityCategory').then(function (data) {
            $scope.CityCategoryList = data;
        });
    };
    var showErrorMessage = function (response) {
        $csnotify.error(response.data);
    };

    //#region Reset Chile_Combo onClick of Parent_Combo

    $scope.regionChange = function () {
        if (angular.isDefined($scope.GPincodedata.State)) {
            $scope.GPincodedata.State = '';
            $scope.GPincodedata.Cluster = '';
            $scope.GPincodedata.District = '';
            $scope.GPincodedata.City = '';
            $scope.citydata = false;
            $scope.GPincodedata.Area = '';
            $scope.GPincodedata.Pincode = '';
        }
    };

    $scope.stateChange = function () {
        if (angular.isDefined($scope.GPincodedata.Cluster)) {
            $scope.GPincodedata.Cluster = '';
            $scope.GPincodedata.District = '';
            $scope.GPincodedata.City = '';
            $scope.citydata = false;
            $scope.GPincodedata.Area = '';
            $scope.GPincodedata.Pincode = '';
        }
    };
    $scope.clusterChange = function () {
        if (angular.isDefined($scope.GPincodedata.District)) {
            $scope.GPincodedata.District = '';
            $scope.GPincodedata.City = '';
            $scope.citydata = false;
            $scope.GPincodedata.Area = '';
            $scope.GPincodedata.Pincode = '';
        }
    };
    $scope.districtChange = function () {
        if (angular.isDefined($scope.GPincodedata.City)) {
            $scope.GPincodedata.City = '';
            $scope.GPincodedata.Area = '';
            $scope.GPincodedata.Pincode = '';
        }
    };
    $scope.cityChange = function () {
        if (angular.isDefined($scope.GPincodedata.Area)) {
            $scope.GPincodedata.Area = '';
            $scope.GPincodedata.Pincode = '';
        }
    };
    $scope.areaChange = function () {
        if (angular.isDefined($scope.GPincodedata.Pincode)) {
            $scope.GPincodedata.Pincode = '';
        }
    };

    //#endregion

    $scope.getData = function () {
        if ($scope.isInEditMode == true) {
            return;
        }
        //#region Set value in Object
        $scope.GPincodedata = {
            Country: 'India',
            Region: $scope.GPincodedata.Region,
            State: $scope.GPincodedata.State,
            Cluster: $scope.GPincodedata.Cluster,
            District: $scope.GPincodedata.District,
            City: $scope.GPincodedata.City
        };
        //#endregion

        var gPincodedata = $scope.GPincodedata;
        pincodeApi.customPOST(gPincodedata, "GetWholedata").then(function (data2) {
            if (angular.isUndefined(gPincodedata.Region) || gPincodedata.Region == "") {
                $scope.RegionList = data2;
                return;
            }

            if (angular.isUndefined(gPincodedata.State) || gPincodedata.State == "") {
                $scope.StateList = data2;
                return;
            }

            if (angular.isUndefined(gPincodedata.Cluster) || gPincodedata.Cluster == "") {
                $scope.ClusterList = data2;
                return;
            }

            if (angular.isUndefined(gPincodedata.District) || gPincodedata.District == "") {
                $scope.DistrictList = data2;
                return;
            }

            if (angular.isUndefined(gPincodedata.City) || gPincodedata.City == "") {
                $scope.CityList = data2;
                $scope.citydata = true;
                return;
            }

        }, showErrorMessage);

    };


    $scope.pincodeCity = function (city, district) {
        if (city.length < 2) {
            return [];
        }
        return pincodeApi.customGET('GetPincodeCity', { city: city, district: district }).then(function (data) {
            return data;
        });
    };

    $scope.pincodeArea = function (pincode, level) {
        if (pincode.length < 2) {
            return [];
        }
        return pincodeApi.customGET('GetPincodesArea', { area: pincode, city: level }).then(function (data) {
            console.log(data);
            return data;
        });
    };

    $scope.missingPincode = function (pincode) {
        if (pincode.length < 3) {
            return [];
        }
        return pincodeApi.customGET('GetMissingPincodes', { pincode: pincode }).then(function (data) {
            console.log(data);
            return data;
        });
    };


    pincodeApi.customGETLIST("GetStates").then(function (data) {
        $scope.States = data;
    }, showErrorMessage);

    pincodeApi.customGET('GetWholePincode').then(function (data) {
        console.log(data);
        $scope.PincodeUintList = data;
    });

    $scope.changeState = function (stateName) {
        pincodeApi.customGETLIST("GetPincodes", { state: stateName }).then(function (data) {
            $scope.GPincodes = data;
            $scope.stateClusters = _.uniq(_.pluck($scope.GPincodes, 'Cluster'));
        }, showErrorMessage);

    };

    //#region ModalPopUp Function

    $scope.openModel = function (gpincode) {
        $scope.isInEditMode = true;
        $scope.eGPincode = angular.copy(gpincode);
        $scope.shouldBeOpen = true;
    };

    $scope.closeModel = function () {
        if ($scope.isInEditMode == false) {
            $scope.shouldBeOpenAdd = false;
            $scope.showTextBox = false;
        }
        if ($scope.isInEditMode == true) {
            $scope.shouldBeOpen = false;
        }
        $scope.reset();
    };

    $scope.reset = function () {
        if ($scope.isInEditMode == true) {
            $scope.eGPincode.Region = '';
            $scope.eGPincode.State = '';
            $scope.eGPincode.Cluster = '';
            $scope.eGPincode.District = '';
            $scope.eGPincode.City = '';
            $scope.eGPincode.Area = '';
            $scope.eGPincode.Pincode = '';

        } else {
            $scope.GPincodedata.Region = '';
            $scope.GPincodedata.State = '';
            $scope.GPincodedata.Cluster = '';
            $scope.GPincodedata.District = '';
            $scope.GPincodedata.City = '';
            $scope.GPincodedata.Area = '';
            $scope.showTextBox = false;
            $scope.GPincodedata.Pincode = '';
        }

    };

    $scope.openAddModal = function () {
        $scope.isInEditMode = false;
        $scope.shouldBeOpen = false;
        $scope.shouldBeOpenAdd = true;
        $scope.getData();

    };

    $scope.modelOption = {
        backdropFade: true,
        dialogFade: true
    };

    //#endregion

    $scope.addNewCity = function (city) {
        $scope.showTextBox = true;
        $scope.GPincodedata.City = '';

    };

    $scope.cancleAddCity = function () {
        $scope.showTextBox = false;
        $scope.GPincodedata.City = '';
    };

    $scope.pincodedata = function (pincode) {
        debugger;
        $scope.alreadyExit = false;
      if ($scope.isInEditMode == true) {
            return;
        }
        if (pincode.length === 6) {
            var isExist = _.find($scope.PincodeUintList, function (item) {
                return item == pincode;
            });
            if (angular.isDefined(isExist)) {
                $scope.alreadyExit = true;
                $scope.dummyPincode = $scope.GPincodedata.Pincode;
                return;
            } 
        }
    };
    $scope.clearSearchTextbox = function () {
        $scope.search = {};
    };

    $scope.editPincode = function (gpincode) {

        $scope.eGPincode.Country = 'India';
        if ($scope.isInEditMode === true) {

            pincodeApi.customPUT(gpincode, "Put", { id: gpincode.Id }).then(function (data) {
                $scope.GPincodes = _.reject($scope.GPincodes, function (pincode) { return pincode.Id == gpincode.Id; });
                $scope.GPincodes.push(data);
                $scope.shouldBeOpen = false;
                $scope.showTextBox = false;
                $scope.clearSearchTextbox();
                $scope.GPincodes = [];
                $scope.selectedState = '';
                $csnotify.success("Data saved");
            }, showErrorMessage);
        } else {
            pincodeApi.customPOST(gpincode, 'Post').then(function () {
                $csnotify.success("Pincode saved..!!");
                $scope.GPincodes = [];
                $scope.selectedState = '';
                $scope.shouldBeOpenAdd = false;
                $scope.closeModel();
            }, function (data) {
                $csnotify.error("Error occured, can't saved", data);
            });
        }
    };

    init();
}])
);
