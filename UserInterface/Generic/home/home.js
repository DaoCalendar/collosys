csapp.controller("HomeCtrl", ['$scope', '$csnotify', '$csfactory', 'Restangular',
    function ($scope, $csnotify, $csfactory, rest) {

        var restApi = rest.all("HomeApi");

        $scope.changeFunc = function (data) {

            $scope.changed = data;
        };

        (function () {
            $scope.array = [{ text: 1, bind: 1 }, { text: 2, bind: 2 }, { text: 3, bind: 3 }, { text: 4, bind: 4 }, { text: 5, bind: 5 }];
            $scope.Field = {};
            $scope.number = { label: 'Age', required: true, type: 'uint' };
            $scope.text = { label: 'User', required: true, template: 'user', type: 'text', minlength: 5 };
            $scope.phone = { label: 'phone', template: 'phone', required: true, type: 'text', minlength: 5 };
            $scope.textarea = { label: 'Textarea', required: true, resize: false, type: 'textarea', minlength: 5, maxlength: 10 };
            $scope.email = { label: 'Email', type: "email", suffix: '@soham.com', required: true };
            $scope.checkbox = { label: 'Checkbox', type: "checkbox", required: false };
            $scope.radio = { label: 'Radio', type: "radio", required: false, options: 'array', textField: 'text' };


            restApi.customGET("GetData", { 'currentUser': $csfactory.getCurrentUserName() }).then(function (data) {
                $scope.datalist = data;
            }, function (data) {
                $csnotify.error(data.data.Message);
            });

        })();


        $scope.showPendingOptions = function () {
            if (angular.isUndefined($scope.datalist) || $scope.datalist.stakeholder != 0 || $scope.datalist.payment != 0 || $scope.datalist.working != 0 || $scope.datalist.allocation != 0 || $scope.datalist.allocationpolicy != 0)
                return true;
            else return false;
        };

    }

]);
