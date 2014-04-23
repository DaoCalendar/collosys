csapp.controller("pincodeCtrl", ["$scope", "pincodeDataLayer", "$modal", "$csGenericModels",
    function ($scope, datalayer, $modal, $csGenericModels) {
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.dldata.Regions = [];
            $scope.dldata.States = [];
            $scope.dldata.Clusters = [];
            $scope.dldata.Districts = [];
            $scope.dldata.City = [];
            datalayer.getRegion();
            datalayer.getState();
            datalayer.getCluster();
            datalayer.getDistrict();
            datalayer.getCity();
            datalayer.getWholePincode();
            $scope.eGPincodeModel = $csGenericModels.models.Pincode;
            $scope.dldata.PincodeUintList = [];
        })();

        $scope.changeState = function (stateName) {
            datalayer.changeState(stateName);
        };
        $scope.getRegion = function () {
            datalayer.getData().then(function () {
                $scope.eGPincodeModel.Region.valueList = datalayer.dldata.RegionList;
            });
        };

        $scope.openAddEditModal = function (mode, gPincodes) {
            $scope.getRegion();
            $modal.open({
                templateUrl: '/Generic/pincode/editPincode-modal.html',
                controller: 'editPincodeModalController',
                resolve: {
                    gPincodes: function () {
                        return {
                            gpincode: angular.copy(gPincodes),
                            displaymode: mode
                        };
                    }
                }

            });
        };
    }]);


csapp.factory("pincodeDataLayer", ["Restangular", "$csnotify", "$csfactory",
    function (rest, $csnotify, $csfactory) {
        var dldata = {};

        var pincodeApi = rest.all('PincodeApi');

        var showErrorMessage = function (response) {
            $csnotify.error(response.data);
        };

        var getState = function () {
            return pincodeApi.customGETLIST("GetStates").then(function (data) {
                dldata.States = data;
            }, showErrorMessage);
        };

        var getRegion = function () {
            return pincodeApi.customGETLIST("GetRegions").then(function (data) {
                dldata.Regions = data;
            }, showErrorMessage);
        };

        var getCluster = function () {
            return pincodeApi.customGETLIST("GetClusters").then(function (data) {
                dldata.Clusters = data;
            }, showErrorMessage);
        };

        var getDistrict = function () {
            return pincodeApi.customGETLIST("GetDistricts").then(function (data) {
                dldata.Districts = data;
            }, showErrorMessage);
        };
        var getCity = function () {
            return pincodeApi.customGETLIST("GetCity").then(function (data) {
                dldata.City = data;
            }, showErrorMessage);
        };

        var changeState = function (stateName) {
            pincodeApi.customGETLIST("GetPincodes", { state: stateName }).then(function (data) {
                dldata.GPincodes = data;
                dldata.stateClusters = _.uniq(_.pluck(dldata.GPincodes, 'Cluster'));
                $csnotify.success("Pincodes loaded successfully");
            }, showErrorMessage);

        };

        var getData = function () {

            if (angular.isUndefined(dldata.GPincodedata)) {
                dldata.GPincodedata = {};
                dldata.GPincodedata.Country = 'India';
            }

            //#region Set value in Object

            //#endregion

            var gPincodedata = dldata.GPincodedata;

            return pincodeApi.customPOST(gPincodedata, "GetWholedata").then(function (data2) {
                if (angular.isUndefined(gPincodedata.Region) || gPincodedata.Region == "") {
                    dldata.RegionList = _.uniq(data2);
                    return;
                }
            }, showErrorMessage);
        };


        var getStateData = function (region) {
            var pincodeData = { Country: 'India', Region: region };
            return pincodeApi.customPOST(pincodeData, "GetWholedata").then(function (data2) {
                dldata.StateList = _.uniq(data2);
                return dldata.StateList;
            });
        };

        var getClusterData = function (region, state) {
            var pincodeData = { Country: 'India', Region: region, State: state };
            return pincodeApi.customPOST(pincodeData, "GetWholedata").then(function (data2) {
                dldata.ClusterList = _.uniq(data2);
                return dldata.ClusterList;
            });
        };

        var getDistrictData = function (region, state, cluster) {
            var pincodeData = { Country: 'India', Region: region, State: state, Cluster: cluster };
            return pincodeApi.customPOST(pincodeData, "GetWholedata").then(function (data2) {
                dldata.DistrictList = _.uniq(data2);
                return dldata.DistrictList;
            });
        };

        var getCityData = function (region, state, cluster, district) {
            var pincodeData = { Country: 'India', Region: region, State: state, Cluster: cluster, District: district };
            return pincodeApi.customPOST(pincodeData, "GetWholedata").then(function (data2) {
                dldata.CityList = _.uniq(data2);
                dldata.citydata = true;
                return dldata.CityList;
            });
        };


        var missingPincode = function (pincode) {
            return pincodeApi.customGET('GetMissingPincodes', { pincode: pincode }).then(function (data) {
                return data;
            });
        };


        var getWholePincode = function () {
            if (!$csfactory.isNullOrEmptyArray(dldata.PincodeUintList)) {
                return;
            }
            pincodeApi.customGET('GetWholePincode').then(function (data) {
                dldata.PincodeUintList = data;
            });
        };

        var pincodeArea = function (value, level) {
            return pincodeApi.customGET('GetPincodesArea', { area: value, city: level }).then(function (data) {
                return data;
            });
        };

        var pincodeCity = function (city, district) {
            return pincodeApi.customGET('GetPincodeCity', { city: city, district: district }).then(function (data) {
                return data;
            });
        };

        var editPincode = function (gpincode, mode) {
            if (mode === "edit") {
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
                    return data;
                }, function (data) {
                    $csnotify.error("Error occured, can't saved", data);
                });
            }
        };

        return {
            dldata: dldata,
            getRegion: getRegion,
            getState: getState,
            getCluster: getCluster,
            getDistrict: getDistrict,
            changeState: changeState,
            getCity: getCity,
            getData: getData,
            getStateData: getStateData,
            getClusterData: getClusterData,
            getDistrictData: getDistrictData,
            getCityData: getCityData,
            getWholePincode: getWholePincode,
            missingPincode: missingPincode,
            pincodeArea: pincodeArea,
            pincodeCity:pincodeCity,
            editPincode: editPincode,
        };

    }]);


