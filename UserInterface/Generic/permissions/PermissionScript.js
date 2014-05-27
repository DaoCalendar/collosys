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
        restApi.customPOST(data, 'Post').then(function (hierarchy) {
            $csnotify.success('Permission Saved');
        });
    };

    var savePerm = function (data) {
        return restApi.customPOST(data, 'SavePerm').then(function (data) {
            $csnotify.success("Permisson Saved");
            return data;
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
            if (!$csfactory.isEmptyObject(item.Permissions)) {
                var perm = JSON.parse(item.Permissions);
                deleteOld(perm, permFactory.permission);
                addNew(perm, permFactory.permission);
                item.Permissions = JSON.stringify(perm);
            }
        });
    };

    var checkChange = function (oldName, newName) {
        if (oldName !== newName) {
            oldName = newName;
            dldata.permissionsChanged = true;
            console.log('changed from: ', oldName, 'to: ', newName);
        }

        return oldName;
    };

    var deleteOld = function (oldPermission, newPermission) {
        angular.forEach(angular.copy(oldPermission), function (value, module) {
            if (newPermission.hasOwnProperty(module)) {

                var newModule = newPermission[module];
                oldPermission[module].name = checkChange(oldPermission[module].name, newModule.name);//check change in name
                oldPermission[module].description = checkChange(oldPermission[module].description, newModule.description);//check change in description

                angular.forEach(oldPermission[module]['childrens'], function (activityValues, activity) {
                    if (newPermission[module]['childrens'].hasOwnProperty(activity)) {

                        var newActivity = newPermission[module]['childrens'][activity];
                        var oldActivity = oldPermission[module]['childrens'][activity];
                        oldPermission[module]['childrens'][activity].name = checkChange(oldActivity.name, newActivity.name);
                        oldPermission[module]['childrens'][activity].description = checkChange(oldActivity.description, newActivity.description);


                        angular.forEach(oldPermission[module]['childrens'][activity]['childrens'], function (extravalues, extra) {
                            if (newPermission[module]['childrens'][activity]['childrens'].hasOwnProperty(extra)) {

                                var newExtra = newPermission[module]['childrens'][activity]['childrens'][extra];
                                var oldExtra = oldPermission[module]['childrens'][activity]['childrens'][extra];
                                oldPermission[module]['childrens'][activity]['childrens'][extra].name = checkChange(oldExtra.name, newExtra.name);
                                oldPermission[module]['childrens'][activity]['childrens'][extra].description = checkChange(oldExtra.description, newExtra.description);

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

                        if (angular.isUndefined(oldPermission[module]['childrens'][activity]))
                            return;
                        console.log('activity deleted: ', oldPermission[module]['childrens'][activity]);
                        dldata.permissionsChanged = true;
                        delete oldPermission[module]['childrens'][activity];

                    }
                });
            } else {

                if (angular.isUndefined(oldPermission[module]))
                    return;
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
                        angular.forEach(newPermission[module]['childrens'][activity]['childrens'], function (extraVal, extra) {
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
        Save: saveNew,
        SavePerm: savePerm,
        SetPermissions: setPermissions,
        GetPermission: getPermission,
        GetAll: getAll,
        GetStakeData: getStakeData
    };
}]);

csapp.controller("newPermissionsController", ['$scope', '$permissionFactory', 'Restangular', 'PermissionsDatalayer', '$csModels',
    function ($scope, permissionsFactory, rest, datalayer, $csModels) {

        (function () {

            datalayer.GetStakeData();
            $scope.datalayer = datalayer;
            $scope.dldata = datalayer.dldata;
            datalayer.SetPermissions(permissionsFactory.permission);
            $scope.Permission = $csModels.getColumns("Permission");
            $scope.prmsn = permissionsFactory.permission;

            console.log("dldata: ", $scope.dldata);
        })();

        $scope.save = function (data) {
            datalayer.saveNew(data);
        };

        $scope.getPermission = function (id) {
            datalayer.GetPermission(id).then(function (data) {
                $scope.displayData = data[0].Childrens;
                $scope.currPermData = data[0];
                console.log("permission: ", $scope.currPermData);
            });
        };

        $scope.save = function (data) {
            debugger;
            $scope.currPermData.Childrens = data;
            $scope.currPermData.Role = getRoleById($scope.perm.designation, $scope.dldata.Hierarchy);
            datalayer.SavePerm($scope.currPermData).then(function (updatedData) {
                $scope.currPermData = updatedData;
            });
        };

        var setParent = function (permData) {
            _.forEach(permData.Childrens, function (activity) {
                var activityParent = activity;
                activity.Parent = permData;
                _.forEach(activity.Childrens, function (subActivity) {
                    var subActivityParent = subActivity;
                    subActivity.Parent = activityParent;
                    _.forEach(subActivity.Childrens, function (child) {
                        child.Parent = subActivityParent;
                    });
                });
            });
        };


        var getRoleById = function (id, listOfHierarchy) {
            var hierarachy = _.find(listOfHierarchy, function (hier) {
                if (hier.Id === id) return hier;
            });
            return hierarachy;
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



//setAccess(data);

//dldata.Hierarchy.Permissions = JSON.stringify(data);
//update the hierarchy
//update the hierarchy in the list of hierarchies
//_.forEach(dldata.stakeHierarchy, function (item) {
//    if (item.Id === hierarchy.Id) {
//        item = hierarchy;
//        return;
//    }
//});




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

