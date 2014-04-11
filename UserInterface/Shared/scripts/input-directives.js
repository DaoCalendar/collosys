
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
            html += (attrs.ngShow ? ' ng-change="' + attrs.ngShow + '"' : '');
            html += (attrs.ngHide ? ' ng-change="' + attrs.ngHide + '"' : '');
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
        html += (attrs.ngShow ? ' ng-change="' + attrs.ngShow + '"' : '');
        html += (attrs.ngHide ? ' ng-change="' + attrs.ngHide + '"' : '');
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
            html += 'placeholder="' + field.placeholder + '"';
            html += 'ng-readonly="setReadonly()"';
            html += angular.isDefined(field.resize) ? (field.resize ? 'class="form-control"' : 'class="form-control noResize"') : 'class="form-control"';
            html += 'ng-model="' + attrs.ngModel + '"';
            html += (attrs.ngChange ? ' ng-change="' + attrs.ngChange + '"' : '');
            html += (attrs.ngShow ? ' ng-change="' + attrs.ngShow + '"' : '');
            html += (attrs.ngHide ? ' ng-change="' + attrs.ngHide + '"' : '');
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
            html += (attrs.ngShow ? ' ng-change="' + attrs.ngShow + '"' : '');
            html += (attrs.ngHide ? ' ng-change="' + attrs.ngHide + '"' : '');
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
            html += (attrs.ngShow ? ' ng-change="' + attrs.ngShow + '"' : '');
            html += (attrs.ngHide ? ' ng-change="' + attrs.ngHide + '"' : '');
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
            html += (attrs.ngShow ? ' ng-change="' + attrs.ngShow + '"' : '');
            html += (attrs.ngHide ? ' ng-change="' + attrs.ngHide + '"' : '');
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
            var html = '<select data-ui-select2="" class="input-large" ';
            html += 'data-ng-model="' + attr.ngModel + '"name="myfield"';
            html += ' ng-required="' + attr.field + '.required"';
            html += (attr.ngChange ? ' ng-change="' + attr.ngChange + '"' : '');
            html += (attr.ngShow ? ' ng-change="' + attr.ngShow + '"' : '');
            html += (attr.ngHide ? ' ng-change="' + attr.ngHide + '"' : '');
            html += 'ng-disabled="setReadonly()">';
            html += ' <option value=""></option> ' +
                       ' <option data-ng-repeat="row in field.valueList" value="{{' + field.valueField + '}}">{{' + field.textField + '}}</option>' +
                   '</select> ';

            return html;
        };

        var validateOptions = function (field) {
            field.label = field.label || "SelectBox";
            if (field.textField != 'row' && field.valueField != 'row') {
                field.textField = field.textField ? "row." + field.textField : "row";
                field.valueField = field.valueField ? "row." + field.valueField : "row";
            }

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
            var html = '<select data-ui-select2="" class="input-large"';
            //html += field.placeholder ? 'placeholder="' + field.placeholder + '"' : ' ';
            html += 'data-ng-model="' + attr.ngModel + '"name="myfield"';
            html += (attr.ngChange ? ' ng-change="' + attr.ngChange + '"' : '');
            html += (attr.ngShow ? ' ng-change="' + attr.ngShow + '"' : '');
            html += (attr.ngHide ? ' ng-change="' + attr.ngHide + '"' : '');
            html += ' ng-required="' + attr.field + '.required"';
            html += 'ng-disabled="setReadonly()">';
            html += ' <option value=""></option> ' +
                       ' <option data-ng-repeat="row in field.valueList" value="{{row}}">{{row}}</option>' +
                   '</select> ';

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

csapp.directive('fieldGroup', ["$parse", function ($parse) {
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

        var linkFunction = function (scope, element, attrs) {
            var fieldGetter = $parse(attrs.field);
            var field = fieldGetter(scope);
            scope.field = field;

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
            require: ['ngModel', '^form', '?fieldGroup'],
            terminal: true,
            controller: controllerFn
        };
    }]);



//csapp.directive("csNumberField", ["$compile", "csNumberFieldFactory", function ($compile, factory) {

//    var linkFunction = function (scope, element) {
//        factory.checkOptions(scope);
//        var template = factory.htmlTemplate(scope);
//        element.html(template);
//        $compile(element.contents())(scope);
//    };

//    return {
//        scope: { options: '=', ngModel: '@', ngChange: '&' },
//        restrict: 'E',
//        link: linkFunction,
//        require: 'ngModel'
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

//csapp.directive("csInput", ["csTemplateFactory", "csNumberFieldFactory", "csTextFieldFactory", "csEmailFieldFactory", "csTextareaFieldFactory", "csCheckBoxFieldFactory",
//    "csEnumFieldFactory", "csSelectFieldFactory", "csRadioFieldFactory", "csDateFieldFactory", "$compile", "Logger",
//    function (templatefactory, numberFactory, textfactory, emailfactory, textareafactory, checkboxfactory, enumfactory, selectfactory, radiofactory, datefactory, $compile, logManager) {

//        var $log = logManager.getInstance("csInput");

//        var getFactory = function (type) {
//            switch (type) {
//                case "text":
//                    return textfactory;
//                case "int":
//                case "uint":
//                case "long":
//                case "ulong":
//                case "phone":
//                case "decimal":
//                case "userId":
//                    return numberFactory;
//                case "email":
//                    return emailfactory;
//                case "textarea":
//                    return textareafactory;
//                case "checkbox":
//                    return checkboxfactory;
//                case "enum":
//                    return enumfactory;
//                case "select":
//                    return selectfactory;
//                case "radio":
//                    return radiofactory;
//                case "date":
//                    return datefactory;
//                default:
//                    $log.error("Invalid type : " + type);
//                    return null;
//            }
//        };

//        var checkAttrs = function (scope) {
//            if (angular.isDefined(scope.csTypeahead)) {
//                scope.csTypeahead = scope.csTypeahead.substring(1, scope.csTypeahead.length - 1);
//                console.log(scope.csTypeahead);
//            }
//        };

//        var factory;
//        var render = function (scope, element) {
//            checkAttrs(scope);
//            factory = getFactory(scope.options.type);
//            factory.checkOptions(scope);
//            var innertemplate = factory.htmlTemplate(scope);
//            var template = templatefactory.getTemplate(innertemplate);
//            console.log(template);
//            element.html(template);
//            $compile(element.contents())(scope);
//        };

//        var linkFunction = function (scope, element) {
//            scope.array = ['a1', 'a2', 'a3'];
//            render(scope, element);
//        };

//        return {
//            scope: { options: '=', ngModel: '@', ngChange: '&', ngClick: '&', csRepeat: '@', textField: '@', valueField: '@', csTypeahead: '@' },
//            restrict: 'E',
//            link: linkFunction,
//            require: ['ngModel', '^form']
//        };
//    }]);

//csapp.factory("csTemplateFactory", function () {
//    var template = function (innertemplate) {

//        var string = '<div class="control-group" >' +
//                        '<div class="control-label">{{options.label}} ' +
//                        '<span class="text-error">{{options.required ? "*" : ""}} </span></div>' +
//                    '<div class="controls">';
//        string += innertemplate;
//        string += '</div>' + //controls
//            '</div>';
//        return string;
//    };

//    var validation = function (inner) {

//        var string = '<ng-form name="myform">';

//        string += inner;

//        string += '<div class="field-validation-error" data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty">' +
//            '<div data-ng-show="myform.myfield.$error.required ">{{options.label}} is required</div>' +
//            '<div data-ng-show="myform.myfield.$error.pattern">{{options.patternMessage}}</div>' +
//            '<div data-ng-show="myform.myfield.$error.minlength">{{options.label}} should have atleast {{options.minlength}} character/s.</div>' +
//            '<div data-ng-show="myform.myfield.$error.maxlength">{{options.label}} can have maximum {{options.maxlength}} character/s.</div>' +
//            '<div data-ng-show="myform.myfield.$error.min">{{options.label}} cannot have value less than {{options.min}}</div>' +
//            '<div data-ng-show="myform.myfield.$error.max">{{options.label}} cannot have value greater than {{options.max}}</div>' +
//            '</div>';

//        string += '</ng-form>';

//        return template(string);
//    };


//    var prefix = function () {
//        return '<ng-form name="myform">';

//    };

//    var suffixTemplate = function () {


//        return '<div class="field-validation-error" data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty">' +
//            '<div data-ng-hide="myform.myfield.$error.required ">{{options.label}} is required</div>' +
//            '<div data-ng-show="myform.myfield.$error.pattern">{{options.patternMessage}}</div>' +
//            '<div data-ng-show="myform.myfield.$error.minlength">{{options.label}} should have atleast {{options.minlength}} character/s.</div>' +
//            '<div data-ng-show="myform.myfield.$error.maxlength">{{options.label}} can have maximum {{options.maxlength}} character/s.</div>' +
//            '<div data-ng-show="myform.myfield.$error.min">{{options.label}} cannot have value less than {{options.min}}</div>' +
//            '<div data-ng-show="myform.myfield.$error.max">{{options.label}} cannot have value greater than {{options.max}}</div>' +
//            '</div>';
//    };
//    return {
//        getTemplate: template,
//        getValTemplate: validation,
//        prefix: prefix,
//        suffixTemplate: suffixTemplate
//    };
//});
//csapp.directive("csTemplate", ["$compile", "csTemplateFactory", function ($compile, csInputTemplate) {

//    var getTemplate = function () {
//        var inner = '<div ng-transclude></div>';
//        return csInputTemplate.getValTemplate(inner);
//    };

//    function linkFunction(scope, element) {
//        $compile(element[0].form)(scope);
//    }

//    return {
//        scope: { options: '=' },
//        restrict: 'E',
//        transclude: true,
//        template: getTemplate,
//        link: linkFunction
//    };
//}]);


//csapp.directive("csTextField", ["$compile", "csTextFieldFactory", function ($compile, csTextFieldFactory) {

//    //options: label, autofocus,  placeholder, required, readonly, minlength, maxlength
//    var linkFunction = function (scope, element) {
//        csTextFieldFactory.checkOptions(scope);
//        var template = csTextFieldFactory.htmlTemplate(scope);
//        element.html(template);
//        $compile(element.contents())(scope);
//    };

//    return {
//        scope: { options: '=', ngModel: '@', ngChange: '&' },
//        restrict: 'E',
//        link: linkFunction,
//        require: 'ngModel'
//    };
//}]);

//csapp.directive("csInputSuffix", function () {

//    var linkFunction = function (scope, element, attrs, ctr) {
//        ctr.$parsers.unshift(function (value) {

//            var isValid = value !== "";

//            ctr.$setValidity("required", isValid);
//            if (!isValid) {
//                return undefined;
//            }

//            if (value.indexOf(attrs.suffix) < 0) {
//                value = value + attrs.suffix;
//            }

//            return value;
//        });

//        ctr.$formatters.unshift(function (value) {
//            value = value || "";
//            return value.replace(attrs.suffix, "");
//        });
//    };

//    return {
//        require: 'ngModel',
//        link: linkFunction
//    };
//});

//csapp.factory("csEmailFieldFactory", ["Logger", function (logManager) {

//    var formTemplate = function (scope) {
//        var html = '<div ng-form="myform">';

//        html += templateFunction(scope);

//        html += '<div class="field-validation-error" data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty"> ' +
//                    '<div data-ng-show="myform.myfield.$error.required ">{{options.label}} is required</div>' +
//                    '<div data-ng-show="myform.myfield.$error.minlength">{{options.label}} should have atleast {{options.minlength}} character/s.</div>' +
//                    '<div data-ng-show="myform.myfield.$error.maxlength">{{options.label}} can have maximum {{options.maxlength}} character/s.</div>' +
//                '</div>';

//        html += '</div>'; //ng-form;

//        return html;
//    };

//    var templateFunction = function (scope) {
//        var hasSuffix = angular.isDefined(scope.options.suffix) && scope.options.suffix !== null && scope.options.suffix.length > 0;

//        var string = '<div class="input-prepend input-append">' +
//            '<span class="add-on"><i class="icon-envelope"></i></span>' +
//            '<input type="email" name="myfield" placeholder="{{options.placeholder}}" ng-required="options.required" ' +
//            'ng-pattern="{{options.pattern}}" ng-minlength="{{options.minlength}}" data-ng-change="ngChange()" ' +
//            'ng-maxlength="{{options.maxlength}}" data-ng-model="$parent.$parent.' + scope.ngModel + '" ';
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

//    var validateOptions = function (scope) {
//        scope.options.label = scope.options.label || "Email";
//        scope.options.placeholder = scope.options.placeholder || "Enter Email";
//        scope.options.minlength = scope.options.suffix ? scope.options.suffix.length + 4 : 8;
//        scope.options.maxlength = 250;
//        scope.options.patternMessage = "Input is not a valid email address.";
//    };


//    return {
//        htmlTemplate: formTemplate,
//        checkOptions: validateOptions
//    };
//}]);
//csapp.directive("csEmailField", ["$compile", "csEmailFieldFactory", function ($compile, factory) {

//    var linkFunction = function (scope, element) {
//        factory.checkOptions(scope.options);
//        var template = factory.htmlTemplate(scope);
//        element.html(template);
//        $compile(element.contents())(scope);
//    };

//    return {
//        scope: { options: '=', ngModel: '@', ngChange: '&' },
//        restrict: 'E',
//        link: linkFunction
//    };
//}]);

//csapp.factory("csTextareaFieldFactory", ["Logger", function (logManager) {

//    var $log = logManager.getInstance("csTextField");

//    var formTemplate = function (scope) {
//        var html = '<div ng-form="myform">';

//        html += templateFunction(scope);

//        html += '<div class="field-validation-error" data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty"> ' +
//                    '<div data-ng-show="myform.myfield.$error.required ">{{options.label}} is required</div>' +
//                    '<div data-ng-show="myform.myfield.$error.minlength">{{options.label}} should have atleast {{options.minlength}} character/s.</div>' +
//                    '<div data-ng-show="myform.myfield.$error.maxlength">{{options.label}} can have maximum {{options.maxlength}} character/s.</div>' +
//                '</div>';

//        html += '</div>'; //ng-form;

//        return html;
//    };

//    var templateFunction = function (scope) {
//        var string = '<textarea type="text" name="myfield" placeholder="{{options.placeholder}}" ng-required="options.required" ' +
//                    'ng-readonly="options.readonly" ng-minlength="{{options.minlength}}" ng-maxlength="{{options.maxlength}}" ' +
//                    'rows="{{options.rows}}" cols="{{options.columns}}" autofocus="options.autofocus" ' +
//                    'data-ng-change="ngChange()"' +
//                    ' data-ng-model="$parent.$parent.' + scope.ngModel + '"/>';
//        return string;
//    };

//    var validateOptions = function (scope) {
//        scope.options.label = scope.options.label || "Description";
//        scope.options.rows = scope.options.rows || 2;
//        scope.options.columns = scope.options.columns || 120;
//        scope.options.placeholder = scope.options.placeholder || "Describe in detail";
//        scope.options.maxlength = scope.options.maxlength || 250;
//        scope.options.readonly = scope.options.readonly || scope.options.disabled;
//    };

//    return {
//        htmlTemplate: formTemplate,
//        checkOptions: validateOptions
//    };
//}]);
//csapp.directive("csTextareaField", ["$compile", "csTextareaFieldFactory", function ($compile, factory) {

//    var linkFunction = function (scope, element) {
//        factory.checkOptions(scope.options);
//        var template = factory.htmlTemplate(scope);
//        element.html(template);
//        $compile(element.contents())(scope);
//    };

//    return {
//        scope: { options: '=', ngModel: '@', ngChange: '&' },
//        restrict: 'E',
//        link: linkFunction
//    };
//}]);

//csapp.factory("csCheckBoxFieldFactory", ["Logger", function (logManager) {

//    var $log = logManager.getInstance("csCheckboxField");

//    var formTemplate = function (scope) {
//        var html = '<div ng-form="myform">';

//        html += templateFunction(scope);

//        html += '<div class="field-validation-error" data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty"> ' +
//                    '<div data-ng-show="myform.myfield.$error.required ">{{options.label}} is required</div>' +
//                '</div>';

//        html += '</div>'; //ng-form;

//        return html;
//    };

//    var templateFunction = function (scope) {

//        var string = '<input type="checkbox" name="myfield"  ng-required="options.required" ' +
//           ' data-ng-model="$parent.$parent.' + scope.ngModel + '" data-ng-click="ngClick()" ' +
//            'data-ng-change="ngChange()"/>';

//        return string;

//    };

//    var validateOptions = function (scope) {
//        scope.options.label = scope.options.label || "CheckBox";
//    };

//    return {
//        htmlTemplate: formTemplate,
//        checkOptions: validateOptions
//    };
//}]);
//csapp.directive("csCheckboxField", ["$compile", "csCheckBoxFieldFactory", function ($compile, factory) {

//    //options: label, required, checked
//    var linkFunction = function (scope, element) {
//        factory.checkOptions(scope.options);
//        var template = factory.htmlTemplate(scope);
//        element.html(template);
//        $compile(element.contents())(scope);
//    };

//    return {
//        scope: { options: '=', csModel: '@csModel', ngChange: '&csChange', ngClick: '&csClick' },
//        restrict: 'E',
//        link: linkFunction
//    };
//}]);

//csapp.factory("csEnumFieldFactory", ["Logger", function (logManager) {

//    var $log = logManager.getInstance("csEnumField");

//    var formTemplate = function (scope) {
//        var html = '<div ng-form="myform">';

//        html += templateFunction(scope);

//        html += '<div class="field-validation-error" data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty"> ' +
//                    '<div data-ng-show="myform.myfield.$error.required ">{{options.label}} is required</div>' +
//                '</div>';

//        html += '</div>'; //ng-form;

//        return html;
//    };

//    var templateFunction = function (scope) {

//        var string = '<select  class="input-large" ng-required="options.required"  ' +
//                    'data-ng-model="$parent.$parent.' + scope.ngModel + '" name="myfield" ' +
//                    'data-ng-change="ngChange()">' +
//                    ' <option value=""></option> ' +
//                    ' <option data-ng-repeat="row in options.values" value="{{row}}">{{row}}</option>' +
//                '</select> ';

//        return string;
//    };

//    var validateOptions = function (scope) {
//        scope.options.label = scope.options.label || "SelectBox";
//    };

//    return {
//        htmlTemplate: formTemplate,
//        checkOptions: validateOptions
//    };
//}]);

//csapp.directive("csOptions", ["$compile", "$csfactory", function ($compile, $csfactory) {

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

//    function preCompileFn(scope, element, attrs) {

//        var getHtml = function () {
//            return '<div data-ng-show="myform.myfield.$invalid">' +
//            '<div data-ng-show="myform.myfield.$error.required ">{{options.label}} is required</div>' +
//            '<div data-ng-show="myform.myfield.$error.pattern">{{options.patternMessage}} pattern</div>' +
//            '<div data-ng-show="myform.myfield.$error.minlength">{{options.label}} should have atleast {{options.minlength}} character/s.</div>' +
//            '<div data-ng-show="myform.myfield.$error.maxlength">{{options.label}} can have maximum {{options.maxlength}} character/s.</div>' +
//            '<div data-ng-show="myform.myfield.$error.min">{{options.label}} cannot have value less than {{options.min}}</div>' +
//            '<div data-ng-show="myform.myfield.$error.max">{{options.label}} cannot have value greater than {{options.max}}</div>' +
//            '</div>';
//        };

//        var fieldText = attrs['csOptions'];
//        var fieldValue = getPropertyByKeyPath(scope, fieldText);
//        element.attr('name', 'myfield');



//        if (!$csfactory.isNullOrEmptyString(fieldValue.suffix)) {
//            var html = '<div class="input-prepend input-append" cs-input-suffix >' +
//                '<div class="add-on"><i class="icon-envelope"></i></div>' +
//                '<div class="add-on">' + fieldValue.suffix + '</div>' +
//                '</div>';
//        }

//        element.wrap('<ng-form name="myform"></ng-form>');
//        element.wrap(html);
//        element.after(getHtml());
//        element.removeAttr('cs-options');

//        element.attr('cs-validator', fieldText);

//        $compile(element.parent())(scope);
//    }

//    return {
//        restrict: 'A',
//        require: ["ngModel"],
//        compile: function () {
//            return {
//                pre: preCompileFn
//            };
//        }
//    };

//}]);

//csapp.factory("csTextFieldFactory", ["$csfactory", "Logger", function ($csfactory, logManager) {

//    var $log = logManager.getInstance("csTextField");

//    var formTemplate = function (scope) {
//        //var html = '<div ng-form="myform">';

//        //html += templateFunction(scope);

//        var html = '<div class="field-validation-error" data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty"> ' +
//                    '<div data-ng-show="myform.myfield.$error.required ">{{optionsTxt.label}} is required</div>' +
//                    '<div data-ng-show="myform.myfield.$error.pattern">{{options.patternMessage}}</div>' +
//                    '<div data-ng-show="myform.myfield.$error.minlength">{{options.label}} should have atleast {{options.minlength}} character/s.</div>' +
//                    '<div data-ng-show="myform.myfield.$error.maxlength">{{options.label}} can have maximum {{options.maxlength}} character/s.</div>' +
//                '</div>';

//        //html += '</div>'; //ng-form;

//        return html;
//    };

//    var templateFunction = function (scope) {
//        console.log(scope);
//        var html = '<input type="text" name="myfield" placeholder="{{options.placeholder}}" ng-required="options.required" ' +
//            ' ng-pattern="{{options.pattern}}" ng-minlength="{{options.minlength}}" ng-maxlength="{{options.maxlength}}" ';

//        if (!$csfactory.isNullOrEmptyString(scope.csTypeahead))
//            html += 'typeahead="' + scope.csTypeahead + '" ';


//        html += ' ng-readonly="options.readonly" autofocus="options.autofocus" data-ng-change="ngChange()" ' +
//         ' autocomplete="off" data-ng-model="$parent.$parent.' + scope.ngModel + '"/>';

//        return html;
//    };

//    var validateOptions = function (options) {
//        applyTemplates(options);

//        // manage lengths
//        options.minlength = options.length || options.minlength || 0;
//        options.maxlength = options.length || options.maxlength || 250;
//        options.minlength = (options.minlength >= 0 && options.minlength <= 250) ? options.minlength : 0;
//        options.maxlength = (options.maxlength >= 0 && options.maxlength <= 250) ? options.maxlength : 250;
//        if (options.minlength > options.maxlength) {
//            var error = "minlength(" + options.minlength + ") cannot be greather than maxlength(" + options.maxlength + ").";
//            $log.error(error); throw error;
//        }

//        options.label = options.label || "Text";
//        options.patternMessage = options.patternMessage || ("Input is not matching with pattern : " + options.pattern);
//        options.readonly = options.readonly || options.disabled || false;
//    };

//    var setElementAttr = function (element, fieldText) {
//        //if (!element.attr('ng-required'))
//        //    element.attr("ng-required", fieldText + ".required");
//        //if (!element.attr('ng-maxlength'))
//        //    element.attr("ng-maxlength", fieldText + ".maxlength");
//        //if (!element.attr('ng-minlength'))
//        //    element.attr("ng-minlength", fieldText + ".minlength");
//        //if (!element.attr('ng-pattern'))
//        //    element.attr("ng-pattern", fieldText + ".pattern");
//        //if (!element.attr('name'))
//        //    element.attr("name", "myfield");

//        element.removeAttr("cs-options");
//        element.removeAttr("data-cs-options");
//        //element.attr("cs-validator", fieldText);


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

//    return {
//        htmlTemplate: formTemplate,
//        checkOptions: validateOptions,
//        setElementAttr: setElementAttr
//    };
//}]);


//csapp.directive("csValidator", ["$compile", "$csfactory", function ($compile, $csfactory) {

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

//    function linkFn(scope, element, attrs, ngModel) {

//        var performValidation = function (value, restrictions) {

//            ngModel.$setValidity("required", true);
//            ngModel.$setValidity("minlength", true);
//            ngModel.$setValidity("maxlength", true);
//            ngModel.$setValidity("max", true);
//            ngModel.$setValidity("min", true);

//            if ($csfactory.isNullOrEmptyString(value)) {
//                if (restrictions.required === true) {
//                    ngModel.$setValidity("required", false);
//                    return;
//                }
//            }

//            if (!$csfactory.isNullOrEmptyString(restrictions.minlength)) {
//                if ($csfactory.isNullOrEmptyString(value)) {
//                    ngModel.$setValidity("minlength", false);
//                    return;
//                } else {
//                    var validmin = value.length >= restrictions.minlength;
//                    ngModel.$setValidity("minlength", validmin);
//                    if (!validmin) return;
//                }
//            }

//            if (!$csfactory.isNullOrEmptyString(restrictions.maxlength)) {
//                if ($csfactory.isNullOrEmptyString(value)) {
//                    ngModel.$setValidity("minlength", false);
//                    return;
//                } else {
//                    var validmax = value.length <= restrictions.maxlength;
//                    ngModel.$setValidity("maxlength", validmax);
//                    if (!validmax) return;
//                }
//            }

//            if (!$csfactory.isNullOrEmptyString(restrictions.min)) {
//                if ($csfactory.isNullOrEmptyString(value)) {
//                    ngModel.$setValidity("min", false);
//                    return;
//                } else {
//                    var minval = value >= restrictions.min;
//                    ngModel.$setValidity("min", minval);
//                    if (!minval) return;
//                }
//            }

//            if (!$csfactory.isNullOrEmptyString(restrictions.max)) {
//                if ($csfactory.isNullOrEmptyString(value)) {
//                    ngModel.$setValidity("max", false);
//                    return;
//                } else {
//                    var maxVal = value <= restrictions.max;
//                    ngModel.$setValidity("max", maxVal);
//                    if (!maxVal) return;
//                }
//            }
//        };

//        scope.$watch(function () {
//            return ngModel.$viewValue;
//        }, function (newval) {
//            var fieldText = attrs['csValidator'];
//            console.log('fieldText: ', fieldText);
//            var fieldValue = getPropertyByKeyPath(scope, fieldText);
//            console.log('fieldValue: ', fieldValue);
//            performValidation(newval, fieldValue);
//        });

//        scope.$watch(function () {
//            var fieldText = attrs['csValidator'];
//            return getPropertyByKeyPath(scope, fieldText);
//        }, function (newval) {
//            performValidation(ngModel.$viewValue, newval);
//        }, true);
//    }

//    return {
//        restrict: 'A',
//        link: linkFn,
//        require: "ngModel"
//    };

//}]);



//csapp.directive("csEnumField", ["$csfactory", "$compile", function ($csfactory, $compile) {

//    //options:label, required, values 

//    var linkFunction = function (scope, element) {
//        factory.checkOptions(scope.options);
//        var template = factory.htmlTemplate(scope);
//        element.html(template);
//        $compile(element.contents())(scope);
//    };

//    return {
//        scope: { options: '=', ngModel: '@', ngChange: '&' },
//        restrict: 'E',
//        link: linkFunction
//    };
//}]);

//csapp.factory("csSelectFieldFactory", ["Logger", function (logManager) {

//    var $log = logManager.getInstance("csSelectField");

//    var formTemplate = function (scope) {
//        var html = '<div ng-form="myform">';

//        html += templateFunction(scope);

//        html += '<div class="field-validation-error" data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty"> ' +
//                    '<div data-ng-show="myform.myfield.$error.required ">{{options.label}} is required</div>' +
//                '</div>';

//        html += '</div>'; //ng-form;

//        return html;
//    };

//    var templateFunction = function (scope) {

//        var string = '<select  data-ng-model="$parent.$parent.' + scope.ngModel + '" ' +
//                 'name="myfield" ' +
//                 'ng-required="options.required" ' +
//                 'data-ng-change="ngChange()">' +
//                    ' <option value=""></option> ' +
//                    ' <option data-ng-repeat="' + scope.options.csRepeat + '"value="{{' + scope.options.valueField + '}}">{{' + scope.options.textField + '}}</option>' +
//                '</select> ';

//        return string;
//    };

//    var validateOptions = function (scope) {
//        scope.options.label = scope.options.label || "SelectBox";
//        //set params on options
//        scope.options.csRepeat = "row in $parent." + scope.csRepeat.substring(1, scope.csRepeat.length - 1);
//        scope.options.textField = scope.textField ? "row." + scope.textField : "row";
//        scope.options.valueField = scope.valueField ? "row." + scope.valueField : "row";
//        //console.log(scope.options.csRepeat);
//    };

//    return {
//        htmlTemplate: formTemplate,
//        checkOptions: validateOptions
//    };
//}]);
//csapp.directive("csSelectField", ["$compile", "csSelectFieldFactory", function ($compile, factory) {

//    //options:label, required 

//    var linkFunction = function (scope, element) {
//        factory.checkOptions(scope);
//        var template = factory.htmlTemplate(scope);

//        element.html(template);
//        $compile(element.contents())(scope);
//    };

//    return {
//        scope: { options: '=', ngModel: '@', ngChange: '&', csRepeat: '@', textField: '@', valueField: '@' },
//        restrict: 'E',
//        link: linkFunction
//    };
//}]);

//csapp.factory("csRadioFieldFactory", ["Logger", function (logManager) {

//    var $log = logManager.getInstance("csRadioField");

//    var formTemplate = function (scope) {
//        var html = '<div ng-form="myform">';

//        html += templateFunction(scope);

//        html += '<div class="field-validation-error" data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty"> ' +
//                    '<div data-ng-show="myform.myfield.$error.required ">{{options.label}} is required</div>' +
//                '</div>';

//        html += '</div>'; //ng-form;

//        return html;
//    };

//    var templateFunction = function (scope) {

//        var string = '<div class="radio" ng-repeat="(key, record) in options.options">' +
//                    '<label> <input type="radio" name="myfield" value="{{' + scope.options.valueField + '}}" ' +
//                                'data-ng-model="$parent.$parent.' + scope.ngModel + '" ' +
//                                'data-ng-change="ngChange()" ' +
//                                'ng-required="options.required"  />{{' + scope.options.textField + '}}' +
//                    '</label>' +
//                  '</div>';

//        return string;
//    };

//    var validateOptions = function (scope) {
//        scope.options.label = scope.options.label || "Description";
//        scope.options.textField = scope.textField ? "record." + scope.textField : "record";
//        scope.options.valueField = scope.valueField ? "record." + scope.valueField : "record";
//    };

//    return {
//        htmlTemplate: formTemplate,
//        checkOptions: validateOptions
//    };
//}]);
//csapp.directive("csRadioField", ["$csfactory", "$compile", function ($csfactory, $compile) {

//    //$scope.gender = { label: "Gender", required: true, textField: "text2", options: [{ text2: "Male", value: "yes" }, { text2: "Female", value: "no" }] };

//    var linkFunction = function (scope, element) {
//        factory.checkOptions(scope.options);
//        var template = factory.htmlTemplate(scope);
//        element.html(template);
//        $compile(element.contents())(scope);
//    };

//    return {
//        scope: { options: '=', ngModel: '@', ngChange: '&csChange', textField: '@', valueField: '@' },
//        restrict: 'E',
//        link: linkFunction
//    };
//}]);

//csapp.factory("csDateFieldFactory", ["$csfactory", "Logger", function ($csfactory, logManager) {

//    var $log = logManager.getInstance("csDateField");
//    var formTemplate = function (scope) {
//        var html = '<div ng-form="myform">';

//        html += templateFunction(scope);

//        html += '<div class="field-validation-error" data-ng-show="myform.myfield.$invalid && myform.myfield.$dirty"> ' +
//                    '<div data-ng-show="myform.myfield.$error.required ">{{options.label}} is required</div>' +
//                    '<div data-ng-show="myform.myfield.$error.minlength">{{options.label}} should have atleast {{options.minlength}} character/s.</div>' +
//                    '<div data-ng-show="myform.myfield.$error.maxlength">{{options.label}} can have maximum {{options.maxlength}} character/s.</div>' +
//                '</div>';

//        html += '</div>'; //ng-form;

//        return html;
//    };

//    var templateFunction = function (scope) {
//        return '<div class="input-append">' +
//                '<input type="text" name="myfield" class="input-medium" data-ng-readonly="true" ' +
//                    'data-ng-model="$parent.$parent.' + scope.ngModel + '" ' +
//                    ' data-ng-required="options.required" data-date-min-view-mode="' + scope.options.minViewMode + '" ' +
//                    ' data-date-days-of-week-disabled="' + scope.options.daysOfWeekDisabled + '" data-date-format="' + scope.options.format + '" ' +
//                    ' placeholder="{{options.placeholder}}" data-date-start-date="' + scope.options.startDate + '"' +
//                    ' data-date-end-date="' + scope.options.endDate + '" bs-datepicker="" data-ng-change="ngChange()">' +
//                '<button type="button" class="btn" data-toggle="datepicker"><i class="icon-calendar"></i></button> ' +
//            '</div>';
//    };

//    var applyTemplate = function (scope) {
//        if (angular.isUndefined(scope.options.template) || scope.options.template === null) {
//            return;
//        }

//        var tmpl = scope.options.template.split(",").filter(function (str) { return str !== ''; });
//        angular.forEach(tmpl, function (template) {
//            if (template.length < 1) return;
//            switch (template) {
//                case "MonthPicker":
//                    scope.options.minViewMode = "months";
//                    break;
//                case "YearPicker":
//                    scope.options.minViewMode = "years";
//                    break;
//                case "future":
//                    scope.options.startDate = "+0";
//                    break;
//                case "past":
//                    scope.options.endDate = "+0";
//                    break;
//                default:
//                    $log.error(template + " is not defined.");
//            }
//            return;
//        });
//    };

//    var manageViewMode = function (scope) {
//        //month/year modes
//        if ($csfactory.isNullOrEmptyString(scope.options.minViewMode)) {
//            scope.options.minViewMode = 0;
//        } else if (scope.options.minViewMode === "1" || scope.options.minViewMode === "months") {
//            scope.options.minViewMode = 1;
//        } else if (scope.options.minViewMode === "2" || scope.options.minViewMode === "years") {
//            scope.options.minViewMode = 2;
//        } else {
//            scope.options.minViewMode = 0;
//        }

//        //format
//        if (scope.options.minViewMode === 0) {
//            scope.options.format = "dd-M-yyyy";
//        } else if (scope.options.minViewMode === 1) {
//            scope.options.format = "M-yyyy";
//        } else {
//            scope.options.format = ".yyyy";
//        }

//        //min date        
//        if ($csfactory.isNullOrEmptyString(scope.options.startDate)) {
//            if (scope.options.minViewMode === 0) {
//                scope.options.startDate = '01-Jan-1800';
//            } else if (scope.options.minViewMode === 1) {
//                scope.options.startDate = 'Jan-1800';
//            } else {
//                scope.options.startDate = '.1800';
//            }
//        }

//        //max date
//        if ($csfactory.isNullOrEmptyString(scope.options.endDate)) {
//            if (scope.options.minViewMode === 0) {
//                scope.options.endDate = '31-Dec-2400';
//            } else if (scope.options.minViewMode === 1) {
//                scope.options.endDate = 'Dec-2400';
//            } else {
//                scope.options.endDate = '.2400';
//            }

//        }
//    };

//    var validateOptions = function (scope) {
//        applyTemplate(scope);
//        manageViewMode(scope);
//        if ($csfactory.isNullOrEmptyString(scope.options.label)) {
//            scope.options.label = "Date";
//        }

//        if ($csfactory.isNullOrEmptyString(scope.options.daysOfWeekDisabled)) {
//            scope.daysOfWeekDisabled = '[]';
//        }
//    };


//    return {
//        applyTemplate: applyTemplate,
//        manageViewMode: manageViewMode,
//        htmlTemplate: formTemplate,
//        checkOptions: validateOptions
//    };
//}]);
//csapp.directive("csDateField", ["$compile", "Logger", "$csfactory", function ($compile, logger, factory) {

//    var $log = logger.getInstance("csDateField");

//    //options: label, placeholder, required, readonly, end-date, start-date, date-format, date-min-view-mode, days-of-week-disabled

//    var linkFunction = function (scope, element) {
//        factory.checkOptions(scope);
//        var template = factory.htmlTemplate(scope);
//        element.html(template);
//        $compile(element.contents())(scope);

//    };

//    return {
//        scope: { options: '=', ngModel: '@', ngChange: '&' },
//        restrict: 'E',
//        link: linkFunction
//    };
//}]);
