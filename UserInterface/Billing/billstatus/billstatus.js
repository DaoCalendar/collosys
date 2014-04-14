csapp.factory('billStatusDataLayer', ['Restangular', '$csnotify',
    function (rest, $csnotify) {
        var dldata = {};
        var restApi = rest.all('BillPaymentStatusApi');

        var fetchData = function(product, billmonth) {
            return restApi.customGET('GetBillAmountDetails', { products: product, billmonth: billmonth }).
                then(function(data) {
                    return data;
                });
        };
        return {
            dldata: dldata,
            fetchData:fetchData
        };
    }]);

csapp.factory('billStausFactory', function () {
    return {
        
    };
});

csapp.controller('billStatusController', ['$scope', 'billStatusDataLayer', 'billStausFactory','$csModels','$csShared',
    function ($scope, datalayer, factory, $csModels, $csShared) {
        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            $scope.BillAmount = $csModels.models.Billing.BillAmount;
        })();

        $scope.fetchData = function(seleData) {
            var month = moment(seleData.Month).format('YYYYMM');
            
            datalayer.fetchData(seleData.Product, month).then(function(data) {
                $scope.billAmountList = data;
            });
        };

        $scope.changeStatus = function (amount) {
            var statusList= $csShared.enums.BillPaymentStatus;
            var index = statusList.indexOf(amount.PayStatus);
            var nextStatus = statusList[index + 1];
            if (angular.isUndefined(amount.PrevStatusList)) {
                amount.PrevStatusList = [];
                amount.PrevStatusList.push({ Status: amount.PayStatus, Date: amount.PayStatusDate });
            }
            
            amount.PayStatus = nextStatus;
            amount.PayStatusDate = amount.ChangeStatus;
            amount.showDate = false;
            amount.ChangeStatus = '';
            amount.PrevStatusList.push({ Status: amount.PayStatus, Date: amount.PayStatusDate });
        };
    }]);