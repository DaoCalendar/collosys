﻿csapp.factory("hierarchyDataLayer", ['Restangular', '$csnotify', function (rest, $csnotify) {

    var dldata = {};

    var apiCalls = rest.all('HierarchyApi');

    var setHierarchy = function (hierarchy, hierarchyList) {
        hierarchy.ReportsToName = _.find(hierarchyList, { 'Id': hierarchy.ReportsTo });
        hierarchy.WorkingReportsToName = _.find(hierarchyList, { 'Id': hierarchy.WorkingReportsTo });
        hierarchy.LocationLevel = JSON.parse(hierarchy.LocationLevel);
    };

    var getAllHierarchy = function () {
        return apiCalls.customGET('GetAllHierarchies').then(function (data) {
            dldata.HierarchyList = data;
            dldata.HierarchyData = _.uniq(_.pluck(data, 'Hierarchy'));
            _.forEach(dldata.HierarchyList, function (item) {
                setHierarchy(item, dldata.HierarchyList);
            });
            getReportee();
        }, function () {
            $csnotify.error('Error loading hierarchies');
        });
    };

    var getReportee = function () {
        return apiCalls.customGET('GetReportingLevel').then(function (data) {
            dldata.ReportingLevelEnum = data;
        }, function () {
            $csnotify.error('Error loading Reporting level');
        });

    };

    var saveUpdatedData = function (hierarchy) {
        hierarchy.ApplicationName = 'ColloSys';
        hierarchy.PositionLevel = 0;
        hierarchy.LocationLevel = JSON.stringify(hierarchy.LocationLevel);
        return apiCalls.customPOST(hierarchy, 'SaveHierarchy').then(function (data) {
            $csnotify.success('Data Saved');
            setHierarchy(data, dldata.HierarchyList);
            var index = _.indexOf(_.pluck(dldata.HierarchyList, 'Id'), data.Id);
            if (index !== -1) dldata.HierarchyList[index] = data;
            //return data;
        }, function () {
            $csnotify.error('error in saving hierarchy');
        });
    };

    var gethierarchy = function (detailsid) {
        return apiCalls.customGET('Get', { id: detailsid })
            .then(function (data) {
                return data;
            }, function () {
                $csnotify.error('Error loading hierarchies');
            });
    };

    return {
        dldata: dldata,
        GetAll: getAllHierarchy,
        Save: saveUpdatedData,
        Get: gethierarchy,
    };
}]);

csapp.factory("hierarchyFactory", ["$csfactory", "hierarchyDataLayer", function ($csfactory, datalayer) {



    var initLocationLevelList = function (dldata) {
        dldata.LocationlevelList = [{ key: 'Pincode', value: 'Pincode' },
                                         { key: 'Area', value: 'Area' },
                                         { key: 'City', value: 'City' },
                                         { key: 'District', value: 'District' },
                                         { key: 'Cluster(MultiDistrict)', value: 'Cluster' },
                                         { key: 'State', value: 'State' },
                                         { key: 'Region(Multistate)', value: 'Region' },
                                         { key: 'Country', value: 'Country' },
                                         { key: 'MultiCountry', value: 'MultiCountry' }];
    };

    var resetPaymentChlidVal = function (stakeholder) {
        stakeholder.HasBankDetails = false;
        stakeholder.HasMobileTravel = false;
        stakeholder.HasVariable = false;
        stakeholder.HasFixed = false;
        stakeholder.HasServiceCharge = false;
    };

    var resetBtnValue = function (stakeholder) {
        stakeholder.HasBuckets = false;
        stakeholder.IsInAllocation = false;

    };


    var reloadReportsTo = function (stakeholder, dldata) {
        dldata.Designation = [];

        dldata.HierarchyList = _.sortBy(dldata.HierarchyList, 'PositionLevel');
        if (!$csfactory.isNullOrEmptyArray(dldata.HierarchyList)) {
            if ((stakeholder.Hierarchy !== 'External')) {//|| (hierarchy.IsIndividual === false
                _.forEach(dldata.HierarchyList, function (item) {
                    dldata.Designation.push(item);
                });
                //$scope.$parent.stakeholderModels.designation.valueList = $scope.Designation;

            } else {

                dldata.Designation = _.filter(dldata.HierarchyList, function (data) {
                    return (data.Hierarchy === stakeholder.Hierarchy);
                });

                _.forEach(dldata.Designation, function (item) {
                    var reportTo = _.find(dldata.HierarchyList, { 'Id': item.ReportsTo });
                    var desig = {
                        Designation: angular.copy(item.Designation) + '(' + reportTo.Designation + ')',
                        Id: item.Id
                    };
                    dldata.Designation.push(desig);
                });
                //$scope.$parent.stakeholderModels.designation.valueList = $scope.Designation;
            }
        }



    };

    var designationName = function (hierarchy) {
        if (!$csfactory.isEmptyObject(hierarchy)) {
            if ((hierarchy.Hierarchy !== 'External') || (hierarchy.IsIndividual === false)) {
                return hierarchy.Designation;
            } else {
                var reportTo = _.find(datalayer.dldata.HierarchyList, { 'Id': hierarchy.ReportsTo });
                return hierarchy.Designation + ' (' + reportTo.Designation + ')';
            }
        }
        return '';
    };

    var setHierarchy = function (hierarchy, hierarchyList) {
        hierarchy.ReportsToName = _.find(hierarchyList, { 'Id': item.ReportsTo });
        hierarchy.WorkingReportsToName = _.find(hierarchyList, { 'Id': item.WorkingReportsTo });
        hierarchy.LocationLevel = JSON.parse(item.LocationLevel);
    };
    return {
        initLocationLevelList: initLocationLevelList,
        resetPaymentChlidVal: resetPaymentChlidVal,
        ResetBtnValue: resetBtnValue,
        reloadReportsTo: reloadReportsTo,
        DesignationName: designationName,
        setHierarchy: setHierarchy
    };
}]);

