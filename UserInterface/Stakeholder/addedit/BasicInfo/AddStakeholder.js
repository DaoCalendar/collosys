
csapp.factory("AddEditStakeholderDatalayer", ["$csfactory", "$csnotify", "Restangular", "$q",
    function ($csfactory, $csnotify, rest, $q) {

        var apistake = rest.all('StakeholderApi');

        var getPincode = function (pincode, level) {
            var params = { pincode: pincode, level: level };
            return apistake.customGET('GetPincodes', params)
                .then(
                    function (data) {
                        return data;
                    });
        };

        var getHierarchies = function () {
            return apistake.customGET('GetAllHierarchies').then(function (data) {
                return data;
            }, function () {
                $csnotify.error('Error loading hierarchies');
            });

        };

        var save = function (data) {
            return apistake.customPOST(data, 'SaveStake').then(function (afterPostData) {
                return afterPostData;
            });
        };

        var dldata = {};
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
            return apistake.customGET('GetReportsToData', { hierarchyId: hierarchyId, level: level }).then(function (data) {
                return data;
            });
        };

        var getStakeholderForEdit = function (id) {
            return apistake.customGET('GetStakeForEdit', { 'stakeholderId': id }).then(function (data) {
                return data;
            });
        };

        return {
            CheckUser: checkUser,
            GetHierarchies: getHierarchies,
            GetPincode: getPincode,
            GetReportsToList: getReportsToList,
            GetStakeholderForEdit: getStakeholderForEdit,
            Save: save
        };
    }]);

csapp.factory("AddEditStakeholderFactory", function () {

    var setHierarchyModel = function (hierarchy, model) {
        if (hierarchy.IsUser) {
            model.stakeholder.MobileNo.required = true;
            model.stakeholder.ExternalId.required = true;
            model.stakeholder.EmailId.required = true;
            model.stakeholder.EmailId.suffix = '@scb.com';
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

    //TODO: move to $csfactory

    var resetVal = function (key, except) {
        if (angular.isUndefined(except)) return true;
        var index = except.indexOf(key);
        return index === -1;
    };

    var resetObj = function (obj, except) {

        angular.forEach(obj, function (value, key) {
            if (resetVal(key, except)) {
                switch (typeof value) {
                    case 'object':
                    case 'boolean':
                    case 'number':
                    case 'string':
                        delete obj[key];
                        break;
                    default:
                        console.log('cannot reset type : ' + typeof value);
                }
            };
        });
    };

    var getValFromId = function (list, id, val) {
        if (angular.isUndefined(list)) return "";
        if (angular.isUndefined(id)) return "";
        if (angular.isUndefined(val)) return "";
        var data = _.find(list, { 'Id': id });
        return data[val];
    };

    return {
        SetHierarchyModel: setHierarchyModel,
        ResetObj: resetObj,
        GetValFromId: getValFromId
    };
});

csapp.controller("AddStakeHolderCtrl", ['$scope', '$log', '$csfactory', "$location", "$csModels", "AddEditStakeholderDatalayer", "AddEditStakeholderFactory", "$timeout", "$routeParams",
    function ($scope, $log, $csfactory, $location, $csModels, datalayer, factory, $timeout, $routeParams) {


        var getModels = function () {
            $scope.stakeholderModels = {
                stakeholder: $csModels.getColumns("Stakeholder"),
                address: $csModels.getColumns("StakeAddress"),
                registration: $csModels.getColumns("StkhRegistration")
            };
        };

        (function () {
            $scope.showForm = true;
            $scope.formMode = ($csfactory.isNullOrEmptyString($routeParams.stakeId)) ? 'add' : 'view';

            $scope.factory = factory;
            $scope.Stakeholder = {
                StkhAddress: [],
                StkhRegistrations: []
            };

            datalayer.GetHierarchies().then(function (data) {
                $scope.HierarchyList = data;
                $scope.hierarchyDisplayList = _.uniq(_.pluck($scope.HierarchyList, "Hierarchy"));
            }).then(function () {
                if (!$csfactory.isNullOrEmptyString($routeParams.stakeId)) {
                    datalayer.GetStakeholderForEdit($routeParams.stakeId).then(function (data) {
                        $scope.Stakeholder = data;
                        $scope.Stakeholder.Address = $csfactory.isNullOrEmptyArray(data.StkhAddress) ? "" : data.StkhAddress[0];
                        $scope.Stakeholder.Regis = $csfactory.isNullOrEmptyArray(data.StkhRegistrations) ? "" : data.StkhRegistrations[0];
                        $scope.selectedHierarchy = $scope.Stakeholder.Hierarchy;
                        $scope.Stakeholder.Hierarchy = angular.copy($scope.selectedHierarchy.Hierarchy);
                        $scope.changeInHierarchy($scope.selectedHierarchy.Hierarchy);
                        $scope.Stakeholder.Designation = angular.copy($scope.selectedHierarchy.Id);
                        $scope.assignSelectedHier($scope.selectedHierarchy.Id);
                    });
                }
            });

            getModels();

        })();

        $scope.reset = function () {
            factory.ResetObj($scope.Stakeholder);
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
            //getModels();
            $timeout(function () { $scope.showForm = true; }, 100);
        };

        $scope.changeInHierarchy = function (hierarchy) {
            $scope.showBasicInfo = false;
            var hierarchies = _.filter($scope.HierarchyList, function (item) {
                return (item.Hierarchy === hierarchy);
            });
            $scope.DesignationList = _.sortBy(hierarchies, 'PositionLevel');
        };

        $scope.assignSelectedHier = function (designation, form) {
            if (angular.isDefined(form)) {
                form.$setPristine();
            }

            $scope.selectedHierarchy = _.find($scope.HierarchyList, { 'Id': designation });
            factory.SetHierarchyModel($scope.selectedHierarchy, $scope.stakeholderModels);
            datalayer.GetReportsToList($scope.selectedHierarchy.Id, $scope.selectedHierarchy.ReportingLevel)
                .then(function (data) {
                    $scope.reportsToList = data;
                });

            if ($scope.formMode === 'add') {
                factory.ResetObj($scope.Stakeholder, ['Hierarchy', 'Designation']);
                $timeout(function () { $scope.showBasicInfo = true; }, 100);
            } else {
                $scope.showBasicInfo = true;
            }
        };

        $scope.saveData = function (data) {
            setStakeObject(data);
            datalayer.Save(data).then(function (savedStakeholder) {
                if ($scope.selectedHierarchy.HasWorking === true || $scope.selectedHierarchy.HasPayment === true) {
                    $scope.formMode === 'add' ? $location.path('/stakeholder/working/' + savedStakeholder.Id)
                        : $location.path('/stakeholder/working/edit/' + data.Id);
                } else {
                    $location.path('/stakeholder/view');
                }
            });
        };

        $scope.getTextForSaveBtn = function () {
            return ($scope.selectedHierarchy.HasWorking === true || $scope.selectedHierarchy.HasPayment === true) ?
                "Next" : "Save";
        };

        var setStakeObject = function (data) {
            //TODO: fix address/registration for edit
            data.StkhAddress = [];
            data.StkhRegistrations = [];
            data.Hierarchy = $scope.selectedHierarchy;//set hierarchy
            if (!$csfactory.isEmptyObject(data.Address)) data.StkhAddress.push(data.Address);
            if (!$csfactory.isEmptyObject(data.Regis)) data.StkhRegistrations.push(data.Regis);
        };


    }
]);
