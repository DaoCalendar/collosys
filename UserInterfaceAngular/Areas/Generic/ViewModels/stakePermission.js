/*global csapp*/
(

csapp.controller("stkPermissionCtrl",
    ["$scope", "$csfactory", "$csnotify", "Restangular",
        function ($scope, $csfactory, $csnotify, rest) {
            "use strict";

            //#region datalayer

            var permApi = rest.all('PermissionApi');

            $scope.GetHierarchies = function () {
                permApi.customGET('HierarchyList')
                    .then(function (data) {
                        $scope.stakeHierarchy = data;
                    }, function () {
                        $csnotify.error("Not able to retrieve verticals related information.");
                    });
            };

            $scope.GetActivities = function () {
                permApi.customGET('ActivityList')
                    .then(function (data) {
                        $scope.Activities = data;
                    }, function () {
                        $csnotify.error("Not able to retrieve ColloSys Activities information.");
                    });
            };

            $scope.GetPermissions = function (id) {
                permApi.customGET('PermissionList', { 'hierarchyId': id })
                    .then(function (data) {
                        $scope.Permissions = data;
                        $scope.GetActivities();
                    }, function () {
                        $csnotify.error("Not able to retrieve Hierarchy permission information.");
                    });
            };

            $scope.SavePermissions = function () {
                debugger;

                if (($scope.modifyPermissions === null) || !angular.isArray($scope.modifyPermissions) || $scope.modifyPermissions.length === 0) {
                    return;
                }

                var newData = $scope.modifyPermissions;
                for (var i = 0; i < newData.length; i++) {
                    newData[i].Role = angular.copy($scope.Hierarchy);
                }

                permApi.customPOST(newData, 'SavePermissions')
                    .then(function (data) {
                        $csnotify.success("Permission Saved");
                        $scope.Permissions = data;
                        $scope.modifyPermissions = [];
                    }, function () {
                        $csnotify.error("Not able to save the permissions");
                    });
            };

            //#endregion

            //#region hierarchy & initial data retrieval part

            $scope.enumPermission = ["NoAccess", "View", "Modify", "Approve"];
            $scope.GetHierarchies();

            $scope.clearOnVertical = function () {
                $scope.SelectedHier.HierarchyId = null;
                $scope.Hierarchy = null;
                $scope.modifyPermissions = [];
                $scope.Permissions = [];
                $scope.Activities = [];
            };

            $scope.changeHierarchy = function (selectedHier) {
                debugger;
                $scope.modifyPermissions = [];
                $scope.Permissions = [];
                $scope.Activities = [];
                if ($csfactory.isNullOrEmptyGuid(selectedHier.HierarchyId))
                    return;
                $scope.Hierarchy = _.find($scope.stakeHierarchy, { 'Id': selectedHier.HierarchyId });
                $scope.GetPermissions(selectedHier.HierarchyId);
            };

            //#endregion

            //#region display/save activity & permission

            $scope.getPermission = function (activity) {
                var perm = _.find($scope.Permissions, { 'Activity': activity });
                if (angular.isDefined(perm)) { return perm.Permission; }
                return $scope.enumPermission[0];
            };

            $scope.getEscalationDays = function (activity) {
                var perm = _.find($scope.Permissions, { 'Activity': activity });
                if (angular.isDefined(perm)) { return perm.EscalationDays; }
                return 3;
            };

            $scope.changeDays = function (activity, escalationDays) {
                debugger;
                var permList = _.filter($scope.Permissions, function (per) {
                    return (per.Activity === activity);
                });

                var modifypermList = _.filter($scope.modifyPermissions, function (per) {
                    return (per.Activity === activity);
                });

                for (var pLength = permList.length; pLength < modifypermList.length; pLength++) {
                    permList.push(modifypermList[pLength]);
                }

                for (var i = 0; i < permList.length; i++) {
                    var newPermission = angular.copy(permList[i]);
                    newPermission.EscalationDays = escalationDays;
                    newPermission.Index = $scope.modifyPermissions.length;

                    var permss = _.find($scope.modifyPermissions, function (per) {
                        return (per.Activity === newPermission.Activity);
                    });

                    if (permss) {
                        newPermission.Index = permss.Index;
                        newPermission.Permission = permss.Permission;
                        $scope.modifyPermissions[permss.Index] = newPermission;
                    } else {
                        $scope.modifyPermissions.push(newPermission);
                    }
                }
            };

            $scope.changePermission = function (activity, permission, escalationDays) {

                var perm = _.find($scope.Permissions, function (per) {
                    return (per.Activity === activity);
                });

                var newPermission;

                if (perm) {
                    newPermission = JSON.parse(JSON.stringify(perm));
                    newPermission.Permission = permission;
                    newPermission.EscalationDays = escalationDays;
                    newPermission.Index = $scope.modifyPermissions.length;
                } else {
                    newPermission = {
                        Activity: activity,
                        Permission: permission,
                        EscalationDays: escalationDays,
                        Role: $scope.Hierarchy,
                        Index: $scope.modifyPermissions.length
                    };
                }

                var permss = _.find($scope.modifyPermissions, function (per) {
                    return (per.Activity === newPermission.Activity);
                });

                if (permss) {
                    newPermission.Index = permss.Index;
                    $scope.modifyPermissions[permss.Index] = newPermission;
                } else {
                    $scope.modifyPermissions.push(newPermission);
                }
            };

            //#endregion

        }])
);