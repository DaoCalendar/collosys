csapp.controller("Working", ['$scope', 'Restangular', '$Validations', '$log', '$timeout', '$csfactory', '$csnotify', 'pincodeMngr', '$modal',
    function ($scope, rest, $validations, $log, $timeout, $csfactory, $csnotify, pincodeMngr, $modal) {

        var restApi = rest.all('PaymentDetailsApi');

        var init = function () {
            $log.info("Wroking Screen");
            //working screen
            $scope.deleteModal = false;
            $scope.bucketArray = [{ display: '1(0-29)', value: 1 }, { display: '2(30-59)', value: 2 },
                { display: '3(60-89)', value: 3 }, { display: '4(90-119)', value: 4 },
                { display: '5(120-149)', value: 5 }, { display: '6(150-179)', value: 6 },
                { display: '6(180+)', value: '>6' }];
            $scope.stateArray = [];
            $scope.uniqueReg = [];
            $scope.buckArray = [];
            $scope.uniqueClus = [];
            $scope.clusterArray = [];
            $scope.areaArray = [];
            $scope.cityArray = [];
            $scope.regionArray = [];
            $scope.changed = {
                Region: false,
                State: false,
                Cluster: false,
                District: false,
                City: false,
                Area: false
            };
            $scope.displayManager = {
                showCountry: true,
                showState: false,
                showCluster: false,
                showDistrict: false,
                showCity: false,
                showArea: false
            };
            $scope.workingModel = {
                DisplayManager: {},
                SelectedPincodeData: {
                },
                MultiSelectValues: [],
                //QueryFor: ''
            };
            $scope.pincodeManager = pincodeMngr;
            $scope.showAddedData = false;
            $scope.showButtons = false;
            $scope.disableLocCombo = false;
            $scope.multiple = false;
            $scope.val = $validations;
            $scope.clusterArray = [];
            $scope.StakeWork = {
                Cluster: []
            };
            if ($scope.$parent.WizardData.IsEditMode()) {
                var locationLevel = JSON.parse($scope.$parent.WizardData.FinalPostModel.Hierarchy.LocationLevel);
                $scope.$parent.WizardData.SetLocationLevel(locationLevel[0]);
                $scope.$parent.WizardData.SetLocationLevelArray(locationLevel);
            }
            $scope.WorkingData = {
                Hierarchy: $scope.$parent.WizardData.GetHierarchy(),
                isEditMode: $scope.$parent.WizardData.IsEditMode(),
                ReportsToList: $scope.$parent.WizardData.GetReporteeList(),
                LocationLevel: $scope.$parent.WizardData.GetLocationLevel(),
                LocationLevelArray: $scope.$parent.WizardData.GetLocationLevelArray(),
                StateList: [],
                clusterList: [],
                cityList: [],
                areaList: [],
                ProductList: [],
                RegionList: [],
                PayWorkModelList: $scope.$parent.WizardData.GetPayWorkModelList(),
                PayWorkModel: $scope.$parent.WizardData.GetPayWorkModel()
            };
            if ($scope.WorkingData.isEditMode) {
                var expiredWorkings = [];
                _.forEach($scope.WorkingData.PayWorkModel.WorkList, function (item) {
                    if (!$csfactory.isNullOrEmptyString(moment(item.EndDate))
                        && moment(item.EndDate).format("DD-MM-YYYY") < moment().format("DD-MM-YYYY"))
                        expiredWorkings.push(item);
                });

                _.forEach(expiredWorkings, function (item) {
                    var index = $scope.WorkingData.PayWorkModel.WorkList.indexOf(item);
                    if (index != -1)
                        $scope.WorkingData.PayWorkModel.WorkList.splice(index, 1);
                });
            }

            $scope.WorkingData.LocationLevelParams = ['COUNTRY', 'REGION', 'STATE', 'CLUSTER', 'CITY', 'AREA'];
            lists();
        };


        $scope.openMultiSelectPopUp = function (array) {

            $scope.modalData = {};

            $scope.modalData.array = array;
            $scope.modalData.areaArray = $scope.areaArray;
            $scope.modalData.clusterArray = $scope.clusterArray;
            $scope.modalData.cityArray = $scope.cityArray;
            $scope.modalData.LocationLevel = $scope.WorkingData.LocationLevel;
            $scope.modalData.StakeWork = $scope.StakeWork;
            $scope.modalData.SelectedPincodeData = $scope.workingModel.SelectedPincodeData;

            var modalInstance = $modal.open({
                templateUrl: baseUrl + 'Stakeholder/add/multiselectPopUp.html',
                controller: 'multiSelectController',
                resolve: {
                    modalData: function () {
                        return $scope.modalData;
                    }
                }
            });

            modalInstance.result.then(function (data) {
                $scope.modalData = data;

            });

        };


        //#region old
        var lists = function () {
            restApi.customGET('GetAllWorkingList').then(function (data) {
                $scope.WorkingData.ProductList = data.Products;
                $scope.WorkingData.RegionList = data.Region;
            }, function (data) {
                $log.error(data.data.Message);
            });

        };

        $scope.closeModal = function () {
            $scope.deleteModal = false;
            $scope.statePopUp = false;
            $scope.clusterPopUp = false;
            $scope.cityPopUp = false;
            $scope.areaPopUp = false;
            $scope.regionPopUp = false;
            $scope.districtPopUp = false;
            //$scope.clusterArray = [];
            //$scope.areaArray = [];
            //$scope.regionArray = [];
            //$scope.stateArray = [];
            //$scope.cityArray = [];
            $scope.endDate = "";

        };

        $scope.multipleModalOk = function (locLevel) {
            if (locLevel === 'City')
                $scope.workingModel.SelectedPincodeData['District'] = "";
            $scope.StakeWork[locLevel] = "";
            $scope.workingModel.SelectedPincodeData[locLevel] = "";
            $scope.statePopUp = false;
            $scope.clusterPopUp = false;
            $scope.cityPopUp = false;
            $scope.areaPopUp = false;
            $scope.regionPopUp = false;
            $scope.districtPopUp = false;
        };



        $scope.getPaymentDetails = function (product) {

            if ($scope.WorkingData.Hierarchy.HasWorking && $scope.WorkingData.Hierarchy.HasPayment) {
                restApi.customGET('GetLinerWriteOff', { 'product': product }).then(function (data) {
                    $scope.$parent.WizardData.BillingPolicy.LinerPolicies = data.LinerList;
                    $scope.$parent.WizardData.BillingPolicy.WriteOffPolicies = data.WriteOffList;
                });
            } else return;
        };



        $scope.endDateStatus = function (data) {
            if (!$csfactory.isNullOrEmptyString(data.EndDate)) return true;
            else return false;
        };

        $scope.enableDeleteIcon = function () {
            if ($scope.WorkingData.isEditMode) return ($scope.WorkingData.PayWorkModel.WorkList.length > 1);
            else return ($scope.WorkingData.PayWorkModel.WorkList.length >= 1);
        };

        $scope.manageDelete = function (index, data) {
            if ($scope.WorkingData.isEditMode) {

                $scope.modalData = {};
                $scope.modalData.endDate = data.EndDate;
                $scope.modalData.currentDeleteData = data;

                $modal.open({
                    templateUrl: baseUrl + 'Stakeholder/add/DeleteWorkingPopUp.html',
                    controller: 'deleteWorkingController',
                    resolve: {
                        modalData: function () {
                            return $scope.modalData;
                        }
                    }
                });

                //$scope.deleteModal = !$scope.deleteModal;
                //$scope.endDate = data.EndDate;
                //$scope.currentDeleteData = data;
            } else {
                $scope.deleteAdded(index, $scope.WorkingData.Hierarchy, data);
            }
        };




        $scope.setEndDate = function (data, endDate) {
            var date = angular.copy(endDate);
            data.EndDate = date;
            $scope.endDate = '';
            $scope.deleteModal = false;
        };

        $scope.cancelEndDate = function (currentDeleteData) {
            currentDeleteData.EndDate = undefined;
            $scope.deleteModal = false;
        };

        var deletedata = function (data, arrays) {
            var index = arrays.indexOf(data);
            arrays.splice(index, 1);
        };

        $scope.checkLocationLevel = function (locLevel) {
            if ($scope.WorkingData.LocationLevelArray.length > 1)
                return !$csfactory.isNullOrEmptyString(locLevel);
            else return true;
        };


        $scope.clrArray = function () {
            $scope.clusterArray = [];
            $scope.stateArray = [];
            $scope.areaArray = [];
        };

        $scope.enableAddButton = function () {
            if (angular.isUndefined($scope.workingModel))
                return true;

            if ($scope.WorkingData.LocationLevelArray.length > 1) {

                if (angular.isUndefined($scope.workingModel.SelectedPincodeData[$scope.WorkingData.LocationLevel])) return true;
                if ($csfactory.isNullOrEmptyString($scope.workingModel.SelectedPincodeData[$scope.WorkingData.LocationLevel])) {
                    switch ($scope.WorkingData.LocationLevel) {
                        case 'Region':
                            return $csfactory.isNullOrEmptyString($scope.workingModel.SelectedPincodeData[$scope.WorkingData.LocationLevel]);
                        case 'District':
                            return $scope.clusterArray.length === 0;
                        case 'Area':
                            return $scope.areaArray.length === 0;
                        case 'City':
                            return $scope.cityArray.length === 0;
                    }
                }
            }

            switch ($scope.WorkingData.LocationLevel) {
                case 'Region':
                    if (angular.isDefined($scope.workingModel.SelectedPincodeData)) {
                        if ($csfactory.isNullOrEmptyString($scope.workingModel.SelectedPincodeData.Region))
                            return true;
                        else return false;
                    }
                case 'District':
                    if (angular.isDefined($scope.workingModel.SelectedPincodeData)) {
                        if ($csfactory.isNullOrEmptyString($scope.workingModel.SelectedPincodeData.District) && $scope.clusterArray.length === 0)
                            return true;
                        else return false;
                    }
                case 'Area':
                    if (angular.isDefined($scope.workingModel.SelectedPincodeData)) {
                        if ($csfactory.isNullOrEmptyString($scope.workingModel.SelectedPincodeData.Area) && $scope.areaArray.length === 0)
                            return true;
                        else return false;
                    }
                case 'City':
                    if (angular.isDefined($scope.workingModel.SelectedPincodeData)) {
                        if ($csfactory.isNullOrEmptyString($scope.workingModel.SelectedPincodeData.District) && $scope.cityArray.length === 0)
                            return true;
                        else return false;
                    }
            }
        };

        $scope.ticks = function (data) {
            switch ($scope.WorkingData.LocationLevel.toUpperCase()) {
                case "REGION":
                    return ($scope.regionArray.indexOf(data) !== -1);
                case "STATE":
                    return ($scope.stateArray.indexOf(data) !== -1);
                case "CLUSTER":
                case "DISTRICT":
                    return ($scope.clusterArray.indexOf(data) !== -1);
                case "CITY":
                    return ($scope.cityArray.indexOf(data) !== -1);
                case "AREA":
                    return ($scope.areaArray.indexOf(data) !== -1);
            }
        };

        $scope.ticksBuck = function (data) {
            return ($scope.buckArray.indexOf(data.value) !== -1);
        };

        $scope.selAllBuck = function () {
            if ($scope.buckArray.length === $scope.bucketArray.length)
                $scope.buckArray = [];
            for (var buck = 0; buck < $scope.bucketArray.length; buck++) {
                var dupBucket = _.find($scope.buckArray, function (item) {
                    if (item === $scope.bucketArray[buck].value)
                        return item;
                });
                if ($csfactory.isNullOrEmptyString(dupBucket))
                    $scope.buckArray.push($scope.bucketArray[buck].value);
                $scope.buckArray.sort();
            }
        };

        $scope.getWorkingReportees = function (product) {
            $scope.ReportsToList = [];
            //var len = $scope.WorkingData.RegionList.length;
            //for (var i = 0; i < len; i++) {
            //    var added = _.find($scope.uniqueReg, function (item) {
            //        if (item === $scope.WorkingData.RegionList[i].Region)
            //            return item;
            //    });
            //    if ($csfactory.isNullOrEmptyString(added))
            //        $scope.uniqueReg.push($scope.WorkingData.RegionList[i].Region);
            //}
            restApi.customPOST($scope.WorkingData.Hierarchy, 'WorkingReportsTo').then(function (data) {
                $scope.WorkingData.ReportsToList = data;
                var x;
                $scope.ReportsToList = [];
                _.forEach($scope.WorkingData.ReportsToList, function (item) {
                    if (item.StkhWorkings.length > 0) {
                        x = _.find(item.StkhWorkings, function (workings) {
                            if (workings.Products === 'ALL')
                                return workings;
                            if (workings.Products === product && product != 'ALL')
                                return workings;
                            if (product === 'ALL') return workings;
                        });
                    } else {
                        x = item;
                    }
                    if (!$csfactory.isNullOrEmptyString(x)) {
                        $scope.ReportsToList.push(item);
                    }
                });
            });


            $scope.selAll = false;
            $scope.selected = false;
        };

        $scope.chooseMultiple = function (data, selected, locLevel) {
            switch (locLevel) {
                case "BUCKET":
                    if ($scope.buckArray.indexOf(data.value) === -1) {
                        $scope.buckArray.push(data.value);
                    } else {
                        $scope.buckArray.splice($scope.buckArray.indexOf(data.value), 1);
                    }
                    break;
                case "Cluster":
                case 'District':
                    //if (selected == true) {
                    //    var repeatCluster = _.filter($scope.clusterArray, function (item) {
                    //        if (item === data)
                    //            return item;
                    //    });
                    //    if (repeatCluster.length === 0) {
                    //        $scope.clusterArray.push(data);
                    //    }
                    //}
                    //if (selected == false) {
                    //    deletedata(data, $scope.clusterArray);
                    //}
                    if ($scope.clusterArray.indexOf(data) === -1) {
                        $scope.clusterArray.push(data);
                    } else {
                        $scope.clusterArray.splice($scope.clusterArray.indexOf(data), 1);
                    }
                    break;
                case "Region":
                    //if (selected == true) {
                    //    
                    //    var repeatRegion = _.filter($scope.regionArray, function (item) {
                    //        if (item === data)
                    //            return data;
                    //    });
                    //    if (repeatRegion.length === 0) {
                    //        $scope.regionArray.push(data);
                    //    }
                    //}
                    //if (selected === false) {
                    //    deletedata(data, $scope.regionArray);
                    //}
                    if ($scope.regionArray.indexOf(data) === -1) {
                        $scope.regionArray.push(data);
                    } else {
                        $scope.regionArray.splice($scope.regionArray.indexOf(data), 1);
                    }
                    break;
                case "State":
                    //if (selected === true) {
                    //    var repeatState = _.filter($scope.WorkingData.PayWorkModel.WorkList, function (item) {
                    //        if (item.State === data)
                    //            return data;
                    //    });
                    //    if (repeatState.length === 0) {
                    //        $scope.stateArray.push(data);
                    //    }
                    //}
                    //if (selected === false) {
                    //    deletedata(data, $scope.stateArray);
                    //}
                    if ($scope.stateArray.indexOf(data) === -1) {
                        $scope.stateArray.push(data);
                    } else {
                        $scope.stateArray.splice($scope.stateArray.indexOf(data), 1);
                    }
                    break;
                case "City":

                    //if (selected === true) {
                    //    var repeatCity = _.filter($scope.cityArray, function (item) {
                    //        if (item === data)
                    //            return data;
                    //    });
                    //    if (repeatCity.length === 0) {
                    //        $scope.cityArray.push(data);
                    //    }
                    //}
                    //if (selected == false) {
                    //    deletedata(data, $scope.cityArray);
                    //}
                    if ($scope.cityArray.indexOf(data) === -1) {
                        $scope.cityArray.push(data);
                    } else {
                        $scope.cityArray.splice($scope.cityArray.indexOf(data), 1);
                    }
                    break;
                case "Area":

                    //if (selected == true) {
                    //    var repeatArea = _.filter($scope.WorkingData.PayWorkModel.WorkList, function (item) {
                    //        if (item.Area == data)
                    //            return data;
                    //    });
                    //    if (repeatArea.length === 0) {
                    //        $scope.areaArray.push(data);
                    //    }
                    //}
                    //if (selected == false) {
                    //    deletedata(data, $scope.areaArray);
                    //}
                    if ($scope.areaArray.indexOf(data) === -1) {
                        $scope.areaArray.push(data);
                    } else {
                        $scope.areaArray.splice($scope.areaArray.indexOf(data), 1);
                    }
            }
        };

        var assignMissingPincodeValues = function (pincodeData) {
            switch ($scope.WorkingData.LocationLevel) {
                case 'Cluster':
                    if ($csfactory.isNullOrEmptyString($scope.workingModel.SelectedPincodeData.State)) {
                        var pincode = _.find(pincodeData.GPincodes, function (item) {
                            if (item.Cluster === $scope.workingModel.SelectedPincodeData.Cluster)
                                return item;
                        });
                        if (!$csfactory.isNullOrEmptyString(pincode))
                            $scope.workingModel.SelectedPincodeData.State = pincode.State;
                    }
                    break;
                case 'State':
                    var pincodeState = _.find(pincodeData.GPincodes, function (item) {
                        if (item.State === $scope.workingModel.SelectedPincodeData.State)
                            return item;
                    });
                    if (!$csfactory.isNullOrEmptyString(pincodeState))
                        $scope.workingModel.SelectedPincodeData.Region = pincodeState.Region;
                    break;
                case 'City':

                    var pincodeCity = _.find(pincodeData.GPincodes, function (item) {
                        if (item.District === $scope.workingModel.SelectedPincodeData.City)
                            return item;
                    });
                    if (!$csfactory.isNullOrEmptyString(pincodeCity)) {
                        $scope.workingModel.SelectedPincodeData.Region = pincodeCity.Region;
                        $scope.workingModel.SelectedPincodeData.Cluster = pincodeCity.Cluster;
                        $scope.workingModel.SelectedPincodeData.State = pincodeCity.State;
                        $scope.workingModel.SelectedPincodeData.District = pincodeCity.District;
                        //$scope.workingModel.SelectedPincodeData.City = pincodeCity.District;
                    }
                    break;
                case 'District':
                    var pincodeDistrict = _.find(pincodeData.GPincodes, function (item) {
                        if (item.District === $scope.workingModel.SelectedPincodeData.District)
                            return item;
                    });
                    if (!$csfactory.isNullOrEmptyString(pincodeDistrict)) {
                        $scope.workingModel.SelectedPincodeData.Region = pincodeDistrict.Region;
                        $scope.workingModel.SelectedPincodeData.Cluster = pincodeDistrict.Cluster;
                        //$scope.workingModel.SelectedPincodeData.City = pincodeDistrict.District;
                        $scope.workingModel.SelectedPincodeData.State = pincodeDistrict.State;
                    }
                    break;
                case "Area":

                    var pincodeArea = _.find(pincodeData.GPincodes, function (item) {
                        if (item.Area === $scope.workingModel.SelectedPincodeData.Area)
                            return item;
                    });
                    if (!$csfactory.isNullOrEmptyString(pincodeArea)) {
                        $scope.workingModel.SelectedPincodeData.Region = pincodeArea.Region;
                        $scope.workingModel.SelectedPincodeData.State = pincodeArea.State;
                        $scope.workingModel.SelectedPincodeData.Cluster = pincodeArea.Cluster;
                        $scope.workingModel.SelectedPincodeData.City = pincodeArea.City;
                    }
                    break;
            }
        };

        var getPincodeForMultiSelect = function () {
            $scope.workingModel.MultiSelectValues = [];
            switch ($scope.WorkingData.LocationLevel) {

                case 'District':
                    for (var k = 0; k < $scope.clusterArray.length; k++) {
                        $scope.workingModel.MultiSelectValues.push($scope.clusterArray[k]);
                    }
                    break;
                case 'City':
                    if ($scope.cityArray.length === 0) {
                        $scope.workingModel.SelectedPincodeData.City = $scope.workingModel.SelectedPincodeData.District;
                        $scope.workingModel.MultiSelectValues.push($scope.workingModel.SelectedPincodeData.District);
                    } else {
                        for (var j = 0; j < $scope.cityArray.length; j++) {
                            $scope.workingModel.MultiSelectValues.push($scope.cityArray[j]);
                        }
                    }
                    break;
                case 'Area':
                    for (var i = 0; i < $scope.areaArray.length; i++) {
                        $scope.workingModel.MultiSelectValues.push($scope.areaArray[i]);
                    }
                    break;
            }
        };

        $scope.addTable = function (stakeWork) {
            if ($scope.buckArray.length > 0)
                stakeWork.BucketStart = $scope.buckArray;
            stakeWork.LocationLevel = $scope.WorkingData.LocationLevel;
            if (!$csfactory.isNullOrEmptyArray($scope.workingModel.SelectedPincodeData.BucketStart)) {
                for (var aa = 0; aa < $scope.workingModel.SelectedPincodeData.BucketStart.length; aa++) {
                    $scope.workingModel.SelectedPincodeData.BucketStart[aa] = parseInt($scope.workingModel.SelectedPincodeData.BucketStart[aa]);
                }
            }
            $scope.workingModel.SelectedPincodeData.Products = stakeWork.Products;
            $scope.workingModel.SelectedPincodeData.ReportsTo = stakeWork.ReportsTo;
            $scope.workingModel.SelectedPincodeData.Country = "INDIA";

            pincodeMngr.ReplaceWithAll($scope.WorkingData.LocationLevel, $scope.workingModel.SelectedPincodeData);

            if (!$csfactory.isNullOrEmptyString($scope.workingModel.SelectedPincodeData[$scope.WorkingData.LocationLevel]))
                $scope.workingModel.MultiSelectValues.push($scope.workingModel.SelectedPincodeData[$scope.WorkingData.LocationLevel]);
            else {
                getPincodeForMultiSelect();
            }
            $scope.workingModel.QueryFor = $scope.WorkingData.LocationLevel;
            restApi.customPOST($scope.workingModel, 'GetPincodeList').then(function (data) {
                $scope.pincodeData = data;
                if (!$scope.WorkingData.Hierarchy.HasBuckets) {
                    $scope.workingModel.SelectedPincodeData.BucketStart = 0;
                }

                var bucketValue = [];
                if (!$scope.WorkingData.Hierarchy.HasBuckets) {
                    bucketValue = [];
                } else {
                    _.forEach($scope.workingModel.SelectedPincodeData.BucketStart, function (item) {
                        bucketValue.push(item);
                    });
                }

                //has no Buckets
                if (bucketValue.length === 0) {
                    // if duplicate, no need to add
                    switch ($scope.WorkingData.LocationLevel) {
                        case "Country":
                            $scope.dupCountry = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.clusterArray);
                            break;
                        case "Region":
                            $scope.dupRegion = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.regionArray);
                            break;
                        case "Cluster":
                        case "District":
                            assignMissingPincodeValues($scope.pincodeData);
                            $scope.dupCluster = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.clusterArray);
                            break;
                        case "State":
                            assignMissingPincodeValues($scope.pincodeData);
                            $scope.dupState = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.stateArray);
                            break;
                        case "City":
                            assignMissingPincodeValues($scope.pincodeData);
                            $scope.dupCity = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.cityArray);
                            break;
                        case "Area":
                            assignMissingPincodeValues($scope.pincodeData);
                            $scope.dupArea = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.areaArray);
                    }
                    //$scope.addMultipleEntries(stakeWork, $scope.WorkingData.LocationLevel);
                    $scope.addMultipleEntries($scope.workingModel.SelectedPincodeData, $scope.WorkingData.LocationLevel);
                }
                //hasBucket
                for (var i = 0; i < bucketValue.length; i++) {
                    $scope.workingModel.SelectedPincodeData.BucketStart = bucketValue[i];
                    switch ($scope.WorkingData.LocationLevel) {
                        case "Country":
                            $scope.dupCountry = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.clusterArray);
                            break;
                        case "Region":
                            $scope.dupRegion = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.regionArray);
                            break;
                        case "Cluster":
                        case "District":
                            assignMissingPincodeValues($scope.pincodeData);
                            $scope.dupCluster = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.clusterArray);
                            break;
                        case "State":
                            assignMissingPincodeValues($scope.pincodeData);
                            $scope.dupState = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.stateArray);
                            break;
                        case "City":
                            assignMissingPincodeValues($scope.pincodeData);
                            $scope.dupCity = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.cityArray);
                            break;
                        case "Area":
                            assignMissingPincodeValues($scope.pincodeData);
                            $scope.dupArea = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.areaArray);
                            break;
                    }
                    $scope.addMultipleEntries($scope.workingModel.SelectedPincodeData, $scope.WorkingData.LocationLevel);
                }
                $scope.showAddedData = true;
                $scope.showButtons = true;
                $scope.StakeWork.BucketStart = '';
                pincodeMngr.ClearOnLocationChange($scope.WorkingData.LocationLevel, stakeWork);
                $scope.cityArray = [];
                $scope.regionArray = [];
                $scope.stateArray = [];
                $scope.clusterArray = [];
                $scope.areaArray = [];
                $scope.buckArray = [];
                $scope.selAll = false;
                $scope.selected = false;
            });

        };

        $scope.addMultipleEntries = function (data, locLevel) {
            data.LocationLevel = locLevel;
            switch (locLevel) {
                case "Country":
                    if (!angular.isDefined($scope.dupCountry)) {
                        $scope.WorkingData.PayWorkModel.WorkList.push(angular.copy(data));
                    }
                    break;
                case "Region":
                    //Addding multiple Regions 
                    if ($csfactory.isNullOrEmptyString(data.Region)) {
                        if ($scope.regionArray.length > 0) {
                            for (var x = 0; x < $scope.regionArray.length; x++) {
                                data[locLevel] = $scope.regionArray[x];
                                $scope.WorkingData.PayWorkModel.WorkList.push(angular.copy(data));
                            }
                        }
                        data[locLevel] = "";
                    } else {
                        //Adding single Region 
                        if (!angular.isDefined($scope.dupRegion)) {
                            $scope.WorkingData.PayWorkModel.WorkList.push(angular.copy(data));
                        }
                    }
                    //data[locLevel] = "";
                    break;
                case "State":
                    if ($csfactory.isNullOrEmptyString(data.State)) {
                        var len9 = angular.copy($scope.clusterArray.length);
                        var stateArray = angular.copy($scope.clusterArray);
                        for (var mm = 0; mm < len9; mm++) {
                            data[locLevel] = stateArray[mm];
                            assignMissingPincodeValues($scope.pincodeData);
                            var dup9 = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.clusterArray);
                        }
                        if ($csfactory.isNullOrEmptyString(dup9)) { //add only if there are no duplicates
                            for (var a = 0; a < $scope.stateArray.length; a++) {
                                data[locLevel] = $scope.stateArray[p];
                                assignMissingPincodeValues($scope.pincodeData);
                                $scope.WorkingData.PayWorkModel.WorkList.push(angular.copy(data));
                            }
                        }
                        data[locLevel] = "";
                    } else {
                        //Adding single cluster
                        if (!angular.isDefined($scope.dupState)) {
                            assignMissingPincodeValues($scope.pincodeData);
                            $scope.WorkingData.PayWorkModel.WorkList.push(angular.copy(data));
                        }
                    }
                    //data[locLevel] = "";
                    break;





                    //Adding multiple States 
                    //if ($csfactory.isNullOrEmptyString(data.State)) {
                    //    var len11 = $scope.stateArray.length;
                    //    for (var z = 0; z < len11; z++) {
                    //        data[locLevel] = $scope.stateArray[z];
                    //        if ($csfactory.isNullOrEmptyString(data.Region)) {
                    //            var reg = _.find($scope.WorkingData.RegionList, function (item) {
                    //                if (item.State.toUpperCase() === data.State.toUpperCase())
                    //                    return item;
                    //            });
                    //            data.Region = reg.Region;
                    //        }
                    //        $scope.WorkingData.PayWorkModel.WorkList.push(angular.copy(data));
                    //        //deleting the added data from the list to avoid duplication
                    //        if ($scope.WorkingData.Hierarchy.HasBuckets === false) {
                    //            var addedStateList = _.find($scope.WorkingData.StateList, function (item) {
                    //                if (item.toUpperCase() === data.State.toUpperCase()) {
                    //                    return item;
                    //                }
                    //                return "";
                    //            });
                    //            $scope.WorkingData.StateList.splice($scope.WorkingData.StateList.indexOf(addedStateList), 1);
                    //            $scope.WorkingData.StateList.sort();
                    //        }
                    //    }
                    //    data[locLevel] = "";
                    //} else {
                    //    //Adding single state 
                    //    if (!angular.isDefined($scope.dupState)) {
                    //        if ($csfactory.isNullOrEmptyString(data.Region)) {
                    //            var reg1 = _.find($scope.WorkingData.RegionList, function (item) {
                    //                if (item.State.toUpperCase() === data.State.toUpperCase())
                    //                    return item;
                    //            });
                    //            data.Region = reg1.Region;
                    //        }
                    //        $scope.WorkingData.PayWorkModel.WorkList.push(angular.copy(data));
                    //        //deleting the added data from the list to avoid duplication
                    //        if ($scope.WorkingData.Hierarchy.HasBuckets === false) {
                    //            var addedState = _.find($scope.WorkingData.StateList, function (item) {
                    //                if (item.toUpperCase() === data.State.toUpperCase()) {
                    //                    return item;
                    //                }
                    //                return "";
                    //            });
                    //            $scope.WorkingData.StateList.splice($scope.WorkingData.StateList.indexOf(addedState), 1);
                    //            $scope.WorkingData.StateList.sort();
                    //        }
                    //    }
                    //}
                    //break;
                    //case "Cluster":
                    //    if ($csfactory.isNullOrEmptyString(data.Cluster)) {
                    //        for (var i = 0; i < $scope.clusterArray.length; i++) {
                    //            data[locLevel] = $scope.clusterArray[i];
                    //            var currPinData = _.find($scope.WorkingData.clusterList, function (item) {
                    //                if (item.Cluster.toUpperCase() === data.Cluster.toUpperCase())
                    //                    return item;
                    //            });
                    //            data.State = currPinData.State;
                    //            if (!$csfactory.isNullOrEmptyString(data.State))
                    //                data.Region = pincodeMngr.GetRegionData(data.State, $scope.WorkingData.RegionList);

                    //            $scope.WorkingData.PayWorkModel.WorkList.push(angular.copy(data));
                    //            //deleting the added data from the list to avoid duplication
                    //            if ($scope.WorkingData.Hierarchy.HasBuckets === false) {
                    //                var addedClusList = _.find($scope.uniqueClus, function (item) {
                    //                    if (item.toUpperCase() === data.Cluster.toUpperCase()) {
                    //                        return item;
                    //                    }
                    //                    return "";
                    //                });
                    //                $scope.uniqueClus.splice($scope.uniqueClus.indexOf(addedClusList), 1);
                    //                $scope.uniqueClus.sort();
                    //            }
                    //        }
                    //        data[locLevel] = "";
                    //    } else {
                    //        //Adding single cluster
                    //        if (!angular.isDefined($scope.dupCluster)) {
                    //            assignMissingPincodeValues($scope.pincodeData);
                    //            $scope.WorkingData.PayWorkModel.WorkList.push(angular.copy(data));
                    //            //deleting the added data from the list to avoid duplication
                    //            if ($scope.WorkingData.Hierarchy.HasBuckets === false) {
                    //                var addedClus = _.find($scope.uniqueClus, function (item) {
                    //                    if (item.toUpperCase() === data.Cluster.toUpperCase()) {
                    //                        return item;
                    //                    }
                    //                    return "";
                    //                });
                    //                $scope.uniqueClus.splice($scope.uniqueClus.indexOf(addedClus), 1);
                    //                $scope.uniqueClus.sort();
                    //            }
                    //        }
                    //    }
                case "District":
                    //Adding multiple Clusters 
                    if ($csfactory.isNullOrEmptyString(data.District)) {
                        var len10 = angular.copy($scope.clusterArray.length);
                        var clusterArray = angular.copy($scope.clusterArray);
                        for (var m = 0; m < len10; m++) {
                            data[locLevel] = clusterArray[m];
                            assignMissingPincodeValues($scope.pincodeData);
                            var dup10 = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.clusterArray);
                        }
                        if ($csfactory.isNullOrEmptyString(dup10)) { //add only if there are no duplicates
                            for (var p = 0; p < $scope.clusterArray.length; p++) {
                                data[locLevel] = $scope.clusterArray[p];
                                assignMissingPincodeValues($scope.pincodeData);
                                $scope.WorkingData.PayWorkModel.WorkList.push(angular.copy(data));
                            }
                        }
                        data[locLevel] = "";
                    } else {
                        //Adding single cluster
                        if (!angular.isDefined($scope.dupCluster)) {
                            assignMissingPincodeValues($scope.pincodeData);
                            $scope.WorkingData.PayWorkModel.WorkList.push(angular.copy(data));
                        }
                    }
                    //data[locLevel] = "";
                    break;

                case "City":
                    //Adding multiple cities
                    if ($csfactory.isNullOrEmptyString(data.City)) {
                        //duplicate check
                        var len0 = angular.copy($scope.cityArray.length);
                        var cityArray = angular.copy($scope.cityArray);
                        for (var l = 0; l < len0 ; l++) {
                            data[locLevel] = cityArray[l];
                            assignMissingPincodeValues($scope.pincodeData);
                            var dup = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.areaArray);
                        }
                        var len1 = $scope.cityArray.length;
                        if ($csfactory.isNullOrEmptyString(dup)) { //add only if there are no duplicates
                            for (var j = 0; j < len1; j++) {
                                data[locLevel] = $scope.cityArray[j];
                                assignMissingPincodeValues($scope.pincodeData);
                                $scope.WorkingData.PayWorkModel.WorkList.push(angular.copy(data));
                            }
                        }
                        data[locLevel] = "";
                    } else {
                        //Adding single City
                        if (!angular.isDefined($scope.dupCity)) {
                            assignMissingPincodeValues($scope.pincodeData);
                            $scope.WorkingData.PayWorkModel.WorkList.push(angular.copy(data));
                            //data[locLevel] = "";
                        }
                    }
                    //data.District = "";
                    break;

                case "Area":
                    //Adding multiple Area
                    if ($csfactory.isNullOrEmptyString(data.Area)) {
                        //duplicate check
                        var len = angular.copy($scope.areaArray.length);
                        var areaArray = angular.copy($scope.areaArray);
                        for (var i = 0; i < len ; i++) {
                            data[locLevel] = areaArray[i];
                            assignMissingPincodeValues($scope.pincodeData);
                            var dup1 = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.areaArray);
                        }
                        var len2 = $scope.areaArray.length;
                        if ($csfactory.isNullOrEmptyString(dup1)) {  //add only if there are no duplicates
                            for (var k = 0; k < len2; k++) {
                                data[locLevel] = $scope.areaArray[k];
                                assignMissingPincodeValues($scope.pincodeData);
                                $scope.WorkingData.PayWorkModel.WorkList.push(angular.copy(data));
                            }
                        }
                        data[locLevel] = "";
                    } else {
                        //Adding single Area
                        if (!angular.isDefined($scope.dupArea)) {
                            assignMissingPincodeValues($scope.pincodeData);
                            $scope.WorkingData.PayWorkModel.WorkList.push(angular.copy(data));
                        }
                    }
                    //data[locLevel] = "";
                    break;
            }

        };

        $scope.selectAll = function (selected) {

            switch ($scope.WorkingData.LocationLevel) {
                case "Region":
                    if ($scope.regionArray.length === $scope.uniqueReg.length)
                        $scope.regionArray = [];
                    else {
                        for (var z = 0; z < $scope.WorkingData.RegionList.length; z++) {
                            var dupReg = _.find($scope.regionArray, function (item) {
                                if (item.toUpperCase() === $scope.WorkingData.RegionList[z].Region.toUpperCase())
                                    return item;
                            });
                            if ($csfactory.isNullOrEmptyString(dupReg))
                                $scope.regionArray.push($scope.WorkingData.RegionList[z].Region);
                            $scope.regionArray.sort();
                        }
                    }
                    break;
                case "Cluster":
                case 'District':
                    if ($scope.clusterArray.length === $scope.workingModel.ListOfDistricts.length)
                        $scope.clusterArray = [];
                    else {
                        for (var i = 0; i < $scope.workingModel.ListOfDistricts.length; i++) {
                            var dupClus = _.find($scope.clusterArray, function (item) {
                                if (item.toUpperCase() === $scope.workingModel.ListOfDistricts[i].toUpperCase())
                                    return item;
                            });
                            if ($csfactory.isNullOrEmptyString(dupClus))
                                $scope.clusterArray.push($scope.workingModel.ListOfDistricts[i]);
                            $scope.clusterArray.sort();
                        }
                    }
                    break;
                case "State":
                    if ($scope.stateArray.length === $scope.WorkingData.StateList.length)
                        $scope.stateArray = [];
                    else {

                        for (var o = 0; o < $scope.WorkingData.StateList.length; o++) {
                            var dupState = _.find($scope.stateArray, function (item) {
                                if (item.toUpperCase() === $scope.WorkingData.StateList[o].toUpperCase())
                                    return item;
                            });
                            if ($csfactory.isNullOrEmptyString(dupState))
                                $scope.stateArray.push($scope.WorkingData.StateList[o]);
                            $scope.stateArray.sort();
                        }

                    }
                    break;
                case "City":

                    if ($scope.cityArray.length === $scope.workingModel.ListOfDistricts.length)
                        $scope.cityArray = [];
                    else {
                        for (var u = 0; u < $scope.workingModel.ListOfDistricts.length; u++) {
                            var dupCity = _.find($scope.cityArray, function (item) {
                                if (item.toUpperCase() === $scope.workingModel.ListOfDistricts[u].toUpperCase())
                                    return item;
                            });
                            if ($csfactory.isNullOrEmptyString(dupCity))
                                $scope.cityArray.push($scope.workingModel.ListOfDistricts[u]);
                            $scope.cityArray.sort();
                        }
                    }
                    break;
                case "Area":
                    if ($scope.areaArray.length === $scope.workingModel.ListOfAreas.length)
                        $scope.areaArray = [];
                    else {

                        for (var l = 0; l < $scope.workingModel.ListOfAreas.length; l++) {
                            var dupArea = _.find($scope.areaArray, function (item) {
                                if (item.toUpperCase() === $scope.workingModel.ListOfAreas[l].toUpperCase())
                                    return item;
                            });
                            if ($csfactory.isNullOrEmptyString(dupArea))
                                $scope.areaArray.push($scope.workingModel.ListOfAreas[l]);
                            $scope.areaArray.sort();
                        }
                    }

                    break;
            }
        };

        $scope.getStates = function () {
            $scope.StakeWork.BucketStart = "";
            pincodeMngr.GetState().then(function (data) {
                $scope.WorkingData.StateList = data;
                $scope.WorkingData.StateList.sort();
            });
        };

        $scope.getClusterList = function (reportdToId) {
            if (!angular.isDefined(reportdToId) || reportdToId == "") {
                return [];
            }

            if ($scope.enableLocation() > 3) {
                pincodeMngr.GetCluster(reportdToId).then(function (data) {
                    $scope.WorkingData.clusterList = data;
                    //creating an array('uniqueClus') which will store unique clusters
                    $scope.uniqueClus = [];
                    var len = $scope.WorkingData.clusterList.length;
                    for (var i = 0; i < len; i++) {
                        var added = _.find($scope.uniqueClus, function (item) {
                            if (item === $scope.WorkingData.clusterList[i].Cluster)
                                return item;
                        });
                        if ($csfactory.isNullOrEmptyString(added))
                            $scope.uniqueClus.push($scope.WorkingData.clusterList[i].Cluster);
                    }
                    //deleting clusters which has already been added
                    if ($scope.WorkingData.Hierarchy.HasBuckets === false) {
                        for (var j = 0; j < $scope.uniqueClus.length; j++) {
                            var x = _.find($scope.uniqueClus, function (item) {
                                if (item === $scope.WorkingData.PayWorkModel.WorkList[j])
                                    return item;
                            });
                            if (angular.isDefined(x))
                                $scope.uniqueClus.splice($scope.uniqueClus.indexOf(x), 1);
                        }
                    }
                });
            }
        };

        $scope.enableLocation = function () {
            if (!$csfactory.isNullOrEmptyString($scope.WorkingData.LocationLevel)) {
                $scope.$parent.WizardData.SetLocationLevel($scope.WorkingData.LocationLevel);
                if ($scope.WorkingData.LocationLevelArray.length == 1) {
                    var x = $scope.WorkingData.LocationLevelParams.indexOf($scope.WorkingData.LocationLevel.toUpperCase()) + 1;
                    return x;
                } else {
                    return $scope.WorkingData.LocationLevelParams.indexOf($scope.WorkingData.LocationLevel.toUpperCase()) + 1;
                }
            }
            return '';
        };

        $scope.setLocationLevel = function () {
            $scope.StakeWork.State = "";
            $scope.StakeWork.Cluster = "";
            $scope.StakeWork.City = "";
            $scope.StakeWork.Region = "";
            $scope.StakeWork.BucketStart = "";
            pincodeMngr.ClearOnLocationChange($scope.WorkingData.LocationLevel, $scope.StakeWork);
            $scope.enableLocation();
        };

        $scope.replace = function (info) {

            if ($csfactory.isNullOrEmptyArray(info)) return "-";
            if ($scope.WorkingData.Hierarchy.HasBuckets) {
                if (info.toString() === "0") {
                    return ">6";
                }
            } else {
                if (info.toString() === "0") {
                    return "-";
                }
            }
            //if (info.toString() === "7") return "6+";
            if (info === "ALL") return '-';
            return info;

        };

        $scope.clearArray = function () {
            switch ($scope.WorkingData.LocationLevel) {
                case "Cluster":
                    $scope.clusterArray = [];
                    break;
                case "City":
                    $scope.cityArray = [];
                    break;
            }
        };

        $scope.deleteAdded = function (index, hierarchy, data) {

            //if (hierarchy.HasWorking && !hierarchy.HasPayment) {
            //    $scope.$parent.WizardData.RemoveWorkingFromPaywork(index, 1);
            //}
            if (hierarchy.HasPayment === true) {

            }
            if ($scope.$parent.WizardData.IsEditMode() === true && !$csfactory.isNullOrEmptyGuid(data.Id) && data.Status === 'Approved') {
                data.Status = 'Submitted';
                data.RowStatus = 'Delete';
            } else {
                $scope.WorkingData.PayWorkModel.WorkList.splice(index, 1);
            }



        };

        $scope.cancelDeleted = function (data) {
            data.Status = null;
            data.RowStatus = null;
        };
        //#endregion

        //#region display manager
        var resetDisplayManager = function () {
            $scope.displayManager = {
                showCountry: true,
                showCluster: false,
                showState: false,
                showRegion: false,
                showDistrict: false,
                showCity: false,
                showArea: false
            };
        };

        $scope.resetChanged = function (param) {
            switch (param) {
                case 'Country':
                    $scope.changed = {
                        Region: false,
                        State: false,
                        Cluster: false,
                        City: false,
                        Area: false
                    };
                    break;
                case 'Region':
                    $scope.changed = {
                        Region: false,
                        State: false,
                        Cluster: false,
                        City: false,
                        Area: false
                    };
                    break;
                case 'State':
                    $scope.changed = {
                        Region: false,
                        State: false,
                        Cluster: false,
                        City: false,
                        Area: false
                    };
                    break;
                case 'Cluster':
                    $scope.changed = {
                        Region: false,
                        State: false,
                        Cluster: false,
                        City: false,
                        Area: false
                    };
                    break;
                case 'District':
                    $scope.changed = {
                        Region: false,
                        State: false,
                        Cluster: false,
                        City: false,
                        Area: false
                    };
                    break;
                case 'City':
                    $scope.changed = {
                        Region: true,
                        State: true,
                        Cluster: true,
                        City: false,
                        Area: false
                    }; break;
                case 'Area':
                    $scope.changed = {
                        Region: true,
                        State: true,
                        Cluster: true,
                        City: true,
                        Area: false
                    };
                    break;
            }
            $scope.changed = {
                Region: false,
                State: false,
                Cluster: false,
                City: false,
                Area: false
            };
        };

        var setDisplayManager = function (locLevel) {
            switch (locLevel) {
                case 'Country':
                    resetDisplayManager();
                    break;
                case 'Region':
                    $scope.displayManager = {
                        showCountry: true,
                        showCluster: false,
                        showState: false,
                        showRegion: true,
                        showDistrict: false,
                        showCity: false,
                        showArea: false
                    };
                    break;
                case 'State':
                    $scope.displayManager = {
                        showCountry: true,
                        showCluster: false,
                        showState: true,
                        showRegion: false,
                        showDistrict: false,
                        showCity: false,
                        showArea: false
                    };
                    break;
                case 'Cluster':
                    $scope.displayManager = {
                        showCountry: true,
                        showCluster: true,
                        showState: false,
                        showRegion: true,
                        showDistrict: false,
                        showCity: false,
                        showArea: false
                    };
                    break;
                case 'District':
                    $scope.displayManager = {
                        showCountry: true,
                        showCluster: false,
                        showState: false,
                        showRegion: false,
                        showDistrict: true,
                        showCity: false,
                        showArea: false
                    };
                    break;
                case 'City':
                    $scope.displayManager = {
                        showCountry: true,
                        showCluster: false,
                        showState: true,
                        showRegion: false,
                        showDistrict: false,
                        showCity: true,
                        showArea: false
                    };
                    break;
                case 'Area':
                    $scope.displayManager = {
                        showCountry: true,
                        showCluster: false,
                        showState: false,
                        showRegion: false,
                        showDistrict: true,
                        showCity: false,
                        showArea: true,
                    };
                    break;

            }
        };

        $scope.manageDisplay = function (locLevel) {

            resetDisplayManager();
            setDisplayManager(locLevel);
            $scope.workingModel.QueryFor = setQueryForInitial($scope.displayManager);
            $scope.workingModel.DisplayManager = $scope.displayManager;
            $scope.workingModel.SelectedPincodeData = $scope.selectedPincode;
            loadWorkingModel($scope.workingModel);
        };

        var setQueryForInitial = function (displayManager) {
            if (displayManager.showRegion) return 'Region';
            if (displayManager.showState) return 'State';
            if (displayManager.showCluster) return 'Cluster';
            if (displayManager.showDistrict) return 'District';
            if (displayManager.showCity) return 'District';
            if (displayManager.showArea) return 'Area';
        };

        $scope.setWorkingModel = function (loclevel) {
            switch (loclevel.toUpperCase()) {
                case 'COUNTRY':
                    return $scope.displayManager;
                case 'STATE':
                    $scope.workingModel.QueryFor = 'State';
                    loadWorkingModel($scope.workingModel);
                    return loadWorkingModel($scope.workingModel);
                case 'CLUSTER':
                    $scope.workingModel.QueryFor = 'Cluster';
                    loadWorkingModel($scope.workingModel);
                    return loadWorkingModel($scope.workingModel);
                case 'DISTRICT':
                    $scope.workingModel.QueryFor = 'District';
                    loadWorkingModel($scope.workingModel);
                    return loadWorkingModel($scope.workingModel);
                case 'CITY':
                    $scope.workingModel.QueryFor = 'District';
                    loadWorkingModel($scope.workingModel);
                    return loadWorkingModel($scope.workingModel);
                case 'AREA':

                    $scope.workingModel.QueryFor = 'Area';
                    loadWorkingModel($scope.workingModel);
                    return loadWorkingModel($scope.workingModel);
            }
        };

        var setChangedParam = function (changedParam) {
            switch (changedParam) {
                case 'Region':
                    $scope.changed.Region = true;
                    $scope.changed.Cluster = false;
                    break;
                case 'City':
                    $scope.changed = {
                        Area: false,
                        City: true,
                        District: true,
                        Cluster: true,
                        State: true,
                        Region: true
                    };
                    $scope.clusterArray = [];
                    $scope.areaArray = [];
                    break;
                case 'District':
                    $scope.changed =
                    {
                        State: true,
                        Cluster: true,
                        District: true,
                        City: true,
                        Area: false
                    };
                    $scope.areaArray = [];
                    break;
                case 'Area':
                    $scope.changed = {
                        Area: true,
                        City: true,
                        District: true,
                        Cluster: true,
                        State: true,
                        Region: true
                    };
                    break;
                case 'State':
                    $scope.changed = {
                        Area: true,
                        City: false,
                        District: true,
                        Cluster: true,
                        State: true,
                        Region: true
                    };
                    $scope.cityArray = [];
                    $scope.clusterArray = [];
                    break;
            }
        };

        $scope.setQueryFor = function (changedParam) {

            $scope.currentSelected = {
                state: angular.copy($scope.workingModel.SelectedPincodeData.State),
                cluster: angular.copy($scope.workingModel.SelectedPincodeData.Cluster),
                city: angular.copy($scope.workingModel.SelectedPincodeData.City),
                area: angular.copy($scope.workingModel.SelectedPincodeData.Area),
                district: angular.copy($scope.workingModel.SelectedPincodeData.District),
                region: angular.copy($scope.workingModel.SelectedPincodeData.Region),
            };

            if ($csfactory.isNullOrEmptyString(changedParam)) return;
            setChangedParam(changedParam);
            //var locLevel = 'Country';
            if ($csfactory.isEmptyObject($scope.workingModel)) return;
            if ($scope.displayManager.showRegion === true && !$scope.changed.Region) {
                $scope.workingModel.QueryFor = 'Region';
                $scope.workingModel = $scope.setWorkingModel('Region');
                //locLevel = 'Region';
            }
            if ($scope.displayManager.showState === true && !$scope.changed.State) {
                if ($scope.workingModel.ListOfStates.length > 0 && $csfactory.isNullOrEmptyString($scope.workingModel.SelectedPincodeData.State))
                    return;
                $scope.workingModel = $scope.setWorkingModel('State');
                //locLevel = 'State';

            }
            if ($scope.displayManager.showCluster === true && !$scope.changed.Cluster) {
                $scope.workingModel = $scope.setWorkingModel('Cluster');
                $scope.workingModel.QueryFor = 'Cluster';
                //locLevel = 'Cluster';
            }
            if ($scope.displayManager.showDistrict === true && !$scope.changed.District) {
                $scope.workingModel = $scope.setWorkingModel('District');
                $scope.workingModel.QueryFor = 'District';
                //locLevel = 'District';
            }
            if ($scope.displayManager.showCity === true && !$scope.changed.City) {
                if ($csfactory.isNullOrEmptyString($scope.workingModel.SelectedPincodeData.State)) return;
                $scope.workingModel = $scope.setWorkingModel('District');
                //locLevel = 'District';
            }
            if ($scope.displayManager.showArea === true && !$scope.changed.Area) {

                if ($csfactory.isNullOrEmptyString($scope.workingModel.SelectedPincodeData.District)) return;
                $scope.workingModel = $scope.setWorkingModel('Area');
                //locLevel = 'Area';
            }
            //$scope.setWorkingModel(locLevel);

        };

        var loadWorkingModel = function (workingModle) {
            restApi.customPOST(workingModle, 'GetPincodeData').then(function (data) {

                $scope.workingModel = data;
                if (!$scope.$$phase) {
                    $scope.$apply();
                }

                //$scope.workingModel.SelectedPincodeData.State = $scope.currentSelected.state;
                //$scope.workingModel.SelectedPincodeData.City = $scope.currentSelected.city;
                //$scope.workingModel.SelectedPincodeData.Cluster = $scope.currentSelected.cluster;
                //$scope.workingModel.SelectedPincodeData.Area = $scope.currentSelected.area;
                //$scope.workingModel.SelectedPincodeData.District = $scope.currentSelected.district;
                //$scope.workingModel.SelectedPincodeData.Region = $scope.currentSelected.region;
            });
        };
        //#endregion

        init();
    }]);

