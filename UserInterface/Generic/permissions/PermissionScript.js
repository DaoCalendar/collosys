csapp.factory("PermissionsDatalayer", ["$csnotify", "$csfactory", "Restangular", function ($csnotify, $csfactory, rest) {
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
            datalayer.GetAll();
            datalayer.SetPermissions(permissionsFactory.permission);
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