csapp.factory("pincodeFactory", ["pincodeDataLayer",
    function (datalayer) {

        var dldata = datalayer.dldata;
        var regionChange = function () {
            if (angular.isDefined(dldata.GPincodedata.State)) {
                dldata.GPincodedata.State = '';
                dldata.GPincodedata.Cluster = '';
                dldata.GPincodedata.District = '';
                dldata.GPincodedata.City = '';
                dldata.GPincodedata.CityCategory = '';
                dldata.GPincodedata.Area = '';
                dldata.showTextBox = false;
                dldata.GPincodedata.Pincode = '';
            }
        };

        var stateChange = function () {
            if (angular.isDefined(dldata.GPincodedata.Cluster)) {
                dldata.GPincodedata.Cluster = '';
                dldata.GPincodedata.District = '';
                dldata.GPincodedata.City = '';
                dldata.GPincodedata.CityCategory = '';
                dldata.GPincodedata.citydata = false;
                dldata.GPincodedata.Area = '';
                dldata.GPincodedata.Pincode = '';
            }
        };

        var clusterChange = function () {
            if (angular.isDefined(dldata.GPincodedata.District)) {
                dldata.GPincodedata.District = '';
                dldata.GPincodedata.City = '';
                dldata.GPincodedata.CityCategory = '';
                dldata.GPincodedata.citydata = false;
                dldata.GPincodedata.Area = '';
                dldata.GPincodedata.Pincode = '';
            }
        };

        var districtChange = function () {
            if (angular.isDefined(dldata.GPincodedata.City)) {
                dldata.GPincodedata.City = '';
                dldata.GPincodedata.CityCategory = '';
                dldata.GPincodedata.Area = '';
                dldata.GPincodedata.Pincode = '';
            }
            //dldata.showTextBox = false;
            //dldata.citydata = true;
        };

        var areaChange = function () {
            if (angular.isDefined(dldata.GPincodedata.Pincode)) {
                dldata.GPincodedata.Pincode = '';
            }
        };

        var pincodedata = function (pincode) {
            var isExist = _.find(dldata.PincodeUintList, function (item) {
                return item == pincode;
            });
            if (angular.isDefined(isExist)) {
                dldata.alreadyExit = true;
                dldata.dummyPincode = dldata.GPincodedata.Pincode;
                return;
            }
        };

        var reset = function (gpincode) {
            gpincode.Region = '';
            gpincode.State = '';
            gpincode.Cluster = '';
            gpincode.District = '';
            gpincode.City = '';
            gpincode.CityCategory = '';
            gpincode.Area = '';
            gpincode.Pincode = '';
        };

        return {
            regionChange: regionChange,
            stateChange: stateChange,
            clusterChange: clusterChange,
            pincodedata: pincodedata,
            districtChange: districtChange,
            areaChange: areaChange,
            reset: reset
        };
    }]);


