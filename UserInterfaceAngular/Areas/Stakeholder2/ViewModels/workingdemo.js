csapp.controller("WorkingDemo", ['$scope', 'Restangular', '$Validations', '$log', '$timeout', '$csfactory', '$csnotify',
    function ($scope, rest, $validations, $log, $timeout, $csfactory, $csnotify) {

        var restApi = rest.all('WorkingFactoryApi');
        $scope.init = function () {
            $scope.locationLevel = ['Country', 'Region', 'State', 'Cluster', 'City', 'Area'];
            $scope.workingModel = {
                DisplayManager: {},
                SelectedPincodeData: {},
                QueryFor: ''
            };
            $scope.changed = {
                Region: false,
                State: false,
                Cluster: false,
                District:false,
                City: false,
                Area: false
            };
            $scope.displayManager = {
                showCountry: true,
                showState: false,
                showCluster: false,
                showDistrict:false,
                showCity: false,
                showArea: false
            };
        };
        $scope.init();
        var resetDisplayManager = function () {
            $scope.displayManager = {
                showCountry: true,
                showCluster: false,
                showState: false,
                showRegion: false,
                showDistrict: false,
                showCity: false,
                showArea: false
            };
        };
        var resetChanged = function () {
            $scope.changed = {
                Region: false,
                State: false,
                Cluster: false,
                City: false,
                Area: false
            };
        };
        var setDisplayManager = function (locLevel) {
            switch (locLevel) {
                case 'Country':
                    resetDisplayManager();
                    break;
                case 'Region':
                    $scope.displayManager = {
                        showCountry: true,
                        showCluster: false,
                        showState: false,
                        showRegion: true,
                        showDistrict: false,
                        showCity: false,
                        showArea: false
                    };
                    break;
                case 'State':
                    $scope.displayManager = {
                        showCountry: true,
                        showCluster: false,
                        showState: true,
                        showRegion: false,
                        showDistrict: false,
                        showCity: false,
                        showArea: false
                    };
                    break;
                case 'Cluster':
                    $scope.displayManager = {
                        showCountry: true,
                        showCluster: true,
                        showState: false,
                        showRegion: true,
                        showDistrict: false,
                        showCity: false,
                        showArea: false
                    };
                    break;
                case 'City':
                    $scope.displayManager = {
                        showCountry: true,
                        showCluster: false,
                        showState: true,
                        showRegion:false,
                        showDistrict: false,
                        showCity: true,
                        showArea: false
                    };
                    break;

            }
        };
        $scope.manageDisplay = function (locLevel) {
            debugger;
            resetDisplayManager();
            setDisplayManager(locLevel);
            $scope.workingModel.QueryFor = setQueryForInitial($scope.displayManager);
            $scope.workingModel.DisplayManager = $scope.displayManager;
            $scope.workingModel.SelectedPincodeData = $scope.selectedPincode;
            loadWorkingModel($scope.workingModel);
        };
        var setQueryForInitial = function (displayManager) {
            if (displayManager.showRegion) return 'Region';
            if (displayManager.showState) return 'State';
            if (displayManager.showCluster) return 'Cluster';
            if (displayManager.showDistrict) return 'District';
            if (displayManager.showCity) return 'City';
            if (displayManager.showArea) return 'Area';
        };
        $scope.setWorkingModel = function (loclevel) {
            switch (loclevel.toUpperCase()) {
                case 'COUNTRY':
                    return $scope.displayManager;
                case 'STATE':
                    $scope.workingModel.QueryFor = 'State';
                    loadWorkingModel($scope.workingModel);
                    return loadWorkingModel($scope.workingModel);
                case 'CLUSTER':
                    $scope.workingModel.QueryFor = 'Cluster';
                    loadWorkingModel($scope.workingModel);
                    return loadWorkingModel($scope.workingModel);
                case 'CITY':
                    $scope.workingModel.QueryFor = 'City';
                    loadWorkingModel($scope.workingModel);
                    return loadWorkingModel($scope.workingModel);
                default:
            }
        };
        $scope.setQueryFor = function () {
            debugger;
            var locLevel = 'Country';

            if ($scope.displayManager.showRegion === true && !$scope.changed.Region) {
                $scope.workingModel.QueryFor = 'Region';
                locLevel = 'Region';
            }
            if ($scope.displayManager.showState === true && !$scope.changed.State) {
                if (!$csfactory.isNullOrEmptyString($scope.check.State)) {
                    $scope.workingModel = $scope.setWorkingModel('State');
                    locLevel = 'State';
                }
            }
            if ($scope.displayManager.showCluster === true && !$scope.changed.Cluster) {
                $scope.workingModel.QueryFor = 'Cluster';
                locLevel = 'Cluster';
            }
            if ($scope.displayManager.showDistrict === true && !$scope.changed.District) {
                $scope.workingModel.QueryFor = 'District';
                locLevel = 'District';
            }
            if ($scope.displayManager.showCity === true && !$scope.changed.City) {
                $scope.workingModel = $scope.setWorkingModel('District');
                locLevel = 'District';
            }
            if ($scope.displayManager.showArea === true) {
                $scope.workingModel = $scope.setWorkingModel('Area');
            }
            $scope.setWorkingModel(locLevel);
        };
        var loadWorkingModel = function (workingModle) {
            restApi.customPOST(workingModle, 'GetPincodeData').then(function (data) {
                $scope.workingModel = data;
                resetChanged();
            });
        };
    }]);



