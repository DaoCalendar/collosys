csapp.factory("csBootstrapInputTemplate", function () {

    var bsTemplateBefore = function (field, noBootstrap, attr) {

        var noBootstrapDiv = '<div style="margin-bottom: 5px"' + (attr.ngShow ? ' ng-show="' + attr.ngShow + '"' : '');
        noBootstrapDiv += (attr.ngHide ? ' ng-hide="' + attr.ngHide + '"' : '');
        noBootstrapDiv += (attr.ngIf ? ' ng-if="' + attr.ngIf + '"' : '');
        noBootstrapDiv += '>';

        var html = noBootstrapDiv;
        html += '<label class="col-md-';
        html += angular.isDefined(field.labelSize) ? field.labelSize : 4;
        html += ' control-label">{{' + attr.field + '.label}}' +
        '<span class="text-danger">{{' + attr.field + '.required ? " *":""}}</span></label>';
        return (noBootstrap || field.labelSize === 0 ? noBootstrapDiv : html);
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
        var html = '<div ng-form="myform" role="form" class="col-md-';
        html += angular.isDefined(field.controlSize) ? field.controlSize : 8;
        html += '">';
        return html;
    };

    var getmessages = function (fieldname, field) {
        field.messages = {
            required: '{{' + fieldname + '.label}} is required.',
            pattern: (field.patternMessage) ? field.patternMessage : '{{' + fieldname + '.label}} is not matching with pattern {{' + fieldname + '.pattern}}.',
            minlength: '{{' + fieldname + '.label}} should have atleast {{' + fieldname + '.minlength}} character/s.',
            maxlength: '{{' + fieldname + '.label}} can have maximum {{' + fieldname + '.maxlength}} character/s.',
            min: '{{' + fieldname + '.label}} cannot be less than {{' + fieldname + '.min}}.',
            max: '{{' + fieldname + '.label}} cannot be greater than {{' + fieldname + '.max}}.'
        };
    };

    var after = function (fieldname, field) {
        getmessages(fieldname, field);
        var html = '<div data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty"> ' +
            '<div class="text-danger" data-ng-show="myform.myfield.$error.required">' + field.messages.required + '</div>' +
            '<div class="text-danger" data-ng-show="myform.myfield.$error.pattern">' + field.messages.pattern + '</div>' +
            '<div class="text-danger" data-ng-show="myform.myfield.$error.minlength">' + field.messages.minlength + '</div>' +
            '<div class="text-danger" data-ng-show="myform.myfield.$error.maxlength">' + field.messages.maxlength + '</div>' +
            '<div class="text-danger" data-ng-show="myform.myfield.$error.min">' + field.messages.min + '</div>' +
            '<div class="text-danger" data-ng-show="myform.myfield.$error.max">' + field.messages.max + '</div>' +
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


        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions
        };
    }]);

