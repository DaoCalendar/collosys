csapp.controller("HomeCtrl", ['$scope', '$csnotify', '$csfactory', 'Restangular',
    function ($scope, $csnotify, $csfactory, rest) {

        var restApi = rest.all("HomeApi");

        $scope.changeFunc = function (data) {
            $scope.clicked++;
            console.log(data);
            $scope.pqr = data;
        };

        $scope.mask = '999-999';

        $scope.clear = function () {
            $scope.dt = null;
        };

        // Disable weekend selection
        $scope.disabled = function (date, mode) {
            return (mode === 'day' && (date.getDay() === 0 || date.getDay() === 6));
        };

        $scope.toggleMin = function () {
            $scope.minDate = $scope.minDate ? null : new Date();
        };
        $scope.toggleMin();

        $scope.open = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.opened = true;
        };

        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1
        };



        (function () {
            //$scope.myModel = 1;
            $scope.fields = [
               { name: 'DOB', label: 'DOB', minDate: "2014-06-22", editable: false, required: true, type: 'date', min: 10, max: 100 },
               { name: 'Name', editable: true, required: true, type: 'password' },
               { name: 'select', label: 'select', textField: 'display', useRepeat: true, valueField: 'value', editable: false, required: true, type: 'select', min: 10, max: 100 },
               { name: 'enum', label: 'enum', editable: false, required: true, type: 'enum', valueList: $scope.array1, min: 10, max: 100 },
               { type: 'btn-radio', options: ['boom1', 'boom2'], textField: 'display', valueField: 'value' },
               { name: 'Age', label: 'Age', editable: false, required: true, type: 'text', template: 'percentage' },
               { name: 'Mobile', label: 'Mobile', template: 'phone', editable: false, required: true, type: 'text', min: 10, max: 100 },
               { name: 'Radio', label: 'Radio', editable: false, required: true, type: 'int' }
            ];

            $scope.initDate = new Date('2016-15-20');
            $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
            $scope.format = $scope.formats[0];
            $scope.cars = [{ id: 1, name: 'Audi' }, { id: 2, name: 'BMW' }, { id: 1, name: 'Honda' }];
            $scope.array1 = [1, 2, 3, 4, 5];
            $scope.array = [{ display: 1, value: 1 }, { display: 2, value: 2 }, { display: 3, value: 3 }];

            $scope.clicked = 0;
            $scope.abc = {};
            $scope.abce = { data: 1 };

            $scope.dataChanged = function () {
                $scope.abce.data1 = 2;
            };

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
