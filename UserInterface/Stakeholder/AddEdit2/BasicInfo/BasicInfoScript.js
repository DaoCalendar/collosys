(
csapp.controller('basicInfoController', ['$scope', '$http', 'Restangular', '$csfactory', '$csnotify', '$log', '$csModels', 
    function ($scope, $http, rest, $csfactory, $csnotify, $log, $csmodel) {
        'use strict';

        var apistake = rest.all('StakeholderApi');

        $scope.userIdVal = function (userId, isUser) {
            if (isUser === false) {
                return true;
            }

            if ($csfactory.isNullOrEmptyString(userId)
                || $csfactory.isNullOrEmptyString(isUser)
                || !angular.isNumber(userId)
                || (userId.length !== 7)) {
                return false;
            }

            $scope.duplicateUserId = true;

            _.forEach($scope.UserIdList, function (item) {
                if (item === userId) {
                    $scope.duplicateUserId = false;
                }
            });
            return $scope.duplicateUserId;


        };

        $scope.setEmailModel = function (hierarchy) {
            if (hierarchy !== 'External')
                $scope.$parent.stakeholderModel.Email.suffix = "'@scb.com'";
        };

        // harish - no need... get true/false from server
        $scope.checkUser = function (userId) {
            if (angular.isUndefined(userId)) {
                return false;
            }
            $scope.$parent.WizardData.userExists = false;
            userId = userId.toString();
            if (userId.length === 7) {
                apistake.customGET('CheckUserId', { id: userId }).then(function (data) {
                    var exist = data;
                    if (exist === "true") {
                        $scope.$parent.WizardData.userExists = true;
                    } else {
                        $scope.$parent.WizardData.userExists = false;
                    }
                });
            }

        };

        var getEmailForEditMode = function (mailId) {

            if ($scope.basicInfoData.Hierarchy.IsUser) {
                var index = mailId.indexOf('@');
                $scope.$parent.WizardData.SetEmailId(mailId.substring(0, index));
            } else {
                $scope.$parent.WizardData.SetEmailId(mailId);
            }
        };


        //$scope.$watch("$parent.WizardData.FinalPostModel.SelHierarchy.Designation", function () {
        //    init();

        //});

        (function () {
            $log.info("Initializing BasicInfo.");
            $scope.duplicateUserId = false;
            $scope.stakeholderModels = $csmodel.getColumns("Stakeholder");
            $scope.UserIdList = [];
            //TODO get basic info data
            console.log('current hierarchy: ', $scope.$parent);

            //if ($scope.$parent.WizardData.IsEditMode() === true) {
            //    getEmailForEditMode($scope.$parent.WizardData.FinalPostModel.EmailId);
            //}

        })();

    }])
);



//apistake.customGET('UserIdVal', { userid: userId }).then(function (data) {
//    $log.info(data);
//    $scope.duplicateUserId = data;
//    if ($scope.duplicateUserId === false) {
//        return true;
//    } else {
//        return false;
//    }
//});
////$scope.duplicateUserId = checkUserId(userId);

//   if ($scope.duplicateUserId === false) {
//       return true;
//   } else {
//       return false;
//   }
//$scope.Next = function (info) {
//    console.log(info);
//    $scope.BasicDetails.push(info);
//};
//var getUserIdList = function () {
//    apistake.customGET('UserIdList').then(function (data) {
//        $log.info(data);
//        $scope.UserIdList = data;
//    }, function (error) {
//        $log.info(error.data.Message);
//    });
//};
