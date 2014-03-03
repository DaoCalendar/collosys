

csapp.factory('connectionFactory', ['$http', '$csnotify', '$q', function ($http, $csnotify, $q) {
    var url = 'api/connectionapi/';
    
    //all connectionstring list 
    var getAllConnectionStrings = function () {
        var deferred = $q.defer();
        $http({
            url: url + 'GetAllConnectionStrings',
            method: 'GET'
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //check connection string
    var checkConnectionString = function (value) {
        var deferred = $q.defer();

        $http({
            url: url + 'CheckConnection',
            method: 'POST',
            data: value
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //save data
    var save = function (value) {
        var deferred = $q.defer();

        $http({
            url: url + 'Save',
            method: 'POST',
            data: value
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };

    return {
        getAllConnectionStrings: getAllConnectionStrings,
        checkConnectionString: checkConnectionString,
        save: save
    };
}]);

csapp.controller('connection', ['$scope', '$http', 'connectionFactory', 'Restangular', function ($scope, $http, conFactory,rest) {
    'use strict';

    //#region data members

    var apiconnection = rest.all('connectionapi');

    $scope.listAllConnectionStrings = [];
    
    //object for Connectionstrin
    $scope.ConnectionStringData = {};

    //for show model
    $scope.showModel = false;
    
    //mode (add or edit)
    var mode = '';
    
    //index in local
    var index = -1;
    
    //#endregion
    
    //#region get
    apiconnection.customGET('GetAllConnectionStrings').then(function (data) {
        $scope.listAllConnectionStrings = data;
    });
  
    //#endregion
    
    //#region public
    
    //save connection string
    $scope.save = function () {
        apiconnection.customPOST($scope.ConnectionStringData, 'CheckConnection').then(function (data) {
            if (data) {
                savedata();
            } else {
                
            }
        });
    };
    
    //show model for edit
    $scope.showModelEdit = function (indexfrompage) {
        index = indexfrompage;
        $scope.showModel = true;
        mode = 'edit';
        $scope.ConnectionStringData = $scope.listAllConnectionStrings[index];
    };

    //show model for add
    $scope.showModelAdd = function () {
        mode = 'add';
        $scope.showModel = true;
    };
    
    //model popupoptions
    $scope.modelOption = {
        backdropFade: true,
        dialogFade: true
    };

    //close model function
    $scope.closeModel = function () {
        $scope.showModel = false;
        $scope.mode = '';
        $scope.Pass = '';
        $scope.ConnectionStringData = {};
    };
    
    //#endregion
    
    //#region private
    var afterSaveInEditMode = function(data) {
        $scope.listAllConnectionStrings[index] = data;
        $scope.ConnectionStringData = {};
        $scope.closeModel();
    };

    var afterSaveInAddMode = function (data) {
        $scope.listAllConnectionStrings.push(data);
        $scope.ConnectionStringData = {};
        $scope.closeModel();
    };

    var savedata = function () {
        apiconnection.customPOST($scope.ConnectionStringData, 'Save').then(function(data) {
            switch (mode) {
                case 'edit':
                    afterSaveInEditMode(data);
                    break;

                case 'add':
                    afterSaveInAddMode(data);
                    break;
            }
        });
    };
    //#endregion
}]);