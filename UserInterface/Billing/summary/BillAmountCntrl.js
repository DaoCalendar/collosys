csapp.controller("BillAmountCntrl", ["$scope", "$csfactory", "$csnotify", "Restangular", '$Validations', function ($scope, $csfactory,
    $csnotify, rest, $validation) {
    var restApi = rest.all("BillingAmountApi");
    $scope.init = function () {
        $scope.billingData = {};
        $scope.billDetails = [];
        $scope.moreDetails = [];
        $scope.transcationTypes = [{ display: 'Incentive', value: 'true' }, { display: 'Fine', value: 'false' }];
        $scope.taxtype = ['PreTax', 'PostTax'];
        $scope.Reasonstype = [{ display: 'Performance', IsCredit: 'true' },
        { display: 'Customer Complaints', IsCredit: 'false' }];
        restApi.customGET('GetProducts').then(function (data) {
            $scope.products = data;
        });
    };
    $scope.init();
    $scope.enableLink = function () {
        return ($csfactory.isEmptyObject($scope.billingData) || $scope.billingData == 'null');
    };

    $scope.addAdhocPayout = function (adhocPayout, billingData) {
        var stakeholder = _.find($scope.stakeholderList, { 'Id': $scope.BillAmount.Stakeholder });
        adhocPayout.Stakeholder = stakeholder;
        billingData.Stakeholder = stakeholder;
        $scope.finalData = {
            billAdhoc: adhocPayout,
            billAmount: billingData
        };
        restApi.customPOST($scope.finalData, 'SaveBillAdhoc').then(function (data) {
            $scope.billingData = data.billAmount;
            $csnotify.success("BillAdhoc Saved");
        });
    };

    $scope.selectTransaction = function (transType) {
        $scope.reasonCode = _.where($scope.Reasonstype, { 'IsCredit': transType });
    };

    $scope.getStakeholders = function (product) {
        if (product != '') {
            restApi.customGET('GetStakeholders', { 'product': product }).
                then(function (data) {
                    $scope.stakeholderList = data;
                    $scope.billingData = {};
                    $scope.billDetails = [];
                    $scope.moreDetails = [];
                    $scope.BillAmount.Stakeholder = '';
                });
        }
    };

    $scope.getDetailData = function (paymentSource) {
        debugger;
        $scope.moreDetails = _.filter($scope.billDetails, function (billDetail) {
            var test = (billDetail.PaymentSource == paymentSource);
             return billDetail.PaymentSource == paymentSource;
        });

        //if (stakeholderId != '') {
        //    var yearMonth = moment(month).format('YYYYMM');
        //    restApi.customGET('GetBillingDetailData', {'product':product, 'stakeId': stakeholderId, 'month': yearMonth }).
        //        then(function (data) {
        //            $scope.billDetails = data;
        //            //_.forEach(data, function (item) {
        //            //    if (data.PaymentSource == 'Fixed') {
        //            //        return;
        //            //    }
        //            //    if (data.PaymentSource == 'Variable' && billdata == $scope.billingData.VariableAmount) {
        //            //        $scope.moreDetails.push(item);
        //            //    }
        //            //    if (data.PaymentSource == 'Adhoc' && billdata == $scope.billingData.Deductions) {
        //            //        $scope.moreDetails.push(item);
        //            //    }

        //            //});

        //            if ($scope.billDetails.length === 0) {
        //                $csnotify.success("Matching Data is not Available ");
        //            }
        //        });
        //}
    };

    $scope.approvePayBillingAmount = function (billingAmount, param) {
        var stakeholder = _.find($scope.stakeholderList, { 'Id': $scope.BillAmount.Stakeholder });
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
            $scope.billingData = data;
            convertToDate($scope.billingData);
        });
    };

    $scope.getBillingData = function (product, stakeholderId, month) {
        if (stakeholderId != '') {
            var yearMonth = moment(month).format('YYYYMM');
            restApi.customGET('GetBillingData', { 'products': product, 'stakeId': stakeholderId, 'month': yearMonth }).
                then(function (data) {
                    $scope.billingData = data;
                    convertToDate($scope.billingData);
                });

            restApi.customGET('GetBillingDetailData', { 'products': product, 'stakeId': stakeholderId, 'month': yearMonth }).
                then(function (data) {
                    $scope.billDetails = data;

                    if ($scope.billDetails.length === 0) {
                        $csnotify.success("Matching Data is not Available ");
                    }
                });
        }
    };
    var convertToDate = function (billingData) {
        billingData.EndDate = moment(billingData.EndDate).format('L');
        billingData.StartDate = moment(billingData.StartDate).format('L');
    };
}])