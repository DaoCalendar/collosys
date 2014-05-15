
//#region controller

csapp.factory("ProductsDatalayer", ["Restangular", "$csnotify", function (rest, $csnotify) {

    var reatApi = rest.all('ProductConfigApi');
    var dldata = {};
    dldata.codes = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30'];

    var getAll = function () {
        reatApi.customGETLIST('Get').then(function (data) {
            dldata.productList = data;
            $csnotify.success('All Products Loaded Successfully.');
        });
    };

    var saveProduct = function (productconfig) {

        productconfig.CycleCodes = JSON.stringify(productconfig.CycleCodes);
        if (productconfig.Id) {
            return reatApi.customPUT(productconfig, 'Put', { id: productconfig.Id }).then(function (data) {
                $csnotify.success('Product Configuration Updated Successfully.');
                return data;
            }, function (response) {
                $csnotify.error(response);
            });
        } else {
            return reatApi.customPOST(productconfig, 'Post').then(function (data) {
                $csnotify.success('Product Configuration Saved Successfully.');
                return data;
            }, function (response) {
                $csnotify.error(response);
            });
        }
    };


    return {
        dldata: dldata,
        GetAll: getAll,
        Save: saveProduct
    };

}]);

csapp.factory("ProductFactory", [function () {

    var reset = function (dldata) {
        dldata.isReadOnly = false;
        dldata.editIndex = -1;
        dldata.productconfig = {
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

    var addinLocal = function (productconfig, dldata) {
        if (dldata.editIndex === -1) {
            dldata.productList.push(productconfig);
        } else {
            dldata.productList[dldata.editIndex] = productconfig;
        }
    };

    var noTelecalling = function (productconfig, dldata) {
        if (dldata.productconfig.HasTelecalling == false) {
            dldata.productconfig.FrCutOffDaysCycle = 0;
            dldata.productconfig.FrCutOffDaysMonth = 0;
        }
    };

    return {
        reset: reset,
        AddinLocal: addinLocal,
        noTelecalling: noTelecalling
    };
}]);


csapp.controller("ProductConfigController", ["$scope", '$csnotify', 'Restangular', '$modal', "ProductsDatalayer",
    function ($scope, $csnotify, rest, $modal, datalayer) {

        $scope.codes = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30'];

        (function () {
            $scope.dldata = datalayer.dldata;
            datalayer.GetAll();
         
        })();

        $scope.openModelData = function(productRow, readOnly, index) {
            $scope.dldata.productconfig = {};
            if (readOnly === false) {
                $scope.dldata.modelHeader = "Update Product Configuration";
                $scope.dldata.isReadOnly = readOnly;
            } else {
                $scope.dldata.modelHeader = "View Product Configuration";
                $scope.dldata.isReadOnly = readOnly;
            }
            $scope.dldata.productconfig = angular.copy(productRow);
            $scope.dldata.productconfig.CycleCodes = JSON.parse($scope.dldata.productconfig.CycleCodes);
            $scope.dldata.editIndex = index;

            $modal.open({
                templateUrl: baseUrl + 'Generic/product/updateViewConfiguration.html',
                controller: 'updateView',
            });
        };       
    }]);


csapp.controller("updateView", ["$scope", "ProductsDatalayer", "$modalInstance", "ProductFactory", "$csModels",
    function ($scope, datalayer, $modalInstance, factory, $csModels) {
    
    (function() {
        $scope.dldata = datalayer.dldata;
        $scope.factory = factory;
        $scope.Products = $csModels.getColumns("Product");
    })();
    
    $scope.isSelected = function (item, val) {
        if (item === val) {
            return "btn btn-small btn-success";
        } else {
            return "btn btn-small btn-info";
        }
    };
    
    $scope.closeModel = function () {
        $modalInstance.dismiss();
    };
   
    $scope.saveProduct = function (productconfig) {
        datalayer.Save(productconfig).then(function (data) {
            factory.AddinLocal(data,$scope.dldata);
            factory.reset($scope.dldata);
            $modalInstance.close();
        });

       
    };

    $scope.noTelecalling = function (productconfig) {
        if (productconfig.HasTelecalling == false) {
            productconfig.FrCutOffDaysCycle = 0;
            productconfig.FrCutOffDaysMonth = 0;
        }
    };

    
}]);

//#endregion



//$scope.reset = function() {
//    $scope.isReadOnly = false;
//    $scope.editIndex = -1;
//    $scope.productconfig = {
//        ProductName: "",
//        ProductGroup: "",
//        AllocationResetStrategy: "HasMonthEndReset",
//        BillingStrategy: "MonthEndOpenAndCloseCycle",
//        HasTelecalling: true,
//        FrCutOffDaysCycle: 5,
//        FrCutOffDaysMonth: 7,
//        CycleCodes: ["1", "5", "15", "25"]
//    };
//};

//$scope.select2Options = {
//    'multiple': true
//};

//$scope.isSelected = function(item, val) {
//    if (item === val) {
//        return "btn btn-small btn-success";
//    } else {
//        return "btn btn-small btn-info";
//    }

//};


//$scope.openModel = function() {
//    $scope.reset();
//    $scope.modelHeader = "Add New Product Configuration";
//    $scope.shouldBeOpen = true;
//};



//$scope.modelOption = {
//    backdropFade: true,
//    dialogFade: true
//};

//$scope.yesToDelete = function() {
//    $scope.deleteProduct($scope.deletedProduct.Id, $scope.editIndex);
//    $scope.showDeleteModel = false;
//};

//$scope.noToDelete = function() {
//    $scope.deletedProduct = {};
//    $scope.editIndex = -1;
//    $scope.showDeleteModel = false;
//};

//$scope.showDeleteModelPopup = function(product, index) {
//    $scope.deletedProduct = product;
//    $scope.editIndex = index;
//    $scope.showDeleteModel = true;
//};


//$scope.AddinLocal = function(productconfig) {
//    if ($scope.editIndex === -1) {
//        $scope.productList.push(productconfig);
//    } else {
//        $scope.productList[$scope.editIndex] = productconfig;
//    }
//};

//$scope.saveProduct = function(productconfig) {
//    productconfig.CycleCodes = JSON.stringify(productconfig.CycleCodes);
//    if (productconfig.Id) {
//        apictrl.customPUT(productconfig, 'Put', { id: productconfig.Id }).then(function(data) {
//            $csnotify.success('Product Configuration Updated Successfully.');
//            $scope.AddinLocal(data);
//            $scope.reset();
//            $scope.closeModel();
//        }, function(response) {
//            $csnotify.error(response);
//        });
//    } else {
//        apictrl.customPOST(productconfig, 'Post').then(function(data) {
//            $csnotify.success('Product Configuration Saved Successfully.');
//            $scope.AddinLocal(data);
//            $scope.reset();
//            $scope.closeModel();
//        }, function(response) {
//            $csnotify.error(response);
//        });
//    }
//};

//$scope.deleteProduct = function(id, index) {
//    apictrl.customDELETE('Delete', { id: id })
//        .then(function() {
//            $csnotify.success('Product Configuration Deleted Successfully.');
//            $scope.productList.splice(index, 1);
//        });
//};


//productconfig.CycleCodes = JSON.stringify(productconfig.CycleCodes);
//if (productconfig.Id) {
//    apictrl.customPUT(productconfig, 'Put', { id: productconfig.Id }).then(function (data) {
//        $csnotify.success('Product Configuration Updated Successfully.');
//        $scope.AddinLocal(data);
//        $scope.reset();
//        $scope.closeModel();
//    }, function (response) {
//        $csnotify.error(response);
//    });
//} else {
//    apictrl.customPOST(productconfig, 'Post').then(function (data) {
//        $csnotify.success('Product Configuration Saved Successfully.');
//        $scope.AddinLocal(data);
//        $scope.reset();
//        $scope.closeModel();
//    }, function (response) {
//        $csnotify.error(response);
//    });
//}


//$scope.reset = function () {
//    $scope.dldata.isReadOnly = false;
//    $scope.dldata.editIndex = -1;
//    $scope.dldata.productconfig = {
//        ProductName: "",
//        ProductGroup: "",
//        AllocationResetStrategy: "HasMonthEndReset",
//        BillingStrategy: "MonthEndOpenAndCloseCycle",
//        HasTelecalling: true,
//        FrCutOffDaysCycle: 5,
//        FrCutOffDaysMonth: 7,
//        CycleCodes: ["1", "5", "15", "25"]
//    };
//};

//$scope.AddinLocal = function (productconfig) {
//    if ($scope.dldata.editIndex === -1) {
//        $scope.dldata.productList.push(productconfig);
//    } else {
//        $scope.dldata.productList[$scope.dldata.editIndex] = productconfig;
//    }
//};
