csapp.factory("PermissionsDatalayer", ["$csnotify", "$csfactory", "Restangular", function ($csnotify, $csfactory, rest) {
    var dldata = {};

    var restApi = rest.all('PermissionApi');

    dldata.stakeHierarchy = [];

    var getPermission = function (id) {
        if (!$csfactory.isNullOrEmptyString(id)) {
            return restApi.customGET('GetPermission', { 'id': id }).then(function (data) {
                dldata.permission = data;
                return dldata.permission;
            });
        }
        return null;
    };

    var getStakeData = function () {
        restApi.customGET("GetStakeHierarchy").then(function (data) {
            dldata.Hierarchy = data;
        });
    };

    var savePerm = function (data) {
        return restApi.customPOST(data, 'SavePerm').then(function (updatedPerm) {
            $csnotify.success("Permisson Saved");
            return updatedPerm;
        });
    };

    return {
        dldata: dldata,
        SavePerm: savePerm,
        GetPermission: getPermission,
        GetStakeData: getStakeData
    };
}]);

csapp.controller("newPermissionsController", ['$scope', 'Restangular', 'PermissionsDatalayer', '$csModels',
    function ($scope, rest, datalayer, $csModels) {

        (function () {
            datalayer.GetStakeData();
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.Permission = $csModels.getColumns("Permission");
        })();

        $scope.getPermission = function (id) {
            datalayer.GetPermission(id).then(function (data) {
                $scope.currPermData = data;
            });
        };

        $scope.save = function (data, id) {
            data.Role = getRoleById(id, $scope.dldata.Hierarchy);
            setModuleAccess(data);
            datalayer.SavePerm(data).then(function (updatedData) {
                $scope.currPermData = updatedData;
            });
        };

        var getRoleById = function (id, listOfHierarchy) {
            var hierarachy = _.find(listOfHierarchy, function (hier) {
                return (hier.Id === id);
            });
            return hierarachy;
        };

        var setModuleAccess = function (root) {

            _.forEach(root.Childrens, function (module) {
                module.HasAccess = false;
                _.forEach(module.Childrens, function (activity) {
                    if (activity.HasAccess === true) module.HasAccess = true;
                    _.forEach(activity.Childrens, function (child) {
                        if (child.HasAccess === true) module.HasAccess = true;
                    });
                });
            });

        };

        $scope.uncheckChildren = function (obj) {
            obj.HasAccess = !obj.HasAccess;
            if (obj.HasAccess === false) {
                _.forEach(obj.Childrens, function (item) {
                    item.HasAccess = false;
                });
            }
        };

    }]);


