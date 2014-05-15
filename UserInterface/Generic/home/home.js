csapp.controller("HomeCtrl", ['$scope', '$csnotify', '$csfactory', 'Restangular',
    function ($scope, $csnotify, $csfactory, rest) {

        var restApi = rest.all("HomeApi");

        $scope.changeFunc = function (data) {
            $scope.clicked++;
            console.log(data);
            $scope.pqr = data;
        };

        (function () {
            //$scope.myModel = 1;

            var availableTags = [
                { text: 'Apple', id: 1 },
                { text: 'Apricot', id: 2 },
                { text: 'Avocado', id: 3 }
            ];
            

            $scope.select2Options = {
                tags: availableTags,
                multiple: true,
                formatResult: function(item) {
                    return item.text;
                },
                formatSelection: function(item) {
                    return item.id;
                },
            };
            $scope.clicked = 0;
            $scope.abc = {};
            $scope.abce = { data: 1};
            $scope.array = [{ display: 1, value: 1 }, { display: 2, value: 2 }, { display: 3, value: 3 }];
            $scope.array1 = [1, 2, 3, 4, 5];

            $scope.dataChanged = function() {
                $scope.abce.data1 = 2;
            };

            $scope.fields = [
                 { name: 'select', label: 'select', textField: 'display',useRepeat:true, valueField: 'value', editable: false, required: true, type: 'select', min: 10, max: 100 },
                 { name: 'enum', label: 'enum', editable: false, required: true, type: 'enum', valueList: $scope.array1, min: 10, max: 100 },
                 { type: 'btn-radio', options: ['boom1', 'boom2'], textField: 'display', valueField: 'value' },
                 { name: 'Name', label: 'Name', editable: true, required: true, type: 'text', pattern: '/^[a-zA-Z]{0,15}$/', patternMessage: "pattern" },
                 { name: 'Age', label: 'Age', editable: false, required: true, type: 'text', template: 'percentage' },
                 { name: 'DOB', label: 'DOB', editable: false, required: true, type: 'int', min: 10, max: 100 },
                 { name: 'DOB', label: 'DOB', template: "MonthPicker", required: true, type: 'date' },

                 { name: 'Mobile', label: 'Mobile', template: 'phone', editable: false, required: true, type: 'text', min: 10, max: 100 },
                 { name: 'Radio', label: 'Radio', editable: false, required: true, type: 'int' }
            ];

            $scope.stakeholder = {};
            $scope.input = {
                changeCount: 0
            };

            restApi.customGET("GetData", { 'currentUser': $csfactory.getCurrentUserName() }).then(function (data) {
                $scope.datalist = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });

        })();

        $scope.changed = function (name) {
            $scope.input.change = name;
            $scope.input.changeCount++;
            $scope.mode = "view";
        };
        $scope.showPendingOptions = function () {
            if (angular.isUndefined($scope.datalist)) return false;
            if ($scope.datalist.stakeholders != 0 || $scope.datalist.allocation != 0 || $scope.datalist.billing != 0)
                return true;
            else return false;
        };

    }

]);
