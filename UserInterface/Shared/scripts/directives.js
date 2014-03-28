
//#region switch-buttons 3 directives

csapp.directive('btnSwitch', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        //templateUrl: 'switcher.html',
        template: '<span class="btn boolean"><span class="on btn-primary">Yes</span>' +
            '<span class="off btn-default">No</span></span>',
        replace: true,
        link: function(scope, element, attrs, ngModel) {

            // Specify how UI should be updated
            ngModel.$render = function() {
                render();
            };

            var render = function() {
                var val = ngModel.$viewValue;

                var open = angular.element(element.children()[0]);
                open.removeClass(val ? 'hide' : 'show');
                open.addClass(val ? 'show' : 'hide');

                var closed = angular.element(element.children()[1]);
                closed.removeClass(val ? 'show' : 'hide');
                closed.addClass(val ? 'hide' : 'show');
            };

            // Listen for the button click event to enable binding
            element.bind('click', function() {
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
            } // do nothing if no ng-model

            // Initial render
            render();
        }
    };
});

csapp.directive('switchyesno', function() {
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
});

csapp.directive("csswitch", function() {

    var linkfunction = function(scope) {

        scope.clickbtn = function(namevalue) {
            scope.ngbind = namevalue.Value;
        };

        scope.$watch('ngbind', function() {
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
});

//#endregion

//#region spinner & bs-datepicker

csapp.directive("spinner", function() {
    return {
        restrict: 'C',
        link: function(scope, element) {
            element.bind("mouseenter", function() {
                element.addClass("icon-spin");
            });
            element.bind("mouseleave", function() {
                element.removeClass("icon-spin");
            });
        }
    };
});

csapp.directive('bsDatepicker', function() {
    var isAppleTouch = /(iP(a|o)d|iPhone)/g.test(navigator.userAgent);
    var regexpMap = function(language) {
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
    var regexpForDateFormat = function(format, language) {
        var re = format, map = regexpMap(language), i;
        i = 0;
        angular.forEach(map, function(v, k) {
            re = re.split(k).join('${' + i + '}');
            i++;
        });
        i = 0;
        angular.forEach(map, function(v) {
            re = re.split('${' + i + '}').join(v);
            i++;
        });
        return new RegExp('^' + re + '$', ['i']);
    };
    return {
        restrict: 'A',
        require: '?ngModel',
        link: function(scope, element, attrs, controller) {
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
                ], function(key) {
                    if (angular.isDefined(attrs[key]))
                        options[key] = attrs[key];
                });
            var language = 'en', readFormat = attrs.dateFormat || options.format || 'dd-M-yyyy', format = readFormat, dateFormatRegexp = regexpForDateFormat(format, language);
            //attrs.dateFormat || options.format || $.fn.datepicker.dates[language] && $.fn.datepicker.dates[language].format ||
            if (controller) {
                controller.$formatters.unshift(function(modelValue) {
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
                controller.$parsers.unshift(function(viewValue) {
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
                controller.$render = function() {
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
                    element.on('changeDate', function(ev) {
                        scope.$apply(function() {
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
                scope.$on('$destroy', function() {
                    var datepicker = element.data('datepicker');
                    if (datepicker) {
                        datepicker.picker.remove();
                        element.data('datepicker', null);
                    }
                });
            }
            var component = element.siblings('[data-toggle="datepicker"]');
            if (component.length) {
                component.on('click', function() {
                    element.trigger('focus');
                });
            }
        }
    };
});

//#endregion



csapp.directive('cspagination', function () {

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


});

////#region input-directives

//csapp.directive("csTextField", ["$csfactory", "$compile", "csInputHelper", "Logger", function ($csfactory, $compile, helper, logManager) {

//    var $log = logManager.getInstance("csTextField");

//    //options: label, autofocus,  placeholder, required, readonly, minlength, maxlength
//    var fieldHtml = function () {
//        return '<input type="text" name="myfield" placeholder="{{options.placeholder}}" ng-required="options.required" ' +
//            'ng-pattern="{{options.pattern}}" ng-minlength="{{options.minlength}}" ng-maxlength="{{options.maxlength}}" ' +
//            'ng-readonly="options.readonly" autofocus="options.autofocus" ng-change="ngChange"  autocomplete="off" data-ng-model="ngModel"/>';
//    };

//    var applyTemplates = function (options) {
//        if (angular.isUndefined(options.template) || options.template === null) {
//            return;
//        }

//        var tmpl = options.template.split(",").filter(function (str) { return str !== ''; });
//        angular.forEach(tmpl, function (template) {
//            if (template.length < 1) return;

//            switch (template) {
//                case "alphanum":
//                    options.pattern = "/^[a-zA-Z0-9 ]*$/";
//                    options.patternMessage = "Value contains non-numeric character/s.";
//                    break;
//                case "alphabates":
//                    options.pattern = "/^[a-zA-Z ]*$/";
//                    options.patternMessage = "Value contains non-alphabtical character/s.";
//                    break;
//                case "numeric":
//                    options.pattern = "/^[0-9]*$/";
//                    options.patternMessage = "Value contains non-numeric character/s.";
//                    break;
//                case "phone":
//                    options.length = 10;
//                    options.pattern = "/^[0-9]{10}$/";
//                    options.patternMessage = "Phone number must contain 10 digits.";
//                    break;
//                case "pan":
//                    options.pattern = "/^([A-Z]{5})(\d{4})([a-zA-Z]{1})$/";
//                    options.patternMessage = "Value not matching with PAN Pattern e.g. ABCDE1234A";
//                default:
//                    $log.error(template + " is not defined");
//            }
//        });
//    };

//    var validateOptions = function (options) {
//        // manage lengths
//        options.minlength = options.length || options.minlength || 0;
//        options.maxlength = options.length || options.maxlength || 250;
//        options.minlength = (options.minlength >= 0 && options.minlength <= 250) ? options.minlength : 0;
//        options.maxlength = (options.maxlength >= 0 && options.maxlength <= 250) ? options.maxlength : 250;
//        if (options.minlength > options.maxlength) {
//            var error = "minlength(" + options.minlength + ") cannot be greather than maxlength(" + options.maxlength + ").";
//            $log.error(error);
//            throw error;
//        }

//        options.label = options.label || "Text";
//        options.patternMessage = options.patternMessage || ("Input is not matching with pattern : " + options.pattern);
//        options.readonly = options.readonly || options.disabled || false;
//    };

//    var compileFunction = function (element) {
//        return function (scope) {
//            applyTemplates(scope.options);
//            validateOptions(scope.options);
//            element.html(helper.getFieldHtml(fieldHtml, scope.options));
//            $compile(element.contents())(scope);
//        };
//    };

//    return {
//        scope: { options: '=', ngModel: '=', ngChange: '=' },
//        required: ['ngModel', '^form'],
//        restrict: 'E',
//        compile: compileFunction
//    };
//}]);

//csapp.directive("csNumberField", ["$csfactory", "$compile", "csInputHelper", "Logger", function ($csfactory, $compile, helper, logManager) {

//    var $log = logManager.getInstance("csNumberField");

//    //options: label, autofocus,  placeholder, required, readonly, minlength, maxlength, min, max
//    var fieldHtml = function () {
//        return '<input type="number" name="myfield" placeholder="{{options.placeholder}}" ng-required="options.required" ' +
//            ' ng-pattern="{{options.pattern}}" ng-minlength="{{options.minlength}}"  ng-maxlength="{{options.maxlength}}" ' +
//            ' step="any" ng-readonly="options.readonly"  max="{{options.max}}" min="{{options.min}}" autofocus="options.autofocus" ' +
//            ' data-ng-model="ngModel" />';
//    };

//    var applyTemplates = function (options) {
//        if (angular.isUndefined(options.template) || options.template === null) {
//            return;
//        }

//        var tmpl = options.template.split(",").filter(function (str) { return str !== ''; });
//        angular.forEach(tmpl, function (template) {
//            if (template.length < 1) return;

//            switch (template) {
//                case "positive":
//                    options.min = 0;
//                    break;
//                case "int":
//                    options.maxlength = 6;
//                    break;
//                case "uint":
//                    options.min = 0;
//                    options.maxlength = 6;
//                    break;
//                case "long":
//                    options.maxlength = 12;
//                    break;
//                case "ulong":
//                    options.min = 0;
//                    options.maxlength = 12;
//                    break;
//                case "decimal":
//                    options.maxlength = 19;
//                default:
//            }
//        });
//    };

//    var validateOptions = function (options) {
//        // manage lengths
//        options.minlength = options.length || options.minlength || 0;
//        options.maxlength = options.length || options.maxlength || 18;
//        options.minlength = (options.minlength >= 0 && options.minlength <= 18) ? options.minlength : 0;
//        options.maxlength = (options.maxlength >= 0 && options.maxlength <= 18) ? options.maxlength : 18;
//        if (options.minlength > options.maxlength) {
//            var error = "minlength(" + options.minlength + ") cannot be greather than maxlength(" + options.maxlength + ").";
//            $log.error(error);
//            throw error;
//        }

//        options.label = options.label || "Number";
//        options.patternMessage = options.patternMessage || "Value cannot have non-numeric character/s.";
//    };

//    var compileFunction = function (element) {
//        return function (scope) {
//            applyTemplates(scope.options);
//            validateOptions(scope.options);
//            var template = helper.getFieldHtml(fieldHtml, scope.options);
//            element.html(template);
//            $compile(element.contents())(scope);
//        };
//    };

//    return {
//        scope: { options: '=', ngModel: '=' },
//        require: ['ngModel', '^form'],
//        restrict: 'E',
//        compile: compileFunction,
//    };
//    //var linkFunction = function (scope) { //, element, attr, ctrl

//    //    scope.$watch('ngModel', function (newVal, oldVal) {
//    //        if (angular.isNumber(newVal) && !isNaN(newVal)) {
//    //            scope.ngModel = parseInt(newVal, 10);
//    //            return;
//    //        }

//    //        if (angular.isNumber(oldVal) && !isNaN(oldVal)) {
//    //            scope.ngModel = Math.abs(oldVal).toString().length === 1 ? null : parseInt(oldVal, 10);
//    //            return;
//    //        }
//    //    });
//    //};
//    //linkFunction.apply(this, arguments);

//}]);

//csapp.directive("csDateField", ["$csfactory", "$compile", "csInputHelper", "Logger", function ($csfactory, $compile, helper, logger) {

//    var $log = logger.getInstance("csDateField");

//    //options: label, placeholder, required, readonly, end-date, start-date, date-format, date-min-view-mode, days-of-week-disabled
//    var fieldHtml = function (options) {
//        return '<div class="input-append">' +
//                '<input type="text" name="myfield" class="input-medium" data-ng-readonly="true" data-ng-model="ngModel" ' +
//                    ' data-ng-required="options.required" data-date-min-view-mode="' + options.minViewMode + '" ' +
//                    ' data-date-days-of-week-disabled="' + options.daysOfWeekDisabled + '" data-date-format="' + options.format + '" ' +
//                    ' placeholder="{{options.placeholder}}" data-date-start-date="' + options.startDate + '"' +
//                    ' data-date-end-date="' + options.endDate + '" bs-datepicker="" >' +
//                '<button type="button" class="btn" data-toggle="datepicker"><i class="icon-calendar"></i></button> ' +
//            '</div>';
//    };

//    var applyTemplate = function (options) {
//        if (angular.isUndefined(options.template) || options.template === null) {
//            return;
//        }

//        var tmpl = options.template.split(",").filter(function (str) { return str !== ''; });
//        angular.forEach(tmpl, function (template) {
//            if (template.length < 1) return;
//            switch (template) {
//                case "MonthPicker":
//                    options.minViewMode = "months";
//                    break;
//                case "YearPicker":
//                    options.minViewMode = "years";
//                    break;
//                case "future":
//                    options.startDate = "+0";
//                    break;
//                case "past":
//                    options.endDate = "+0";
//                    break;
//                default:
//                    $log.error(template + " is not defined.");
//            }
//            return;
//        });
//    };

//    var manageViewMode = function (options) {
//        //month/year modes
//        if ($csfactory.isNullOrEmptyString(options.minViewMode)) {
//            options.minViewMode = 0;
//        } else if (options.minViewMode === "1" || options.minViewMode === "months") {
//            options.minViewMode = 1;
//        } else if (options.minViewMode === "2" || options.minViewMode === "years") {
//            options.minViewMode = 2;
//        } else {
//            options.minViewMode = 0;
//        }

//        //format
//        if (options.minViewMode === 0) {
//            options.format = "dd-M-yyyy";
//        } else if (options.minViewMode === 1) {
//            options.format = "M-yyyy";
//        } else {
//            options.format = ".yyyy";
//        }

//        //min date        
//        if ($csfactory.isNullOrEmptyString(options.startDate)) {
//            if (options.minViewMode === 0) {
//                options.startDate = '01-Jan-1800';
//            } else if (options.minViewMode === 1) {
//                options.startDate = 'Jan-1800';
//            } else {
//                options.startDate = '.1800';
//            }
//        }

//        //max date
//        if ($csfactory.isNullOrEmptyString(options.endDate)) {
//            if (options.minViewMode === 0) {
//                options.endDate = '31-Dec-2400';
//            } else if (options.minViewMode === 1) {
//                options.endDate = 'Dec-2400';
//            } else {
//                options.endDate = '.2400';
//            }

//        }
//    };

//    var validateOptions = function (options) {
//        if ($csfactory.isNullOrEmptyString(options.label)) {
//            options.label = "Date";
//        }

//        if ($csfactory.isNullOrEmptyString(options.daysOfWeekDisabled)) {
//            options.daysOfWeekDisabled = '[]';
//        }
//    };

//    var compileFunction = function (element) {
//        return function (scope) {
//            applyTemplate(scope.options);
//            manageViewMode(scope.options);
//            validateOptions(scope.options);
//            element.html(helper.getFieldHtml(fieldHtml, scope.options));
//            $compile(element.contents())(scope);
//        };
//    };

//    return {
//        scope: { options: '=', ngModel: '=' },
//        required: ['ngModel', '^form'],
//        restrict: 'E',
//        compile: compileFunction
//    };
//}]);

//csapp.directive("csRadioField", ["$csfactory", "$compile", "csInputHelper", function ($csfactory, $compile, helper) {
//    //$scope.gender = { label: "Gender", required: true, textField: "text2", options: [{ text2: "Male", value: "yes" }, { text2: "Female", value: "no" }] };
//    var fieldHtml = function (options) {
//        return '<div class="radio" ng-repeat="(key, record) in options.options">' +
//                    '<label> <input type="radio" name="myfield" value="{{' + options.valueField + '}}" ng-model="$parent.ngModel" ' +
//                        'ng-required="options.required"  />{{' + options.textField + '}}</label>' +
//                '</div>';
//    };

//    var checkOptions = function (options) {
//        options.label = options.label || "Description";
//        options.textField = "record." + (options.textField || "text");
//        options.valueField = "record." + (options.valueField || "value");
//    };

//    var compileFunction = function (element) {
//        return function (scope) {
//            checkOptions(scope.options);
//            element.html(helper.getFieldHtml(fieldHtml, scope.options));
//            $compile(element.contents())(scope);
//        };
//    };

//    return {
//        scope: { options: '=', ngModel: '=' },
//        require: ['ngModel', '^form'],
//        restrict: 'E',
//        compile: compileFunction,
//    };
//}]);

//csapp.directive("csInputSuffix", function () {
//    return {
//        require: 'ngModel',

//        restrict: 'A',
//        link: function (scope, element, attrs, ctr) {
//            ctr.$parsers.unshift(function (value) {

//                var isValid = value !== "";

//                ctr.$setValidity("required", isValid);
//                if (!isValid) {
//                    return undefined;
//                }

//                if (value.indexOf(attrs.suffix) < 0) {
//                    value = value + attrs.suffix;
//                }

//                return value;
//            });

//            ctr.$formatters.unshift(function (value) {
//                value = value || "";
//                return value.replace(attrs.suffix, "");
//            });
//        }
//    };
//});

//csapp.directive("csEmailField", ["$csfactory", "$compile", "csInputHelper", function ($csfactory, $compile, helper) {
//    //options:label, placeholder, pattern, minlength, maxlength, readonly, required
//    var fieldHtml = function (options) {
//        var hasSuffix = angular.isDefined(options.suffix) && options.suffix !== null && options.suffix.length > 0;

//        var string = '<div class="input-prepend input-append">' +
//            '<span class="add-on"><i class="icon-envelope"></i></span>' +
//            '<input type="email" name="myfield" placeholder="{{options.placeholder}}" ng-required="options.required" ' +
//            'ng-pattern="{{options.pattern}}" ng-minlength="{{options.minlength}}" ' +
//            'ng-maxlength="{{options.maxlength}}" data-ng-model="ngModel" ';
//        if (hasSuffix) {
//            string += 'class="input-medium" cs-input-suffix suffix="{{options.suffix}}" />';
//        } else {
//            string += 'class="input-large" />';
//        }

//        if (hasSuffix) {
//            string += '<span class="add-on">{{options.suffix}}</span>';
//        }

//        string += '</div>';
//        return string;
//    };

//    var checkOptions = function (options) {
//        options.label = options.label || "Email";
//        options.placeholder = options.placeholder || "Enter Email";
//        options.minlength = 5;
//        options.maxlength = 250;

//        if ($csfactory.isNullOrEmptyString(options.patternMessage)) {
//            options.patternMessage = "Input is not a valid email address.";
//        }
//    };

//    var compileFunction = function (element) {
//        return function (scope) {
//            checkOptions(scope.options);
//            element.html(helper.getFieldHtml(fieldHtml, scope.options));
//            $compile(element.contents())(scope);
//        };
//    };

//    return {
//        scope: { options: '=', ngModel: '=' },
//        required: ['ngModel', '^form'],
//        restrict: 'E',
//        compile: compileFunction,
//    };
//}]);

//csapp.directive("csCheckboxField", ["$csfactory", "$compile", "csInputHelper", function ($csfactory, $compile, helper) {
//    //options: label, required, checked
//    var fieldHtml = function () {
//        return '<input type="checkbox" name="myfield"  ng-required="options.required" ' +
//           ' data-ng-model="ngModel"/>';
//    };

//    var checkOptions = function (options) {
//        options.label = options.label || "CheckBox";
//    };

//    var compileFunction = function (element) {
//        return function (scope) {
//            checkOptions(scope.options);
//            element.html(helper.getFieldHtml(fieldHtml, scope.options));
//            $compile(element.contents())(scope);
//        };
//    };

//    return {
//        scope: { options: '=', ngModel: '=' },
//        required: ['ngModel', '^form'],
//        restrict: 'E',
//        compile: compileFunction,
//    };
//}]);

//csapp.directive("csSelectField", ["$csfactory", "$compile", "csInputHelper", function ($csfactory, $compile, helper) {

//    //options:label, required 
//    var fieldHtml = function (options) {
//        return '<select data-ng-model="ngModel" name="myfield" ng-required="options.required" >' +
//                    ' <option value=""></option> ' +
//                    ' <option data-ng-repeat="' + options.csRepeat + '"value="{{' + options.valueField + '}}">{{' + options.textField + '}}</option>' +
//                '</select> ';
//    };

//    //<cs-select-field options="selectopts" parse-object ng-model="selected" 
//    //cs-repeat="'row in $parent.values'" text-field="row.a" value-field="row"></cs-select-field>

//    var checkOptions = function (options) {
//        options.label = options.label || "SelectBox";
//    };

//    var compileFunction = function (element) {
//        return function (scope) {
//            //set params on options
//            scope.options.csRepeat = "row in $parent." + scope.csRepeat.substring(1, scope.csRepeat.length - 1);
//            scope.options.textField = scope.textField ? "row." + scope.textField : "row";
//            scope.options.valueField = scope.valueField ? "row." + scope.valueField : "row";
//            //console.log(scope.options.csRepeat);

//            checkOptions(scope.options);
//            element.html(helper.getFieldHtml(fieldHtml, scope.options));
//            $compile(element.contents())(scope);
//        };
//    };

//    return {
//        scope: { options: '=', ngModel: '=', csRepeat: '@', textField: '@', valueField: '@' },
//        require: ['ngModel', '^form'],
//        restrict: 'E',
//        compile: compileFunction,
//    };
//}]);

//csapp.directive("csEnumField", ["$csfactory", "$compile", "csInputHelper", function ($csfactory, $compile, helper) {
//    //options:label, required 
//    var fieldHtml = function () {
//        return '<select data-ui-select2="" class="input-large" ng-required="options.required" data-ng-model="ngModel" name="myfield" >' +
//                    ' <option value=""></option> ' +
//                    ' <option data-ng-repeat="row in options.values" value="{{row}}">{{row}}</option>' +
//                '</select> ';
//    };

//    var checkOptions = function (options) {
//        options.label = options.label || "SelectBox";
//    };

//    var compileFunction = function (element) {
//        return function (scope) {
//            checkOptions(scope.options);
//            element.html(helper.getFieldHtml(fieldHtml, scope.options));
//            $compile(element.contents())(scope);
//        };
//    };

//    return {
//        scope: { options: '=', ngModel: '=' },
//        require: ['ngModel', '^form'],
//        restrict: 'E',
//        compile: compileFunction,
//    };
//}]);

//csapp.directive("csButton", ["$csfactory", "$compile", function ($csfactory, $compile) {

//    var getOptionsByType = function (type) {
//        var options = {};
//        switch (type) {
//            case "save":
//                options.class = "btn btn-success";
//                options.caption = 'Save';
//                break;
//            case "cancel":
//                options.class = "btn btn-warning";
//                options.caption = 'Cancel';
//                break;
//            case "reset":
//                options.class = "btn btn-primary";
//                options.caption = 'Reset';
//                break;
//            case "ok":
//                options.class = "btn btn-info";
//                options.caption = 'Ok';
//                break;
//            case "close":
//                options.class = "btn btn-danger";
//                options.caption = 'Close';
//                break;
//            case "delete":
//                options.class = "btn icon-trash";
//                break;
//            case "edit":
//                options.class = "btn  icon-edit-sign";

//                break;
//            case "add":
//                options.class = "btn icon-plus";
//                break;
//            default:
//                throw "Invalid type : " + type;
//        }
//        return options;
//    };

//    var generateHtml = function (scope) {
//        return '<button class="{{options.class}}" ng-click="$parent.' + scope.ngClick + '" data-ng-disabled="ngDisabled" > {{options.caption}} </button>';
//    };

//    var linkFunction = function (scope, element) {
//        console.log(scope);
//        scope.options = getOptionsByType(scope.type);
//        element.html(generateHtml(scope));
//        $compile(element.contents())(scope);
//    };

//    return {
//        restrict: 'E',
//        link: linkFunction,
//        scope: { type: '@', ngClick: '@', ngDisabled: '=' },
//        replace: true
//    };
//}]);


//csapp.factory("radioButtonFactory", [function () {


//    var geTemplate = function (scope) {
//        return '<div class="radio" ng-repeat="(key, record) in options.options">' +
//                    '<label> <input type="radio" name="myfield" value="{{' + scope.options.valueField + '}}" ng-model="$parent.ngModel" ' +
//                        'ng-required="options.required"  />{{' + scope.options.textField + '}}</label>' +
//                '</div>';
//    };
//    var checkOptions = function (scope) {
//        scope.options.label = scope.options.label || "Description";
//        scope.options.textField = "record." + (scope.options.textField || "text");
//        scope.options.valueField = "record." + (scope.options.valueField || "value");
//    };
//    return {
//        htmlTemplate: geTemplate,
//        checkOptions: checkOptions
//    };

//}]);

//csapp.factory("textareaFactory", function () {
//    var geTemplate = function () {
//        console.log("textareaFactory : geTemplate");
//        return '<textarea type="text" name="myfield" placeholder="{{options.placeholder}}" ng-required="options.required" ' +
//            'ng-readonly="options.readonly" ng-minlength="{{options.minlength}}" ng-maxlength="{{options.maxlength}}" ' +
//            'rows="{{options.rows}}" cols="{{options.columns}}" autofocus="options.autofocus" data-ng-model="ngModel"/>';
//    };

//    var checkOptions = function (scope) {
//        scope.options.label = scope.options.label || "Description";
//        scope.options.rows = scope.options.rows || 2;
//        scope.options.columns = scope.options.columns || 120;
//        scope.options.placeholder = scope.options.placeholder || "Describe in detail";
//        scope.options.maxlength = scope.options.maxlength || 250;
//        scope.options.readonly = scope.options.readonly || scope.options.disabled;
//    };

//    return {
//        htmlTemplate: geTemplate,
//        checkOptions: checkOptions
//    };
//});

//csapp.factory("textFactory", function () {

//    var geTemplate = function () {
//        return '<input type="text" name="myfield" placeholder="{{options.placeholder}}" ng-required="options.required" ' +
//            'ng-pattern="{{options.pattern}}" ng-minlength="{{options.minlength}}" ng-maxlength="{{options.maxlength}}" ' +
//            'ng-readonly="options.readonly" autofocus="options.autofocus" ng-change="ngChange"  autocomplete="off" data-ng-model="ngModel"/>';
//    };

//    var checkOptions = function (scope) {
//        // manage lengths
//        scope.options.minlength = scope.options.length || scope.options.minlength || 0;
//        scope.options.maxlength = scope.options.length || scope.options.maxlength || 250;
//        scope.options.minlength = (scope.options.minlength >= 0 && scope.options.minlength <= 250) ? scope.options.minlength : 0;
//        scope.options.maxlength = (scope.options.maxlength >= 0 && scope.options.maxlength <= 250) ? scope.options.maxlength : 250;
//        if (scope.options.minlength > scope.options.maxlength) {
//            var error = "minlength(" + scope.options.minlength + ") cannot be greather than maxlength(" + scope.options.maxlength + ").";
//            throw error;
//        }

//        scope.options.label = scope.options.label || "Text";
//        scope.options.patternMessage = scope.options.patternMessage || ("Input is not matching with pattern : " + scope.options.pattern);
//        scope.options.readonly = scope.options.readonly || scope.options.disabled || false;
//    };

//    return {
//        htmlTemplate: geTemplate,
//        checkOptions: checkOptions
//    };

//});

//csapp.factory("numberFactory", function () {
//    var geTemplate = function () {
//        console.log("textareaFactory : geTemplate");
//        return '<input type="number" name="myfield" placeholder="{{options.placeholder}}" ng-required="options.required" ' +
//            ' ng-pattern="{{options.pattern}}" ng-minlength="{{options.minlength}}"  ng-maxlength="{{options.maxlength}}" ' +
//            ' step="any" ng-readonly="options.readonly"  max="{{options.max}}" min="{{options.min}}" autofocus="options.autofocus" ' +
//            ' data-ng-model="ngModel" />';
//    };

//    var applyTemplates = function (options) {
//        switch (options.type) {
//            case "uint":
//                options.min = 0;
//            case "int":
//                options.maxlength = 6;
//                break;
//            case "ulong":
//                options.min = 0;
//            case "long":
//                options.maxlength = 12;
//                break;
//            case "decimal":
//                options.maxlength = 19;
//            default:

//        }
//    };

//    var checkOptions = function (scope) {
//        applyTemplates(scope.options);
//        scope.options.minlength = scope.options.length || scope.options.minlength || 0;
//        scope.options.maxlength = scope.options.length || scope.options.maxlength || 18;
//        scope.options.minlength = (scope.options.minlength >= 0 && scope.options.minlength <= 18) ? scope.options.minlength : 0;
//        scope.options.maxlength = (scope.options.maxlength >= 0 && scope.options.maxlength <= 18) ? scope.options.maxlength : 18;
//        if (parseInt(scope.options.minlength) > parseInt(scope.options.maxlength)) {
//            var error = "minlength(" + scope.options.minlength + ") cannot be greather than maxlength(" + scope.options.maxlength + ").";
//            throw error;
//        }
//        scope.options.label = scope.options.label || "Number";
//        scope.options.patternMessage = scope.options.patternMessage || "Value cannot have non-numeric character/s.";
//    };

//    return {
//        htmlTemplate: geTemplate,
//        checkOptions: checkOptions
//    };
//});

//csapp.factory("emailFactory", [function () {
//    var geTemplate = function (scope) {
//        var hasSuffix = angular.isDefined(scope.options.suffix) && scope.options.suffix !== null && scope.options.suffix.length > 0;

//        //if (hasSuffix) scope.options.pattern = "/^[a-zA-Z0-9._]{1,100}$/";

//        var string = '<div class="input-prepend input-append">' +
//            '<span class="add-on"><i class="icon-envelope"></i></span>' +
//            '<input type="email" name="myfield" placeholder="{{options.placeholder}}" ng-required="options.required" ' +
//            'ng-pattern="{{options.pattern}}" ng-minlength="{{options.minlength}}" ' +
//            'ng-maxlength="{{options.maxlength}}" data-ng-model="ngModel" ';
//        if (hasSuffix) {
//            string += 'class="input-medium" cs-input-suffix suffix="{{options.suffix}}" />';
//        } else {
//            string += 'class="input-large" />';
//        }

//        if (hasSuffix) {
//            string += '<span class="add-on">{{options.suffix}}</span>';
//        }

//        string += '</div>';
//        return string;
//    };

//    var checkOptions = function (scope) {
//        scope.options.label = scope.options.label || "Email";
//        scope.options.placeholder = scope.options.placeholder || "Enter Email";
//        scope.options.minlength = scope.options.suffix ? scope.options.suffix.length + 3 : 8;
//        scope.options.maxlength = 250;
//        scope.options.patternMessage = "Input is not a valid email address.";
//    };

//    return {
//        htmlTemplate: geTemplate,
//        checkOptions: checkOptions
//    };

//}]);

//csapp.factory("checkboxFactory", [function () {

//    var geTemplate = function () {
//        return '<input type="checkbox" name="myfield"  ng-required="options.required" ' +
//           ' data-ng-model="ngModel"/>';
//    };

//    var checkOptions = function (scope) {
//        scope.options.label = scope.options.label || "CheckBox";
//    };
//    return {
//        htmlTemplate: geTemplate,
//        checkOptions: checkOptions
//    };

//}]);

//csapp.directive("csInput", ["$csfactory", "$compile", "Logger", "textareaFactory", "numberFactory", "textFactory", "emailFactory", "checkboxFactory", "radioButtonFactory", function ($csfactory, $compile, logManager, textarea, numberBox, text, email, checkbox, radio) {

//    var getFactoryByType = function (type) {
//        switch (type) {
//            case "textarea":
//                return textarea;
//            case "uint":
//            case "int":
//            case "ulong":
//            case "long":
//            case "decimal":
//                return numberBox;
//            case "text":
//                return text;
//            case "email":
//                return email;
//            case "checkbox":
//                return checkbox;
//            case "radio":
//                return radio;
//            default:
//                throw "Invalid type specification in csInput directive : " + type;
//        }
//    };

//    var fieldLayoutHtml = function (factory, scope) {

//        var html = '<div ng-form="myform">' +
//            '<div class="control-group" class="{{options.class}}" >' +
//            '<div class="control-label">{{options.label}} {{options.required ? "*" : ""}} </div>' +
//            '<div class="controls">';

//        html += factory.htmlTemplate(scope);

//        html += '<div class="field-validation-error" data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty"> ' +
//                    '<div data-ng-show="myform.myfield.$error.required ">{{options.label}} is required!!!</div>' +
//                    '<div data-ng-show="myform.myfield.$error.pattern">{{options.patternMessage}}</div>' +
//                    '<div data-ng-show="myform.myfield.$error.minlength">{{options.label}} should have atleast {{options.minlength}} character/s.</div>' +
//                    '<div data-ng-show="myform.myfield.$error.maxlength">{{options.label}} can have maximum {{options.maxlength}} character/s.</div>' +
//                    '<div data-ng-show="myform.myfield.$error.min">{{options.label}} cannot have value less than {{options.min}}</div>' +
//                    '<div data-ng-show="myform.myfield.$error.max">{{options.label}} cannot have value greater than {{options.max}}</div>' +
//                '</div>';

//        html += '</div>' + //controls
//            '</div>' + // control-group
//            '</div>'; //ng-form;

//        return html;
//    };

//    var linkFunction = function (scope, element) { //scope, element, attr, ctrl
//        var factory = getFactoryByType(scope.options.type);
//        factory.checkOptions(scope);
//        element.html(fieldLayoutHtml(factory, scope));
//        $compile(element.contents())(scope);
//    };

//    return {
//        restrict: 'E',
//        scope: { options: '=', ngModel: '=' },
//        require: ['ngModel', '^form'],
//        link: linkFunction
//    };
//}]);

////#endregion
