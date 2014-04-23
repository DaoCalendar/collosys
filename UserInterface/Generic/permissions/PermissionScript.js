csapp.factory("PermissionsDatalayer", ["$csnotify", "$csfactory", "Restangular", "$permissionFactory", function ($csnotify, $csfactory, rest, permFactory) {
    var dldata = {};

    var restApi = rest.all('PermissionApi');

    var getPermission = function (id) {
        if ($csfactory.isNullOrEmptyString(id)) return;
        restApi.customGET('Get', { 'id': id }).then(function (data) {
            dldata.permission = JSON.parse(data.Permissions);
            dldata.Hierarchy = data;
        });
    };


    var setAccess = function (data) {
        angular.forEach(data, function (value, module) {
            var hasPerm = false;
            var currModule = data[module];
            console.log('current module: ', currModule);
            angular.forEach(currModule.childrens, function (activityVal, activity) {
                if (activity === 'access' && activityVal === true)
                    hasPerm = true;
                if (angular.isObject(activityVal)) {
                    angular.forEach(activityVal, function (extraPermVal, extraPermKey) {
                        if (extraPermKey === 'access' && extraPermVal === true)
                            hasPerm = true;
                    });
                }
            });
            currModule['access'] = hasPerm;
        });
    };


    var saveNew = function (data) {

        setAccess(data);

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
            getAll();
        });
    };

    var saveUpdates = function (hierarchies) {
        restApi.customPOST(hierarchies, 'SaveHierarchies').then(function (data) {
            dldata.stakeHierarchy = data;
            $csnotify.success('Permissions Updated');
        });
    };

    var getAll = function () {
        restApi.customGET('Get').then(function (data) {
            dldata.stakeHierarchy = data;
            setUpdatesinPrem(dldata.stakeHierarchy);
            if (dldata.permissionsChanged)//save updates only if there is any change in the permissions
                saveUpdates(dldata.stakeHierarchy);
        });

    };

    var setUpdatesinPrem = function (hierarchies) {
        dldata.permissionsChanged = false;
        _.forEach(hierarchies, function (item) {
            var perm = JSON.parse(item.Permissions);
            deleteOld(perm, permFactory.permission);
            addNew(perm, permFactory.permission);
            item.Permissions = JSON.stringify(perm);
        });
    };

    var deleteOld = function (oldPermission, newPermission) {
        angular.forEach(angular.copy(oldPermission), function (value, module) {
            if (newPermission.hasOwnProperty(module)) {
                var newModule = newPermission[module];
                angular.forEach(oldPermission[module]['childrens'], function (activityValues, activity) {
                    if (newPermission[module]['childrens'].hasOwnProperty(activity)) {
                        var newActivity = newPermission[module]['childrens'][activity];
                        angular.forEach(oldPermission[module]['childrens'][activity]['childrens'], function (extravalues, extra) {
                            if (newPermission[module]['childrens'][activity]['childrens'].hasOwnProperty(extra)) {
                                var newExtra = newPermission[module]['childrens'][activity]['childrens'][extra];
                            }
                            else {

                                if (angular.isUndefined(oldPermission[module]['childrens'][activity]['childrens'][extra]))
                                    return;
                                else {
                                    console.log('extraPerm deleted: ', oldPermission[module]['childrens'][activity]['childrens'][extra]);
                                    dldata.permissionsChanged = true;
                                    delete oldPermission[module]['childrens'][activity]['childrens'][extra];
                                }

                            }
                        });
                    } else {
                        console.log('activity deleted: ', oldPermission[module]['childrens'][activity]);
                        dldata.permissionsChanged = true;
                        delete oldPermission[module]['childrens'][activity];
                    }
                });
            } else {
                console.log('module deleted: ', oldPermission[module]);
                dldata.permissionsChanged = true;
                delete oldPermission[module];
            }
        });
    };

    var addNew = function (oldPermission, newPermission) {
        angular.forEach(newPermission, function (value, module) {
            if (!oldPermission.hasOwnProperty(module)) {
                oldPermission[module] = value;
                dldata.permissionsChanged = true;
                console.log('module added: ', module);
            } else {
                angular.forEach(newPermission[module]['childrens'], function (activityVal, activity) {
                    if (!oldPermission[module]['childrens'].hasOwnProperty(activity)) {
                        oldPermission[module]['childrens'][activity] = activityVal;
                        dldata.permissionsChanged = true;
                        console.log('activity added: ', activity);
                    } else {
                        angular.forEach(oldPermission[module]['childrens'][activity]['childrens'], function (extraVal, extra) {
                            if (!oldPermission[module]['childrens'][activity]['childrens'].hasOwnProperty(extra)) {
                                oldPermission[module]['childrens'][activity]['childrens'][extra] = extraVal;
                                dldata.permissionsChanged = true;
                                console.log('extraPerm added: ', extra);
                            }
                        });
                    }
                });
            }
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
            $scope.prmsn = permissionsFactory.permission;
        })();

        $scope.save = function (data) {
            datalayer.saveNew(data);
        };


        $scope.uncheckChildren = function (obj) {
            obj.access = !obj.access;
            if (obj.access === false) {
                angular.forEach(obj.childrens, function (value, key) {
                    value.access = false;
                });
            }
        };

    }]);





