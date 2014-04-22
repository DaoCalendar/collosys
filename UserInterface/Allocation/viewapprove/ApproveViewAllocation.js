
csapp.controller('approveViewCntrl', ['$scope', 'approveViewDataLayer', 'approveViewFactory', '$modal', '$csfactory', "$csGrid",
    function ($scope, datalayer, factory, $modal, $csfactory, $grid) {

        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
            $csfactory.enableSpinner();
            $scope.datalayer.getProducts();
           
            $scope.$grid = $grid;
        })();

        $scope.openChangeModal = function () {
            $modal.open({
                templateUrl: '/Allocation/viewapprove/change-allocation-modal.html',
                controller: 'changAllocCtrl',
            });
        };

        $scope.openChurnModal = function () {
            $modal.open({
                templateUrl: '/Allocation/viewapprove/churn-allocation-modal.html',
                controller: 'churnAllocCtrl',
                resolve: {
                    modalData: function () {
                        return $scope.modalData;
                    }
                }
            });
        };

        $scope.getPagedData = function (allocData) {
            if (($csfactory.isNullOrEmptyString(allocData.selectedProduct))
                || ($csfactory.isNullOrEmptyString(allocData.selectedAllocation))) {
                return;
            }

            if ($scope.gettingPageData === true) return;
            $scope.gettingPageData = true;
            $csfactory.enableSpinner();

            datalayer.fetchData(allocData).then(function () {
                $scope.gridOptions = datalayer.dldata.gridOptions;
            }).finally(function () {
                $scope.gettingPageData = false;
            });;
        };

    }]);

csapp.factory('approveViewDataLayer', ['Restangular', '$csnotify', '$csGrid', '$csfactory',
    function (rest, $csnotify, $grid, $csfactory) {

        var restApi = rest.all("ApproveViewAllocationApi");

        var dldata = {};
        dldata.selectedAllocations = [];

        dldata.fromDate = moment().startOf('month').add('minute', 330).format('YYYY-MM-DD');
        dldata.toDate = moment().endOf('month').format('YYYY-MM-DD');

        var showErrorMessage = function (response) {
            $csnotify.error(response.data);
        };

        var getProducts = function () {
            restApi.customGETLIST("GetScbSystems").then(function (data) {
                dldata.ProductList = data;
            }, showErrorMessage);
        };


        var fetchData = function (allocData) {
            var allocStatus = "None";
            var aprovedStatus = "Approved";
            if (!$csfactory.isNullOrEmptyString(allocData.selectedAllocation)) {
                if (allocData.selectedAllocation === "Submitted") {
                    aprovedStatus = allocData.selectedAllocation;
                    allocStatus = "None";
                } else {
                    allocStatus = allocData.selectedAllocation;
                }
            }

            dldata.viewAllocationModel = {
                Products: allocData.selectedProduct,
                AllocationStatus: allocStatus,
                AproveStatus: aprovedStatus,
                FromDate: allocData.fromDate,
                ToDate: allocData.toDate
            };

            dldata.gridOptions = {};
            return restApi.customPOST(dldata.viewAllocationModel, "FetchPageData")
                .then(function (data) {
                    if (angular.isUndefined(data.QueryParams) || angular.isUndefined(data.QueryResult)) { return; }
                    dldata.gridOptions = $grid.InitGrid(data.QueryParams, dldata.gridOptions); // query params
                    $grid.SetData(dldata.gridOptions, data.QueryResult); // query result
                    $grid.RepotingHelper.GetReportList(dldata.gridOptions, data.ScreenName);
                }, function (response) {
                    $csnotify.error("Failed to fetch the data." + response.data.Message);
                });
        };

        var getstakeholders = function (param) {
            if (param != 'DoNotAllocate' && param != 'AllocateToTelecalling') {
                //var products = dldata.selectedProduct;
                var products = dldata.viewAllocationModel.Products;
                restApi.customGET("GetStakeholders", { 'products': products }).then(function (data) {
                    dldata.stakeholder = data;
                });
            }
        };

        var approveAllocations = function () {

            var allocStatus = "None";
            var aprovedStatus = "Approved";
            if (!$csfactory.isNullOrEmptyString(dldata.selectedAllocation)) {
                if (dldata.selectedAllocation === "Submitted") {
                    aprovedStatus = dldata.selectedAllocation;
                    allocStatus = "None";
                } else {
                    allocStatus = dldata.selectedAllocation;
                }
            }

            dldata.ChangeAllocationModel = {
                AllocList: dldata.gridOptions.$gridScope.selectedItems,
                AllocationStatus: allocStatus,
                Products: dldata.selectedProduct,
                AproveStatus: aprovedStatus,
                fromDate: dldata.fromDate,
                toDate: dldata.toDate
            };

            dldata.isInProcessing = true;
            restApi.customPOST(dldata.ChangeAllocationModel, "ApproveAllocations").then(function () {
                $csnotify.success("Allocations Approved");
                fetchData();
                dldata.isInProcessing = false;
            });
        };

        var rejectAllocations = function () {

            var allocStatus = "None";
            var aprovedStatus = "Approved";
            if (!$csfactory.isNullOrEmptyString(dldata.selectedAllocation)) {
                if (dldata.selectedAllocation === "Submitted") {
                    aprovedStatus = dldata.selectedAllocation;
                    allocStatus = "None";
                } else {
                    allocStatus = dldata.selectedAllocation;
                }
            }

            dldata.ChangeAllocationModel = {
                AllocList: dldata.gridOptions.$gridScope.selectedItems,
                AllocationStatus: allocStatus,
                Products: dldata.selectedProduct,
                AproveStatus: aprovedStatus,
                fromDate: dldata.fromDate,
                toDate: dldata.toDate
            };

            dldata.isInProcessing = true;
            restApi.customPOST(dldata.ChangeAllocationModel, "RejectChangeAllocations").then(function () {
                $csnotify.success("Allocations Rejected");
                fetchData();
                dldata.isInProcessing = false;
            });
        };

        var saveAllocationChanges = function (param) {
            dldata.isInProcessing = true;

            var allocStatus = "None";
            var aprovedStatus = "Approved";
            if (!$csfactory.isNullOrEmptyString(dldata.selectedAllocation)) {
                if (dldata.selectedAllocation === "Submitted") {
                    aprovedStatus = dldata.selectedAllocation;
                    allocStatus = "None";
                } else {
                    allocStatus = dldata.selectedAllocation;
                }
            }

            dldata.ChangeAllocationModel = {
                AllocList: dldata.selectedAllocations,
                ChangeAllocStatus: param,
                AllocationStatus: allocStatus,
                Products: dldata.selectedProduct,
                AproveStatus: aprovedStatus,
                fromDate: dldata.fromDate,
                toDate: dldata.toDate
            };

            return restApi.customPOST(dldata.ChangeAllocationModel, "ChangeAllocations").then(function () {
                $csnotify.success("Allocations Changed");
                fetchData();
                dldata.isInProcessing = false;
            });

        };

        return {
            dldata: dldata,
            getProducts: getProducts,
            fetchData: fetchData,
            getstakeholders: getstakeholders,
            approveAllocations: approveAllocations,
            rejectAllocations: rejectAllocations,
            saveAllocationChanges: saveAllocationChanges
        };
    }]);

