csapp.controller("HomeCtrl", ['$scope', '$csnotify', '$csfactory', 'Restangular',
    function ($scope, $csnotify, $csfactory, rest) {

        var restApi = rest.all("HomeApi");

        $scope.changeFunc = function (data) {
            console.log(data);
            $scope.pqr = data;
        };



        (function () {
            $scope.abc = {};
            $scope.array = [{display: 1,value:1 }];

            $scope.fields = [
                 { name: 'Name', label: 'Name', editable: true, required: true, type: 'text', pattern: '/^[a-zA-Z]{0,15}$/', patternMessage: "pattern" },
                 { name: 'Age', label: 'Age', editable: false, required: true, type: 'int', min: 10, max: 100 },
                 { name: 'DOB', label: 'DOB', editable: false, required: true, type: 'int', min: 10, max: 100 },
                 { name: 'DOB', label: 'DOB', required: true, type: 'date'},
                 { name: 'enum', label: 'enum', editable: false, csRepeat: 'array', required: true, type: 'enum', min: 10, max: 100 },
                 { name: 'select',textField:'display',valueField:'value', label: 'select', csRepeat: 'array', editable: false, required: true, type: 'select', min: 10, max: 100 },
                 { name: 'Mobile', label: 'Mobile', template: 'phone', editable: false, required: true, type: 'text', min: 10, max: 100 },
            ];

            $scope.stakeholder = {};
            $scope.input = {
                changeCount: 0
            };

            $scope.mode = "add";
            restApi.customGET("GetData", { 'currentUser': $csfactory.getCurrentUserName() }).then(function (data) {
                $scope.datalist = data;
                console.log($scope.datalist);
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
            if ($scope.datalist.stakeholder != 0 || $scope.datalist.payment != 0 || $scope.datalist.working != 0) //|| $scope.datalist.allocation != 0 || $scope.datalist.allocationpolicy != 0)
                return true;
            else return false;
        };

    }

]);
