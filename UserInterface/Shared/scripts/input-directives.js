csapp.factory("csBootstrapInputTemplate", function () {

    var bsTemplateBefore = function (field, noBootstrap, attr) {

        var noLabel = '<div ' + (attr.ngShow ? ' ng-show="' + attr.ngShow + '"' : '');
        noLabel += 'class="cs-fields ' + field.layoutClass.div + '"';
        noLabel += (attr.ngHide ? ' ng-hide="' + attr.ngHide + '"' : '');
        noLabel += (attr.ngIf ? ' ng-if="' + attr.ngIf + '"' : '');
        noLabel += '>';

        var withLabel = noLabel;
        withLabel += '<div data-ng-hide="field.size.nolabel">' +
            '<label class="cs-label ' + field.layoutClass.label + '">{{' + attr.field + '.label}}' +
                '<span class="text-danger">{{' + attr.field + '.required ? " *":""}}</span></label>' +
            '</div>';
        return (noBootstrap || field.size.label === 0 ? noLabel : withLabel);
    };

    var bsTemplateAfter = function () {
        return '</div>';
    };

    return {
        before: bsTemplateBefore,
        after: bsTemplateAfter
    };
});

csapp.factory("csValidationInputTemplate", function () {

    var before = function (field) {
        if (!field.validate) return '<div class="' + field.layoutClass.control + '">';
        var html = '<div ng-form="myform" role="form" class="form-group has-feedback ' + field.layoutClass.control + '">';
        return html;
    };

    var getmessages = function (fieldname, field) {
        field.messages = {
            required: '{{' + fieldname + '.label}} is required.',
            pattern: (field.patternMessage) ? field.patternMessage : '{{' + fieldname + '.label}} is not matching with pattern {{' + fieldname + '.pattern}}.',
            minlength: '{{' + fieldname + '.label}} should have atleast {{' + fieldname + '.minlength}} character/s.',
            maxlength: '{{' + fieldname + '.label}} can have maximum {{' + fieldname + '.maxlength}} character/s.',
            min: '{{' + fieldname + '.label}} cannot be less than {{' + fieldname + '.min}}.',
            max: '{{' + fieldname + '.label}} cannot be greater than {{' + fieldname + '.max}}.',
            unique: '{{' + fieldname + '.label}} is already used.'
        };
    };

    var after = function (fieldname, field) {

        if (!field.validate) return "</div>";

        getmessages(fieldname, field);

        var html = '<span data-ng-show=" myform.myfield.$dirty" ng-class="{  \'has-success\'   : myform.myfield.$valid, \'has-error\' : myform.myfield.$invalid }">';
        html += '<span class=" form-control-feedback validation-icon" ng-class="{ \'glyphicon glyphicon-ok\' : myform.myfield.$valid, \'glyphicon glyphicon-remove\' : myform.myfield.$invalid }"></span>' +
                '</span>';
        html += '<div  data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty"> ' +
           '<div class="text-danger" data-ng-show="myform.myfield.$error.required">' + field.messages.required + '</div>' +
           '<div class="text-danger" data-ng-show="myform.myfield.$error.pattern">' + field.messages.pattern + '</div>' +
           '<div class="text-danger" data-ng-show="myform.myfield.$error.minlength">' + field.messages.minlength + '</div>' +
           '<div class="text-danger" data-ng-show="myform.myfield.$error.maxlength">' + field.messages.maxlength + '</div>' +
           '<div class="text-danger" data-ng-show="myform.myfield.$error.min">' + field.messages.min + '</div>' +
           '<div class="text-danger" data-ng-show="myform.myfield.$error.max">' + field.messages.max + '</div>' +
           '<div class="text-danger" data-ng-show="myform.myfield.$error.unique">' + field.messages.unique + '</div>' +
           '</div>';
        html += '</div>'; //ng-form; 
        return html;
    };

    return {
        before: before,
        after: after
    };
});

csapp.directive("csInputSuffix", function () {
    return {
        require: 'ngModel',

        restrict: 'A',
        link: function (scope, element, attrs, ctr) {
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
        }
    };
});

csapp.directive("csFileUpload", ["Restangular", "Logger", "$csfactory", "$upload",
    function (rest, logManager, $csfactory, $upload) {
        //var $log = logManager.getInstance("csFileUploadDirective");

        var getFileInputTemplate = function (element, attrs) {
            return '<div ng-form="myform" style="margin: 20px">' +
                    '<div class="form-group"><div class="controls">' +
                        '<div data-ng-show="fileInfo.isUploading">' +
                            '<progressbar class="progress-striped active" value="fileInfo.uploadPercent" ' +
                                'type="success"></progressbar>' +
                            '<div class="text-danger">Copying file to server!!!</div>' +
                        '</div>' +
                        '<div data-ng-hide="fileInfo.isUploading">' +
                            '<label>' + attrs.cslabel + '</label>' +
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

        var saveFileOnServer = function (scope, ngModel, attr) {
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
                    //  scope.onSave({ 'fileInfo': scope.fileInfo });
                    scope.$eval(attr.onSave);
                }
                ngModel.$setValidity("noncopying", true);
            }).error(function () {
                scope.fileInfo.isUploading = false;
                //scope.onSave({ 'fileInfo': scope.fileInfo });
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
                    saveFileOnServer(scope, ngModel, attr);
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
    }]);

csapp.factory("csFileFieldFactory", ["Logger", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function (logManager, bstemplate, valtemplate) {

        var input = function (field, attrs) {
            var html = '<div class="btn-group">';
            html += '<button  ng-repeat="data in field.options" class="btn btn-primary"';
            html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
            html += (attrs.ngClick ? ' ng-click="' + attrs.ngClick + '"' : '');
            html += (attrs.ngDisabled ? ' ng-disabled="' + attrs.ngDisabled + '"' : ' ng-disabled="setReadonly()"');
            html += ' ng-model="$parent.' + attrs.ngModel + '" btn-radio="data" uncheckable>';
            html += '{{data}}<i data-ng-show="$parent.' + attrs.ngModel + '=== data " class="glyphicon glyphicon-ok"></i>';
            html += '</button>';
            html += '</div>';
            return html;
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs),
                valtemplate.before(field),
                input(field, attrs),
                valtemplate.after(attrs.field, field),
                bstemplate.after(noBootstrap)
            ].join(' ');
            return template;
        };

        var validateOptions = function () { };

        var linkFunction = function () { };

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions,
            linkFunction: linkFunction
        };
    }]);

