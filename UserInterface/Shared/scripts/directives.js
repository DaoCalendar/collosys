
csapp.directive('csInclude', ["$document", function ($document) {
    var templateFn = function () {
        var hostedUrl = $document.location.path || "/";
        var template = '<div ng-include="' + hostedUrl + '{{src}}"></div>';
        return template;
    };

    return {
        restrict: 'E',
        replace: true,
        scope: { src: '@' },
        template: templateFn
    };
}]);

csapp.directive("csFileUpload", ["Restangular", "Logger", "$csfactory", "$upload",
    function (rest, logManager, $csfactory, $upload) {
        //var $log = logManager.getInstance("csFileUploadDirective");

        var getFileInputTemplate = function () {
            return '<div ng-form="" name="myform" style="margin: 20px">' +
                    '<div class="form-group"><div class="controls">' +
                        '<div data-ng-show="fileInfo.isUploading">' +
                            '<progressbar class="progress-striped active" value="fileInfo.uploadPercent" ' +
                                'type="success"></progressbar>' +
                            '<div class="text-danger">Copying file to server!!!</div>' +
                        '</div>' +
                        '<div data-ng-hide="fileInfo.isUploading">' +
                            '<label class="fileContainer btn btn-default col-md-2"> Select ' +
                            '<input name="myfield" ng-model="ngModel" type="file" ' +
                                'ng-file-select="copyToServer($files)" ng-required="validations.required" />' +
                             '</label>' +
                             '<div class="col-md-6">' +
                                '<input type="text" class="form-control" tooltip-position="top" tooltip="{{fileInfo.name}}" readonly="readonly" ng-model="fileInfo.name">' +
                             '</div>' +
                        '</div>' +
                        '<div data-ng-show="valerror.$invalid">' +
                            '<div class="text-danger" data-ng-show="valerror.$error.nonempty">Please provide non-empty files</div>' +
                            '<div class="text-danger" data-ng-show="valerror.$error.extension">Please select {{validations.extension}} file.</div>' +
                            '<div class="text-danger" data-ng-show="valerror.$error.pattern">Pattern {{validations.pattern}} mismatch.</div>' +
                            '<div class="text-danger" data-ng-show="valerror.$error.required">Please select a file.</div>' +
                        '</div>' +
                    '</div></div>' +
                    '</div>';
        };

        var setParams = function (cfile, file) {
            file.name = cfile.name;
            file.size = cfile.size;
        };

        var saveFileOnServer = function (scope, ngModel) {
            scope.fileInfo.isUploading = true;
            scope.fileInfo.copied = false;
            ngModel.$setValidity("noncopying", false);

            $upload.upload({
                url: '/api/FileIoApi/SaveFile',
                method: "Post",
                file: scope.cfile
            }).progress(function (evt) {
                scope.fileInfo.uploadPercent = parseInt(100.0 * evt.loaded / evt.total);
            }).success(function (data) {
                scope.fileInfo.path = data.FullPath;
                scope.fileInfo.isUploading = false;
                scope.fileInfo.copied = true;
                if (angular.isFunction(scope.onSave)) {
                    scope.onSave({ 'fileInfo': scope.fileInfo });
                }
                ngModel.$setValidity("noncopying", true);
            }).error(function () {
                scope.fileInfo.isUploading = false;
                scope.onSave({ 'fileInfo': scope.fileInfo });
                ngModel.$setValidity("noncopying", true);
            });
        };

        var linkFunction = function (scope, element, attr, ngModel) {
            ngModel.$render = function (filename) {
                ngModel.$setViewValue(filename);
            };

            if (angular.isUndefined(scope.fileInfo)) {
                throw "please provide file info.";
            }

            scope.valerror = {
                $invalid: false,
                $error: {},
                add: function (prop) {
                    scope.valerror.$invalid = true;
                    scope.valerror.$error[prop] = true;
                },
                reset: function () {
                    scope.valerror.$invalid = false;
                    scope.valerror.$error = {};
                }
            };

            scope.isFileValid = function () {
                scope.valerror.reset();
                ngModel.$setValidity("pattern", true);
                ngModel.$setValidity("extension", true);
                ngModel.$setValidity("nonEmpty", true);
                ngModel.$setValidity("required", true);
                if (angular.isUndefined(scope.validations)) {
                    return true;
                }

                if (scope.validations.required === true) {
                    if ($csfactory.isNullOrEmptyString(scope.fileInfo.name)) {
                        ngModel.$setValidity("required", false);
                        scope.valerror.add("required");
                        return false;
                    }
                }

                if (!$csfactory.isNullOrEmptyString(scope.validations.pattern)) {
                    if (!scope.fileInfo.name.match(scope.validations.pattern)) {
                        ngModel.$setValidity("pattern", false);
                        scope.valerror.add("pattern");
                        return false;
                    }
                }

                if (scope.fileInfo.size === 0) {
                    ngModel.$setValidity("nonempty", false);
                    scope.valerror.add("nonempty");
                    return false;
                }

                if (!$csfactory.isNullOrEmptyString(scope.validations.extension)) {
                    var extension = scope.fileInfo.name.substring(scope.fileInfo.name.lastIndexOf('.') + 1);
                    if (extension !== scope.validations.extension) {
                        ngModel.$setValidity("extension", false);
                        scope.valerror.add("extension");
                        return false;
                    }
                }

                return true;
            };
            scope.isFileValid();

            scope.copyToServer = function ($files) {
                scope.cfile = $files[0];
                setParams(scope.cfile, scope.fileInfo);
                ngModel.$render(scope.fileInfo.name);
                if (scope.isFileValid()) {
                    saveFileOnServer(scope, ngModel);
                }
            };
        };

        return {
            scope: { onSave: '&', ngModel: '=', fileInfo: '=', validations: '=' },
            restrict: 'E',
            template: getFileInputTemplate,
            link: linkFunction,
            require: 'ngModel'
        };
    }
]);

