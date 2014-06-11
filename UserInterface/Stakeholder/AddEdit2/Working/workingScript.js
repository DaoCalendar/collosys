
csapp.factory("StakeWorkingDatalayer", ["$csnotify", "Restangular", function ($csnotify, rest) {

    var restApi = rest.all('WorkingApi');

    var getReportsTo = function (stake) {
        return restApi.customGET('GetWorkingReportsTo', { 'id': stake.Hierarchy.Id, 'level': stake.Hierarchy.ReportingLevel })
            .then(function (data) {
                return data;
            });
    };

    var getWorkingData = function (stakeId) {
        return restApi.customGET('GetStakeWorkingData', { stakeholderId: stakeId })
            .then(function (data) {
                return data;
            });
    };

    var getPaymentDetails = function () {
        return restApi.customGET('GetStakePaymentData')
            .then(function (data) {
                return data;
            });
    };

    var savePayment = function (paymentData) {
        restApi.customPOST(paymentData, "SavePayment");
    };

    return {
        GetStakeholder: getWorkingData,
        GetPaymentDetails: getPaymentDetails,
        SavePayment: savePayment,
        GetReportsTo: getReportsTo
    };
}]);

csapp.factory("StakeWorkingFactory", ["$csfactory", function ($csfactory) {

    var getFixedPayObj = function (data) {
        var fixedPayObj = {};
        _.forEach(data, function (item) {
            fixedPayObj[item.ParamName] = parseFloat(item.Value);
        });
        return fixedPayObj;
    };

    return {
        GetFixedPayObj: getFixedPayObj
    };
}]);

csapp.controller("StakeWorkingCntrl", ["$scope", "$routeParams", "StakeWorkingDatalayer", "$csModels", "StakeWorkingFactory", function ($scope, $routeParams, datalayer, $csModels, factory) {

    (function () {
        datalayer.GetStakeholder($routeParams.stakeId).then(function (data) {
            $scope.selectedHierarchy = data.Hierarchy;
            $scope.currStakeholder = data;
        }).then(function () {
            datalayer.GetPaymentDetails().then(function (data) {
                $scope.FixedPay = factory.GetFixedPayObj(data);
                console.log("fixedPay: ", $scope.FixedPay);
            });
        });

        $scope.paymentModel = $csModels.getColumns("StkhPayment");
        $scope.workingModel = $csModels.getColumns("StkhWorking");
        $scope.Payment = {};

    })();

    $scope.TotalPayment = function (basic, hra, other) {
        $scope.SalDetails = {};

        if (angular.isUndefined(basic)) {
            basic = 0;
        }
        if (angular.isUndefined(hra)) {
            hra = 0;
        }
        if (angular.isUndefined(other)) {
            other = 0;
        }
        if (basic !== 0) {
            if ($scope.selectedHierarchy.HasFixedIndividual || true) {
                $scope.SalDetails.pf = Number((basic) * ($scope.FixedPay.EmployeePF) / 100);
                $scope.SalDetails.pf = Number($scope.SalDetails.pf.toFixed(2));

                $scope.SalDetails.employerPf = Number((basic) * ($scope.FixedPay.EmployerPF) / 100);
                $scope.SalDetails.employerPf = Number($scope.SalDetails.employerPf.toFixed(2));

                $scope.SalDetails.totalNotEsic = Number((basic) + (hra) + (other));

                $scope.SalDetails.esicEmployee = Number(($scope.SalDetails.totalNotEsic * ($scope.FixedPay.EmployeeESIC)) / 100);
                $scope.SalDetails.esicEmployee = Number($scope.SalDetails.esicEmployee.toFixed(2));

                $scope.SalDetails.esicEmployer = Number($scope.SalDetails.totalNotEsic * Number($scope.FixedPay.EmployerESIC) / 100);
                $scope.SalDetails.esicEmployer = Number($scope.SalDetails.esicEmployer.toFixed(2));

                $scope.SalDetails.ServiceCharge = $scope.FixedPay.ServiceCharge !== 0 ? Number((basic * $scope.FixedPay.ServiceCharge)) / 100 : 0;
                $scope.SalDetails.ServiceCharge = Number($scope.SalDetails.ServiceCharge.toFixed(2));

                $scope.SalDetails.serviceTax = Number(basic * ($scope.FixedPay.ServiceTax) / 100);
                $scope.SalDetails.serviceTax = Number($scope.SalDetails.serviceTax.toFixed(2));

                $scope.SalDetails.total =
                    $scope.SalDetails.pf * 2 +
                    $scope.SalDetails.totalNotEsic +
                    $scope.SalDetails.esicEmployee +
                    $scope.SalDetails.esicEmployer +
                    $scope.SalDetails.ServiceCharge +
                    $scope.SalDetails.serviceTax;
                $scope.SalDetails.total = Number($scope.SalDetails.total.toFixed(2));
            } else {
                $scope.SalDetails.total = basic;
            }
        }
        return 0;
    };

    $scope.getReportsTo = function (product) {
        datalayer.GetReportsTo($scope.currStakeholder).then(function (reportsToList) {
            $scope.reportsToList = reportsToList;
        });
    };

    $scope.savePayment = function (paymentData) {
        paymentData.Stakeholder = $scope.currStakeholder;
        datalayer.SavePayment(paymentData);
    };

}]);