csapp.factory("csBooleanFieldFactory", ["Logger", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function (logManager, bstemplate, valtemplate) {

        var input = function (field, attrs) {
            var html = '<div class="btn-group">';
            html += '<button  ng-repeat="data in field.options" class="btn btn-primary"';
            html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
            html += (attrs.ngClick ? ' ng-click="' + attrs.ngClick + '"' : '');
            html += (attrs.ngDisabled ? ' ng-disabled="' + attrs.ngDisabled + '"' : ' ng-disabled="setReadonly()"');
            html += ' ng-model="$parent.' + attrs.ngModel + '" btn-radio="data" uncheckable>';
            html += '{{data}}<i data-ng-show="$parent.' + attrs.ngModel + '=== data " class="glyphicon glyphicon-ok"></i>';
            html += '</button>';
            html += '</div>';
            return html;
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs),
                valtemplate.before(field),
                input(field, attrs),
                valtemplate.after(attrs.field, field),
                bstemplate.after(noBootstrap)
            ].join(' ');
            return template;
        };

        var validateOptions = function (options) {
            options.label = options.label || "Boolean";

            if (angular.isUndefined(options.options)) {
                options.options = [true, false];
            }

            if (angular.isDefined(options.valueField)) {
                if (options.valueField.substring(0, 6) !== "record") {
                    options.valueField = "record." + options.valueField;
                }
            } else {
                options.valueField = "record";
            }

            if (angular.isDefined(options.textField)) {
                if (options.textField.substring(0, 6) !== "record") {
                    options.textField = "record." + options.textField;
                }
            } else {
                options.textField = "record";
            }

        };

        var linkFunction = function () { };

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions,
            linkFunction: linkFunction
        };
    }]);

//{ label: 'Number', editable: false, required: true, type: 'number'}
csapp.factory("csNumberFieldFactory", ["Logger", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function (logManager, bstemplate, valtemplate) {

        var $log = logManager.getInstance("csNumberFieldFactory");

        var prefix = function (fields) {
            var html = ' ';
            switch (fields.template) {
                case 'rupee':
                    html += '<div class="input-group"><span class="input-group-addon"><i class="fa fa-rupee"></i></span>';
                    break;
                case 'percentage':
                    html += '<div class="input-group">';
                    break;
                default:
                    break;
            }
            return html;
        };

        var suffix = function (fields) {
            var html = ' ';
            switch (fields.template) {
                case 'rupee':
                    html += '</div>';
                    break;
                case 'percentage':
                    html += '<span class="input-group-addon"><label>%</label></span></div>';
                default:
                    break;
            }
            return html;
        };

        //#region template
        var input = function (field, attrs) {
            var html = '<input name="myfield" type="number"';
            html += ' ng-model="$parent.' + attrs.ngModel + '"';
            html += (angular.isDefined(attrs.ngRequired) ? 'ng-required = "' + attrs.ngRequired + '"' : ' ng-required="' + attrs.field + '.required"');
            html += (attrs.ngDisabled ? ' ng-readonly="' + attrs.ngDisabled + '"' : ' ng-readonly="setReadonly()"');
            html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
            html += ' class ="minWidth form-control"';
            html += ((field.type === "decimal") ? ' step="any"' : '');
            html += (angular.isDefined(field.minlength) ? ' ng-minlength="' + field.minlength + '"' : '');
            html += (angular.isDefined(field.maxlength) ? ' ng-maxlength="' + field.maxlength + '"' : '');
            html += (angular.isDefined(field.min) ? ' min="' + field.min + '"' : '');
            html += (angular.isDefined(field.max) ? ' max="' + field.max + '"' : '');
            html += (angular.isDefined(field.pattern) ? ' ng-pattern="' + field.pattern + '"' : '');
            html += (angular.isDefined(field.placeholder) ? ' placeholder="' + field.placeholder + '"' : '');
            html += (angular.isDefined(attrs.typeahead) ? ' typeahead="' + attrs.typeahead + '"' : ' ');
            html += (angular.isDefined(attrs.typeahead) ? ' typeahead-min-length="' + field.typeaheadMinLength + '"' : '');
            html += (angular.isDefined(attrs.typeahead) ? ' typeahead-wait-ms="' + field.typeaheadWaitMs + '"' : '');
            html += (angular.isDefined(attrs.typeahead) && attrs.ngChange ? ' typeahead-on-select="' + attrs.ngChange + '"' : '');
            html += (angular.isDefined(attrs.uiValidate) ? ' ui-validate="' + attrs.uiValidate + '"' : '');
            html += '/>';
            return html;
        };

        var configureTypeahead = function (field, attrs) {
            if (angular.isUndefined(attrs.typeahead)) return;
            field.typeaheadMinLength = field.typeaheadMinLength || 3;
            field.typeaheadWaitMs = field.typeaheadWaitMs || 400;
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            configureTypeahead(field, attrs);
            var template = [
                bstemplate.before(field, noBootstrap, attrs),
                valtemplate.before(field),
                prefix(field),
                input(field, attrs),
                suffix(field),
                valtemplate.after(attrs.field, field),
                bstemplate.after(noBootstrap)
            ].join(' ');
            return template;
        };
        //#endregion

        //#region validations
        var applyTemplates = function (options) {

            options.template = angular.isUndefined(options.template) ? 'decimal' : options.template;

            var tmpl = options.template.split(",").filter(function (str) { return str !== ''; });
            angular.forEach(tmpl, function (template) {
                if (template.length < 1) return;
                switch (template) {
                    case "uint":
                        if (angular.isUndefined(options.min))
                            options.min = 0;
                    case "int":
                        if (angular.isUndefined(options.maxlength))
                            options.maxlength = 6;
                        break;
                    case "ulong":
                        if (angular.isUndefined(options.min))
                            options.min = 0;
                    case "long":
                        if (angular.isUndefined(options.maxlength))
                            options.maxlength = 12;
                        break;
                    case "decimal":
                        if (angular.isUndefined(options.maxlength))
                            options.maxlength = 19;
                        break;
                    case "percentage":
                        options.min = 0;
                        options.max = 100;
                        options.pattern = "/^[0-9]+(\.[0-9][0-9]?)?$/";
                        options.patternMessage = "allows percentage with precision of 2";
                        break;
                    case "rupee":
                        options.min = 0;
                        break;
                    default:
                        $log.error(options.type + " is not defined");
                }
            });
        };

        var validateOptions = function (options) {
            applyTemplates(options);
            options.minlength = options.length || options.minlength || 0;
            options.maxlength = options.length || options.maxlength || 18;
            options.minlength = (options.minlength >= 0 && options.minlength <= 18) ? options.minlength : 0;
            options.maxlength = (options.maxlength >= 0 && options.maxlength <= 18) ? options.maxlength : 18;
            if (parseInt(options.minlength) > parseInt(options.maxlength)) {
                var error = "minlength(" + options.minlength + ") cannot be greather than maxlength(" + options.maxlength + ").";
                throw error;
            }
            options.label = options.label || "Number";
            if (angular.isDefined(options.patternMessage)) {
                //options.messages.pattern = options.patternMessage;
            }

            options.class = options.template === 'percentage' ? 'input-small' : 'input-medium';
        };
        //#endregion

        var linkFunction = function ($scope, $element, $attrs) {
            if (angular.isDefined(scope.field.default)) {
                var getter = $parse($attrs.ngModel);
                var setter = getter.assign;
                setter($scope, scope.field.default);
            }
        };

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions,
            linkFunction: linkFunction
        };
    }]);

