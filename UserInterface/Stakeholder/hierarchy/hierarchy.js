csapp.factory("hierarchyDataLayer", ['Restangular', '$csnotify', function (rest, $csnotify) {

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


    return {
        dldata: dldata,
        GetAll: getAllHierarchy,
        Save: saveUpdatedData
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

    var reloadReportsTo = function (stakeholder) {
        stakeholder.Designation = '';
        stakeholder.Hierarchy = '';
        stakeholder.ReportingLevel = '';
        stakeholder.ReportsTo = '';
        stakeholder.WorkingReportsTo = '';
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

csapp.controller('hierarchyController', ['$scope', '$csfactory', '$Validations', 'hierarchyDataLayer', "hierarchyFactory", "$modal",
    function ($scope, $csfactory, $validation, datalayer, factory, $modal) {

        (function () {
            $scope.datalayer = datalayer;
            $scope.val = $validation;
            $scope.dldata = datalayer.dldata;

            factory.initLocationLevelList(datalayer.dldata);
            datalayer.GetAll();
        })();

        $scope.openEditModal = function (hierarchy) {
            $modal.open({
                templateUrl: baseUrl+'Stakeholder/hierarchy/hierarchy-edit.html',
                controller: 'hierarchyEditController',
                resolve: {
                    editHierarchy: function () {
                        return hierarchy;
                    }
                }
            });
        };
    }]);

csapp.controller("hierarchyAddController", ["$scope", '$csfactory', '$Validations', 'hierarchyDataLayer',
    "hierarchyFactory", function ($scope, $csfactory, $validation, datalayer, factory) {

        (function () {
            $scope.factoryMethods = factory;
            $scope.datalayer = datalayer;
            $scope.val = $validation;
            $scope.dldata = datalayer.dldata;

            factory.initLocationLevelList(datalayer.dldata);
            datalayer.GetAll();

        })();


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

csapp.controller("hierarchyEditController", ["$scope", "editHierarchy", "$modalInstance", "hierarchyDataLayer",
    function ($scope, editHierarchy, $modalInstance, datalayer) {

        (function () {
            $scope.hierarchy = angular.copy(editHierarchy);
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
        })();

        $scope.save = function (hierarchy) {
            datalayer.Save(hierarchy).then(function () {
                $modalInstance.close();
            });
        };

        $scope.closeModal = function () {
            $modalInstance.dismiss();
        };
    }]);



//#region Save Hierarchy

//$scope.saveUpdatedData = function (hierarchy) {
//    $scope.closeform();

//    datalayer.saveUpdatedData(hierarchy);

//    //hierarchy.ApplicationName = 'collosys';
//    //hierarchy.PositionLevel = 0;
//    //var list = JSON.stringify(hierarchy.LocationLevel);
//    //hierarchy.LocationLevel = list;
//    //apiCalls.customPOST(hierarchy, 'SaveHierarchy').then(function () {
//    //    $scope.stakeholder = {};
//    //    $scope.hierarchy = {};
//    //    if ($scope.isInEditMode === true) {
//    //        displayHierarchy();
//    //        getReporty();
//    //    }
//    //    $scope.closeEditBox();
//    //    $csnotify.success('Data Saved');
//    //}, function () {
//    //    $csnotify.error('error in saving hierarchy');
//    //});


//    //apiCalls.customPOST(hierarchy, 'SaveHierarchy').then(function () {
//    //    $scope.hierarchy = {};
//    //    $scope.closeEditBox();
//    //    $csnotify.success(' Data Updated');

//    //}, function () {
//    //    $csnotify.error('error in saving hierarchy');
//    //});
//};


//#endregion