(csapp.factory('pincodeMngr', ['Restangular', '$csfactory', '$csnotify', function (rest, $csfactory, $csnotify) {
    var restApi = rest.all('PaymentDetailsApi');

    var getState = function () {
        return restApi.customGET('GetStateList');
    };

    var getCluster = function (reportdToId) {
        if (angular.isUndefined(reportdToId) || reportdToId == "") {
            return [];
        }
        return restApi.customGET('GetClusters', { 'Id': reportdToId });//, { 'state': states }
    };

    var getRegionData = function (state, clusterList) {

        var region = _.find(clusterList, function (item) {
            if (item.State.toUpperCase() === state.toUpperCase())
                return item;
        });
        if (angular.isDefined(region)) {
            return region.Region.toUpperCase();
        }
    };

    var getCity = function (cluster, clusterList) {

        var array = [];
        var city = _.filter(clusterList, function (item) {
            if (item.Cluster.toUpperCase() === cluster) {
                return item.City;
            }
            return "";
        });
        for (var i = 0; i < city.length; i++) {
            var added = _.find(array, function (item) {
                if (item == city[i].City.toUpperCase())
                    return item;
            });
            if ($csfactory.isNullOrEmptyString(added)) {
                array.push(city[i].City.toUpperCase());
            }
        }
        return array.sort();
    };

    var getDistrict = function (city, clusterList) {
        var district = _.find(clusterList, function (item) {
            if (angular.isDefined(city)) {
                if (item.City.toUpperCase() === city.toUpperCase()) {
                    return item.District;
                }
            }
            return '';
        });
        if (angular.isDefined(district))
            return district.District;
    };

    var getArea = function (city, clusterList) {
        var array = [];
        if (angular.isUndefined(city))
            return "";
        var area = _.filter(clusterList, function (item) {
            if (item.City.toUpperCase() == city.toUpperCase()) {
                return item.Area;
            }
            return '';
        });
        for (var i = 0; i < area.length; i++) {
            var added = _.find(array, function (item) {
                if (item.toUpperCase() == area[i].Area.toUpperCase())
                    return item;
            });
            if ($csfactory.isNullOrEmptyString(added)) {
                array.push(area[i].Area.toUpperCase());
            }
        }
        return array.sort();
    };

    var clearOnLocationChange = function (location, stakeWork) {
        if (!$csfactory.isEmptyObject(stakeWork)) {
            switch (location.toUpperCase()) {
                case "COUNTRY":
                    stakeWork.Region = "";
                case "REGION":
                    stakeWork.State = "";
                case "STATE":
                    stakeWork.Cluster = "";
                case "CLUSTER":
                    stakeWork.City = "";
                case "CITY":
                    stakeWork.Area = "";
                    break;
            }
        }
    };

    var replaceWithAll = function (location, stakeWork) {
        if (!$csfactory.isEmptyObject(stakeWork)) {
            switch (location.toUpperCase()) {
                case "COUNTRY":
                    stakeWork.Region = "ALL";
                case "REGION":
                    stakeWork.State = "ALL";
                case "STATE":
                    stakeWork.Cluster = "ALL";
                case "CLUSTER":
                    stakeWork.District = "ALL";
                case "DISTRICT":
                    stakeWork.City = "ALL";
                case "CITY":
                    stakeWork.Area = "ALL";
                    break;
                default:
                    break;
            }
        }
    };

    var checkDuplicate = function (stakeWork, stakeinfo, loclevel, list) {

        if (list.length > 0) {
            if (stakeinfo.length > 0) {
                switch (loclevel) {
                    case "Country":
                        var duplic = _.where(stakeinfo, {
                            'Products': stakeWork.Products,
                            'Region': stakeWork.Region,
                            'State': stakeWork.State,
                            'Cluster': stakeWork.Cluster,
                            'City': stakeWork.City,
                            'Area': stakeWork.Area,
                            'BucketStart': stakeWork.BucketStart
                        });
                        if (duplic.length > 0) {
                            $csnotify.error("Record already exists");
                            return duplic;
                        }
                        break;
                    case "State":
                        var len0 = list.length;
                        var copyState = angular.copy(list);
                        for (var h = 0; h < len0; h++) {
                            var same0 = _.where(stakeinfo, {
                                'Products': stakeWork.Products,
                                'Region': stakeWork.Region,
                                'State': copyState[h],
                                'Cluster': stakeWork.Cluster,
                                'City': stakeWork.City,
                                'Area': stakeWork.Area,
                                'BucketStart': stakeWork.BucketStart
                            });
                            if (same0.length > 0) {
                                var index0 = list.indexOf(same0[0].Cluster);
                                list.splice(index0, 1);
                                $csnotify.error("Record already exists");
                            }
                        }
                        break;
                    case "Region":
                        var len4 = list.length;
                        var copyRegion = angular.copy(list);
                        for (var x = 0; x < len4; x++) {
                            var same5 = _.where(stakeinfo, {
                                'Products': stakeWork.Products,
                                'Region': copyRegion[x],
                                'State': stakeWork.State,
                                'Cluster': stakeWork.Cluster,
                                'City': stakeWork.City,
                                'Area': stakeWork.Area,
                                'BucketStart': stakeWork.BucketStart
                            });
                            if (same5.length > 0) {
                                var index5 = list.indexOf(same5[0].Cluster);
                                list.splice(index5, 1);
                                $csnotify.error("Record already exists");
                            }
                        }
                        break;
                    case "Cluster":
                    case "District":
                        var len = list.length;
                        var copyOfList = angular.copy(list);
                        for (var i = 0; i < len; i++) {
                            var same = _.where(stakeinfo, {
                                'Products': stakeWork.Products,
                                'Region': stakeWork.Region,
                                'State': stakeWork.State,
                                'District': copyOfList[i],
                                'City': stakeWork.City,
                                'Area': stakeWork.Area,
                                'BucketStart': stakeWork.BucketStart
                            });
                            if (same.length > 0) {
                                var index = list.indexOf(same[0].District);
                                list.splice(index, 1);
                                $csnotify.error("Record already exists");
                            }
                        }
                        break;
                    case "City":
                        var len1 = list.length;
                        var copycity = angular.copy(list);
                        for (var j = 0; j < len1; j++) {
                            var same1 = _.where(stakeinfo, {
                                'Products': stakeWork.Products,
                                'Region': stakeWork.Region,
                                'State': stakeWork.State,
                                'Cluster': stakeWork.Cluster,
                                'City': copycity[j],
                                'Area': stakeWork.Area,
                                'BucketStart': stakeWork.BucketStart
                            });
                            if (same1.length > 0) {
                                var index1 = list.indexOf(same1[0].City);
                                list.splice(index1, 1);
                                $csnotify.error("Record already exists");
                            }
                        }
                        break;
                    case "Area":
                        var len2 = list.length;
                        var copyArea = angular.copy(list);
                        for (var k = 0; k < len2; k++) {
                            var same2 = _.where(stakeinfo, {
                                'Products': stakeWork.Products,
                                'Region': stakeWork.Region,
                                'State': stakeWork.State,
                                'Cluster': stakeWork.Cluster,
                                'City': stakeWork.City,
                                'Area': copyArea[k],
                                'BucketStart': stakeWork.BucketStart
                            });
                            if (same2.length > 0) {
                                var index2 = list.indexOf(same2[0].Area);
                                list.splice(index2, 1);
                                $csnotify.error("Record already exists");
                            }
                        }
                        break;
                }
            }
        } else {
            var dup = _.where(stakeinfo, {
                'Products': stakeWork.Products,
                'Region': stakeWork.Region,
                'State': stakeWork.State,
                'Cluster': stakeWork.Cluster,
                'City': stakeWork.City,
                'Area': stakeWork.Area,
                'District': stakeWork.District,
                'BucketStart': stakeWork.BucketStart
            });
            if (dup.length > 0) {
                $csnotify.error("Record already exists");
                return dup;
            }
        }
    };

    return {
        GetState: getState,
        GetCluster: getCluster,
        GetCity: getCity,
        GetArea: getArea,
        ClearOnLocationChange: clearOnLocationChange,
        ReplaceWithAll: replaceWithAll,
        CheckDuplicate: checkDuplicate,
        GetDistrict: getDistrict,
        GetRegionData: getRegionData
    };
}])
);