//{ label: 'Name', template: 'phone', editable: false, required: true, type: 'text'},
//don't use placeholder with template phone
csapp.factory("csTextFieldFactory", ["Logger", "csBootstrapInputTemplate", "csValidationInputTemplate", function (logManager, bstemplate, valtemplate) {

    var $log = logManager.getInstance("csTextFieldFactory");

    //#region template
    var prefix = function (fields) {
        var html = ' ';
        switch (fields.template) {
            case 'user':
                html += '<div class="input-group"><span class="input-group-addon"><i class="glyphicon glyphicon-user"></i></span>';
                break;
            case 'phone':
                html += '<div class="input-group"><span class="input-group-addon"><i class="glyphicon glyphicon-phone"></i></span><span class="input-group-addon">+91</span>';
                break;
            case 'percentage':
                html += '<div class="input-group">';
                break;
            default:
                break;
        }
        return html;
    };

    var suffix = function (fields) {
        var html = ' ';
        switch (fields.template) {
            case 'user':
                html += '</div>';
                break;
            case 'phone':
                html += '</div>';
                break;
            case 'percentage':
                html += '<span class="input-group-addon"><label>%</label></span></div>';
            default:
                break;
        }
        return html;
    };

    var input = function (field, attrs) {
        var html = '<input  name="myfield" type="text"';
        html += ' ng-model="$parent.' + attrs.ngModel + '"';
        html += angular.isDefined(attrs.ngRequired) ? 'ng-required = "' + attrs.ngRequired + '"' : ' ng-required="' + attrs.field + '.required"';
        html += (attrs.ngDisabled ? ' ng-readonly="' + attrs.ngDisabled + '"' : ' ng-readonly="setReadonly()"');
        html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
        html += ' class ="minWidth form-control" ';
        html += angular.isDefined(attrs.onValidate) ? 'ui-validate="' + attrs.onValidate + '"' : '';
        html += angular.isDefined(field.mask) ? 'ui-mask="{{field.mask}}"' : ' ';
        html += (angular.isDefined(field.minlength) && angular.isUndefined(attrs.typeahead) ? ' ng-minlength="' + field.minlength + '"' : '');
        html += (angular.isDefined(field.maxlength) && angular.isUndefined(attrs.typeahead) ? ' ng-maxlength="' + field.maxlength + '"' : '');
        html += (angular.isDefined(field.pattern) ? ' ng-pattern="' + field.pattern + '"' : '');
        html += (angular.isDefined(field.placeholder) ? ' placeholder="' + field.placeholder + '"' : '');
        html += (angular.isDefined(attrs.ngFocus) ? ' ng-focus="' + attrs.ngFocus + '"' : ' ');
        html += (angular.isDefined(attrs.typeahead) ? ' typeahead="' + attrs.typeahead + '"' : ' ');
        html += (angular.isDefined(attrs.typeahead) ? ' typeahead-min-length="' + field.typeaheadMinLength + '"' : '');
        html += (angular.isDefined(attrs.typeahead) ? ' typeahead-wait-ms="' + field.typeaheadWaitMs + '"' : '');
        html += (angular.isDefined(attrs.typeahead) && attrs.ngChange ? ' typeahead-on-select="' + attrs.ngChange + '"' : '');
        html += '/>';
        return html;
    };



    var configureTypeahead = function (field, attrs) {
        if (angular.isUndefined(attrs.typeahead)) return;
        field.typeaheadMinLength = field.typeaheadMinLength || 3;
        field.typeaheadWaitMs = field.typeaheadWaitMs || 400;
    };

    var htmlTemplate = function (field, attrs) {
        var noBootstrap = angular.isDefined(attrs.noLabel);
        configureTypeahead(field, attrs);
        var template = [
            bstemplate.before(field, noBootstrap, attrs),
            valtemplate.before(field),
            prefix(field),
            input(field, attrs),
            //valdationsIcons(),
            suffix(field),
            valtemplate.after(attrs.field, field),
            bstemplate.after(noBootstrap)
        ].join(' ');
        return template;
    };
    //#endregion

    //#region validations
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
                    //options.length = 10;
                    //options.pattern = "/^[0-9]{10}$/";
                    //options.patternMessage = "Phone number must contain 10 digits.";
                    options.mask = "(999) 999-9999";
                    break;
                case "pan":
                    options.pattern = "/^([A-Z]{5})([0-9]{4})([a-zA-Z]{1})$/";
                    options.patternMessage = "Value not matching with PAN Pattern e.g. ABCDE1234A";
                    break;
                case "user":
                    options.pattern = "/^[0-9]{7}$/";
                    // options.mask = '9999999';
                    options.patternMessage = "UserId must be a 7 digit number";
                    break;
                case "percentage":
                    options.pattern = "/^[0-9]+(\.[0-9][0-9]?)?$/";
                    options.patternMessage = "allows percentage with precision of 2";
                    break;
                default:
                    $log.error(template + " is not defined");
            }
        });
    };

    var validateOptions = function (options) {
        applyTemplates(options);
        options.minlength = options.length || options.minlength || 0;
        options.maxlength = options.length || options.maxlength || 255;
        options.minlength = (options.minlength >= 0 && options.minlength <= 18) ? options.minlength : 0;
        options.maxlength = (options.maxlength >= 0 && options.maxlength <= 255) ? options.maxlength : 255;
        if (parseInt(options.minlength) > parseInt(options.maxlength)) {
            var error = "minlength(" + options.minlength + ") cannot be greather than maxlength(" + options.maxlength + ").";
            throw error;
        }
        options.label = options.label || "Text";
        options.patternMessage = options.patternMessage || "Dosen't follow the specified pattern: " + options.pattern;
    };
    //#endregion

    var linkFunction = function () { };

    return {
        htmlTemplate: htmlTemplate,
        checkOptions: validateOptions,
        linkFunction: linkFunction
    };
}]);

