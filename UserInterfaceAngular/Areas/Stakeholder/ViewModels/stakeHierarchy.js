(
csapp.controller("stkHrchyCtrl",
    ["$scope", "$csfactory", "$csnotify", 'Restangular', function ($scope, $csfactory, $csnotify, rest) {
        "use strict";

        //#region controller Initialization

        $scope.hierarchys = ["Telecalling", "Field", "BackOffice"];

        $scope.locationLevels = ["Pincode", "Area", "City",
                                 "District", "Cluster(MultiDistrict)", "State",
                                 "Region(MultiState)", "Country", "MultiCountry"];

        $scope.stkhierarchys = [];

        //#endregion

        //#region Other functions

        $scope.reset = function () {
            $scope.isReadOnly = false;
            $scope.editIndex = 0;
            $scope.stkHcy = {};
        };

        //#endregion

        //#region Model Operations

        $scope.openModel = function () {
            $scope.reset();
            $scope.shouldBeOpen = true;
        };

        $scope.closeModel = function () {
            $scope.shouldBeOpen = false;
        };

        $scope.modelOption = {
            backdropFade: true,
            dialogFade: true
        };

        $scope.openModelData = function (stkHcycRow, readOnly, index) {
            $scope.isReadOnly = readOnly;
            $scope.stkHcy = angular.copy(stkHcycRow);
            $scope.shouldBeOpen = true;
            $scope.editIndex = index;
        };

        //#endregion

        //#region Db Operations
        var apihierarchy = rest.all('StakeHierarchyApi');

        apihierarchy.customGET('Get').then(function (data) {
            if (angular.isUndefined(data) || !angular.isArray(data)) {
                return;
            }
            $scope.stkhierarchys = data;

            for (var i = 0; i < $scope.stkhierarchys.length; i++) {
                populateReportsToDetails($scope.stkhierarchys[i]);
            }
        }, function (data) {
            $csnotify.error(data);
        });

        var loadHierarchies = function () {
            apihierarchy.customGET('Get').then(function (data) {
                if (angular.isUndefined(data) || !angular.isArray(data)) {
                    return;
                }
                $scope.stkhierarchys = data;

                for (var i = 0; i < $scope.stkhierarchys.length; i++) {
                    populateReportsToDetails($scope.stkhierarchys[i]);
                }
            }, function (data) {
                $csnotify.error(data);
            });
        };

        var populateReportsToDetails = function (hier) {
            var reportToId = hier.ReportsTo;
            if ($csfactory.isNullOrEmptyGuid(reportToId)) {
                //reportToId === "00000000-0000-0000-0000-000000000000"
                return;
            }
            var hie = _.find($scope.stkhierarchys, { 'Id': reportToId });
            var reportToDetails = {};
            reportToDetails.Designation = hie.Designation;
            reportToDetails.Hierarchy = hie.Hierarchy;
            hier.ReportToDetails = angular.copy(reportToDetails);
        };

        $scope.AddinLocal = function (stkH) {
            populateReportsToDetails(stkH);
            if ($scope.editIndex === 0) {
                $scope.stkhierarchys.push(stkH);
            } else {
                $scope.stkhierarchys[$scope.editIndex] = stkH;
            }
        };

        $scope.saveHierarchy = function (stkHcyc) {
            debugger;
            stkHcyc.ApplicationName = 'ColloSys';
            stkHcyc.LocationLevel = JSON.stringify(stkHcyc.LocationLevel);
            if (!stkHcyc.IsInAllocation && stkHcyc.IsInField) {
                stkHcyc.IsInField = false;
            }
            if (stkHcyc.Id) {
                apihierarchy.customPUT(stkHcyc, 'Put', { Id: stkHcyc.Id }).then(function () {
                    loadHierarchies();
                    $scope.reset();
                    $scope.closeModel();
                    $csnotify.success('Hierarchy Updated');
                }, function (data) {
                    $csnotify.error(data);
                });
            } else {
                apihierarchy.customPOST(stkHcyc, 'Post').then(function () {
                    loadHierarchies();
                    $scope.reset();
                    $scope.closeModel();
                    $csnotify.success('Hierarchy Saved');
                }, function (data) {
                    $csnotify.error(data);
                });
            }
        };

        //#endregion

    }])
);