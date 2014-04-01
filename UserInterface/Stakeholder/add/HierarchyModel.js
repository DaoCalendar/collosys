(
csapp.controller('StakeHierarchy', ['$scope', '$http', 'Restangular', '$csfactory', '$csnotify',
    function ($scope, $http, rest, $csfactory, $csnotify) {

        //#region init

        var apiCalls = rest.all('HierarchyApi');

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
                $scope.$parent.WizardData.FinalPostModel.SelHierarchy.Designation = null;
                $scope.$parent.WizardData.showBasicInfo = false;
                // $scope.$parent.resetStakeholder();
                $scope.$parent.WizardData.FinalPostModel.PayWorkModel.Payment = {};//to reset payment
                $scope.$parent.resetWizardData();
            }
            $scope.Designation = [];
            var hierarchies = _.filter($scope.HierarchyList,function(item) {
                if (item.Hierarchy === $scope.$parent.WizardData.FinalPostModel.SelHierarchy.Hierarchy)
                    return item;
            });
            
            getHierarchyDisplayName(hierarchies);
        };

        var getHierarchyDisplayName = function (hierarchy) {

            hierarchy =_.sortBy(hierarchy,'PositionLevel');
            if (!$csfactory.isNullOrEmptyArray(hierarchy)) {
                if ((hierarchy[0].Hierarchy !== 'External')) {//|| (hierarchy.IsIndividual === false
                    _.forEach(hierarchy, function (item) {
                        $scope.Designation.push(item);
                    });
                    
                } else {
                    _.forEach(hierarchy, function(item) {
                        var reportTo = _.find($scope.HierarchyList, { 'Id': item.ReportsTo });
                        var desig = {
                            Designation: angular.copy(item.Designation) + '(' + reportTo.Designation + ')',
                            Id:  item.Id
                        };
                        $scope.Designation.push(desig);
                    });

                }
            }
            return '';
        };

        $scope.assignSelectedHier = function () {
            //
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
            $scope.$parent.WizardData.FinalPostModel.PayWorkModel.Payment = {};//to reset payment
            $scope.$parent.resetWizardData();
            //setBasicInfoModel(hierarchy);
        };

        var setBasicInfoModel = function (hierarchy) {
            
            $scope.$parent.stakeholderModel.Email.suffix = "";
            if (hierarchy.Hierarchy != 'External') {
                $scope.$parent.stakeholderModel.Email.suffix = "@sc.com";//set email model
                $scope.$parent.stakeholderModel.UserId.required = true;//set userId model
            }

            if (hierarchy.IsEmployee)
                $scope.$parent.stakeholderModel.Date.label = "Date of Joining";
            else $scope.$parent.stakeholderModel.Date.label = "Date of Starting";


        };


        //#endregion

    }])
);