//{ label: 'Password',  editable: false, required: true, type: 'password'},
csapp.factory("csPasswordFieldFactory", ["Logger", "csBootstrapInputTemplate", "csValidationInputTemplate", function (logManager, bstemplate, valtemplate) {

    var $log = logManager.getInstance("csPasswordFieldFactory");

    var input = function (field, attrs) {
        var html = '<input  name="myfield" type="password"';
        html += ' ng-model="$parent.' + attrs.ngModel + '"';
        html += angular.isDefined(attrs.ngRequired) ? 'ng-required = "' + attrs.ngRequired + '"' : ' ng-required="' + attrs.field + '.required"';
        html += ' class ="minWidth form-control"';
        html += (angular.isDefined(field.minlength) ? ' ng-minlength="' + field.minlength + '"' : '');
        html += (angular.isDefined(field.maxlength) ? ' ng-maxlength="' + field.maxlength + '"' : '');
        html += (angular.isDefined(field.pattern) ? ' ng-pattern="' + field.pattern + '"' : '');
        html += (angular.isDefined(field.placeholder) ? ' placeholder="' + field.placeholder + '"' : '');
        html += '/>';
        return html;
    };

    var htmlTemplate = function (field, attrs) {
        var noBootstrap = angular.isDefined(attrs.noLabel);
        var template = [
            bstemplate.before(field, noBootstrap, attrs),
            valtemplate.before(field),
            input(field, attrs),
            valtemplate.after(attrs.field, field),
            bstemplate.after(noBootstrap)
        ].join(' ');
        return template;
    };
    //#endregion

    //#region validations
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
                default:
                    $log.error(template + " is not defined");
            }
        });
    };

    var validateOptions = function (options) {
        applyTemplates(options);
        options.minlength = options.length || options.minlength || 0;
        options.maxlength = options.length || options.maxlength || 255;
        options.minlength = (options.minlength >= 0 && options.minlength <= 18) ? options.minlength : 0;
        options.maxlength = (options.maxlength >= 0 && options.maxlength <= 255) ? options.maxlength : 255;
        if (parseInt(options.minlength) > parseInt(options.maxlength)) {
            var error = "minlength(" + options.minlength + ") cannot be greather than maxlength(" + options.maxlength + ").";
            throw error;
        }
        options.label = options.label || "Password";
        options.patternMessage = options.patternMessage || "Dosen't follow the specified pattern: " + options.pattern;
    };
    //#endregion

    var linkFunction = function () { };

    return {
        htmlTemplate: htmlTemplate,
        checkOptions: validateOptions,
        linkFunction: linkFunction
    };
}]);

//{ label: "label", type: 'textarea', pattern: '/^[a-zA-Z ]{1,100}$/', patternMessage: 'Invalid Name' }
csapp.factory("csTextareaFactory", ["Logger", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function (logManager, bstemplate, valtemplate) {

        //var $log = logManager.getInstance("csTextareaFactory");

        //#region template
        var input = function (field, attrs) {
            var html = '<textarea  name="myfield"';
            html += ' ng-model="$parent.' + attrs.ngModel + '"';
            html += angular.isDefined(attrs.ngRequired) ? 'ng-required = "' + attrs.ngRequired + '"' : ' ng-required="' + attrs.field + '.required"';
            html += (attrs.ngDisabled ? ' ng-readonly="' + attrs.ngDisabled + '"' : ' ng-readonly="setReadonly()"');
            html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
            html += ' class ="minWidth form-control"';
            html += (angular.isDefined(field.minlength) ? ' ng-minlength="' + field.minlength + '"' : '');
            html += (angular.isDefined(field.maxlength) ? ' ng-maxlength="' + field.maxlength + '"' : '');
            html += (angular.isDefined(field.placeholder) ? ' placeholder="' + field.placeholder + '"' : '');
            html += (angular.isDefined(field.pattern) ? ' ng-pattern="' + field.pattern + '"' : '');
            html += (angular.isDefined(field.resize) ? 'class="form-control"' : 'class="form-control noResize"');
            html += '></textarea>';
            return html;
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs),
                valtemplate.before(field),
                input(field, attrs),
                valtemplate.after(attrs.field, field),
                bstemplate.after(noBootstrap)
            ].join(' ');
            return template;
        };
        //#endregion

        //#region validations
        var validateOptions = function (options) {
            options.minlength = options.length || options.minlength || 0;
            options.maxlength = options.length || options.maxlength || 40;
            options.minlength = (options.minlength >= 0 && options.minlength <= 40) ? options.minlength : 0;
            options.maxlength = (options.maxlength >= 0 && options.maxlength <= 40) ? options.maxlength : 40;
            if (parseInt(options.minlength) > parseInt(options.maxlength)) {
                var error = "minlength(" + options.minlength + ") cannot be greather than maxlength(" + options.maxlength + ").";
                throw error;
            }
            options.label = options.label || "Textarea";
            options.patternMessage = options.patternMessage || "Dosen't follow the specified pattern: " + options.pattern;
        };
        //#endregion

        var linkFunction = function () { };

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions,
            linkFunction: linkFunction
        };
    }]);

