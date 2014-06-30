(
    csapp.controller("DbGenerationController", ["$scope", "Restangular", "$csnotify", "$location",
        "modalService","$csfactory","$timeout","loadingWidget",
        function ($scope, rest, $csnotify, $location, modalService, $csfactory, $timeout, loadingWidget) {

        var restApi = rest.all('DbGenerationApi');


        $scope.generateDB = function () {

                var modalOptions = {
                    actionButtonText: 'Yes',
                    closeButtonText: 'Cancel',
                    headerText:'Proceed' ,
                    bodyText: 'Are you sure you want to Generate DB?'
                };

                modalService.showModal({}, modalOptions).then(function () {
                    $csfactory.enableSpinner();
                    restApi.customGET("CreateDatabase").then(function () {
                        $csnotify.success("DB Generated Successfully");
                        $location.path('/account/logoff');
                    });
                });
            };
       

        //#region web.config

        var init = function () {
            restApi.customGET('GetSectionsNames').then(function (data) {
                $scope.sections = data;
            });
        };
        init();

        $scope.encrypt = function () {
            restApi.customPOST('EncryptData');
        };

        $scope.decrypt = function () {
            restApi.customPOST('EncryptData');
        };

        $scope.encryptSection = function () {
            restApi.customGET('EncryptSection', { sectionName: $scope.sectionName }).then(function () {
                $csnotify.success('Section Encrypted');
            });
        };

        $scope.decryptSection = function () {
            restApi.customGET('DecryptSection', { sectionName: $scope.sectionName }).then(function () {
                $csnotify.success('Section Decrypted');
            });
        };
        //#endregion
    }])
);


//$scope.sectionName = '';
//$scope.sections = [];

