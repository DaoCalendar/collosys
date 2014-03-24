
csapp.factory("loginDataLayer", [
    "Restangular", function (rest) {

    }
]);

csapp.factory("$csAuthFactory", ["$cookieStore", "Logger", "$csfactory",
    function ($cookieStore, logManager, $csfactory) {
    var $log = logManager.getInstance("$csAuthFactory");

    var authInfo = {
        isAuthorized: false,
        username: '',
        loginTime: null
    };

    var loadCookie = function () {
        var cookie = $cookieStore.get("authInfo");
        if (angular.isUndefined(cookie)) return;
        if (cookie.isAuthorized === true && !$csfactory.isNullOrEmptyString(cookie.username)) {
            var time = moment(cookie.loginTime);
            if (time.isValid() && moment().diff(time, 'minutes') <= 30) {
                authInfo = cookie;
                $log.info(authInfo.username + "has logged in from cookie.");
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
        $log.info(authInfo.username + "has logged in from ui.");
    };

    var getUsername = function () {
        return authInfo.username;
    };

    var logoutUser = function () {
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
        loadAuthCookie: loadCookie
    };
}]);

csapp.controller("logoutController", [
    "$csAuthFactory", "$location", function ($csAuthFactory, $location) {
        $csAuthFactory.logoutUser();
        $location.path("/login");
    }
]);

csapp.controller("loginController", ["$scope", "$modalInstance", "$csAuthFactory",
    function ($scope, $modalInstance, $csAuthFactory) {
        $scope.loginErrorMessage = "Invalid username or password.";
        $scope.login = {
            error: false,
            showForgot: false
        };
        $csAuthFactory.logoutUser();

        $scope.loginUser = function (login) {
            if (login.username === login.password) {
                $csAuthFactory.loginUser(login.username);
                $modalInstance.close(login.username);
            } else {
                login.error = true;
            }
        };

        $scope.forgotPassword = function () {
            $scope.login.showForgot = true;
        };
    }]);

csapp.controller('LoginCtrl', ["$scope", "$modal", "$location", "Logger",
    function ($scope, $modal, $location, logManager) {
        var $log = logManager.getInstance("LoginCtrl");
        var modalInst = $modal.open({
            controller: "loginController",
            templateUrl: "/Generic/login/login.html",
            backdrop: false,
            keyboard: false
        });

        modalInst.result.then(function () {
            $log.info("redirecting user to home page");
            $location.path("/home");
        });
    }]);