//button directive

csapp.factory('buttonFactory', ['Logger', function (logManager) {

    var $log = logManager.getInstance('buttonFactory');
    var getTemplateParams = function (type, text) {
        var templateParams = {
            type: 'button',
            className: ' btn-default'
        };

        switch (type) {
            case 'Submit':
                templateParams.text = type || 'submit';
                templateParams.text = text || 'Submit';
                break;
            case 'Delete':
                templateParams.className = 'btn-danger';
                templateParams.text = text || 'Delete';
                break;
            case 'Save':
                templateParams.className = 'btn-success';
                templateParams.text = text || 'Save';
                break;
            case 'Add':
                templateParams.className = 'btn-default';
                templateParams.text = text || 'Add';
                break;
            case 'Edit':
                templateParams.className = 'btn-default';
                templateParams.text = text || 'Edit';
                break;
            case 'View':
                templateParams.className = 'btn-default';
                templateParams.text = text || 'View';
                break;
            default:
                $log.error('invalid button type: ' + type);
        }
        return templateParams;
    };

    var generateTemplate = function (templateParams, attrs) {

        var html = '<input';
        html += ' class=" btn ' + templateParams.className + '"';
        html += ' type="' + templateParams.type + '"';
        html += ' value="' + templateParams.text + '"';
        html += (attrs.ngShow ? ' ng-show="' + attrs.ngShow + '"' : '');
        html += (attrs.ngHide ? ' ng-hide="' + attrs.ngHide + '"' : '');
        html += (attrs.ngClick ? ' ng-click="' + attrs.ngClick + '"' : '');
        html += (attrs.ngDisabled ? ' ng-disabled="' + attrs.ngDisabled + '"' : '');
        html += '/>';

        return html;
    };

    return {
        getTemplateParams: getTemplateParams,
        generateTemplate: generateTemplate
    };
}]);

csapp.directive('csButton', ['$parse', '$compile', 'buttonFactory',
    function ($parse, $compile, buttonFactory) {

        var linkFunction = function (scope, element, attrs) {
            var buttonType = attrs.type;
            var templateParams = buttonFactory.getTemplateParams(buttonType, attrs.text);
            var template = buttonFactory.generateTemplate(templateParams, attrs);

            var newElem = angular.element(template);
            element.replaceWith(newElem);
            $compile(newElem)(scope);
        };

        return {
            restrict: 'E',
            link: linkFunction,
            require: '^form'
        };
    }
]);

//#region switch-buttons 3 directives
csapp.directive('btnSwitch', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        //templateUrl: 'switcher.html',
        template: '<span class="btn boolean">' +
                    '<span class="on btn-primary">Yes</span>' +
                    '<span class="off btn-default">No</span>' +
                '</span>',
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
            } // do nothing if no ng-model

            // Initial render
            render();
        }
    };
});

