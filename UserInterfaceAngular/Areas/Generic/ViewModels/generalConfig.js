(
csapp.factory('configFactory', ['$http', '$csnotify', '$q', function ($http, $csnotify, $q) {

    //encrypt 
    var encryptData = function () {
        var deferred = $q.defer();

        $http({
            url: '/GeneralConfig/EncryptData',
            method: 'POST',
            data: {}
        }).success(function () {
            deferred.resolve();
            $csnotify.success('Data Encrypted');
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };

    //decrypt data
    var decryptdata = function () {
        var deferred = $q.defer();

        $http({
            url: '/GeneralConfig/DecryptData',
            method: 'POST',
            data: {}
        }).success(function () {
            deferred.resolve();
            $csnotify.success('Data Encrypted');
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };

    var sectionNames = function () {
        debugger;
        var deferred = $q.defer();

        $http({
            url: '/api/connectionapi/GetSectionsNames/',
            method: 'GET'
        }).success(function (data) {
            deferred.resolve(data);
            $csnotify.success('Section Names Loaded');
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };

    var encryptSection = function (sectionName) {
        debugger;
        var deferred = $q.defer();
        $http({
            url: '/api/connectionapi/EncryptSection/',
            method: 'GET',
            params: { sectionName: sectionName }
        }).success(function () {
            deferred.resolve();
            $csnotify.success('Section Encrypted');
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };

    var decryptSection = function (sectionName) {
        debugger;
        var deferred = $q.defer();
        $http({
            url: '/api/connectionapi/DecryptSection/',
            method: 'GET',
            params: { sectionName: sectionName }
        }).success(function () {
            deferred.resolve();
            $csnotify.success('Section Decrypted');
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };

    return {
        encryptData: encryptData,
        decryptdata: decryptdata,
        getSectionNames: sectionNames,
        encryptSection: encryptSection,
        decryptSection: decryptSection
    };


}])
);


(
csapp.controller('config', ['$scope', '$http', 'configFactory', '$csnotify', 'Restangular', function ($scope, $http, configFactory, $csnotify, rest) {
    'use strict';
    var apiconfig = rest.all('GeneralConfig');
    var apiconnection = rest.all('connectionapi');
    $scope.sectionName = '';

    $scope.encrypt = function () {
        apiconfig.customPOST('EncryptData');
        //configFactory.encryptData();
    };

    $scope.decrypt = function () {
        apiconfig.customPOST('DecryptData');
        //configFactory.decryptdata();
    };

    $scope.sections = [];
    apiconnection.customGET('GetSectionsNames').then(function (data) {
        $scope.sections = data;
    });

    $scope.encryptSection = function () {
        apiconnection.customGET('EncryptSection', { sectionName: $scope.sectionName }).then(function () {
            $csnotify.success('Section Encrypted');
        });
        //configFactory.encryptSection($scope.sectionName);
    };

    $scope.decryptSection = function () {
        apiconnection.customGET('DecryptSection', { sectionName: $scope.sectionName }).then(function () {
            $csnotify.success('Section Decrypted');
        });
        //configFactory.decryptSection($scope.sectionName);
    };
}])
);