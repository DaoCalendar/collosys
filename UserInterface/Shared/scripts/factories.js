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

csapp.factory('$permissionFactory', [function () {
    //#region file upload activities
    var createFileActivity = function () {
        return {
            name: "CreateFile",
            access: false,
            description: "create file",
            childrens: {
                View: {
                    name: "View",
                    access: false,
                    description: "view file",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "update file",
                    childrens: {}
                },
                Create: {
                    name: "Approve",
                    access: false,
                    description: "approve file",
                    childrens: {}
                },
            }
        };
    };
    var scheduleFileActivity = function () {
        return {
            name: "ScheduleFile",
            access: false,
            description: "schedule file",
            childrens: {
                Schedule: {
                    name: "Schedule",
                    access: false,
                    description: "schedule file",
                    childrens: {}
                },
                Status: {
                    name: "Status",
                    access: false,
                    description: "check status",
                    childrens: {}
                }
            }
        };
    };
    var customerDataActivity = function () {
        return {
            name: "Customer Data",
            access: false,
            description: "create file",
            childrens: {
                View: {
                    name: "View",
                    access: false,
                    description: "view file",
                    childrens: {}
                }
            }
        };
    };
    var uploadCustomerInfoActivity = function () {
        return {
            name: "Upload Customer Info",
            access: false,
            description: "upload customer info",
            childrens: {
                Update: {
                    name: "Update",
                    access: false,
                    description: "view file",
                    childrens: {}
                }
            }
        };
    };
    var errorCorrectionActivity = function () {
        return {
            name: 'Error Correction',
            access: false,
            description: "error correction",
            childrens: {
                Update: {
                    name: "Update",
                    access: false,
                    description: "update error correction",
                    childrens: {}
                },
                Approve: {
                    name: "Approve",
                    access: false,
                    description: "approve error correction",
                    childrens: {}
                },
            }
        };
    };
    var modifyPaymentActivity = function () {
        return {
            name: "Modify Payment",
            access: false,
            description: "modify payment",
            childrens: {
                View: {
                    name: "View",
                    access: false,
                    description: "view payment",
                    childrens: {}
                },
                Create: {
                    name: "Create",
                    access: false,
                    description: "create payment",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "update payment",
                    childrens: {}
                },
                Approve: {
                    name: "Approve",
                    access: false,
                    description: "update file",
                    childrens: {}
                },
            }
        };
    };
    //#endregion

    //#region stakeholder activities
    var addStakeholderActivity = function () {
        return {
            name: "Add Stakeholder",
            access: false,
            description: "add stakeholder",
            childrens: {

                Create: {
                    name: "Create",
                    access: false,
                    description: "create stakeholder",
                    childrens: {}
                }
            }
        };
    };
    var viewStakeholderActivity = function () {
        return {
            name: "View Stakeholder",
            access: false,
            description: "view stakeholder",
            childrens: {
                Update: {
                    name: "Update",
                    access: false,
                    description: "update  stakeholder",
                    childrens: {}
                },
                View: {
                    name: "View",
                    access: false,
                    description: "view stakeholder",
                    childrens: {}
                },
                Approve: {
                    name: "Approve",
                    access: false,
                    description: "approve stakeholder",
                    childrens: {}
                },
            }
        };
    };
    var addHierarchyActivity = function () {
        return {
            name: "Add Hierarchy",
            access: false,
            description: "add hierarchy",
            childrens: {
                Create: {
                    name: "Create",
                    access: false,
                    description: "create hierarchy",
                    childrens: {}
                },
            }
        };
    };
    var viewHierarchyActivity = function () {
        return {
            name: "View Hierarchy",
            access: false,
            description: "view hierarchy",
            childrens: {
                Update: {
                    name: "Update",
                    access: false,
                    description: "update  hierarchy",
                    childrens: {}
                },
                View: {
                    name: "View",
                    access: false,
                    description: "view hierarchy",
                    childrens: {}
                },
                Approve: {
                    name: "Approve",
                    access: false,
                    description: "approve hierarchy",
                    childrens: {}
                },
            }
        };
    };
    //#endregion

    //#region allocation activities
    var definePolicyActivity = function () {
        return {
            name: "Define Policy",
            access: false,
            description: "define policy",
            childrens: {

                View: {
                    name: "View",
                    access: false,
                    description: "view policy",
                    childrens: {}
                },
                Create: {
                    name: "Create",
                    access: false,
                    description: "create policy",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "update policy",
                    childrens: {}
                },
                Approve: {
                    name: "Approve",
                    access: false,
                    description: "approve policy",
                    childrens: {}
                },
            }
        };

    };
    var defineSubPolicyActivity = function () {
        return {
            name: "Define Subpolicy",
            access: false,
            description: "define subpolicy",
            childrens: {

                View: {
                    name: "View",
                    access: false,
                    description: "view policy",
                    childrens: {}
                },
                Create: {
                    name: "Create",
                    access: false,
                    description: "create policy",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "update policy",
                    childrens: {}
                },
                Approve: {
                    name: "Approve",
                    access: false,
                    description: "approve policy",
                    childrens: {}
                }
            }
        };
    };
    var checkAllocationActivity = function () {
        return {
            name: "Check Allocation",
            access: false,
            description: "check allocation",
            childrens: {

                View: {
                    name: "View",
                    access: false,
                    description: "view policy",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "update policy",
                    childrens: {}
                },
                Approve: {
                    name: "Approve",
                    access: false,
                    description: "approve policy",
                    childrens: {}
                },
            }
        };
    };
    //#endregion

    //#region billing activities
    var defineBillingPolicyActivity = function () {
        return {
            name: "Define Policy",
            access: false,
            description: "define billing policy",
            childrens: {

                View: {
                    name: "View",
                    access: false,
                    description: "view billing policy",
                    childrens: {}
                },
                Create: {
                    name: "Create",
                    access: false,
                    description: "create billing policy",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "update billing policy",
                    childrens: {}
                },
                Approve: {
                    name: "Approve",
                    access: false,
                    description: "approve billing policy",
                    childrens: {}
                },
            }
        };
    };
    var defineBillingSubpolicyActivity = function () {
        return {
            name: "Define Billing Subpolicy",
            access: false,
            description: "define billing subpolicy",
            childrens: {

                View: {
                    name: "View",
                    access: false,
                    description: "view billing policy",
                    childrens: {}
                },
                Create: {
                    name: "Create",
                    access: false,
                    description: "create billing policy",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "update billing policy",
                    childrens: {}
                },
                Approve: {
                    name: "Approve",
                    access: false,
                    description: "approve billing policy",
                    childrens: {}
                },
            }
        };
    };
    var defineFormulaActivity = function () {
        return {
            name: "Define Formula",
            access: false,
            description: "define formula",
            childrens: {
                View: {
                    name: "View",
                    access: false,
                    description: "view formula",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "update formula",
                    childrens: {}
                },
                Create: {
                    name: "Create",
                    access: false,
                    description: "create formula",
                    childrens: {}
                },
            }
        };
    };
    var defineMatrixActivity = function () {
        return {
            name: "Define Matrix",
            access: false,
            description: "define matrix",
            childrens: {
                View: {
                    name: "View",
                    access: false,
                    description: "view matrix",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "update matrix",
                    childrens: {}
                },
                Create: {
                    name: "Create",
                    access: false,
                    description: "create matrix",
                    childrens: {}
                },
            }
        };
    };
    var adhocPayoutActivity = function () {
        return {
            name: "Adhoc Payout",
            access: false,
            description: "adhoc payout",
            childrens: {

                View: {
                    name: "View",
                    access: false,
                    description: "view adhoc payout",
                    childrens: {}
                },
                Create: {
                    name: "Create",
                    access: false,
                    description: "create adhoc payout",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "update adhoc payout",
                    childrens: {}
                },
                Approve: {
                    name: "Approve",
                    access: false,
                    description: "approve adhoc payout",
                    childrens: {}
                },
            }

        };
    };
    var readyForBillingActivity = function () {
        return {
            name: "Ready for Billing",
            access: false,
            description: "ready for billing",
            childrens: {
                View: {
                    name: "View",
                    access: false,
                    description: "view billing",
                    childrens: {}
                },
                Approve: {
                    name: "Approve",
                    access: false,
                    description: "approve billing",
                    childrens: {}
                }
            }
        };
    };
    var payoutStatusActivity = function () {
        return {
            name: "Payout Status",
            access: false,
            description: "payout status",
            childrens: {

                View: {
                    name: "View",
                    access: false,
                    description: "view payout status",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "update payout status",
                    childrens: {}
                }
            }
        };
    };
    //#endregion

    //#region config
    var permissionActivity = function () {
        return {
            name: "Permission",
            access: false,
            description: "permission",
            childrens: {

                View: {
                    name: "View",
                    access: false,
                    description: "view permission",
                    childrens: {}
                },
                Approve: {
                    name: "Approve",
                    access: false,
                    description: "approve permission",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "update permission",
                    childrens: {}
                }
            }
        };
    };
    var productsActivity = function () {
        return {
            name: "Products",
            access: false,
            description: "products",
            childrens: {

                View: {
                    name: "View",
                    access: false,
                    description: "view products",
                    childrens: {}
                },
                Approve: {
                    name: "Approve",
                    access: false,
                    description: "approve products",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "create products",
                    childrens: {}
                }
            }
        };
    };
    var keyValueActivity = function () {
        return {
            name: "KeyValue",
            access: false,
            description: "key value",
            childrens: {

                View: {
                    name: "View",
                    access: false,
                    description: "view key value",
                    childrens: {}
                },
                Approve: {
                    name: "Approve",
                    access: false,
                    description: "approve key value",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "create key value",
                    childrens: {}
                }
            }
        };
    };
    var pincodeActivity = function () {
        return {
            name: "Pincode",
            access: false,
            description: "pincode",
            childrens: {

                View: {
                    name: "View",
                    access: false,
                    description: "view pincode",
                    childrens: {}
                },
                Create: {
                    name: "Create",
                    access: false,
                    description: "create pincode",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "create pincode",
                    childrens: {}
                }
            }
        };
    };
    var taxListActivity = function () {
        return {
            name: "Tax List",
            access: false,
            description: "tax list",
            childrens: {

                View: {
                    name: "View",
                    access: false,
                    description: "view tax list",
                    childrens: {}
                },
                Create: {
                    name: "Create",
                    access: false,
                    description: "create tax list",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "update tax list",
                    childrens: {}
                }
            }
        };
    };
    var taxMasterActivity = function () {
        return {
            name: "Tax Master",
            access: false,
            description: "tax master",
            childrens: {

                View: {
                    name: "View",
                    access: false,
                    description: "view tax master",
                    childrens: {}
                },
                Create: {
                    name: "Create",
                    access: false,
                    description: "create tax master",
                    childrens: {}
                },
                Update: {
                    name: "Update",
                    access: false,
                    description: "update tax master",
                    childrens: {}
                }
            }
        };
    };
    //#endregion

    var permission = {

        FileUpload: {
            name: "FileUpload",
            access: false,
            description: "file upload",
            childrens: {
                CreateFile: createFileActivity(),
                ScheduleFile: scheduleFileActivity(),
                CustomerData: customerDataActivity(),
                UploadCustInfo: uploadCustomerInfoActivity(),
                ErrorCorrection: errorCorrectionActivity(),
                ModifyPayment: modifyPaymentActivity()
            }
        },

        Stakeholder: {
            name: "Stakeholder",
            access: false,
            description: "stakeholders",
            childrens: {
                AddStakeholder: addStakeholderActivity(),
                ViewStakeholder: viewStakeholderActivity(),
                AddHierarchy: addHierarchyActivity(),
                ViewHierarchy: viewHierarchyActivity()
            }
        },

        Billing: {
            name: "Billing",
            access: false,
            description: "billing",
            childrens: {
                DefinePolicy: defineBillingPolicyActivity(),
                DefineSubPolicy: defineBillingSubpolicyActivity(),
                DefineFormula: defineFormulaActivity(),
                DefineMatrix: defineMatrixActivity(),
                AdhocPayout: adhocPayoutActivity(),
                ReadyForBilling: readyForBillingActivity(),
                PayoutStatus: payoutStatusActivity()
            }
        },

        Allocation: {
            name: 'Allocation',
            access: false,
            description: 'allocation',
            childrens: {
                DefinePolicy: definePolicyActivity(),
                DefineSubpolicy: defineSubPolicyActivity(),
                CheckAllocation: checkAllocationActivity()
            }
        },

        Config: {
            name: 'Config',
            assess: false,
            childrens: {
                Permission: permissionActivity(),
                Products: productsActivity(),
                KeyValue: keyValueActivity(),
                Pincode: pincodeActivity(),
                TaxList: taxListActivity(),
                TaxMaster: taxMasterActivity()
            }
        }
    };


    return {
        permission: permission
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
                console.log(rejection);
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
                console.log(rejection);
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
                if (item.Activity.toUpperCase() === perm[i].toUpperCase()) return item;
            });
            if ($csfactory.isEmptyObject(permData)) return false;
            if (permData.HasAccess === false) return false;
            permission = permData;
        }

        return permission.HasAccess;
    };

    var hasPermission = function (perm) {
        if ($csfactory.isNullOrEmptyString(perm)) throw "permission is undefined";
        if (perm === "All") return true;
        perm = perm.split(',');
        var currentPerm = $csShared.Permissions;
        return checkAccess(perm, currentPerm);
    };

    return {
        HasPermission: hasPermission
    };
}]);



