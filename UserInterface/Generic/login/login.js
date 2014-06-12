csapp.factory("loginDataLayer", ["Restangular", function (rest) {
    var restApi = rest.all("AutheticationApi");

    var authenticate = function (loginInfo) {
        return restApi.customPOST(loginInfo, "AutheticateUser");
    };

    var validate = function (forgotInfo) {
        return restApi.customPOST(forgotInfo, "CheckUser");
    };

    var resetPassword = function (forgotInfo) {
        return restApi.customPOST(forgotInfo, "ResetPassword");
    };

    return {
        authenticate: authenticate,
        doesUserExist: validate,
        resetPassword: resetPassword,
    };
}
]);

csapp.factory("$csAuthFactory", ["$cookieStore", "Logger",
    function ($cookieStore, logManager) {

        var $log = logManager.getInstance("$csAuthFactory");

        var authInfo = {
            isAuthorized: false,
            username: '',
            loginTime: null
        };

        var loadCookie = function () {
            var cookie = $cookieStore.get("authInfo");
            if (angular.isUndefined(cookie)) return;
            if (cookie.isAuthorized === true) {
                var time = moment(cookie.loginTime);
                if (time.isValid() && moment().diff(time, 'minutes') <= 30) {
                    authInfo = cookie;
                    $log.info(authInfo.username + " has logged in from cookie.");
                } else {
                    $log.info("cookie expired!!!");
                }
            }
        };

        var hasLoggedIn = function () {
            return authInfo.isAuthorized;
        };

        var loginUser = function (user) {
            authInfo.isAuthorized = true;
            authInfo.username = user;
            authInfo.loginTime = moment().format();
            $cookieStore.remove("authInfo");
            $cookieStore.put("authInfo", authInfo);
            $log.info(authInfo.username + " has logged in from ui.");
        };

        var getUsername = function () {
            return authInfo.username;
        };

        var logoutUser = function () {
            if (angular.isDefined(authInfo.username) && authInfo.username !== '' && authInfo.username !== null)
                $log.info(authInfo.username + " has logged out.");
            authInfo.isAuthorized = false;
            authInfo.username = undefined;
            $cookieStore.remove("authInfo");
            $cookieStore.put("authInfo", authInfo);
        };

        return {
            hasLoggedIn: hasLoggedIn,
            loginUser: loginUser,
            logoutUser: logoutUser,
            getUsername: getUsername,
            loadAuthCookie: loadCookie,
            authInfo: authInfo,
        };
    }]);

csapp.controller("logoutController", [
    "$csAuthFactory", "$location", function ($csAuthFactory, $location) {
        $csAuthFactory.logoutUser();
        $location.path("/login");
    }
]);

csapp.controller("loginController",
    ["$scope", "$csAuthFactory", "loginDataLayer", "$location", "$csfactory", "$csnotify",
    function ($scope, $csAuthFactory, datalayer, $location, $csfactory, $csnotify) {

        if ($csAuthFactory.hasLoggedIn()) {
            $location.path("/home");
            return;
        }

        $scope.login = { error: false };
        $scope.forgot = { isUserValid: false };

        $csAuthFactory.logoutUser();
        $scope.hasLoggedIn = false;

        $scope.loginUser = function () {
            $('input').checkAndTriggerAutoFillEvent();

            $scope.login.error = false;
            //$csfactory.enableSpinner();

            datalayer.authenticate($scope.login).then(function (data) {
                if (data === "true") {
                    $scope.hasLoggedIn = true;
                    $csAuthFactory.loginUser($scope.login.username);
                    $location.path("/home");
                } else {
                    $scope.login.error = true;
                    $scope.loginErrorMessage = "Invalid username or password.";
                }
            }, function () {
                $scope.login.error = true;
                $scope.loginErrorMessage = "Server unavailable.";
            });
        };

        $scope.toggleForgotPassword = function () {
            $scope.hasForgotPassword = !$scope.hasForgotPassword;
        };

        $scope.validateUser = function () {
            if ($csfactory.isNullOrEmptyString($scope.forgot.username)) {
                return;
            }
            $scope.forgot.isUserValid = false;
            datalayer.doesUserExist($scope.forgot).then(function (data) {
                if (data === "true") {
                    $scope.forgot.isUserValid = true;
                }
            });
        };

        $scope.resetPassword = function () {
            $scope.forgot.email = null;
            $scope.forgot.passwordResetError = false;
            datalayer.resetPassword($scope.forgot).then(function (data) {
                $scope.forgot.email = data.email;
                $scope.forgot.password = data.password;
                $csnotify.success("Emailed new password.");
                $location.path("/logout");
            }, function () {
                $scope.forgot.passwordResetError = true;
            });
        };
    }]);

//csapp.controller('LoginCtrl', ["$scope", "$modal", "$location", "Logger", "routeManagerFactory",
//    function ($scope, $modal, $location, logManager, routeManagerFactory) {
//        var $log = logManager.getInstance("LoginCtrl");
//        var modalInst = $modal.open({
//            controller: "loginController",
//            templateUrl: "/Generic/login/login.html",
//            backdrop: false,
//            keyboard: false
//        });

//        modalInst.result.then(function () {
//            $log.info("redirecting user to home page");
//            $location.path("/home");
//            routeManagerFactory.getLastLocation();
//        });
//    }]);