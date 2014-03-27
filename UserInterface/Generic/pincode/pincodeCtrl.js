csapp.factory("pincodeDataLayer", ["Restangular", "$csnotify", "$csfactory", function (rest, $csnotify, $csfactory) {
    var dldata = {};

    var pincodeApi = rest.all('PincodeApi');

    var cityCategoty = function () {
        pincodeApi.customGET('GetCityCategory').then(function (data) {
            dldata.CityCategoryList = data;
        });
    };

    var showErrorMessage = function (response) {
        $csnotify.error(response.data);
    };

    var getData = function () {
        if (dldata.isInEditMode == true) {
            return;
        }
        if (angular.isDefined(dldata.GPincodedata)) {
            dldata.GPincodedata = {
                Country: 'India',
                Region: dldata.GPincodedata.Region,
                State: dldata.GPincodedata.State,
                Cluster: dldata.GPincodedata.Cluster,
                District: dldata.GPincodedata.District,
                City: dldata.GPincodedata.City
            };
        }
        if (angular.isUndefined(dldata.GPincodedata)) {
            dldata.GPincodedata = {};
            dldata.GPincodedata.Country = 'India';
        }
        //#region Set value in Object

        //#endregion

        var gPincodedata = dldata.GPincodedata;
        pincodeApi.customPOST(gPincodedata, "GetWholedata").then(function (data2) {
            if (angular.isUndefined(gPincodedata.Region) || gPincodedata.Region == "") {
                dldata.RegionList = data2;
                return;
            }

            if (angular.isUndefined(gPincodedata.State) || gPincodedata.State == "") {
                dldata.StateList = data2;
                return;
            }

            if (angular.isUndefined(gPincodedata.Cluster) || gPincodedata.Cluster == "") {
                dldata.ClusterList = data2;
                return;
            }

            if (angular.isUndefined(gPincodedata.District) || gPincodedata.District == "") {
                dldata.DistrictList = data2;
                return;
            }

            if (angular.isUndefined(gPincodedata.City) || gPincodedata.City == "") {
                dldata.CityList = data2;
                dldata.citydata = true;
                return;
            }

        }, showErrorMessage);

    };

    var pincodeCity = function (city, district) {
        if (city.length < 2) {
            return [];
        }
        return pincodeApi.customGET('GetPincodeCity', { city: city, district: district }).then(function (data) {
            return data;
        });
    };

    var pincodeArea = function (pincode, level) {
        if (pincode.length < 2) {
            return [];
        }
        return pincodeApi.customGET('GetPincodesArea', { area: pincode, city: level }).then(function (data) {
            console.log(data);
            return data;
        });
    };

    var missingPincode = function (pincode) {
        if (pincode.length < 3) {
            return [];
        }
        return pincodeApi.customGET('GetMissingPincodes', { pincode: pincode }).then(function (data) {
            return data;
        });
    };

    var getState = function () {
        return pincodeApi.customGETLIST("GetStates").then(function (data) {
            dldata.States = data;
        }, showErrorMessage);
    };

    var getWholePincode = function () {
        if (!$csfactory.isNullOrEmptyArray(dldata.PincodeUintList)) {
            return;
        }

        pincodeApi.customGET('GetWholePincode').then(function (data) {
            dldata.PincodeUintList = data;
        });
    };

    var changeState = function (stateName) {
        if ($csfactory.isNullOrEmptyString(stateName)) {
            return;
        }
        pincodeApi.customGETLIST("GetPincodes", { state: stateName }).then(function (data) {
            dldata.GPincodes = data;
            dldata.stateClusters = _.uniq(_.pluck(dldata.GPincodes, 'Cluster'));
        }, showErrorMessage);

    };

    var editPincode = function (gpincode) {
        if (dldata.isInEditMode === true) {
            return pincodeApi.customPUT(gpincode, "Put", { id: gpincode.Id }).then(function (data) {
                dldata.GPincodes = _.reject(dldata.GPincodes, function (pincode) { return pincode.Id == gpincode.Id; });
                dldata.GPincodes.push(data);
                dldata.PincodeUintList.push(data);
                dldata.showTextBox = false;
                dldata.isInEditMode = false;
                $csnotify.success("Data saved");
                return;
            }, showErrorMessage);
        } else {
            return pincodeApi.customPOST(gpincode, 'Post').then(function (data) {
                $csnotify.success("Pincode saved..!!");
                dldata.GPincodes.push(data);
                dldata.PincodeUintList.push(data);
                return;
            }, function (data) {
                $csnotify.error("Error occured, can't saved", data);
            });
        }
    };

    return {
        dldata: dldata,
        cityCategoty: cityCategoty,
        getData: getData,
        pincodeCity: pincodeCity,
        pincodeArea: pincodeArea,
        missingPincode: missingPincode,
        getState: getState,
        getWholePincode: getWholePincode,
        changeState: changeState,
        editPincode: editPincode
    };
}]);

