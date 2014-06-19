
csapp.factory('ViewStakeholderDatalayer', ["Restangular", function (rest) {

    var restApi = rest.all('ViewStakeApi');

    var getStakeholder = function () {
        return restApi.customGET('GetAllStakeHolders').then(function (stakes) {
            return stakes;
        });
    };


    return {
        GetStakeholder: getStakeholder
    };

}]);

csapp.controller('viewStake', ['$scope', '$http', '$log', '$window', '$csfactory', '$csnotify', '$csConstants', '$location', '$modal', '$routeParams', '$csShared', 'ViewStakeholderDatalayer',
    function ($scope, $http, $log, $window, $csfactory, $csnotify, $csConstants, $location, $modal, $routeParams, $csShared, datalayer) {


        //var restApi = rest.all('ViewStakeApi');

        //#region pagination 

        $scope.setPageNumber = function () {
            $scope.startCount = 0;
            $scope.pagenum = 1;
        };

        $scope.gotoFirstPage = function () {
            $scope.pagenum = 1;
            $scope.startCount = parseInt($scope.size) * ($scope.pagenum - 1);
            //get stakeholder and its reporting manager data respectively
            getDataForPagination();
        };

        $scope.gotoLastPage = function () {
            if ($scope.tolRec % $scope.size != 0)
                $scope.pagenum = parseInt($scope.tolRec / $scope.size) + 1;
            else if ($scope.tolRec % $scope.size === 0)
                $scope.pagenum = $scope.tolRec / $scope.size;
            $scope.startCount = parseInt($scope.size) * ($scope.pagenum - 1);
            //get stakeholder and its reporting manager data respectively
            getDataForPagination();
        };

        $scope.stepForward = function () {
            $scope.pagenum += 1;
            $scope.startCount = parseInt($scope.size) * ($scope.pagenum - 1);
            if ($scope.startCount < parseInt($scope.tolRec)) {
                //get stakeholder and its reporting manager data respectively
                getDataForPagination();

            } else $scope.pagenum -= 1;
        };
        $scope.stepBackward = function () {
            $scope.pagenum -= 1;
            if ($scope.pagenum != 0) {
                $scope.startCount = parseInt($scope.size) * ($scope.pagenum - 1);
                //get stakeholder and its reporting manager data respectively
                getDataForPagination();
            }
            else $scope.pagenum += 1;
        };
        var getDataForPagination = function () {
            switch ($scope.stakeholder.filter) {
                case "All":
                case "Active":
                case "Inactive":
                    if (!$csfactory.isNullOrEmptyGuid($scope.stakeholder.Designation)) {
                        restApi.customGET('GetStakeholderData', { hierarchyId: $scope.stakeholder.Designation, filterView: $scope.stakeholder.filter, start: $scope.startCount, size: $scope.size })
                            .then(function (data) {
                                $scope.stakeholderData = data.stkhholderData;
                                deleteExpiredWorkings($scope.stakeholderData);
                                var reportingMngr = data.reportingManager;
                                for (var i = 0; i < $scope.stakeholderData.length; i++) {
                                    $scope.stakeholderData[i].ReportingManagerName = reportingMngr[i].Name;
                                }
                            });
                    }
                    break;
                case "PendingForMe":
                case "PendingForAll":
                    //if ($scope.stakeholder.filter != 'product') {
                    restApi.customGET('GetPendingStkhData', { filterView: $scope.stakeholder.filter, start: $scope.startCount, size: $scope.size, currUser: $scope.currUser })
                        .then(function (data) {
                            $scope.stakeholderData = data.stkhholderData;
                            var reportingMngr = data.reportingManager;
                            for (var i = 0; i < $scope.stakeholderData.length; i++) {
                                $scope.stakeholderData[i].ReportingManagerName = reportingMngr[i].Name;
                            }
                        });
                    // }
                    break;
                case "BasedOnWorking":
                    //if ($scope.stakeholder.filter === 'product') {
                    restApi.customGET('GetStkhDataForProduct', { 'product': $scope.product, start: $scope.startCount, size: $scope.size })
                        .then(function (data) {
                            $scope.stakeholderData = data.stkhholderData;
                            deleteExpiredWorkings($scope.stakeholderData);
                            var reportingMngr = data.reportingManager;
                            for (var i = 0; i < $scope.stakeholderData.length; i++) {
                                $scope.stakeholderData[i].ReportingManagerName = reportingMngr[i].Name;
                            }
                        });
                    //}
                    break;
                case "ReportingTo":
                    restApi.customGET('GetStkhDataByStakeHolder', { 'Id': $scope.reportsToStakeholder, start: $scope.startCount, size: $scope.size })
                            .then(function (data) {
                                $scope.stakeholderData = data.stkhholderData;
                                deleteExpiredWorkings($scope.stakeholderData);
                                var reportingMngr = data.reportingManager;
                                for (var i = 0; i < $scope.stakeholderData.length; i++) {
                                    $scope.stakeholderData[i].ReportingManagerName = reportingMngr[i].Name;
                                }
                            });

            }
        };
        //#endregion


        (function () {
            datalayer.GetStakeholder().then(function (data) {
                $scope.stakeholders = data;
                console.log('stakeholders:', data);
            });
        })();

        $scope.switchPage = function (data) {
            $location.path('/stakeholder/add/' + data.Id);
        };


        $scope.assignName = function (data) {
            $scope.viewStake = {};
            $scope.viewStake.Name = data.Name;
            return data.Name;
        };

        //index in list
        var getAllData = function () {
            restApi.customGET("AllData", { currUser: $csfactory.getCurrentUserName() }).then(function (data) {
                $scope.completeData = data.completeData;
                $scope.hierarchyDesignation = data.hierarchyDesignation;
                $scope.HierarchyData = _.uniq(_.pluck(data.hierarchyDesignation, 'Hierarchy'));
                $scope.productList = data.products;
                $scope.CurrUserInfo = data.currUserData;
                showButton();
                setFilters();
            });
        };

        // harish - fix access level
        var showButton = function () {
            if ($csfactory.isEmptyObject($csShared.Permissions)) {
                return;
            }

            var stakePerm = _.find($csShared.Permissions.Childrens, function (item) {
                if (item.Activity === "Stakeholder") return item;
            });

            var stakeActivity = _.find(stakePerm.Childrens, function (item) {
                if (item.Activity === "Stakeholder") return item;
            });

            _.forEach(stakeActivity.Childrens, function (child) {
                if (child.Activity === "AddEdit") $scope.canModify = child.HasAccess;
                if (child.Activity === "Approve") $scope.canApprove = child.HasAccess;
            });

            //if ($scope.CurrUserInfo.Permission === "Approve") {
            //    $scope.canModify = true;
            //    $scope.canApprove = true;
            //}
            //if ($scope.CurrUserInfo.Permission === "View") {
            //    $scope.canModify = false;
            //    $scope.canApprove = false;
            //}
            //if ($scope.CurrUserInfo.Permission === "Modify") {
            //    $scope.canModify = true;
            //    $scope.canApprove = false;
            //}

        };

        $scope.updateScreen = function () {
            $scope.stakeholderData = [];
            $scope.moreDetails = false;
        };

        $scope.checkFilter = function (filters) {
            switch (filters) {
                case "All":
                case "Active":
                case "Inactive":
                case "ReportingTo":
                    return true;
                default:
                    return false;
            }
        };

        $scope.changeHierarchylist = function () {
            $scope.showDiv = false;
            $scope.stakeholder.Designation = "";
            $scope.product = "";
            $scope.Designation = [];

            $scope.hierarchyDesignation = _.sortBy($scope.hierarchyDesignation, 'PositionLevel');
            if (($scope.stakeholder.Hierarchy !== 'External')) {
                $scope.Designation = _.filter($scope.hierarchyDesignation, function (data) {
                    if (data.Hierarchy === $scope.stakeholder.Hierarchy) return data;
                });
            } else {


                var hierarchydesig = _.filter($scope.hierarchyDesignation, function (data) {
                    if (data.Hierarchy === $scope.stakeholder.Hierarchy) return data;
                });

                _.forEach(hierarchydesig, function (item) {
                    var reportTo = _.find($scope.hierarchyDesignation, { 'Id': item.ReportsTo });
                    var desig = {
                        Designation: item.Designation + '(' + reportTo.Designation + ')',
                        Id: item.Id
                    };
                    $scope.Designation.push(desig);
                });
            }
            return '';
        };


        $scope.setHierarchy = function (data) {
            $scope.hierarchyId = data;
        };

        $scope.getPendindData = function () {
            restApi.customGET('GetPendingStakeholder', { filterView: $scope.stakeholder.filter, start: $scope.startCount, size: $scope.size, currUser: $scope.currUser })
                .then(function (data) {
                    $log.info("stakeholders list: ", data);
                    $scope.stakeholderData = data;
                    deleteExpiredWorkings($scope.stakeholderData);
                    if ($scope.stakeholderData.length == 0) {
                        $scope.showDiv = false;
                        $csnotify.success("Stakeholder not available");
                    } else {
                        $scope.showDiv = true;
                        restApi.customGET('GetTotalCountforPending', { filterView: $scope.stakeholder.filter, currUser: $scope.currUser })
                       .then(function (count) {
                           $log.info("total number of records: ", count);
                           $scope.tolRec = count;
                       });

                        //attatches Reporting Manager
                        restApi.customPOST($scope.stakeholderData, 'GetReportingManager').then(function (reportingMngr) {
                            $log.info("reporting manager name: ", reportingMngr);
                            if (reportingMngr.length > 0) {
                                $log.info("reporting manager name: ", reportingMngr);
                                for (var i = 0; i < $scope.stakeholderData.length; i++) {
                                    $scope.stakeholderData[i].ReportingManagerName = reportingMngr[i].Name;
                                }
                            }
                        });
                    }
                });
        };

        $scope.getStakeData = function (stakeholder) {

            if ($csfactory.isNullOrEmptyString(stakeholder.filter)) return;

            $scope.chosenStakeholders = [];
            $scope.moreDetails = false;
            $log.info(stakeholder);
            switch (stakeholder.filter) {
                case "All":
                case "Active":
                case "Inactive":
                    if ($scope.stakeholder.Hierarchy != "" && $scope.stakeholder.Designation != "") {
                        restApi.customGET('GetStakeholder', { hierarchyId: $scope.stakeholder.Designation, filterView: $scope.stakeholder.filter, start: $scope.startCount, size: $scope.size })
                            .then(function (data) {

                                $log.info("stakeholders list: ", data);
                                $scope.stakeholderData = data;
                                deleteExpiredWorkings
                                ($scope.stakeholderData);

                                if ($scope.stakeholderData.length == 0) {
                                    $scope.showDiv = false;
                                    $csnotify.success("Stakeholder not available");
                                } else {
                                    $scope.showDiv = true;
                                    //gets the total count of records
                                    restApi.customGET('GetTotalCount', { hierarchyId: $scope.stakeholder.Designation, filterView: $scope.stakeholder.filter, currUser: $scope.currUser })
                                        .then(function (count) {
                                            $log.info("total number of records: ", count);
                                            $scope.tolRec = count;
                                        });

                                    //attatches Reporting Manager
                                    restApi.customPOST($scope.stakeholderData, 'GetReportingManager')
                                        .then(function (reportingMngr) {
                                            $log.info("reporting manager name: ", reportingMngr);
                                            if (reportingMngr.length > 0) {
                                                $log.info("reporting manager name: ", reportingMngr);
                                                for (var i = 0; i < $scope.stakeholderData.length; i++) {
                                                    $scope.stakeholderData[i].ReportingManagerName = reportingMngr[i].Name;
                                                }
                                            }
                                        });
                                }
                            }, function () {
                                $csnotify.error("Error in Loading Data");
                                $scope.showDiv = false;
                            });

                        return;
                    }
                    break;
                case "ReportingTo":
                    if ($scope.stakeholder.Hierarchy != "" && $scope.stakeholder.Designation != "") {
                        restApi.customGET('GetStakeholder', { hierarchyId: $scope.stakeholder.Designation, filterView: $scope.stakeholder.filter, start: $scope.startCount, size: $scope.size })
                            .then(function (data) {
                                $log.info("stakeholders list: ", data);
                                $scope.selStakeholder = data;
                            });
                    }
                    break;
                case "PendingForAll":
                case "PendingForMe":
                    $scope.getPendindData();
                    break;
            }

        };

        var deleteExpiredWorkings = function (stakeholderData) {
            var expiredWorkings = [];
            //finds all the expired workings
            _.forEach(stakeholderData, function (item) {
                _.forEach(item.StkhWorkings, function (workings) {
                    if (!$csfactory.isNullOrEmptyString(moment(workings.EndDate))
                        && (moment(workings.EndDate).format("DD-MM-YYYY") < moment().format("DD-MM-YYYY"))) {
                        expiredWorkings.push(workings);
                    }
                });
            });
            //deletes the expired workings
            for (var i = 0; i < expiredWorkings.length; i++) {
                var expiredWorking = expiredWorkings[i];
                for (var j = 0; j < stakeholderData.length; j++) {
                    var index = stakeholderData[j].StkhWorkings.indexOf(expiredWorking);
                    if (index !== -1)
                        stakeholderData[j].StkhWorkings.splice(index, 1);
                }
            }
        };

        $scope.getStakeById = function (param) {
            if (param.length < 3) {
                $scope.stakeholderData = [];
                $scope.showDiv = false;
                return;
            }
            restApi.customGET('GetStakeById', { 'param': param })
                 .then(function (data) {
                     $scope.stakeholderData = data.stake;
                     var reportingMngr = data.reportingMngr;
                     for (var i = 0; i < $scope.stakeholderData.length; i++) {
                         $scope.stakeholderData[i].ReportingManagerName = reportingMngr[i].Name;
                     }
                     $scope.stakeholderData.length > 0 ? $scope.showDiv = true : $csnotify.success('stakeholder not available');
                 }, function () {
                     $csnotify.error('eror loading data');
                 });
        };

        $scope.getReportsToStakeholders = function (stakeId) {

            if (!$csfactory.isNullOrEmptyString(stakeId)) {
                // $scope.stakeholder.filter = 'stake';
                restApi.customGET('GetReportees', { 'Id': stakeId, start: $scope.startCount, size: $scope.size })
                    .then(function (data) {
                        $log.info("stakeholders list: ", data);
                        $scope.stakeholderData = data;
                        deleteExpiredWorkings($scope.stakeholderData);
                        if ($scope.stakeholderData.length == 0) {
                            $scope.showDiv = false;
                            $csnotify.success("Stakeholder not available");
                        } else {
                            $scope.showDiv = true;
                            //attatches Reporting Manager
                            restApi.customPOST($scope.stakeholderData, 'GetReportingManager').then(function (reportingMngr) {
                                $log.info("reporting manager name: ", reportingMngr);
                                if (reportingMngr.length > 0) {
                                    $log.info("reporting manager name: ", reportingMngr);
                                    for (var i = 0; i < $scope.stakeholderData.length; i++) {
                                        $scope.stakeholderData[i].ReportingManagerName = reportingMngr[i].Name;
                                    }
                                }
                            });
                        }
                    }, function () {
                        $csnotify.error("Error in Loading Data");
                        $scope.showDiv = false;
                    });
                //gets the total count of records
                restApi.customGET('GetTotalCountForStake', { 'Id': stakeId })
                    .then(function (count) {
                        $log.info("total number of records: ", count);
                        $scope.tolRec = count;
                    });

            }

        };

        $scope.getStakeForProduct = function (product, filter) {

            if (!$csfactory.isNullOrEmptyString(product)) {
                //$scope.stakeholder.filter = filter;
                restApi.customGET('GetStakeByProduct', { 'product': product, start: $scope.startCount, size: $scope.size })
                    .then(function (data) {
                        $log.info("stakeholders list: ", data);
                        $scope.stakeholderData = data;
                        deleteExpiredWorkings($scope.stakeholderData);
                        if ($scope.stakeholderData.length == 0) {
                            $scope.showDiv = false;
                            $csnotify.success("Stakeholder not available");
                        } else {
                            $scope.showDiv = true;
                            //attatches Reporting Manager
                            restApi.customPOST($scope.stakeholderData, 'GetReportingManager').then(function (reportingMngr) {
                                $log.info("reporting manager name: ", reportingMngr);
                                if (reportingMngr.length > 0) {
                                    $log.info("reporting manager name: ", reportingMngr);
                                    for (var i = 0; i < $scope.stakeholderData.length; i++) {
                                        $scope.stakeholderData[i].ReportingManagerName = reportingMngr[i].Name;
                                    }
                                }
                            });
                        }
                    }, function () {
                        $csnotify.error("Error in Loading Data");
                        $scope.showDiv = false;
                    });
                //gets the total count of records
                restApi.customGET('GetTotalCountForProduct', { 'product': product })
                    .then(function (count) {
                        $log.info("total number of records: ", count);
                        $scope.tolRec = count;
                    });

            }
        };

        $scope.getLabelDisplay = function (data) {
            var designation = _.find($scope.hierarchyDesignation, { 'Id': data });
            if (angular.isDefined(designation))
                return designation.Designation;
        };

        $scope.getHierarchyDisplayName = function (hierarchy) {
            if (!$csfactory.isNullOrEmptyArray(hierarchy)) {
                if ((hierarchy.Hierarchy !== 'External') || (hierarchy.IsIndividual === false)) {
                    return hierarchy.Designation;
                } else {
                    var reportTo = _.find($scope.hierarchyDesignation, { 'Id': hierarchy.ReportsTo });
                    return hierarchy.Designation + ' (' + reportTo.Designation + ')';
                }
            }
            return '';
        };

        $scope.getHierarchyDisplayName2 = function (hierarchy) {

            if (!$csfactory.isNullOrEmptyArray(hierarchy)) {
                if (hierarchy.IsIndividual === false) {
                    return hierarchy.Designation;
                } else {
                    return null;
                }
            }
        };

        $scope.seeMoreDetail = function (index, msg) {
            var newmessage = index + msg;
            $scope.moreDetails = $scope.moreDetails === newmessage ? '' : newmessage;
        };

        //#region Function for selected Stakeholder    
        $scope.selectedStakeholder = function (data, selected) {
            selected = !selected; //when we click on checkbox ,on client side it is showing true but in js it is showing false
            if (selected === true) {
                if (data.Status != 'Approved') {
                    $scope.Approved = false;
                } else {
                    $scope.Approved = true;
                }
                if (data.Status != 'Rejected') {
                    $scope.Rejected = false;
                } else {
                    $scope.Rejected = true;
                }

                $scope.enableEditButtons = true;
                if (data.LeavingDate) {
                    $scope.LeavingDate = true;
                } else {
                    $scope.LeavingDate = false;
                }
                //get stakeholder for set leave 
                $scope.StakeForLeave = data;
                $log.info('$stakholder-view: Stakeholder selected from view page for set leave');
                $log.info($scope.StakeForLeave);

                $scope.chosenStakeholders.push(data);
            } else {
                var x = findIndex(data, $scope.chosenStakeholders);
                $scope.chosenStakeholders.splice(x, 1);
            }
        };

        var findIndex = function (dta, array) {

            var index = array.indexOf(dta);
            return index;

        };
        //#endregion   

        $scope.clickOnReportee = function (index) {
            $scope.ReporteeList = [];
            var data = $scope.stakeholderData[index];
            //_.forEach($scope.completeData, function (item) {
            //    if (item.ReportingManager === data.Id) {
            //        $scope.ReporteeList.push(item);
            //    }
            //});
            //if ($scope.ReporteeList.length == 0) {
            //    $csnotify.success("No Reportee");
            //}
            restApi.customGET('GetReportee', { stakeId: data.Id }).then(function (reporteeList) {
                $scope.ReporteeList = reporteeList;
            });
        };

        //push approval to higher level
        $scope.pushToHigher = function (data) {
            var higherReportingManager;
            $log.info('Push approval to higher level');
            $log.info(data);
            _.forEach(data.StkhWorkings, function (item) {
                var approvar = _.find($scope.completeData, { 'Id': item.ReportsTo });
                if (angular.isDefined(approvar) && !$csfactory.isNullOrEmptyGuid(approvar)) {
                    var higherapprover = _.find($scope.completeData, { 'Id': approvar.ReportingManager });
                    if (angular.isDefined(higherapprover) && !$csfactory.isNullOrEmptyGuid(higherapprover)) {
                        higherReportingManager = higherapprover.Id;
                        // data.ReportingManager = higherapprover.Id;
                    }
                }
            });
            var appId = _.find($scope.completeData, { 'Id': higherReportingManager });// $scope.WizardData.FinalPostModel.ReportsToList
            if (angular.isDefined(appId)) {
                data.ApprovedBy = appId.ExternalId;
            }
            restApi.customPOST(data, 'SavePushToHigher').then(function () {
                $csnotify.success("Done");
            });
            $scope.showDiv = false;
            $scope.stakeholder.Hierarchy = '';
        };

        //Amol
        $scope.approve = function (stakeholder) {
            $log.info(stakeholder);
            stakeholder.Status = 'Approved';
            restApi.customPOST(stakeholder, 'SaveApprovedAndRejectUser').then(function () {
                //removing data from the list after apporval
                $scope.stakeholderData.splice(stakeholder, 1);
                $csnotify.success('Stakeholder Approved');
                $scope.getStakeData($scope.stakeholder);//gets the list of upapproved stakeholder
            }, function (error) {
                $csnotify.error(error.data.Message);
            });
        };

        $scope.reject = function (stakeholder, description) {
            $log.info(stakeholder);
            stakeholder.Description = description;
            stakeholder.Status = 'Rejected';
            restApi.customPOST(stakeholder, 'SaveApprovedAndRejectUser').then(function () {
                //removing data from the list after reject
                $scope.chosenStakeholders.splice(stakeholder, 1);
                $scope.Description = '';
                $csnotify.success('Stakeholder Rejected');
                $scope.showDiv = false;
                $scope.stakeholder.Hierarchy = '';
                $scope.getStakeData($scope.stakeholder);//gets the list of upapproved stakeholder
            }, function (data) {
                $csnotify.error(data.data.Message);
            });
        };

        $scope.approveWorkings = function (stakeholder) {
            restApi.customPOST(stakeholder, 'SaveApprovedWorkings').then(function (data) {
                $csnotify.success('Stakeholder Workings Approved');
                $scope.showDiv = false;
                $scope.stakeholder.Hierarchy = '';
            }, function (data) {
                $csnotify.success(data.data.Message);
            });
        };

        $scope.rejectWorkings = function (stakeholder) {
            restApi.customPOST(stakeholder, 'SaveRejectedWorkings').then(function (data) {
                $csnotify.success('Stakeholder Workings Rejected');
                $scope.showDiv = false;
                $scope.stakeholder.Hierarchy = '';
            }, function (data) {
                $csnotify.success(data.data.Message);
            });
        };

        $scope.displayApproveReject = function (data) {
            if (data.Status !== 'Approved') {
                return false;
            }
            var result = false;
            _.forEach(data.StkhWorkings, function (item) {
                if (item.Status === 'Submitted') {
                    result = true;
                    return result;
                }
            });
            return result;
        };

        $scope.approveSelected = function () {

            $log.info("approveSelected");
            $log.info($scope.chosenStakeholders);

            var listOfStakeholders = setStatus($scope.chosenStakeholders, 'Approved', '');
            $log.info(listOfStakeholders);

            restApi.customPOST(listOfStakeholders, 'SaveListApprovedAndRejectUser').then(function () {
                $scope.stakeholderData = updateStakeholderList($scope.stakeholderData, listOfStakeholders);
                $log.info($scope.stakeholderData);
                //$scope.showDiv = false;
                //$scope.stakeholder.Hierarchy = '';
                $scope.getStakeData($scope.stakeholder);//gets the list of upapproved stakeholder
                $csnotify.success('Stakeholder Approved');
                $scope.chosenStakeholders = [];

            }, function (error) {
                $csnotify.error(error.data.Message);
            });

        };

        $scope.rejected = function () {
            //$scope.modelshow = true;

            $scope.modalData = {};
            $scope.modalData.chosenStakeholders = $scope.chosenStakeholders;
            $scope.modalData.stakeholder = $scope.stakeholder;

            var modalInstance = $modal.open({
                templateUrl: baseUrl + 'Stakeholder/View/approve.html',
                controller: 'rejectController',
                resolve: {
                    modalData: function () {
                        return $scope.modalData;
                    }
                }
            });

            modalInstance.result.then(function () {
                $scope.getStakeData($scope.stakeholder);
            });

        };

        //model popupoptions
        $scope.modelOption = {
            backdropFade: true,
            dialogFade: true
        };

        //#region close model function
        $scope.closeModel = function () {
            $scope.modelshow = false;
            $scope.editmodelshow = false;
        };

        $scope.closeEditModel = function () {
            $scope.editmodelshow = false;
        };

        $scope.closeManageWorking = function () {

            $scope.LeaveFrom = {};
            $scope.showManageworkingPopUp = false;
        };
        //#endregion

        $scope.editStakeholder = function (data) {
            $log.info("Edit Model");
            $location.path('/stakeholder/edit/' + data.Id);
            //var downloadpath = $csConstants.MVC_BASE_URL + "Stakeholder2/AddStakeHolder/EditStakeholder?id=" + data.Id;
            //$log.info(downloadpath);
            //$window.location = downloadpath;
        };

        $scope.rejectedSelected = function (description) {
            $log.info("rejectedSelected");
            $log.info($scope.chosenStakeholders);
            $scope.modelshow = false;

            var listOfStakeholders = setStatus($scope.chosenStakeholders, 'Rejected', description);
            $log.info(listOfStakeholders);

            restApi.customPOST(listOfStakeholders, 'SaveListApprovedAndRejectUser').then(function () {

                $scope.stakeholderData = updateStakeholderList($scope.stakeholderData, listOfStakeholders);
                $log.info($scope.stakeholderData);
                $csnotify.success('Stakeholder Rejected');
                $scope.chosenStakeholders = [];
                $scope.showDiv = false;
                $scope.stakeholder.Hierarchy = '';
                $scope.getStakeData($scope.stakeholder);//gets the list of upapproved stakeholder
            }, function (error) {
                $csnotify.error(error.data.Message);
            });
        };

        var updateStakeholderList = function (list, removeList) {
            list = list.filter(function (listItem) {
                return removeList.indexOf(listItem) == -1;
            });

            return list;
        };

        var setStatus = function (list, strStatus, description) {
            list.forEach(function (listItem) {
                listItem.Status = strStatus;
                listItem.Description = description;
            });

            return list;
        };

        $scope.manageLeave = function () {
            if ($scope.reason == 'exit') {
                $scope.showme = false;
            } else {
                $scope.showme = true;
            }
        };
        //code for set leave period for stakeholder
        $scope.setLeave = function () {
            //$scope.StakeListLeave = _.find($scope.completeData, { 'Hierarchy.Id': $scope.StakeForLeave.Hierarchy.Id });
            $scope.StakeListLeave = [];
            //_.forEach($scope.completeData, function (item) {
            //    if (item.Hierarchy.Id === $scope.StakeForLeave.Hierarchy.Id
            //    && item.Id !== $scope.StakeForLeave.Id) {
            //        $scope.StakeListLeave.push(angular.copy(item));
            //    }
            //});

            restApi.customGET('GetStakeListForManageWorking', { stakeholders: $scope.StakeForLeave.Id, hierarcyId: $scope.StakeForLeave.Hierarchy.Id }).then(function (data) {
                $scope.StakeListLeave = data;
            }, function (data) {
            });
            var reportsToStake = _.find($scope.completeData, { 'Id': $scope.StakeForLeave.ReportingManager });//getStakeholder($scope.StakeForLeave.ReportingManager);
            if (angular.isDefined(reportsToStake)) {
                $scope.ReportsToStake = reportsToStake;
            }

            $scope.showLeaveModal = true;

            $scope.modalData = {};
            $scope.modalData.StakeForLeave = $scope.StakeForLeave;
            $scope.modalData.StakeListLeave = $scope.StakeListLeave;

            var modalInstance = $modal.open({
                templateUrl: baseUrl + 'Stakeholder/view/leavemodal.html',
                controller: 'setLeaveController',
                windowClass: 'modal-large',
                resolve: {
                    modalData: function () {
                        return $scope.modalData;
                    }
                }
            });

            modalInstance.result.then(function () {
                $scope.showDiv = false;
            });


        };

        $scope.checkStakeholderForLeave = function (startDate) {

            //restApi.customGET('GetStakeListForManageWorking', { stakeholders: $scope.StakeForLeave.Id, hierarcyId: $scope.StakeForLeave.Hierarchy.Id }).then(function (data) {
            //    $scope.StakeListLeave = data;
            //    //_.forEach(list, function (item) {
            //    //    var date = moment(item.JoiningDate);
            //    //    $scope.StakeListLeave.push(angular.copy(item));
            //    //});
            //}, function (data) {
            //});
            //$scope.StakeListLeave = [];

            //_.forEach($scope.completeData, function (item) {
            //    if (item.Hierarchy.Id === $scope.StakeForLeave.Hierarchy.Id
            //        && item.Id !== $scope.StakeForLeave.Id) {
            //        var date = moment(item.JoiningDate);
            //        if ((date <= startDate)) {
            //            $scope.StakeListLeave.push(angular.copy(item));
            //        }
            //    }
            //});
        };

        //stakeholder going to leave bank
        //$scope.changeManageWorking = function (stakeholder) {
        //    var manageWorkingList = [];
        //    stakeholder.LeavingDate = $scope.LeaveFrom;
        //    _.forEach(stakeholder.StkhWorkings, function (item) {
        //        if (!$csfactory.isNullOrEmptyGuid(item.ReplaceBy)) {

        //            var item2 = angular.copy(item);
        //            item.EndDate = $scope.LeaveFrom;
        //            var newStakeholder = _.find($scope.StakeListLeave, { 'Id': item.ReplaceBy });
        //            item2.StartDate = $scope.LeaveFrom;
        //            item2.Id = $csfactory.getDefaultGuid();
        //            newStakeholder.StkhWorkings.push(item2);
        //            manageWorkingList.push(newStakeholder);
        //            manageWorkingList.push(stakeholder);

        //        }
        //    });
        //    restApi.customPOST(manageWorkingList, 'SetLeaveForStakeholder').then(function (data) {
        //        $csnotify.success('Leaving date set for stakeholder');
        //        $scope.closeManageWorking();
        //        getAllData();
        //    }, function (data) {
        //        $csnotify.error(data.data.Message);
        //    });
        //};

        //stakeholder going on leave
        $scope.changeWorking = function (stakeholder) {
            var manageWorkingModel = {
                Source: stakeholder,
                Workings: [],
                StartDate: $scope.ChangedDates.LeaveFrom,
                EndDate: $scope.ChangedDates.LeaveTo,
                Reason: $scope.reason
            };
            _.forEach(stakeholder.StkhWorkings, function (item) {
                if ($csfactory.isNullOrEmptyGuid(item.ReplaceBy)) {
                    return;
                }
                var stake = _.find($scope.StakeListLeave, { 'Id': item.ReplaceBy });
                manageWorkingModel.Workings.push({ 'Stakeholders': stake, 'Working': item });
            });
            restApi.customPOST(manageWorkingModel, 'SetLeaveForStakeholder').then(function (data) {
                $csnotify.success('Leave period set for stakeholder');
                $scope.closeSetLeave();
                $scope.showDiv = false;
                $scope.stakeholder.Hierarchy = '';

                //getAllData();
            }, function (data) {
                $csnotify.error(data.data.Message);
            });


        };

        var createDuplicateWorking = function (working) {
            var newWorking = angular.copy(working);
            newWorking.StartDate = moment($scope.ChangedDates.LeaveTo).add('days', 1);
            newWorking.Id = $csfactory.getDefaultGuid();
            newWorking.Version = 0;
            return newWorking;
        };
        var createDuplicatePayment = function (payment) {
            var newPayment = angular.copy(payment);
            newPayment.StartDate = moment($csfactory.ChangedDates.LeaveTo).add('days', 1);
            newPayment.Id = $csfactory.getDefaultGuid();
            newPayment.Version = 0;
            return newPayment;
        };
        $scope.replace = function (info, data) {
            if (info === 'ALL') return "-";
            if (info.toString() === "0") return "-";
            if (info.toString() === "7") return "6+";
            return info;
        };

        $scope.closeSetLeave = function () {

            $scope.ChangedDates = {};
            $scope.showLeaveModal = false;
        };

        //Permissions
        var init = function () {

            $scope.size = $scope.size ? $scope.size : 5;
            $scope.pagenum = 1;
            $scope.viewModels = {
                View: { label: "View", type: "enum" },
                Hierarchy: { label: "Hierarchy", type: "enum" },
                Designation: { label: "Designation", type: "select", valueField: 'Id', textField: 'Designation' },
                Stake: { label: "Stakeholder", type: "select", valueField: 'Id', textField: 'Name' },
                Products: { label: "Products", type: "enum" },
                SearchByName: { label: 'ID', placeholder: "enter ID/Name to edit", type: 'text' }
            };
            $scope.currUser = $csfactory.getCurrentUserName();

            $scope.startCount = 0;
            $scope.showDiv = false;
            $scope.showme = false;
            $scope.LeavingDate = false;
            $scope.Modify = false;
            $scope.Approve = false;
            $scope.detailFlag = true;
            $scope.stakeholderData = [];
            $scope.ReporteeList = [];
            $scope.RegistrationList = [];
            $scope.chosenStakeholders = [];
            $scope.permissionList = [];
            $scope.enableEditButtons = false;
            $scope.modelshow = false;
            $scope.editmodelshow = false;
            $scope.showLeaveModal = false;
            $scope.CurrUserInfo = {};
            $scope.Approved = true;
            $scope.Rejected = true;

            //$scope.selected = false;

            $scope.filters = ["All", "Active", "Inactive", "PendingForMe", "PendingForAll", "BasedOnWorking", "ReportingTo", "SearchById"];

            //$scope.getStakeData($scope.stakeholder);

            $scope.completeData = {
                Name: ""
            };
            $scope.stakeholder = {
                filter: ''
            };
            //load all stakeholders
            getAllData();
            //stakeholder for set leave period
            $scope.StakeForLeave = {};
            $scope.ChangedDates = {
                LeaveFrom: '',
                LeaveTo: ''
            };
            $scope.StakeListLeave = [];
            $scope.ReportsToStake = {};
        };

        var setFilters = function () {
            if (!$csfactory.isNullOrEmptyString($routeParams.data)) {
                $scope.stakeholder.filter = $routeParams.data;
                $scope.size = 5;
                $scope.getStakeData($scope.stakeholder);
            }
        };

        //init();


    }]
);


