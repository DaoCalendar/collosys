/// <reference path="../Scripts/angular.js" />

var testApp = angular.module("testApp", []);

testApp.controller("ModalDemoCtrl", function ($scope) {

    $scope.shouldBeOpen = false;

    $scope.open = function () {
        $scope.shouldBeOpen = true;
    };

    $scope.close = function () {
        $scope.closeMsg = 'I was closed at: ' + new Date();
        $scope.shouldBeOpen = false;
    };

    $scope.items = ['item1', 'item2'];

    $scope.opts = {
        backdropFade: true,
        dialogFade: true
    };

});