csapp.controller("editPincodeModalController", ["$scope", "pincodeDataLayer", "$modalInstance", "gPincodes",
    "$csGenericModels", "pincodeFactory",
    function ($scope, datalayer, $modalInstance, gPincodes, $csGenericModels, factory) {
        (function () {
            $scope.GPincodedata = {};
            $scope.eGPincodeModel = $csGenericModels.models.Pincode;
            if (gPincodes.displaymode === 'edit') {
                $scope.GPincodedata = gPincodes.gpincode;
                $scope.GPincodedata.Region = $scope.GPincodedata.Region.toUpperCase();
                $scope.eGPincodeModel.Region.valueList = datalayer.dldata.Regions;
                $scope.GPincodedata.State = $scope.GPincodedata.State.toUpperCase();
                $scope.eGPincodeModel.State.valueList = datalayer.dldata.States;
                $scope.GPincodedata.Cluster = $scope.GPincodedata.Cluster.toUpperCase();
                $scope.eGPincodeModel.Cluster.valueList = datalayer.dldata.Clusters;
                $scope.GPincodedata.District = $scope.GPincodedata.District.toUpperCase();
                $scope.eGPincodeModel.District.valueList = datalayer.dldata.Districts;
                $scope.GPincodedata.City = $scope.GPincodedata.City.toUpperCase();
                $scope.eGPincodeModel.City.valueList = datalayer.dldata.City;
            } else {
                $scope.GPincodedata = datalayer.dldata.GPincodedata;
            };
            $scope.dldata = datalayer.dldata;
            $scope.dldata.RegionList = [];
            $scope.dldata.StateList = [];
            $scope.dldata.ClusterList = [];
            $scope.dldata.DistrictList = [];
            $scope.dldata.CityList = [];
        })();

        $scope.getState = function (region) {
            datalayer.getStateData(region).then(function (data) {
                $scope.eGPincodeModel.State.valueList = data;
            });
        };

        $scope.getCluster = function (region, state) {
            datalayer.getClusterData(region, state).then(function (data) {
                $scope.eGPincodeModel.Cluster.valueList = data;
            });
        };

        $scope.getDistrict = function (region, state, cluster) {
            datalayer.getDistrictData(region, state, cluster).then(function (data) {
                $scope.eGPincodeModel.District.valueList = data;
            });
        };

        $scope.getCity = function (region, state, cluster, district) {
            datalayer.getCityData(region, state, cluster, district).then(function (data) {
                $scope.eGPincodeModel.City.valueList = data;
            });
        };

        $scope.pincodedata = function (pincode) {
            factory.pincodedata(pincode);
        };

        $scope.save = function (pincode, mode) {
            datalayer.editPincode(pincode, mode).then(function (data) {
                $scope.GPincodedata.Region = '';
                $scope.GPincodedata.State = '';
                $scope.GPincodedata.Cluster = '';
                $scope.GPincodedata.District = '';
                $scope.GPincodedata.City = '';
                $scope.GPincodedata.CityCategory = '';
                $scope.GPincodedata.Area = '';
                $scope.GPincodedata.Pincode = '';
                $scope.closeEditModel(data);
            });
        };

        $scope.closeEditModel = function () {
            $scope.GPincodedata.Region = '';
            $scope.GPincodedata.State = '';
            $scope.GPincodedata.Cluster = '';
            $scope.GPincodedata.District = '';
            $scope.GPincodedata.City = '';
            $scope.GPincodedata.CityCategory = '';
            $scope.GPincodedata.Area = '';
            $scope.GPincodedata.Pincode = '';
            $modalInstance.dismiss();
        };

        $scope.reset = function (gpincode) {
            factory.reset(gpincode);
        };

        $scope.regionChange = function () {
            factory.regionChange();
        };

        $scope.stateChange = function () {
            factory.stateChange();
        };

        $scope.clusterChange = function () {
            factory.clusterChange();
        };

        $scope.districtChange = function () {
            factory.districtChange();
        };

        $scope.areaChange = function () {
            factory.areaChange();
        };

        $scope.addCity = function () {
            $scope.showTextBox = true;
        };

        $scope.pincodeArea = function (value, city) {
            if (value.length >= 3) {
                return datalayer.pincodeArea(value, city);
            }
        };

        $scope.pincodeCity = function (value, city) {
            if (value.length >= 3) {
                return datalayer.pincodeCity(value, city);
            }
        };

        $scope.missingPincode = function(value) {
            if (value.length >= 3) {
                return datalayer.missingPincode(value);
            };
        };
            

        $scope.cancelCity = function () {
            $scope.showTextBox = false;
        };


        (function (mode) {
            switch (mode) {
                case "add":
                    $scope.modelTitle = "Add New Pincode";
                    break;
                case "edit":
                    $scope.modelTitle = "Edit Pincode";
                    break;
                default:
                    throw ("Invalid display mode : " + JSON.stringify(gPincodes));
            }
            $scope.mode = mode;
        })(gPincodes.displaymode);
    }]);































