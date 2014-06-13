csapp.controller("HomeCtrl", ['$scope', '$csnotify', '$csfactory', 'Restangular',
    function ($scope, $csnotify, $csfactory, rest) {

        var restApi = rest.all("HomeApi");

        $scope.notBlackListed = function (value) {
            var blacklist = ['bad@domain.com', 'verybad@domain.com'];
            return blacklist.indexOf(value) === -1;
        };


        var init = function () {
            $scope.data = {};
            $scope.fields = [
                { label: 'Upload Mode', type: 'radio', options: [{ value: 'true', key: 'Immediate' }, { value: 'false', key: 'Nightly' }], valueField: 'value', textField: 'key', required: true },
                { name: 'DOB', label: 'DOB', endDate: "1y", defaultDate: "+10d", editable: false, required: true, type: 'date', min: 10, max: 100 },
                { name: 'Name', editable: true, required: true, type: 'password' },
                { name: 'select', label: 'select', textField: 'display', useRepeat: true, valueField: 'value', editable: false, required: true, type: 'select', min: 10, max: 100 },
                { name: 'enum', label: 'enum', editable: false, required: true, type: 'enum', valueList: $scope.array1, min: 10, max: 100 },
                { type: 'btn-radio', options: ['boom1', 'boom2'], textField: 'display', valueField: 'value' },
                { name: 'Age', label: 'Age', editable: false, required: true, type: 'text', template: 'percentage' },
                { name: 'Mobile', label: 'Mobile', template: 'phone', editable: false, required: true, type: 'text', min: 10, max: 100 },
                { name: 'Radio', label: 'Radio', editable: false, required: true, type: 'int' }
            ];

            restApi.customGET("GetData", { 'currentUser': $csfactory.getCurrentUserName() }).then(function (data) {
                $scope.datalist = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });

        };
        init();

        $scope.showPendingOptions = function () {
            if (angular.isUndefined($scope.datalist)) return false;
            if ($scope.datalist.stakeholders != 0 || $scope.datalist.allocation != 0 || $scope.datalist.billing != 0)
                return true;
            else return false;
        };

    }

]);
