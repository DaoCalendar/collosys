/*globals  angular, console, render, toggle */
var csapp = (angular.module("ui.collosys",
    ['ui.bootstrap', 'ui.filters', 'ui.directives', 'ui.select2', 'ngUpload', 'ngGrid', 'restangular']));

//#region angular global config

var apiBaseUrl = mvcBaseUrl + "api/";

(
csapp.config(["RestangularProvider", "$logProvider", "$provide", "$httpProvider",
    function (restangularProvider, $logProvider, $provide, $httpProvider) {

        $logProvider.debugEnabled(true);

        restangularProvider.setBaseUrl(apiBaseUrl);

        $provide.factory('MyHttpInterceptor', ["$q", "$rootScope", function ($q, $rootScope) {
            var requestInterceptor = function (config) {
                if (config.url.indexOf("/api/") !== -1) {
                    $rootScope.$broadcast('$csShowSpinner');
                    console.log("HttpInterceptor : Request : " + moment().format("HH:mm:ss:SSS") + " : " + config.url);
                    //console.log(config);
                }
                // Return the config or wrap it in a promise if blank.
                return config || $q.when(config);
            };

            var requestErrorInterceptor = function (rejection) {
                if (rejection.config.url.indexOf("/api/") !== -1) {
                    $rootScope.$broadcast('$csHideSpinner');
                    console.log("HttpInterceptor : RequestError : " + moment().format("HH:mm:ss:SSS") + " : " + rejection.config.url);
                    console.log(rejection);
                }
                // Return the promise rejection.
                return $q.reject(rejection);
            };

            var responseInterceptor = function (response) {
                if (response.config.url.indexOf("/api/") !== -1) {
                    $rootScope.$broadcast('$csHideSpinner');
                    console.log("HttpInterceptor : Response : " + moment().format("HH:mm:ss:SSS") + " : " + response.config.url);
                }
                //console.log(response);
                // Return the response or promise.
                return response || $q.when(response);
            };

            var responseErrorInterceptor = function (rejection) {
                if (rejection.config.url.indexOf("/api/") !== -1) {
                    $rootScope.$broadcast('$csHideSpinner');
                    console.log("HttpInterceptor : ResponseError : " + moment().format("HH:mm:ss:SSS") + " : " + rejection.config.url);
                    console.log(rejection);
                }

                if (rejection.status == 401) {
                    var deferred = $q.defer();
                    $rootScope.$broadcast('$csLoginRequired');
                    return deferred.promise;
                }

                if (rejection.status == 0 || rejection.status == 404) {
                    rejection.Message = "You are offline !";
                }

                // Return the promise rejection.
                return $q.reject(rejection);
            };

            return {
                request: requestInterceptor,
                requestError: requestErrorInterceptor,
                response: responseInterceptor,
                responseError: responseErrorInterceptor
            };
        }]);
        $httpProvider.interceptors.push('MyHttpInterceptor');
    }])
);

(csapp.run(function ($rootScope, $log, $window) {
    $rootScope.$log = $log;
    $rootScope.loadingElement = { waitingForServerResponse: false };

    $rootScope.$on("$csHideSpinner", function () {
        $rootScope.loadingElement.waitingForServerResponse = false;
    });

    $rootScope.$on("$csShowSpinner", function () {
        $rootScope.loadingElement.waitingForServerResponse = true;
    });

    $rootScope.$on("$csLoginRequired", function () {
        $log.info("FATAL: Unauthorized access");
        $window.location = $window.mvcBaseUrl + "Account/Login";
    });
}));

(csapp.constant("$csConstants", {
    API_BASE_URL: apiBaseUrl,
    MVC_BASE_URL: mvcBaseUrl,
    GUID_EMPTY: '00000000-0000-0000-0000-000000000000',
    DaysOfWeek: {
        list: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
        value: { Mon: "Mon", Tue: "Tue", Wed: "Wed", Thu: "Thu", Fri: "Fri", Sat: "Sat", Sun: "Sun" }
    },
    EmailFrequency: {
        list: ["Daily", "Weekly", "Monthly"],
        value: { Daily: "Daily", Weekly: "Weekly", Monthly: "Monthly" }
    }
}));

//#endregion

//#region Boolean DisplayOnly

(csapp.directive("showboolean", function () {
    return {
        restrict: 'E',
        scope: {
            value: "@"
        },
        template: '<div style="text-align:center">' +
            '<i ng-show="value" class="btn icon-check" style="color: green"></i>' +
            '<i ng-hide="value" class="btn icon-remove" style="color: red"></i>' +
            '</div>'
    };
}));

(csapp.filter('custBillSubString', function () {
    return function (columnNames) {
        var filtered = [];
        angular.forEach(columnNames, function (columnName) {
            if (columnName.search("CustBillViewModel") > -1) {
                var cust = columnName.replace("CustBillViewModel", "Customer");
                filtered.push(cust);
            }
            if (columnName.search("GPincode") > -1) {
                var pin = columnName.replace("GPincode", "Pincode");
                filtered.push(pin);
            }
            if (columnName.search("StkhBillViewModel") > -1) {
                var stake = columnName.replace("StkhBillViewModel", "Stakeholder");
                filtered.push(stake);
            }
        });
        return filtered;
    };
}));

