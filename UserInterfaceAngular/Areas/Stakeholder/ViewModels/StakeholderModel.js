/*globals  angular, console */

(
csapp.controller('stakeholder',
    ['$scope', '$http', 'Restangular', '$csfactory', '$csConstants', '$csnotify', "$timeout",
        function ($scope, $http, rest, $csfactory, csConstants, $csnotify, $timeout) {
            'use strict';

            //#region global lists
            $scope.WorkingList = [];
            $scope.PaymentList = [];
            $scope.AddressList = [];
            $scope.Registrations = [];
            //#endregion

            //#region Data List 
            $scope.ProductList = [];
            $scope.PincodeList = [];
            $scope.BucketList = [];
            $scope.LinerPolicyList = [];
            $scope.WriteoffPolicyList = [];
            $scope.StateList = [];
            $scope.ClusterList = [];
            $scope.Cluster = '';
            //#endregion

            $scope.LocationObj = {};
            $scope.SelLocationLevel = {};
            $scope.showWorkingboxex = false;

            //validation variables
            var addressCount = false;
            var paymentCount = false;
            var workingCount = false;

            //typehead validation variables
            $scope.NoCluster = false;

            //rest api 
            var apistake = rest.all('stakeholderapi');

            //#region parent
            $scope.stakeHierarchy = [];
            $scope.ReportsToList = [];
            //$scope.ReportsToHierarchies = [];

            $scope.clearScope = function () {
                $scope.Stakeholder = {};
                $scope.Stakeholder.Gender = 0;
                $scope.WorkingList = [];
                $scope.PaymentList = [];
                $scope.AddressList = [];
                $scope.Registrations = [];
                $scope.SelLocationLevel = {};
            };

            $scope.clearOnVertical = function () {
                //$scope.clearScope();
                $scope.Stakeholder = {};
                $scope.Stakeholder.Gender = 0;
                $scope.WorkingList = [];
                $scope.PaymentList = [];
                $scope.AddressList = [];
                $scope.Registrations = [];
                $scope.SelectedHier.HierarchyId = null;
                $scope.Onereportee = null;
                $scope.Hierarchy = null;
            };

            var clearOnDesignation = function () {
                $scope.Stakeholder = {};
                $scope.Stakeholder.Gender = 0;
                $scope.WorkingList = [];
                $scope.PaymentList = [];
                $scope.AddressList = [];
                $scope.Registrations = [];
                $scope.Onereportee = null;
            };

            $scope.getHierarchyDisplayName = function (hierarchy) {
                if (!$csfactory.isNullOrEmptyArray(hierarchy)) {
                    if ((hierarchy.Hierarchy !== 'External') || (hierarchy.IsIndividual === false)) {
                        return hierarchy.Designation;
                    } else {
                        var reportTo = _.find($scope.stakeHierarchy, { 'Id': hierarchy.ReportsTo });
                        return hierarchy.Designation + ' (' + reportTo.Designation + ')';
                    }
                }
                return '';
            };

            $scope.changeHierarchy = function (selectedHier) {
                clearOnDesignation();
                if ($csfactory.isNullOrEmptyGuid(selectedHier.HierarchyId))
                    return;
                $scope.Hierarchy = _.find($scope.stakeHierarchy, { 'Id': selectedHier.HierarchyId });

                apistake.customGET('GetReportsToInHierarchy', { reportsto: $scope.Hierarchy.ReportsTo })
                    .then(function (data) { $scope.ReportsToList = data; });

                console.log($scope.Hierarchy.LocationLevel);
                if (!angular.isArray($scope.Hierarchy.LocationLevel)) {
                    $scope.Hierarchy.LocationLevel = JSON.parse($scope.Hierarchy.LocationLevel);
                }
                if ($scope.Hierarchy.LocationLevel.length == 1) {

                    $scope.SelLocationLevel = $scope.Hierarchy.LocationLevel[0];
                    $scope.showWorkingboxex = true;
                }
            };

            $scope.setShowWorkingboxes = function () {
                $scope.showWorkingboxex = true;
            };

            $scope.Pincodes = function (pincode, level) {
                if ($csfactory.isNullOrEmptyString(pincode)) {
                    return [];
                }
                return apistake.customGET('GetPincodes', { pincode: pincode, level: level }).then(function (data) {
                    return $scope.PincodeList = data;
                });
            };

            //get clusters
            var timeoutCluster;
            $scope.Clusters = function (cluster) {
                if (timeoutCluster) $timeout.cancel(timeoutCluster);

                timeoutCluster = $timeout(function () {
                    return apistake.customGET('GetClusters', { cluster: cluster })
                        .then(function (data) {
                            debugger;
                            console.log(data);
                            if ($csfactory.isNullOrEmptyString(data)) {
                                $scope.NoCluster = true;
                                return $scope.ClusterList = [];
                            }
                            $scope.NoCluster = false;
                            return $scope.ClusterList = data;
                        });
                }, 400);

                return timeoutCluster;
            };

            $scope.setLocationCluster = function (cluster) {
                var data = _.find($scope.ClusterList, { 'Cluster': cluster });
                if (!angular.isUndefined(data)) {
                    $scope.LocationObj = data;
                }
            };

            $scope.setLocationArea = function (areaname) {
                var data = _.find($scope.PincodeList, { 'Area': areaname });
                if (!angular.isUndefined(data)) {
                    $scope.LocationObj = data;
                }
            };

            $scope.setLocationCity = function (cityName) {
                var data = _.find($scope.PincodeList, { 'City': cityName });
                if (!angular.isUndefined(data)) {
                    $scope.LocationObj = data;
                }
            };

            //get hierarchy
            apistake.customGET('GetAllHierarchies').then(function (data) {
                $scope.stakeHierarchy = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });

            apistake.customGET('GetStateList').then(function (data) {
                $scope.StateList = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });

            //#endregion


            //#region getters

            //Get list of products
            apistake.customGET('GetProducts').then(function (data) {
                $scope.ProductList = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });

            //get list of variable liner policies 
            apistake.customGET('VariableLinerPolicies').then(function (data) {
                $scope.LinerPolicyList = data;
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

            //save stakeholder
            $scope.SaveData = function (value, hierarchy) {
                value.Status = 'Submitted';
                value.HierarchyId = hierarchy.Id;
                value.Hierarchy = hierarchy.Hierarchy;
                value.HierarchyObj = hierarchy;
                value.Designation = hierarchy.Designation;
                value.LocationLevel = $scope.SelLocationLevel;

                if ($scope.SelectedHier.Vertical != 'External') {
                    value.EmailId = $scope.EmailId + '@sc.com';
                }

                if ($csfactory.isNullOrEmptyGuid(value.ReportsTo)) {
                    value.ReportsTo = csConstants.GUID_EMPTY; //'00000000-0000-0000-0000-000000000000';
                }

                //add registration object to stakeholder registration list
                value = addRegistration(value);

                //asign working list to stakeholder working list
                value = assignWorkingList(value);

                //asign payment list to stakeholder payment list
                value = addPayment(value);

                //asign address list to stakeholder address list
                if (hierarchy.HasAddress) {
                    if (hierarchy.HasMultipleAddress) {
                        value = asignAddressList(value);
                    } else {
                        value = asignSingleAddress(value);
                    }
                }
                //post stakeholder to api
                apistake.customPOST(value, 'SaveStakeholder').then(function () {
                    $scope.clear();
                    loadValidationData();
                    $csnotify.success('Stakeholder saved');
                }, function (data) {
                    var message = data.data.Message;
                    $csnotify.error(message);
                });
            };

            $scope.clear = function () {
                $scope.WorkingList = [];
                $scope.Registrations = [];
                $scope.PaymentList = [];
                $scope.AddressList = [];
                $scope.Stakeholder = {};
                $scope.Stakeholder.Gender = 0;
                $scope.EmailId = '';
                addressCount = false;
                paymentCount = false;
                workingCount = false;
            };

            //check working is duplicate or not
            var checkWorking = function (working, list) {
                var result = false;
                console.log(working);
                for (var i = 0; i < list.length; i++) {
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

            //#region add and delete methods on list

            $scope.AddWorking = function (stakeholder, hierarchy) {

                stakeholder.Working.StartDate = new Date();
                stakeholder.Working.Country = 'India';
                var val;
                if (stakeholder.Working) {
                    if ($scope.SelLocationLevel === 'Region') {
                        processRegionWorking(stakeholder, hierarchy);
                        postWorkingProcess(stakeholder, hierarchy);
                    }
                    if ($scope.SelLocationLevel === 'Country') {
                        processCountryWorking(stakeholder, hierarchy);
                        postWorkingProcess(stakeholder, hierarchy);
                    }
                    if ($scope.SelLocationLevel === 'State') {
                        apistake.customGET('GetRegionOfState', { state: stakeholder.Working.State }).then(function (data) {
                            processStateWorking(stakeholder, data, hierarchy);
                            postWorkingProcess(stakeholder, hierarchy);
                            return;
                        });
                    }
                    if ($scope.SelLocationLevel === 'Cluster') {
                        val = processClusterWorking(stakeholder);
                        if (val) {
                            postWorkingProcess(stakeholder, hierarchy);
                        }
                        //apistake.customGET('GetRegionOfState', { state: stakeholder.Working.State }).then(function (data) {
                        //    processClusterWorking(stakeholder, data, hierarchy);
                        //    postWorkingProcess(stakeholder, hierarchy);
                        //    return;
                        //});
                    }
                    if ($scope.SelLocationLevel === 'City') {
                        val = processCityWorking(stakeholder, hierarchy);
                        if (val) {
                            postWorkingProcess(stakeholder, hierarchy);
                        }
                    }

                    if ($scope.SelLocationLevel === 'Area') {
                        val = processAreaWorking(stakeholder, hierarchy);
                        if (val) {
                            postWorkingProcess(stakeholder, hierarchy);
                        }
                    }
                }
            };

            var postWorkingProcess = function (stakeholder, hierarchy) {
                var obj;
                var result;
                if (hierarchy.HasBuckets) {
                    for (var i = 0; i < stakeholder.BucketStart.length; i++) {
                        stakeholder.Working.BucketStart = stakeholder.BucketStart[i];
                        obj = angular.copy(stakeholder.Working);
                        result = checkWorking(stakeholder.Working, $scope.WorkingList);
                        if (!result) {
                            $scope.WorkingList.push(obj);
                            workingCount = true;
                        }
                    }
                } else {
                    obj = angular.copy(stakeholder.Working);
                    result = checkWorking(stakeholder.Working, $scope.WorkingList);
                    if (!result) {
                        $scope.WorkingList.push(obj);
                        workingCount = true;
                    }
                }
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

            $scope.AddPayment = function (value) {
                if (value) {
                    console.log(value.Payment);
                    //value.Payment.Stakeholder = value;
                    $scope.PaymentList.push(value.Payment);
                    paymentCount = true;
                    value.Payment = null;
                }
            };

            $scope.AddAddress = function (value) {
                if (value) {
                    $scope.AddressList.push(value.Address);
                    addressCount = true;
                    value.Address = null;
                }
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

            $scope.TotalPayment = function (basic, hra, other) {
                var reportStakeholder = null;
                var serviceCharge = 1;
                if (angular.isUndefined($scope.Stakeholder)) {
                    return 0;
                }

                if (angular.isDefined($scope.Stakeholder.ReportsTo)) {
                    reportStakeholder = _.find($scope.ReportsToList, function (stake) { return stake.Id == $scope.Stakeholder.ReportsTo; });
                    if (angular.isUndefined(reportStakeholder) || $csfactory.isNullOrEmptyArray(reportStakeholder.StkhPayments))
                        serviceCharge = 0;
                    else {
                        serviceCharge = Number(parseFloat(reportStakeholder.StkhPayments[0].ServiceCharge));
                        $scope.SalDetails.servPer = serviceCharge;
                    }
                } else {
                    serviceCharge = 0;
                }

                if (angular.isUndefined(basic)) {
                    basic = 0;
                }
                if (angular.isUndefined(hra)) {
                    hra = 0;
                }
                if (angular.isUndefined(other)) {
                    other = 0;
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
                return $scope.SalDetails.total; //Number(basic) + Number(hra) + Number(other);
            };

            //delete from payment list
            $scope.DeletePayment = function (payment) {
                $scope.PaymentList.splice(payment, 1);
                if ($scope.PaymentList.length < 1) {
                    paymentCount = false;
                }
            };

            //delete from address list
            $scope.DeleteAddress = function (address) {
                $scope.AddressList.splice(address, 1);
                if ($scope.AddressList.length < 1) {
                    addressCount = false;
                }
            };

            //delete from working list
            $scope.DeleteWorking = function (working) {
                $scope.WorkingList.splice(working, 1);
                if ($scope.WorkingList.length < 1) {
                    workingCount = false;
                }
            };

            //#endregion

            //validate form
            $scope.validateForm = function (formhierarchy, hierarchy, frmDetails, frmRegist, frmAddress) {
                if (typeof hierarchy === 'undefined') {
                    return true;
                }
                if (formhierarchy !== true) {
                    return false;
                }
                var isValid = true;
                if (frmDetails !== undefined) {
                    isValid = frmDetails;
                }

                if (frmRegist !== undefined) {
                    isValid = isValid && frmRegist;
                }

                if (hierarchy !== null && hierarchy.HasAddress) {
                    if (hierarchy.HasMultipleAddress) {
                        isValid = isValid && addressCount;
                    } else if (frmAddress === undefined) {
                        isValid = false;
                    } else {
                        isValid = isValid && frmAddress;
                    }
                }

                return isValid && $scope.validateList(hierarchy);
            };

            //validate lists
            $scope.validateList = function (hierarchy) {

                var result = true;

                if (hierarchy.HasPayment) {
                    result = paymentCount;
                }
                if (hierarchy.HasWorking) {
                    result = result && workingCount;
                }
                return result;
            };

            $scope.validateDOJ = function (isIndividual, dojValue, dobValue) {
                if (isIndividual === true) {
                    return dojValue > dobValue;
                }
                return true;
            };

            //get first day of current month
            $scope.currentMonthDate = function () {
                var dt = new Date();

                // Display the month, day, and year. getMonth() returns a 0-based number.
                var month = dt.getMonth() + 1;
                var year = dt.getFullYear();
                console.log(year + "-" + month + "-" + "01");
                return year + "-" + month + "-" + "01";
            };

            //function for Address
            var asignAddressList = function (value) {
                value.GAddress = $scope.AddressList;
                return value;
            };

            var asignSingleAddress = function (value) {
                value.Address.AddressType = 'Head Office';
                value.Address.IsOfficial = true;
                $scope.AddressList.push(value.Address);
                value = asignAddressList(value);
                return value;
            };

            //function for registration
            var addRegistration = function (value) {
                if (value.Registration) {
                    // value.Registration.Stakeholder = value;
                    $scope.Registrations.push(value.Registration);
                    value.StkhRegistrations = $scope.Registrations;
                }
                return value;
            };

            //function for Payment 
            var addPayment = function (value) {
                value.StkhPayments = $scope.PaymentList;
                return value;
            };

            //function for working
            var assignWorkingList = function (stakeholder) {
                stakeholder.StkhWorkings = $scope.WorkingList;
                return stakeholder;
            };

            //#region validations for registration details

            //#region lists for fields
            var regNo = [];
            var panNo = [];
            var tanNo = [];
            var serviceNo = [];

            //#endregion

            //#region fill lists

            //Registration list
            var apiregi = rest.all('Registration');

            apiregi.customGET('ListOfRegistrationNo').then(function (data) {
                regNo = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });

            //Pan No list
            apiregi.customGET('ListOfPanNo').then(function (data) {
                panNo = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });

            //Tan No list
            apiregi.customGET('ListOfTanNo').then(function (data) {
                tanNo = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });

            //Service tax no
            apiregi.customGET('ListOfServiceNo').then(function (data) {
                serviceNo = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });

            //#endregion

            //#region methods
            $scope.checkPanNo = function (panno) {
                var index = panNo.indexOf(panno);
                if (index > -1) {
                    return false;
                } else {
                    return true;
                }
            };

            $scope.checkTanNo = function (tanno) {
                var index = tanNo.indexOf(tanno);
                if (index > -1) {
                    return false;
                } else {
                    return true;
                }
            };

            $scope.checkRegNo = function (regno) {
                var index = regNo.indexOf(regno);
                if (index > -1) {
                    return false;
                } else {
                    return true;
                }
            };

            $scope.checkServiceNo = function (serviceno) {
                var index = serviceNo.indexOf(serviceno);
                if (index > -1) {
                    return false;
                } else {
                    return true;
                }
            };
            //#endregion
            //#endregion

            //#region validation for user id unique
            var users = [];

            //User id list

            //Registration list
            apistake.customGET('ListOfUserID').then(function (data) {
                users = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });

            $scope.checkUserID = function (user) {
                var index = users.indexOf(user);
                if (index > -1) {
                    return false;
                } else {
                    return true;
                }
            };
            //#endregion 

            //check date of birth 
            $scope.checkDOB = function (dateOfBirth) {
                var today = new Date();
                var birthDate = new Date(dateOfBirth);
                var age = today.getFullYear() - birthDate.getFullYear();
                var m = today.getMonth() - birthDate.getMonth();
                if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
                    age--;
                }
                return age > 18;
            };

            //call to load new data if stakeholder saved
            var loadValidationData = function () {

                apiregi.customGET('ListOfRegistrationNo').then(function (data) {
                    regNo = data;
                }, function (data) {
                    $csnotify.error(data.data.Message);
                });

                //Pan No list
                apiregi.customGET('ListOfPanNo').then(function (data) {
                    panNo = data;
                }, function (data) {
                    $csnotify.error(data.data.Message);
                });

                //Tan No list
                apiregi.customGET('ListOfTanNo').then(function (data) {
                    tanNo = data;
                }, function (data) {
                    $csnotify.error(data.data.Message);
                });

                //Service tax no
                apiregi.customGET('ListOfServiceNo').then(function (data) {
                    serviceNo = data;
                }, function (data) {
                    $csnotify.error(data.data.Message);
                });

                apistake.customGET('ListOfUserID').then(function (data) {
                    users = data;
                }, function (data) {
                    $csnotify.error(data.data.Message);
                });
            };

        }])
);


