csapp.controller('StakeholderHierarchy', ['$scope', '$http', 'Restangular', '$csfactory', '$csnotify', '$Validations',
        function ($scope, $http, rest, $csfactory, $csnotify, $validation) {

            //#region  init
            var apiCalls = rest.all('Stake1');

            var displayHierarchy = function () {
                apiCalls.customGET('GetAllHierarchies')
                    .then(function (data) {
                        $scope.HierarchyList = data;
                    }, function () {
                        $csnotify.error('Error loading hierarchies');
                    });
            };

            var init = function () {
                $scope.val = $validation;
                $scope.HierarchyList = [];
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
            };
            init();

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

            $scope.ResetBtnValue = function () {
                $scope.stakeholder.HasBuckets = false;
                $scope.stakeholder.IsInAllocation = false;
            };

            $scope.ResetPaymentChlidVal = function () {
                $scope.stakeholder.HasBankDetails = false;
                $scope.stakeholder.HasMobileTravel = false;
                $scope.stakeholder.HasVariable = false;
                $scope.stakeholder.HasFixed = false;
                $scope.stakeholder.HasServiceCharge = false;
            };

            //#endregion

            $scope.saveData = function (stakeholder) {

                $scope.closeform();
                stakeholder.ApplicationName = 'collosys';
                stakeholder.PositionLevel = 0;
                stakeholder.LocationLevel = JSON.stringify(stakeholder.LocationLevel);
                apiCalls.customPOST(stakeholder, 'SaveHierarchy').then(function () {
                    $scope.stakeholder = {};
                    $csnotify.success('Data Saved');
                }, function () {
                    $csnotify.error('error in saving hierarchy');
                });
            };

            $scope.closeform = function () {
                $scope.step = 1;
                $scope.stakeholder = null;
            };
        }]);