csapp.controller("multiSelectController", ["$scope", "$csfactory", "modalData", "$modalInstance", function ($scope, $csfactory, modalData, $modalInstance) {


    (function () {
        $scope.modalData = modalData;
    })();


    $scope.closeModal = function () {
        $modalInstance.close();
    };

    $scope.multipleModalOk = function (locLevel) {
        if (locLevel === 'City')
            modalData.SelectedPincodeData['District'] = "";
        modalData.StakeWork[locLevel] = "";
        modalData.SelectedPincodeData[locLevel] = "";

        $modalInstance.close(modalData);
    };


    $scope.checkLength = function (locLevel) {
        switch (locLevel) {
            case 'District':
            case 'Cluster':
                return modalData.clusterArray.length;

            case 'Area':
                return modalData.areaArray.length;
        }
    };

    $scope.chooseMultiple = function (data, selected, locLevel) {
        switch (locLevel) {
            case "Cluster":
            case 'District':

                if (modalData.clusterArray.indexOf(data) === -1) {
                    modalData.clusterArray.push(data);
                } else {
                    modalData.clusterArray.splice(modalData.clusterArray.indexOf(data), 1);
                }
                break;
            case "Region":

                if (modalData.regionArray.indexOf(data) === -1) {
                    modalData.regionArray.push(data);
                } else {
                    modalData.regionArray.splice(modalData.regionArray.indexOf(data), 1);
                }
                break;
            case "State":

                if (modalData.stateArray.indexOf(data) === -1) {
                    modalData.stateArray.push(data);
                } else {
                    modalData.stateArray.splice(modalData.stateArray.indexOf(data), 1);
                }
                break;
            case "City":

                if (modalData.cityArray.indexOf(data) === -1) {
                    modalData.cityArray.push(data);
                } else {
                    modalData.cityArray.splice(modalData.cityArray.indexOf(data), 1);
                }
                break;
            case "Area":

                if (modalData.areaArray.indexOf(data) === -1) {
                    modalData.areaArray.push(data);
                } else {
                    modalData.areaArray.splice(modalData.areaArray.indexOf(data), 1);
                }
        }
    };

    $scope.ticks = function (data) {
        switch (modalData.LocationLevel.toUpperCase()) {
            case "REGION":
                return (modalData.regionArray.indexOf(data) !== -1);
            case "STATE":
                return (modalData.stateArray.indexOf(data) !== -1);
            case "CLUSTER":
            case "DISTRICT":
                return (modalData.clusterArray.indexOf(data) !== -1);
            case "CITY":
                return (modalData.cityArray.indexOf(data) !== -1);
            case "AREA":
                return (modalData.areaArray.indexOf(data) !== -1);
        }
    };

}]);

