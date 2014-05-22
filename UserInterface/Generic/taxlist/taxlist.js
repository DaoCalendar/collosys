csapp.factory('taxlistDataLayer', ['$csnotify', 'Restangular',
    function ($csnotify, rest) {
        var api = rest.all('TaxListApi');
        var dldata = {};

        var initTaxList = function () {
        };

        var save = function (tax) {
            return api.post(tax).then(function (data) {
                $csnotify.success('data saved');
                return;
            });
        };
      
        var getList = function () {
            return api.getList().then(function (data) {
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
            //save: save,
            Save: save,
            getList: getList,
            Get: gettaxDetails,
        };
    }
]);

csapp.controller('taxlistCtrl', ['$scope', 'taxlistDataLayer', '$csModels', '$location',
    function ($scope, datalayer, $csModels, $location) {
        'use strict';
        
        var initLocal = function () {
            //$scope.TaxList = $csModels.getColumns("TaxList");
            $scope.taxList = [];
            $scope.tax = {};
            $scope.indexOfSelected = -1;
            $scope.search = {};
        };
        (function () {
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            initLocal();
            datalayer.getList().then(function (data) {
                $scope.taxList = data;
            });
        })();


        $scope.showAddEditPopup = function (mode, tax) {
            if (mode === "edit" || mode === "view") {
                $location.path("/generic/taxlist/addedit/" + mode + "/" + tax.Id);
            } else {
                $location.path("/generic/taxlist/addedit/" + mode);
            }
        };
    }
]);

csapp.controller('taxlistAddEditCtrl', ['$scope', 'taxlistDataLayer', '$csModels', '$location',
    '$routeParams',
    function ($scope, datalayer, $csModels, $location, $routeParams) {
        'use strict';

        (function () {
            if (angular.isDefined($routeParams.id)) {
                datalayer.Get($routeParams.id).then(function (data) {
                    $scope.tax = data;
                });
            } else {
                $scope.tax = {};
            }
            $scope.tax = {};
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.TaxList = $csModels.getColumns("TaxList");
            datalayer.getList().then(function (data) {
                $scope.taxList = data;
            });
        })();
        
        (function (mode) {
            switch (mode) {
                case "add":
                    $scope.modelTitle = "Add Tax List";
                    break;
                case "edit":
                    $scope.modelTitle = "Update Tax List";
                    break;
                case "view":
                    $scope.modelTitle = "View Tax List";
                    break;
                default:
                    throw ("Invalid display mode : " + JSON.stringify(tax));
            }
            $scope.mode = mode;
        })($routeParams.mode);
        
        $scope.resetTax = function () {
            $scope.tax = {};
            $scope.taxForm.$setPristine();
        };

        $scope.closeTaxlist = function() {
            $location.path('/generic/taxlist');
        };

        $scope.applyedit = function (t) {
            datalayer.Save(t).then(function () {
                $scope.taxList[$scope.indexOfSelected] = t;
                $scope.resetTax();
                $location.path('/generic/taxlist');
            });
        };

    }
]);




//$scope.add = function (tax) {
//    //save tax then
//    datalayer.create(tax).then(function () {
//        $scope.taxList.push(tax);
//        $scope.TaxList = {};
//        $location.path('/generic/taxlist');
//    });
//};


//$scope.edit = function (t, index) {
//    $scope.tax = angular.copy(t);
//    $scope.indexOfSelected = index;
//    $scope.isAddMode = false;
//};

//var save = function (tax) {
//    return tax.put().then(function (data) {
//        $csnotify.success('data saved');
//        return;
//    });
//};