csapp.controller('hierarchyController', ['$scope', '$csfactory',
    'hierarchyDataLayer', "hierarchyFactory", "$location",
    function ($scope, $csfactory, datalayer, factory, $location) {

        (function () {
            $scope.datalayer = datalayer;
            //$scope.val = $validation;
            $scope.dldata = datalayer.dldata;
            $scope.factory = factory;
            factory.initLocationLevelList(datalayer.dldata);
            datalayer.GetAll();
        })();



        $scope.openEditModal = function (mode, hierarchy) {
            $location.path("/generic/hierarchy/editview/" + mode + "/" + hierarchy.Id);

            //$modal.open({
            //    templateUrl: baseUrl + 'Stakeholder/hierarchy/hierarchy-edit.html',
            //    controller: 'hierarchyEditController',
            //    windowClass: 'modal-large',
            //    resolve: {
            //        editHierarchy: function () {
            //            return {
            //                edithierarchy: angular.copy(hierarchy),
            //                displayMode: mode
            //            };
            //        }
            //    }
            //});
        };
    }]);

csapp.controller("hierarchyAddController", ["$csShared", "$scope", '$csfactory', 'hierarchyDataLayer', '$csModels',
    "hierarchyFactory", function ($csShared, $scope, $csfactory, datalayer, $csModels, factory) {

        (function () {
            $scope.factoryMethods = factory;
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.stakeholderfield = $csModels.getColumns("StkhHierarchy");
            factory.initLocationLevelList(datalayer.dldata);
            datalayer.GetAll();
            $scope.stakeholder = {};
        })();


        $scope.hierarchyChange = function () {
            if (angular.isDefined($scope.stakeholder) && angular.isDefined($scope.stakeholder.Hierarchy)) {
                $scope.stakeholder.ReportingLevel = '';
                $scope.stakeholder.ReportsTo = '';
                $scope.stakeholder.WorkingReportsTo = '';
                $scope.stakeholder.WorkingReportsLevel = '';
            };
        };

        $scope.reportsChange = function () {
            if (angular.isDefined($scope.stakeholder) && angular.isDefined($scope.stakeholder.ReportsTo)) {
                $scope.stakeholder.ReportingLevel = '';
                $scope.stakeholder.WorkingReportsTo = '';
                $scope.stakeholder.WorkingReportsLevel = '';
            };
        };

        $scope.reportinglevelChange = function () {
            if (angular.isDefined($scope.stakeholder) && angular.isDefined($scope.stakeholder.ReportingLevel)) {
                $scope.stakeholder.WorkingReportsTo = '';
                $scope.stakeholder.WorkingReportsLevel = '';
            };
        };

        $scope.workingReportsChange = function () {
            if (angular.isDefined($scope.stakeholder) && angular.isDefined($scope.stakeholder.WorkingReportsTo)) {
                $scope.stakeholder.WorkingReportsLevel = '';
            };
        };


        $scope.save = function (hierarchy) {
            datalayer.Save(hierarchy).then(function () {
                $scope.step = 1;
                $scope.stakeholder = {};
            });
        };

        $scope.closeform = function () {
            $scope.step = 1;
            $scope.stakeholder = null;
        };
    }]);

csapp.controller("hierarchyEditController", ["$scope", "$routeParams",
    "hierarchyDataLayer", "$csModels", "hierarchyFactory","$location",
    function ($scope, $routeParams, datalayer, $csModels, factory, $location) {

        (function () {
            $scope.factory = factory;
            datalayer.Get($routeParams.id).then(function (data) {
                $scope.hierarchy = data;
            });
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.hierarchyfield = $csModels.getColumns("StkhHierarchy");
        })();


        (function (mode) {
            switch (mode) {
                case "edit":
                    $scope.modelTitle = "Update File Details";
                    break;
                case "view":
                    $scope.modelTitle = "View File Details";
                    break;
                default:
                    throw ("Invalid display mode : " + JSON.stringify(hierarchy));
            }
            $scope.mode = mode;
        })($routeParams.mode);


        $scope.save = function (hierarchy) {
            datalayer.Save(hierarchy).then(function () {
                $location.path("/generic/hierarchy");
            });
        };

        $scope.closeModal = function () {
            $location.path("/generic/hierarchy");
        };
    }]);

