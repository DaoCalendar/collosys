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


csapp.controller('adhocPayoutCtrl', ['$scope', 'adhocPayoutDataLayer', 'adhocPayoutFactory', '$modal','$csBillingModels',
    function ($scope, datalayer, factory, $modal, $csBillingModels) {
        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            datalayer.getProducts();
            $scope.dldata.selectedProduct = '';
            $scope.dldata.adhocPayoutList = [];
            $scope.adhocPayout = {};
            $scope.adhocPayoutbill = $csBillingModels.models.AdhocPayout;
        })();

        $scope.ShowIndividual = function (stkh) {
            if (angular.isUndefined(stkh.Hierarchy)) return false;
            return stkh.Hierarchy.IsIndividual === true;
        };

        $scope.changeCredit = function () {
            $scope.adhocPayoutbill.IsCredit.valueList = datalayer.dldata.transcationtypes;
            $scope.adhocPayoutbill.IsPretax.valueList = datalayer.dldata.taxtype;
        };

        $scope.openmodal = function () {
            $scope.dldata.adhocPayout.Products = $scope.dldata.selectedProduct;
            if ($scope.dldata.selectedStkholderId) {
                $scope.dldata.adhocPayout.Stakeholder = _.find($scope.dldata.stakeholderList, { Id: $scope.dldata.selectedStkholderId });
            }
            $scope.changeCredit();
            $modal.open({
                templateUrl: baseUrl + 'Billing/adhoc/add-hoc-payment-details.html',
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
           
            if (adhocPayout.IsRecurring !== true) {
                adhocPayout.Tenure = 1;
            }
            var endDate = moment(adhocPayout.StartMonth).add('month', (adhocPayout.Tenure - 1));
            adhocPayout.StartMonth = moment(adhocPayout.StartMonth).format('YYYYMM');
            adhocPayout.EndMonth = moment(endDate).format('YYYYMM');
            adhocPayout.RemainingAmount = adhocPayout.TotalAmount;

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
            var selecttransdata = _.where(dldata.Reasonstype, { 'transcationtype': st });
            return selecttransdata;
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

csapp.controller('adhocPaymentCtrl', ['$scope', 'adhocPayoutDataLayer', 'adhocPayoutFactory', '$modalInstance','$csBillingModels',
    function ($scope, datalayer, factory, $modalInstance, $csBillingModels) {
        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.adhocPayoutbill = $csBillingModels.models.AdhocPayout;
            $scope.factory = factory;
            $scope.adhocPayout = {};
        })();

        $scope.CloseAdhocPayoutManager = function () {
            $scope.adhocPayout = {};
            $modalInstance.dismiss(); //failure
        };

        $scope.getdetails = function (product, month) {
            datalayer.getdetails(product, month);
        };

        $scope.changeCredit = function(credit) {
            $scope.selecttransdata = factory.selectTransaction(credit);
            $scope.adhocPayoutbill.ReasonCode.valueList = $scope.selecttransdata;
        };

        $scope.saveData = function (adhocPayout) {
            adhocPayout.Products = $scope.dldata.selectedProduct;
            adhocPayout.Stakeholder = $scope.dldata.adhocPayout.Stakeholder;
            datalayer.saveData(adhocPayout).then(function (data) {
                $scope.adhocPayout.TotalAmount = '';
                $scope.adhocPayout.IsCredit = [];
                $scope.adhocPayout.ReasonCode = [];
                $scope.adhocPayout.Description = '';
                $scope.adhocPayout.IsRecurring = '';
                $scope.adhocPayout.StartMonth = '';
                $modalInstance.close(data); //success
            });
        };

    }]);