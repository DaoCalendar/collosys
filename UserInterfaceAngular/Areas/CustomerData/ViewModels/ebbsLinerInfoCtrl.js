/// <reference path="../../../Scripts/angular.js" />


var rlsLinerInfoApp = angular.module("ebbsLinerInfoApp", ["ui.collosys"]);

rlsLinerInfoApp.controller("ebbsLinerInfoCtrl", ["$scope", "$csnotify", "Restangular", function ($scope, $csnotify, rest) {

    var restApi = rest.all("EBBSLinerApi");

    //var urlapp = "api/EBBSLinerApi/";

    $scope.ebbsLiners = [];

    //$http({
    //    url: urlapp+"Get",
    //    method:"GET"
    //}).success(function(data) {
    //    $scope.ebbsLiners = data;
    //}).error(function() {
    //    $csnotify.error("EBBS Liner is not comming from database. Please try again", "Error");
    //});

    restApi.customGET("Get").then(function (data) {
        $scope.ebbsLiners = data;
    }, function (data) {
        $csnotify.error(data);
    });

}]);