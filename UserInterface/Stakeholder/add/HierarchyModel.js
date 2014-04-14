(
csapp.controller('StakeHierarchy', ['$scope', '$http', 'Restangular', '$csfactory', '$csnotify',
    function ($scope, $http, rest, $csfactory, $csnotify) {

        //#region init

        var apiCalls = rest.all('HierarchyApi');

        var getHierarchies = function () {
            apiCalls.customGET('GetAllHierarchies').then(function (data) {
                $scope.hierarchy = false;
                $scope.HierarchyList = data;
                var hierarchyArray = [];
                var hierarchy = _.pluck($scope.HierarchyList, "Hierarchy");
                _.forEach(hierarchy, function (item) {

                    if (hierarchyArray.indexOf(item) === -1) {
                        hierarchyArray.push(item);
                    }
                });
                $scope.$parent.stakeholderModels.hierarchy.valueList = hierarchyArray;
                console.log($scope.$parent.stakeholderModels.hierarchy);
                $scope.hierarchy = true;
            }, function () {
                $csnotify.error('Error loading hierarchies');
            });
        };

        var init = function () {
            $scope.HierarchyList = [];
            $scope.Designation = [];

            $scope.$parent.WizardData.showBasicInfo = false;
            getHierarchies();
        };
        if ($scope.$parent.WizardData.prevStatus === false) {
            init();
        } else {
            getHierarchies();
        }


        //#endregion

        //#region hierarchies

        $scope.changeInHierarchy = function () {
            if ($scope.$parent.WizardData.IsEditMode() === true) {
            } else {
                $scope.$parent.WizardData.FinalPostModel.SelHierarchy.Designation = null;
                $scope.$parent.WizardData.showBasicInfo = false;
                // $scope.$parent.resetStakeholder();
                $scope.$parent.WizardData.FinalPostModel.PayWorkModel.Payment = {};//to reset payment
                $scope.$parent.resetWizardData();
            }
            $scope.Designation = [];
            var hierarchies = _.filter($scope.HierarchyList, function (item) {
                if (item.Hierarchy === $scope.$parent.WizardData.FinalPostModel.SelHierarchy.Hierarchy)
                    return item;
            });

            getHierarchyDisplayName(hierarchies);
        };

        var getHierarchyDisplayName = function (hierarchy) {
            $scope.Designation = [];
            hierarchy = _.sortBy(hierarchy, 'PositionLevel');
            if (!$csfactory.isNullOrEmptyArray(hierarchy)) {
                if ((hierarchy[0].Hierarchy !== 'External')) {//|| (hierarchy.IsIndividual === false
                    _.forEach(hierarchy, function (item) {
                        $scope.Designation.push(item);
                    });
                    $scope.$parent.stakeholderModels.designation.valueList = $scope.Designation;

                } else {
                    _.forEach(hierarchy, function (item) {
                        var reportTo = _.find($scope.HierarchyList, { 'Id': item.ReportsTo });
                        var desig = {
                            Designation: angular.copy(item.Designation) + '(' + reportTo.Designation + ')',
                            Id: item.Id
                        };
                        $scope.Designation.push(desig);
                    });
                    $scope.$parent.stakeholderModels.designation.valueList = $scope.Designation;
                }
            }
            return '';
        };

        $scope.assignSelectedHier = function () {
            $scope.$parent.WizardData.showBasicInfo = false;
            if (angular.isUndefined($scope.$parent.WizardData)) {
                return;
            }

            if ($csfactory.isNullOrEmptyString(
                $scope.$parent.WizardData.FinalPostModel.SelHierarchy.Designation)) {
                return;
            }
            var hierarchy = _.find($scope.HierarchyList, { 'Id': $scope.$parent.WizardData.FinalPostModel.SelHierarchy.Designation });

            if (angular.isUndefined(hierarchy)) {
                return;
            }
            //set selected hierarchy

            $scope.$parent.WizardData.SetHierarchy(hierarchy);
            //$scope.$parent.WizardData.Hierarchy = hierarchy;

            $scope.$parent.StepManager.PopulateSteps(hierarchy);

            var locationLevel = JSON.parse(hierarchy.LocationLevel);
            $scope.$parent.WizardData.SetLocationLevelArray(locationLevel);
            $scope.$parent.WizardData.SetLocationLevel(locationLevel[0]);

            //set reports to list for stakeholder
            $scope.$parent.getReportsTo(hierarchy);

            $scope.$parent.WizardData.FinalPostModel.PayWorkModel.Payment = {};//to reset payment
            $scope.$parent.resetWizardData();
            
            //$scope.$parent.WizardData.showBasicInfo = true;
        };

        


        //#endregion

    }])
);