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
            $scope.dldata.selectedProduct = '';
            $scope.dldata.adhocPayoutList = [];
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

        var getdetails = function (product, month) {
            month = moment(month).format('YYYYMM');
           return restApi.customGET("GetStatus", { product: product, startmonth: month }).then(function (data) {
                dldata.isBilled = data;
                if (data == "true") {
                    $csnotify.success("Billing Already Done");
                }
                return data;
            }, function () {
                $csnotify.error();
            });
        };

        var changeProductCategory = function () {
            //restApi.customGET("GetBillStatus", { product: dldata.selectedProduct }).then(function(status) {
            //    if (status) {
            restApi.customGET("GetStakeHolders", { products: dldata.selectedProduct }).then(function (data) {
                dldata.stakeholderList = data;
                dldata.adhocPayout.Tenure = 1;
               }, function (data) {
                $csnotify.error(data);
            });
            restApi.customGET("GetAdhocdata", { products: dldata.selectedProduct }).then(function (data) {
                //convert date
                _.forEach(data, function (item) {
                    item.StartMonth = moment(item.StartMonth, 'YYYYMM');
                    item.StartMonth = moment(item.StartMonth).format('MMM-YYYY');
                    item.EndMonth = moment(item.EndMonth, 'YYYYMM');
                    item.EndMonth = moment(item.EndMonth).format('MMM-YYYY');
                });
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
            if (dldata.adhocPayout.IsRecurring === "false") {
                dldata.adhocPayout.Tenure = 1;
            }
            var endDate = moment(dldata.adhocPayout.StartMonth).add('month', (dldata.adhocPayout.Tenure - 1));
            dldata.adhocPayout.StartMonth = moment(dldata.adhocPayout.StartMonth).format('YYYYMM');
            dldata.adhocPayout.EndMonth = moment(endDate).format('YYYYMM');
            dldata.adhocPayout.RemainingAmount = dldata.adhocPayout.TotalAmount;

            return restApi.customPOST(adhocPayout, 'Post').then(function (data) {
                data.StartMonth = moment(data.StartMonth, 'YYYYMM');
                data.StartMonth = moment(data.StartMonth).format('MMM-YYYY');
                data.EndMonth = moment(data.EndMonth, 'YYYYMM');
                data.EndMonth = moment(data.EndMonth).format('MMM-YYYY');
                dldata.adhocPayoutList.push(data);
                $csnotify.success('All File Columns Save Successfully');
                return data;
            }, function () {
                $csnotify.error();
            });
        };

        return {
            dldata: dldata,
            getProducts: getProducts,
            changeProductCategory: changeProductCategory,
            saveData: saveData,
            getdetails: getdetails
        };
    }]);

csapp.factory('adhocPayoutFactory', ['$csfactory', 'adhocPayoutDataLayer',
    function ($csfactory, datalayer) {
        var dldata = datalayer.dldata;
        dldata.adhocPayout = {};

        var resetadhocPayout = function () {
            dldata.adhocPayout = {};
            //dldata.selectedProduct = products;
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
            $modalInstance.dismiss(); //failure
        };

        $scope.getdetails = function (product, month) {
            datalayer.getdetails(product, month);
        };

        $scope.saveData = function (adhocPayout) {
            datalayer.saveData(adhocPayout).then(function (data) {
                $scope.dldata.adhocPayout.TotalAmount = '';
                $scope.dldata.adhocPayout.IsCredit = [];
                $scope.dldata.adhocPayout.ReasonCode = [];
                $scope.dldata.adhocPayout.Description = '';
                $scope.dldata.adhocPayout.IsRecurring = '';
                $scope.dldata.adhocPayout.StartMonth = '';
                $modalInstance.close(data); //success
            });
        };

    }]);