csapp.factory("csCheckboxFactory", ["Logger", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function (logManager, bstemplate, valtemplate) {

        //#region template
        var input = function (field, attrs) {
            var html = '<input  name="myfield" type="checkbox"';
            html += ' ng-model="$parent.' + attrs.ngModel + '"';
            html += 'style="margin-left: 0"';
            html += angular.isDefined(attrs.ngRequired) ? 'ng-required = "' + attrs.ngRequired + '"' : ' ng-required="' + attrs.field + '.required"';
            html += (attrs.ngReadonly ? ' ng-readonly="' + attrs.ngReadonly + '"' : ' ng-readonly="setReadonly()"');
            html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
            html += (attrs.ngClick ? ' ng-click="' + attrs.ngClick + '"' : '');
            html += '/>';
            return html;
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs),
                valtemplate.before(field),
                input(field, attrs),
                valtemplate.after(attrs.field, field),
                bstemplate.after(noBootstrap)
            ].join(' ');
            return template;
        };
        //#endregion

        //#region validations
        var validateOptions = function (options) {
            options.label = options.label || "CheckBox";
        };
        //#endregion

        var linkFunction = function () { };

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions,
            linkFunction: linkFunction
        };
    }]);

//{ label: "Email", type:'email'  patternMessage: 'Invalid Email' };
csapp.factory("csEmailFactory", ["Logger", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function (logManager, bstemplate, valtemplate) {

        //#region template
        var prefix = function (field) {

            var hasSuffix = angular.isDefined(field.suffix) && field.suffix !== null && field.suffix.length > 0;

            var html = '<div class="input-group';
            html += hasSuffix ? ' input-group">' : '">';
            html += '<span class="input-group-addon"><i class="glyphicon glyphicon-envelope"></i></span>';
            return html;

        };

        var addEmailSuffix = function (fields) {
            var string = ' ';
            var hasSuffix = angular.isDefined(fields.suffix) && fields.suffix !== null && fields.suffix.length > 0;

            if (hasSuffix) {
                string += 'class="input-medium" cs-input-suffix suffix="' + fields.suffix + '" />';
            } else {
                string += 'class="input-large" />';
            }

            if (hasSuffix) {
                string += '<span class="input-group-addon">' + fields.suffix + '</span>';
            }
            return string;
        };

        var suffix = function () {
            return '</div>';
        };

        var input = function (field, attrs) {
            var html = '<input  name="myfield" type="email"';
            html += ' ng-model="$parent.' + attrs.ngModel + '"';
            html += ' class ="minWidth form-control"';
            html += angular.isDefined(attrs.ngRequired) ? 'ng-required = "' + attrs.ngRequired + '"' : ' ng-required="' + attrs.field + '.required"';
            html += (attrs.ngReadonly ? ' ng-readonly="' + attrs.ngReadonly + '"' : ' ng-readonly="setReadonly()"');
            html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
            html += (angular.isDefined(field.minlength) ? ' ng-minlength="' + field.minlength + '"' : '');
            html += (angular.isDefined(field.maxlength) ? ' ng-maxlength="' + field.maxlength + '"' : '');
            html += (angular.isDefined(field.pattern) ? ' ng-pattern="' + field.pattern + '"' : '');
            html += (angular.isDefined(field.placeholder) ? ' placeholder="' + field.placeholder + '"' : '');
            html += addEmailSuffix(field);
            //html += '/>';
            return html;
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs),
                valtemplate.before(field),
                prefix(field),
                input(field, attrs),
                suffix(field),
                valtemplate.after(attrs.field, field),
                bstemplate.after(noBootstrap)
            ].join(' ');
            return template;
        };
        //#endregion

        //#region validations
        var validateOptions = function (options) {
            options.minlength = options.length || options.minlength || 0;
            options.maxlength = options.length || options.maxlength || 45;
            options.minlength = (options.minlength >= 0 && options.minlength <= 45) ? options.minlength : 0;
            options.maxlength = (options.maxlength >= 0 && options.maxlength <= 45) ? options.maxlength : 45;
            if (parseInt(options.minlength) > parseInt(options.maxlength)) {
                var error = "minlength(" + options.minlength + ") cannot be greather than maxlength(" + options.maxlength + ").";
                throw error;
            }
            options.label = options.label || "Email";
            options.patternMessage = options.patternMessage || "Dosen't follow the specified pattern: " + options.pattern;
        };
        //#endregion

        var linkFunction = function () { };

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions,
            linkFunction: linkFunction
        };
    }]);

//{label: 'Radio', valueField: 'value', textField: 'display', editable: false, required: true, type: 'radio', options: arrayOfObjects }
csapp.factory("csRadioButtonFactory", ["Logger", "csBootstrapInputTemplate",
    function (logManager, bstemplate) {

        var input = function (field, attrs) {

            var html = '<div class="row radio-margin"';
            html += angular.isDefined(field.defaultValue) ? 'ng-init="$parent.' + attrs.ngModel + ' = field.defaultValue">' : '>';
            html += '<span class="text-right" ng-repeat="record in  field.options" > ';
            html += '<input type="radio"';
            html += ' ng-model="$parent.' + attrs.ngModel + '"';
            html += angular.isDefined(attrs.ngRequired) ? 'ng-required = "' + attrs.ngRequired + '"' : ' ng-required="' + attrs.field + '.required"';
            html += ' ng-value="' + field.valueField + '"';
            html += (attrs.ngDisabled ? ' ng-Disabled="' + attrs.ngDisabled + '"' : ' ng-Disabled="setReadonly()"');
            html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
            html += (attrs.ngClick ? ' ng-click="' + attrs.ngClick + '"' : '');
            html += '/><label>{{' + field.textField + '}}</label>';
            html += '</span></div>';
            return html;
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs),
                input(field, attrs),
                bstemplate.after(noBootstrap)
            ].join(' ');
            return template;
        };

        var validateOptions = function (field) {
            field.label = field.label || "Description";
            field.defaultValue = field.required === true ? field.options[0][field.valueField].toString() : undefined;

            if (angular.isDefined(field.valueField)) {
                if (field.valueField.substring(0, 6) !== "record") {
                    field.valueField = "record." + field.valueField;
                }
            } else {
                field.valueField = "record";
            }

            if (angular.isDefined(field.textField)) {
                if (field.textField.substring(0, 6) !== "record") {
                    field.textField = "record." + field.textField;
                }
            } else {
                field.textField = "record";
            }

        };

        var linkFunction = function () { };

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions,
            linkFunction: linkFunction
        };

    }]);