(csapp.filter('custBillSubString2', function () {
    return function (columnName) {
        if (columnName.search("CustBillViewModel") > -1) {
            return columnName.replace("CustBillViewModel", "Customer");

        }
        if (columnName.search("GPincode") > -1) {
            return columnName.replace("GPincode", "Pincode");
        }
        if (columnName.search("StkhBillViewModel") > -1) {
            return columnName.replace("StkhBillViewModel", "Stakeholder");
        }
        return columnName;
    };
}));

(csapp.directive('btnSwitch', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        //templateUrl: 'switcher.html',
        template: '<span class="btn boolean"><span class="on btn-primary">Yes</span>' +
                  '<span class="off btn-default">No</span></span>',
        replace: true,
        link: function (scope, element, attrs, ngModel) {

            // Specify how UI should be updated
            ngModel.$render = function () {
                render();
            };

            var render = function () {
                var val = ngModel.$viewValue;

                var open = angular.element(element.children()[0]);
                open.removeClass(val ? 'hide' : 'show');
                open.addClass(val ? 'show' : 'hide');

                var closed = angular.element(element.children()[1]);
                closed.removeClass(val ? 'show' : 'hide');
                closed.addClass(val ? 'hide' : 'show');
            };

            // Listen for the button click event to enable binding
            element.bind('click', function () {
                scope.$apply(toggle);
            });

            // Toggle the model value
            function toggle() {
                var val = ngModel.$viewValue;
                ngModel.$setViewValue(!val);
                render();
            }

            if (!ngModel) {
                return;
            }  // do nothing if no ng-model

            // Initial render
            render();
        }
    };
}));

(csapp.directive('switchyesno', function () {
    return {
        restrict: 'E',
        scope: {
            text: "@",
            readonly: "=",
            ngbind: "="
        },
        template: '<div class="control-group row-fluid">' +
                        '<label class="control-label">{{text}}</label>' +
                    '<div class="controls" ng-show="!readonly">' +
                        '<div btn-switch data-ng-model="ngbind"></div>' +
                    '</div>' +
                    '<div class="controls" ng-show="readonly">' +
                        '<i ng-show="{{ngbind}}" class="btn icon-check" style="color: green"></i>' +
                        '<i ng-show="{{!ngbind}}" class="btn icon-remove" style="color: red"></i>' +
                    '</div>' +
                 '</div>'
    };
}));

(csapp.directive('cspagination', function () {

    return {
        restrict: 'E',
        scope: {
            gotofirstpage: '&',
            gotolastpage: '&',
            stepforward: '&',
            stepbackward: '&',
            totalrecords: '=',
            currpagenum: '=',
            pagesize: '='
        },

        link: function (scope) {
            scope.pagesize = 5;
            scope.currpagenum = 1;
            scope.getrecordnum = function () {
                if (scope.currpagenum * scope.pagesize > scope.totalrecords)
                    return scope.totalrecords;
                else return (scope.currpagenum * scope.pagesize);
            };
        },

        template: '<div>' +
            '<div class="text-right">' +
            '<div><b>Records: {{(pagesize*(currpagenum-1))+1}}</b> - <b>{{getrecordnum()}}</b> of <b>{{totalrecords}}</b></div>' +
            '<button class="btn" data-ng-click="gotofirstpage()"><i class="icon-step-backward"></i></button>' +
            '<button class="btn" data-ng-click="stepbackward()"><i class="icon-caret-left icon-large"></i></button>' +
            '<input type="text" readonly data-ng-model=currpagenum style="margin-top: 0px" class="input-mini text-center">' +
            '<button class="btn" data-ng-click="stepforward()"><i class="icon-caret-right icon-large"></i></button>' +
            '<button class="btn" data-ng-click="gotolastpage()"><i class="icon-step-forward"></i></button>' +
            '</div>' +
            '</div>'
    };


}));

(csapp.directive("csswitch", function () {

    var linkfunction = function (scope) {

        scope.clickbtn = function (namevalue) {
            scope.ngbind = namevalue.Value;
        };

        scope.$watch('ngbind', function () {
            scope.onbtnclick();
        });
    };

    return {
        restrict: 'E',
        template: '<div class="btn-group">' +
            '<button data-ng-disabled="ngdisabled" data-ng-repeat="namevalue in namevalues" ' +
            'data-ng-click=clickbtn(namevalue); ' +
            'data-ng-class="(ngbind===namevalue.Value)?\'btn btn-success\':\'btn\'">{{namevalue.Name}}</button>' +
            '</div>',
        scope: {
            ngbind: "=",
            namevalues: "=",
            ngdisabled: "=",
            onbtnclick: "&"
        },
        link: linkfunction
    };
}));

//#endregion

//#region datepicker / fileReader / debounce

(csapp.directive("spinner", function () {
    return {
        restrict: 'C',
        link: function (scope, element) {
            element.bind("mouseenter", function () {
                element.addClass("icon-spin");
            });
            element.bind("mouseleave", function () {
                element.removeClass("icon-spin");
            });
        }
    };
}));

(csapp.directive("fileread", function () {
    return {
        scope: { fileread: "=" },
        link: function (scope, element) {
            element.bind("change", function (changeEvent) {
                scope.$apply(function () {
                    scope.fileread = changeEvent.target.files[0];
                    // or all selected files:
                    // scope.fileread = changeEvent.target.files;
                });
            });
        }
    };
}));

