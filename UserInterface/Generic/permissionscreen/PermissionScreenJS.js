csapp.factory("Datalayer", ["$csnotify", "Restangular", function ($csnotify, rest) {
    var dldata = {};

    var restApi = rest.all('PermissionScreenApi');

    var getWholeData = function () {
        restApi.customGET('GetWholeData').then(function (data) {
            dldata.Activities = data.ActivityData;
            dldata.stakeHierarchy = data.HierarchyData;
            dldata.Permissions = data.PermissionData;
        });
    };



    return {
        dldata: dldata,
        GetWholeData: getWholeData
    };
}]);

csapp.factory("PermissionsFactory", ["$csfactory", function ($csfactory) {

    var setFinalPermission = function (vertical, activity, dldata) {
        console.log('setFinalPermission factory');
        var perm = _.where(dldata.Permissions, function (item) {
            if (!$csfactory.isEmptyObject(item.Role)) {
                if (item.Role.Hierarchy === vertical && item.Activity === activity)
                    return item;
            }
        });
        dldata.finalPermissionList = perm;
    };

    var getPermission = function (id, activity, dldata) {
        console.log('get permissions factory');
        dldata.eccalationDays = [];
        var perm = _.find(dldata.Permissions, function (par) {
            if (!$csfactory.isEmptyObject(par.Role)) {
                if (par.Activity == activity && par.Role.Id == id) {
                    return par;
                }
            }
        });
        if (!$csfactory.isEmptyObject(perm)) {
            return perm.Permission;
        }
    };

    var approvPositionlevel = function (hierarchy, permission, dldata) {
        if ($csfactory.isEmptyObject(hierarchy)) {
            return;
        }
        var maxposition = _.find(dldata.SortedHierarchyList, function (item) {
            return (item.Id === hierarchy.Id);
        });
        dldata.selectedposition = maxposition.PositionLevel;
    };

    var clearOnVertical = function (hierarchy, activity,selectPermission, dldata) {
        console.log('clearOnVertical factory');
        if ($csfactory.isNullOrEmptyGuid(hierarchy)) {
            return;
        }
        var list = [];
        dldata.SavePermissions = [];
        _.forEach(dldata.stakeHierarchy, function (item) {
            if (item.Hierarchy != hierarchy) {
                return [];
            }
            list.push(item);
        });
        dldata.SortedHierarchyList = _.sortBy(list, 'PositionLevel');
        selectPermission = new Array(dldata.SortedHierarchyList.length); //sets the array length
        approvPositionlevel();
        getPermission(hierarchy.Id,activity, dldata);
    };

    var changePermission = function(activity, permission, hierarchy, index, selectPermission, dldata) {

        if (permission !== 'Approve') {
            //set permission object
            var PermissionObj = {
                Activity: activity,
                Permission: permission,
                EscalationDays: 0,
                Role: hierarchy
            };
            setFinalPermissionsArray(activity, PermissionObj, hierarchy, dldata);
        }

        if (permission === 'Approve') {
            for (var j = 0; j < index; j++) {
                selectPermission[j] = 'Approve';
            }
            for (var i = dldata.SortedHierarchyList[0].PositionLevel; i <= hierarchy.PositionLevel; i++) {

                var newHierarchy = _.find(dldata.SortedHierarchyList, function(item) {
                    if (item.PositionLevel === i)
                        return item;
                });
                //set permission object
                PermissionObj = {
                    Activity: activity,
                    Permission: permission,
                    EscalationDays: '3',
                    Role: newHierarchy
                };
                setFinalPermissionsArray(activity,PermissionObj, newHierarchy, dldata);
            }
        }
    };

    var setFinalPermissionsArray = function (activity, permissionObj, hierarchy, dldata) {
        
        var index = dldata.finalPermissionList.indexOf(permissionObj);
        if (index === -1) {
            //finds the data with same Activity and Hierarchy
            var data = _.find(dldata.finalPermissionList, function (item) {
                if (item.Activity === activity && item.Role.Id === hierarchy.Id)
                    return item;
            });
            //deletes the 'data' 
            var index2 = dldata.finalPermissionList.indexOf(data);
            if (index2 !== -1) dldata.finalPermissionList.splice(index2, 1);
            //add the updated permission object
            dldata.finalPermissionList.push(angular.copy(permissionObj));
        }
    };

    var enableEscalation = function (hierarchy, activity, index, eccalationDays, dldata) {
        if (!$csfactory.isEmptyObject(hierarchy)) {
            var permissionObj = _.find(dldata.finalPermissionList, function (item) {
                if (!$csfactory.isEmptyObject(item.Role)) {
                    if (item.Role.Id === hierarchy.Id && item.Activity === activity)
                        return item;
                }
            });
            if (!$csfactory.isEmptyObject(permissionObj)) {
                if (permissionObj.Permission === 'Approve') {
                    dldata.eccalationDays[index] = permissionObj.EscalationDays;
                    eccalationDays[index] = permissionObj.EscalationDays;
                    return false;
                } else {
                    dldata.eccalationDays[index] = '';
                    eccalationDays[index] = '';
                    return true;
                }

            } else {
                dldata.eccalationDays[index] = '';
                eccalationDays[index] = '';
                return true;
            }
        }
    };
    
    var assignEscalationDay = function (noOfDays, hierarchy,dldata) {
        var permissionObj = _.find(dldata.finalPermissionList, function (item) {
            if (item.Role.Id === hierarchy.Id) {
                return item;
            }
        });
        if (!$csfactory.isEmptyObject(permissionObj))
            permissionObj.EscalationDays = noOfDays;
    };
    
    var enableSelect = function (positionLevel, selectPermission, index) {
        if (!$csfactory.isNullOrEmptyString(selectPermission[index + 1])) {
            if (selectPermission[index + 1] === 'Approve')
                return true;
            else return false;
        }
    };

    return {
        setFinalPermission: setFinalPermission,
        changePermission: changePermission,
        getPermission: getPermission,
        clearOnVertical: clearOnVertical,
        enableEscalation: enableEscalation,
        assignEscalationDay: assignEscalationDay,
        enableSelect:enableSelect
    };
}]);