csapp.controller('rejectController', ["$scope", "Restangular", "$csnotify", "$modalInstance", "modalData", function ($scope, rest, $csnotify, $modalInstance, modalData) {

    var restApi = rest.all('ViewStakeApi');

    (function () {
        $scope.modalData = modalData;
    })();

    var setStatus = function (list, strStatus, description) {
        list.forEach(function (listItem) {
            listItem.Status = strStatus;
            listItem.Description = description;
        });

        return list;
    };

    $scope.closeModel = function () {
        $modalInstance.dismiss();
    };

    $scope.rejectedSelected = function (description) {

        var listOfStakeholders = setStatus(modalData.chosenStakeholders, 'Rejected', description);

        restApi.customPOST(listOfStakeholders, 'SaveListApprovedAndRejectUser').then(function () {
            $csnotify.success('Stakeholder Rejected');
            modalData.chosenStakeholders = [];
            modalData.stakeholder.Hierarchy = '';
            $modalInstance.close();
        }, function (error) {
            $csnotify.error(error.data.Message);
        });
    };


}]);

csapp.controller('setLeaveController', ["$scope", "Restangular", "$csnotify", "$modalInstance", "modalData", function ($scope, rest, $csnotify, $modalInstance, modalData) {

    var restApi = rest.all('ViewStakeApi');

    (function () {
        $scope.modalData = modalData;
        $scope.ChangedDates = {};
        $scope.Changed = {};
    })();


    $scope.checkStakeholderForLeave = function (startDate) {

        //restApi.customGET('GetStakeListForManageWorking', { stakeholders: $scope.StakeForLeave.Id, hierarcyId: $scope.StakeForLeave.Hierarchy.Id }).then(function (data) {
        //    $scope.StakeListLeave = data;
        //    //_.forEach(list, function (item) {
        //    //    var date = moment(item.JoiningDate);
        //    //    $scope.StakeListLeave.push(angular.copy(item));
        //    //});
        //}, function (data) {
        //});
        //$scope.StakeListLeave = [];

        //_.forEach($scope.completeData, function (item) {
        //    if (item.Hierarchy.Id === $scope.StakeForLeave.Hierarchy.Id
        //        && item.Id !== $scope.StakeForLeave.Id) {
        //        var date = moment(item.JoiningDate);
        //        if ((date <= startDate)) {
        //            $scope.StakeListLeave.push(angular.copy(item));
        //        }
        //    }
        //});
    };

    $scope.closeSetLeave = function () {
        $modalInstance.dismiss();
    };

    $scope.changeWorking = function (stakeholder) {
        var manageWorkingModel = {
            Source: stakeholder,
            Workings: [],
            StartDate: $scope.ChangedDates.LeaveFrom,
            EndDate: $scope.ChangedDates.LeaveTo,
            Reason: $scope.Changed.reason
        };
        _.forEach(stakeholder.StkhWorkings, function (item) {
            if ($csfactory.isNullOrEmptyGuid(item.ReplaceBy)) {
                return;
            }
            var stake = _.find(modalData.StakeListLeave, { 'Id': item.ReplaceBy });
            manageWorkingModel.Workings.push({ 'Stakeholders': stake, 'Working': item });
        });
        restApi.customPOST(manageWorkingModel, 'SetLeaveForStakeholder').then(function (data) {
            $csnotify.success('Leave period set for stakeholder');
            $modalInstance.close();
        }, function (data) {
            $csnotify.error(data.data.Message);
        });


    };


    $scope.manageLeave = function () {
        if ($scope.reason == 'exit') {
            $scope.showme = false;
        } else {
            $scope.showme = true;
        }
    };

}]);