csapp.factory("csNumberFieldFactory", ["Logger", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function (logManager, bstemplate, valtemplate) {

        var $log = logManager.getInstance("csNumberFieldFactory");

        var prefix = function (fields) {
            var html = ' ';
            switch (fields.template) {
                case 'rupee':
                    html += '<div class="input-group"><span class="input-group-addon"><i class="glyphicon glyphicon-rupee"></i></span>';
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
            html += 'class ="form-control ' + (attrs.class ? attrs.class : field.class) + '"';
            html += ((field.type === "decimal") ? ' step="any"' : '');
            html += (angular.isDefined(field.minlength) ? ' ng-minlength="' + field.minlength + '"' : '');
            html += (angular.isDefined(field.maxlength) ? ' ng-maxlength="' + field.maxlength + '"' : '');
            html += (angular.isDefined(field.min) ? ' min="' + field.min + '"' : '');
            html += (angular.isDefined(field.max) ? ' max="' + field.max + '"' : '');
            html += (angular.isDefined(field.pattern) ? ' ng-pattern="' + field.pattern + '"' : '');
            html += (angular.isDefined(field.placeholder) ? ' placeholder="' + field.placeholder + '"' : '');
            html += (angular.isDefined(attrs.typeahead) ? 'typeahead="' + attrs.typeahead + '"' : ' ');
            html += (angular.isDefined(attrs.typeahead) ? 'typeahead-min-length="' + field.typeaheadMinLength + '"' : '');
            html += (angular.isDefined(attrs.typeahead) ? 'typeahead-wait-ms="' + field.typeaheadWaitMs + '"' : '');
            html += (angular.isDefined(attrs.typeahead) && attrs.ngChange ? 'typeahead-on-select="' + attrs.ngChange + '"' : '');
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

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions
        };
    }]);

//{ label: 'Name', template: 'phone', editable: false, required: true, type: 'text'},
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
        html += 'class ="form-control ' + (attrs.class ? attrs.class : field.class) + '"';
        html += (angular.isDefined(field.minlength) && angular.isUndefined(attrs.typeahead) ? ' ng-minlength="' + field.minlength + '"' : '');
        html += (angular.isDefined(field.maxlength) && angular.isUndefined(attrs.typeahead) ? ' ng-maxlength="' + field.maxlength + '"' : '');
        html += (angular.isDefined(field.pattern) ? ' ng-pattern="' + field.pattern + '"' : '');
        html += (angular.isDefined(field.placeholder) ? ' placeholder="' + field.placeholder + '"' : '');
        html += (angular.isDefined(attrs.typeahead) ? 'typeahead="' + attrs.typeahead + '"' : ' ');
        html += (angular.isDefined(attrs.typeahead) ? 'typeahead-min-length="' + field.typeaheadMinLength + '"' : '');
        html += (angular.isDefined(attrs.typeahead) ? 'typeahead-wait-ms="' + field.typeaheadWaitMs + '"' : '');
        html += (angular.isDefined(attrs.typeahead) && attrs.ngChange ? 'typeahead-on-select="' + attrs.ngChange + '"' : '');
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
                    options.mask = "(999) 999-9999";
                    break;
                case "pan":
                    options.pattern = "/^([A-Z]{5})(\d{4})([a-zA-Z]{1})$/";
                    options.patternMessage = "Value not matching with PAN Pattern e.g. ABCDE1234A";
                    break;
                case "user":
                    options.pattern = "/^[0-9]{7}$/";
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

    return {
        htmlTemplate: htmlTemplate,
        checkOptions: validateOptions
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
            html += 'class ="form-control ' + (attrs.class ? attrs.class : field.class) + '"';
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
            options.maxlength = options.length || options.maxlength || 18;
            options.minlength = (options.minlength >= 0 && options.minlength <= 18) ? options.minlength : 0;
            options.maxlength = (options.maxlength >= 0 && options.maxlength <= 18) ? options.maxlength : 18;
            if (parseInt(options.minlength) > parseInt(options.maxlength)) {
                var error = "minlength(" + options.minlength + ") cannot be greather than maxlength(" + options.maxlength + ").";
                throw error;
            }
            options.label = options.label || "Textarea";
            options.patternMessage = options.patternMessage || "Dosen't follow the specified pattern: " + options.pattern;
        };
        //#endregion

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions
        };
    }]);

csapp.factory("csCheckboxFactory", ["Logger", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function (logManager, bstemplate, valtemplate) {

        //var $log = logManager.getInstance("csCheckboxFactory");

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

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions
        };
    }]);

//{ label: "Email", type:'email'  patternMessage: 'Invalid Email' };
csapp.factory("csEmailFactory", ["Logger", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function (logManager, bstemplate, valtemplate) {

        //var $log = logManager.getInstance("csEmailFactory");

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
            html += 'class ="form-control ' + (attrs.class ? attrs.class : field.class) + '"';
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

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions
        };
    }]);