csapp.controller("PermissionscreenCtrl",
    ["$scope", "$csfactory", "$csnotify", "Restangular", "permissionFactory", "$log", "Datalayer","PermissionsFactory",
        function ($scope, $csfactory, $csnotify, rest, permissionFactory, $log, datalayer,factory) {
            "use strict";

            //#region datalayer

            (function () {
                $scope.escDate = ['3', '4', '5',' 6', '7', '8', '9', '10'];
                $scope.eccalationDays = [];
                $scope.finalPermissionList = [];
                $scope.SelectPermission = [];
                $scope.permissionList = [];
                $scope.designationshow = false;
                $scope.Activities = [];
                $scope.stakeHierarchy = [];
                $scope.Hierarchy = [];
                $scope.SortedHierarchyList = [];
                $scope.modifyPermissions = [];
                $scope.FinalPermission = [];
                $scope.Permissions = [];

                $scope.selectedposition = '';

                $scope.PermissionObj = {
                    Id: "",
                    Activity: "",
                    Permission: "",
                    EscalationDays: "",
                    Role: {}
                };

                $scope.dldata = datalayer.dldata;
                $scope.factory = factory;
                datalayer.GetWholeData();

            })();

            $scope.enableSelect = function (positionLevel, selectedPosition, index) {
                if (!$csfactory.isNullOrEmptyString($scope.SelectPermission[index + 1])) {
                    if ($scope.SelectPermission[index + 1] === 'Approve')
                        return true;
                    else return false;
                }
            };

            $scope.SavePermission = function () {
                if ($scope.dldata.finalPermissionList.length === 0) {
                    return;
                }
                for (var i = 0; i < $scope.dldata.finalPermissionList.length; i++) {
                    var activity = $scope.dldata.finalPermissionList[i].Activity;
                    var id = $scope.dldata.finalPermissionList[i].Role.Id;
                    for (var j = 0; j < $scope.Permissions.length; j++) {
                        if ($scope.dldata.Permissions[j].Activity === activity && $scope.dldata.Permissions[j].Role.Id === id) {
                            var permObj = $scope.dldata.Permissions[j];
                            break;
                        }
                    }
                    if (!$csfactory.isEmptyObject(permObj)) {
                        $scope.dldata.finalPermissionList[i].Id = permObj.Id;
                        $scope.dldata.finalPermissionList[i].Version = permObj.Version;
                        permObj = {};
                    }
                }
                permApi.customPOST($scope.dldata.finalPermissionList, 'SavePermissions')
                    .then(function (data) {
                        $csnotify.success("Permission Saved");
                        $scope.Permissions = data;
                        //$scope.modifyPermissions = [];
                        //$scope.Hierarchy = [];
                    }, function () {
                        $csnotify.error("Not able to save the permissions");
                    });
            };

            //#endregion

            //#region hierarchy & initial data retrieval part

            $scope.enumPermission = ["NoAccess", "View", "Modify", "Approve"];

            $scope.clearOnVertical = function (hierarchy,activity,selectPermission,dldata) {
                
                factory.clearOnVertical(hierarchy, activity,selectPermission, dldata);
                $scope.designationshow = true;

                //if ($csfactory.isNullOrEmptyGuid(hierarchy)) {
                //    return;
                //}
                //var list = [];
                //$scope.SavePermissions = [];
                //_.forEach($scope.dldata.stakeHierarchy, function (item) {
                //    if (item.Hierarchy != hierarchy) {
                //        return [];
                //    }
                //    list.push(item);
                //});
                //$scope.SortedHierarchyList = _.sortBy(list, 'PositionLevel');
                //$scope.SelectPermission = new Array($scope.SortedHierarchyList.length); //sets the array length
                //$scope.approvPositionlevel();
                //$scope.getPermission(hierarchy.Id);
                //$scope.SelectPermission = new Array($scope.dldata.SortedHierarchyList.length);
            };

            $scope.changeActivity = function () {
                $scope.SelectedHier.Vertical = '';
                $scope.designationshow = false;
            };

            $scope.approvPositionlevel = function (hierarchy) {
                if ($csfactory.isEmptyObject(hierarchy)) {
                    return;
                }
                var maxposition = _.find($scope.SortedHierarchyList, function (item) {
                    return (item.Id === hierarchy.Id);
                });
                $scope.selectedposition = maxposition.PositionLevel;
            };
            //#endregion

            //#region display/save activity & permission

            $scope.getPermission = function (id, activity) {
                factory.getPermission(id, activity, $scope.dldata);
                //$scope.eccalationDays = [];
                //var perm = _.find($scope.dldata.Permissions, function (par) {
                //    if (!$csfactory.isEmptyObject(par.Role)) {
                //        if (par.Activity == activity && par.Role.Id == id) {
                //            return par;
                //        }
                //    }
                //});
                //if (!$csfactory.isEmptyObject(perm)) {
                //    return perm.Permission;
                //}
            };

            $scope.changePermission1 = function (activity, permission, hierarchy, index) {

                //factory.changePermission(activity, permission, hierarchy, index,$scope.dldata);

                if (permission !== 'Approve') {
                    //set permission object
                    $scope.PermissionObj = {
                        Activity: activity,
                        Permission: permission,
                        EscalationDays: 0,
                        Role: hierarchy
                    };
                    setFinalPermissionsArray(activity, $scope.PermissionObj, hierarchy);
                    //setFinalPermissionsArray(activity,$scope.PermissionObj, hierarchy,$scope.dldata);
                }

                if (permission === 'Approve') {
                    for (var j = 0; j < index; j++) {
                        $scope.SelectPermission[j] = 'Approve';
                    }
                    for (var i = $scope.SortedHierarchyList[0].PositionLevel; i <= hierarchy.PositionLevel; i++) {

                        var newHierarchy = _.find($scope.SortedHierarchyList, function (item) {
                            if (item.PositionLevel === i)
                                return item;
                        });
                        //set permission object
                        $scope.PermissionObj = {
                            Activity: activity,
                            Permission: permission,
                            EscalationDays: 3,
                            Role: newHierarchy
                        };
                        setFinalPermissionsArray($scope.PermissionObj, newHierarchy);
                        //setFinalPermissionsArray(activity,$scope.PermissionObj, hierarchy,$scope.dldata);
                    }
                }
            };

            $scope.setFinalPermission = function (vertical, activity) {

                //factory.setFinalPermission(vertical, activity, $scope.dldata);

                var permissions = _.where($scope.dldata.Permissions, function (item) {
                    if (!$csfactory.isEmptyObject(item.Role)) {
                        if (item.Role.Hierarchy === vertical && item.Activity === activity)
                            return item;
                    }
                });
                $scope.dldata.finalPermissionList = permissions;
            };

            $scope.enableEscalation = function (hierarchy, activity, index) {
               if (!$csfactory.isEmptyObject(hierarchy)) {
                    var permissionObj = _.find($scope.dldata.finalPermissionList, function (item) {
                        if (!$csfactory.isEmptyObject(item.Role)) {
                            if (item.Role.Id === hierarchy.Id && item.Activity === activity)
                                return item;
                        }
                    });
                    if (!$csfactory.isEmptyObject(permissionObj)) {
                        if (permissionObj.Permission === 'Approve') {
                            $scope.eccalationDays[index] = permissionObj.EscalationDays;
                            return false;
                        } else {
                            $scope.eccalationDays[index] = '';
                            return true;
                        }

                    } else {
                        $scope.eccalationDays[index] = '';
                        return true;
                    }
                }
            };

            $scope.assignEscalationDay = function (noOfDays, hierarchy) {
                var permissionObj = _.find($scope.dldata.finalPermissionList, function (item) {
                    if (item.Role.Id === hierarchy.Id) {
                        return item;
                    }
                });
                if (!$csfactory.isEmptyObject(permissionObj))
                    permissionObj.EscalationDays = noOfDays;
            };

            var setFinalPermissionsArray = function (activity, permissionObj, hierarchy) {
                console.log('setFinalPermissionArray controller');
                var index = $scope.dldata.finalPermissionList.indexOf(permissionObj);
                if (index === -1) {
                    //finds the data with same Activity and Hierarchy
                    var data = _.find($scope.dldata.finalPermissionList, function (item) {
                        if (item.Activity === activity && item.Role.Id === hierarchy.Id)
                            return item;
                    });
                    //deletes the 'data' 
                    var index2 = $scope.dldata.finalPermissionList.indexOf(data);
                    if (index2 !== -1) $scope.dldata.finalPermissionList.splice(index2, 1);
                    //add the updated permission object
                    $scope.dldata.finalPermissionList.push(angular.copy(permissionObj));
                }
            };

            //#endregion
            $scope.changePermission = function (activity, permission, hierarchyId) {
                $scope.Hierarchy = _.find($scope.SortedHierarchyList, { 'Id': hierarchyId });
                var index = $scope.SortedHierarchyList.indexOf($scope.Hierarchy);
                //$csnotify.success(index);
                if (index == null) {
                    return;
                }
                if (permission === 'Approve') {
                    for (var i = 0; i < index; i++) {
                        $scope.SelectPermission[i] = 'Approve';
                    }
                    $scope.selectedposition = $scope.Hierarchy.PositionLevel;
                    _.forEach($scope.SortedHierarchyList, function (item) {
                        var itemIndex = $scope.SortedHierarchyList.indexOf(item);
                        if (itemIndex > index) {
                            return;
                        }
                        $scope.FinalPermission.push(item);
                    });

                } else {
                    $scope.FinalPermission.push($scope.Hierarchy);
                    //$scope.SortedHierarchyList.splice(index,1);
                    $scope.approvPositionlevel($scope.Hierarchy, permission);
                    //$scope.SortedHierarchyList.push($scope.Hierarchy);
                    $scope.SortedHierarchyList = _.sortBy($scope.SortedHierarchyList, 'PositionLevel');
                }

                _.forEach($scope.FinalPermission, function (finalper) {
                    var perm = _.find($scope.Permissions, function (item) {
                        if (item.Activity === 'Development') {
                            return null;
                        }
                        return (item.Role.Id === finalper.Id && item.Activity === $scope.SelectedHier.Activity);
                    });
                    var newpermission;
                    if (perm) {
                        newpermission = JSON.parse(JSON.stringify(perm));
                        newpermission.Permission = permission;
                        newpermission.Index = $scope.modifyPermissions.length;
                    } else {
                        newpermission = {
                            Activity: activity,
                            Permission: permission,
                            Role: finalper,
                            Index: $scope.modifyPermissions.length
                        };
                    }
                    var permss = _.find($scope.modifyPermissions, function (per) {
                        if ($scope.modifyPermissions.length == 0) {
                            return [];
                        }
                        return (per.Role.Id === newpermission.Role.Id);
                    });

                    if (permss) {
                        newpermission.Index = permss.Index;
                        $scope.modifyPermissions[permss.Index] = newpermission;
                    } else {
                        $scope.modifyPermissions.push(newpermission);
                    }
                });
            };
        }]);