//csapp.factory("pincodeDataLayer", ["Restangular", "$csnotify", "$csfactory",
//    function (rest, $csnotify, $csfactory) {
//        var dldata = {};

//        var pincodeApi = rest.all('PincodeApi');

//        var cityCategoty = function () {
//            pincodeApi.customGET('GetCityCategory').then(function (data) {
//                dldata.CityCategoryList = data;
//            });
//        };

//        var showErrorMessage = function (response) {
//            $csnotify.error(response.data);
//        };

//        var getData = function () {
//            //if (dldata.isInEditMode == true) {
//            //return;
//            //}
//            //if (angular.isDefined(dldata.GPincodedata)) {
//            //    dldata.GPincodedata = {
//            //        Country: 'India',
//            //        Region: dldata.GPincodedata.Region,
//            //        State: dldata.GPincodedata.State,
//            //        Cluster: dldata.GPincodedata.Cluster,
//            //        District: dldata.GPincodedata.District,
//            //        City: dldata.GPincodedata.City
//            //    };
//            //}
//            if (angular.isUndefined(dldata.GPincodedata)) {
//                dldata.GPincodedata = {};
//                dldata.GPincodedata.Country = 'India';
//            }
//            //#region Set value in Object

//            //#endregion

//            var gPincodedata = dldata.GPincodedata;

//            return pincodeApi.customPOST(gPincodedata, "GetWholedata").then(function (data2) {
//                if (angular.isUndefined(gPincodedata.Region) || gPincodedata.Region == "") {
//                    //var Region = data2;
//                    dldata.RegionList = _.uniq(data2);
//                    return;
//                }

//                if (angular.isUndefined(gPincodedata.State) || gPincodedata.State == "") {
//                    dldata.StateList = _.uniq(data2);
//                    return;
//                }

