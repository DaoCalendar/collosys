(
csapp.controller('EditHierarchy', ['$scope', '$http', 'Restangular', '$csfactory', '$csnotify', '$Validations',
    function ($scope, $http, rest, $csfactory, $csnotify, $validation) {

        //#region Init
        var apiCalls = rest.all('StakeApi');

        var displayHierarchy = function () {
            apiCalls.customGET('GetAllHierarchies').then(function (data) {
                $scope.HierarchyList = data;
                _.forEach($scope.HierarchyList, function (item) {
                    item.ReportsToName = _.find($scope.HierarchyList, { 'Id': item.ReportsTo });
                    item.WorkingReportsToName = _.find($scope.HierarchyList, {'Id':item.WorkingReportsTo});
                });
            }, function () {
                $csnotify.error('Error loading hierarchies');
            });
        };
        var getReporty = function () {
            debugger;
            apiCalls.customGET('GetReportingLevel').then(function (data) {
                debugger;
                $scope.ReportingLevelList = data;
            }, function () {
                $csnotify.error('Error loading Reporting level');
            });
        };


        var init = function () {
            $scope.val = $validation;
            $scope.isInEditMode = false;
            $scope.active = false;
            $scope.HierarchyList = [];
            $scope.ReportingLevelList = [];
            $scope.LocationlevelList = [{ key: 'Pincode', value: 'Pincode' },
                                         { key: 'Area', value: 'Area' },
                                         { key: 'City', value: 'City' },
                                         { key: 'District', value: 'District' },
                                         { key: 'Cluster(MultiDistrict)', value: 'Cluster' },
                                         { key: 'State', value: 'State' },
                                         { key: 'Region(Multistate)', value: 'Region' },
                                         { key: 'Country', value: 'Country' },
                                         { key: 'MultiCountry', value: 'MultiCountry' }];
            displayHierarchy();
            getReporty();

        };

        init();

        //#endregion

        //#region Helper for EditHierarchy 
        $scope.openEditBox = function (hierarchyRow) {
            $scope.active = true;
            $scope.isInEditMode = true;
            // $scope.hierarchy = angular.copy(hierarchyRow);
            $scope.hierarchy = hierarchyRow;
            var list = JSON.parse($scope.hierarchy.LocationLevel);
            $scope.hierarchy.LocationLevel = list;

        };

        $scope.closeEditBox = function () {
            $scope.active = false;
            $scope.step = 1;
        };

        $scope.reloadReportsTo = function () {
            $scope.stakeholder.ReportsTo = '';
            $scope.stakeholder.WorkingReportsTo = '';
        };

        $scope.ResetBtnValue = function () {
            if ($scope.isInEditMode == true) {
                return;
            }
            $scope.stakeholder.HasBuckets = false;
            $scope.stakeholder.IsInAllocation = false;

        };
        //#endregion

        //#region Reset both EditHierarchy and Hierarchy
        $scope.ResetPaymentChlidVal = function () {
            $scope.stakeholder.HasBankDetails = false;
            $scope.stakeholder.HasMobileTravel = false;
            $scope.stakeholder.HasVariable = false;
            $scope.stakeholder.HasFixed = false;
            $scope.stakeholder.HasServiceCharge = false;
        };

        //#endregion

        //#region DesignationName for Hierarchy
        $scope.DesignationName = function (hierarchy) {
            if (!$csfactory.isNullOrEmptyArray(hierarchy)) {
                if ((hierarchy.Hierarchy !== 'External') || (hierarchy.IsIndividual === false)) {
                    return hierarchy.Designation;
                } else {
                    var reportTo = _.find($scope.HierarchyList, { 'Id': hierarchy.ReportsTo });
                    return hierarchy.Designation + ' (' + reportTo.Designation + ')';
                }
            }
            return '';
        };
        //#endregion

        //#region Save Hierarchy

        $scope.saveUpdatedData = function (hierarchy) {
            $scope.closeform();
            hierarchy.ApplicationName = 'collosys';
            hierarchy.PositionLevel = 0;
            var list = JSON.stringify(hierarchy.LocationLevel);
            hierarchy.LocationLevel = list;
            apiCalls.customPOST(hierarchy, 'SaveHierarchy').then(function () {
                $scope.stakeholder = {};
                $scope.hierarchy = {};
                if ($scope.isInEditMode === true) {
                    displayHierarchy();
                    getReporty();
                }
                $scope.closeEditBox();
                $csnotify.success('Data Saved');
            }, function () {
                $csnotify.error('error in saving hierarchy');
            });

            //apiCalls.customPOST(hierarchy, 'SaveHierarchy').then(function () {
            //    $scope.hierarchy = {};
            //    $scope.closeEditBox();
            //    $csnotify.success(' Data Updated');

            //}, function () {
            //    $csnotify.error('error in saving hierarchy');
            //});
        };


        //#endregion

        // #region closeform for StakeHolderHierarchy
        $scope.closeform = function () {
            $scope.step = 1;
            $scope.stakeholder = null;
        };
        //#endregion
    }])
);