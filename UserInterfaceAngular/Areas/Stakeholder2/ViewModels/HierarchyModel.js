(
csapp.controller('StakeHierarchy', ['$scope', '$http', 'Restangular', '$csfactory', '$csnotify',
    function ($scope, $http, rest, $csfactory, $csnotify) {

        //#region init

        var apiCalls = rest.all('StakeApi');

        var getHierarchies = function () {
            apiCalls.customGET('GetAllHierarchies').then(function (data) {
                $scope.HierarchyList = data;
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
                debugger;
                $scope.$parent.WizardData.FinalPostModel.SelHierarchy.Designation = null;
                $scope.$parent.WizardData.showBasicInfo = false;
                // $scope.$parent.resetStakeholder();
                $scope.$parent.resetWizardData();
            }

        };

        $scope.getHierarchyDisplayName = function (hierarchy) {
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

        $scope.assignSelectedHier = function () {
            //debugger;
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
            $scope.$parent.WizardData.showBasicInfo = true;
            $scope.$parent.resetWizardData();

        };




        //#endregion

    }])
);