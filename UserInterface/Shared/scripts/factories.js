csapp.factory("$csfactory", ["$csConstants", "$csAuthFactory", "loadingWidget", function (consts, auth, loadingWidget) {

    var enableSpinner = function () {
        loadingWidget.params.enableSpinner = true;
    };

    var downloadFile = function (filename) {
        enableSpinner();
        var ifr = document.createElement('iframe');
        ifr.style.display = 'none';
        document.body.appendChild(ifr);
        ifr.src = baseUrl + "api/GridApi/DownloadFile?filename='" + filename + "'";
        ifr.onload = function () {
            document.body.removeChild(ifr);
            ifr = null;
        };
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

    function getPropertyValue(targetObj, keyPath) {
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
    }

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
        getPropertyValue: getPropertyValue
    };
}]);

csapp.factory('$csnotify', function () {

    var success = function (messege) {
        $.notify(messege, "success");
    };

    var log = function (messege) {
        $.notify(messege, "info");
        //if (isSticky) {
        //    alertify.log(messege, '', 0);
        //} else {
        //    alertify.log(messege);
        //}
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
    var data = { value: 1, laps: [], stringValue: '' };
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
        data.laps = [];
    };

    var lap = function () {
        data.laps.push(data.value);
    };

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
        lap: lap
    };
}]);

csapp.filter("minLength", ["$csfactory", function ($csfactory) {
    return function (array, value, length) {
        if (angular.isUndefined(array)
            || $csfactory.isNullOrEmptyString(value)
            || $csfactory.isNullOrEmptyString(length)
            || !angular.isNumber(length)) {
            return [];
        }

        var filteredRow = _.where(array, function (row) { return (row[value].length > length); });
        return filteredRow;
    };
}]);

csapp.filter('csmakeRange', function () {
    return function (input) {
        var lowBound, highBound;
        switch (input.length) {
            case 1:
                lowBound = 0;
                highBound = parseInt(input[0]) - 1;
                break;
            case 2:
                lowBound = parseInt(input[0]);
                highBound = parseInt(input[1]);
                break;
            default:
                return input;
        }
        if (highBound < lowBound)
            highBound = lowBound;
        var result = [];
        for (var i = lowBound; i <= highBound; i++)
            result.push(i);
        return result;
    };
});

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

csapp.factory("loadingWidget", ["Logger", function (logManager) {

    var requestCount = 0;
    // ReSharper disable once UnusedLocals
    var $log = logManager.getInstance("loadingWidget");
    var params = {
        enableSpinner: false,
        showSpinner: false
    };

    var requestStarted = function () {
        requestCount++;
        params.showSpinner = params.enableSpinner === true && requestCount > 0;
        //params.showSpinner = true;
        //$log.debug("showing spinner : " + params.showSpinner);
    };

    var requestEnded = function () {
        requestCount--;
        params.showSpinner = params.showSpinner === true && requestCount > 0;
        params.enableSpinner = false;
        //params.showSpinner = true;
        //$log.debug("hding spinner : " + params.showSpinner);
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
                if (item.Activity.trim().toUpperCase() === perm[i].trim().toUpperCase()) return item;
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
                perm = "FileUploader,ScheduleFile,Schedule";
                break;
            case "FileStatus":
                perm = "FileUploader,Status";
                break;
            case "FileStatusAddEdit":
                perm = "FileUploader,Status,AddEdit";
                break;
            case "FileStatusDelete":
                perm = "FileUploader,Status,Delete";
                break;
            case "FileCustomerData":
                perm = "FileUploader,CustomerData,View";
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
            case "AllocationPolicyView":
                perm = "Allocation,AllocationPolicy,View";
                break;
            case "AllocationSubpolicyAddEdit":
                perm = "Allocation,AllocationSubpolicy,AddEdit";
                break;
            case "AllocationSubpolicyView":
                perm = "Allocation,AllocationSubpolicy,View";
                break;
            case "AllocationSubpolicyApprove":
                perm = "Allocation,AllocationSubpolicy,Approve";
                break;
            case "CheckAllocationView":
                perm = "Allocation,CheckAllocation,View";
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
            case "BillingPolicyView":
                perm = "Billing,BillingPolicy,View";
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
            case "BillingSubpolicyView":
                perm = "Billing,BillingSubpolicy,View";
                break;
            case "BillingFormulaAddEdit":
                perm = "Billing,Formula,AddEdit";
                break;
            case "BillingFormulaView":
                perm = "Billing,Formula,View";
                break;
            case "BillingMatrixView":
                perm = "Billing,Matrix,View";
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
            case "BillingHoldingPolicyView":
                perm = "Billing,HoldingPolicy,View";
                break;
            case "BillingManageHolidngAddEdit":
                perm = "Billing,ManageHolidng,AddEdit";
                break;
            case "BillingManageHolidng":
                perm = "Billing,ManageHolidng";
                break;
            case "BillingManageHolidngView":
                perm = "Billing,ManageHolidng,View";
                break;
            case "BillingAdhocPayoutAddEdit":
                perm = "Billing,AdhocPayout,AddEdit";
                break;
            case "BillingAdhocPayoutView":
                perm = "Billing,AdhocPayout,View";
                break;
            case "BillingAdhocPayoutApprove":
                perm = "Billing,AdhocPayout,Approve";
                break;
            //case "BillingPayoutStatusApprove":
            //    perm = "Billing,PayoutStatus,Approve";
            //    break;
            //case "BillingPayoutStatusApprove":
            //    perm = "Billing,PayoutStatus,Approve";
            //    break;

                //Config
            case "Configpincode":
                perm = "Config,Pincode";
                break;
            case "ConfigpincodeAddEdit":
                perm = "Config,Pincode,AddEdit";
                break;
            case "ConfigpincodeView":
                perm = "Config,Pincode,View";
                break;
            case "ConfigTaxlistAddEdit":
                perm = "Config,Taxlist,AddEdit";
                break;
            case "ConfigTaxlist":
                perm = "Config,Taxlist";
                break;
            case "ConfigTaxlistView":
                perm = "Config,Taxlist,View";
                break;
            case "ConfigTaxmasterAddEdit":
                perm = "Config,Taxmaster,AddEdit";
                break;
            case "ConfigTaxmaster":
                perm = "Config,Taxmaster";
                break;
            case "ConfigTaxmasterView":
                perm = "Config,Taxmaster,View";
                break;
            case "ConfigKeyValueAddEdit":
                perm = "Config,KeyValue,AddEdit";
                break;
            case "ConfigProductView":
                perm = "Config,Product,View";
                break;
            case "ConfigProductAddEdit":
                perm = "Config,Product,AddEdit";
                break;
            case "ConfigHierarchyAddEdit":
                perm = "Config,Hierarchy,AddEdit";
                break;
            case "ConfigHierarchy":
                perm = "Config,Hierarchy";
                break;
            case "ConfigHierarchyView":
                perm = "Config,Hierarchy,View";
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



