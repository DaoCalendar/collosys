csapp.controller("GNotificationsCtrl", ["$scope", "$csModels", "GnotificationDatalayer",
    function ($scope, $csModels, datalayer) {

        $scope.save = function (data, $index) {
            datalayer.Save(data).then(function (data2) {
                data.isEdit = false;
                //var index = _.findIndex(data, { Id: Id });
                $scope.notificationsList.splice($index, 1);
                $scope.notificationsList.push(data2);
            });
        };

        var getData = function () {
            datalayer.GetData().then(function (data) {
                $scope.notificationsList = data;
            });
        };

        $scope.toggleEdit = function(data) {
            data.isEdit = !data.isEdit;
        };

        (function () {
            getData();
            $scope.Gnotify = $csModels.getColumns("GNotification");
        })();
    }]);


csapp.factory("GnotificationDatalayer", ["Restangular", "$csnotify", function (restApi, $csnotify) {

    var apictrl = restApi.all('GNotificationsApi');

    var getdata = function () {
        return apictrl.customGET('Get').then(function(data2) {
            return data2;
        }, function(response) {
            $csnotify.error(response.Message);
        });
    };

    var save = function (data) {
        return apictrl.customPOST(data, 'Post')
            .then(function (data2) {
                return data2;
            }, function (response) {
                $csnotify.error(response.Message);
            });
    };

    return {
        GetData: getdata,
        Save: save,
    };
}]);