(csapp.directive('bsDatepicker', function () {
    var isAppleTouch = /(iP(a|o)d|iPhone)/g.test(navigator.userAgent);
    var regexpMap = function (language) {
        language = language || 'en';
        return {
            '/': '[\\/]',
            '-': '[-]',
            '.': '[.]',
            ' ': '[\\s]',
            'dd': '(?:(?:[0-2]?[0-9]{1})|(?:[3][01]{1}))',
            'd': '(?:(?:[0-2]?[0-9]{1})|(?:[3][01]{1}))',
            'mm': '(?:[0]?[1-9]|[1][012])',
            'm': '(?:[0]?[1-9]|[1][012])',
            'DD': '(?:' + $.fn.datepicker.dates[language].days.join('|') + ')',
            'D': '(?:' + $.fn.datepicker.dates[language].daysShort.join('|') + ')',
            'MM': '(?:' + $.fn.datepicker.dates[language].months.join('|') + ')',
            'M': '(?:' + $.fn.datepicker.dates[language].monthsShort.join('|') + ')',
            'yyyy': '(?:(?:[1]{1}[0-9]{1}[0-9]{1}[0-9]{1})|(?:[2]{1}[0-9]{3}))(?![[0-9]])',
            'yy': '(?:(?:[0-9]{1}[0-9]{1}))(?![[0-9]])'
        };
    };
    var regexpForDateFormat = function (format, language) {
        var re = format, map = regexpMap(language), i;
        i = 0;
        angular.forEach(map, function (v, k) {
            re = re.split(k).join('${' + i + '}');
            i++;
        });
        i = 0;
        angular.forEach(map, function (v) {
            re = re.split('${' + i + '}').join(v);
            i++;
        });
        return new RegExp('^' + re + '$', ['i']);
    };
    return {
        restrict: 'A',
        require: '?ngModel',
        link: function (scope, element, attrs, controller) {
            var options = angular.extend({ autoclose: true, todayBtn: true, todayHighlight: true, clearBtn: false }), type = attrs.dateType || options.type || 'date';
            angular.forEach([
                    'format',
                    'weekStart',
                    'calendarWeeks',
                    'startDate',
                    'endDate',
                    'daysOfWeekDisabled',
                    'autoclose',
                    'startView',
                    'minViewMode',
                    'todayBtn',
                    'todayHighlight',
                    'keyboardNavigation',
                    'language',
                    'forceParse'
            ], function (key) {
                if (angular.isDefined(attrs[key]))
                    options[key] = attrs[key];
            });
            var language = 'en', readFormat = attrs.dateFormat || options.format || 'dd-M-yyyy', format = readFormat, dateFormatRegexp = regexpForDateFormat(format, language);
            //attrs.dateFormat || options.format || $.fn.datepicker.dates[language] && $.fn.datepicker.dates[language].format ||
            if (controller) {
                controller.$formatters.unshift(function (modelValue) {
                    if (type !== 'date') return modelValue;
                    if (!angular.isString(modelValue)) return modelValue;
                    if (modelValue === '') return modelValue;
                    if (moment(modelValue).isValid()) {
                        if (modelValue.match('Z$')) {
                            return moment(modelValue).utc().format('DD-MMM-YYYY');
                        } else {
                            return moment(modelValue).format('DD-MMM-YYYY');
                        }

                    }
                    return $.fn.datepicker.DPGlobal.parseDate(modelValue, $.fn.datepicker.DPGlobal.parseFormat(readFormat), language);
                });
                controller.$parsers.unshift(function (viewValue) {
                    if (!viewValue) {
                        controller.$setValidity('date', true);
                        return null;
                    } else if (type === 'date' && angular.isDate(viewValue)) {
                        controller.$setValidity('date', true);
                        return viewValue;
                    } else if (angular.isString(viewValue) && dateFormatRegexp.test(viewValue)) {
                        controller.$setValidity('date', true);
                        if (isAppleTouch)
                            return new Date(viewValue);
                        return type === 'string' ? viewValue : $.fn.datepicker.DPGlobal.parseDate(viewValue, $.fn.datepicker.DPGlobal.parseFormat(format), language);
                    } else {
                        controller.$setValidity('date', false);
                        return undefined;
                    }
                });
                controller.$render = function () {
                    if (isAppleTouch) {
                        var date = controller.$viewValue ? $.fn.datepicker.DPGlobal.formatDate(controller.$viewValue, $.fn.datepicker.DPGlobal.parseFormat(format), language) : '';
                        element.val(date);
                        return date;
                    }
                    if (!controller.$viewValue)
                        element.val('');
                    return element.datepicker('update', controller.$viewValue);
                };
            }
            if (isAppleTouch) {
                element.prop('type', 'date').css('-webkit-appearance', 'textfield');
            } else {
                if (controller) {
                    element.on('changeDate', function (ev) {
                        scope.$apply(function () {
                            controller.$setViewValue(type === 'string'
                                ? element.val()
                                : new Date(moment(ev.date.valueOf()).utc().subtract('m', moment().zone()).valueOf()));
                        });
                    });
                }
                element.datepicker(angular.extend(options, {
                    format: format,
                    language: language
                }));
                scope.$on('$destroy', function () {
                    var datepicker = element.data('datepicker');
                    if (datepicker) {
                        datepicker.picker.remove();
                        element.data('datepicker', null);
                    }
                });
            }
            var component = element.siblings('[data-toggle="datepicker"]');
            if (component.length) {
                component.on('click', function () {
                    element.trigger('focus');
                });
            }
        }
    };
}));

//#endregion

//#region factory ($csnotify, $csfactory, $csStopWatch)

