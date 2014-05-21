csapp.factory('taxmasterDataLayer', ['$csnotify', 'Restangular',
    function ($csnotify, rest) {
        var api = rest.all('TaxMasterApi');
        var dldata = {};

        var stateList = function () {
            return api.customGET('States').then(function (data) {
                return data;
            });
        };

        var taxList = function () {
            return api.customGET('GTaxList').then(function (data) {
                return data;
            });
        };

        var save = function (tax) {
            return api.post(tax).then(function (data) {
                $csnotify.success('data saved');
                return;
            });
        };

        var taxMasterList = function () {
            return api.customGET('TaxMasterList').then(function (data) {
                return data;
            });
        };
        
        var gettaxDetails = function (detailsid) {
            return api.customGET('Get', { id: detailsid })
                .then(function (data) {
                    return data;
                }, function () {
                    $csnotify.error('error in saving hierarchy');
                });
        };
        

        return {
            dldata: dldata,
            stateList: stateList,
            taxList: taxList,
            save: save,
            taxMasterList: taxMasterList,
            Get: gettaxDetails,
        };
    }
]);

csapp.controller('taxmasterCtrl', ['$scope', '$location', 'taxmasterDataLayer', '$csModels',
    function ($scope, $location, datalayer, $csModels) {
        'use strict';

        var initLocal = function () {
            $scope.taxMasterList = [];
            $scope.tax = {
                Country: 'India',
                District: 'ALL',
                IndustryZone: 'ALL'
            };
            $scope.indexOfSelected = -1;
            $scope.search = {};
        };

        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            datalayer.taxMasterList().then(function (data) {
                $scope.taxMasterList = data;
            });
            initLocal();
        })();


        $scope.showAddEditPopup = function (mode, tax) {
            if (mode === "edit" || mode === "view") {
                $location.path("/generic/taxmaster/addedit/" + mode + "/" + tax.Id);
            } else {
                $location.path("/generic/taxmaster/addedit/" + mode);
            }
        };
    }
]);

csapp.controller('taxmasterAddEditCtrl', ['$scope', 'taxmasterDataLayer', '$csModels', '$location',
    '$routeParams',
    function ($scope, datalayer, $csModels, $location, $routeParams) {
        'use strict';

        $scope.resetTax = function () {
            $scope.tax = {
                Country: 'India',
                District: 'ALL',
                IndustryZone: 'ALL'
            };
            $scope.taxForm.$setPristine();
        };

        var initLocal = function () {
            $scope.taxMasterList = [];
            $scope.tax = {
                Country: 'India',
                District: 'ALL',
                IndustryZone: 'ALL'
            };
            $scope.indexOfSelected = -1;
            $scope.search = {};
        };

        var initListFromDb = function () {
            datalayer.stateList().then(function (data) {
                $scope.TaxMaster.State.valueList = data;
            });

            datalayer.taxList().then(function (data) {
                $scope.TaxMaster.GTaxesList.valueList = data;
            });
        };

        (function () {
            if (angular.isDefined($routeParams.id)) {
                datalayer.Get($routeParams.id).then(function (data) {
                    $scope.tax = data;
                });
            } else {
                $scope.tax = {};
            }
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.tax = {};
            $scope.TaxMaster = $csModels.getColumns("TaxMaster");
            initLocal();
            initListFromDb();
        })();


        (function (mode) {
            switch (mode) {
                case "add":
                    $scope.modelTitle = "Add Tax Master";
                    break;
                case "edit":
                    $scope.modelTitle = "Update Tax Master";
                    break;
                case "view":
                    $scope.modelTitle = "View Tax Master";
                    break;
                default:
                    throw ("Invalid display mode : " + JSON.stringify(tax));
            }
            $scope.mode = mode;
        })($routeParams.mode);


        $scope.applyedit = function (t) {
            //save t first and then
            datalayer.save(t).then(function () {
                $scope.taxMasterList[$scope.indexOfSelected] = t;
                $scope.resetTax();
                $location.path('/generic/taxmaster');
            });
        };


        $scope.closeTaxlist = function () {
            $location.path('/generic/taxmaster');
        };

    }
]);


//$scope.edit = function (t, index) {
//    $scope.tax = angular.copy(t);
//    $scope.indexOfSelected = index;
//    $scope.isAddMode = false;
//};

//$scope.add = function (tax) {
//    //save tax then
//    datalayer.save(tax).then(function () {
//        $scope.taxMasterList.push(tax);
//        $scope.resetTax();
//    });
//};