//                if (angular.isUndefined(gPincodedata.Cluster) || gPincodedata.Cluster == "") {
//                    dldata.ClusterList = _.uniq(data2);
//                    return;
//                }

//                if (angular.isUndefined(gPincodedata.District) || gPincodedata.District == "") {
//                    dldata.DistrictList = _.uniq(data2);
//                    return;
//                }

//                if (angular.isUndefined(gPincodedata.City) || gPincodedata.City == "") {
//                    dldata.CityList = _.uniq(data2);
//                    dldata.citydata = true;
//                    return;
//                }

//            }, showErrorMessage);

//        };

//        var pincodeCity = function (city, district) {
//            if (city.length < 2) {
//                return [];
//            }
//            return pincodeApi.customGET('GetPincodeCity', { city: city, district: district }).then(function (data) {
//                return data;
//            });
//        };

//        var pincodeArea = function (pincode, level) {
//            if (pincode.length < 2) {
//                return [];
//            }
//            return pincodeApi.customGET('GetPincodesArea', { area: pincode, city: level }).then(function (data) {
//                return data;
//            });
//        };

//        var missingPincode = function (pincode) {
//            if (pincode.length < 3) {
//                return [];
//            }
//            return pincodeApi.customGET('GetMissingPincodes', { pincode: pincode }).then(function (data) {
//                return data;
//            });
//        };

//        var getState = function () {
//            return pincodeApi.customGETLIST("GetStates").then(function (data) {
//                dldata.States = data;
//            }, showErrorMessage);
//        };

//        var getWholePincode = function () {
//            if (!$csfactory.isNullOrEmptyArray(dldata.PincodeUintList)) {
//                return;
//            }

//            pincodeApi.customGET('GetWholePincode').then(function (data) {
//                dldata.PincodeUintList = data;
//            });
//        };

//        var changeState = function (stateName) {
//            if ($csfactory.isNullOrEmptyString(stateName)) {
//                return;
//            }
//            pincodeApi.customGETLIST("GetPincodes", { state: stateName }).then(function (data) {
//                $csnotify.success("Pincodes loaded successfully");
//                dldata.GPincodes = data;
//                dldata.stateClusters = _.uniq(_.pluck(dldata.GPincodes, 'Cluster'));
//            }, showErrorMessage);

//        };

//        var editPincode = function (gpincode) {
//            if (dldata.isInEditMode === true) {
//                return pincodeApi.customPUT(gpincode, "Put", { id: gpincode.Id }).then(function (data) {
//                    dldata.GPincodes = _.reject(dldata.GPincodes, function (pincode) { return pincode.Id == gpincode.Id; });
//                    dldata.GPincodes.push(data);
//                    dldata.PincodeUintList.push(data);
//                    dldata.showTextBox = false;
//                    dldata.isInEditMode = false;
//                    $csnotify.success("Data saved");
//                    return;
//                }, showErrorMessage);
//            } else {
//                return pincodeApi.customPOST(gpincode, 'Post').then(function (data) {
//                    $csnotify.success("Pincode saved..!!");
//                    dldata.GPincodes.push(data);
//                    dldata.PincodeUintList.push(data);
//                    return data;
//                }, function (data) {
//                    $csnotify.error("Error occured, can't saved", data);
//                });
//            }
//        };

//        return {
//            dldata: dldata,
//            cityCategoty: cityCategoty,
//            getData: getData,
//            pincodeCity: pincodeCity,
//            pincodeArea: pincodeArea,
//            missingPincode: missingPincode,
//            getState: getState,
//            getWholePincode: getWholePincode,
//            changeState: changeState,
//            editPincode: editPincode
//        };
//    }]);

//csapp.factory("pincodeFactory", ["pincodeDataLayer",
//    function (datalayer) {

