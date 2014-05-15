csapp.factory('billStatusDataLayer', ['Restangular', '$csnotify',
    function (rest, $csnotify) {
        var dldata = {};
        var restApi = rest.all('BillPaymentStatusApi');

        var fetchData = function (product, billmonth) {
            return restApi.customGET('GetBillAmountDetails', { products: product, billmonth: billmonth }).
                then(function (data) {
                    return data;
                });
        };

        var savestatus = function(amount) {
            return restApi.customPOST(amount, 'SaveBillStatus').then(function (data) {
                $csnotify.success('data saved');
                return data;
            });
        };
        return {
            dldata: dldata,
            fetchData: fetchData,
            save: savestatus
        };
    }]);

csapp.factory('billStausFactory', function () {
    return {

    };
});

csapp.controller('billStatusController', ['$scope', 'billStatusDataLayer', 'billStausFactory', '$csModels', '$csShared',
    function ($scope, datalayer, factory, $csModels, $csShared) {
        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            $scope.BillAmount = $csModels.getColumns("BillAmount");
        })();

        $scope.fetchData = function (seleData) {
            var month = moment(seleData.Month).format('YYYYMM');

            datalayer.fetchData(seleData.Product, month).then(function (data) {
                $scope.billAmountList = data;
            });
        };

        $scope.changeStatus = function (amount) {

            if (angular.isUndefined(amount.PayStatusHistory)) {
                amount.PayStatusHistory = JSON.stringify({ PayStatus: amount.PayStatus, Date: amount.PayStatusDate });
            }
            var nextStatus = $scope.getNextStatus(amount.PayStatus);
            amount.PayStatus = nextStatus;
            amount.PayStatusDate = amount.ChangeStatus;
            amount.PayStatusHistory += JSON.stringify({ PayStatus: amount.PayStatus, Date: amount.PayStatusDate });

            datalayer.save(amount).then(function (data) {
                amount.ChangeStatus = null;
            });
        };

        $scope.getNextStatus = function (currentStatus) {
            if (currentStatus == 'Closed') {
                return currentStatus;
            }
            var statusList = $csShared.enums.BillPaymentStatus;
            var index = statusList.indexOf(currentStatus);
            return statusList[index + 1];
        };
    }]);