//#region "Factory"

csapp.factory('permissionFactory', ['Restangular', '$csfactory', '$csnotify', function (rest, $csfactory, $csnotify) {
    var restapi = rest.all('PermissionScreenApi');
    var getwholeData = function () {
        return restapi.customGET('GetWholeData');
    };

    return {
        GetwholeData: getwholeData
    };

}]);


//#endregion

//#region " Row Code"

//    var perm = _.find($scope.Permissions, function (per) {
//        return (per.Activity === activity && per.Role.Id === hierarchyId);
//    });
//    $scope.Hierarchy = _.find($scope.SortedHierarchyList, { 'Id': hierarchyId });
//    var newPermission;

//    if (perm) {
//        newPermission = JSON.parse(JSON.stringify(perm));
//        newPermission.Permission = permission;
//        newPermission.Index = $scope.modifyPermissions.length;
//    } else {
//        newPermission = {
//            Activity: activity,
//            Permission: permission,
//            Role: $scope.Hierarchy,
//            Index: $scope.modifyPermissions.length
//        };
//    }

//var permss = _.find($scope.modifyPermissions, function (per) {
//    return (per.Role.Id === newPermission.Role.Id);
//});

//if (permss) {
//    newPermission.Index = permss.Index;
//    $scope.modifyPermissions[permss.Index] = newPermission;
//} else {
//    $scope.modifyPermissions.push(newPermission);
//}


