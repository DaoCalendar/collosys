csapp.factory('stakeFactory', ['$http', '$csnotify', '$q', function ($http, $csnotify, $q) {

    //#region urls
    var urlStakeholder = 'api/stakeholderapi/';
    var url = 'api/showstakeholders/';
    var urlProduct = 'api/productapi/';
    var urlLocationList = 'api/location/';
    var urlVariableLiner = 'api/VariableLinerPolicyApi/';
    var urlVariableWriteoff = 'api/variablewriteoffpolicyapi/';
    var urlBucketList = 'api/BucketList/';
    var urlRegistration = 'api/Registration/';

    //#endregion

    //#region hierarchy

    var getAllHierarchy = function () {

        var deferred = $q.defer();

        $http({
            method: 'GET',
            url: urlStakeholder + 'Get'
        }).success(function (data) {
            deferred.resolve(data);
            //$scope.stakeHierarchy = data;
        }).error(function (data) {
            $csnotify.error(data.Message);
        });

        return deferred.promise;
    };

    var getCities = function (cityName) {

        var deferred = $q.defer();

        $http({
            method: 'GET',
            url:urlStakeholder + 'GetPincodes/',
            params: { pincode: cityName }
        }).success(function (data) {
            deferred.resolve(data);
            //$scope.stakeHierarchy = data;
        }).error(function (data) {
            $csnotify.error(data.Message);
        });
        return deferred.promise;
    };


    var getReportsTo = function (reportToInHierarchy) {
        
        var deferred = $q.defer();

        $http({
            method: 'GET',
            url: urlStakeholder + 'GetReportsToInHierarchy',
            params: { hierarchy: reportToInHierarchy }
        }).success(function (data) {
            deferred.resolve(data);
            // $scope.ReportsToList = data;
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };

    var getReportingList = function (reportToInHierarchy) {

        var deferred = $q.defer();

        $http({
            method: 'GET',
            url: urlStakeholder + 'GetReportingList',
            params: { reportsTo: reportToInHierarchy , hierarchy:reportToInHierarchy}
        }).success(function (data) {
            deferred.resolve(data);
            // $scope.ReportsToList = data;
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };


    //#endregion

    //#region stakeholder

    //#region page getters

    //get data from Gpincode table
    var getLocationList = function () {
        var deferred = $q.defer();
        $http({
            method: 'GET',
            url: urlLocationList + 'Get'
        }).success(function (data) {
            deferred.resolve(data);
            //$scope.LocationList = data;
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //get list of products
    var getproductList = function() {
        var deferred = $q.defer();
        $http({
            method: 'GET',
            url: urlProduct + 'Get'
        }).success(function (data) {
            deferred.resolve(data);
            //$scope.ProductList = data;
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //get list of variable liner policies
    var getVariableLinerPolicies = function () {
            var deferred = $q.defer();
            $http({
                method: 'GET',
                url: urlVariableLiner + 'Get'
            }).success(function (data) {
                deferred.resolve(data);
            }).error(function (status) {
                $csnotify.error(status.Message);
            });
            return deferred.promise;
    };
    
    //get list of variable writeoff policies
    var getVariableWriteoffPolicies = function () {
        var deferred = $q.defer();
        $http({
            method: 'GET',
            url: urlVariableWriteoff + 'Get'
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //get list of buckets
    var getBucketList = function () {
        var deferred = $q.defer();
        $http({
            method: 'GET',
            url: urlBucketList + 'Get'
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };

    //#endregion
    
    //#region registration getters
    
    //registration list 
    var getRegistrationNoList = function () {
        var deferred = $q.defer();
        $http({
            method: 'GET',
            url: urlRegistration + 'ListOfRegistrationNo'
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //Pan no list 
    var getPanNoList = function () {
        var deferred = $q.defer();
        $http({
            method: 'GET',
            url: urlRegistration + 'ListOfPanNo'
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //Tan no list 
    var getTanNoList = function () {
        var deferred = $q.defer();
        $http({
            method: 'GET',
            url: urlRegistration + 'ListOfTanNo'
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //ServiceTax no list 
    var getServiceTaxNoList = function () {
        var deferred = $q.defer();
        $http({
            method: 'GET',
            url: urlRegistration + 'ListOfServiceNo'
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };

    //UserID list 
    var getUserIdList = function () {
        var deferred = $q.defer();
        $http({
            method: 'GET',
            url: urlStakeholder + 'ListOfUserID'
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //#endregion
    
    //#region EditStakeholder getters
    
    //all stakeholders list 
    var getAllStakeholders = function () {
        var deferred = $q.defer();
        $http({
            method: 'GET',
            url: url + 'Get'
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //list for approve
    var getListForApprove = function () {
        var deferred = $q.defer();
        $http({
            method: 'GET',
            url: url + 'ListForApprove'
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //get hierarchy on designation and hierarchy 
    var getHierarchy = function (designation, hierarchy) {
        var deferred = $q.defer();
        $http({
            method: 'GET',
            url: 'api/LoadHierarchy/' + 'Get',
            params: { designation: designation, hierarchy: hierarchy }
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //get hierarchy on id
    var getHierarchyOnId = function (hierarchyId) {
        var deferred = $q.defer();
        $http({
            method: 'GET',
            url: 'api/LoadHierarchy/' + 'GetHierarchyWithId',
            params: { hierarchyId:hierarchyId }
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };

    //#endregion

    //#region all post calls

    //save stakeholder
    var saveStakeholder = function (value) {
        var deferred = $q.defer();

        $http({
            method: 'POST',
            url: urlStakeholder + 'Post',
            data: value
        }).success(function () {
            deferred.resolve();
            $csnotify.success('Data Saved');
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //save stakeholder in edit with save and next
    var saveStakeholderWithSaveAndNext = function (value) {
        var deferred = $q.defer();

        $http({
            method: 'POST',
            url: url + 'Post',
            data: value
        }).success(function (data) {
            deferred.resolve(data);
            $csnotify.success('Data Saved');
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //delete lists
    var deletelist = function (value) {
        var deferred = $q.defer();

        $http({
            method: 'POST',
            url: urlStakeholder + 'DeleteLists',
            data: value
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //save stakeholder in edit
    var saveStakeholderinEdit = function (value) {
        var deferred = $q.defer();

        $http({
            method: 'POST',
            url: url + 'Post',
            data: value
        }).success(function (data) {
            deferred.resolve(data);
            $csnotify.success('Data Saved');
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //approve stakeholder
    var approveStakeholder = function (value) {
        var deferred = $q.defer();

        $http({
            method: 'POST',
            url: url + 'SaveApprovedWithUser',
            data: value,
            async: false
        }).success(function (data) {
            deferred.resolve(data);
            //$csnotify.success('Stakeholder Approved');
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //reject stakeholder
    var rejectStakeholder = function (value) {
        var deferred = $q.defer();

        $http({
            method: 'POST',
            url: url + 'Post',
            data: value
        }).success(function (data) {
            deferred.resolve(data);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //approve individual 
    var approveIndividual = function (value, method, message) {
        var deferred = $q.defer();

        $http({
            method: 'POST',
            url: url + method,
            data: value
        }).success(function (data) {
            deferred.resolve(data);
            $csnotify.success(message);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    //reject individual
    var rejectIndividual = function (value, method, message) {
        var deferred = $q.defer();

        $http({
            method: 'POST',
            url: url + method,
            data: value
        }).success(function (data) {
            deferred.resolve(data);
            $csnotify.success(message);
        }).error(function (status) {
            $csnotify.error(status.Message);
        });
        return deferred.promise;
    };
    
    var approveAllStakeholders = function (value) {
        var deferred = $q.defer();

        $http({
            method: 'POST',
            url: url + 'ApproveAllStakeholders',
            data: value
        }).success(function (data) {
            deferred.resolve(data);
            $csnotify.success('All stakeholders approved');
        }).error(function (data, status, headers, config) {
            Error(status);
        });
        return deferred.promise;
    };
    //#endregion
    
    //#endregion

    return {
        getAllHierarchy: getAllHierarchy,
        getReportsTo: getReportsTo,
        saveStakeholder: saveStakeholder,
        getLocationList: getLocationList,
        getproductList: getproductList,
        getVariableLinerPolicies: getVariableLinerPolicies,
        getVariableWriteoffPolicies: getVariableWriteoffPolicies,
        getBucketList: getBucketList,
        getRegistrationNoList: getRegistrationNoList,
        getPanNoList: getPanNoList,
        getTanNoList: getTanNoList,
        getServiceTaxNoList: getServiceTaxNoList,
        getUserIdList: getUserIdList,
        getAllStakeholders: getAllStakeholders,
        getHierarchy: getHierarchy,
        saveStakeholderWithSaveAndNext: saveStakeholderWithSaveAndNext,
        deletelist: deletelist,
        saveStakeholderinEdit: saveStakeholderinEdit,
        approveStakeholder: approveStakeholder,
        rejectStakeholder: rejectStakeholder,
        approveIndividual: approveIndividual,
        rejectIndividual: rejectIndividual,
        getListForApprove: getListForApprove,
        approveAllStakeholders: approveAllStakeholders,
        getHierarchyOnId: getHierarchyOnId,
        getCities: getCities
    };
}]);