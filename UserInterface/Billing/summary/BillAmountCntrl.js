
csapp.controller('BillAmountCntrl', ['$scope', 'billAmountDataLayer', 'billAmountFactory', '$modal', '$csBillingModels',
    function ($scope, datalayer, factory, $modal, $csBillingModels) {
        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            datalayer.getProducts();
            factory.initEnums();
            $scope.dldata.BillAmount = {};
            $scope.dldata.billingData = {};
            $scope.adhocPayoutbill = $csBillingModels.models.Summary;
        })();

        $scope.changeCredit = function () {
            $scope.adhocPayoutbill.IsCredit.valueList = datalayer.dldata.transcationtypes;
            $scope.adhocPayoutbill.IsPretax.valueList = datalayer.dldata.taxtype;
        };

        $scope.openViewModal = function () {
            $modal.open({
                templateUrl: baseUrl + 'Billing/summary/view-billamount-modal.html',
                controller: 'billAmountViewModal',
            });
        };

        $scope.openAddModal = function () {
            $scope.changeCredit();
            $modal.open({
                templateUrl: baseUrl + 'Billing/summary/add-billamount-modal.html',
                controller: 'billAmountAddModal',
            });
        };

    }]);

csapp.factory('billAmountDataLayer', ['Restangular', '$csnotify',
    function (rest, $csnotify) {
        var restApi = rest.all("BillingAmountApi");
        var dldata = {};

        var getProducts = function () {
            restApi.customGET('GetProducts').then(function (data) {
                dldata.products = data;
            });
        };

        dldata.transcationtypes = [{ display: 'Incentive', value: 'true' }, { display: 'Fine', value: 'false' }];
        dldata.taxtype = ['PreTax', 'PostTax'];
        dldata.Reasonstype = [{ display: 'Performance', transcationtype: 'true' },
            { display: 'Customer Complaints', transcationtype: 'false' }];

        var addAdhocPayout = function (adhocPayout, billingData) {
            var stakeholder = _.find(dldata.stakeholderList, { 'Id': dldata.BillAmount.Stakeholder });
            adhocPayout.Stakeholder = stakeholder;
            billingData.Stakeholder = stakeholder;
            dldata.finalData = {
                billAdhoc: adhocPayout,
                billAmount: billingData
            };
            return restApi.customPOST(dldata.finalData, 'SaveBillAdhoc').then(function (data) {
                dldata.billingData = data.billAmount;
                $csnotify.success("BillAdhoc Saved");
                return data;
            });
        };

        var getStakeholders = function (product) {
            if (product != '') {
                restApi.customGET('GetStakeholders', { 'product': product }).
                    then(function (data) {
                        dldata.stakeholderList = data;
                        dldata.billingData = {};
                        dldata.billDetails = [];
                        dldata.moreDetails = [];
                        dldata.BillAmount.Stakeholder = '';
                    });
            }
        };

        var approvePayBillingAmount = function (billingAmount, param) {
            var stakeholder = _.find(dldata.stakeholderList, { 'Id': dldata.BillAmount.Stakeholder });
            billingAmount.Stakeholder = stakeholder;

            switch (param) {
                case 'Pay':
                    billingAmount.PayStatus = 'Paid';
                    $csnotify.success("Amount Paid");
                    break;
                case 'Approve':
                    billingAmount.Status = 'Approved';
                    $csnotify.success("Amount Approved");
                    break;
            }
            restApi.customPOST(billingAmount, 'ApproveBillingAmount').then(function (data) {
                dldata.billingData = data;
                convertToDate($scope.billingData);
            });
        };

        var getBillingData = function (product, stakeholderId, month) {
            if (stakeholderId != '') {
                var yearMonth = moment(month).format('YYYYMM');
                restApi.customGET('GetBillingData', { 'products': product, 'stakeId': stakeholderId, 'month': yearMonth }).
                      then(function (data) {
                          dldata.billingData = data;
                          convertToDate(dldata.billingData);

                      });

                restApi.customGET('GetBillingDetailData', { 'products': product, 'stakeId': stakeholderId, 'month': yearMonth }).
                      then(function (data) {
                          dldata.billDetails = data;

                          if (dldata.billDetails.length === 0) {
                              $csnotify.success("Matching Data is not Available");
                          }
                      });
            }
        };

        var convertToDate = function (billingData) {
            billingData.EndDate = moment(billingData.EndDate).format('L');
            billingData.StartDate = moment(billingData.StartDate).format('L');
        };

        return {
            dldata: dldata,
            getProducts: getProducts,
            addAdhocPayout: addAdhocPayout,
            getStakeholders: getStakeholders,
            approvePayBillingAmount: approvePayBillingAmount,
            getBillingData: getBillingData
        };

    }]);

csapp.factory('billAmountFactory', ['billAmountDataLayer', function (datalayer) {
    var dldata = datalayer.dldata;

    var initEnums = function () {};

    var enableLink = function () {
        return false;
        //return ($csfactory.isEmptyObject(dldata.billingData) || dldata.billingData == 'null');
    };

    var selectTransaction = function (st) {
        var selecttransdata = _.where(dldata.Reasonstype, { 'transcationtype': st });
        return selecttransdata;
    };

    var getDetailData = function (paymentSource) {
        dldata.moreDetails = _.filter(dldata.billDetails, function (billDetail) {
            return billDetail.PaymentSource == paymentSource;
        });
    };

    return {
        initEnums: initEnums,
        enableLink: enableLink,
        selectTransaction: selectTransaction,
        getDetailData: getDetailData
    };
}]);

csapp.controller('billAmountViewModal', ['$scope', 'billAmountDataLayer', 'billAmountFactory', '$modalInstance',
    function ($scope, datalayer, factory, $modalInstance) {
        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
        })();

        $scope.closeModal = function () {
            $modalInstance.dismiss();
        };
    }]);

csapp.controller('billAmountAddModal', ['$scope', 'billAmountDataLayer', 'billAmountFactory',
    '$modalInstance','$csBillingModels',
    function ($scope, datalayer, factory, $modalInstance, $csBillingModels) {
        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            $scope.adhocPayoutbill = $csBillingModels.models.Summary;
        })();

        $scope.changeCredit = function (credit) {
            $scope.selecttransdata = factory.selectTransaction(credit);
            $scope.adhocPayoutbill.ReasonCode.valueList = $scope.selecttransdata;
        };

        $scope.closeModal = function () {
            $modalInstance.dismiss();
        };

        $scope.addAdhocPayout = function (adhocPayout, billingData) {
            datalayer.addAdhocPayout(adhocPayout, billingData).then(function (data) {
                $modalInstance.close(data);
            });
        };
    }]);