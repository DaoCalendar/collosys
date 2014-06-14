
csapp.factory("ChangePasswordDataLayer", ["$csnotify", 'Restangular', "$csAuthFactory", "$csfactory",
    function ($csnotify, rest, auth, $csfactory) {
        var apictrl = rest.all('ChangePasswordApi');
        var dldata = {};

        dldata.UserName = auth.getUsername();

        var savepassword = function (changepassword) {
            return apictrl.customPOST(changepassword, 'changedpassword').then(function (data) {
                dldata.PasswordChanged = data;
                if (data == "null") {
                    $csnotify.error("User does not exist or Password do not Match");
                } else {
                    $csnotify.success("Password Changed Successfully");
                }
                return data;
            }, function (data) {
                $csnotify.error(data.message);
            });
        };

        return {
            dldata: dldata,
            SavePassword: savepassword,
        };
    }
]);

csapp.controller("changepasswordCtrl", ["$scope", "ChangePasswordDataLayer",
    function ($scope, datalayer) {
        $scope.dldata = datalayer.dldata;
        (function () {
            $scope.UserName = datalayer.dldata.UserName;
            $scope.datalayer = datalayer;
        })();


        $scope.SavePassword = function (changepassword) {
            datalayer.SavePassword(changepassword).then(function() {
                $scope.changepassword = {};
            });
        };

    }
]);
//#endregion