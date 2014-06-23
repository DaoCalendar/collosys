
csapp.factory("AddEditStakeholderDatalayer", ["$csfactory", "$csnotify", "Restangular", "$q",
    function ($csfactory, $csnotify, rest, $q) {

        var dldata = {};

        var apistake = rest.all('StakeholderApi');

        var getHierarchies = function () {
            return apistake.customGET('GetAllHierarchies').then(function (data) {
                return data;
            }, function () {
                $csnotify.error('Error loading hierarchies');
            });

        };

        var checkUser = function (userId) {
            var deferred = $q.defer();

            if ($csfactory.isNullOrEmptyString(userId) || userId.length !== 7 || dldata.isAlreadyCheckingUser === true) {
                deferred.resolve();
            } else {
                dldata.isAlreadyCheckingUser = true;
                apistake.customGET('CheckUserId', { id: userId })
                    .then(function (data) {
                        if (data === "true") {
                            deferred.reject();
                        } else {
                            deferred.resolve();
                        }
                    }).finally(function () {
                        dldata.isAlreadyCheckingUser = false;
                    });
            }

            return deferred.promise;
        };

        var getReportsToList = function (hierarchyId, level) {
            return apistake.customGET('GetReportsToList', { hierarchyId: hierarchyId, level: level }).then(function (data) {
                return data;
            });
        };

        var getStakeholder = function (id) {
            return apistake.customGET('GetStakeholder', { 'stakeholderId': id }).then(function (data) {
                return data;
            });
        };

        var save = function (data) {
            return apistake.customPOST(data, 'SaveStakeholder').then(function (afterPostData) {
                return afterPostData;
            });
        };

        var approveStakeholder = function (stakeObj) {
            return apistake.customPOST(stakeObj, 'ApproveStakeholder').then(function (data) {
                $csnotify.success('Stakeholder Approved');
                return data;
            });
        };

        var rejectStakeholder = function (stakeObj) {
            return apistake.customPOST(stakeObj, 'RejectStakeholder').then(function (data) {
                $csnotify.success('Stakeholder Rejected');
                return data;
            });
        };


        return {
            CheckUser: checkUser,
            GetHierarchies: getHierarchies,
            GetReportsToList: getReportsToList,
            GetStakeholder: getStakeholder,
            SaveStakeholder: save,
            RejectStakeholder: rejectStakeholder,
            ApproveStakeholder: approveStakeholder
        };
    }]);

csapp.factory("AddEditStakeholderFactory", function () {

    var setHierarchyModel = function (hierarchy, model) {
        if (hierarchy.IsUser) {
            model.stakeholder.MobileNo.required = true;
            model.stakeholder.ExternalId.required = true;
            model.stakeholder.EmailId.required = true;
            model.stakeholder.EmailId.suffix = '@sc.com';
        } else {
            model.stakeholder.MobileNo.required = false;
            model.stakeholder.ExternalId.required = false;
            model.stakeholder.EmailId.required = false;
            model.stakeholder.EmailId.suffix = undefined;
        }

        if (hierarchy.IsEmployee) {
            model.stakeholder.JoiningDate.label = "Date of Joining";
        } else {
            model.stakeholder.JoiningDate.label = "Date of Starting";
        }

        if (hierarchy.ManageReportsTo) {
            if (hierarchy.Hierarchy != 'External') {
                model.stakeholder.ReportingManager.label = "Line Manager";

            } else if (hierarchy.Hierarchy === 'External' && !(hierarchy.Designation == 'ExternalAgency' || hierarchy.Designation == 'ManpowerAgency')) {
                model.stakeholder.ReportingManager.label = "Agency Name";

            } else if (hierarchy.Designation == 'ExternalAgency' || hierarchy.Designation == 'ManpowerAgency') {
                model.stakeholder.ReportingManager.label = "Agency Supervisor";

            }
            model.stakeholder.ReportingManager.required = true;
        } else {
            model.label = 'Reporting Manager';
        }
    };

    return {
        ResetHierarchyModel: setHierarchyModel,
    };
});