(csapp.factory("$csfactory", ["$csConstants", function (consts) {

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

    return {
        isNullOrEmptyArray: isNullOrEmptyArray,
        isNullOrEmptyString: isNullOrEmptyString,
        isNullOrEmptyGuid: isNullOrEmptyGuid,
        getDefaultGuid: getDefaultGuid,
        isEmptyObject: isEmptyObject
    };
}]));

(csapp.factory('$csnotify', function () {

    var success = function (messege) {
        alertify.success(messege);
    };

    var log = function (messege, isSticky) {
        if (isSticky) {
            alertify.log(messege, '', 0);
        } else {
            alertify.log(messege);
        }
    };

    var error = function (messege) {
        alertify.error(messege);
    };

    return {
        success: success,
        error: error,
        log: log
    };
}));

(csapp.factory('$Validations', function () {

    var required = function () {
        return 'Required field';
    };

    var validName = function (val) {
        return 'Enter valid ' + val;
    };

    var format = function (frmt) {
        return 'Accepts only ' + frmt + ' format';
    };

    var equalvalue = function (val) {
        return 'Length should be equal to' + val;
    };

    var onlyNumbers = function () {
        return 'Accept only numbers. ';
    };

    var graterThan = function (property, value) {
        return property + ' must be greater than ' + value;
    };

    var maxlength = function (val) {

        return 'Exceeds maximum size ( ' + val + ' )';
    };

    var minlength = function (val) {

        return 'Minimum required length is ' + val;
    };

    var onlytext = function () {
        return 'Accept characters only.';
    };

    var mandatory = function () {
        return '';
    };

    var specialChar = function () {
        return 'Special characters not allowed';
    };

    var pattern = function (val) {
        return 'Only number with ' + val + ' digits.';
    };
    var mobile = function () {
        return 'Accept 10 digit mobile number.';
    };

    var alreadyExist = function (val) {
        return val + ' already exist.';
    };

    var maxAmt = function (maxamt) {
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
}));

(csapp.factory('csStopWatch', ["$timeout", function ($timeout) {
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
}]));

//#endregion

//#region filters (bytes, fromNow, make-range, minLength)

(csapp.filter("minLength", ["$csfactory", function ($csfactory) {
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
}]));

(csapp.filter('csmakeRange', function () {
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
}));

(csapp.filter('bytes', function () {
    return function (bytes, precision) {
        if (bytes == 0 || isNaN(parseFloat(bytes)) || !isFinite(bytes)) return '-';
        if (typeof precision === 'undefined') precision = 1;
        var units = ['bytes', 'KB', 'MB', 'GB', 'TB', 'PB'],
            number = Math.floor(Math.log(bytes) / Math.log(1024));
        return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) + ' ' + units[number];
    };
}));

(csapp.filter('fromNow', function () {
    return function (dateString) {
        return moment(dateString).fromNow();
    };
}));

//#endregion

//#region input directives

// 2be: file, button, password

csapp.directive("csDateField", ["$csfactory", "$compile", "csInputHelper", "Logger", function ($csfactory, $compile, helper, logger) {

    var $log = logger.getInstance("csDateField");

    //options: label, placeholder, required, readonly, end-date, start-date, date-format, date-min-view-mode, days-of-week-disabled
    var fieldHtml = function (options) {
        return '<div class="input-append">' +
                '<input type="text" name="myfield" class="input-medium" data-ng-readonly="true" data-ng-model="ngModel" ' +
                    ' data-ng-required="options.required" data-date-min-view-mode="' + options.minViewMode + '" ' +
                    ' data-date-days-of-week-disabled="' + options.daysOfWeekDisabled + '" data-date-format="' + options.format + '" ' +
                    ' placeholder="{{options.placeholder}}" data-date-start-date="' + options.startDate + '"' +
                    ' data-date-end-date="' + options.endDate + '" bs-datepicker="" >' +
                '<button type="button" class="btn" data-toggle="datepicker"><i class="icon-calendar"></i></button> ' +
            '</div>';
    };

    var applyTemplate = function (options) {
        if (angular.isUndefined(options.template) || options.template === null) {
            return;
        }

        var tmpl = options.template.split(",").filter(function (str) { return str !== ''; });
        angular.forEach(tmpl, function (template) {
            if (template.length < 1) return;
            switch (template) {
                case "MonthPicker":
                    options.minViewMode = "months";
                    break;
                case "YearPicker":
                    options.minViewMode = "years";
                    break;
                case "future":
                    options.startDate = "+0";
                    break;
                case "past":
                    options.endDate = "+0";
                    break;
                default:
                    $log.error(template + " is not defined.");
            }
            return;
        });
    };

    var manageViewMode = function (options) {
        //month/year modes
        if ($csfactory.isNullOrEmptyString(options.minViewMode)) {
            options.minViewMode = 0;
        } else if (options.minViewMode === "1" || options.minViewMode === "months") {
            options.minViewMode = 1;
        } else if (options.minViewMode === "2" || options.minViewMode === "years") {
            options.minViewMode = 2;
        } else {
            options.minViewMode = 0;
        }

        //format
        if (options.minViewMode === 0) {
            options.format = "dd-M-yyyy";
        } else if (options.minViewMode === 1) {
            options.format = "M-yyyy";
        } else {
            options.format = ".yyyy";
        }

        //min date        
        if ($csfactory.isNullOrEmptyString(options.startDate)) {
            if (options.minViewMode === 0) {
                options.startDate = '01-Jan-1800';
            } else if (options.minViewMode === 1) {
                options.startDate = 'Jan-1800';
            } else {
                options.startDate = '.1800';
            }
        }

        //max date
        if ($csfactory.isNullOrEmptyString(options.endDate)) {
            if (options.minViewMode === 0) {
                options.endDate = '31-Dec-2400';
            } else if (options.minViewMode === 1) {
                options.endDate = 'Dec-2400';
            } else {
                options.endDate = '.2400';
            }

        }
    };

    var validateOptions = function (options) {
        if ($csfactory.isNullOrEmptyString(options.label)) {
            options.label = "Date";
        }

        if ($csfactory.isNullOrEmptyString(options.daysOfWeekDisabled)) {
            options.daysOfWeekDisabled = '[]';
        }
    };

    var compileFunction = function (element) {
        return function (scope) {
            applyTemplate(scope.options);
            manageViewMode(scope.options);
            validateOptions(scope.options);
            element.html(helper.getFieldHtml(fieldHtml, scope.options));
            $compile(element.contents())(scope);
        };
    };

    return {
        scope: { options: '=', ngModel: '=' },
        required: ['ngModel', '^form'],
        restrict: 'E',
        compile: compileFunction
    };
}]);