//$scope.changeDays = function (activity, escalationDays) {
//    debugger;
//    var permList = _.filter($scope.Permissions, function (per) {
//        return (per.Activity === activity);
//    });

//    var modifypermList = _.filter($scope.modifyPermissions, function (per) {
//        return (per.Activity === activity);
//    });

//    for (var pLength = permList.length; pLength < modifypermList.length; pLength++) {
//        permList.push(modifypermList[pLength]);
//    }

//    for (var i = 0; i < permList.length; i++) {
//        var newPermission = angular.copy(permList[i]);
//        newPermission.EscalationDays = escalationDays;
//        newPermission.Index = $scope.modifyPermissions.length;

//        var permss = _.find($scope.modifyPermissions, function (per) {
//            return (per.Activity === newPermission.Activity);
//        });

//        if (permss) {
//            newPermission.Index = permss.Index;
//            newPermission.Permission = permss.Permission;
//            $scope.modifyPermissions[permss.Index] = newPermission;
//        } else {
//            $scope.modifyPermissions.push(newPermission);
//        }
//    }
//};
//$scope.getEscalationDays = function (activity) {
//    var perm = _.find($scope.Permissions, { 'Activity': activity });
//    if (angular.isDefined(perm)) { return perm.EscalationDays; }
//    return 3;
//};

//var perlist = [];
//_.forEach($scope.SortedHierarchyList, function (hie) {
//    var item = _.find($scope.Permissions, function (permission) {
//        return (permission.Activity === $scope.SelectedHier.Activity && permission.Role.Id === hie.Id && permission.Permission === 'Approve');
//    });
//    if (item) {
//        perlist.push(item);
//    }
//});
// if (perlist.length != 0) {
//#endregion