//csapp.controller("WorkingDemo", ['$scope', 'Restangular', '$Validations', '$log', '$timeout', '$csfactory', '$csnotify',
//    function ($scope, rest, $validations, $log, $timeout, $csfactory, $csnotify) {

//        var restApi = rest.all('WorkingFactoryApi');

//        $scope.init = function () {
//            $scope.locationLevel = ['Country', 'Region', 'State', 'Cluster', 'City', 'Area'];
//            $scope.workingModel = {
//                DisplayManager: {},
//                SelectedPincodeData: {},
//                QueryFor: ''
//            };
//            $scope.check = {
//                Region: "",
//                State: "",
//                Cluster: "",
//                City: "",
//                Area: ""
//            };
//            $scope.displayManager = {
//                showCountry: true,
//                showState: false,
//                showCluster: false,
//                showCity: false,
//                showArea: false
//            };
//        };
//        $scope.init();
//        var resetDisplayManager = function () {
//            $scope.displayManager = {
//                showCountry: true,
//                showCluster: false,
//                showState: false,
//                showRegion: false,
//                showDistrict: false,
//                showCity: false,
//                showArea: false
//            };
//        };
//        var setDisplayManager = function (locLevel) {
//            switch (locLevel) {
//                case 'Country':
//                    resetDisplayManager();
//                    break;
//                case 'Region':
//                    $scope.displayManager = {
//                        showCountry: true,
//                        showCluster: false,
//                        showState: false,
//                        showRegion: true,
//                        showDistrict: false,
//                        showCity: false,
//                        showArea: false
//                    };
//                    break;
//                case 'State':
//                    $scope.displayManager = {
//                        showCountry: true,
//                        showCluster: false,
//                        showState: true,
//                        showRegion: false,
//                        showDistrict: false,
//                        showCity: false,
//                        showArea: false
//                    };
//                    break;
//                case 'Cluster':
//                    $scope.displayManager = {
//                        showCountry: true,
//                        showCluster: true,
//                        showState: true,
//                        showRegion: false,
//                        showDistrict: false,
//                        showCity: false,
//                        showArea: false
//                    };
//                    break;
//                case 'City':
//                    $scope.displayManager = {
//                        showCountry: true,
//                        showCluster: false,
//                        showState: false,
//                        showRegion: false,
//                        showDistrict: false,
//                        showCity: false,
//                        showArea: false
//                    };
//                    break;