//var getStakeholder = function (id) {
//    var stake;
//    restApi.customGET('GetReportsToStake', { stakeId: id }).then(function (data) {
//        stake = data;
//    }, function(data) {
//        $csnotify.error('Stakeholder not found');
//    });
//    return stake;
//};

//#region ReporteeFunctions (redundant)
//$scope.getEmail = function (data) {
//       if (!$csfactory.isNullOrEmptyArray(data)) {

//           var reporteeData = _.find($scope.completeData, { 'ReportsTo': data.Id });
//           return reporteeData.EmailId;
//       }
//       return '';

//   };
//   $scope.getDateOfJoining = function (data) {
//       if (!$csfactory.isNullOrEmptyArray(data)) {

//           var reporteeData = _.find($scope.completeData, { 'ReportsTo': data.Id });
//           return reporteeData.JoiningDate;
//       }
//       return '';

//   };
//   $scope.getPayment = function (data) {
//       if (!$csfactory.isNullOrEmptyArray(data)) {

//           var reporteeData = _.find($scope.completeData, { 'ReportsTo': data.Id });
//           return reporteeData.StkhPayments.length;
//       }
//       return '';

//   };
//   $scope.getWorking = function (data) {
//       if (!$csfactory.isNullOrEmptyArray(data)) {

