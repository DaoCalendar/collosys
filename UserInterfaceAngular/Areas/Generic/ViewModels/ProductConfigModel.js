
//#region controller
(
csapp.controller("ProductConfigController", ["$scope", '$csnotify', 'Restangular', function ($scope, $csnotify, rest) {

    'use strict';

    var apictrl = rest.all('ProductConfigApi');

    $scope.codes = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30'];


    //#region Other functions

    $scope.reset = function () {
        $scope.isReadOnly = false;
        $scope.editIndex = -1;
        $scope.productconfig = {
            ProductName: "",
            ProductGroup: "",
            AllocationResetStrategy: "HasMonthEndReset",
            BillingStrategy: "MonthEndOpenAndCloseCycle",
            HasTelecalling: true,
            FrCutOffDaysCycle: 5,
            FrCutOffDaysMonth: 7,
            CycleCodes: ["1", "5", "15", "25"]
        };
    };

    $scope.select2Options = {
        'multiple': true

    };

    $scope.isSelected = function (item, val) {
        if (item === val) {
            return "btn btn-small btn-success";
        } else {
            return "btn btn-small btn-info";
        }
       
    };

    //#endregion

    //#region Model Operations

    $scope.openModel = function () {
        $scope.reset();
        $scope.modelHeader = "Add New Product Configuration";
        $scope.shouldBeOpen = true;
    };

    $scope.closeModel = function () {
        $scope.shouldBeOpen = false;
    };

    $scope.modelOption = {
        backdropFade: true,
        dialogFade: true
    };

    $scope.yesToDelete = function () {
        $scope.deleteProduct($scope.deletedProduct.Id, $scope.editIndex);
        $scope.showDeleteModel = false;
    };

    $scope.noToDelete = function () {
        $scope.deletedProduct = {};
        $scope.editIndex = -1;
        $scope.showDeleteModel = false;
    };

    $scope.showDeleteModelPopup = function (product, index) {
        $scope.deletedProduct = product;
        $scope.editIndex = index;
        $scope.showDeleteModel = true;
    };

    $scope.openModelData = function (productRow, readOnly, index) {
        debugger;
        if (readOnly === false) {
            $scope.modelHeader = "Update Product Configuration";
            $scope.isReadOnly = readOnly;
        } else {
            $scope.modelHeader = "View Product Configuration";
            $scope.isReadOnly = readOnly;
        }
        $scope.productconfig = angular.copy(productRow);
        $scope.productconfig.CycleCodes = JSON.parse($scope.productconfig.CycleCodes);
        $scope.shouldBeOpen = true;
        $scope.editIndex = index;
    };

    $scope.noTelecalling = function (productconfig) {
        if (productconfig.HasTelecalling == false) {
            productconfig.FrCutOffDaysCycle = 0;
            productconfig.FrCutOffDaysMonth = 0;
        }
    };


    $scope.AddinLocal = function (productconfig) {
        if ($scope.editIndex === -1) {
            $scope.productList.push(productconfig);
        } else {
            $scope.productList[$scope.editIndex] = productconfig;
        }
    };

    //#endregion

    //#region DB Operations

    apictrl.customGETLIST('Get').then(function (data) {
        $scope.productList = data;
        $csnotify.success('All Products Loaded Successfully.');
    });

    $scope.saveProduct = function (productconfig) {

        productconfig.CycleCodes = JSON.stringify(productconfig.CycleCodes);
        if (productconfig.Id) {
            apictrl.customPUT(productconfig, 'Put', { id: productconfig.Id }).then(function (data) {
                $csnotify.success('Product Configuration Updated Successfully.');
                $scope.AddinLocal(data);
                $scope.reset();
                $scope.closeModel();
            }, function (response) {
                $csnotify.error(response);
            });
        } else {
            apictrl.customPOST(productconfig, 'Post').then(function (data) {
                $csnotify.success('Product Configuration Saved Successfully.');
                $scope.AddinLocal(data);
                $scope.reset();
                $scope.closeModel();
            }, function (response) {
                $csnotify.error(response);
            });
        }
    };

    $scope.deleteProduct = function (id, index) {
        apictrl.customDELETE('Delete', { id: id })
            .then(function () {
                $csnotify.success('Product Configuration Deleted Successfully.');
                $scope.productList.splice(index, 1);
            });
    };

    //#endregion

}])
);
//#endregion