csapp.factory("pincodeFactory", ["pincodeDataLayer",
    function (datalayer) {

        var dldata = datalayer.dldata;
        var regionChange = function () {
            if (angular.isDefined(dldata.GPincodedata.State)) {
                dldata.StateList = [];
                dldata.ClusterList = [];
                dldata.DistrictList = [];
                dldata.CityList = [];
                dldata.CityCategoryList = [];
                dldata.GPincodedata.Area = '';
                dldata.showTextBox = false;
                dldata.GPincodedata.Pincode = '';
            }
        };

        var stateChange = function () {
            if (angular.isDefined(dldata.GPincodedata.Cluster)) {
                dldata.ClusterList = [];
                dldata.DistrictList = [];
                dldata.CityList = [];
                dldata.CityCategoryList = [];
                dldata.citydata = false;
                dldata.GPincodedata.Area = '';
                dldata.GPincodedata.Pincode = '';
            }
        };

        var clusterChange = function () {
            if (angular.isDefined(dldata.GPincodedata.District)) {
                dldata.DistrictList = [];
                dldata.CityList = [];
                dldata.CityCategoryList = [];
                dldata.citydata = false;
                dldata.GPincodedata.Area = '';
                dldata.GPincodedata.Pincode = '';
            }
        };

        var districtChange = function () {
            if (angular.isDefined(dldata.GPincodedata.City)) {
                dldata.CityList = [];
                dldata.CityCategoryList = [];
                dldata.GPincodedata.Area = '';
                dldata.GPincodedata.Pincode = '';
            }
            dldata.showTextBox = false;
            dldata.citydata = true;
        };

        var cityChange = function () {
            if (angular.isDefined(dldata.GPincodedata.Area)) {
                dldata.GPincodedata.Area = '';
                dldata.GPincodedata.Pincode = '';
            }
        };

        var areaChange = function () {
            if (angular.isDefined(dldata.GPincodedata.Pincode)) {
                dldata.GPincodedata.Pincode = '';
            }
        };

        var addNewCity = function () {
            dldata.showTextBox = true;
            dldata.GPincodedata.City = '';

        };

        var cancleAddCity = function () {
            dldata.showTextBox = false;
            dldata.GPincodedata.City = '';
        };

        var pincodedata = function (pincode) {
            dldata.alreadyExit = false;
            if (dldata.isInEditMode == true) {
                return;
            }
            if (pincode.length === 6) {
                var isExist = _.find(dldata.PincodeUintList, function (item) {
                    return item == pincode;
                });
                if (angular.isDefined(isExist)) {
                    dldata.alreadyExit = true;
                    dldata.dummyPincode = dldata.GPincodedata.Pincode;
                    return;
                }
            }
        };

        var clearSearchTextbox = function () {
            dldata.search = {};
        };

        var reset = function () {
            dldata.RegionList = [];
            dldata.StateList = [];
            dldata.ClusterList = [];
            dldata.DistrictList = [];
            dldata.CityList = [];
            dldata.CityCategoryList = [];
            dldata.GPincodedata.Area = '';
            dldata.showTextBox = false;
            dldata.GPincodedata.Pincode = '';
        };

        return {
            regionChange: regionChange,
            stateChange: stateChange,
            clusterChange: clusterChange,
            districtChange: districtChange,
            cityChange: cityChange,
            areaChange: areaChange,
            addNewCity: addNewCity,
            cancleAddCity: cancleAddCity,
            pincodedata: pincodedata,
            clearSearchTextbox: clearSearchTextbox,
            reset: reset
        };
    }]);

csapp.controller("pincodeCtrl", ["$scope", "pincodeDataLayer", "pincodeFactory", "$modal", "$Validations",
    function ($scope, datalayer, factory, $modal, $Validation) {
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.dldata.val = $Validation;
            $scope.dldata.States = [];
            $scope.dldata.PincodeUintList = [];
            datalayer.cityCategoty();
            datalayer.getState();
            datalayer.getWholePincode();
            $scope.factory = factory;

        })();

        $scope.openAddModel = function () {
            $scope.dldata.isInEditMode = false;
            datalayer.getData();
            $modal.open({
                templateUrl: '/Generic/pincode/pincode-modal.html',
                controller: 'pincodeModalController',
            });
        };

        $scope.openEditModal = function (gPincode) {
            $scope.dldata.isInEditMode = true;
            datalayer.getData();
            $modal.open({
                templateUrl: '/Generic/pincode/editPincode-modal.html',
                controller: 'editPincodeModalController',
                resolve: {
                    gPincode: function () {
                        return gPincode;
                    }
                }
            });
        };
    }]);

csapp.controller("pincodeModalController", ["$scope", "pincodeDataLayer", "pincodeFactory", "$modalInstance",
    function ($scope, datalayer, factory, $modalInstance) {
        $scope.save = function () {
            datalayer.editPincode($scope.dldata.GPincodedata).then(function () {
                $scope.closeAddModal();
            });
        };

        $scope.closeAddModal = function () {
            $modalInstance.dismiss();
            factory.reset();
        };
        $scope.reset = function () {
            factory.reset();
        };
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.factory = factory;
            datalayer.cityCategoty();

        })();
    }]);

csapp.controller("editPincodeModalController", ["$scope", "pincodeDataLayer", "pincodeFactory", "$modalInstance", "gPincode",
    function ($scope, datalayer, factory, $modalInstance, gPincode) {

        $scope.closeEditModel = function () {
            $modalInstance.dismiss();
        };
        $scope.reset = function () {
            factory.reset();
        };

        $scope.save = function () {
            datalayer.editPincode($scope.eGPincode).then(function () {
                $scope.closeEditModel();
            });

        };
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.factory = factory;
            $scope.eGPincode = gPincode;
            datalayer.cityCategoty();

        })();
    }]);