//            }
//        };
//        $scope.manageDisplay = function (locLevel) {
//            debugger;
//            resetDisplayManager();
//            setDisplayManager(locLevel);
//            $scope.workingModel.QueryFor = setQueryForInitial($scope.displayManager);
//            $scope.workingModel.DisplayManager = $scope.displayManager;
//            $scope.workingModel.SelectedPincodeData = $scope.selectedPincode;
//            loadWorkingModel($scope.workingModel);
//        };
//        var setQueryForInitial = function (displayManager) {
//            if (displayManager.showRegion) return 'Region';
//            if (displayManager.showState) return 'State';
//            if (displayManager.showCluster) return 'Cluster';
//            if (displayManager.showCity) return 'City';
//            if (displayManager.showArea) return 'Area';
//        };
//        $scope.setWorkingModel = function (loclevel) {
//            switch (loclevel.toUpperCase()) {
//                case 'COUNTRY':
//                    return $scope.displayManager;
//                case 'STATE':
//                    $scope.workingModel.QueryFor = 'State';
//                    loadWorkingModel($scope.workingModel);
//                    return loadWorkingModel($scope.workingModel);
//                case 'CLUSTER':
//                    $scope.workingModel.QueryFor = 'Cluster';
//                    loadWorkingModel($scope.workingModel);
//                    return loadWorkingModel($scope.workingModel);
//                case 'CITY':
//                    $scope.workingModel.QueryFor = 'City';
//                    loadWorkingModel($scope.workingModel);
//                    return loadWorkingModel($scope.workingModel);
//                default:
//            }
//        };

//        $scope.setQueryFor = function () {
//            debugger;
//            var locLevel = 'Country';
//            if ($scope.displayManager.showRegion === true && $scope.workingModel.ListOfRegions.length === 0) {
//                $scope.workingModel.QueryFor = 'Region';
//                $scope.check.Region = angular.copy(workingModel.SelectedPincodeData.Region);
//                locLevel = 'Region';
//            }

//            if (($scope.displayManager.showState === true && $scope.workingModel.ListOfStates.length === 0) ||
//                 ($scope.displayManager.showState === true && $scope.check.State !== $scope.workingModel.SelectedPincodeData.State)) {
//                $scope.workingModel = $scope.setWorkingModel('State');
//                $scope.check.State = angular.copy($scope.workingModel.SelectedPincodeData.State);
//                locLevel = 'State';
//            }
//            if (($scope.displayManager.showCluster === true && $scope.workingModel.ListOfClusters.length === 0) ||
//                 ($scope.displayManager.showCluster === true && $scope.check.Cluster !== $scope.workingModel.SelectedPincodeData.Cluster)) {
//                $scope.workingModel.QueryFor = 'Cluster';
//                $scope.check.Cluster = angular.copy($scope.workingModel.SelectedPincodeData.Cluster);
//                locLevel = 'Cluster';
//            }
//            if ($scope.displayManager.showCity === true && $scope.workingModel.ListOfCities.length === 0) {
//                $scope.workingModel = $scope.setWorkingModel('City');
//                $scope.check.City = angular.copy(workingModel.SelectedPincodeData.City);
//                locLevel = 'City';
//            }
//            if ($scope.displayManager.showArea === true) {
//                $scope.workingModel = $scope.setWorkingModel('Area');
//            }
//            $scope.setWorkingModel(locLevel);
//        };

//        var loadWorkingModel = function (workingModle) {
//            restApi.customPOST(workingModle, 'GetPincodeData').then(function (data) {
//                $scope.workingModel = data;
//            });
//        };
//    }]);



//// $scope.workingModel = $scope.setWorkingModel(locLevel);

////if ($scope.displayManager.showState === true) {
////    $scope.workingModel = $scope.setWorkingModel('State');
////}
////if ($scope.displayManager.showRegion === true) {
////    $scope.workingModel.QueryFor = 'Region';
////}
////if ($scope.displayManager.showCity === true) {
////    $scope.workingModel = $scope.setWorkingModel('City');
////}
////if ($scope.displayManager.showCluster === true) {
////    $scope.workingModel.QueryFor = 'Cluster';
////}