csapp.controller("deleteWorkingController", ["$scope", "$csfactory", "modalData", "$modalInstance",
    function ($scope, $csfactory, modalData, $modalInstance) {

        (function () {
            $scope.modalData = modalData;
        })();


        $scope.setEndDate = function (data, endDate) {
            var date = angular.copy(endDate);
            data.EndDate = date;
            // ReSharper disable AssignedValueIsNeverUsed
            endDate = '';
            // ReSharper restore AssignedValueIsNeverUsed
            $modalInstance.close();
        };

        $scope.cancelEndDate = function (currentDeleteData) {
            currentDeleteData.EndDate = undefined;
            $modalInstance.close();
        };


    }]);



//if EditMode
//if ($scope.$parent.isEditMode) {
//    
//    $scope.Stakeinfo = $scope.$parent.Stakeholder.StkhWorkings;
//    //$scope.Hierarchy = $scope.$parent.Stakeholder.Hierarchy;
//    var len = $scope.Stakeinfo.length;
//    for (var i = 0; i < len; i++) {
//        $scope.replace($scope.Stakeinfo[i]);
//    }
//    $scope.$parent.WizardData.SelHierarchy.Designation = $scope.$parent.Stakeholder.Hierarchy.Id;
//    $scope.WorkingData.LocationLevel = $scope.$parent.Stakeholder.LocationLevel;
//    if ($scope.WorkingData.LocationLevelArray.length === 0) {
//        $scope.WorkingData.LocationLevelArray.push($scope.WorkingData.LocationLevel);
//        //$scope.setLocationLevel();
//    }
//    $log.info($scope.Stakeinfo);
//}