//TODO: clean hierarchy designation variable name, why you complicated it so much
//TODO: can you please not make it simple
csapp.controller("AddStakeHolderCtrl", ['$scope', '$log', '$csfactory', "$location", "$csModels", "AddEditStakeholderDatalayer", "AddEditStakeholderFactory", "$timeout", "$routeParams",
    function ($scope, $log, $csfactory, $location, $csModels, datalayer, factory, $timeout, $routeParams) {

        var getModels = function () {
            $scope.stakeholderModels = {
                stakeholder: $csModels.getColumns("Stakeholder"),
                address: $csModels.getColumns("StakeAddress"),
                registration: $csModels.getColumns("StkhRegistration")
            };
            $scope.selectedHier = {};
            $scope.Stakeholder = {};
        };

        var initdata = function (data) {
            $scope.Stakeholder.Address = $csfactory.isNullOrEmptyArray(data.StkhAddress)
                ? {}
                : data.StkhAddress[0];
            $scope.Stakeholder.Regis = $csfactory.isNullOrEmptyArray(data.StkhRegistrations)
                ? {}
                : data.StkhRegistrations[0];
            $scope.selectedHierarchy = data.Hierarchy;

            //if edit mode
            $scope.selectedHier.Hierarchy = $scope.selectedHierarchy.Hierarchy;
            $scope.changeInHierarchy($scope.selectedHierarchy.Hierarchy);
            $scope.selectedHier.Designation = $scope.selectedHierarchy.Id;
            $scope.assignSelectedHier($scope.selectedHierarchy.Id);
        };

        var getHiearachy = function () {
            return datalayer.GetHierarchies().then(function (data) {
                $scope.HierarchyList = data;
                $scope.hierarchyDisplayList = _.uniq(_.pluck($scope.HierarchyList, "Hierarchy"));
                $scope.showForm = true;
            });
        };

        var getStakeholder = function () {
            if ($csfactory.isNullOrEmptyGuid($routeParams.stakeId)) return;
            datalayer.GetStakeholder($routeParams.stakeId).then(function (data) {
                $scope.Stakeholder = data;
                initdata(data);
            });
        };

        (function () {
            getModels();
            $scope.formMode = ($csfactory.isNullOrEmptyString($routeParams.stakeId)) ? 'add' : 'view';
            $scope.factory = factory;
            $scope.Stakeholder = { StkhAddress: [], StkhRegistrations: [] };
            getHiearachy().then(function () { getStakeholder(); });
        })();

        $scope.setApprovalStatus = function (id, status) {
            var stakeObj = { Id: id };
            switch (status) {
                case 'approve':
                    return datalayer.ApproveStakeholder(stakeObj).then(function (data) {
                        return $scope.Stakeholder = data;
                    });
                case 'reject':
                    return datalayer.RejectStakeholder(stakeObj).then(function (data) {
                        return $scope.Stakeholder = data;
                    });
                default:
                    throw "invalid approval status";
            }
        };

        $scope.reset = function () {
            $csfactory.ResetObject($scope.Stakeholder);
        };

        $scope.cancel = function () {
            $location.path('/stakeholder/view');
        };

        $scope.validate = function (userId) {
            return datalayer.CheckUser(userId);
        };

        $scope.changeMode = function () {
            $scope.showForm = false;
            $scope.formMode = 'edit';
            $timeout(function () { $scope.showForm = true; }, 100);
        };

        $scope.changeInHierarchy = function (hierarchy) {
            $scope.showBasicInfo = false;
            var hierarchies = _.filter($scope.HierarchyList, function (item) {
                return (item.Hierarchy === hierarchy);
            });
            $scope.DesignationList = _.sortBy(hierarchies, 'PositionLevel');
            console.log('DesignationList: ', $scope.DesignationList);
            console.log('HierarchyList: ', $scope.HierarchyList);
        };

        $scope.assignSelectedHier = function (designation, form) {
            $scope.showBasicInfo = false;
            if (angular.isDefined(form)) {
                form.$setPristine();
            }

            $scope.selectedHierarchy = _.find($scope.HierarchyList, { 'Id': designation });

            factory.ResetHierarchyModel($scope.selectedHierarchy, $scope.stakeholderModels);
            datalayer.GetReportsToList($scope.selectedHierarchy.Id, $scope.selectedHierarchy.ReportingLevel)
                .then(function (data) {
                    $scope.reportsToList = data;
                });

            if ($scope.formMode === 'add') {
                $csfactory.ResetObject($scope.Stakeholder, ['Hierarchy', 'Designation']);
            }
            $timeout(function () {
                console.log("timeout");
                $scope.showBasicInfo = true;
            }, 100);
        };

        $scope.saveData = function (data) {
            setStakeObject(data);
            return datalayer.SaveStakeholder(data).then(function (savedStakeholder) {
                if ($scope.selectedHierarchy.HasWorking === true || $scope.selectedHierarchy.HasPayment === true) {
                    $scope.formMode === 'add'
                        ? $location.path('/stakeholder/working/' + savedStakeholder.Id)
                        : $location.path('/stakeholder/working/edit/' + data.Id);
                } else {
                    $location.path('/stakeholder/view');
                }
            });
        };

        $scope.getTextForSaveBtn = function () {
            return ($scope.selectedHierarchy.HasWorking === true
                    || $scope.selectedHierarchy.HasPayment === true)
                ? "Next"
                : "Save";
        };

        var setStakeObject = function (data) {
            data.StkhAddress = [];
            data.StkhRegistrations = [];
            data.Hierarchy = $scope.selectedHierarchy;//set hierarchy
            if (!$csfactory.isEmptyObject(data.Address)) data.StkhAddress.push(data.Address);
            if (!$csfactory.isEmptyObject(data.Regis)) data.StkhRegistrations.push(data.Regis);
        };
    }
]);