csapp.directive('switchyesno', function () {
    return {
        restrict: 'E',
        scope: {
            text: "@",
            readonly: "=",
            ngbind: "="
        },
        template: '<div class="form-group row">' +
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

csapp.directive("csswitch", function () {

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
});
//#endregion

//#region spinner & bs-datepicker
csapp.directive("spinner", function () {
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
});

csapp.directive('bsDatepicker', function () {
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

        template:
            '<div class="pull-right col-md-3">' +
            '<div><b>Records: {{(pagesize*(currpagenum-1))+1}}</b> - <b>{{getrecordnum()}}</b> of <b>{{totalrecords}}</b></div>' +
            '<div class="input-group">' +
            '<span class="input-group-btn"><button class="btn btn-default" data-ng-click="gotofirstpage()"><i class="fa fa-angle-double-left"></i></button></span>' +
            '<span class="input-group-btn"><button class="btn btn-default" data-ng-click="stepbackward()"><i class="fa fa-chevron-left"></i></button></span>' +
            '<input type="text" readonly data-ng-model=currpagenum style="margin-top: 0px" class="form-control text-center"></span>' +
            '<span class="input-group-btn"><button class="btn btn-default" data-ng-click="stepforward()"><i class="fa fa-chevron-right"></i></button></span>' +
            '<span class="input-group-btn"><button class="btn btn-default" data-ng-click="gotolastpage()"><i class="fa fa-angle-double-right"></i></button></span>' +
            '</div>' +
            '</div>' +
            '</div>'
    };
});




//csapp.directive("csTemplate", ["$compile", function ($compile) {

//    var getTemplate = function () {
//        var html = '<div ng-form="myform">' +
//                    '<div class="form-group" class="{{options.class}}" >' +
//                    '<div class="control-label">{{options.label}} <span style="color:red">{{options.required ? "*" : ""}} </span></div>' +
//                    '<div class="controls">';

//        html += '<div ng-transclude></div>';

//        html += '<div class="field-validation-error" data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty"> ' +
//            '<div data-ng-show="myform.myfield.$error.required ">{{options.label}} is required!!!</div>' +
//            '<div data-ng-show="myform.myfield.$error.pattern">{{options.patternMessage}}</div>' +
//            '<div data-ng-show="myform.myfield.$error.minlength">{{options.label}} should have atleast {{options.minlength}} character/s.</div>' +
//            '<div data-ng-show="myform.myfield.$error.maxlength">{{options.label}} can have maximum {{options.maxlength}} character/s.</div>' +
//            '<div data-ng-show="myform.myfield.$error.min">{{options.label}} cannot have value less than {{options.min}}</div>' +
//            '<div data-ng-show="myform.myfield.$error.max">{{options.label}} cannot have value greater than {{options.max}}</div>' +
//        '</div>';

//        html += '</div>' + //controls
//            '</div>' + // form-group
//            '</div>'; //ng-form;

//        return html;

//    };

//    return {
//        scope: { options: '=' },
//        restrict: 'E',
//        transclude: true,
//        replace: true,
//        template: getTemplate
//    };
//}]);

//csapp.directive('csOptions', ["$compile", function ($compile) {


//    function getPropertyByKeyPath(targetObj, keyPath) {
//        var keys = keyPath.split('.');
//        if (keys.length === 0) return undefined;
//        keys = keys.reverse();
//        var subObject = targetObj;
//        while (keys.length) {
//            var k = keys.pop();
//            if (!subObject.hasOwnProperty(k)) {
//                return undefined;
//            } else {
//                subObject = subObject[k];
//            }
//        }
//        return subObject;
//    }

//    var validations = function (options) {
//        var html = '<div data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty">';
//        html += '<div data-ng-show="myform.myfield.$error.required">' + options.label + ' required!!</div>' +
//            '<div data-ng-show="myform.myfield.$error.minlength">' + options.label + ' must have atleast ' + options.minlength + ' characters</div>' +
//            '<div data-ng-show="myform.myfield.$error.maxlength">' + options.label + ' can have atmost ' + options.maxlength + ' characters</div>' +
//            '<div data-ng-show="myfrom.myfield.$error.pattern">pattern error</div>';
//        html += '</div>';

//        return html;
//    };

//    var before = function (options) {
//        var html = '<form name="myform">';
//        html += '<div class="form-group"><div class="control-label">' + options.label + '</div>' +
//            '<div class="controls">';
//        return html;
//    };

//    var after = function (options) {

//        var html = '</div>' +
//            '</div>' +
//            validations(options) +
//        '</form>';
//        return html;
//    };

//    var getHTML = function (element, options) {

//        var html = before(options);
//        html += element.html();
//        html += after(options);

//        return html;
//    };

//    var setElementAttr = function (element, fieldValue) {

//        console.log('setting attrs');

//        if (!element.attr('ng-required'))
//            element.attr("ng-required", fieldValue.required);
//        console.log(fieldValue);

//        if (!element.attr('ng-maxlength'))
//            element.attr("ng-maxlength", fieldValue.maxlength);

//        if (!element.attr('ng-minlength'))
//            element.attr("ng-minlength", fieldValue.minlength);

//        if (!element.attr('ng-pattern'))
//            element.attr("ng-pattern", fieldValue.pattern);

//        if (!element.attr('name'))
//            element.attr("name", "myfield");

//        element.removeAttr("cs-options");
//    };


//    var linkFunction = function (scope, element, attrs) {

//        var fieldText = attrs['csOptions'];
//        var fieldValue = getPropertyByKeyPath(scope, fieldText);
//        setElementAttr(element, fieldValue);

//        var $parent = element.parent();

//        var html = getHTML($parent, fieldValue);
//        $parent.html(html);
//        $compile($parent)(scope);
//    };

//    return {
//        restrict: 'A',
//        compile: function () {
//            return {
//                pre: linkFunction
//            };
//        },
//        require: 'ngModel'
//    };
//}]);