//           var reporteeData = _.find($scope.completeData, { 'ReportsTo': data.Id });
//           return reporteeData.StkhWorkings.length;
//       }
//       return '';

//   };
//#endregion
//}
//        if (stakeholder.filter == 'Active') {
//            restApi.customGET('GetActiveData', { hierarchyId: $scope.stakeholder.Designation }).
//           then(function (data) {
//               $scope.stakeholderData = data;
//               if ($scope.stakeholderData.length == 0) {
//                   $scope.showDiv = false;
//                   $csnotify.success("Stakeholder not available");
//               } else {
//                   $scope.showDiv = true;
//               }
//           }, function () {
//               $csnotify.error("Error in Loading Data");
//               $scope.showDiv = false;
//           });
//            return;
//        }
//        if (stakeholder.filter == 'InActive') {
//            restApi.customGET('GetInActiveData', { hierarchyId: $scope.stakeholder.Designation }).
//           then(function (data) {
//               $scope.stakeholderData = data;
//               if ($scope.stakeholderData.length == 0) {
//                   $scope.showDiv = false;
//                   $csnotify.success("Stakeholder not available");
//               } else {
//                   $scope.showDiv = true;
//               }

//           }, function () {
//               $csnotify.error("Error in Loading Data");
//               $scope.showDiv = false;

