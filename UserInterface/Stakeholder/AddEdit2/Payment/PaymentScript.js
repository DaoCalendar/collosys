﻿csapp.controller("PaymentCntrl", ["$scope", "$csModels", function ($scope, $csModels) {
    (function () {
        $scope.paymentModel = $csModels.getColumns("StkhPayment");
        $scope.FixedPay = {
            EmployeePF: 0.0,
            EmployerPF: 0.0,
            EmployeeESIC: 0.0,
            EmployerESIC: 0.0,
            ServiceCharge: 0.0,
            ServiceTax: 0.0,
            ServPer: 0.0,
            Total: 0.0
        };

        $scope.SalDetails = {
            pfEmployer: 0.0,
            pfEmployee: 0.0,
            totalNotEsic: 0.0,
            esicEmployee: 0.0,
            esicEmployer: 0.0,
            serviceCharge: 0.0,
            serviceTax: 0.0,
            servPer: 0.0,
            total: 0.0
        };
    })();
    

    $scope.TotalPayment = function (basic, hra, other) {
        var serviceCharge = 1;

        if (angular.isUndefined(basic)) {
            basic = 0;
        }
        if (angular.isUndefined(hra)) {
            hra = 0;
        }
        if (angular.isUndefined(other)) {
            other = 0;
        }
        if (Number(basic) !== 0) {
            if ($scope.selectedHierarchy.HasFixedIndividual || true) {
                $scope.SalDetails.pf = (Number(basic) * Number($scope.FixedPay.EmployeePF)) / 100;
                $scope.SalDetails.totalNotEsic = Number(basic) + Number(hra) + Number(other);

                $scope.SalDetails.esicEmployee = (($scope.SalDetails.totalNotEsic * Number($scope.FixedPay.EmployeeESIC)) / 100);
                $scope.SalDetails.esicEmployee = Number(parseFloat($scope.SalDetails.esicEmployee).toFixed(2));

                $scope.SalDetails.esicEmployer = ($scope.SalDetails.totalNotEsic * Number($scope.FixedPay.EmployerESIC)) / 100;
                $scope.SalDetails.esicEmployer = Number(parseFloat($scope.SalDetails.esicEmployer).toFixed(2));

                $scope.SalDetails.serviceCharge = ((Number(basic) * serviceCharge) / 100);
                $scope.SalDetails.serviceCharge = Number(parseFloat($scope.SalDetails.serviceCharge).toFixed(2));

                $scope.SalDetails.serviceTax = ((Number(basic) * Number($scope.FixedPay.ServiceTax)) / 100);
                $scope.SalDetails.serviceTax = Number(parseFloat($scope.SalDetails.serviceTax).toFixed(2));

                $scope.SalDetails.total = $scope.SalDetails.pf * 2 +
                    $scope.SalDetails.totalNotEsic +
                    $scope.SalDetails.esicEmployee +
                    $scope.SalDetails.esicEmployer +
                    $scope.SalDetails.serviceCharge +
                    $scope.SalDetails.serviceTax;
                $scope.SalDetails.total = $scope.SalDetails.total.toFixed(2);
            } else {
                $scope.SalDetails.total = basic;
            }
            return $scope.SalDetails.total;
        }
        return 0;
    };



}]);