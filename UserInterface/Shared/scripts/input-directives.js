
csapp.factory("csBootstrapInputTemplate", function () {

    var bsTemplateBefore = function (field, noBootstrap, fieldname) {
        var html = '<div class="control-group">' +
            '<div class="control-label">{{' + fieldname + '.label}}' +
            '<span class="text-error"> {{' + fieldname + '.required ? " *":""}}</span></div>' +
            '<div class="controls">';
        return (noBootstrap ? "<div>" : html);
    };

    var bsTemplateAfter = function (noBootstrap) {
        var html = '</div>' + //controls
            '</div>'; // control-group
        return (noBootstrap ? "</div>" : html);
    };

    return {
        before: bsTemplateBefore,
        after: bsTemplateAfter
    };
});

csapp.factory("csValidationInputTemplate", function () {

    var before = function () {
        var html = '<div ng-form="myform">';
        return html;
    };

    var getmessages = function (fieldname, field) {
        field.messages = {
            required: '{{' + fieldname + '.label}} is required.',
            pattern: '{{' + fieldname + '.patternMessage}}',
            minlength: '{{' + fieldname + '.label}} should have atleast {{' + fieldname + '.minlength}} character/s.',
            maxlength: '{{' + fieldname + '.label}} can have maximum {{' + fieldname + '.maxlength}} character/s.',
            min: '{{' + fieldname + '.label}} cannot be less than {{' + fieldname + '.min}}.',
            max: '{{' + fieldname + '.label}} cannot be greater than {{' + fieldname + '.max}}.'
        };
    };

    var after = function (fieldname, field) {
        getmessages(fieldname, field);
        var html = '<div data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty"> ' +
            '<div class="field-validation-error" data-ng-show="myform.myfield.$error.required">' + field.messages.required + '</div>' +
            '<div class="field-validation-error" data-ng-show="myform.myfield.$error.pattern">' + field.messages.pattern + '</div>' +
            '<div class="field-validation-error" data-ng-show="myform.myfield.$error.minlength">' + field.messages.minlength + '</div>' +
            '<div class="field-validation-error" data-ng-show="myform.myfield.$error.maxlength">' + field.messages.maxLength + '</div>' +
            '<div class="field-validation-error" data-ng-show="myform.myfield.$error.min">' + field.messages.min + '</div>' +
            '<div class="field-validation-error" data-ng-show="myform.myfield.$error.max">' + field.messages.max + '</div>' +
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

csapp.factory("csNumberFieldFactory", ["Logger", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function (logManager, bstemplate, valtemplate) {

        var $log = logManager.getInstance("csNumberFieldFactory");

        //#region template
        var input = function (field, attrs) {
            var html = '<input class="form-control" name="myfield"';
            html += field.placeholder ? 'placeholder="' + field.placeholder + '"' : ' ';
            html += 'ng-model="' + attrs.ngModel + '" type="number"';
            html += 'ng-readonly="setReadonly()"';
            html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
            html += (attrs.ngShow ? ' ng-show="' + attrs.ngShow + '"' : '');
            html += (attrs.ngHide ? ' ng-hide="' + attrs.ngHide + '"' : '');
            html += ' ng-required="' + attrs.field + '.required"';
            html += (angular.isDefined(field.minlength) ? ' ng-minlength="' + field.minlength + '"' : '');
            html += (angular.isDefined(field.maxLength) ? ' ng-maxlength="' + field.maxLength + '"' : '');
            html += (angular.isDefined(field.min) ? ' min="' + field.min + '"' : '');
            html += (angular.isDefined(field.max) ? ' max="' + field.max + '"' : '');
            html += (field.pattern ? ' ng-pattern="' + field.pattern + '"' : '');
            html += '/>';
            return html;
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs.field),
                valtemplate.before(),
                input(field, attrs),
                valtemplate.after(attrs.field, field),
                bstemplate.after(noBootstrap)
            ].join(' ');
            return template;
        };
        //#endregion

        //#region validations
        var applyTemplates = function (options) {
            switch (options.type) {
                case "uint":
                    if (angular.isUndefined(options.min))
                        options.min = 0;
                    break;
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
                default:
                    $log.error(options.type + " is not defined");
            }
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
            options.patternMessage = options.patternMessage || "Value cannot have non-numeric character/s.";
        };
        //#endregion

        return {
            htmlTemplate: htmlTemplate,
            checkOptions: validateOptions
        };
    }]);

//{ label: 'Name', template: 'phone', editable: false, required: true, type: 'text', min: 10, max: 100 },
csapp.factory("csTextFieldFactory", ["Logger", "csBootstrapInputTemplate", "csValidationInputTemplate", function (logManager, bstemplate, valtemplate) {

    var $log = logManager.getInstance("csTextFieldFactory");

    var prefix = function (fields) {
        var html = ' ';
        switch (fields.template) {
            case 'user':
                html += '<div class="input-prepend"><span class="add-on"><i class="icon-user"></i></span>';
                break;
            case 'phone':
                html += '<div class="input-prepend"><span class=" add-on"><i class="icon-phone"></i></span><span class="add-on">+91</span>';
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
        }
        return html;
    };

    //#region template
    var input = function (field, attrs) {
        var html = '<input class="input-large" name="myfield"';
        html += field.placeholder ? 'placeholder="' + field.placeholder + '"' : ' ';
        html += 'ng-model="' + attrs.ngModel + '" type="text"';
        //html += (field.mask ? 'ui-mask="' + field.mask + '"' : '');
        html += angular.isDefined(attrs.typeahead) ? 'typeahead="' + attrs.typeahead + '"' : ' ';
        html += angular.isDefined(attrs.typeaheadMinLength) ? 'typeahead-min-length="' + attrs.typeaheadMinLength + '"' : ' ';
        html += angular.isDefined(attrs.typeaheadWaitMs) ? 'typeahead-wait-ms="' + attrs.typeaheadWaitMs + '"' : ' ';
        html += 'ng-readonly="setReadonly()"';
        html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
        html += (attrs.ngShow ? ' ng-show="' + attrs.ngShow + '"' : '');
        html += (attrs.ngHide ? ' ng-hide="' + attrs.ngHide + '"' : '');
        html += ' ng-required="' + attrs.field + '.required"';
        html += (angular.isDefined(field.minlength) && angular.isUndefined(attrs.typeahead) ? ' ng-minlength="' + field.minlength + '"' : '');
        html += (angular.isDefined(field.maxLength) && angular.isUndefined(attrs.typeahead) ? ' ng-maxlength="' + field.maxLength + '"' : '');
        html += (angular.isDefined(field.min) && angular.isUndefined(attrs.typeahead) ? ' min="' + field.min + '"' : '');
        html += (angular.isDefined(field.max) && angular.isUndefined(attrs.typeahead) ? ' max="' + field.max + '"' : '');
        html += (field.pattern ? ' ng-pattern="' + field.pattern + '"' : '');
        html += '/>';
        return html;
    };

    var htmlTemplate = function (field, attrs) {
        var noBootstrap = angular.isDefined(attrs.noLabel);
        var template = [
            bstemplate.before(field, noBootstrap, attrs.field),
            valtemplate.before(),
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
                default:
                    $log.error(template + " is not defined");
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

        var $log = logManager.getInstance("csTextareaFactory");

        //#region template
        var input = function (field, attrs) {
            var html = '<textarea  name="myfield"';
            html += field.placeholder ? 'placeholder="' + field.placeholder + '"' : ' ';
            html += 'ng-readonly="setReadonly()"';
            html += angular.isDefined(field.resize) ? (field.resize ? 'class="form-control"' : 'class="form-control noResize"') : 'class="form-control"';
            html += 'ng-model="' + attrs.ngModel + '"';
            html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
            html += (attrs.ngShow ? ' ng-show="' + attrs.ngShow + '"' : '');
            html += (attrs.ngHide ? ' ng-hide="' + attrs.ngHide + '"' : '');
            html += ' ng-required="' + attrs.field + '.required"';
            html += (angular.isDefined(field.minlength) ? ' ng-minlength="' + field.minlength + '"' : '');
            html += (angular.isDefined(field.maxLength) ? ' ng-maxlength="' + field.maxLength + '"' : '');
            html += (angular.isDefined(field.min) ? ' min="' + field.min + '"' : '');
            html += (angular.isDefined(field.max) ? ' max="' + field.max + '"' : '');
            html += (field.pattern ? ' ng-pattern="' + field.pattern + '"' : '');
            html += '></textarea>';
            return html;
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs.field),
                valtemplate.before(),
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

        var $log = logManager.getInstance("csCheckboxFactory");

        //#region template
        var input = function (field, attrs) {
            var html = '<input  name="myfield"';
            html += 'ng-model="' + attrs.ngModel + '" type="checkbox"';
            html += 'ng-readonly="setReadonly()"';
            html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
            html += (attrs.ngClick ? ' ng-click="' + attrs.ngClick + '"' : '');
            html += (attrs.ngShow ? ' ng-show="' + attrs.ngShow + '"' : '');
            html += (attrs.ngHide ? ' ng-hide="' + attrs.ngHide + '"' : '');
            html += ' ng-required="' + attrs.field + '.required"';
            html += '/>';
            return html;
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs.field),
                valtemplate.before(),
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

        var $log = logManager.getInstance("csEmailFactory");

        var prefix = function (field) {

            var hasSuffix = angular.isDefined(field.suffix) && field.suffix !== null && field.suffix.length > 0;

            var html = '<div class="input-prepend';
            html += hasSuffix ? ' input-append">' : '">';
            html += '<span class="add-on"><i class="icon-envelope"></i></span>';
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
                string += '<span class="add-on">' + fields.suffix + '</span>';
            }
            return string;
        };

        var suffix = function () {
            return '</div>';
        };

        //#region template
        var input = function (field, attrs) {
            var html = '<input  name="myfield"';
            html += field.placeholder ? 'placeholder="' + field.placeholder + '"' : ' ';
            html += 'ng-model="' + attrs.ngModel + '" type="email"';
            html += 'ng-readonly="setReadonly()"';
            html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
            html += (attrs.ngShow ? ' ng-show="' + attrs.ngShow + '"' : '');
            html += (attrs.ngHide ? ' ng-hide="' + attrs.ngHide + '"' : '');
            html += ' ng-required="' + attrs.field + '.required"';
            html += (angular.isDefined(field.minlength) ? ' ng-minlength="' + field.minlength + '"' : '');
            html += (angular.isDefined(field.maxLength) ? ' ng-maxlength="' + field.maxLength + '"' : '');
            html += (angular.isDefined(field.min) ? ' min="' + field.min + '"' : '');
            html += (angular.isDefined(field.max) ? ' max="' + field.max + '"' : '');
            html += (field.pattern ? ' ng-pattern="' + field.pattern + '"' : '');
            html += addEmailSuffix(field);
            html += '/>';
            return html;
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs.field),
                valtemplate.before(),
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
            options.maxlength = options.length || options.maxlength || 18;
            options.minlength = (options.minlength >= 0 && options.minlength <= 18) ? options.minlength : 0;
            options.maxlength = (options.maxlength >= 0 && options.maxlength <= 18) ? options.maxlength : 18;
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

csapp.factory("csRadioButtonFactory", ["Logger", "csBootstrapInputTemplate", "csValidationInputTemplate",
    function (logManager, bstemplate, valtemplate) {

        var input = function (field, attrs) {

            var html = '<div class="row-fluid">';
            html += '<div class="span1 radio" ng-repeat="(key, record) in ' + field.options + '">';
            html += 'ng-readonly="setReadonly()"';
            html += '<label><input  name="myfield"';
            html += 'ng-model="' + attrs.ngModel + '" type="radio"';
            html += 'ng-value="{{' + field.valueField + '}}"';
            html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
            html += (attrs.ngClick ? ' ng-click="' + attrs.ngClick + '"' : '');
            html += (attrs.ngShow ? ' ng-show="' + attrs.ngShow + '"' : '');
            html += (attrs.ngHide ? ' ng-hide="' + attrs.ngHide + '"' : '');
            html += ' ng-required="' + attrs.field + '.required"';
            html += '/>{{' + field.textField + '}} </label>';
            html += '</div></div>';
            return html;

        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs.field),
                valtemplate.before(),
                input(field, attrs),
                valtemplate.after(attrs.field, field),
                bstemplate.after(noBootstrap)
            ].join(' ');
            return template;
        };



        var validateOptions = function (field) {
            field.label = field.label || "Description";
            //field.textField = "record." + (field.textField || "text");
            field.textField = angular.isDefined(field.textField) ? "record." + (field.textField) : "record";
            field.valueField = angular.isDefined(field.valueField) ? "record." + (field.valueField) : "record";
            //field.valueField = "record." + (field.valueField || );
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
            var html = '<select  class="input-large" ';
            html += 'data-ng-model="' + attr.ngModel + '"name="myfield"';
            html += ' ng-required="' + attr.field + '.required"';
            html += 'ng-options="' + field.ngOptions + '"';
            html += (attr.ngChange ? ' ng-change="' + attr.ngChange + '"' : '');
            html += (attr.ngShow ? ' ng-show="' + attr.ngShow + '"' : '');
            html += (attr.ngHide ? ' ng-hide="' + attr.ngHide + '"' : '');
            html += 'ng-disabled="setReadonly()">';

            return html;
        };

        var validateOptions = function (field) {

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

            setNgOptions(field);
        };

        var setNgOptions = function (field) {
            field.ngOptions = field.valueField + ' as ' + field.textField + ' for row in field.valueList';
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs.field),
                valtemplate.before(),
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
            var html = '<select class="input-large" ng-options="row for row in field.valueList"';
            //html += field.placeholder ? 'placeholder="' + field.placeholder + '"' : ' ';
            html += 'data-ng-model="$parent.' + attr.ngModel + '"name="myfield"';
            html += (attr.ngChange ? ' ng-change="' + attr.ngChange + '"' : '');
            html += (attr.ngShow ? ' ng-show="' + attr.ngShow + '"' : '');
            html += (attr.ngHide ? ' ng-hide="' + attr.ngHide + '"' : '');
            html += ' ng-required="' + attr.field + '.required"';
            html += 'ng-disabled="setReadonly()">';
            html += ' <option value="" selectable="false"></option> ';
            //html += ' <option data-ng-repeat="row in field.valueList" value="row">{{row}}</option;';
            html += '</select> ';

            return html;
        };

        var validateOptions = function (field) {
            field.label = field.label || "SelectBox";
            field.csRepeat = "row in " + field.csRepeat;//.substring(1, scope.csRepeat.length - 1);
        };

        var htmlTemplate = function (field, attrs) {
            var noBootstrap = angular.isDefined(attrs.noLabel);
            var template = [
                bstemplate.before(field, noBootstrap, attrs.field),
                valtemplate.before(),
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
        var html = '<div class="input-append">';
        html += '<input type="text" name="myfield" class="input-medium" data-ng-readonly="true"';
        html += field.placeholder ? 'placeholder="' + field.placeholder + '"' : ' ';
        html += 'ng-readonly="setReadonly()"';
        html += ' data-ng-model="' + attr.ngModel + '"';
        html += (angular.isDefined(attr.ngChange) ? 'data-ng-change="' + attr.ngChange + '"' : '');
        html += (attr.ngShow ? ' ng-change="' + attr.ngShow + '"' : '');
        html += (attr.ngHide ? ' ng-change="' + attr.ngHide + '"' : '');
        html += ' ng-required="' + attr.field + '.required"';
        html += ' data-date-min-view-mode="' + (angular.isDefined(field.minViewMode) ? field.minViewMode : '') + '" ' +
      ' data-date-days-of-week-disabled="' + (angular.isDefined(field.daysOfWeekDisabled) ? field.daysOfWeekDisabled : '') + '" data-date-format="' + field.format + '" ' +
      ' data-date-start-date="' + (angular.isDefined(field.startDate) ? field.startDate : '') + '"' +
      ' data-date-end-date="' + (angular.isDefined(field.endDate) ? field.endDate : '') + '" bs-datepicker="" >' +
  '<button type="button" class="btn" data-toggle="datepicker"><i class="icon-calendar"></i></button> ' +
'</div>';

        return html;
    };


    var htmlTemplate = function (field, attrs) {
        var noBootstrap = angular.isDefined(attrs.noLabel);
        var template = [
            bstemplate.before(field, noBootstrap, attrs.field),
            valtemplate.before(),
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

csapp.directive('csFieldGroup', ["$parse", function ($parse) {
    return {
        template: '<div><div ng-transclude=""/></div>',
        scope: { mode: '=', model: '@' },
        restrict: 'E',
        transclude: true,
        require: '^form',
        controller: function ($scope) { this.mode = $scope.mode; }
    };
}]);

csapp.directive('csField', ["$compile", "$parse", "csNumberFieldFactory", "csTextFieldFactory", "csTextareaFactory", "csEmailFactory", "csCheckboxFactory", "csRadioButtonFactory", "csSelectField", "csEnumFactory", "csDateFactory",
    function ($compile, $parse, numberFactory, textFactory, textareaFactory, emailFactory, checkboxFactory, radioFactory, selectFactory, enumFactory, dateFactory) {

        var getFactory = function (type) {
            switch (type) {
                case "textarea":
                    return textareaFactory;
                case "uint":
                case "int":
                case "ulong":
                case "long":
                case "decimal":
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

        var linkFunction = function (scope, element, attrs, ctrl) {
            var fieldGetter = $parse(attrs.field);
            var field = fieldGetter(scope);
            scope.field = field;
            scope.mode = angular.isDefined(ctrl[2]) ? ctrl[2].mode : '';

            var typedFactory = getFactory(field.type);
            typedFactory.checkOptions(field);

            var html = typedFactory.htmlTemplate(field, attrs);

            var newElem = angular.element(html);
            element.replaceWith(newElem);
            $compile(newElem)(scope);
        };

        return {
            restrict: 'E',
            link: linkFunction,
            scope: true,
            require: ['ngModel', '^form', '?^csFieldGroup'],
            terminal: true,
            controller: controllerFn
        };
    }]);