//        var dldata = datalayer.dldata;
//        var regionChange = function () {
//            if (angular.isDefined(dldata.GPincodedata.State)) {
//                dldata.StateList = [];
//                dldata.ClusterList = [];
//                dldata.DistrictList = [];
//                dldata.CityList = [];
//                dldata.CityCategoryList = [];
//                dldata.GPincodedata.Area = '';
//                dldata.showTextBox = false;
//                dldata.GPincodedata.Pincode = '';
//            }
//        };

//        var stateChange = function () {
//            if (angular.isDefined(dldata.GPincodedata.Cluster)) {
//                dldata.ClusterList = [];
//                dldata.DistrictList = [];
//                dldata.CityList = [];
//                dldata.CityCategoryList = [];
//                dldata.citydata = false;
//                dldata.GPincodedata.Area = '';
//                dldata.GPincodedata.Pincode = '';
//            }
//        };

//        var clusterChange = function () {
//            if (angular.isDefined(dldata.GPincodedata.District)) {
//                dldata.DistrictList = [];
//                dldata.CityList = [];
//                dldata.CityCategoryList = [];
//                dldata.citydata = false;
//                dldata.GPincodedata.Area = '';
//                dldata.GPincodedata.Pincode = '';
//            }
//        };

//        var districtChange = function () {
//            if (angular.isDefined(dldata.GPincodedata.City)) {
//                dldata.CityList = [];
//                dldata.CityCategoryList = [];
//                dldata.GPincodedata.Area = '';
//                dldata.GPincodedata.Pincode = '';
//            }
//            dldata.showTextBox = false;
//            dldata.citydata = true;
//        };

//        var cityChange = function () {
//            if (angular.isDefined(dldata.GPincodedata.Area)) {
//                dldata.GPincodedata.Area = '';
//                dldata.GPincodedata.Pincode = '';
//            }
//        };

//        var areaChange = function () {
//            if (angular.isDefined(dldata.GPincodedata.Pincode)) {
//                dldata.GPincodedata.Pincode = '';
//            }
//        };

//        var addNewCity = function () {
//            dldata.showTextBox = true;
//            dldata.GPincodedata.City = '';

//        };

//        var cancleAddCity = function () {
//            dldata.showTextBox = false;
//            dldata.GPincodedata.City = '';
//        };

//        var pincodedata = function (pincode) {
//            dldata.alreadyExit = false;
//            if (dldata.isInEditMode == true) {
//                return;
//            }
//            if (pincode.length === 6) {
//                var isExist = _.find(dldata.PincodeUintList, function (item) {
//                    return item == pincode;
//                });
//                if (angular.isDefined(isExist)) {
//                    dldata.alreadyExit = true;
//                    dldata.dummyPincode = dldata.GPincodedata.Pincode;
//                    return;
//                }
//            }
//        };

//        var clearSearchTextbox = function () {
//            dldata.search = {};
//        };

//        var reset = function () {
//            dldata.RegionList = [];
//            dldata.StateList = [];
//            dldata.ClusterList = [];
//            dldata.DistrictList = [];
//            dldata.CityList = [];
//            dldata.CityCategoryList = [];
//            dldata.GPincodedata.Area = '';
//            dldata.showTextBox = false;
//            dldata.GPincodedata.Pincode = '';
//        };

//        return {
//            regionChange: regionChange,
//            stateChange: stateChange,
//            clusterChange: clusterChange,
//            districtChange: districtChange,
//            cityChange: cityChange,
//            areaChange: areaChange,
//            addNewCity: addNewCity,
//            cancleAddCity: cancleAddCity,
//            pincodedata: pincodedata,
//            clearSearchTextbox: clearSearchTextbox,
//            reset: reset
//        };
//    }]);

//csapp.controller("pincodeCtrl", ["$scope", "pincodeDataLayer", "pincodeFactory", "$modal", "$Validations",
//    function ($scope, datalayer, factory, $modal, $Validation) {
//        (function () {
//            $scope.datalayer = datalayer;
//            $scope.dldata = datalayer.dldata;
//            $scope.dldata.val = $Validation;
//            $scope.dldata.States = [];
//            $scope.dldata.PincodeUintList = [];
//            //datalayer.cityCategoty();
//            datalayer.getState();
//            datalayer.getWholePincode();
//            $scope.factory = factory;
//            $scope.GPincodedata = datalayer.dldata.GPincodedata;
//            $scope.getData = datalayer.getData;

