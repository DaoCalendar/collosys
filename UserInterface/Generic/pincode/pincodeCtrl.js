csapp.controller("pincodeCtrl", ["$scope", "pincodeDataLayer", "$modal", "$csGenericModels",
    function ($scope, datalayer, $modal, $csGenericModels) {
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.dldata.States = [];
            datalayer.getState();
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
            if (pincode.toString().length < 3) {
                return [];
            }
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

        var pincodeArea = function (pincode, level) {
            if (pincode.toString().length < 2) {
                return [];
            }
            return pincodeApi.customGET('GetPincodesArea', { area: pincode, city: level }).then(function (data) {
                return data;
            });
        };

        return {
            dldata: dldata,
            getState: getState,
            changeState: changeState,
            getData: getData,
            getStateData: getStateData,
            getClusterData: getClusterData,
            getDistrictData: getDistrictData,
            getCityData: getCityData,
            getWholePincode: getWholePincode,
            missingPincode: missingPincode,
            pincodeArea: pincodeArea
        };

    }]);


csapp.factory("pincodeFactory", ["pincodeDataLayer",
    function (datalayer) {

        var dldata = datalayer.dldata;
        var regionChange = function () {
            if (angular.isDefined(dldata.GPincodedata.State)) {
                dldata.GPincodedata.StateList = [];
                dldata.GPincodedata.ClusterList = [];
                dldata.GPincodedata.DistrictList = [];
                dldata.GPincodedata.CityList = [];
                dldata.GPincodedata.CityCategoryList = [];
                dldata.GPincodedata.Area = '';
                dldata.showTextBox = false;
                dldata.GPincodedata.Pincode = '';
            }
        };

        var stateChange = function () {
            if (angular.isDefined(dldata.GPincodedata.Cluster)) {
                dldata.GPincodedata.ClusterList = [];
                dldata.GPincodedata.DistrictList = [];
                dldata.GPincodedata.CityList = [];
                dldata.GPincodedata.CityCategoryList = [];
                dldata.GPincodedata.citydata = false;
                dldata.GPincodedata.Area = '';
                dldata.GPincodedata.Pincode = '';
            }
        };

        var clusterChange = function () {
            if (angular.isDefined(dldata.GPincodedata.District)) {
                dldata.GPincodedata.DistrictList = [];
                dldata.GPincodedata.CityList = [];
                dldata.GPincodedata.CityCategoryList = [];
                dldata.GPincodedata.citydata = false;
                dldata.GPincodedata.Area = '';
                dldata.GPincodedata.Pincode = '';
            }
        };

        var districtChange = function () {
            if (angular.isDefined(dldata.GPincodedata.City)) {
                dldata.GPincodedata.CityList = [];
                dldata.GPincodedata.CityCategoryList = [];
                dldata.GPincodedata.Area = '';
                dldata.GPincodedata.Pincode = '';
            }
            dldata.showTextBox = false;
            dldata.citydata = true;
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

        var cancelAddCity = function () {
            dldata.showTextBox = false;
            dldata.GPincodedata.City = '';
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

        var reset = function () {
            dldata.GPincodedata.RegionList = [];
            dldata.GPincodedata.StateList = [];
            dldata.GPincodedata.ClusterList = [];
            dldata.GPincodedata.DistrictList = [];
            dldata.GPincodedata.CityList = [];
            dldata.GPincodedata.CityCategoryList = [];
            dldata.GPincodedata.Area = '';
            dldata.GPincodedata.Pincode = '';
        };

        return {
            regionChange: regionChange,
            stateChange: stateChange,
            clusterChange: clusterChange,
            pincodedata: pincodedata,
            districtChange: districtChange,
            areaChange: areaChange,
            addNewCity: addNewCity,
            cancelAddCity: cancelAddCity,
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
            }
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

        $scope.closeEditModel = function () {
            $modalInstance.dismiss();
        };

        $scope.reset = function () {
            factory.reset();
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
            factory.addNewCity();
        };

        $scope.cancelCity = function () {
            factory.cancelAddCity();
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