//LocationLevel Complex
//if ($scope.WorkingData.LocationLevel !== 'Region') {
//     if ($scope.WorkingData.LocationLevelArray.length === 1) {
//         $scope.WorkingData.LocationLevel = $scope.WorkingData.LocationLevelArray[0];
//         locValue = getLocationLevel($scope.WorkingData.LocationLevel);
//         $scope.WorkingData.LocationLevelArray.push({ level: $scope.WorkingData.LocationLevel.toUpperCase(), value: locValue });

//     } else {
//         _.forEach($scope.WorkingData.LocationLevelArray, function (item) {
//             locValue = getLocationLevel(item);
//             $scope.WorkingData.LocationLevelArray.push({ level: item.toUpperCase(), value: locValue });
//         });
//         $scope.WorkingData.LocationLevelArray = $scope.WorkingData.LocationLevelArray;
//     }
// }







//assigning region
//if ($scope.WorkingData.LocationLevel.toUpperCase() !== 'REGION') {
//    var x = $scope.enableLocation();
//    if (x > 2) {
//        if ($scope.clusterArray.length === 0) {
//            var currPinData = _.find($scope.WorkingData.clusterList, function (item) {
//                if (item.Cluster.toUpperCase() === stakeWork.Cluster.toUpperCase())
//                    return item;
//            });
//            stakeWork.State = currPinData.State;
//            if (!$csfactory.isNullOrEmptyString(stakeWork.State)) {
//                stakeWork.Region = pincodeMngr.GetRegionData(stakeWork.State, $scope.WorkingData.RegionList); //$scope.WorkingData.clusterList
//            }
//        }
//    }
//}
//pincodeMngr.ReplaceWithAll($scope.WorkingData.LocationLevel, stakeWork);
////assigning district
//if (!$csfactory.isNullOrEmptyString(stakeWork.City)) {
//    if (stakeWork.City != 'ALL') {
//        stakeWork.District = pincodeMngr.GetDistrict(stakeWork.City, $scope.WorkingData.clusterList);
//    } else {
//        stakeWork.District = 'ALL';
//    }
//}
//var bucketValue = angular.copy(stakeWork.BucketStart);
////has no Buckets
//if (bucketValue.length == 0) {
//    //var same = pincodeMngr.CheckDuplicate(stakeWork, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel);
//    // if duplicate, no need to add
//    switch ($scope.WorkingData.LocationLevel) {
//        case "Country":
//            $scope.dupCountry = pincodeMngr.CheckDuplicate(stakeWork, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.clusterArray);
//            break;
//        case "Region":
//            $scope.dupRegion = pincodeMngr.CheckDuplicate(stakeWork, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.regionArray);
//            break;
//        case "Cluster":
//            $scope.dupCluster = pincodeMngr.CheckDuplicate($scope.workingModel.SelectedPincodeData, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.clusterArray);
//            break;
//        case "State":
//            $scope.dupState = pincodeMngr.CheckDuplicate(stakeWork, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.stateArray);
//            break;
//        case "City":
//            $scope.dupCity = pincodeMngr.CheckDuplicate(stakeWork, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.cityArray);
//            break;
//        case "Area":
//            $scope.dupArea = pincodeMngr.CheckDuplicate(stakeWork, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.areaArray);
//    }
//    //$scope.addMultipleEntries(stakeWork, $scope.WorkingData.LocationLevel);
//    $scope.addMultipleEntries($scope.workingModel.SelectedPincodeData, $scope.WorkingData.LocationLevel);
//}
////hasBucket
//for (var i = 0; i < bucketValue.length; i++) {
//    stakeWork.BucketStart = bucketValue[i];
//    switch ($scope.WorkingData.LocationLevel) {
//        case "Country":
//            $scope.dupCountry = pincodeMngr.CheckDuplicate(stakeWork, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.clusterArray);
//            break;
//        case "Region":
//            $scope.dupRegion = pincodeMngr.CheckDuplicate(stakeWork, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.regionArray);
//            break;
//        case "Cluster":
//            $scope.dupCluster = pincodeMngr.CheckDuplicate(stakeWork, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.clusterArray);
//            break;
//        case "State":
//            $scope.dupState = pincodeMngr.CheckDuplicate(stakeWork, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.stateArray);
//            break;
//        case "City":
//            $scope.dupCity = pincodeMngr.CheckDuplicate(stakeWork, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.cityArray);
//            break;
//        case "Area":
//            $scope.dupArea = pincodeMngr.CheckDuplicate(stakeWork, $scope.WorkingData.PayWorkModel.WorkList, $scope.WorkingData.LocationLevel, $scope.areaArray);
//            break;
//    }
//    if (!$csfactory.isNullOrEmptyString(stakeWork.City)) {
//        if (stakeWork.City != 'ALL') {
//            stakeWork.District = pincodeMngr.GetDistrict(stakeWork.City, $scope.WorkingData.clusterList);
//        } else {
//            stakeWork.District = 'ALL';
//        }
//    }
//    $scope.addMultipleEntries(stakeWork, $scope.WorkingData.LocationLevel);
//}
//$scope.showAddedData = true;
//$scope.showButtons = true;
//$scope.StakeWork.BucketStart = '';
//pincodeMngr.ClearOnLocationChange($scope.WorkingData.LocationLevel, stakeWork);
//$scope.cityArray = [];
//$scope.regionArray = [];
//$scope.stateArray = [];
//$scope.clusterArray = [];
//$scope.areaArray = [];
//$scope.buckArray = [];
//$scope.selAll = false;
//$scope.selected = false;




