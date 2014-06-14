
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
                _.forEach(data, function (item) {
                    if (item.Hierarchy !== 'External') return;
                    var reportTo = _.find(data, { 'Id': item.ReportsTo });
                    reportTo.Designation = item.Designation + '(' + reportTo.Designation + ')';
                });
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

        return {
            CheckUser: checkUser,
            GetHierarchies: getHierarchies,
            GetPincode: getPincode,
            GetReportsToList: getReportsToList,
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

    return {
        SetHierarchyModel: setHierarchyModel,
        ResetObj: resetObj
    };
});

csapp.controller("AddStakeHolderCtrl", ['$scope', '$log', '$csfactory', "$location", "$csModels", "AddEditStakeholderDatalayer", "AddEditStakeholderFactory", "$timeout",
    function ($scope, $log, $csfactory, $location, $csModels, datalayer, factory, $timeout) {

        (function () {
            $scope.factory = factory;
            $scope.Stakeholder = {
                GAddress: [],
                StkhRegistrations: []
            };

            $scope.stakeholderModels = {
                stakeholder: $csModels.getColumns("Stakeholder"),
                address: $csModels.getColumns("StakeAddress"),
                registration: $csModels.getColumns("StkhRegistration")
            };

            datalayer.GetHierarchies().then(function (data) {
                $scope.HierarchyList = data;
                $scope.hierarchyDisplayList = _.uniq(_.pluck($scope.HierarchyList, "Hierarchy"));
            });
        })();

        $scope.validate = function (userId) {
            return datalayer.CheckUser(userId);
        };

        $scope.changeInHierarchy = function (hierarchy) {
            $scope.showBasicInfo = false;
            var hierarchies = _.filter($scope.HierarchyList, function (item) {
                return (item.Hierarchy === hierarchy);
            });
            $scope.Designation = _.sortBy(hierarchies, 'PositionLevel');
        };

        $scope.assignSelectedHier = function (designation, form) {
            $scope.showBasicInfo = false;
            form.$setPristine();
            factory.ResetObj($scope.Stakeholder, ['Hierarchy', 'Designation']);
            $scope.selectedHierarchy = _.find($scope.HierarchyList, { 'Id': designation });

            datalayer.GetReportsToList($scope.selectedHierarchy.Id, $scope.selectedHierarchy.ReportingLevel)
                .then(function (data) {
                    $scope.reportsToList = data;
                });

            factory.SetHierarchyModel($scope.selectedHierarchy, $scope.stakeholderModels);
            $timeout(function () { $scope.showBasicInfo = true; }, 100);
        };

        $scope.saveData = function (data) {
            setStakeObject(data);
            datalayer.Save(data).then(function (savedStakeholder) {
                $location.path('/stakeholder/working/' + savedStakeholder.Id);
            });
        };

        var setStakeObject = function (data) {
            //TODO: fix address/registration for edit
            data.GAddress = [];
            data.Registration = [];
            data.Hierarchy = $scope.selectedHierarchy;//set hierarchy
            if (!$csfactory.isEmptyObject(data.Address)) data.GAddress.push(data.Address);
            if (!$csfactory.isEmptyObject(data.Regis)) data.StkhRegistrations.push(data.Regis);
        };

    }
]);
