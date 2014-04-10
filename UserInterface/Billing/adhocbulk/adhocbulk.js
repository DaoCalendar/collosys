

csapp.factory('adhocbulkDataLayer', ['Restangular', '$csnotify',
    function (rest, $csnotify) {
        var dldata = {};
        var restApi = rest.all("AdhocBulkApi");

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

        return {
            dldata: dldata,
            getProducts: getProducts
        };
    }]);

csapp.factory('adhocbulkFactory', ['$csfactory', 'adhocbulkDataLayer',
    function ($csfactory, datalayer) {
        var dldata = datalayer.dldata;
        var selectTransaction = function (st) {
            var data = _.where(dldata.Reasonstype, { 'transcationtype': st });
            return data;
        };
        return {
            selectTransaction: selectTransaction
        };
    }]);

csapp.controller('adhocbulkCtrl', ['$scope', 'adhocbulkDataLayer', 'adhocbulkFactory',
    function ($scope, datalayer, factory) {
        var addDefaultPayment = function () {
            $scope.PaymentList.push(getDefaultPayment());
        };

        var getDefaultPayment = function () {
            var payment = {
                Product: '',
                TotalAmount: '',
                IsCredit: '',
                ReasonCode: '',
                Description: '',
                IsRecurring: '',
            };
            return payment;
        };
        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            datalayer.getProducts();
            $scope.PaymentList = [];
            addDefaultPayment();
        })();
        $scope.onSubmit = function (payment) {
            addDefaultPayment();
        };


    }]);