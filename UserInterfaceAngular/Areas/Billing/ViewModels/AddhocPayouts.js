csapp.controller("adhocPayoutCtrl", ["$scope", "$csnotify", "Restangular", '$Validations', function ($scope,
    $csnotify, rest, $validation) {

    var restApi = rest.all("AddhocPayoutsApi");

    //#region "init"
    $scope.init = function () {
        $scope.val = $validation;
        $scope.productsList = [];
        $scope.stakeholderList = [];
        $scope.adhocPayout = {};
        $scope.adhocPayoutList = [];
        $scope.adhocPayoutAllList = [];
        $scope.adhocPayout.Tenure = 1;
        $scope.transcationtypes = [{ display: 'Incentive', value: 'true' }, { display: 'Fine', value: 'false' }];
        $scope.taxtype = ['PreTax', 'PostTax'];
        $scope.Reasonstype = [{ display: 'Performance', transcationtype: 'true' },
            { display: 'Customer Complaints', transcationtype: 'false' }];
    };
    $scope.init();
    //#endregion

    //#region "Get Products"
    restApi.customGET("GetProducts").then(function (data) {
        $scope.productsList = data;
    }, function (data) {
        $csnotify.error(data);
    });
    //#endregion

    //#region "Get StakeholderName And the existing data for that product"
    $scope.changeProductCategory = function () {
        restApi.customGET("GetStakeHolders", { products: $scope.selectedProduct }).then(function (data) {
            $scope.stakeholderList = data;
            $scope.adhocPayout.Tenure = 1;
        }, function (data) {
            $csnotify.error(data);
        });
        restApi.customGET("GetAdhocdata", { products: $scope.selectedProduct }).then(function (data) {
            $scope.adhocPayoutAllList = data;
            $scope.adhocPayoutList = data;
        }, function (data) {
            $csnotify.error(data);
        });
    };
    //#endregion

    $scope.resetadhocPayout = function (products) {
        $scope.adhocPayout = {};
        $scope.adhocPayout.Products = products;
    };

    //#region "SaveData"
    $scope.SaveData = function (adhocPayout) {
        if ($scope.adhocPayout.IsRecurring === "false") {
            $scope.adhocPayout.Tenure = 1;
        }
        var endDate = moment($scope.adhocPayout.StartMonth).add('month', ($scope.adhocPayout.Tenure - 1));
        $csnotify.success(endDate);
        $scope.adhocPayout.StartMonth = moment($scope.adhocPayout.StartMonth).format('MMM-YYYY');
        $scope.adhocPayout.EndMonth = moment(endDate).format('MMM-YYYY');
        $scope.adhocPayout.RemainingAmount = $scope.adhocPayout.TotalAmount;

        restApi.customPOST(adhocPayout, 'Post').then(function (data) {
            $scope.adhocPayoutList.push(adhocPayout);
            $csnotify.success('All File Columns Save Successfully');
            $scope.CloseAdhocPayoutManager();
        }, function () {
            $csnotify.error();
        });
    };
    //#endregion

    $scope.SelectTransaction = function (st) {
        $scope.selecttransdata = _.where($scope.Reasonstype, { 'transcationtype': st });
    };


    $scope.OpenAdhocPayoutManager = function () {
        $scope.adhocPayout.Products = $scope.selectedProduct;

        if ($scope.selectedStkholderId) {
            $scope.adhocPayout.Stakeholder = _.find($scope.stakeholderList, { Id: $scope.selectedStkholderId });
        }

        $scope.OpenAdhocPayout = true;
    };


    $scope.CloseAdhocPayoutManager = function () {
        $scope.OpenAdhocPayout = false;
        $scope.adhocPayout = {};
        //$scope.resetadhocPayout($scope.adhocPayout.Products);
    };

    $scope.ShowData = function (selectedStkholderId) {
        $scope.adhocPayoutList = _.filter($scope.adhocPayoutAllList, function (adhocPayout) {
            return (adhocPayout.Stakeholder.Id == selectedStkholderId);
        });
    };


}]);

