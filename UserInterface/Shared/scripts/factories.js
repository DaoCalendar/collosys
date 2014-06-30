
csapp.factory("$cslocation", [
    '$location', function () {
        var getUrl = function (path, params) {
            switch (path) {
                case "stakeholder.add":
                    return "/stakeholder/add";
                case "stakeholder.edit":
                    return "/stakeholder/edit/" + params;
                default:
                    throw 'invalid path';
            }
        };

        var persistData = {};
        var getSharedData = function () {
            return persistData;
        };

        var changePath = function (url, params) {
            persistData = params;
            $location.path(url);
        };

        return {
            GetUrl: getUrl,
            Change: changePath,
            GetData: getSharedData
        };
    }
]);

csapp.factory("$csfactory", ["$csConstants", "$csAuthFactory", "loadingWidget",
    function (consts, auth, loadingWidget) {

        var enableSpinner = function () {
            loadingWidget.params.enableSpinner = true;
        };

        var downloadFile = function (filename) {
            enableSpinner();
            var ifr = document.createElement('iframe');
            ifr.style.display = 'none';
            document.body.appendChild(ifr);
            ifr.src = baseUrl + "api/GridApi/DownloadFile?filename='" + escape(filename) + "'";
            ifr.onload = function () {
                document.body.removeChild(ifr);
                ifr = null;
            };
        };

        var safeSplice = function (list, row, properties) {
            var indx = list.indexOf(row);
            if (indx !== -1) {
                list.splice(indx, 1);
                return;
            }

            if ($csfactory.isNullOrEmptyString(properties)) {
                return;
            }

            var record = _.find(list, comparisons(row, properties));
            var index = list.indexOf(record);
            if (index !== -1) list.splice(index, 1);
        };
        var comparisons = function (row, properties) {
            var comparison = {};
            _.forEach(properties, function (property) {
                comparison[property] = row[property];
            });
            return comparison;
        };

        var isNullOrEmptyString = function (val) {

            if (angular.isUndefined(val)) {
                return true;
            }

            if (val === null) {
                return true;
            }

            if (val === "") {
                return true;
            }

            if (val === 'null') {
                return true;
            }

            return false;
        };

        var isEmptyObject = function (obj) {
            var name;
            // ReSharper disable AssignedValueIsNeverUsed
            for (name in obj) {
                return false;
            }
            // ReSharper restore AssignedValueIsNeverUsed
            return true;
        };

        var doResetVal = function (key, except) {
            if (angular.isUndefined(except)) return true;
            var index = except.indexOf(key);
            return index === -1;
        };

        var resetObject = function (obj, except) {

            angular.forEach(obj, function (value, key) {
                if (doResetVal(key, except)) {
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

        var isNullOrEmptyArray = function (val) {
            if (isNullOrEmptyString(val)) {
                return true;
            }

            if (angular.isArray(val) && (val.length === 0)) {
                return true;
            }

            return false;
        };

        var isNullOrEmptyGuid = function (val) {
            if (isNullOrEmptyString(val)) {
                return true;
            }

            if (val === consts.GUID_EMPTY) {
                return true;
            }

            return false;
        };

        var getDefaultGuid = function () {
            return consts.GUID_EMPTY;
        };

        var getCurrentUserName = function () {
            return auth.getUsername();
        };

        var findIndex = function (array, field, value) {
            var index = _.indexOf(_.pluck(array, field), value);
            return index;
        };

        var getPropertyValue = function (targetObj, keyPath) {
            var keys = keyPath.split('.');
            if (keys.length === 0) return undefined;
            keys = keys.reverse();
            var subObject = targetObj;
            while (keys.length) {
                var k = keys.pop();
                if (!subObject.hasOwnProperty(k)) {
                    return undefined;
                } else {
                    subObject = subObject[k];
                }
            }
            return subObject;
        };

        var getPropValFromListById = function (list, id, val) {
            if (angular.isUndefined(list)) return undefined;
            if (angular.isUndefined(id)) return undefined;
            if (angular.isUndefined(val)) return undefined;
            var data = _.find(list, { 'Id': id });
            return data[val];
        };

        return {
            isNullOrEmptyArray: isNullOrEmptyArray,
            isNullOrEmptyString: isNullOrEmptyString,
            isNullOrEmptyGuid: isNullOrEmptyGuid,
            getDefaultGuid: getDefaultGuid,
            isEmptyObject: isEmptyObject,
            downloadFile: downloadFile,
            getCurrentUserName: getCurrentUserName,
            enableSpinner: enableSpinner,
            findIndex: findIndex,
            getPropertyValue: getPropertyValue,
            ResetObject: resetObject,
            GetPropertyById: getPropValFromListById,
            Splice: safeSplice
        };
    }]);

csapp.factory('$csnotify', function () {

    var success = function (messege) {
        $.notify(messege, "success");
    };

    var log = function (messege) {
        $.notify(messege, "info");
    };

    var error = function (messege) {
        $.notify(messege, "error");
    };

    return {
        success: success,
        error: error,
        log: log
    };
});

csapp.factory('$csStopWatch', ["$timeout", function ($timeout) {
    var data = { value: 1, stringValue: '' };
    var stopwatch = null;

    var start = function () {
        stopwatch = $timeout(function () {
            data.value++;
            data.stringValue = getTime(data.value);
            start();
        }, 1000);
    };

    var stop = function () {
        $timeout.cancel(stopwatch);
        stopwatch = null;
    };

    var reset = function () {
        stop();
        data.value = 0;
        data.stringValue = '';
        //data.laps = [];
    };

    //var lap = function () {
    //    data.laps.push(data.value);
    //};

    var getTime = function (totalTime) {

        var secs = totalTime % 60;
        var mins = (totalTime - secs) / 60;

        if (mins === 0 && secs === 0) {
            return 'timer just started.';
        }

        var stringTime = '';
        if (mins === 1) {
            stringTime = 1 + " minute";
        } else if (mins >= 1) {
            stringTime = mins + " minutes";
        }

        if (secs === 0) {
            return stringTime;
        }
        if (mins !== 0) {
            stringTime = stringTime + ", ";
        }
        if (secs === 1) {
            stringTime = stringTime + 1 + " second";
        } else {
            stringTime = stringTime + secs + " seconds";
        }

        return stringTime;
    };

    return {
        data: data,
        start: start,
        stop: stop,
        reset: reset,
        //lap: lap
    };
}]);

csapp.filter('bytes', function () {
    return function (bytes, precision) {
        if (bytes == 0 || isNaN(parseFloat(bytes)) || !isFinite(bytes)) return '-';
        if (typeof precision === 'undefined') precision = 1;
        var units = ['bytes', 'KB', 'MB', 'GB', 'TB', 'PB'],
            number = Math.floor(Math.log(bytes) / Math.log(1024));
        return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) + ' ' + units[number];
    };
});

csapp.filter('fromNow', function () {
    return function (dateString) {
        return moment(dateString).fromNow();
    };
});

csapp.provider('Logger', [function () {
    var isEnabled = true;
    this.enabled = function (wasEnabled) {
        isEnabled = !!wasEnabled;
    };
    this.$get = ['$log', function ($log) {
        // ReSharper disable InconsistentNaming
        var Logger = function (context) {
            this.context = context;
        };
        // ReSharper restore InconsistentNaming
        Logger.getInstance = function (context) {
            return new Logger(context);
        };
        Logger.supplant = function (str, o) {
            return str.replace(
                /\{([^{}]*)\}/g,
                function (a, b) {
                    var r = o[b];
                    return typeof r === 'string' || typeof r === 'number' ? r : a;
                }
            );
        };
        Logger.getFormattedTimestamp = function () {
            return moment().format("HH:mm:ss-SSS");
        };
        Logger.prototype = {
            _log: function (originalFn, args) {
                if (!isEnabled) {
                    return;
                }

                var now = Logger.getFormattedTimestamp(new Date());
                var message = '', supplantData = [];
                switch (args.length) {
                    case 1:
                        message = Logger.supplant("{0} - {1}: {2}", [now, this.context, args[0]]);
                        break;
                    case 3:
                        supplantData = args[2];
                        message = Logger.supplant("{0} - {1}::{2}(\'{3}\')", [now, this.context, args[0], args[1]]);
                        break;
                    case 2:
                        if (typeof args[1] === 'string') {
                            message = Logger.supplant("{0} - {1}::{2}(\'{3}\')", [now, this.context, args[0], args[1]]);
                        } else {
                            supplantData = args[1];
                            message = Logger.supplant("{0} - {1}: {2}", [now, this.context, args[0]]);
                        }
                        break;
                }

                $log[originalFn].call(null, Logger.supplant(message, supplantData));
            },
            log: function () {
                this._log('log', arguments);
            },
            info: function () {
                this._log('info', arguments);
            },
            warn: function () {
                this._log('warn', arguments);
            },
            debug: function () {
                this._log('debug', arguments);
            },
            error: function () {
                this._log('error', arguments);
            }
        };
        return Logger;
    }];
}]);

csapp.service('modalService', ['$modal', function ($modal) {

    var modalDefaults = {
        backdrop: true,
        keyboard: true,
        modalFade: true,
        templateUrl: baseUrl + 'Shared/templates/confirm-modal.html'
    };

    var modalOptions = {
        closeButtonText: 'Close',
        actionButtonText: 'OK',
        headerText: 'Proceed?',
        bodyText: 'Perform this action?'
    };

    this.showModal = function (customModalDefaults, customModalOptions) {
        if (!customModalDefaults) customModalDefaults = {};
        customModalDefaults.backdrop = 'static';
        return this.show(customModalDefaults, customModalOptions);
    };

    this.show = function (customModalDefaults, customModalOptions) {
        //Create temp objects to work with since we're in a singleton service
        var tempModalDefaults = {};
        var tempModalOptions = {};

        //Map angular-ui modal custom defaults to modal defaults defined in service
        angular.extend(tempModalDefaults, modalDefaults, customModalDefaults);

        //Map modal.html $scope custom properties to defaults defined in service
        angular.extend(tempModalOptions, modalOptions, customModalOptions);

        if (!tempModalDefaults.controller) {
            tempModalDefaults.controller = function ($scope, $modalInstance) {
                $scope.modalOptions = tempModalOptions;
                $scope.modalOptions.ok = function (result) {
                    $modalInstance.close(result);
                };
                $scope.modalOptions.close = function () {
                    $modalInstance.dismiss('cancel');
                };
            };
        }

        return $modal.open(tempModalDefaults).result;
    };

}]);

csapp.factory("loadingWidget", ["Logger", "$csStopWatch", function (logManager, $csStopWatch) {

    var requestCount = 0;
    // ReSharper disable once UnusedLocals
    var $log = logManager.getInstance("loadingWidget");

    var params = {
        enableSpinner: false,
        showSpinner: false,
        timer: $csStopWatch.data,

    };

    var requestStarted = function () {
        requestCount++;
        params.showSpinner = params.enableSpinner === true && requestCount > 0;
        $csStopWatch.start();
    };

    var requestEnded = function () {
        requestCount--;
        params.showSpinner = params.showSpinner === true && requestCount > 0;
        params.enableSpinner = false;
        $csStopWatch.stop();
        $csStopWatch.reset();
    };

    return {
        requestStarted: requestStarted,
        requestEnded: requestEnded,
        params: params,
    };
}]);

csapp.factory('MyHttpInterceptor', ["$q", "$rootScope", '$csAuthFactory', "Logger", "loadingWidget",
    function ($q, $rootScope, $csAuthFactory, logManager, loadingWidget) {

        var $log = logManager.getInstance("HttpInterceptor");
        var requestInterceptor = function (config) {
            if (config.url.indexOf("/api/") !== -1) {
                loadingWidget.requestStarted();
                config.headers.Authorization = $csAuthFactory.getUsername();
                $log.info("Request : " + config.url);
            }
            return config || $q.when(config);
        };

        var requestErrorInterceptor = function (rejection) {
            if (rejection.config.url.indexOf("/api/") !== -1) {
                loadingWidget.requestEnded();
                $log.info("RequestError : " + rejection.config.url);
            }
            return $q.reject(rejection);
        };

        var responseInterceptor = function (response) {
            if (response.config.url.indexOf("/api/") !== -1) {
                loadingWidget.requestEnded();
                $log.info("Response : " + response.config.url);
            }
            return response || $q.when(response);
        };

        var responseErrorInterceptor = function (rejection) {
            if (rejection.config.url.indexOf("/api/") !== -1) {
                loadingWidget.requestEnded();
                $log.info("ResponseError : " + rejection.config.url);
            }
            return $q.reject(rejection);
        };

        return {
            request: requestInterceptor,
            requestError: requestErrorInterceptor,
            response: responseInterceptor,
            responseError: responseErrorInterceptor
        };
    }
]);

csapp.factory("PermissionFactory", ["$csShared", "$csfactory", function ($csShared, $csfactory) {

    var checkAccess = function (perm, root) {
        var permission = root;
        for (var i = 0; i < perm.length; i++) {
            var permData = _.find(permission.Childrens, function (item) {
                return (item.Activity.trim().toUpperCase() === perm[i].trim().toUpperCase());
            });
            if ($csfactory.isEmptyObject(permData)) return false;
            if (permData.HasAccess === false) return false;
            permission = permData;
        }

        return permission.HasAccess;
    };

    var hasPermission2 = function (perm) {
        if ($csfactory.isNullOrEmptyString(perm)) throw "permission is undefined";
        if (perm.trim().toUpperCase() === "ALL") return true;

        switch (perm) {
            //fileupload
            case "FileDetailAddEdit":
                perm = "FileUploader,CreateFile,AddEdit";
                break;
            case "FileDetail":
                perm = "FileUploader,CreateFile";
                break;
            case "FileDetailApprove":
                perm = "FileUploader,CreateFile,Approve";
                break;
            case "FileSchedule":
                perm = "FileUploader,ScheduleFile";
                break;
            case "FileScheduleRetry":
                perm = "FileUploader,ScheduleFile,Retry";
                break;
            case "FileScheduleDelete":
                perm = "FileUploader,ScheduleFile,Delete";
                break;
            case "FileCustomerData":
                perm = "FileUploader,CustomerData";
                break;
            case "FileUploadCustInfo":
                perm = "FileUploader,UploadCustInfo";
                break;
            case "FileErrorCorrection":
                perm = "FileUploader,ErrorCorrection,AddEdit";
                break;

                //Allocation
            case "AllocationPolicyAddEdit":
                perm = "Allocation,AllocationPolicy,AddEdit";
                break;
            case "AllocationPolicyApprove":
                perm = "Allocation,AllocationPolicy,Approve";
                break;
            case "AllocationPolicy":
                perm = "Allocation,AllocationPolicy";
                break;
            case "AllocationSubpolicyAddEdit":
                perm = "Allocation,AllocationSubpolicy,AddEdit";
                break;
            case "AllocationSubpolicy":
                perm = "Allocation,AllocationSubpolicy";
                break;
            case "AllocationSubpolicyApprove":
                perm = "Allocation,AllocationSubpolicy,Approve";
                break;
            case "CheckAllocation":
                perm = "Allocation,CheckAllocation";
                break;
            case "CheckAllocationAddEdit":
                perm = "Allocation,CheckAllocation,AddEdit";
                break;
            case "CheckAllocationApprove":
                perm = "Allocation,CheckAllocation,Approve";
                break;

                //Billing
            case "BillingPolicyAddEdit":
                perm = "Billing,BillingPolicy,AddEdit";
                break;
            case "BillingPolicy":
                perm = "Billing,BillingPolicy";
                break;
            case "BillingPolicyApprove":
                perm = "Billing,BillingPolicy,Approve";
                break;
            case "BillingSubpolicyApprove":
                perm = "Billing,BillingSubpolicy,Approve";
                break;
            case "BillingSubpolicyAddEdit":
                perm = "Billing,BillingSubpolicy,AddEdit";
                break;
            case "BillingSubpolicy":
                perm = "Billing,BillingSubpolicy";
                break;
            case "BillingFormulaAddEdit":
                perm = "Billing,Formula,AddEdit";
                break;
            case "BillingFormula":
                perm = "Billing,Formula";
                break;
            case "BillingMatrix":
                perm = "Billing,Matrix";
                break;
            case "BillingMatrixAddEdit":
                perm = "Billing,Matrix,AddEdit";
                break;
            case "BillingHoldingPolicyAddEdit":
                perm = "Billing,HoldingPolicy,AddEdit";
                break;
            case "BillingHoldingPolicy":
                perm = "Billing,HoldingPolicy";
                break;
            case "BillingManageHolidngAddEdit":
                perm = "Billing,ManageHolidng,AddEdit";
                break;
            case "BillingManageHolidng":
                perm = "Billing,ManageHolidng";
                break;
            case "BillingAdhocPayoutAddEdit":
                perm = "Billing,AdhocPayout,AddEdit";
                break;
            case "BillingAdhocPayoutApprove":
                perm = "Billing,AdhocPayout,Approve";
                break;
            case "BillingModifyPaymentAddEdit":
                perm = "Billing,ModifyPayment,AddEdit";
                break;
            case "BillingModifyPaymentApprove":
                perm = "Billing,ModifyPayment,Approve";
                break;
            case "BillingReadyForBillingApprove":
                perm = "Billing,ReadyForBilling,Approve";
                break;
            case "BillingPayoutStatusAddEdit":
                perm = "Billing,PayoutStatus,AddEdit";
                break;
            case "BillingPayoutStatusApprove":
                perm = "Billing,PayoutStatus,Approve";
                break;
            case "BillingPayoutStatus":
                perm = "Billing,PayoutStatus";
                break;


                //Config
            case "ConfigPincode":
                perm = "Config,Pincode";
                break;
            case "ConfigPincodeAddEdit":
                perm = "Config,Pincode,AddEdit";
                break;
            case "ConfigTaxlistAddEdit":
                perm = "Config,Taxlist,AddEdit";
                break;
            case "ConfigTaxlist":
                perm = "Config,Taxlist";
                break;
            case "ConfigTaxmasterAddEdit":
                perm = "Config,Taxmaster,AddEdit";
                break;
            case "ConfigTaxmaster":
                perm = "Config,Taxmaster";
                break;
            case "ConfigKeyValueAddEdit":
                perm = "Config,KeyValue,AddEdit";
                break;
            case "ConfigProductAddEdit":
                perm = "Config,Product,AddEdit";
                break;
            case "ConfigProductApprove":
                perm = "Config,Product,Approve";
                break;
            case "ConfigProduct":
                perm = "Config,Product";
                break;
            case "ConfigHierarchyAddEdit":
                perm = "Config,Hierarchy,AddEdit";
                break;
            case "ConfigHierarchy":
                perm = "Config,Hierarchy";
                break;

                //user
            case "UserLogout":
                perm = "User,Logout";
                break;
            case "UserProfile":
                perm = "User,Profile";
                break;
            case "UserChangePassword":
                perm = "User,ChangePassword";
                break;

                //Developer
            case "DeveloperGenerateDb":
                perm = "Developer,GenerateDb";
                break;
            case "DeveloperSystemExplorer":
                perm = "Developer,SystemExplorer";
                break;
            case "DeveloperDbTables":
                perm = "Developer,DbTables";
                break;
            case "DeveloperExecuteQuery":
                perm = "Developer,ExecuteQuery";
                break;

                //stakeholder
            case "StakeholderAddEdit":
                perm = "Stakeholder,Stakeholder,AddEdit";
                break;
            case "StakeholderApprove":
                perm = "Stakeholder,Stakeholder,Approve";
                break;


            default:
                break;
        }

        perm = perm.split(',');
        var currentPerm = $csShared.Permissions;
        return checkAccess(perm, currentPerm);
    };

    return {
        HasPermission: hasPermission2
    };
}]);

