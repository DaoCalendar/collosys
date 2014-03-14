csapp.factory("$csfactory", ["$csConstants", "$rootScope", function(consts, $rootScope) {

    var hideSpinner = function () {
        $rootScope.loadingElement.disableSpiner = true;
    };

    var isNullOrEmptyString = function(val) {

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

    var isEmptyObject = function(obj) {
        var name;
        // ReSharper disable AssignedValueIsNeverUsed
        for (name in obj) {
            return false;
        }
        // ReSharper restore AssignedValueIsNeverUsed
        return true;
    };

    var isNullOrEmptyArray = function(val) {
        if (isNullOrEmptyString(val)) {
            return true;
        }

        if (angular.isArray(val) && (val.length === 0)) {
            return true;
        }

        return false;
    };

    var isNullOrEmptyGuid = function(val) {
        if (isNullOrEmptyString(val)) {
            return true;
        }

        if (val === consts.GUID_EMPTY) {
            return true;
        }

        return false;
    };

    var getDefaultGuid = function() {
        return consts.GUID_EMPTY;
    };

    return {
        isNullOrEmptyArray: isNullOrEmptyArray,
        isNullOrEmptyString: isNullOrEmptyString,
        isNullOrEmptyGuid: isNullOrEmptyGuid,
        getDefaultGuid: getDefaultGuid,
        isEmptyObject: isEmptyObject,
        hideSpinner: hideSpinner
    };
}]);

csapp.factory('$csnotify', function() {

    var success = function(messege) {
        $.notify(messege, "success");
    };

    var log = function(messege) {
        $.notify(messege, "info");
        //if (isSticky) {
        //    alertify.log(messege, '', 0);
        //} else {
        //    alertify.log(messege);
        //}
    };

    var error = function(messege) {
        $.notify(messege, "error");
    };

    return {
        success: success,
        error: error,
        log: log
    };
});

csapp.factory('$Validations', function() {

    var required = function() {
        return 'Required field';
    };

    var validName = function(val) {
        return 'Enter valid ' + val;
    };

    var format = function(frmt) {
        return 'Accepts only ' + frmt + ' format';
    };

    var equalvalue = function(val) {
        return 'Length should be equal to' + val;
    };

    var onlyNumbers = function() {
        return 'Accept only numbers. ';
    };

    var graterThan = function(property, value) {
        return property + ' must be greater than ' + value;
    };

    var maxlength = function(val) {

        return 'Exceeds maximum size ( ' + val + ' )';
    };

    var minlength = function(val) {

        return 'Minimum required length is ' + val;
    };

    var onlytext = function() {
        return 'Accept characters only.';
    };

    var mandatory = function() {
        return '';
    };

    var specialChar = function() {
        return 'Special characters not allowed';
    };

    var pattern = function(val) {
        return 'Only number with ' + val + ' digits.';
    };
    var mobile = function() {
        return 'Accept 10 digit mobile number.';
    };

    var alreadyExist = function(val) {
        return val + ' already exist.';
    };

    var maxAmt = function(maxamt) {
        return 'Exceeds maximum amount limit ( ' + maxamt + ' )';
    };


    return {
        Required: required,
        OnlyNumbers: onlyNumbers,
        GreaterThan: graterThan,
        MaxLength: maxlength,
        MinLength: minlength,
        ValidName: validName,
        EqualLength: equalvalue,
        OnlyText: onlytext,
        Mandatory: mandatory,
        SpecialCharNotAllow: specialChar,
        Pattern: pattern,
        Mobile: mobile,
        AlreadyExist: alreadyExist,
        MaxAmt: maxAmt,
        Format: format
    };
});

csapp.factory('$csStopWatch', ["$timeout", function($timeout) {
    var data = { value: 1, laps: [], stringValue: '' };
    var stopwatch = null;

    var start = function() {
        stopwatch = $timeout(function() {
            data.value++;
            data.stringValue = getTime(data.value);
            start();
        }, 1000);
    };

    var stop = function() {
        $timeout.cancel(stopwatch);
        stopwatch = null;
    };

    var reset = function() {
        stop();
        data.value = 0;
        data.stringValue = '';
        data.laps = [];
    };

    var lap = function() {
        data.laps.push(data.value);
    };

    var getTime = function(totalTime) {

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

csapp.filter("minLength", ["$csfactory", function($csfactory) {
    return function(array, value, length) {
        if (angular.isUndefined(array)
            || $csfactory.isNullOrEmptyString(value)
            || $csfactory.isNullOrEmptyString(length)
            || !angular.isNumber(length)) {
            return [];
        }

        var filteredRow = _.where(array, function(row) { return (row[value].length > length); });
        return filteredRow;
    };
}]);

csapp.filter('csmakeRange', function() {
    return function(input) {
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

csapp.filter('bytes', function() {
    return function(bytes, precision) {
        if (bytes == 0 || isNaN(parseFloat(bytes)) || !isFinite(bytes)) return '-';
        if (typeof precision === 'undefined') precision = 1;
        var units = ['bytes', 'KB', 'MB', 'GB', 'TB', 'PB'],
            number = Math.floor(Math.log(bytes) / Math.log(1024));
        return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) + ' ' + units[number];
    };
});

csapp.filter('fromNow', function() {
    return function(dateString) {
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

csapp.service('modalService', ['$modal', function($modal) {

    var modalDefaults = {
        backdrop: true,
        keyboard: true,
        modalFade: true,
        templateUrl: '/Shared/templates/confirm-modal.html'
    };

    var modalOptions = {
        closeButtonText: 'Close',
        actionButtonText: 'OK',
        headerText: 'Proceed?',
        bodyText: 'Perform this action?'
    };

    this.showModal = function(customModalDefaults, customModalOptions) {
        if (!customModalDefaults) customModalDefaults = {};
        customModalDefaults.backdrop = 'static';
        return this.show(customModalDefaults, customModalOptions);
    };

    this.show = function(customModalDefaults, customModalOptions) {
        //Create temp objects to work with since we're in a singleton service
        var tempModalDefaults = {};
        var tempModalOptions = {};

        //Map angular-ui modal custom defaults to modal defaults defined in service
        angular.extend(tempModalDefaults, modalDefaults, customModalDefaults);

        //Map modal.html $scope custom properties to defaults defined in service
        angular.extend(tempModalOptions, modalOptions, customModalOptions);

        if (!tempModalDefaults.controller) {
            tempModalDefaults.controller = function($scope, $modalInstance) {
                $scope.modalOptions = tempModalOptions;
                $scope.modalOptions.ok = function(result) {
                    $modalInstance.close(result);
                };
                $scope.modalOptions.close = function() {
                    $modalInstance.dismiss('cancel');
                };
            };
        }

        return $modal.open(tempModalDefaults).result;
    };

}]);