//           });

//            return;

//        }
//    } else {
//        $scope.showDiv = false;
//    }


//    console.log($scope.stakeholderData);
//};


//if ($scope.reason == 'exit') {
//    stakeholder.LeavingDate = $scope.ChangedDates.LeaveFrom;
//    _.forEach(stakeholder.StkhWorkings, function (item) {
//        if (!$csfactory.isNullOrEmptyGuid(item.ReplaceBy)) {
//            var item2 = angular.copy(item);
//            newStakeholder = _.find($scope.StakeListLeave, { 'Id': item.ReplaceBy });
//            item.EndDate = $scope.ChangedDates.LeaveFrom;
//            item2.StartDate = $scope.ChangedDates.LeaveFrom;
//            item2.Id = $csfactory.getDefaultGuid();

//            //condition stakeholders having payment

//            if (stakeholder.Hierarchy.HasPayment) {
//                var payment = angular.copy(item.StkhPayment);
//                item.StkhPayment.EndDate = $scope.ChangedDates.LeaveFrom;
//                payment.StartDate = moment($scope.ChangedDates.LeaveFrom).add('days', 1);
//                payment.Id = $csfactory.getDefaultGuid();
//                payment.Stakeholder = newStakeholder;
//                item2.StkhPayment = payment;

//                newStakeholder.StkhPayments.push(payment);
//            }
//            newStakeholder.StkhWorkings.push(item2);

