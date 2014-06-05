
csapp.factory("stakeHierarchyFactory", function () {

    var currHierarchy;

    var setHierarchy = function (hierarchy) {
        currHierarchy = hierarchy;
        currHierarchy.LocationLevelArray = JSON.parse(hierarchy.LocationLevel);
        currHierarchy.LocationLevel = currHierarchy.LocationLevelArray[0];
    };

    var getLocationLevel = function () {
        return currHierarchy.LocationLevel;
    };

    var getLocationLevelArray = function () {
        return currHierarchy.LocationLevelArray;
    };

    var getHierarchy = function () {
        return currHierarchy;
    };

    var getReportsToId = function () {
        return currHierarchy.ReportsTo;
    };

    return {
        SetHierarchy: setHierarchy,
        GetHierarchy: getHierarchy,
        GetLocationLevel: getLocationLevel,
        GetLocationLevelArray: getLocationLevelArray,
    };
});


csapp.controller('stakeHierarchy', ['$scope', '$http', 'Restangular', '$csfactory', '$csnotify', '$csModels', 
    function ($scope, $http, rest, $csfactory, $csnotify, $csModels) {

        //#region init

        var apiCalls = rest.all('HierarchyApi');

        var getHierarchies = function () {
            if ($scope.$parent.WizardData.IsEditMode() !== true) {
                apiCalls.customGET('GetAllHierarchies').then(function (data) {
                    $scope.HierarchyList = data;
                    $scope.hierarchyDisplayList = _.uniq(_.pluck($scope.HierarchyList, "Hierarchy"));
                }, function () {
                    $csnotify.error('Error loading hierarchies');
                });
            }
        };

        (function () {
            $scope.stakeholderModels = $csModels.getColumns("Stakeholder");
            $scope.HierarchyList = [];
            $scope.hierarchyDisplayList = [];
            $scope.Designation = [];
            getHierarchies();
        })();

        //#endregion

        //#region hierarchies

        $scope.changeInHierarchy = function (hierarchy) {
            $scope.showBasicInfo = false;
            $scope.Designation = [];
            var hierarchies = _.filter($scope.HierarchyList, function (item) {
                if (item.Hierarchy === hierarchy)
                    return item;
            });
            getHierarchyDisplayName(hierarchies);
        };

        var getHierarchyDisplayName = function (hierarchy) {
            $scope.Designation = [];
            hierarchy = _.sortBy(hierarchy, 'PositionLevel');
            if (!$csfactory.isNullOrEmptyArray(hierarchy)) {
                if ((hierarchy[0].Hierarchy !== 'External')) {
                    _.forEach(hierarchy, function (item) {
                        $scope.Designation.push(item);
                    });
                } else {
                    _.forEach(hierarchy, function (item) {
                        var reportTo = _.find($scope.HierarchyList, { 'Id': item.ReportsTo });
                        var desig = {
                            Designation: angular.copy(item.Designation) + '(' + reportTo.Designation + ')',
                            Id: item.Id
                        };
                        $scope.Designation.push(desig);
                    });
                }
            }
            return '';
        };

        $scope.assignSelectedHier = function (designation) {
            if ($csfactory.isNullOrEmptyArray(designation)) return;
            $scope.hierarchy = _.find($scope.HierarchyList, { 'Id': designation });
            $scope.$parent.currentHierarchy = $scope.hierarchy;
            $scope.$parent.showBasicInfo = true;
        };

        //#endregion

    }]);