//var updatePermission = function (oldPermission, newPermission) {


//    _.forEach(oldPermission, function (perm) {
//        //gets the currently  referrenced area from the newPermission
//        var newPermArea = _.find(newPermission, function (item) {
//            if (item.area === perm.area)
//                return item;
//        });
//        if (angular.isUndefined(newPermArea)) return;

//        _.forEach(perm.permissions, function (category) {
//            //gets the currently  referrenced category from the newPermission
//            var newPermCategory = _.find(newPermArea.permissions, function (item) {
//                if (item.category === category.category)
//                    return item;
//            });
//            if (angular.isUndefined(newPermCategory)) return;

//            _.forEach(angular.copy(newPermCategory.extrapermission), function (extraPerm) {
//                //checks for new permission if any
//                var checkForNew = _.find(category.extrapermission, function (newExtraPerm) {
//                    if (newExtraPerm.display === extraPerm.display)
//                        return newExtraPerm;
//                });
//                if (angular.isUndefined(checkForNew)) {
//                    //add the extrapermission if it dosen't exist
//                    category.extrapermission.push(extraPerm);
//                    dldata.permissionsChanged = true;
//                }

//            });
//            _.forEach(angular.copy(category.extrapermission), function (oldExtraPerm) {
//                //checks for removed permission if any
//                var checkForRemoved = _.find(newPermCategory.extrapermission, function (newExtraPerm) {
//                    if (oldExtraPerm.display === newExtraPerm.display)
//                        return newExtraPerm;
//                });
//                //delete the extrapermission from oldPermission which dosen't exist in newPermission 
//                if (angular.isUndefined(checkForRemoved)) {
//                    var oldPermArray = _.pluck(category.extrapermission, 'display');
//                    var index = oldPermArray.indexOf(oldExtraPerm.display);
//                    category.extrapermission.splice(index, 1);
//                    dldata.permissionsChanged = true;
//                }
//            });
//        });

//    });
//};


//$scope.uncheckExtraPerm = function (checked, extraPermission) {
//    if (checked === false) {
//        _.forEach(extraPermission, function (item) {
//            item.access = false;
//        });
//    } else return;
//};



//var oldStakePerm = _.find(oldPermission, function (item) {
//    if (item.area === 'Stakeholder') return item;
//});
//var newStakePerm = _.find(newPermission, function (item) {
//    if (item.area === 'Stakeholder') return item;
//});
//updateArea(oldStakePerm, newStakePerm);//updates stakeholder

//var newAllocPerm = _.find(oldPermission, function (item) {
//    if (item.area === 'Allocation') return item;
//});
//var oldAllocPerm = _.find(oldPermission, function (item) {
//    if (item.area === 'Allocation') return item;
//});
//updateArea(newAllocPerm, oldAllocPerm);//updates allocation

//var oldBillingPerm = _.find(oldPermission, function (item) {
//    if (item.area === 'Billing') return item;
//});
//var newBillingPerm = _.find(oldPermission, function (item) {
//    if (item.area === 'Billing') return item;
//});
//updateArea(oldBillingPerm, newBillingPerm);//updates billing

//var oldFileUploadPerm = _.find(oldPermission, function (item) {
//    if (item.area === 'File Upload') return item;
//});
//var newFileUploadPerm = _.find(oldPermission, function (item) {
//    if (item.area === 'File Upload') return item;
//});
//updateArea(oldFileUploadPerm, newFileUploadPerm);//updates file upload

//var updateArea = function (oldPerm, newPerm) {
//    var oldView = _.find(oldPerm.permissions, function (item) {
//        if (item.category === 'view') return item;
//    });
//    var newView = _.find(newPerm.permissions, function (item) {
//        if (item.category === 'view') return item;
//    });
//    updateExtraPerm(oldView, newView);

//};

//var updateExtraPerm = function (oldCategory, newCategory) {
//    _.forEach(newCategory.extrapermission, function (newExtraPerm) {
//        var perm = _.find(oldCategory.extrapermission, function (item) {
//            if (item.display === newExtraPerm.display)
//                return item;
//        });

//        if (angular.isUndefined(perm))
//            oldCategory.extrapermission.push(newExtraPerm);
//    });
//};