//        })();

//        //$scope.openAddModel = function () {
//        //    $scope.dldata.isInEditMode = false;
//        //    datalayer.getData();
//        //    $modal.open({
//        //        templateUrl: '/Generic/pincode/editPincode-modal.html',
//        //        controller: 'editPincodeModalController',
//        //    });
//        //};

//        $scope.openAddEditModal = function (mode, gPincode) {
//            //$scope.dldata.isInEditMode = true;
//            datalayer.getData();
//            $modal.open({
//                templateUrl: '/Generic/pincode/editPincode-modal.html',
//                controller: 'editPincodeModalController',
//                resolve: {
//                    gPincode: function () {
//                        //return gPincode;
//                        return {
//                            gpincodes: angular.copy(gPincode),
//                            displaymode: mode
//                        };
//                    }
//                }
//            });
//        };
//    }]);

////csapp.controller("pincodeModalController", ["$scope", "pincodeDataLayer", "pincodeFactory", "$modalInstance",
////    function ($scope, datalayer, factory, $modalInstance) {
////        $scope.save = function () {
////            datalayer.editPincode($scope.dldata.GPincodedata).then(function () {
////                $scope.closeAddModal();
////            });
////        };

////        $scope.closeAddModal = function () {
////            $modalInstance.dismiss();
////            factory.reset();
////        };
////        $scope.reset = function () {
////            factory.reset();
////        };
////        (function () {
////            $scope.datalayer = datalayer;
////            $scope.dldata = datalayer.dldata;
////            $scope.factory = factory;
////            datalayer.cityCategoty();

////        })();
////}]);

//csapp.controller("editPincodeModalController", ["$scope", "pincodeDataLayer", "pincodeFactory", "$modalInstance", "gPincode", "$csGenericModels",
//    function ($scope, datalayer, factory, $modalInstance, gPincode, $csGenericModels) {

//        $scope.closeEditModel = function () {
//            $modalInstance.dismiss();
//        };
//        $scope.reset = function () {
//            factory.reset();
//        };


//        $scope.save = function () {
//            datalayer.editPincode($scope.eGPincode).then(function () {
//                $scope.closeEditModel();
//            });
//        };

//        (function () {
//            $scope.eGPincodeModel = $csGenericModels.models.Pincode;
//            //$scope.datalayer = datalayer;
//            //$scope.dldata = datalayer.dldata;
//            //$scope.factory = factory;
//            //$scope.eGPincode = gPincode;
//            //datalayer.cityCategoty();
//            //$scope.GPincodedata = gPincode.gpincodes;
//            //datalayer.getData($scope.).then(function () {
//            //    factory.regionChange();
//            //});
//            //datalayer.getData().then(function() {
//            //    factory.stateChange();
//            //});
//            $scope.GPincodedata = datalayer.dldata.GPincodedata;
//            $scope.getData = datalayer.getData();
//            $scope.eGPincodeModel.Region.valueList = datalayer.dldata.RegionList;
//            $scope.eGPincodeModel.State.valueList = datalayer.dldata.StateList;
//            $scope.eGPincodeModel.Cluster.valueList = datalayer.dldata.ClusterList;
//            $scope.eGPincodeModel.District.valueList = datalayer.dldata.DistrictList;
//            $scope.eGPincodeModel.City.valueList = datalayer.dldata.CityList;
//        })();






//        (function (mode) {
//            switch (mode) {
//                case "add":
//                    $scope.modelTitle = "Add New Pincode";
//                case "edit":
//                    $scope.modelTitle = "Edit Pincode";
//                    break;
//                default:
//                    throw ("Invalid display mode : " + JSON.stringify(gPincode));
//            }
//            $scope.mode = mode;
//        })(gPincode.displaymode);

//    }]);