//        }
//    });
//    listofchanged.push(newStakeholder);
//    listofchanged.push(stakeholder);
//} else { // for leave
//    _.forEach(stakeholder.StkhWorkings, function (oldWorking) {

//        if (!$csfactory.isNullOrEmptyGuid(oldWorking.ReplaceBy)) {

//            var newWorking = angular.copy(oldWorking);
//            newStakeholder = _.find($scope.StakeListLeave, { 'Id': oldWorking.ReplaceBy });
//            newWorking.Id = $csfactory.getDefaultGuid();
//            newWorking.Version = 0;
//            newWorking.StartDate = $scope.ChangedDates.LeaveFrom;

//            newWorking.EndDate = $scope.ChangedDates.LeaveTo;

//            //condition stakeholders having payment

//            if (stakeholder.Hierarchy.HasPayment) {
//                var newPayment = angular.copy(oldWorking.StkhPayment);

//                oldWorking.StkhPayment.EndDate = $scope.ChangedDates.LeaveFrom;

//                newPayment.StartDate = $scope.ChangedDates.LeaveFrom;

//                newPayment.EndDate = $scope.ChangedDates.LeaveTo;

//                newPayment.Id = $csfactory.getDefaultGuid();
//                newPayment.Version = 0;

//                newWorking.StkhPayment = newPayment;
//                newStakeholder.StkhPayments.push(newPayment);
//                //if (angular.isUndefined(newPayment.StkhWorkings)) {
//                //    newPayment.StkhWorkings = [];
//                //}
//                //newPayment.StkhWorkings.push(newWorking);
//                stakeholder.StkhPayments.push(createDuplicateWorking(oldWorking.StkhPayment));
//            }
//            newStakeholder.StkhWorkings.push(newWorking);
//            //add new working
//            stakeholder.StkhWorkings.push(createDuplicateWorking(oldWorking));
//            oldWorking.EndDate = moment(newWorking.StartDate).subtract('days', 1).format("YYYY-MM-DD"); //-1
//        }