csapp.directive("csButton", ["$csfactory", "$compile", function ($csfactory, $compile) {

    var getOptionsByType = function (type) {
        var options = {};
        switch (type) {
            case "save":
                options.class = "btn btn-success";
                options.caption = 'Save';
                break;
            case "cancel":
                options.class = "btn btn-warning";
                options.caption = 'Cancel';
                break;
            case "reset":
                options.class = "btn btn-primary";
                options.caption = 'Reset';
                break;
            case "ok":
                options.class = "btn btn-info";
                options.caption = 'Ok';
                break;
            case "close":
                options.class = "btn btn-danger";
                options.caption = 'Close';
                break;
            case "delete":
                options.class = "btn icon-trash";
                break;
            case "edit":
                options.class = "btn  icon-edit-sign";

                break;
            case "add":
                options.class = "btn icon-plus";
                break;
            default:
                throw "Invalid type : " + type;
        }
        return options;
    };

    var generateHtml = function (scope) {
        return '<button class="{{options.class}}" ng-click="$parent.' + scope.ngClick + '" data-ng-disabled="ngDisabled" > {{options.caption}} </button>';
    };

    var linkFunction = function (scope, element) {
        console.log(scope);
        scope.options = getOptionsByType(scope.type);
        element.html(generateHtml(scope));
        $compile(element.contents())(scope);
    };

    return {
        restrict: 'E',
        link: linkFunction,
        scope: { type: '@', ngClick: '@', ngDisabled: '=' },
        replace: true
    };
}]);

csapp.directive("csInput", ["$csfactory", "$compile", "Logger", "textareaFactory", "numberFactory", "textFactory", "emailFactory", "checkboxFactory", "radioButtonFactory", "selectFactory", function ($csfactory, $compile, logManager, textarea, numberBox, text, email, checkbox, radio, select) {

    var getFactoryByType = function (type) {
        switch (type) {
            case "textarea":
                return textarea;
            case "uint":
            case "int":
            case "ulong":
            case "long":
            case "decimal":
                return numberBox;
            case "text":
                return text;
            case "email":
                return email;
            case "checkbox":
                return checkbox;
            case "radio":
                return radio;
            case "select":
                return select;
            default:
                throw "Invalid type specification in csInput directive : " + type;
        }
    };

    var fieldLayoutHtml = function (factory, scope) {

        var html = '<div ng-form="myform">' +
            '<div class="control-group" class="{{options.class}}" >' +
            '<div class="control-label">{{options.label}} <span style="color:red">{{options.required ? "*" : ""}} </span></div>' +
            '<div class="controls">';

        html += factory.htmlTemplate(scope);

        html += '<div class="field-validation-error" data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty"> ' +
                    '<div data-ng-show="myform.myfield.$error.required ">{{options.label}} is required!!!</div>' +
                    '<div data-ng-show="myform.myfield.$error.pattern">{{options.patternMessage}}</div>' +
                    '<div data-ng-show="myform.myfield.$error.minlength">{{options.label}} should have atleast {{options.minlength}} character/s.</div>' +
                    '<div data-ng-show="myform.myfield.$error.maxlength">{{options.label}} can have maximum {{options.maxlength}} character/s.</div>' +
                    '<div data-ng-show="myform.myfield.$error.min">{{options.label}} cannot have value less than {{options.min}}</div>' +
                    '<div data-ng-show="myform.myfield.$error.max">{{options.label}} cannot have value greater than {{options.max}}</div>' +
                '</div>';

        html += '</div>' + //controls
            '</div>' + // control-group
            '</div>'; //ng-form;

        return html;
    };

    var linkFunction = function (scope, element) { //scope, element, attr, ctrl
        console.log("executing link function...");
        //scope.$digest();
        var factory = getFactoryByType(scope.options.type);
        factory.checkOptions(scope);
        element.html(fieldLayoutHtml(factory, scope));
        $compile(element.contents())(scope);
    };

    return {
        restrict: 'E',
        scope: { options: '=', ngModel: '=', ngChange: '&' },
        require: ['ngModel', '^form'],
        link: linkFunction
    };
}]);


// all checked