//{ name: 'select', label: 'select', csRepeat: 'objectArrayNameToBeRepeated',textField:'propertyToBeDisplayed',valueField:'propertyToBeBound', editable: false, required: true, type: 'select'},
csapp.factory("csSelectField", ["$csfactory", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function ($csfactory, bstemplate, valtemplate) {

        var input = function (field, attr) {
            var html = '<select  name="myfield" ';
            html += ' ng-model="$parent.' + attr.ngModel + '" class="minWidth form-control" ';
            html += ' ng-options="' + field.ngOptions + '"';
            html += angular.isDefined(attr.ngRequired) ? 'ng-required = "' + attr.ngRequired + '"' : ' ng-required="' + attr.field + '.required"';
            html += (attr.ngChange ? ' ng-change="' + attr.ngChange + '"' : '');
            html += (attr.multiple) ? 'multiple = "multiple" ' : '';
            html += (attr.ngDisabled ? ' ng-disabled="' + attr.ngDisabled + '"' : ' ng-disabled="setReadonly()"');
            html += '>';
            html += '<option value=""></option>';
            html += '</select> ';
            return html;
        };

        var validateOptions = function (field, attr) {

            field.label = field.label || "SelectBox";

            if (angular.isDefined(field.valueField)) {
                if (field.valueField.substring(0, 3) !== "row") {
                    field.valueField = "row." + field.valueField;
                }
            } else {
                field.valueField = "row";
            }

            if (angular.isDefined(field.textField)) {
                if (field.textField.substring(0, 3) !== "row") {
                    field.textField = "row." + field.textField;
                }
            } else {
                field.textField = "row";
            }

            field.ngOptions = field.valueField + ' as ' + field.textField;
            field.ngOptions += ' for row in ';
            field.ngOptions += attr.valueList ? attr.valueList : ' field.valueList';
            field.ngOptions += attr.trackBy ? ' track by row.' + attr.trackBy : ' ';

            var valueList = attr.valueList ? attr.valueList : 'field.valueList';
            field.ngRepeat = '<option data-ng-repeat="row in ' + valueList + '"  value="{{' + field.valueField + '}}">{{' + field.textField + '}}</option>';
            field.select3Options = {
                initPristrine: true,
                allowClear: field.allowClear || false,
            };
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs),
                valtemplate.before(field),
                input(field, attrs),
                valtemplate.after(attrs.field, field),
                bstemplate.after(noBootstrap)
            ].join(' ');
            return template;
        };

        var linkFunction = function () { };

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions,
            linkFunction: linkFunction
        };
    }]);

//{ name: 'enum', label: 'enum', editable: false, csRepeat: 'arrayNameToBeRepeated', required: true, type: 'enum'},
csapp.factory("csEnumFactory", ["$csfactory", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function ($csfactory, bstemplate, valtemplate) {

        var input = function (field, attr) {
            var html = '<select  name="myfield" ng-options="' + field.ngOptions + '"';
            html += ' ng-model="$parent.' + attr.ngModel + '" class="form-control minWidth" ';
            html += angular.isDefined(attr.ngRequired) ? 'ng-required = "' + attr.ngRequired + '"' : ' ng-required="' + attr.field + '.required"';
            html += (attr.ngChange ? ' ng-change="' + attr.ngChange + '"' : '');
            html += (attr.ngDisabled ? ' ng-disabled="' + attr.ngDisabled + '"' : ' ng-disabled="setReadonly()"');
            html += '>';
            html += '<option value="" disabled="true" selected="false"></option>';
            html += '</select> ';

            return html;
        };

        var validateOptions = function (field, attr) {
            field.label = field.label || "Select";
            field.ngOptions = 'row for row in ';
            field.ngOptions += attr.valueList ? attr.valueList : ' field.valueList';
            field.ngOptions += " track by row";
            field.select3Options = {
                initPristrine: true,
                allowClear: field.allowClear || false,
            };
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs),
                valtemplate.before(field),
                input(field, attrs),
                valtemplate.after(attrs.field, field),
                bstemplate.after(noBootstrap)
            ].join(' ');
            return template;
        };

        var linkFunction = function () { };

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions,
            linkFunction: linkFunction
        };
    }]);

