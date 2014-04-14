csapp.factory("PermissionsDatalayer", ["$csnotify", "$csfactory", "Restangular", "$permissionFactory", function ($csnotify, $csfactory, rest, permFactory) {
    var dldata = {};

    var restApi = rest.all('PermissionApi');


    var getPermission = function (id) {
        if ($csfactory.isNullOrEmptyString(id)) return;
        restApi.customGET('Get', { 'id': id }).then(function (data) {
            dldata.permission = JSON.parse(data.Permissions);
            dldata.Hierarchy = data;
            //$csnotify.success('Permissions Loaded');
        });
    };

    var saveNew = function (data) {
        dldata.Hierarchy.Permissions = JSON.stringify(data);
        restApi.customPOST(dldata.Hierarchy, 'Post').then(function (hierarchy) {
            dldata.Hierarchy = hierarchy;//update the hierarchy
            //update the hierarchy in the list of hierarchies
            _.forEach(dldata.stakeHierarchy, function (item) {
                if (item.Id === hierarchy.Id) {
                    item = hierarchy;
                    return;
                }
            });

            $csnotify.success('Permission Saved');
        });
    };

    var setPermissions = function (permission) {
        var perm = JSON.stringify(permission);
        restApi.customPOST({ 'json': perm }, 'SetPermission').then(function () {
            $csnotify.success('Permission Screen Initialized');
        });
    };

    var getAll = function () {
        restApi.customGET('Get').then(function (data) {
            dldata.stakeHierarchy = data;
            setUpdatesinPrem(dldata.stakeHierarchy);
        });

    };

    var setUpdatesinPrem = function (hierarchies) {
        _.forEach(hierarchies, function (item) {
            var perm = JSON.parse(item.Permissions);
            console.log(angular.copy(perm));
            updatePermission(perm, permFactory.permission);
            console.log(perm);
            return;
        });
    };
    
    var updatePermission = function (oldPermission, newPermission) {

        var oldStakePerm = _.find(oldPermission, function (item) {
            if (item.area === 'Stakeholder') return item;
        });
        var newStakePerm = _.find(newPermission, function (item) {
            if (item.area === 'Stakeholder') return item;
        });
        updateArea(oldStakePerm, newStakePerm);//updates stakeholder

        var newAllocPerm = _.find(oldPermission, function (item) {
            if (item.area === 'Allocation') return item;
        });
        var oldAllocPerm = _.find(oldPermission, function (item) {
            if (item.area === 'Allocation') return item;
        });
        updateArea(newAllocPerm, oldAllocPerm);//updates allocation

        var oldBillingPerm = _.find(oldPermission, function (item) {
            if (item.area === 'Billing') return item;
        });
        var newBillingPerm = _.find(oldPermission, function (item) {
            if (item.area === 'Billing') return item;
        });
        updateArea(oldBillingPerm, newBillingPerm);//updates billing

        var oldFileUploadPerm = _.find(oldPermission, function (item) {
            if (item.area === 'File Upload') return item;
        });
        var newFileUploadPerm = _.find(oldPermission, function (item) {
            if (item.area === 'File Upload') return item;
        });
        updateArea(oldFileUploadPerm, newFileUploadPerm);//updates file upload

    };

    var updateArea = function (oldPerm, newPerm) {
        var oldView = _.find(oldPerm.permissions, function (item) {
            if (item.category === 'view') return item;
        });
        var newView = _.find(newPerm.permissions, function (item) {
            if (item.category === 'view') return item;
        });
        updateExtraPerm(oldView, newView);

    };

    var updateExtraPerm = function (oldCategory, newCategory) {
        _.forEach(newCategory.extrapermission, function (newExtraPerm) {
            var perm = _.find(oldCategory.extrapermission, function(item) {
                if (item.display === newExtraPerm.display)
                    return item;
            });
            
            if (angular.isUndefined(perm))
                oldCategory.extrapermission.push(newExtraPerm);
        });
    };

    


    return {
        dldata: dldata,
        saveNew: saveNew,
        SetPermissions: setPermissions,
        GetPermission: getPermission,
        GetAll: getAll
    };
}]);


csapp.controller("newPermissionsController", ['$scope', '$permissionFactory', 'Restangular', 'PermissionsDatalayer',
    function ($scope, permissionsFactory, rest, datalayer) {

        (function () {
            $scope.dldata = datalayer.dldata;
            $scope.datalayer = datalayer;
            datalayer.SetPermissions(permissionsFactory.permission);
            datalayer.GetAll();

        })();

        $scope.save = function (data) {
            datalayer.saveNew(data);
        };

        $scope.uncheckExtraPerm = function (checked, extraPermission) {
            if (checked === false) {
                _.forEach(extraPermission, function (item) {
                    item.access = false;
                });
            } else return;
        };

    }]);