csapp.directive("csTemplate", [function () {

    var getTemplate = function () {
        var html = '<div ng-form="myform">' +
                    '<div class="control-group" class="{{options.class}}" >' +
                    '<div class="control-label">{{options.label}} <span style="color:red">{{options.required ? "*" : ""}} </span></div>' +
                    '<div class="controls">';

        html += '<div ng-transclude></div>';

        html += '<div class="field-validation-error" data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty"> ' +
            '<div data-ng-show="myform.myfield.$error.required ">{{options.label}} is required!!!</div>' +
            '<div data-ng-show="myform.myfield.$error.pattern">{{options.patternMessage}}</div>' +
            '<div data-ng-show="myform.myfield.$error.minlength">{{options.label}} should have atleast {{options.minlength}} character/s.</div>' +
            '<div data-ng-show="myform.myfield.$error.maxlength">{{options.label}} can have maximum {{options.maxlength}} character/s.</div>' +
            '<div data-ng-show="myform.myfield.$error.min">{{options.label}} cannot have value less than {{options.min}}</div>' +
            '<div data-ng-show="myform.myfield.$error.max">{{options.label}} cannot have value greater than {{options.max}}</div>' +
        '</div>';

        html += '</div>' + //controls
            '</div>' + // control-group
            '</div>'; //ng-form;

        return html;
    };

    return {
        scope: { options: '=' },
        restrict: 'E',
        transclude: true,
        template: getTemplate
    };
}]);

csapp.directive("csTextField", ["$compile", "Logger", function ($compile, logManager) {

    //options: label, autofocus,  placeholder, required, readonly, minlength, maxlength
    var $log = logManager.getInstance("csTextField");

    var validateOptions = function (options) {
        // manage lengths
        options.minlength = options.length || options.minlength || 0;
        options.maxlength = options.length || options.maxlength || 250;
        options.minlength = (options.minlength >= 0 && options.minlength <= 250) ? options.minlength : 0;
        options.maxlength = (options.maxlength >= 0 && options.maxlength <= 250) ? options.maxlength : 250;
        if (options.minlength > options.maxlength) {
            var error = "minlength(" + options.minlength + ") cannot be greather than maxlength(" + options.maxlength + ").";
            $log.error(error); throw error;
        }

        options.label = options.label || "Text";
        options.patternMessage = options.patternMessage || ("Input is not matching with pattern : " + options.pattern);
        options.readonly = options.readonly || options.disabled || false;
    };

    var applyTemplates = function (options) {
        if (angular.isUndefined(options.template) || options.template === null) {
            return;
        }

        var tmpl = options.template.split(",").filter(function (str) { return str !== ''; });
        angular.forEach(tmpl, function (template) {
            if (template.length < 1) return;

            switch (template) {
                case "alphanum":
                    options.pattern = "/^[a-zA-Z0-9 ]*$/";
                    options.patternMessage = "Value contains non-numeric character/s.";
                    break;
                case "alphabates":
                    options.pattern = "/^[a-zA-Z ]*$/";
                    options.patternMessage = "Value contains non-alphabtical character/s.";
                    break;
                case "numeric":
                    options.pattern = "/^[0-9]*$/";
                    options.patternMessage = "Value contains non-numeric character/s.";
                    break;
                case "phone":
                    options.length = 10;
                    options.pattern = "/^[0-9]{10}$/";
                    options.patternMessage = "Phone number must contain 10 digits.";
                    break;
                case "pan":
                    options.pattern = "/^([A-Z]{5})(\d{4})([a-zA-Z]{1})$/";
                    options.patternMessage = "Value not matching with PAN Pattern e.g. ABCDE1234A";
                default:
                    $log.error(template + " is not defined");
            }
        });
    };

    var templateFunction = function (scope) {
        var html = '<cs-template options="options">' +
                        '<input type="text" name="myfield" placeholder="{{options.placeholder}}" ng-required="options.required" ' +
                            ' ng-pattern="{{options.pattern}}" ng-minlength="{{options.minlength}}" ng-maxlength="{{options.maxlength}}" ' +
                            ' ng-readonly="options.readonly" autofocus="options.autofocus" data-ng-change="ngChange()" ' +
                            ' autocomplete="off" data-ng-model="$parent.$parent.' + scope.ngModel + '"/>' +
                    '</cs-template>';
        return html;
    };

    var linkFunction = function (scope, element) {
        applyTemplates(scope.options);
        validateOptions(scope.options);
        var template = templateFunction(scope);
        element.replaceWith($compile(template)(scope));
    };

    return {
        scope: { options: '=', ngModel: '@csModel', ngChange: '&csChange' },
        restrict: 'E',
        link: linkFunction
    };
}]);