//    });
//    listofchanged.push(newStakeholder);
//    listofchanged.push(stakeholder);
//}

////save changed list into database
//restApi.customPOST(listofchanged, 'SetLeaveForStakeholder').then(function (data) {
//    $csnotify.success('Leave period set for stakeholder');
//    $scope.closeSetLeave();
//    $scope.showDiv = false;
//    $scope.stakeholder.Hierarchy = '';

//    //getAllData();
//}, function (data) {
//    $csnotify.error(data.data.Message);
//});



//switch ($scope.stakeholder.filter) {
//    case "All":
//    case "Active":
//    case "Inactive":
//    case "ReportingTo":
//        if (!$csfactory.isNullOrEmptyGuid($scope.stakeholder.Designation)) {
//            restApi.customGET('GetStakeholderData', { hierarchyId: $scope.stakeholder.Designation, filterView: $scope.stakeholder.filter, start: $scope.startCount, size: $scope.size })
//                .then(function (data) {
//                    $scope.stakeholderData = data.stkhholderData;
//                    var reportingMngr = data.reportingManager;
//                    for (var i = 0; i < $scope.stakeholderData.length; i++) {
//                        $scope.stakeholderData[i].ReportingManagerName = reportingMngr[i].Name;
//                    }
//                });
//        }
//        break;
//    case "PendingForMe":
//    case "PendingForAll":
//        //if ($scope.stakeholder.filter != 'product') {
//        restApi.customGET('GetPendingStkhData', { filterView: $scope.stakeholder.filter, start: $scope.startCount, size: $scope.size })
//            .then(function (data) {
//                $scope.stakeholderData = data.stkhholderData;
//                var reportingMngr = data.reportingManager;
//                for (var i = 0; i < $scope.stakeholderData.length; i++) {
//                    $scope.stakeholderData[i].ReportingManagerName = reportingMngr[i].Name;
//                }
//            });
//        // }
//        break;
//    case "product":
//        if ($scope.stakeholder.filter === 'product') {
//            restApi.customGET('GetStkhDataForProduct', { 'product': $scope.product, start: $scope.startCount, size: $scope.size })
//                .then(function (data) {
//                    $scope.stakeholderData = data.stkhholderData;
//                    var reportingMngr = data.reportingManager;
//                    for (var i = 0; i < $scope.stakeholderData.length; i++) {
//                        $scope.stakeholderData[i].ReportingManagerName = reportingMngr[i].Name;
//                    }
//                });
//        }
//        break;
//}