//{label: 'Radio', valueField: 'value', textField: 'display', editable: false, required: true, type: 'radio', options: arrayOfObjects }
csapp.factory("csRadioButtonFactory", ["Logger", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function (logManager, bstemplate, valtemplate) {

        var input = function (field, attrs) {

            var html = '<div class="row">';
            html += '<div class="col-md-5 radio" ng-repeat="(key, record) in  field.options ">';
            html += '<label><input name="myfield" type="radio"';
            html += ' ng-model="$parent.' + attrs.ngModel + '"';
            html += ' style="margin-left: 0"';
            html += angular.isDefined(attrs.ngRequired) ? 'ng-required = "' + attrs.ngRequired + '"' : ' ng-required="' + attrs.field + '.required"';
            html += ' ng-value="' + field.valueField + '"';
            html += (attrs.ngDisabled ? ' ng-Disabled="' + attrs.ngDisabled + '"' : ' ng-Disabled="setReadonly()"');
            html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
            html += (attrs.ngClick ? ' ng-click="' + attrs.ngClick + '"' : '');
            html += '/>{{' + field.textField + '}}</label>';
            html += '</div></div>';
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



        var validateOptions = function (field) {
            field.label = field.label || "Description";
            //field.textField = "record." + (field.textField || "text");

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
        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions
        };

    }]);

//{ name: 'select', label: 'select', csRepeat: 'objectArrayNameToBeRepeated',textField:'propertyToBeDisplayed',valueField:'propertyToBeBound', editable: false, required: true, type: 'select'},
csapp.factory("csSelectField", ["$csfactory", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function ($csfactory, bstemplate, valtemplate) {

        var input = function (field, attr) {
            var html = '<select  name="myfield"  ui-select2="field.select2Options"';
            //html += attr.valueList ? 'chosen="' + attr.valueList + '"' : ' chosen = "field.valueList"';
            html += ' ng-model="$parent.' + attr.ngModel + '"';
            html += (attr.class) ? 'class =" ' + attr.class + '"' : ' style="width: 100%;" ';
            html += (field.useRepeat !== true) ? ' ng-options="' + field.ngOptions + '"' : ' ';
            html += angular.isDefined(attr.ngRequired) ? 'ng-required = "' + attr.ngRequired + '"' : ' ng-required="' + attr.field + '.required"';
            html += (attr.ngChange ? ' ng-change="' + attr.ngChange + '"' : '');
            html += (attr.multiple) ? 'multiple = "multiple" ' : '';
            html += (attr.ngDisabled ? ' ng-disabled="' + attr.ngDisabled + '"' : ' ng-disabled="setReadonly()"');
            html += '>';
            html += '<option value=""></option>';
            html += field.useRepeat === true ? field.ngRepeat : ' ';
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

            if (field.useRepeat !== true) {
                field.ngOptions = field.valueField + ' as ' + field.textField;
                field.ngOptions += ' for row in ';
                field.ngOptions += attr.valueList ? attr.valueList : ' field.valueList';
                field.ngOptions += attr.trackBy ? ' track by row.' + attr.trackBy : ' ';
            } else {
                var valueList = attr.valueList ? attr.valueList : 'field.valueList';
                field.ngRepeat = '<option data-ng-repeat="row in ' + valueList + '"  value="{{' + field.valueField + '}}">{{' + field.textField + '}}</option>';
            }

            field.select2Options = {
                //more options here
                setPristine: true,
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

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions
        };
    }]);

//{ name: 'enum', label: 'enum', editable: false, csRepeat: 'arrayNameToBeRepeated', required: true, type: 'enum'},
csapp.factory("csEnumFactory", ["$csfactory", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function ($csfactory, bstemplate, valtemplate) {

        var input = function (field, attr) {
            var html = '<select  name="myfield" data-ui-select3="field.select3Options"';
            html += ' ng-model="$parent.' + attr.ngModel + '"';
            //html += ' ng-options="' + field.ngOptions + '"';
            html += (attr.class) ? 'class =" ' + attr.class + '"' : ' style="width: 100%;" ';
            html += (attr.multiple) ? 'multiple = "multiple" ' : '';
            html += angular.isDefined(attr.ngRequired) ? 'ng-required = "' + attr.ngRequired + '"' : ' ng-required="' + attr.field + '.required"';
            html += (attr.ngChange ? ' ng-change="' + attr.ngChange + '"' : '');
            html += (attr.ngDisabled ? ' ng-disabled="' + attr.ngDisabled + '"' : ' ng-disabled="setReadonly()"');
            html += '>';
            html += '<option value="" disabled="true" selected="false"></option>';
            html += '<option data-ng-repeat="row in ' + (attr.valueList ? attr.valueList : 'field.valueList') + '"  value="{{row}}">{{row}}</option>';
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

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions
        };
    }]);

csapp.factory("csDateFactory", ["$csfactory", "csBootstrapInputTemplate", "csValidationInputTemplate", function ($csfactory, bstemplate, valtemplate) {

    //options: label, placeholder, required, readonly, end-date, start-date, date-format, date-min-view-mode, days-of-week-disabled
    var input = function (field, attr) {
        var html = '<div class="input-group">';
        html += '<input type="text" name="myfield" class="form-control" ng-readonly="true"';
        html += ' ng-model="$parent.' + attr.ngModel + '"';
        html += angular.isDefined(attr.ngRequired) ? 'ng-required = "' + attr.ngRequired + '"' : ' ng-required="' + attr.field + '.required"';
        html += (attr.ngChange ? ' ng-change="' + attr.ngChange + '"' : '');
        html += (angular.isDefined(field.placeholder) ? ' placeholder="' + field.placeholder + '"' : '');
        html += (angular.isDefined(field.minViewMode) ? ' data-date-min-view-mode="' + field.minViewMode + '"' : '');
        html += (angular.isDefined(field.daysOfWeekDisabled) ? ' data-date-days-of-week-disabled="' + field.daysOfWeekDisabled + '"' : '');
        html += (angular.isDefined(field.format) ? ' data-date-format="' + field.format + '"' : '');
        html += (angular.isDefined(field.startDate) ? ' data-date-start-date="' + field.startDate + '"' : '');
        html += (angular.isDefined(field.endDate) ? ' data-date-end-date="' + field.endDate + '"' : '');
        html += ' bs-datepicker="" >';
        html += ' <span class="input-group-btn"  data-toggle="datepicker"> <button type="button" class="btn btn-default"';
        html += (attr.ngDisabled ? ' ng-disabled="' + attr.ngDisabled + '"' : ' ng-disabled="setReadonly()"');
        html += '><i class="glyphicon glyphicon-calendar"></i></button> ';
        html += ' </span></div>';
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
                    field.minViewMode = "months";
                    break;
                case "YearPicker":
                    field.minViewMode = "years";
                    break;
                case "future":
                    field.startDate = "+0";
                    break;
                case "past":
                    field.endDate = "+0";
                    break;
                case "Daily":
                    field.startDate = "-15d";
                    field.endDate = "+5d";
                    break;
                case "Weekly":
                    field.startDate = "-30d";
                    field.endDate = "+15d";
                    break;
                case "Monthly":
                    field.minViewMode = "months";
                    field.startDate = "-80d";
                    field.endDate = "+30d";
                    break;
                default:
                    $log.error(template + " is not defined.");
            }
            return;
        });
    };

    var manageViewMode = function (field) {
        //month/year modes
        if ($csfactory.isNullOrEmptyString(field.minViewMode)) {
            field.minViewMode = 0;
        } else if (field.minViewMode === "1" || field.minViewMode === "months") {
            field.minViewMode = 1;
        } else if (field.minViewMode === "2" || field.minViewMode === "years") {
            field.minViewMode = 2;
        } else {
            field.minViewMode = 0;
        }

        //format
        if (field.minViewMode === 0) {
            field.format = "dd-M-yyyy";
        } else if (field.minViewMode === 1) {
            field.format = "M-yyyy";
        } else {
            field.format = ".yyyy";
        }

        //min date        
        if ($csfactory.isNullOrEmptyString(field.startDate)) {
            if (field.minViewMode === 0) {
                field.startDate = '01-Jan-1800';
            } else if (field.minViewMode === 1) {
                field.startDate = 'Jan-1800';
            } else {
                field.startDate = '.1800';
            }
        }

        //max date
        if ($csfactory.isNullOrEmptyString(field.endDate)) {
            if (field.minViewMode === 0) {
                field.endDate = '31-Dec-2400';
            } else if (field.minViewMode === 1) {
                field.endDate = 'Dec-2400';
            } else {
                field.endDate = '.2400';
            }

        }
    };

    var validateOptions = function (field) {
        applyTemplate(field);
        manageViewMode(field);

        if ($csfactory.isNullOrEmptyString(field.label)) {
            field.label = "Date";
        }

        if ($csfactory.isNullOrEmptyString(field.daysOfWeekDisabled)) {
            field.daysOfWeekDisabled = '[]';
        }
    };

    return {
        htmlTemplate: htmlTemplate,
        checkOptions: validateOptions
    };
}]);