csapp.directive("csNumberField", ["$csfactory", "$compile", "Logger", function ($csfactory, $compile, logManager) {

    var $log = logManager.getInstance("csNumberField");

    //options: label, autofocus,  placeholder, required, readonly, minlength, maxlength, min, max
    var fieldHtml = function (scope) {
        var html = '<cs-template options="options">';
        html += '<input type="number" name="myfield" placeholder="{{options.placeholder}}" ng-required="options.required" ' +
            ' ng-pattern="{{options.pattern}}" ng-minlength="{{options.minlength}}"  ng-maxlength="{{options.maxlength}}" ' +
            ' step="any" ng-readonly="options.readonly"  max="{{options.max}}" min="{{options.min}}" ' +
            ' autofocus="options.autofocus" autocomplete="off" data-ng-change="ngChange()" ' +
            ' data-ng-model="$parent.$parent.' + scope.csModel + '"/>';
        html += '</cs-template>';
        return html;
    };

    var applyTemplates = function (options) {
        switch (options.type) {
            case "uint":
                options.min = 0;
            case "int":
                options.maxlength = 6;
                break;
            case "ulong":
                options.min = 0;
            case "long":
                options.maxlength = 12;
                break;
            case "decimal":
                options.maxlength = 19;
            default:
                $log.error(options.type + " is not defined");
        }
    };

    var validateOptions = function (scope) {
        applyTemplates(scope.options);
        scope.options.minlength = scope.options.length || scope.options.minlength || 0;
        scope.options.maxlength = scope.options.length || scope.options.maxlength || 18;
        scope.options.minlength = (scope.options.minlength >= 0 && scope.options.minlength <= 18) ? scope.options.minlength : 0;
        scope.options.maxlength = (scope.options.maxlength >= 0 && scope.options.maxlength <= 18) ? scope.options.maxlength : 18;
        if (parseInt(scope.options.minlength) > parseInt(scope.options.maxlength)) {
            var error = "minlength(" + scope.options.minlength + ") cannot be greather than maxlength(" + scope.options.maxlength + ").";
            throw error;
        }
        scope.options.label = scope.options.label || "Number";
        scope.options.patternMessage = scope.options.patternMessage || "Value cannot have non-numeric character/s.";
    };

    var linkFunction = function (scope, element) {
        applyTemplates(scope.options);
        validateOptions(scope);
        var template = fieldHtml(scope);
        element.replaceWith($compile(template)(scope));
    };

    return {
        scope: { options: '=', csModel: '@csModel', ngChange: '&csChange' },
        restrict: 'E',
        link: linkFunction
    };
}]);

csapp.directive("csInputSuffix", function () {

    var linkFunction = function (scope, element, attrs, ctr) {
        ctr.$parsers.unshift(function (value) {

            var isValid = value !== "";

            ctr.$setValidity("required", isValid);
            if (!isValid) {
                return undefined;
            }

            if (value.indexOf(attrs.suffix) < 0) {
                value = value + attrs.suffix;
            }

            return value;
        });

        ctr.$formatters.unshift(function (value) {
            value = value || "";
            return value.replace(attrs.suffix, "");
        });
    };

    return {
        require: 'ngModel',
        link: linkFunction
    };
});

csapp.directive("csEmailField", ["$csfactory", "$compile", function ($csfactory, $compile) {

    //options:label, placeholder, pattern, minlength, maxlength, readonly, required
    var fieldHtml = function (scope) {
        var hasSuffix = angular.isDefined(scope.options.suffix) && scope.options.suffix !== null && scope.options.suffix.length > 0;

        var string = '<cs-template options="options">';

        string += '<div class="input-prepend input-append">' +
            '<span class="add-on"><i class="icon-envelope"></i></span>' +
            '<input type="email" name="myfield" placeholder="{{options.placeholder}}" ng-required="options.required" ' +
            'ng-pattern="{{options.pattern}}" ng-minlength="{{options.minlength}}" data-ng-change="ngChange()" ' +
            'ng-maxlength="{{options.maxlength}}" data-ng-model="$parent.$parent.' + scope.csModel + '" ';
        if (hasSuffix) {
            string += 'class="input-medium" cs-input-suffix suffix="{{options.suffix}}" />';
        } else {
            string += 'class="input-large" />';
        }

        if (hasSuffix) {
            string += '<span class="add-on">{{options.suffix}}</span>';
        }

        string += '</div>';
        string += '</cs-template>';
        return string;
    };

    var validateOptions = function (scope) {
        scope.options.label = scope.options.label || "Email";
        scope.options.placeholder = scope.options.placeholder || "Enter Email";
        scope.options.minlength = scope.options.suffix ? scope.options.suffix.length + 4 : 8;
        scope.options.maxlength = 250;
        scope.options.patternMessage = "Input is not a valid email address.";
    };

    var linkFunction = function (scope, element) {
        validateOptions(scope);
        var template = fieldHtml(scope);
        element.replaceWith($compile(template)(scope));
    };

    return {
        scope: { options: '=', csModel: '@csModel', ngChange: '&csChange' },
        restrict: 'E',
        link: linkFunction
    };
}]);

csapp.directive("csTextareaField", ["$csfactory", "$compile", function ($csfactory, $compile) {

    //options:label, placeholder, pattern, minlength, maxlength, readonly, required
    var fieldHtml = function (scope) {
        var string = '<cs-template options="options">';
        string += '<textarea type="text" name="myfield" placeholder="{{options.placeholder}}" ng-required="options.required" ' +
                    'ng-readonly="options.readonly" ng-minlength="{{options.minlength}}" ng-maxlength="{{options.maxlength}}" ' +
                    'rows="{{options.rows}}" cols="{{options.columns}}" autofocus="options.autofocus" ' +
                    'data-ng-change="ngChange()"' +
                    ' data-ng-model="$parent.$parent.' + scope.csModel + '"/>';
        string += '</cs-template>';
        return string;
    };

    var validateOptions = function (scope) {
        scope.options.label = scope.options.label || "Description";
        scope.options.rows = scope.options.rows || 2;
        scope.options.columns = scope.options.columns || 120;
        scope.options.placeholder = scope.options.placeholder || "Describe in detail";
        scope.options.maxlength = scope.options.maxlength || 250;
        scope.options.readonly = scope.options.readonly || scope.options.disabled;
    };

    var linkFunction = function (scope, element) {
        validateOptions(scope);
        var template = fieldHtml(scope);
        element.replaceWith($compile(template)(scope));
    };

    return {
        scope: { options: '=', csModel: '@csModel', ngChange: '&csChange' },
        restrict: 'E',
        link: linkFunction
    };
}]);