csapp.factory('approveViewFactory', ['approveViewDataLayer',
    function (datalayer) {

        var dldata = datalayer.dldata;

        var disableSelect = function (data) {
            if (dldata.selectedAllocations.length > 0) {
                return (dldata.selectedAllocations.indexOf(data) === -1);
            }
            return true;
        };

        var showChurnButton = function () {
            var cnt = 0;
            if (angular.isDefined(dldata.gridOptions)) {
                if (angular.isDefined(dldata.gridOptions.$gridScope)) {
                    if (dldata.gridOptions.$gridScope.selectedItems.length > 0) {
                        _.forEach(dldata.gridOptions.$gridScope.selectedItems, function (item) {
                            if (item.AllocStatus !== "PolicyAllocated") {
                                return;
                            }
                            cnt++;
                        });
                        return (cnt === dldata.gridOptions.$gridScope.selectedItems.length);
                    } else return false;
                } else return false;
            }
            return false;
        };

        var setRowColor = function (data) {
            if (dldata.selectedAllocations.indexOf(data) !== -1) {
                return ({ backgroundColor: 'rgba(196, 224, 230, 0.66)' });
            }
            if (data.Status === "Approved")
                return ({ backgroundColor: '#c9dde1' });
            return {};
        };

        var ticks = function (data) {
            return (dldata.selectedAllocations.indexOf(data) !== -1);
        };

        //dldata.viewAllocationModel

        var selectAllocation = function (selected, allocation) {
            if (selected === true) {
                dldata.selectedAllocations.push(allocation);
                if (dldata.selectedAllocations.length === dldata.gridOptions.$gridScope.selectedItems.length)
                    dldata.selAll = true;
            }
            if (selected === false) {
                dldata.selectedAllocations.splice(dldata.selectedAllocations.indexOf(allocation), 1);
                dldata.selAll = false;
            }
        };

        var selectAll = function (selected) {
            if (selected === true) {
                _.forEach(dldata.gridOptions.$gridScope.selectedItems, function (item) {
                    dldata.selectedAllocations.push(item);
                });
                //$scope.selectedAllocations = $scope.gridOptions.$gridScope.selectedItems;
                dldata.sel = true;
            }
            if (selected === false) {
                dldata.selectedAllocations = [];
                dldata.sel = false;
            }
        };

        var assignStakeholderToAll = function (stkh) {
            if (stkh == "") {
                return null;
            }
            for (var i = 0; i < dldata.gridOptions.$gridScope.selectedItems.length; i++) {
                dldata.gridOptions.$gridScope.selectedItems[i].Stakeholder = JSON.parse(stkh);
            }
            return dldata;
        };

        var setStakeholder = function (stkh) {
            if (stkh === "" || angular.isUndefined(stkh)) {
                return null;
            }
            return JSON.parse(stkh);
        };

        return {
            disableSelect: disableSelect,
            showChurnButton: showChurnButton,
            setRowColor: setRowColor,
            ticks: ticks,
            selectAllocation: selectAllocation,
            selectAll: selectAll,
            assignStakeholderToAll: assignStakeholderToAll,
            setStakeholder: setStakeholder
        };

    }]);

csapp.controller('changAllocCtrl', ['$scope', '$modalInstance', 'approveViewDataLayer', 'approveViewFactory',
function ($scope, $modalInstance, datalayer, factory) {

    (function () {
        $scope.dldata = datalayer.dldata;
        $scope.gridOptions = $scope.dldata.gridOptions;
        $scope.datalayer = datalayer;
        $scope.factory = factory;
    })();

    $scope.closeModel = function () {
        $modalInstance.close();
    };

    $scope.saveAllocationChanges = function (param) {
        datalayer.saveAllocationChanges(param).then(function () {
            $modalInstance.close();
        });
    };

}]);

csapp.controller('churnAllocCtrl', ['$scope', '$modalInstance', 'approveViewDataLayer', 'approveViewFactory',
    function ($scope, $modalInstance, datalayer, factory) {

        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            $scope.factory = factory;
        })();


        $scope.closeModel = function () {
            $modalInstance.close();
        };
    }]);