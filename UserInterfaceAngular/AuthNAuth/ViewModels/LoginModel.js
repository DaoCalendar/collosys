
csapp.controller("loginController", ["$scope", "$http", function ($scope, $http) {
    'use strict';

  // var loginUrl = "/Account/";

    //$scope.user = {
    //    UserName:"",
    //    Password:""
    //};

    //$scope.forgotPassword = false;

    //$scope.forgotPasswordModel = {
    //    UserName:"",
    //    JoiningDate:""
    //};


    //$scope.localPasswordModel = {
    //    OldPassword:"",
    //    NewPassword:"",
    //    ConfirmPassword:""
    //};


    //#region DB operations
    //http({
    //    url: loginUrl + "Login",
    //    type: "Get",
    //}).success(function(data) {
    //    if (data.IsAuthenticated) {
    //        window.location.href = data.url;
    //    }
    //}).error(function() {
    //    Error("please check username or password");
    //});

    //#endregion

}]);

csapp.controller("forgotPasswordController", ["$scope", "$locale", function ($scope, $locale) {
    'use strict';
}]);

csapp.controller("changePasswordController", ["$scope", function ($scope) {
    'use strict';
}]);