csapp.directive("csCheckboxField", ["$csfactory", "$compile", function ($csfactory, $compile) {
    //options: label, required, checked
    var fieldHtml = function (scope) {

        var string = '<cs-template options="options">';

        string += '<input type="checkbox" name="myfield"  ng-required="options.required" ' +
           ' data-ng-model="$parent.$parent.' + scope.csModel + '" ' +
            'data-ng-click="ngClick()" ' +
            'data-ng-change="ngChange()"/>';

        string += '</cs-template>';

        return string;

    };

    var validateOptions = function (scope) {
        scope.options.label = scope.options.label || "CheckBox";
    };

    var linkFunction = function (scope, element) {

        validateOptions(scope);
        var template = fieldHtml(scope);
        element.replaceWith($compile(template)(scope));

    };

    return {
        scope: { options: '=', csModel: '@csModel', ngChange: '&csChange', ngClick: '&csClick' },
        restrict: 'E',
        link: linkFunction
    };
}]);

csapp.directive("csEnumField", ["$csfactory", "$compile", function ($csfactory, $compile) {
    //options:label, required, values 
    var fieldHtml = function (scope) {

        var string = '<cs-template options="options">';

        string += '<select data-ui-select2="" class="input-large" ng-required="options.required"  ' +
                    'data-ng-model="$parent.$parent.' + scope.csModel + '" name="myfield" ' +
                    'data-ng-change="ngChange()">' +
                    ' <option value=""></option> ' +
                    ' <option data-ng-repeat="row in options.values" value="{{row}}">{{row}}</option>' +
                '</select> ';

        string += '</cs-template>';

        return string;
    };

    var validateOptions = function (scope) {
        scope.options.label = scope.options.label || "SelectBox";
    };

    var linkFunction = function (scope, element) {
        validateOptions(scope);
        var template = fieldHtml(scope);
        element.replaceWith($compile(template)(scope));
    };

    return {
        scope: { options: '=', csModel: '@csModel', ngChange: '&csChange' },
        restrict: 'E',
        link: linkFunction
    };
}]);

csapp.directive("csSelectField", ["$csfactory", "$compile", function ($csfactory, $compile) {

    //options:label, required 
    var fieldHtml = function (scope) {

        var string = '<cs-template options="options">';

        string += '<select  data-ng-model="$parent.$parent.' + scope.csModel + '" ' +
                 'name="myfield" ' +
                 'ng-required="options.required" ' +
                 'data-ng-change="ngChange()">' +
                    ' <option value=""></option> ' +
                    ' <option data-ng-repeat="' + scope.options.csRepeat + '"value="{{' + scope.options.valueField + '}}">{{' + scope.options.textField + '}}</option>' +
                '</select> ';

        string += '</cs-template>';

        return string;
    };


    var validateOptions = function (scope) {
        scope.options.label = scope.options.label || "SelectBox";
    };

    var linkFunction = function (scope, element) {
        //set params on options
        scope.options.csRepeat = "row in $parent.$parent." + scope.csRepeat.substring(1, scope.csRepeat.length - 1);
        scope.options.textField = scope.textField ? "row." + scope.textField : "row";
        scope.options.valueField = scope.valueField ? "row." + scope.valueField : "row";
        //console.log(scope.options.csRepeat);

        validateOptions(scope);
        var template = fieldHtml(scope);
        element.replaceWith($compile(template)(scope));

    };

    return {
        scope: {
            options: '=',
            csModel: '@csModel',
            ngChange: '&csChange',
            csRepeat: '@',
            textField: '@',
            valueField: '@'
        },
        restrict: 'E',
        link: linkFunction
    };
}]);

csapp.directive("csRadioField", ["$csfactory", "$compile", function ($csfactory, $compile) {
    //$scope.gender = { label: "Gender", required: true, textField: "text2", options: [{ text2: "Male", value: "yes" }, { text2: "Female", value: "no" }] };
    var fieldHtml = function (scope) {

        var string = '<cs-template options="options">';

        string += '<div class="radio" ng-repeat="(key, record) in options.options">' +
                    '<label> <input type="radio" name="myfield" value="{{' + scope.options.valueField + '}}" ' +
                                'data-ng-model="$parent.$parent.$parent.' + scope.csModel + '" ' +
                                'data-ng-change="ngChange()" ' +
                                'ng-required="options.required"  />{{' + scope.options.textField + '}}' +
                    '</label>' +
                  '</div>';

        string += '</cs-template>';

        return string;
    };

    var validateOptions = function (scope) {

        scope.options.label = scope.options.label || "Description";
        scope.options.textField = scope.textField ? "record." + scope.textField : "record";
        scope.options.valueField = scope.valueField ? "record." + scope.valueField : "record";

    };

    var linkFunction = function (scope, element) {

        validateOptions(scope);
        var template = fieldHtml(scope);
        element.replaceWith($compile(template)(scope));

    };

    return {
        scope: {
            options: '=',
            csModel: '@csModel',
            ngChange: '&csChange',
            textField: '@',
            valueField: '@'
        },
        restrict: 'E',
        link: linkFunction
    };
}]);
//#endregion

//#region logger
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
//#endregion
