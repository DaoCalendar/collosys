/*globals  angular, console */

(
csapp.controller('liststakeholders', ['$scope', '$http', 'Restangular', '$csnotify', "$timeout", '$csfactory',
    function ($scope, $http, rest, $csnotify, $timeout, $csfactory) {
        'use strict';

        //list for all stakeholders
        $scope.listOfStakeholders = [];

        //list for all hierarchy
        $scope.listOfHierarchy = [];

        //save index
        $scope.saveindex = -1;

        //index of first clicked stakeholder for multiedit
        $scope.multiEditIndex = -1;

        //used for show grid in popup
        $scope.editFor = '';

        //#region variables for track edit
        $scope.isWorkingChange = false;
        $scope.isPaymentChange = false;
        $scope.isAddressChange = false;

        //#endregion

        //#region data list

        $scope.ProductList = [];
        $scope.LocationList = [];
        $scope.BucketList = [];

        $scope.LinerPolicyList = [];
        $scope.WriteoffPolicyList = [];

        $scope.ReportsToList = [];
        $scope.PincodeList = [];
        $scope.StateList = [];
        $scope.ClusterList = [];
        $scope.Cluster = '';
        //#endregion

        $scope.LocationObj = {};

        $scope.displayAdd = false;

        //#endregion

        //#region get
        var apistake = rest.all('stakeholderapi');
        var apieditstake = rest.all('showstakeholders');
        //Get list of products

        $scope.Pincodes = function (pincode, level) {
            return apistake.customGET('GetPincodes', { pincode: pincode, level: level }).then(function (data) {
                return $scope.PincodeList = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });
        };

        //get clusters
        var timeoutCluster;
        $scope.Clusters = function (cluster) {
            if (timeoutCluster) $timeout.cancel(timeoutCluster);

            timeoutCluster = $timeout(function () {
                return apistake.customGET('GetClusters', { cluster: cluster }).then(function (data) {
                    return $scope.ClusterList = data;
                }, function (data) {
                    $csnotify.error(data.data.Message);
                });
            }, 400);

            return timeoutCluster;
        };

        apistake.customGET('GetProducts').then(function (data) {
            $scope.ProductList = data;
        }, function (data) {
            $csnotify.error(data.data.Message);
        });

        apistake.customGET('GetAllHierarchies').then(function (data) {
            $scope.listOfHierarchy = data;
        }, function (data) {
            $csnotify.error(data.data.Message);
        });

        //get list of variable liner policies 
        apistake.customGET('VariableLinerPolicies').then(function (data) {
            $scope.LinerPolicyList = data;
        }, function (data) {
            $csnotify.error(data.data.Message);
        });

        apistake.customGET('GetStateList').then(function (data) {
            $scope.StateList = data;
        }, function (data) {
            $csnotify.error(data.data.Message);
        });

        //get list of variable writeoff policies
        apistake.customGET('VariableWriteoffPolicies').then(function (data) {
            $scope.WriteoffPolicyList = data;
        }, function (data) {
            $csnotify.error(data.data.Message);
        });

        //get list of buckets
        apistake.customGET('GetBucketList').then(function (data) {
            $scope.BucketList = data;
        }, function (data) {
            $csnotify.error(data.data.Message);
        });

        //#endregion

        //#region lists for delete
        var deleteListAll = [];
        //#endregion

        //stakeholder object
        $scope.Stakeholder = {};

        //stakehierarchy object
        $scope.Hierarchy = {};

        $scope.count = 0;

        //Get list of all stakeholders
        apieditstake.customGET('GetAllStakeholders').then(function (data) {
            if (data.length > 0) {
                $csnotify.success(' stakeholders loaded');
            }
            $scope.listOfStakeholders = data;
            console.log(data);
            $scope.displayAdd = true;
        }, function (data) {
            $csnotify.error(data.data.Message);
            $scope.displayAdd = true;
        });

        //get stakeholder
        $scope.getStakeholder = function (stakeholder, index) {
            debugger;
            $scope.saveindex = index;
            getHierarchy(stakeholder);

            $scope.count = 1;
            return;
        };

        //save stakeholder
        $scope.save = function (value) {
            debugger;
            value = setTrackers(value);
            value.Status = 'Submitted';
            //post stakeholder to api
            apieditstake.customPOST(value, 'SaveEditedStakeholder').then(function (data) {
                resetTrackers();
                deleteList();
                $scope.listOfStakeholders[$scope.saveindex] = data;
                $scope.Stakeholder = {};
                $scope.count = 0;
                $csnotify.success('Staekholder saved');
            }, function (data) {
                $csnotify.error(data.data.Message);
            });
        };

        //cancel stakeholder
        $scope.cancel = function () {
            $scope.Stakeholder = {};
            $scope.count = 0;
            resetTrackers();
        };

        $scope.validateForm = function (formdetails) {
            //isAddressChange || isWorkingChange || isPaymentChange || formDetails.$valid 
            var result = ($scope.isAddressChange || $scope.isWorkingChange || $scope.isPaymentChange);
            result = result || (formdetails.$dirty && formdetails.$valid);
            return result;
        };

        //check working is duplicate or not
        var checkWorking = function (working, list) {
            debugger;
            var i;
            var result = false;
            console.log(working);
            for (i = 0; i < list.length; i++) {
                console.log(list[i]);
                result = (list[i].Products === working.Products);
                result = result && (list[i].Region === working.Region);
                result = result && (list[i].Cluster === working.Cluster);
                result = result && (list[i].State === working.State);
                result = result && (list[i].District === working.District);
                result = result && (list[i].City === working.City);
                result = result && (list[i].Area === working.Area);
                if (working.BucketStart) {
                    result = result && (list[i].BucketStart === working.BucketStart);
                }

                if (result === true) {
                    break;
                }
            }
            return result;
        };

        //add working to list
        $scope.AddWorking = function (stakeholder, hierarchy) {
            stakeholder.Working.StartDate = new Date();
            stakeholder.Working.Country = 'India';
            var val;
            if (stakeholder.Working) {
                if (stakeholder.LocationLevel === 'Region') {
                    processRegionWorking(stakeholder, hierarchy);
                    postWorkingprocess(stakeholder, hierarchy);
                }
                if (stakeholder.LocationLevel === 'Country') {
                    processCountryWorking(stakeholder, hierarchy);
                    postWorkingprocess(stakeholder, hierarchy);
                }
                if (stakeholder.LocationLevel === 'State') {
                    apistake.customGET('GetRegionOfState', { state: stakeholder.Working.State }).then(function (data) {
                        processStateWorking(stakeholder, data, hierarchy);
                        postWorkingprocess(stakeholder, hierarchy);
                        return;
                    });
                }
                if (stakeholder.LocationLevel === 'Cluster') {
                    val = processClusterWorking(stakeholder);
                    if (val) {
                        postWorkingprocess(stakeholder, hierarchy);
                    }
                    //apistake.customGET('GetRegionOfState', { state: stakeholder.Working.State }).then(function (data) {
                    //    processClusterWorking(stakeholder, data, hierarchy);
                    //    postWorkingprocess(stakeholder, hierarchy);
                    //    return;
                    //});
                }
                if (stakeholder.LocationLevel === 'City') {
                    val = processCityWorking(stakeholder, hierarchy);
                    if (val) {
                        postWorkingprocess(stakeholder, hierarchy);
                    }
                }

                if (stakeholder.LocationLevel === 'Area') {
                    val = processAreaWorking(stakeholder, hierarchy);
                    if (val) {
                        postWorkingprocess(stakeholder, hierarchy);
                    }

                }
            }
        };

        var postWorkingprocess = function (stakeholder, hierarchy) {
            var obj;
            var result;
            if (hierarchy.HasBuckets) {
                for (var i = 0; i < stakeholder.BucketStart.length; i++) {
                    stakeholder.Working.BucketStart = stakeholder.BucketStart[i];
                    obj = angular.copy(stakeholder.Working);
                    result = checkWorking(stakeholder.Working, $scope.Stakeholder.StkhWorkings);
                    if (!result) {
                        $scope.Stakeholder.StkhWorkings.push(obj);
                    }
                }
            } else {
                obj = angular.copy(stakeholder.Working);
                result = checkWorking(stakeholder.Working, $scope.Stakeholder.StkhWorkings);
                if (!result) {
                    $scope.Stakeholder.StkhWorkings.push(obj);
                }
            }
            $scope.isWorkingChange = true;
            $scope.LocationObj = {};
            stakeholder.Working = {};
            stakeholder.BucketStart = [];
            $scope.Pincode = null;
            $scope.Cluster = '';
        };

        var processCountryWorking = function (stakeholder) {
            stakeholder.Working.Region = 'All';
            stakeholder.Working.Cluster = 'All';
            stakeholder.Working.State = 'All';
            stakeholder.Working.District = 'All';
            stakeholder.Working.City = 'All';
            stakeholder.Working.Area = 'All';
        };

        var processRegionWorking = function (stakeholder) {
            stakeholder.Working.Cluster = 'All';
            stakeholder.Working.State = 'All';
            stakeholder.Working.District = 'All';
            stakeholder.Working.City = 'All';
            stakeholder.Working.Area = 'All';
        };

        var processStateWorking = function (stakeholder, data) {
            stakeholder.Working.Region = data;

            stakeholder.Working.Cluster = 'All';
            stakeholder.Working.District = 'All';
            stakeholder.Working.City = 'All';
            stakeholder.Working.Area = 'All';
        };

        var processClusterWorking = function (stakeholder) {
            var data = _.find($scope.ClusterList, { 'Cluster': $scope.Cluster });
            if (angular.isDefined(data)) {
                $scope.LocationObj = data;
            } else {
                return false;
            }
            stakeholder.Working.Region = $scope.LocationObj.Region;
            stakeholder.Working.State = $scope.LocationObj.State;
            stakeholder.Working.Cluster = $scope.LocationObj.Cluster;
            stakeholder.Working.District = 'All';
            stakeholder.Working.City = 'All';
            stakeholder.Working.Area = 'All';
            return true;
        };

        var processCityWorking = function (stakeholder) {
            var data = _.find($scope.PincodeList, { 'City': $scope.Pincode });
            if (angular.isDefined(data)) {
                $scope.LocationObj = data;
            } else {
                return false;
            }
            console.log($scope.LocationObj);
            stakeholder.Working.Region = $scope.LocationObj.Region;
            stakeholder.Working.Cluster = $scope.LocationObj.Cluster;
            stakeholder.Working.State = $scope.LocationObj.State;
            stakeholder.Working.District = $scope.LocationObj.District;
            stakeholder.Working.City = $scope.LocationObj.City;
            stakeholder.Working.Area = 'All';
            return true;
        };

        var processAreaWorking = function (stakeholder) {
            var data = _.find($scope.PincodeList, { 'City': $scope.Pincode });
            if (angular.isDefined(data)) {
                $scope.LocationObj = data;
            } else {
                return false;
            }
            stakeholder.Working.Region = $scope.LocationObj.Region;
            stakeholder.Working.Cluster = $scope.LocationObj.Cluster;
            stakeholder.Working.State = $scope.LocationObj.State;
            stakeholder.Working.District = $scope.LocationObj.District;
            stakeholder.Working.City = $scope.LocationObj.City;
            stakeholder.Working.Area = $scope.LocationObj.Area;
            return true;
        };

        //add payment to list
        $scope.AddPayment = function (value) {
            if (value) {
                //value.Payment.Stakeholder = value;
                $scope.isPaymentChange = true;
                $scope.Stakeholder.StkhPayments.push(value.Payment);
                value.Payment = null;
            }
        };

        $scope.addressCount = 0;

        //add address to list
        $scope.AddAddress = function (stakeholder, isMultiple) {
            $scope.isAddressChange = true;
            $scope.Stakeholder.GAddress.push(stakeholder.Address);
            if (!isMultiple) {
                $scope.addressCount = 1;
            }
        };

        $scope.AddAddressSingleIndividual = function (stakeholder, isMultiple) {
            $scope.isAddressChange = true;
            var data = new custlist('Address', $scope.Stakeholder.GAddress[0].Id);
            deleteListAll.push(data);
            $scope.Stakeholder.GAddress = [];
            $scope.Stakeholder.GAddress.push(stakeholder.Address);
            if (!isMultiple) {
                $scope.addressCount = 1;
            }
        };

        //check address is single 
        $scope.isSingleAddress = function (formAddress, hasMultiple) {
            if (formAddress) {
                if (!hasMultiple) {
                    $scope.isAddressChange = true;
                }
            }
        };

        $scope.checkDates = function (leavingDate, joiningDate) {
            var checkDates = moment(leavingDate) < moment(joiningDate);
            return !checkDates;
        };

        $scope.SalDetails = {
            pfEmployer: 0,
            pfEmployee: 0,
            totalNotEsic: 0,
            esicEmployee: 0,
            esicEmployer: 0,
            serviceCharge: 0,
            serviceTax: 0,
            servPer: 0,
            total: 0
        };

        $scope.TotalPay = function (payment) {
            var reportStakeholder = null;
            var serviceCharge = 1;
            if (angular.isUndefined($scope.Stakeholder)) {
                return 0;
            }

            if (angular.isDefined($scope.Stakeholder.ReportsTo)) {
                reportStakeholder = _.find($scope.ReportsToList, function (stake) { return stake.Id == $scope.Stakeholder.ReportsTo; });
                if (angular.isUndefined(reportStakeholder) || $csfactory.isNullOrEmptyArray(reportStakeholder.StkhPayments)) {
                    serviceCharge = 0;
                } else {
                    serviceCharge = Number(parseFloat(reportStakeholder.StkhPayments[0].ServiceCharge));
                    $scope.SalDetails.servPer = serviceCharge;
                }

            } else {
                serviceCharge = 0;
            }

            var basic = 0;
            var hra = 0;
            var other = 0;
            if (angular.isUndefined(payment)) {
                return 0;
            }
            if (angular.isDefined(payment.FixpayBasic)) {
                basic = Number(payment.FixpayBasic);
            }
            if (angular.isDefined(payment.FixpayHra)) {
                hra = Number(payment.FixpayHra);
            }
            if (angular.isDefined(payment.FixpayOther)) {
                other = Number(payment.FixpayOther);
            }
            if (Number(basic) !== 0) {
                $scope.SalDetails.pf = (Number(basic) * 12) / 100;
                $scope.SalDetails.totalNotEsic = Number(basic) + Number(hra) + Number(other);
                $scope.SalDetails.esicEmployee = (($scope.SalDetails.totalNotEsic * 1.75) / 100);
                $scope.SalDetails.esicEmployee = Number(parseFloat($scope.SalDetails.esicEmployee).toFixed(2));
                $scope.SalDetails.esicEmployer = ($scope.SalDetails.totalNotEsic * 4.5) / 100;
                $scope.SalDetails.esicEmployer = Number(parseFloat($scope.SalDetails.esicEmployer).toFixed(2));

                $scope.SalDetails.serviceCharge = ((Number(basic) * serviceCharge) / 100);
                $scope.SalDetails.serviceCharge = Number(parseFloat($scope.SalDetails.serviceCharge).toFixed(2));

                $scope.SalDetails.serviceTax = ((Number(basic) * 12.36) / 100);
                $scope.SalDetails.serviceTax = Number(parseFloat($scope.SalDetails.serviceTax).toFixed(2));

                $scope.SalDetails.total = $scope.SalDetails.pf * 2 +
                   $scope.SalDetails.totalNotEsic +
                            $scope.SalDetails.esicEmployee +
                            $scope.SalDetails.esicEmployer +
                            $scope.SalDetails.serviceCharge +
                            $scope.SalDetails.serviceTax;
                $scope.SalDetails.total = $scope.SalDetails.total.toFixed(2);
            }
            return $scope.SalDetails.total;
        };

        //delete from working list
        $scope.DeleteWorking = function (workingIndex) {
            $scope.isWorkingChange = true;

            var data = new custlist('Working', $scope.Stakeholder.StkhWorkings[workingIndex].Id);
            deleteListAll.push(data);

            $scope.Stakeholder.StkhWorkings.splice(workingIndex, 1);
        };

        //delete from payment list
        $scope.DeletePayment = function (paymentIndex) {
            $scope.isPaymentChange = true;
            var data = new custlist('Payment', $scope.Stakeholder.StkhPayments[paymentIndex].Id);
            deleteListAll.push(data);
            $scope.Stakeholder.StkhPayments.splice(paymentIndex, 1);
        };

        //delete from address list
        $scope.DeleteAddress = function (addressIndex) {
            $scope.isAddressChange = true;
            var data = new custlist('Address', $scope.Stakeholder.GAddress[addressIndex].Id);
            deleteListAll.push(data);
            $scope.Stakeholder.GAddress.splice(addressIndex, 1);
        };

        var deleteList = function () {
            var success = false;
            apistake.customPOST(deleteListAll, 'DeleteLists').then(function () {
                success = true;
                deleteListAll = [];
            }, function (data) {
                $csnotify.error(data.data.Message);
            });

            return success;
        };

        //get hierarchy object on base of designation and hierarchy
        var getHierarchy = function (stakeholder) {

            apistake.customGET('GetHierarchyWithId', { hierarchyId: stakeholder.HierarchyId })
                .then(function (data) {
                    $scope.Hierarchy = data;
                    apistake.customGET('GetReportsToInHierarchy', { reportsto: $scope.Hierarchy.ReportsTo })
                        .then(function (data2) {
                            $scope.ReportsToList = data2;
                            $scope.Stakeholder = stakeholder;
                        }, function (data2) {
                            $scope.Stakeholder = stakeholder;
                            $csnotify.error(data2.data.Message);
                        });

                });

        };

        //get stakeholder for multiedit
        $scope.getStakeholderForMulti = function (index, editfor) {
            $scope.count = 0;
            $scope.editFor = editfor;
            $scope.Stakeholder = angular.copy($scope.listOfStakeholders[index]);
            getHierarchy($scope.Stakeholder.Designation, $scope.Stakeholder.Hierarchy);
            $scope.multiEditIndex = index;
        };

        //model popupoptions
        $scope.modelOption = {
            backdropFade: true,
            dialogFade: true
        };

        //close model function
        $scope.closeModel = function () {
            resetTrackers();
            $scope.multiEditIndex = -1;
            $scope.editFor = '';
            $scope.Stakeholder = {};
            $scope.addressCount = 0;
            $scope.count = 0;
        };

        //get next stakeholder
        $scope.Nextbtn = function (multiEditIndex, editfor) {
            resetTrackers();
            multiEditIndex = multiEditIndex + 1;
            $scope.multiEditIndex = multiEditIndex;
            $scope.addressCount = 0;
            console.log('All stakeholders' + $scope.listOfStakeholders.length);
            if (multiEditIndex <= $scope.listOfStakeholders.length) {
                $scope.getStakeholderForMulti(multiEditIndex, editfor);
            }
        };

        //previous stakeholder
        $scope.Prevbtn = function (multiEditIndex, editfor) {
            if (multiEditIndex != 0) {
                $scope.addressCount = 0;
                $scope.multiEditIndex = multiEditIndex - 1;
                $scope.getStakeholderForMulti(multiEditIndex - 1, editfor);
            }


        };

        //save data and proceed to next
        $scope.saveNext = function (multiEditIndex, editfor) {
            $scope.Stakeholder.Status = 'Submitted';
            $scope.Stakeholder = setTrackersMulti($scope.Stakeholder, editfor);
            //TODO : harish : commented call to stakefactory as it was undefined.
            //stakeFactory.saveStakeholderWithSaveAndNext($scope.Stakeholder).then(function (data) {
            //    deleteList();
            //    $scope.listOfStakeholders[multiEditIndex] = data;
            //    $scope.Nextbtn(multiEditIndex, editfor);
            //});
        };

        var setTrackers = function (stakeholder) {

            stakeholder.IsPaymentChange = $scope.isPaymentChange || stakeholder.IsPaymentChange;
            stakeholder.IsWorkingChange = $scope.isWorkingChange || stakeholder.IsWorkingChange;
            stakeholder.IsAddressChange = $scope.isAddressChange || stakeholder.IsAddressChange;
            return stakeholder;
        };

        var setTrackersMulti = function (stakeholder, editfor) {
            console.log(editfor);
            if (editfor === 'Address') {
                stakeholder.IsAddressChange = true;
            }
            if (editfor === 'Payment') {
                stakeholder.IsPaymentChange = true;
            }
            if (editfor === 'Working') {
                stakeholder.IsWorkingChange = true;
            }
            return stakeholder;
        };

        var resetTrackers = function () {
            $scope.isWorkingChange = false;
            $scope.isPaymentChange = false;
            $scope.isAddressChange = false;
        };

        $scope.showReposrtiesList = false;
        //$scope.ReportiesList = [];
        $scope.ReportiesData = {};
        $scope.TotalPayment = 0;

        $scope.closeReportiesModel = function () {
            $scope.ReportiesData = {};
            $scope.showReposrtiesList = false;
            $scope.TotalPayment = 0;
        };

        $scope.displayReporties = function (id, hierarchyid) {
            debugger;
            $scope.showReposrtiesList = true;
            apieditstake.customGET('GetReporteesStakeholder', { reportsToId: id, hierarchyid: hierarchyid }).then(function (data) {
                //$scope.ReportiesList = data;
                $scope.ReportiesData = data;
                angular.forEach($scope.ReportiesData.ReportsToList, function (payment) {
                    if (payment.StkhPayments.length > 0) {
                        $scope.TotalPayment = $scope.TotalPayment + Number(payment.StkhPayments[0].FixpayTotal);
                    }
                });
            });
        };

        var custlist = function (name, key) {
            this.Name = name;
            this.Key = key;
        };
    }])
);