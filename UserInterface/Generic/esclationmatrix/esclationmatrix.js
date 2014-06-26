csapp.controller("EsclationMatrixCtrl", ["$scope", "$csModels", "EsclationMatrixDatalayer",
    function ($scope, $csModels, datalayer) {

        $scope.save = function (data, $index) {
            datalayer.Save(data).then(function (data2) {
                data.isEdit = false;
                //var index = _.findIndex(data, { Id: Id });
                $scope.eMatrixList.splice($index, 1);
                $scope.eMatrixList.push(data2);
            });
        };

        var getData = function () {
            datalayer.GetData().then(function (data) {
                $scope.eMatrixList = data;
            });
        };

        $scope.toggleEdit = function(data) {
            data.isEdit = !data.isEdit;
        };

        (function () {
            getData();
            $scope.eMatrix = $csModels.getColumns("EsclationMatrix");
        })();
    }]);


csapp.factory("EsclationMatrixDatalayer", ["Restangular", "$csnotify", function (restApi, $csnotify) {

    var apictrl = restApi.all('EsclationMatrixApi');

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