//{ label: 'Datepicker',  min:"+2d",template:"MonthPicker" max: "1y", default: "+10d",  type: 'date',defaultDate:'today'},
csapp.factory("csDateFactory", ["$csfactory", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function ($csfactory, bstemplate, valtemplate) {

        var openDatePicker = function ($event, field) {
            $event.preventDefault();
            $event.stopPropagation();
            field.opened = !field.opened;
        };

        var disableDate = function (date, field) {
            return (field.daysOfWeekDisabled.indexOf(date.getDay()) !== -1);
        };

        var input = function (field, attr) {
            var html = '<p class="input-group"';
            html += angular.isDefined(field.dateOptions.defaultDate) ? 'data-ng-init="$parent.' + attr.ngModel + ' = field.dateOptions.defaultDate">' : '>';
            html += '<input type="text" class="form-control" disabled="disabled"';
            html += 'datepicker-popup="' + field.format + '" ng-model="$parent.' + attr.ngModel + '" ';//datepicker-popup="' + field.format + '"
            html += 'is-open="field.opened" show-button-bar="field.showButtons"  datepicker-options="field.dateOptions"';
            html += (angular.isDefined(attr.ngRequired) ? 'ng-required = "' + attr.ngRequired + '"' : ' ng-required="' + attr.field + '.required"');
            html += (attr.ngChange ? ' ng-change="' + attr.ngChange + '"' : '');
            html += (angular.isDefined(field.placeholder) ? ' placeholder="' + field.placeholder + '"' : '');
            html += angular.isDefined(attr.ngRequired) ? 'ng-required = "' + attr.ngRequired + '"' : ' ng-required="' + attr.field + '.required"';
            html += angular.isDefined(field.daysOfWeekDisabled) ? 'date-disabled="field.disableDate(date,field)"' : ' ';
            html += '/>';
            html += '<span class="input-group-btn">';
            html += '<button type="button" class="btn btn-default"';
            html += (attr.ngDisabled ? ' ng-disabled="' + attr.ngDisabled + '"' : ' ng-disabled="setReadonly()"');
            html += ' ng-click="field.open($event,field);"><i class="glyphicon glyphicon-calendar"></i>';
            html += '</button>';
            html += '</span>';
            html += '</p>';

            return html;
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs),
                valtemplate.before(field),
                input(field, attrs),
                valtemplate.after(attrs.field, field),
                bstemplate.after(noBootstrap)
            ].join(' ');
            return template;
        };

        var applyTemplate = function (field) {
            if (angular.isUndefined(field.template) || field.template === null) {
                return;
            }

            var tmpl = field.template.split(",").filter(function (str) { return str !== ''; });
            angular.forEach(tmpl, function (template) {
                if (template.length < 1) return;
                switch (template) {
                    case "MonthPicker":
                        field.minViewMode = "month";
                        break;
                    case "YearPicker":
                        field.minViewMode = "year";
                        break;
                    case "future":
                        field.min = "tomorrow";
                        if (field.minViewMode === "month")
                            field.min = "next-month";
                        if (field.minViewMode === "year")
                            field.min = "next-year";
                        break;
                    case "past":
                        field.max = "yesterday";
                        if (field.minViewMode === "month")
                            field.max = "prev-month";
                        if (field.minViewMode === "year")
                            field.max = "prev-year";
                        break;
                    default:
                        console.error(template + " is not defined.");
                }

                return;
            });
        };

        var manageViewMode = function (field) {
            field.dateOptions = {
                showWeeks: false,
            };
            if ($csfactory.isNullOrEmptyString(field.minViewMode))
                field.minViewMode = '';
            switch (field.minViewMode.toUpperCase()) {
                case "MONTH":
                    field.format = "yyyy-MM";
                    field.showButtons = false;
                    field.dateOptions.datepickerMode = "'month'";
                    field.dateOptions.minMode = 'month';
                    field.dateOptions.minDate = angular.isDefined(field.minDate) ? "'" + field.minDate + "'" : "'1900-01'";
                    field.dateOptions.maxDate = angular.isDefined(field.maxDate) ? "'" + field.maxDate + "'" : "'2200-12'";
                    break;
                case "YEAR":
                    field.format = "yyyy";
                    field.showButtons = false;
                    field.dateOptions.datepickerMode = "'year'";
                    field.dateOptions.minMode = 'year';
                    field.dateOptions.minDate = angular.isDefined(field.minDate) ? "'" + field.minDate + "'" : "'1900'";
                    field.dateOptions.maxDate = angular.isDefined(field.maxDate) ? "'" + field.maxDate + "'" : "'2200'";
                    break;
                default:
                    field.minViewMode = 'DAY';
                    field.format = "yyyy-MM-dd";
                    field.showButtons = true;
                    field.dateOptions.datepickerMode = "'day'";
                    field.dateOptions.minMode = 'day';
                    field.dateOptions.minDate = angular.isDefined(field.minDate) ? "'" + field.minDate + "'" : "'1900-01-01'";
                    field.dateOptions.maxDate = angular.isDefined(field.maxDate) ? "'" + field.maxDate + "'" : "'2200-12-31'";
            }
        };

        var parseLogicalDate = function (dateParams) {
            var newDateParams = dateParams.match(/[a-zA-Z]+|[-+0-9]+/g);
            var addBy = parseInt(newDateParams[0]);
            if (isNaN(addBy)) throw "date param is not valid :" + dateParams;

            var addParam = newDateParams[1];
            if (!isNaN(addParam)) throw "date param is not valid :" + dateParams;

            switch (addParam) {
                case 'd':
                case 'day':
                case 'days':
                    return moment().add('d', addBy).format("YYYY-MM-DD");
                case 'm':
                case 'month':
                case 'months':
                    return moment().add('M', addBy).format("YYYY-MM-DD");
                case 'y':
                case 'year':
                case 'years':
                    return moment().add('y', addBy).format("YYYY-MM-DD");
                default:
                    throw "date param is not valid :" + dateParams;
            }
        };

        var parseDate = function (dateParams) {
            var date = moment(dateParams);
            if (date.isValid()) return date.format("YYYY-MM-DD");

            switch (dateParams) {
                case "today":
                    return moment().format("YYYY-MM-DD");
                case "tomorrow":
                    return moment().add('days', 1).format("YYYY-MM-DD");
                case "yesterday":
                    return moment().subtract('days', 1).format("YYYY-MM-DD");
                case "next-month":
                    return moment().add('months', 1).format("YYYY-MM");
                case "prev-month":
                    return moment().subtract('months', 1).format("YYYY-MM");
                case "next-year":
                    return moment().add('years', 1).format("YYYY");
                case "prev-year":
                    return moment().subtract('years', 1).format("YYYY");
                default:
                    return parseLogicalDate(dateParams);
            }
        };

        var parseDates = function (field) {
            if (!$csfactory.isNullOrEmptyString(field.min)) field.dateOptions.minDate = "'" + parseDate(field.min) + "'";
            if (!$csfactory.isNullOrEmptyString(field.max)) field.dateOptions.maxDate = "'" + parseDate(field.max) + "'";
            if (!$csfactory.isNullOrEmptyString(field.defaultDate)) field.dateOptions.defaultDate = parseDate(field.defaultDate);

            if (moment(field.dateOptions.maxDate).isBefore(field.dateOptions.minDate))
                throw "start date: " + field.dateOptions.minDate + " is greater than end date" + field.dateOptions.maxDate;

            if (angular.isDefined(field.dateOptions.defaultDate) &&
                moment(field.dateOptions.defaultDate).isBefore(field.dateOptions.minDate))
                throw "start date: " + field.dateOptions.minDate
                    + " is greater than default date" + field.dateOptions.defaultDate;

            if (angular.isDefined(field.dateOptions.defaultDate) &&
                moment(field.dateOptions.maxDate).isBefore(field.dateOptions.defaultDate))
                throw "default date: " + field.dateOptions.defaultDate
                    + " is greater than end date" + field.dateOptions.maxDate;
        };

        var validateOptions = function (field) {
            applyTemplate(field);
            manageViewMode(field);
            parseDates(field);

            field.opened = false;
            field.open = openDatePicker;
            field.disableDate = disableDate; //fn ptr

            if ($csfactory.isNullOrEmptyString(field.label)) {
                field.label = "Date";
            }

            if ($csfactory.isNullOrEmptyString(field.daysOfWeekDisabled)) {
                field.daysOfWeekDisabled = [];
            }
        };

        var linkFunction = function () { };

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions,
            linkFunction: linkFunction
        };

    }]);

