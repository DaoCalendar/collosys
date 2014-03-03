/// <reference path="temp.js" />

var approveStake = (angular.module('approvemodel', ['ui.bootstrap', 'ui.filters', 'ui.bootstrap.modal']));

var url = 'api/showstakeholders/';


(
csapp.controller('approveCtrl', ['$scope', '$http', '$log', '$csnotify', 'Restangular', function ($scope, $http, $log, $csnotify, rest) {

    'use strict';

    //#region members

    //show model
    $scope.count = 0;

    //show model of reject
    $scope.showRejectModel = false;

    //show approve model
    $scope.showApproveModel = false;

    //show approve all model
    $scope.showApproveAllModel = false;

    //show individual approval and reject functionality
    $scope.isApprovedFirst = false;

    //index in list
    var indexStake = -1;

    //list for all stakeholders
    $scope.listOfStakeholders = [];

    //stakeholder object
    $scope.Stakeholder = {};

    //stakehierarchy object
    $scope.Hierarchy = {};

    //description
    $scope.Description = '';

    //#endregion

    //#region get

    //Get list of all stakeholders
    var apiedit = rest.all('showstakeholders');

    apiedit.customGET('ListForApprove').then(function (data) {
        $scope.listOfStakeholders = data;
    }, function (data) {
        $csnotify.error(data.data.Message);
    });

    //#endregion

    //#region public 

    //approve stakeholder
    $scope.approvemulti = function (stakeholder, index) {
        //stakeholder.Status = 'Approved';
        indexStake = index;
        $scope.approve(stakeholder);
    };

    //approve single
    $scope.approve = function (stakeholder) {

        stakeholder.IsPaymentChange = false;
        stakeholder.IsWorkingChange = false;
        stakeholder.IsAddressChange = false;
        stakeholder.Status = 'Approved';

        apiedit.customPOST(stakeholder, 'SaveApprovedWithUser').then(function () {
            $scope.listOfStakeholders.splice(indexStake, 1);
            $csnotify.success('Stakeholder Approved');
        }, function (data) {
            $csnotify.error(data.data.Message);
        });

        $scope.count = 0;
    };

    $scope.rejectSingle = function (stakeholder) {
        debugger;
        $scope.reject(stakeholder);
        $scope.count = 0;
    };

    //reject stakeholder
    $scope.reject = function (stakeholder) {
        stakeholder.Description = $scope.Description;
        stakeholder.Status = 'Rejected';
        apiedit.customPOST(stakeholder, 'SaveEditedStakeholder').then(function () {
            $scope.listOfStakeholders.splice(indexStake, 1);
            $scope.closeModel();
            $csnotify.success('Stakeholder Rejected');
        }, function (data) {
            $csnotify.error(data.data.Message);
        });
    };

    $scope.showRejectModelFun = function (stakeholder, index) {
        $scope.Stakeholder = stakeholder;
        indexStake = index;
        $scope.showRejectModel = true;
    };

    $scope.approveFromDialoge = function (stakeholder) {
        debugger;
        stakeholder.IsPaymentChange = false;
        stakeholder.IsWorkingChange = false;
        stakeholder.IsAddressChange = false;
        stakeholder.Status = 'Approved';

        apiedit.customPOST(stakeholder, 'SaveApprovedWithUser').then(function () {
            var index = $scope.listOfStakeholders.indexOf(stakeholder);
            $scope.listOfStakeholders.splice(index, 1);
            $csnotify.success('Stakeholder approved');
            $log.warn('Staekholder Approved and user created');
            return;
        }, function (data) {
            $csnotify.error(data.data.Message);
        });
    };


    $scope.yesToApprove = function () {
        $log.warn('Clicked on Yes to apperove');
        $scope.approveFromDialoge($scope.Stakeholder);
        $scope.showApproveModel = false;
    };

    $scope.yesToApproveAll = function () {
        $scope.approveAll();
        $scope.showApproveAllModel = false;
    };

    $scope.noToApprove = function () {
        $scope.Stakeholder = {};
        indexStake = -1;
        $scope.showApproveModel = false;
    };

    $scope.noToApproveAll = function () {
        $scope.showApproveAllModel = false;
    };

    $scope.showApproveModelFun = function (stakeholder, index) {
        debugger;
        $scope.Stakeholder = stakeholder;
        indexStake = index;
        $scope.showApproveModel = true;
    };

    $scope.showApproveAllModelFun = function () {
        $scope.showApproveAllModel = true;
    };
    //get stakeholder
    $scope.getStakeholder = function (stakeholder, index) {
        setIsApprovedFirst(stakeholder);
        indexStake = index;
        $scope.count = 1;
        // ReSharper disable UseOfImplicitGlobalInFunctionScope
        $scope.Stakeholder = angular.copy(stakeholder);
        // ReSharper restore UseOfImplicitGlobalInFunctionScope
        console.log($scope.Stakeholder);
        getHierarchy(stakeholder.Designation, stakeholder.Hierarchy);
    };

    //approve working
    $scope.approveWorking = function () {
        $scope.Stakeholder.IsWorkingChange = false;

        apiedit.customPOST($scope.Stakeholder, 'ApproveWorking').then(function (data) {
            $scope.listOfStakeholders[indexStake] = data;
            $scope.Stakeholder = data;
            $csnotify.success('Working Details Approved');
        }, function (data) {
            $csnotify.error(data.data.Message);
        });
    };

    //approve payment
    $scope.approvePayment = function () {
        $scope.Stakeholder.IsPaymentChange = false;
        apiedit.customPOST($scope.Stakeholder, 'ApprovePayment').then(function (data) {
            $scope.listOfStakeholders[indexStake] = data;
            $scope.Stakeholder = data;
            $csnotify.success('Payment Details Approved');
        }, function (data) {
            $csnotify.error(data.data.Message);
        });
    };

    //approve address
    $scope.approveAddress = function () {
        $scope.Stakeholder.IsAddressChange = false;
        apiedit.customPOST($scope.Stakeholder, 'ApproveAddress').then(function (data) {
            $scope.listOfStakeholders[indexStake] = data;
            $scope.Stakeholder = data;
            $csnotify.success('Address Details Approved');
        }, function (data) {
            $csnotify.error(data.data.Message);
        });
    };

    //reject working
    $scope.rejectWorkingDetails = function () {
        for (var i = 0; i < $scope.Stakeholder.StkhWorkings.length; i++) {
            $scope.Stakeholder.StkhWorkings[i].Description = $scope.RejectWorking;
        }
        $scope.Stakeholder.IsWorkingChange = false;
        apiedit.customPOST($scope.Stakeholder, 'RejectWorking').then(function (data) {
            $scope.listOfStakeholders[indexStake] = data;
            $scope.Stakeholder = data;
            $scope.RejectWorking = '';
            $csnotify.success('Working Details Rejected');
        }, function (data) {
            $csnotify.error(data.data.Message);
        });
    };

    //reject Payment
    $scope.rejectPaymentDetails = function () {
        $scope.Stakeholder.IsPaymentChange = false;

        for (var i = 0; i < $scope.Stakeholder.StkhPayments.length; i++) {
            $scope.Stakeholder.StkhPayments[i].Description = $scope.RejectPayment;
        }
        apiedit.customPOST($scope.Stakeholder, 'RejectPayment').then(function (data) {
            $scope.listOfStakeholders[indexStake] = data;
            $scope.Stakeholder = data;
            $scope.RejectPayment = '';
            $csnotify.success('Payment Details Rejected');
        }, function (data) {
            $csnotify.error(data.data.Message);
        });
    };

    //reject Address
    $scope.rejectAddressDetails = function () {
        for (var i = 0; i < $scope.Stakeholder.GAddress.length; i++) {
            $scope.Stakeholder.GAddress[i].Description = $scope.RejectAddress;
        }
        $scope.Stakeholder.IsAddressChange = false;

        apiedit.customPOST($scope.Stakeholder, 'RejectAddress').then(function (data) {
            $scope.listOfStakeholders[indexStake] = data;
            $scope.Stakeholder = data;
            $scope.RejectPayment = '';
            $csnotify.success('Address Details Rejected');
        }, function (data) {
            $csnotify.error(data.data.Message);
        });
    };

    //model popupoptions
    $scope.modelOption = {
        backdropFade: true,
        dialogFade: true
    };

    //close model function
    $scope.closeModel = function () {
        $scope.Stakeholder = {};
        $scope.count = 0;
        $scope.showRejectModel = false;
        $scope.isApprovedFirst = false;
    };

    //cancel stakeholder
    $scope.cancel = function () {
        $scope.closeModel();
    };

    //approve all stakeholders
    $scope.approveAll = function () {
        apiedit.customPOST($scope.listOfStakeholders, 'ApproveAllStakeholders')
            .then(function () {
                $scope.listOfStakeholders = null;
                $csnotify.success('All Stakeholders approved');
            }, function (data) {
                $csnotify.error(data.data.Message);
            });
    };

    //#endregion

    //#region private

    //get hierarchy object on base of designation and hierarchy
    var getHierarchy = function (designation, hierarchy) {
        apiedit.customGET('GetHierarchy', { designation: designation, hierarchy: hierarchy }).then(function (data) {
            $scope.Hierarchy = data;
        }, function (data) {
            $csnotify.error(data.data.Message);
        });
    };

    var setIsApprovedFirst = function (stakeholder) {
        if (stakeholder.ApprovedBy !== null) {
            $scope.isApprovedFirst = true;
        }
    };

    //#endregion

}])
);