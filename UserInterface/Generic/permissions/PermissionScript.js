csapp.factory("PermissionsDatalayer", ["$csnotify", "$csfactory", "Restangular", "$permissionFactory", function ($csnotify, $csfactory, rest, permFactory) {
    var dldata = {};

    var restApi = rest.all('PermissionApi');

    dldata.stakeHierarchy = [];

    var getPermission = function (id) {
        if ($csfactory.isNullOrEmptyString(id)) return;
        return restApi.customGET('GetPermission', { 'id': id }).then(function (data) {
            dldata.permission = data;
            return dldata.permission;
        });
    };

    var getStakeData = function () {
        restApi.customGET("GetStakeHierarchy").then(function (data) {
            dldata.Hierarchy = data;
        });
    };


    var saveNew = function (data) {
        restApi.customPOST(data, 'Post').then(function (hierarchy) {
            $csnotify.success('Permission Saved');
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
        Save: saveNew,
        SavePerm: savePerm,
        GetPermission: getPermission,
        GetStakeData: getStakeData
    };
}]);

csapp.controller("newPermissionsController", ['$scope', '$permissionFactory', 'Restangular', 'PermissionsDatalayer', '$csModels',
    function ($scope, permissionsFactory, rest, datalayer, $csModels) {

        (function () {

            datalayer.GetStakeData();
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            $scope.Permission = $csModels.getColumns("Permission");
            $scope.prmsn = permissionsFactory.permission;

            console.log("dldata: ", $scope.dldata);
        })();

        $scope.getPermission = function (id) {
            datalayer.GetPermission(id).then(function (data) {
                $scope.currPermData = data;
                //var obj = JSON.parse(data);
                //console.log("permission: ", JSON.parse(obj));
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
                if (hier.Id === id) return hier;
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

        $scope.ticks = function (module) {
            var access = true;
            angular.forEach(module.childrens, function (activityVal, activityKey) {
                if (activityVal.access === false) {
                    console.log('return false', activityVal.access);
                    access = false;
                }
                else {
                    angular.forEach(activityVal.childrens, function (extraVal) {
                        if (extraVal.access === false) {
                            access = false;
                        }
                    });
                }
            });
            return access;
        };

        $scope.selectAll = function (selected, module) {

            selected = !selected;

            if (selected === true) {

                angular.forEach(module.childrens, function (activityVal, activitykey) {
                    activityVal.access = true;
                    angular.forEach(activityVal.childrens, function (extraVal, extraKey) {
                        extraVal.access = true;
                    });
                });

            } else if (selected === false) {
                angular.forEach(module.childrens, function (activityVal, activitykey) {
                    activityVal.access = false;
                    angular.forEach(activityVal.childrens, function (extraVal, extraKey) {
                        extraVal.access = false;
                    });
                });
            }

        };

        $scope.uncheckChildren = function (obj) {
            obj.HasAccess = !obj.HasAccess;
            if (obj.HasAccess === false) {
                //angular.forEach(obj.childrens, function (value, key) {
                //    value.access = false;
                //});
                _.forEach(obj.Childrens, function (item) {
                    item.HasAccess = false;
                });
            }
        };

    }]);



//var deleteOld = function (oldPermission, newPermission) {
//    angular.forEach(angular.copy(oldPermission), function (value, module) {
//        if (newPermission.hasOwnProperty(module)) {

//            var newModule = newPermission[module];
//            oldPermission[module].name = checkChange(oldPermission[module].name, newModule.name);//check change in name
//            oldPermission[module].description = checkChange(oldPermission[module].description, newModule.description);//check change in description

//            angular.forEach(oldPermission[module]['childrens'], function (activityValues, activity) {
//                if (newPermission[module]['childrens'].hasOwnProperty(activity)) {

//                    var newActivity = newPermission[module]['childrens'][activity];
//                    var oldActivity = oldPermission[module]['childrens'][activity];
//                    oldPermission[module]['childrens'][activity].name = checkChange(oldActivity.name, newActivity.name);
//                    oldPermission[module]['childrens'][activity].description = checkChange(oldActivity.description, newActivity.description);


//                    angular.forEach(oldPermission[module]['childrens'][activity]['childrens'], function (extravalues, extra) {
//                        if (newPermission[module]['childrens'][activity]['childrens'].hasOwnProperty(extra)) {

//                            var newExtra = newPermission[module]['childrens'][activity]['childrens'][extra];
//                            var oldExtra = oldPermission[module]['childrens'][activity]['childrens'][extra];
//                            oldPermission[module]['childrens'][activity]['childrens'][extra].name = checkChange(oldExtra.name, newExtra.name);
//                            oldPermission[module]['childrens'][activity]['childrens'][extra].description = checkChange(oldExtra.description, newExtra.description);

//                        }
//                        else {

//                            if (angular.isUndefined(oldPermission[module]['childrens'][activity]['childrens'][extra]))
//                                return;
//                            else {
//                                console.log('extraPerm deleted: ', oldPermission[module]['childrens'][activity]['childrens'][extra]);
//                                dldata.permissionsChanged = true;
//                                delete oldPermission[module]['childrens'][activity]['childrens'][extra];

//                            }

//                        }
//                    });
//                } else {

//                    if (angular.isUndefined(oldPermission[module]['childrens'][activity]))
//                        return;
//                    console.log('activity deleted: ', oldPermission[module]['childrens'][activity]);
//                    dldata.permissionsChanged = true;
//                    delete oldPermission[module]['childrens'][activity];

//                }
//            });
//        } else {

//            if (angular.isUndefined(oldPermission[module]))
//                return;
//            console.log('module deleted: ', oldPermission[module]);
//            dldata.permissionsChanged = true;
//            delete oldPermission[module];

//        }
//    });
//};

//var addNew = function (oldPermission, newPermission) {
//    angular.forEach(newPermission, function (value, module) {
//        if (!oldPermission.hasOwnProperty(module)) {
//            oldPermission[module] = value;
//            dldata.permissionsChanged = true;
//            console.log('module added: ', module);
//        } else {
//            angular.forEach(newPermission[module]['childrens'], function (activityVal, activity) {
//                if (!oldPermission[module]['childrens'].hasOwnProperty(activity)) {
//                    oldPermission[module]['childrens'][activity] = activityVal;
//                    dldata.permissionsChanged = true;
//                    console.log('activity added: ', activity);
//                } else {
//                    angular.forEach(newPermission[module]['childrens'][activity]['childrens'], function (extraVal, extra) {
//                        if (!oldPermission[module]['childrens'][activity]['childrens'].hasOwnProperty(extra)) {
//                            oldPermission[module]['childrens'][activity]['childrens'][extra] = extraVal;
//                            dldata.permissionsChanged = true;
//                            console.log('extraPerm added: ', extra);
//                        }
//                    });
//                }
//            });
//        }
//    });
//};