csapp.directive('csFieldGroup', [function () {
    return {
        template: '<div><div ng-transclude=""/></div>',
        scope: { mode: '=', model: '@' },
        restrict: 'E',
        transclude: true,
        require: '^form',
        controller: function ($scope) { this.mode = $scope.mode; }
    };
}]);

csapp.directive('csField', ["$compile", "$parse", "csNumberFieldFactory", "csTextFieldFactory", "csTextareaFactory", "csEmailFactory", "csCheckboxFactory", "csRadioButtonFactory", "csSelectField", "csEnumFactory", "csDateFactory", "csBooleanFieldFactory",
    function ($compile, $parse, numberFactory, textFactory, textareaFactory, emailFactory, checkboxFactory, radioFactory, selectFactory, enumFactory, dateFactory, boolFactory) {

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
                case 'btn-radio':
                    return boolFactory;
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

        var setSpan = function (field, ctrl) {
            field.labelSize = angular.isDefined(ctrl[3]) ? parseInt(ctrl[3].labelSize) : undefined;
            field.controlSize = angular.isDefined(ctrl[3]) ? parseInt(ctrl[3].controlSize) : undefined;
            var lSize = angular.isDefined(field.labelSize) ? field.labelSize : 0;
            var cSize = angular.isDefined(field.controlSize) ? field.controlSize : 0;
            field.span = lSize + cSize;
        };

        var linkFunction = function (scope, element, attrs, ctrl) {

            var fieldGetter = $parse(attrs.field);
            var field = fieldGetter(scope);
            scope.field = field;

            scope.mode = angular.isDefined(ctrl[2]) ? ctrl[2].mode : '';
            setSpan(field, ctrl);

            var typedFactory = getFactory(field.type);
            typedFactory.checkOptions(field, attrs);

            var html = typedFactory.htmlTemplate(field, attrs);

            var newElem = angular.element(html);
            element.replaceWith(newElem);
            $compile(newElem)(scope);
        };

        return {
            restrict: 'E',
            link: linkFunction,
            scope: true,
            require: ['ngModel', '^form', '?^csFieldGroup', '?^csForm'],
            terminal: true,
            controller: controllerFn
        };
    }]);


csapp.directive('csForm', ["$compile", function ($compile) {


    var cntrlFn = function ($scope) {

        //$scope.layout = $scope.layout.split(".");

        this.rowWidth = parseInt($scope.layout[0]);
        this.labelSize = parseInt($scope.layout[1]);
        this.controlSize = parseInt($scope.layout[2]);
    };

    return {
        restrict: 'E',
        scope: {
            layout: '=',
        },
        controller: cntrlFn
    };
}]);