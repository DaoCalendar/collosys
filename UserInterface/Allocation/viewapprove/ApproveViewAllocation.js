(
csapp.controller("approveViewCntrl", ["$scope", "$csfactory", "$csnotify", "Restangular", "$csGrid", function ($scope, $csfactory, $csnotify, rest, $grid) {
    "use strict";

    var restApi = rest.all("ApproveViewAllocationApi");

    $scope.init = function () {
        $scope.selectedAllocations = [];
        $scope.selAll = false;
        $scope.fromDate = moment().startOf('month').add('minute',330).format('YYYY-MM-DD');
        $scope.toDate = moment().endOf('month').format('YYYY-MM-DD');
    };
    $scope.init();

    $scope.disableSelect = function (data) {
        if ($scope.selectedAllocations.length > 0) {
            //var index = $scope.selectedAllocations.indexOf(data);
            //if (index != -1) return false;
            return ($scope.selectedAllocations.indexOf(data) === -1);
        }

        return true;
    };

    $scope.showChurnButton = function () {
        var cnt = 0;
        if (angular.isDefined($scope.gridOptions))
            if (angular.isDefined($scope.gridOptions.$gridScope)) {
                if ($scope.gridOptions.$gridScope.selectedItems.length > 0) {
                    _.forEach($scope.gridOptions.$gridScope.selectedItems, function (item) {
                        if (item.AllocStatus !== "PolicyAllocated") {
                            return;
                        }
                        cnt++;
                    });
                    return (cnt === $scope.gridOptions.$gridScope.selectedItems.length);
                } else return false;
            } else return false;
    };

    $scope.setRowColor = function (data) {
        if ($scope.selectedAllocations.indexOf(data) !== -1) {
            return ({ backgroundColor: 'rgba(196, 224, 230, 0.66)' });
        }
        if (data.Status === "Approved")
            return ({ backgroundColor: '#c9dde1' });
    };

    $scope.closeModal = function () {
        $scope.openModal = false;
        $scope.selectedAllocations = [];
        $scope.approveView = {};
    };

    //$scope.openChangeModal = function() {
    //    $scope.openModal = true;
    //    $scope.selecForChangeAllocations = $scope.gridOptions.$gridScope.selectedItems;

    //    _.forEach($scope.gridOptions.$gridScope.selectedItems, function (item) {
    //        $scope.selecForChangeAllocations.push(item);
    //    });
    //};

    $scope.ticks = function (data) {
        return ($scope.selectedAllocations.indexOf(data) !== -1);
    };

    $scope.selectAllocation = function (selected, allocation) {
        debugger;
        if (selected === true) {
            $scope.selectedAllocations.push(allocation);
            if ($scope.selectedAllocations.length === $scope.gridOptions.$gridScope.selectedItems.length)
                $scope.selAll = true;
        }
        if (selected === false) {
            $scope.selectedAllocations.splice($scope.selectedAllocations.indexOf(allocation), 1);
            $scope.selAll = false;
        }
    };

    $scope.selectAll = function (selected) {
        debugger;
        if (selected === true) {
            _.forEach($scope.gridOptions.$gridScope.selectedItems, function (item) {
                $scope.selectedAllocations.push(item);
            });
            //$scope.selectedAllocations = $scope.gridOptions.$gridScope.selectedItems;
            $scope.sel = true;
        }
        if (selected === false) {
            $scope.selectedAllocations = [];
            $scope.sel = false;
        }
    };

    $scope.assignStakeholderToAll = function (stkh) {
        debugger;
        if (stkh == "") {
            return null;
        }
        for (var i = 0; i < $scope.gridOptions.$gridScope.selectedItems.length; i++) {
            $scope.gridOptions.$gridScope.selectedItems[i].Stakeholder = JSON.parse(stkh);
        }
    };

    $scope.setStakeholder = function (stkh) {
        if (stkh == "") {
            return null;
        }
        return JSON.parse(stkh);
    };

    $scope.selectedProduct = '';

    var showErrorMessage = function (response) {
        $csnotify.error(response.data);
    };

    restApi.customGETLIST("GetScbSystems").then(function (data) {
        $scope.scbSystems = data;
        debugger;
    }, showErrorMessage);


    $scope.fetchData = function () {
        $scope.$grid = $grid;
        $scope.gridOptions = {};
        debugger;
        var allocStatus = "None";
        var aprovedStatus = "Approved";
        if (!$csfactory.isNullOrEmptyString($scope.selectedAllocation)) {
            if ($scope.selectedAllocation === "Submitted") {
                aprovedStatus = $scope.selectedAllocation;
                allocStatus = "None";
            } else {
                allocStatus = $scope.selectedAllocation;
            }
        }
        
        $scope.viewAllocationModel = {
            Products: $scope.selectedProduct,
            AllocationStatus: allocStatus,
            AproveStatus: aprovedStatus,
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate
        };

        debugger;
        if (!$csfactory.isNullOrEmptyString($scope.selectedProduct) && !$csfactory.isNullOrEmptyString($scope.selectedAllocation)) {
            restApi.customPOST($scope.viewAllocationModel,"FetchPageData")
                .then(function (data) {
                    if (angular.isUndefined(data.QueryParams) || angular.isUndefined(data.QueryResult)) {
                        return;
                    }
                    $scope.gridOptions = $grid.InitGrid(data.QueryParams); // query params
                    $grid.SetData($scope.gridOptions, data.QueryResult); // query result
                    $grid.RepotingHelper.GetReportList($scope.gridOptions, data.ScreenName);
                }, showErrorMessage);
        }
    };

    $scope.getstakeholders = function (param) {
        debugger;
        if (param != 'DoNotAllocate' && param != 'AllocateToTelecalling') {
            var products = $scope.selectedProduct;
            restApi.customGET("GetStakeholders", { 'products': products }).then(function (data) {
                $scope.stakeholder = data;
            });
        }
    };

    $scope.approveAllocations = function () {

        var allocStatus = "None";
        var aprovedStatus = "Approved";
        if (!$csfactory.isNullOrEmptyString($scope.selectedAllocation)) {
            if ($scope.selectedAllocation === "Submitted") {
                aprovedStatus = $scope.selectedAllocation;
                allocStatus = "None";
            } else {
                allocStatus = $scope.selectedAllocation;
            }
        }

        $scope.ChangeAllocationModel = {
            AllocList: $scope.gridOptions.$gridScope.selectedItems,
            AllocationStatus: allocStatus,
            Products: $scope.selectedProduct,
            AproveStatus: aprovedStatus,
            fromDate: $scope.fromDate,
            toDate: $scope.toDate
        };

        $scope.isInProcessing = true;
        restApi.customPOST($scope.ChangeAllocationModel, "ApproveAllocations").then(function () {
            $csnotify.success("Allocations Approved");
            $scope.fetchData();
            $scope.isInProcessing = false;
        });
    };

    $scope.rejectAllocations = function () {

        var allocStatus = "None";
        var aprovedStatus = "Approved";
        if (!$csfactory.isNullOrEmptyString($scope.selectedAllocation)) {
            if ($scope.selectedAllocation === "Submitted") {
                aprovedStatus = $scope.selectedAllocation;
                allocStatus = "None";
            } else {
                allocStatus = $scope.selectedAllocation;
            }
        }

        $scope.ChangeAllocationModel = {
            AllocList: $scope.gridOptions.$gridScope.selectedItems,
            AllocationStatus: allocStatus,
            Products: $scope.selectedProduct,
            AproveStatus: aprovedStatus,
            fromDate: $scope.fromDate,
            toDate: $scope.toDate
        };

        $scope.isInProcessing = true;
        restApi.customPOST($scope.ChangeAllocationModel, "RejectChangeAllocations").then(function () {
            $csnotify.success("Allocations Rejected");
            $scope.fetchData();
            $scope.isInProcessing = false;
        });
    };

    $scope.saveAllocationChanges = function (param) {
        debugger;
        $scope.isInProcessing = true;

        var allocStatus = "None";
        var aprovedStatus = "Approved";
        if (!$csfactory.isNullOrEmptyString($scope.selectedAllocation)) {
            if ($scope.selectedAllocation === "Submitted") {
                aprovedStatus = $scope.selectedAllocation;
                allocStatus = "None";
            } else {
                allocStatus = $scope.selectedAllocation;
            }
        }

        $scope.ChangeAllocationModel = {
            AllocList: $scope.selectedAllocations,
            ChangeAllocStatus: param,
            AllocationStatus: allocStatus,
            Products: $scope.selectedProduct,
            AproveStatus: aprovedStatus,
            fromDate: $scope.fromDate,
            toDate: $scope.toDate
        };
        $scope.openModal = false;
        restApi.customPOST($scope.ChangeAllocationModel, "ChangeAllocations").then(function () {
            $csnotify.success("Allocations Changed");
            $scope.closeModal();
            $scope.fetchData();
            $scope.isInProcessing = false;
        });

    };
}])
);
//{ products: $scope.selectedSystem, allocStatus: param }
//$scope.assignStakeholder = function (data, stkh) {
//    debugger;
//    var currDataIndex = $scope.selectedAllocations.indexOf(data);
//    $scope.selectedAllocations[currDataIndex].Stakeholder = stkh;
//};