csapp.directive('csField', ["$compile", "$parse", "csNumberFieldFactory", "csTextFieldFactory", "csTextareaFactory", "csEmailFactory", "csCheckboxFactory", "csRadioButtonFactory", "csSelectField", "csEnumFactory", "csBooleanFieldFactory", "csDateFactory", "csPasswordFieldFactory", "$csfactory", "csFileFieldFactory",
    function ($compile, $parse, numberFactory, textFactory, textareaFactory, emailFactory, checkboxFactory, radioFactory, selectFactory, enumFactory, boolFactory, dateFactory, passwordFactory, $csfactory, fileFactory) {

        var getFactory = function (type) {
            switch (type) {
                case "textarea":
                    return textareaFactory;
                case "number":
                    return numberFactory;
                case "text":
                    return textFactory;
                case "email":
                    return emailFactory;
                case "checkbox":
                    return checkboxFactory;
                case "radio":
                    return radioFactory;
                case "select":
                    return selectFactory;
                case "enum":
                    return enumFactory;
                case 'date':
                    return dateFactory;
                case 'password':
                    return passwordFactory;
                case 'bool':
                case 'boolean':
                    return boolFactory;
                case 'file':
                    return fileFactory;
                default:
                    throw "Invalid type specification in csField directive : " + type;
            }
        };

        var controllerFn = function ($scope, $element, $attrs) {
            var fieldGetter = $parse($attrs.field);
            var field = fieldGetter($scope);
            $scope.setReadonly = function () {
                switch ($scope.mode) {
                    case 'add':
                        return false;
                    case 'view':
                        return true;
                    case 'edit':
                        return field.editable === false;
                    default:
                        return false;
                }
            };
        };

        var setLayout = function (field, csFormCtrl, attr) {
            field.size = {};
            if (angular.isUndefined(csFormCtrl)) {
                field.size = { label: 4, div: 6, control: 8 };
                return;
            }

            field.size = csFormCtrl.getSize();

            if (angular.isUndefined(attr.layout)) {
                return;
            }

            var layout = attr.layout.split(".");
            var size = {
                div: angular.isUndefined(layout[0]) ? -1 : parseInt(layout[0]),
                label: angular.isUndefined(layout[1]) ? -1 : parseInt(layout[1]),
                control: angular.isUndefined(layout[2]) ? -1 : parseInt(layout[2]),
            };

            size.div = (isNaN(size.div) || size.div < 1 || size.div > 12) ? field.size.div : size.div;
            size.label = (isNaN(size.label) || size.label < 0 || size.label > 12) ? field.size.label : size.label;
            size.control = (isNaN(size.control) || size.control < 1 || size.control > 12) ? field.size.control : size.control;
            field.size = size;

            return;
        };

        var setLayoutClasses = function (field) {
            field.size.nolabel = field.size.label === 0;

            field.layoutClass = {
                label: field.size.nolabel ? ' ' : 'col-md-' + field.size.label,
                div: 'col-md-' + field.size.div,
                control: 'col-md-' + field.size.control,
            };
        };

        var setValidateParam = function (validateParam) {

            switch (validateParam) {
                case 'on':
                case 'enable':
                case 'true':
                    return true;

                case 'off':
                case 'disable':
                case 'false':
                    return false;

                default:
                    return true;
            }

        };

        var setValidation = function (field, attrs, csFormCntrl) {

            var validateParam = $csfactory.isNullOrEmptyString(attrs.validation) ? csFormCntrl.validation : attrs.validation;
            field.validate = csFormCntrl.mode == 'view' ? false : setValidateParam(validateParam);
            if (!field.validate) {
                field.minlength = undefined;
                field.maxlength = undefined;
                field.pattern = undefined;
                field.required = false;
                attrs.required = false;
                field.min = undefined;
                field.max = undefined;
            }
        };

        var setDefaultValue = function (scope, $attrs) {
            if (angular.isUndefined(scope.field.defaultValue)) return;
            var getter = $parse($attrs.ngModel);
            var setter = getter.assign;
            setter(scope, scope.field.defaultValue);
        };

        var linkFunction = function (scope, element, attrs, ctrl) {
            var controllers = {
                ngModelCtrl: ctrl[0],
                formCtrl: ctrl[1],
                csFormCtrl: ctrl[2]
            };

            var fieldGetter = $parse(attrs.field);
            scope.field = fieldGetter(scope);
            if (angular.isUndefined(scope.field)) {
                console.error("field cannot be undefined : " + attrs.field);
                throw new "Invalid field exception";
            }

            scope.mode = angular.isDefined(controllers.csFormCtrl) ? controllers.csFormCtrl.mode : '';
            setLayout(scope.field, controllers.csFormCtrl, attrs);
            setLayoutClasses(scope.field);
            setValidation(scope.field, attrs, controllers.csFormCtrl);

            var typedFactory = getFactory(scope.field.type);
            typedFactory.checkOptions(scope.field, attrs);
            setDefaultValue(scope, attrs);
            typedFactory.linkFunction(scope, element, attrs, ctrl);

            var html = typedFactory.htmlTemplate(scope.field, attrs);
            var newElem = angular.element(html);
            element.replaceWith(newElem);
            $compile(newElem)(scope);
        };

        return {
            restrict: 'E',
            link: linkFunction,
            scope: true,
            require: ['ngModel', '^form', '^csForm'],
            terminal: true,
            controller: controllerFn
        };
    }]);

csapp.directive('csForm', function () {

    var cntrlFn = function ($scope, $element, $attrs) {

        $scope.layout = angular.isDefined($scope.layout) ? $scope.layout.split(".") : [6, 4, 8];

        var size = {
            div: angular.isUndefined($scope.layout[0]) ? 6 : parseInt($scope.layout[0]),
            label: angular.isUndefined($scope.layout[1]) ? 4 : parseInt($scope.layout[1]),
            control: angular.isUndefined($scope.layout[2]) ? 8 : parseInt($scope.layout[2]),
        };

        size.div = (isNaN(size.div) || size.div < 1 || size.div > 12) ? 6 : size.div;
        size.label = (isNaN(size.label) || size.label < 0 || size.label > 12) ? 4 : size.label;
        size.control = (isNaN(size.control) || size.control < 1 || size.control > 12) ? 8 : size.control;

        this.mode = $scope.mode;
        this.validation = $attrs.validation;
        this.getSize = function () {
            return angular.copy(size);
        };
    };

    var templateFn = function () {
        var template = '<div class="clearfix cs-forms">' +
            '<div ng-transclude="">' +
            '</div>' +
            '</div>';
        return template;
    };

    return {
        restrict: 'E',
        transclude: true,
        template: templateFn,
        scope: { layout: '@', mode: '=' },
        controller: cntrlFn,
        require: '^form'
    };
});