//Adding data to the list after deleting 
//if ($scope.WorkingData.Hierarchy.HasBuckets === false) {
//    switch ($scope.WorkingData.LocationLevel) {
//        case "Region":
//            $scope.uniqueReg.push(data.Region);
//            $scope.uniqueReg.sort();
//            break;
//        case "Cluster":
//            var state = _.find($scope.WorkingData.clusterList, function (item) {
//                if (item.Cluster.toUpperCase() === data.Cluster.toUpperCase()) {
//                    return item;
//                }
//            });
//            if (state.State.toUpperCase() === data.State.toUpperCase())
//                $scope.uniqueClus.push(data.Cluster.toUpperCase());
//            $scope.uniqueClus.sort();
//            break;
//        case "State":
//            $scope.WorkingData.StateList.push(data.State);
//            $scope.WorkingData.StateList.sort();
//            break;
//        case "City":
//            var city = _.find($scope.WorkingData.clusterList, function (item) {
//                if (item.City.toUpperCase() === data.City.toUpperCase()) {
//                    return item;
//                }
//            });
//            if (city.Cluster.toUpperCase() === $scope.StakeWork.Cluster.toUpperCase())
//                $scope.WorkingData.cityList.push(data.City);
//            $scope.WorkingData.cityList.sort();
//            break;
//        case "Area":
//            var area = _.find($scope.WorkingData.clusterList, function (item) {
//                if (item.Area.toUpperCase() === data.Area.toUpperCase()) {
//                    return item;
//                }
//            });
//            if (area.City.toUpperCase() === $scope.StakeWork.City.toUpperCase())
//                $scope.WorkingData.areaList.push(data.Area);
//            $scope.WorkingData.areaList.sort();
//            break;
//    }
//}