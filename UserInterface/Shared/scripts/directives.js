
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

csapp.directive('csButton', ['$parse', '$compile', 'PermissionFactory', 'Logger',
    function ($parse, $compile, permFactory, logManager) {

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
            var permission = attrs.permission;
            if (angular.isUndefined(permission)) permission = "all";

            var html = '<span data-ng-show="' + permFactory.HasPermission(permission) + '">';
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

        var linkFunction = function (scope, element, attrs) {
            var buttonType = attrs.type;
            var templateParams = getTemplateParams(buttonType, attrs.text);
            var template = generateTemplate(templateParams, attrs);

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
        template += '<div class="panel panel-default" style="height: 400px;overflow: auto">';
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
        scope: { heading: '@', valueList: '=', ngModel: '=', onClick: '&', selectedIndex: '=' }, //textField
        template: templateFn,
        link: linkFn,
        require: 'ngModel'
    };
});

csapp.directive('csDualList', function () {
    var templateFunction = function (element, attrs) {

        var lhsTemplate = '<div class="col-md-5">';
        lhsTemplate += '<div class="panel panel-default">';
        lhsTemplate += '<div class="panel-heading">{{config.lhsHeading}}</div>';
        lhsTemplate += '<ul class="list-group" style="height: 250px">';
        lhsTemplate += '<li class="list-group-item" ng-repeat="row in config.lhsValueList"';
        lhsTemplate += ' ng-click="clicked.left(row)' + (angular.isDefined(attrs.onClick) ? ';onClick()' : ' ') + '"';
        lhsTemplate += ' ng-class="{\'activ\' : isSelected($index,\'lhs\') }"';
        lhsTemplate += ' value="row">{{getDisplayName.left(row, config.lhsTextField)}}</li>';
        lhsTemplate += '</ul>';
        lhsTemplate += '</div>';
        lhsTemplate += '</div>';

        var rhsTemplate = '<div class="col-md-5">';
        rhsTemplate += '<div class="panel panel-default" style="height: 100%, overflow: auto">';
        rhsTemplate += '<div class="panel-heading">{{config.rhsHeading}}</div>';
        rhsTemplate += '<ul class="list-group" style="height: 250px">';
        rhsTemplate += '<li class="list-group-item" ng-repeat="row in config.rhsValueList"';
        rhsTemplate += ' ng-click="clicked.right(row)' + (angular.isDefined(attrs.onClick) ? ';onClick()' : ' ') + '"';
        rhsTemplate += ' ng-class="{\'activ\' : isSelected($index, \'rhs\') }"';
        rhsTemplate += ' value="row">{{getDisplayName.right(row, config.rhsTextField)}}</li>';
        rhsTemplate += '</ul>';
        rhsTemplate += '</div>';
        rhsTemplate += '</div>';

        var rightLeftButtonTemplate = '<div ng-show="config.showRightLeftButtons" class="col-md-1">';
        rightLeftButtonTemplate += '<button class="btn btn-success" ng-click="move.left()" '
            + (angular.isDefined(attrs.onMove) ? ';onMove()' : ' ')
            + 'ng-disabled="!direction.left"><i class="glyphicon glyphicon-arrow-left"></i></button>';
        rightLeftButtonTemplate += '<button class="btn btn-success" ng-click="move.right()" '
            + (angular.isDefined(attrs.onMove) ? ';onMove()' : ' ')
            + 'ng-disabled="!direction.right"><i class="glyphicon glyphicon-arrow-right"></i></button>';
        rightLeftButtonTemplate += '</div>';

        var upDownButtonTemplate = '<div ng-show="config.showUpDownButtons" class="col-md-1">';
        upDownButtonTemplate += '<button class="btn btn-success" ng-click="move.up()'
            + (angular.isDefined(attrs.onMove) ? ';onMove()' : ' ')
            + '" ng-disabled="!direction.up"><i class="glyphicon glyphicon-arrow-up"></i></button>';
        upDownButtonTemplate += '<button class="btn btn-success" ng-click="move.down()'
            + (angular.isDefined(attrs.onMove) ? ';onMove()' : ' ')
            + '" ng-disabled="!direction.down"><i class="glyphicon glyphicon-arrow-down"></i></button>';
        upDownButtonTemplate += '</div>';

        var html = '<div class="row dual-list">';
        html += lhsTemplate;
        html += rightLeftButtonTemplate;
        html += rhsTemplate;
        html += upDownButtonTemplate;
        html += '</div>';
        return html;
    };

    var linkFunction = function (scope) {
        scope.direction = { left: false, right: false, up: false, down: false };
        scope.config.showRightLeftButtons = angular.isDefined(scope.config.showRightLeftButtons)
            ? scope.config.showRightLeftButtons === true : true;
        scope.config.showUpDownButtons = angular.isDefined(scope.config.showUpDownButtons)
            ? scope.config.showUpDownButtons === true : true;

        scope.isSelected = function (index, dir) {
            return ((dir === scope.params.selectedSide) && (index === scope.params.selectedItemIndex));
        };

        scope.manageDirections = function () {
            scope.direction = { left: false, right: false, up: false, down: false };
            scope.direction.left = scope.params.selectedSide === "rhs";
            scope.direction.right = scope.params.selectedSide === "lhs";
            scope.direction.up = scope.direction.left && scope.params.selectedItemIndex !== 0;
            scope.direction.down = scope.direction.left && scope.config.rhsValueList.length !== scope.params.selectedItemIndex + 1;
        };

        scope.getDisplayName = {
            left: function (row, field) { return row[field]; },
            right: function (row, field) { return row[field]; }
        };

        scope.clicked = {
            left: function (selected) {
                scope.params.selectedItem = selected;
                scope.params.selectedItemIndex = scope.config.lhsValueList.indexOf(selected);
                scope.params.selectedSide = "lhs";
                scope.params.moveDir = undefined;
                scope.manageDirections();
            },
            right: function (selected) {
                scope.params.selectedItem = selected;
                scope.params.selectedItemIndex = scope.config.rhsValueList.indexOf(selected);
                scope.params.selectedSide = "rhs";
                scope.params.moveDir = undefined;
                scope.manageDirections();
            },
        };

        scope.move = {
            left: function () {
                var rhsIndex = scope.config.rhsValueList.indexOf(scope.params.selectedItem);
                scope.config.rhsValueList.splice(rhsIndex, 1);
                scope.config.lhsValueList.push(scope.params.selectedItem);
                scope.clicked.left(scope.params.selectedItem);
                scope.params.moveDir = "left";
            },
            right: function () {
                var lhsIndex = scope.config.lhsValueList.indexOf(scope.params.selectedItem);
                scope.config.lhsValueList.splice(lhsIndex, 1);
                scope.config.rhsValueList.push(scope.params.selectedItem);
                scope.clicked.right(scope.params.selectedItem);
                scope.params.moveDir = "right";
            },
            up: function () {
                var index = scope.config.rhsValueList.indexOf(scope.params.selectedItem);
                var temp = scope.config.rhsValueList[index];
                scope.config.rhsValueList[index] = scope.config.rhsValueList[index - 1];
                scope.config.rhsValueList[index - 1] = temp;
                scope.clicked.right(temp);
                scope.params.moveDir = "up";
            },
            down: function () {
                var index = scope.config.rhsValueList.indexOf(scope.params.selectedItem);
                var temp = scope.config.rhsValueList[index];
                scope.config.rhsValueList[index] = scope.config.rhsValueList[index + 1];
                scope.config.rhsValueList[index + 1] = temp;
                scope.clicked.right(temp);
                scope.params.moveDir = "down";
            }
        };
    };

    return {
        restrict: 'E',
        scope: { config: '=', params: '=selected', onClick: '&', onMove: '&' },
        template: templateFunction,
        link: linkFunction
    };
});
