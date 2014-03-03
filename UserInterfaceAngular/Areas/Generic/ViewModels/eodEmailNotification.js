csapp.controller("eodController",['$scope', '$csnotify','Restangular', function($scope,$csnotify ,rest) {
    'use strict';
    var apictrl = rest.all('EmailApi');

    $scope.sendEmailNotification = function() {
        var status = true;
        apictrl.customPOST(status,'Post').then(function (data) {
            if(data){
                $csnotify.success('End Of the Day Email Notification sent.');
            }
        },function(response) {
            $csnotify.error('Error:-'+response.Message);
        });
    };

}]);