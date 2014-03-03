/// <reference path="../../../Scripts/angular.js" />

var rlsLinerInfoApp = angular.module("rlsLinerInfoApp", ["ui.collosys"]);

rlsLinerInfoApp.controller("rlsLinerInfoCtrl", ["$scope", "$csnotify", "Restangular", function ($scope, $csnotify, rest) {

    var restApi = rest.all("RLSLinerApi");
    //var urlapp = "api/RLSLinerApi/";

    $scope.rlsLiners = [];
    
    //$http({
    //    url: urlapp+"Get",
    //    method:"GET"
    //}).success(function(data) {
    //    $scope.rlsLiners = data;
    //}).error(function() {
    //    $csnotify.error("RLS Liner is not comming from database. Please try again", "Error");
    //});
    
    restApi.customGET("Get").then(function (data) {
        $scope.rlsLiners = data;
    }, function (data) {
        $csnotify.error(data);
    });

}]);