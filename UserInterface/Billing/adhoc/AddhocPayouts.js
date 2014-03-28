//csapp.controller("adhocPayoutCtrl1", ["$scope", "$csnotify", "Restangular", '$Validations', function ($scope,
//    $csnotify, rest, $validation) {
//    //#region "init"
//    $scope.init = function () {
//        $scope.val = $validation;
//        $scope.productsList = [];
//        $scope.stakeholderList = [];
//        $scope.adhocPayout = {};
//        $scope.adhocPayoutList = [];
//        $scope.adhocPayoutAllList = [];
//        $scope.adhocPayout.Tenure = 1;
//        $scope.transcationtypes = [{ display: 'Incentive', value: 'true' }, { display: 'Fine', value: 'false' }];
//        $scope.taxtype = ['PreTax', 'PostTax'];
//        $scope.Reasonstype = [{ display: 'Performance', transcationtype: 'true' },
//            { display: 'Customer Complaints', transcationtype: 'false' }];
//    };
//    $scope.init();
//    //#endregion


//    $scope.OpenAdhocPayoutManager = function () {
//        $scope.OpenAdhocPayout = true;
//    };


//}]);


csapp.controller('adhocPayoutCtrl', ['$scope', 'adhocPayoutDataLayer', 'adhocPayoutFactory', '$modal',
    function ($scope, datalayer, factory, $modal) {
        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            datalayer.getProducts();
        })();

        $scope.ShowIndividual = function (stkh) {
            if (angular.isUndefined(stkh.Hierarchy)) return false;
            return stkh.Hierarchy.IsIndividual === true;
        };

        $scope.openmodal = function () {
            $scope.dldata.adhocPayout.Products = $scope.dldata.selectedProduct;
            if ($scope.dldata.selectedStkholderId) {
                $scope.dldata.adhocPayout.Stakeholder = _.find($scope.dldata.stakeholderList, { Id: $scope.dldata.selectedStkholderId });
            }
            $modal.open({
                templateUrl: '/Billing/adhoc/add-hoc-payment-details.html',
                controller: 'adhocPaymentCtrl',
            });
        };
    }]);

csapp.factory('adhocPayoutDataLayer', ['Restangular', '$csnotify',
    function (rest, $csnotify) {
        var dldata = {};
        var restApi = rest.all("AddhocPayoutsApi");

        dldata.transcationtypes = [{ display: 'Incentive', value: 'true' }, { display: 'Fine', value: 'false' }];
        dldata.taxtype = ['PreTax', 'PostTax'];
        dldata.Reasonstype = [{ display: 'Performance', transcationtype: 'true' },
            { display: 'Customer Complaints', transcationtype: 'false' }];

        var getProducts = function () {
            restApi.customGET("GetProducts").then(function (data) {
                dldata.productsList = data;
            }, function (data) {
                $csnotify.error(data);
            });
        };

        var changeProductCategory = function () {
            //restApi.customGET("GetBillStatus", { product: dldata.selectedProduct }).then(function(status) {
            //    if (status) {
            debugger;
            restApi.customGET("GetStakeHolders", { products: dldata.selectedProduct }).then(function (data) {
                dldata.stakeholderList = data;
                dldata.adhocPayout.Tenure = 1;
                console.log(dldata.stakeholderList);
            }, function (data) {
                $csnotify.error(data);
            });
            restApi.customGET("GetAdhocdata", { products: dldata.selectedProduct }).then(function (data) {
                dldata.adhocPayoutAllList = data;
                dldata.adhocPayoutList = data;
            }, function (data) {
                $csnotify.error(data);
            });
            //} else {
            //    $csnotify.success("Some message");
            //}

            //});
        };

        var saveData = function (adhocPayout) {
            debugger;
            if (dldata.adhocPayout.IsRecurring === "false") {
                dldata.adhocPayout.Tenure = 1;
            }
            var endDate = moment(dldata.adhocPayout.StartMonth).add('month', (dldata.adhocPayout.Tenure - 1));
            //$csnotify.success(endDate);
            dldata.adhocPayout.StartMonth = moment(dldata.adhocPayout.StartMonth).format('MMM-YYYY');
            dldata.adhocPayout.EndMonth = Date.parse(endDate);
            dldata.adhocPayout.RemainingAmount = dldata.adhocPayout.TotalAmount;

            restApi.customPOST(adhocPayout, 'Post').then(function (data) {
                dldata.adhocPayoutList.push(adhocPayout);
                $csnotify.success('All File Columns Save Successfully');
                $scope.CloseAdhocPayoutManager();//check here
            }, function () {
                $csnotify.error();
            });
        };

        return {
            dldata: dldata,
            getProducts: getProducts,
            changeProductCategory: changeProductCategory,
            saveData: saveData
        };
    }]);

csapp.factory('adhocPayoutFactory', ['$csfactory', 'adhocPayoutDataLayer',
    function ($csfactory, datalayer) {
        var dldata = datalayer.dldata;
        dldata.adhocPayout = {};

        var resetadhocPayout = function (products) {
            dldata.adhocPayout = {};
            dldata.adhocPayout.Products = products;
        };

        var selectTransaction = function (st) {
            dldata.selecttransdata = _.where(dldata.Reasonstype, { 'transcationtype': st });
        };

        var showData = function (selectedStkholderId) {
            dldata.adhocPayoutList = _.filter(dldata.adhocPayoutAllList, function (adhocPayout) {
                return (adhocPayout.Stakeholder.Id == selectedStkholderId);
            });
        };

        return {
            resetadhocPayout: resetadhocPayout,
            selectTransaction: selectTransaction,
            showData: showData
        };
    }]);

csapp.controller('adhocPaymentCtrl', ['$scope', 'adhocPayoutDataLayer', 'adhocPayoutFactory', '$modalInstance',
    function ($scope, datalayer, factory, $modalInstance) {
        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
        })();

        $scope.CloseAdhocPayoutManager = function () {
            $scope.dldata.adhocPayout = {};
            $modalInstance.dismiss();
        };


        $scope.saveData = function () {
            datalayer.saveData(adhocPayout).then(function () {
                $modalInstance.dismiss();
            });
        };

    }]);