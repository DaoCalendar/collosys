
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

csapp.factory('buttonFactory', ['Logger', 'PermissionFactory', function (logManager, permFactory) {

    var $log = logManager.getInstance('buttonFactory');
    var getTemplateParams = function (type, text) {
        var templateParams = {
            type: 'button',
            className: ' btn-default'
        };

        switch (type) {
            case 'submit':
                templateParams.type = 'submit';
                templateParams.text = text || 'Submit';
                break;
            case 'delete':
                //templateParams.className = 'btn-danger';
                templateParams.text = text || 'Delete';
                break;
            case 'save':
                //templateParams.className = 'btn-success';
                templateParams.text = text || 'Save';
                break;
            case 'reset':
                //templateParams.className = 'btn-info';
                templateParams.text = text || 'Reset';
                break;
            case 'close':
                //templateParams.className = 'btn-warning';
                templateParams.text = text || 'Close';
                break;
            case 'ok':
                //templateParams.className = 'btn-primary';
                templateParams.text = text || 'OK';
                break;
            case 'cancel':
                //templateParams.className = 'btn-primary';
                templateParams.text = text || 'Cancel';
                break;
            case 'add':
                //templateParams.className = 'btn-primary';
                templateParams.text = text || 'Add';
                break;
            case 'edit':
                templateParams.text = text || 'Edit';
                break;
            case 'view':
                templateParams.text = text || 'View';
                break;
            default:
                $log.error('invalid button type: ' + type);
        }

        return templateParams;
    };

    var generateTemplate = function (templateParams, attrs) {


        var html = '<span data-ng-show="' + permFactory.HasPermission(attrs.permission) + '">';
        html += '<input';
        html += ' class=" btn ' + templateParams.className + '"';
        html += ' type="' + templateParams.type + '"';
        html += ' value="' + templateParams.text + '"';
        html += (attrs.ngShow ? ' ng-show="' + attrs.ngShow + '"' : '');
        html += (attrs.ngHide ? ' ng-hide="' + attrs.ngHide + '"' : '');
        html += (attrs.ngClick ? ' ng-click="' + attrs.ngClick + '"' : '');
        html += (attrs.ngDisabled ? ' ng-disabled="' + attrs.ngDisabled + '"' : '');
        html += '/>';
        html += '</span>';

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
            //scope.currpagenum = 1;
            scope.getrecordnum = function () {
                if (scope.currpagenum * scope.pagesize > scope.totalrecords)
                    return scope.totalrecords;
                else return (scope.currpagenum * scope.pagesize);
            };
            scope.getInitialNum = function () {
                return (scope.pagesize * (scope.currpagenum - 1)) + 1;
            };
        },

        template:
            '<div class="pull-right col-md-3">' +
            '<div><b>Records: {{getInitialNum()}}</b> - <b>{{getrecordnum()}}</b> of <b>{{totalrecords}}</b></div>' +
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

csapp.directive('iconBtn', ['PermissionFactory', function (permFactory) {

    var templateFn = function (element, attrs) {
        switch (attrs.type) {
            case 'add':
                return '<button type="button"' +
                    'data-toggle="tooltip" data-ng-show = "' + permFactory.HasPermission(attrs.permission) + '" data-placement="top" title="Add">' +
                    '<span class="glyphicon glyphicon-plus"></span>' +
                    '</button>';
            case 'edit':
                return '<button type="button"' +
                    'data-toggle="tooltip"  data-ng-show = "' + permFactory.HasPermission(attrs.permission) + '"  data-placement="top" title="Edit">' +
                    '<span class="glyphicon glyphicon-edit"></span>' +
                    '</button>';
            case 'view':
                return '<button type="button"' +
                    ' data-ng-show = "' + permFactory.HasPermission(attrs.permission) + '" data-toggle="tooltip" data-placement="top" title="View">' +
                    '<span class="glyphicon glyphicon-zoom-in"></span>' +
                    '</button>';
            case 'delete':
                return '<button type="button"' +
                    'data-toggle="tooltip"  data-ng-show = "' + permFactory.HasPermission(attrs.permission) + '" data-placement="top" title="Delete">' +
                    '<span class="glyphicon glyphicon-trash"></span>' +
                    '</button>';
            case 'up-arrow':
                return '<button type="button"' +
                    'data-toggle="tooltip"  data-ng-show = "' + permFactory.HasPermission(attrs.permission) + '" data-placement="top" title="Arrow-up">' +
                    '<span class="glyphicon glyphicon-arrow-up"></span>' +
                    '</button>';
            case 'down-arrow':
                return '<button type="button"' +
                     'data-toggle="tooltip"  data-ng-show = "' + permFactory.HasPermission(attrs.permission) + '" data-placement="top" title="Arrow-down">' +
                     '<span class="glyphicon glyphicon-arrow-down"></span>' +
                     '</button>';
            case 'remove':
                return '<button type="button"' +
                      'data-toggle="tooltip"  data-ng-show = "' + permFactory.HasPermission(attrs.permission) + '" data-placement="top" title="Remove">' +
                      '<span class="glyphicon glyphicon-remove"></span>' +
                      '</button>';
            case 'save':
                return '<button type="button"' +
                     'data-toggle="tooltip"  data-ng-show = "' + permFactory.HasPermission(attrs.permission) + '" data-placement="top" title="Save">' +
                     '<span class="glyphicon glyphicon-floppy-save"></span>' +
                     '</button>';
            case 'download':
                return '<button type="button"' +
                      '<data-toggle="tooltip"  data-ng-show = "' + permFactory.HasPermission(attrs.permission) + '"  data-placement="top" title="Download">' +
                      '<span class="glyphicon glyphicon-download-alt"></span>' +
                      '</button>';
            case 'status':
                return '<button type="button"' +
                     'data-toggle="tooltip"  data-ng-show = "' + permFactory.HasPermission(attrs.permission) + '"  data-placement="top" title="Status">' +
                     '<span class="glyphicon glyphicon-zoom-in"></span>' +
                     '</button>';
            case 'retry':
                return '<button type="button"' +
                     'data-toggle="tooltip"  data-ng-show = "' + permFactory.HasPermission(attrs.permission) + '"  data-placement="top" title="Retry">' +
                     '<span class="glyphicon glyphicon-repeat"></span>' +
                     '</button>';
            case 'forward':
                return '<button type="button"' +
                     'data-toggle="tooltip"  data-ng-show = "' + permFactory.HasPermission(attrs.permission) + '"  data-placement="top" title="Immediate">' +
                     '<span class="glyphicon glyphicon-forward"></span>' +
                     '</button>';
            case 'calendar':
                return '<span class="glyphicon glyphicon-calendar"></span>';
            default:
        }
        return type;
    };

    return {
        scope: { type: '@' },
        restrict: 'E',
        template: templateFn
    };
}]);

csapp.directive('csList2', function () {

    var templateFn = function (element, attrs) {
        var template = '<div class="row">';
        template += '<div class="panel panel-default" style="height:400px;overflow: auto">';
        template += '<div class="panel-heading">' + attrs.listHeading + ' </div>';
        template += '<ul class="list-group">';
        template += '<li class="list-group-item" ng-repeat="row in valueList"';
        template += ' ng-click="onChange(row, $index)' + (angular.isDefined(attrs.onClick) ? ';onClick()' : ' ') + '"';
        template += ' ng-model="ngModel" ng-class="{active : isSelected($index) }"';
        template += ' value="row">{{row.' + attrs.textField + '}}</li>';
        template += '</ul>';
        template += '</div>';
        template += '</div>';
        return template;
    };

    var linkFn = function (scope, element, attrs) {
        scope.onChange = function (row, index) {
            if (angular.isUndefined(row)) return;
            scope.$parent[attrs.ngModel] = row;
            scope.selectedIndex = index;
        };

        scope.isSelected = function (index) {
            return (scope.selectedIndex === index);
        };
    };

    return {
        restrict: 'E',
        replace: true,
        scope: { heading: '@', valueList: '=', ngModel: '=', onClick: '&' }, //textField
        template: templateFn,
        link: linkFn,
        require: 'ngModel'
    };
});
csapp.directive('csList', function () {

    var templateFn = function (element, attrs) {
        var template = '<div class="row">';
        template += '<div class="panel panel-default" style="height:400px;overflow: auto">';
        template += '<div class="panel-heading">' + attrs.listHeading + ' </div>';
        template += '<ul class="list-group">';
        template += '<li class="list-group-item" ng-repeat="row in ' + attrs.valueList + '"';
        template += ' ng-click="onClick(row, $index)' + (angular.isDefined(attrs.onClick) ? ';' + attrs.onClick : '') + '"';
        template += attrs.ngModel ? ' ng-model="' + attrs.ngModel + '"' : ' ';
        template += angular.isDefined(attrs.ngClass) ? attrs.ngClass : ' ng-class="{active : isSelected($index) }"';
        template += ' value="row">{{row.' + attrs.textField + '}}</li>';
        template += '</ul>';
        template += '</div>';
        template += '</div>';
        return template;
    };

    var linkFn = function (scope, element, attrs) {
        scope.onClick = function (row, index) {
            if (angular.isUndefined(row)) return;
            scope.$parent[attrs.ngModel] = row;
            scope.selectedIndex = index;
        };

        scope.isSelected = function (index) {
            var result = scope.selectedIndex === index;
            if (result === false) return false;
            if (angular.isDefined(scope.$parent.isSelected)) {
                return scope.$parent.isSelected(attrs.dir);
            }
            return false;
        };
    };

    return {
        restrict: 'E',
        replace: true,
        scope: true,
        template: templateFn,
        link: linkFn
    };
});

csapp.directive('csDualList', ["$csfactory", function ($csfactory) {
    var templateFunction = function (element, attrs) {
        var html = '<div class="row">';
        html += '<div class="col-md-5">';
        html += '<cs-list list-heading="' + attrs.lhsHeading + '" value-list="' + attrs.lhsValueList + '"  text-field="' + attrs.textField + '" ';
        html += attrs.ngModel ? ' ng-model="' + attrs.ngModel + '"' : ' ';
        html += ' dir = "lhs"';
        html += ' on-click="clicked.left(' + attrs.ngModel + ', $index)' + (angular.isDefined(attrs.onClick) ? ';' + attrs.onClick : '') + '">';
        html += '</cs-list>';
        html += '</div>';
        html += '<div class="col-md-1">';
        html += '<button class="btn btn-success" ng-click="move.left(selectedItem)" ng-disabled="direction.left"><i class="glyphicon glyphicon-arrow-left"></i></button>';
        html += '<button class="btn btn-success" ng-click="move.right(selectedItem)" ng-disabled="direction.right"><i class="glyphicon glyphicon-arrow-right"></i></button>';
        html += '</div>';
        html += '<div class="col-md-5">';
        html += '<cs-list list-heading="' + attrs.rhsHeading + '" value-list="' + attrs.rhsValueList + '"  text-field="' + attrs.textField + '" ';
        html += attrs.ngModel ? ' ng-model="' + attrs.ngModel + '"' : ' ';
        html += ' dir = "rhs"';
        html += 'on-click="clicked.right(' + attrs.ngModel + ', $index)' + (angular.isDefined(attrs.onClick) ? ';' + attrs.onClick : '') + '"></cs-list>';
        html += '</div>';
        html += '<div class="col-md-1">';
        html += '<button class="btn btn-success" ng-click="move.up(selectedItem,selectedIndex)" ng-disabled="direction.up"><i class="glyphicon glyphicon-arrow-up"></i></button>';
        html += '<button class="btn btn-success" ng-click="move.down(selectedItem,selectedIndex)" ng-disabled="direction.down"><i class="glyphicon glyphicon-arrow-down"></i></button>';
        html += '</div>';
        html += '</div>';
        console.log(html);
        return html;

    };

    var linkFunction = function (scope, el, attrs) {
        scope.direction = {
            left: true,
            right: true,
            up: true,
            down: true
        };

        scope.isSelected = function (dir) {
            return (dir === scope.selectedDir);
        };

        scope.clicked = {
            left: function (selected, index) {
                if (angular.isUndefined(index)) return;
                if (angular.isUndefined(selected) || $csfactory.isEmptyObject(selected) || selected === null) {
                    return;
                }
                scope.direction = {
                    left: true,
                    right: false,
                    up: true,
                    down: true
                };
                scope.selectedItem = selected;
                scope.selectedIndex = index;
                scope.selectedDir = "lhs";
            },
            right: function (selected, index) {
                if (angular.isUndefined(index)) return;
                if (angular.isUndefined(selected) || $csfactory.isEmptyObject(selected) || selected === null) {
                    return;
                }
                scope.direction = {
                    left: false,
                    right: true,
                    up: true,
                    down: true
                };
                if (index !== 0) {
                    scope.direction.up = false;
                }
                var maxindex = ($csfactory.getPropertyValue(scope.$parent.$parent, attrs.rhsValueList).length) - 1;
                if (maxindex !== index) {
                    scope.direction.down = false;
                }
                scope.selectedItem = selected;
                scope.selectedIndex = index;
                scope.selectedDir = "rhs";
            },
        },

           scope.move = {
               left: function (selected) {
                   var lhslist = $csfactory.getPropertyValue(scope.$parent.$parent, attrs.lhsValueList);
                   var rhslist = $csfactory.getPropertyValue(scope.$parent.$parent, attrs.rhsValueList);
                   lhslist.push(selected);
                   rhslist.splice(rhslist.indexOf(selected), 1);
                   scope.selectedItem = {};
                   scope.$parent[attrs.ngModel] = null;
                   scope.direction = {
                       right: true,
                       left: true,
                       up: true,
                       down: true
                   };

               },
               right: function (selected) {
                   var lhslist = $csfactory.getPropertyValue(scope.$parent.$parent, attrs.lhsValueList);
                   var rhslist = $csfactory.getPropertyValue(scope.$parent.$parent, attrs.rhsValueList);
                   rhslist.push(selected);
                   lhslist.splice(lhslist.indexOf(selected), 1);
                   scope.direction.right = true;
                   scope.selectedItem = {};
                   scope.$parent[attrs.ngModel] = null;
               },
               up: function (selected, index) {
                   var rhslist = $csfactory.getPropertyValue(scope.$parent.$parent, attrs.rhsValueList);
                   var temp = rhslist[index];
                   rhslist[index] = rhslist[index - 1];
                   rhslist[index - 1] = temp;
               },
               down: function (selected, index) {
                   var rhslist = $csfactory.getPropertyValue(scope.$parent.$parent, attrs.rhsValueList);
                   var temp = rhslist[index];
                   rhslist[index] = rhslist[index + 1];
                   rhslist[index + 1] = temp;
               },
           };
    };

    return {
        restrict: 'E',
        scope: true,
        template: templateFunction,
        link: linkFunction
    };
}]);
